using System;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace lilToon.PCSS.Editor
{
    public static class PCSSNDMFAAOFixUtility
    {
        private const string HipLightGroupName = "PCSS Hip Lights (MA)";
        private const string AutoLightGroupName = "PCSS External Lights (Auto)";
        private const string LightControlsName = "PCSS Light Controls (MA)";
        private const string HipAnchorName = "HipAnchor (MA Bone Proxy)";
        private const string AutoRepairSessionKey = "lilToon.PCSS.NDMFAAOFixUtility.AutoRepairOpenScenes.v2";

        [InitializeOnLoadMethod]
        private static void AutoRepairOpenScenesAfterReload()
        {
            if (SessionState.GetBool(AutoRepairSessionKey, false)) return;
            SessionState.SetBool(AutoRepairSessionKey, true);
            EditorApplication.delayCall += AutoRepairOpenScenes;
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/Repair/NDMF警告修復（AAO併用）", false, 90)]
        public static void RepairSelectedAvatar()
        {
            GameObject avatarRoot = ResolveAvatarRoot(Selection.activeGameObject);
            if (avatarRoot == null)
            {
                EditorUtility.DisplayDialog("PCSS", "修復するアバターのルートを選択してください。", "OK");
                return;
            }

            if (!PCSSAvatarDescriptorGuard.TryEnsureDescriptor(avatarRoot, useUndo: true, out PCSSAvatarDescriptorGuard.RepairReport descriptorReport))
            {
                EditorUtility.DisplayDialog("PCSS", descriptorReport.ToDialogString(), "OK");
                return;
            }

            avatarRoot = descriptorReport.AvatarRoot ?? avatarRoot;

            Type boneProxyType = FindType("nadena.dev.modular_avatar.core.ModularAvatarBoneProxy");
            Type menuInstallerType = FindType("nadena.dev.modular_avatar.core.ModularAvatarMenuInstaller");
            if (boneProxyType == null && menuInstallerType == null)
            {
                EditorUtility.DisplayDialog("PCSS", "Modular Avatarの型が見つかりません。Modular Avatarの導入状態を確認してください。", "OK");
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo(avatarRoot, "Repair PCSS NDMF Warnings");
            RepairSummary summary = RepairAvatar(avatarRoot, boneProxyType, menuInstallerType);
            EditorSceneManager.MarkSceneDirty(avatarRoot.scene);

            StringBuilder message = new StringBuilder();
            message.AppendLine($"{avatarRoot.name} のPCSS/MA設定を確認しました。");
            message.AppendLine();
            message.AppendLine($"Bone Proxy修復: {summary.BoneProxyFixed}");
            message.AppendLine($"Menu Installer修復: {summary.MenuInstallerFixed}");
            message.AppendLine($"空Menu Installer削除: {summary.EmptyInstallerRemoved}");
            if (summary.BoneProxySkipped > 0)
            {
                message.AppendLine($"Hips未検出で保留: {summary.BoneProxySkipped}");
            }
            message.AppendLine();
            message.AppendLine("AAOの最適化メトリクス表示は正常です。NDMF Consoleを更新し、MA-1100 / MA-1200が消えたか確認してください。");

            EditorUtility.DisplayDialog("PCSS", message.ToString(), "OK");
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/Repair/NDMF警告修復（AAO併用）", true)]
        public static bool ValidateRepairSelectedAvatar()
        {
            return Selection.activeGameObject != null;
        }

        internal static RepairSummary RepairAvatar(GameObject avatarRoot, Type boneProxyType, Type menuInstallerType)
        {
            RepairSummary summary = new RepairSummary();
            if (avatarRoot == null) return summary;

            Animator animator = avatarRoot.GetComponent<Animator>();
            Transform fallbackHips = IsUsableHumanoidAnimator(animator) ? animator.GetBoneTransform(HumanBodyBones.Hips) : null;

            RepairHierarchy(avatarRoot.transform, fallbackHips, boneProxyType, menuInstallerType, ref summary);

            Transform sceneRoot = avatarRoot.transform.root;
            if (sceneRoot != null && sceneRoot != avatarRoot.transform)
            {
                RepairHierarchy(sceneRoot, fallbackHips, boneProxyType, menuInstallerType, ref summary);
            }

            return summary;
        }

        internal static RepairSummary RepairAvatar(GameObject avatarRoot)
        {
            Type boneProxyType = FindType("nadena.dev.modular_avatar.core.ModularAvatarBoneProxy");
            Type menuInstallerType = FindType("nadena.dev.modular_avatar.core.ModularAvatarMenuInstaller");
            return RepairAvatar(avatarRoot, boneProxyType, menuInstallerType);
        }

        private static void RepairHierarchy(Transform searchRoot, Transform fallbackHips, Type boneProxyType, Type menuInstallerType, ref RepairSummary summary)
        {
            if (searchRoot == null) return;

            if (boneProxyType != null)
            {
                RepairBoneProxiesInHierarchy(searchRoot, fallbackHips, boneProxyType, ref summary);
            }

            if (menuInstallerType != null)
            {
                RepairMenuInstallersInHierarchy(searchRoot, menuInstallerType, ref summary);
            }
        }

        private static void RepairBoneProxiesInHierarchy(Transform searchRoot, Transform fallbackHips, Type boneProxyType, ref RepairSummary summary)
        {
            Component[] proxies = searchRoot.GetComponentsInChildren(boneProxyType, true);
            foreach (Component proxy in proxies)
            {
                if (proxy == null || !IsPCSSGeneratedTransform(proxy.transform)) continue;
                if (TryGetReflected(proxy, "target") != null) continue;

                Transform hips = ResolveHipsFor(proxy.transform) ?? fallbackHips;
                if (hips == null)
                {
                    summary.BoneProxySkipped++;
                    continue;
                }

                Undo.RecordObject(proxy, "Repair PCSS Bone Proxy");
                bool targetAssigned = TrySetReflected(proxy, "target", hips);
                bool boneAssigned = TrySetReflected(proxy, "boneReference", HumanBodyBones.Hips) ||
                                    TrySetReflected(proxy, "bone", HumanBodyBones.Hips);
                EditorUtility.SetDirty(proxy);

                if (targetAssigned || boneAssigned)
                {
                    summary.BoneProxyFixed++;
                }
            }
        }

        private static void RepairMenuInstallersInHierarchy(Transform searchRoot, Type menuInstallerType, ref RepairSummary summary)
        {
            Component[] installers = searchRoot.GetComponentsInChildren(menuInstallerType, true);
            foreach (Component installer in installers)
            {
                if (installer == null) continue;
                if (TryGetReflected(installer, "menuToAppend") != null || HasMenuSourceComponent(installer)) continue;

                if (IsLightControlsTransform(installer.transform))
                {
                    bool includeIntensity = FindChildByName(installer.transform, "PCSS Lights Intensity") != null ||
                                            FindChildByNamePrefix(installer.transform, "PCSS Intensity - ") != null;
                    bool includeSway = FindChildByName(installer.transform, "PCSS Lights Sway Strength") != null;
                    object menu = ModularAvatarLightToggleBuilder.CreateOrUpdateLightMenu(includeIntensity, includeSway);

                    Undo.RecordObject(installer, "Repair PCSS Menu Installer");
                    if (TrySetReflected(installer, "menuToAppend", menu))
                    {
                        EditorUtility.SetDirty(installer);
                        summary.MenuInstallerFixed++;
                    }
                    continue;
                }

                if (installer.transform == searchRoot)
                {
                    Undo.DestroyObjectImmediate(installer);
                    summary.EmptyInstallerRemoved++;
                }
            }
        }

        private static bool HasMenuSourceComponent(Component installer)
        {
            Type menuSourceType = FindType("nadena.dev.modular_avatar.core.menu.MenuSource");
            if (menuSourceType == null || installer == null) return false;

            Component[] components = installer.GetComponents<Component>();
            return components.Any(component => component != null && menuSourceType.IsInstanceOfType(component));
        }

        private static GameObject ResolveAvatarRoot(GameObject selected)
        {
            return PCSSAvatarDescriptorGuard.ResolveAvatarRoot(selected);
        }

        private static void AutoRepairOpenScenes()
        {
            int totalRepairs = 0;

            for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIndex);
                if (!scene.isLoaded) continue;

                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    Animator[] animators = root.GetComponentsInChildren<Animator>(true);
                    foreach (Animator animator in animators)
                    {
                        if (animator == null || !IsUsableHumanoidAnimator(animator) || !ContainsPCSSGeneratedContent(animator.transform)) continue;

                        RepairSummary summary = RepairAvatar(animator.gameObject);
                        int repaired = summary.BoneProxyFixed + summary.MenuInstallerFixed + summary.EmptyInstallerRemoved;
                        if (repaired <= 0) continue;

                        totalRepairs += repaired;
                        EditorSceneManager.MarkSceneDirty(animator.gameObject.scene);
                    }
                }
            }

            if (totalRepairs > 0)
            {
                Debug.Log($"[PCSS] NDMF/AAO compatibility auto-repair applied to {totalRepairs} generated PCSS item(s).");
            }
        }

        private static Transform ResolveHipsFor(Transform transform)
        {
            if (transform == null) return null;

            Animator animator = transform.GetComponentInParent<Animator>(true);
            if (!IsUsableHumanoidAnimator(animator)) return null;

            return animator.GetBoneTransform(HumanBodyBones.Hips);
        }

        private static bool IsUsableHumanoidAnimator(Animator animator)
        {
            return animator != null && animator.avatar != null && animator.isHuman;
        }

        private static bool IsPCSSGeneratedTransform(Transform transform)
        {
            if (transform == null) return false;
            if (transform.name == HipAnchorName) return true;
            if (transform.name.Contains("PCSS")) return true;
            return HasAncestor(transform, HipLightGroupName) || HasAncestor(transform, AutoLightGroupName);
        }

        private static bool IsLightControlsTransform(Transform transform)
        {
            if (transform == null) return false;
            return transform.name == LightControlsName || HasAncestor(transform, LightControlsName);
        }

        private static bool ContainsPCSSGeneratedContent(Transform root)
        {
            if (root == null) return false;

            foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (IsPCSSGeneratedTransform(transform) || IsLightControlsTransform(transform))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasAncestor(Transform transform, string name)
        {
            Transform current = transform;
            while (current != null)
            {
                if (current.name == name) return true;
                current = current.parent;
            }

            return false;
        }

        private static Transform FindChildByName(Transform root, string name)
        {
            if (root == null) return null;

            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child != null && child.name == name)
                {
                    return child;
                }
            }

            return null;
        }

        private static Transform FindChildByNamePrefix(Transform root, string prefix)
        {
            if (root == null) return null;

            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child != null && child.name.StartsWith(prefix, StringComparison.Ordinal))
                {
                    return child;
                }
            }

            return null;
        }

        private static Type FindType(string fullName)
        {
            try
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly =>
                    {
                        try { return assembly.GetTypes(); }
                        catch { return Type.EmptyTypes; }
                    })
                    .FirstOrDefault(type => type.FullName == fullName);
            }
            catch
            {
                return null;
            }
        }

        private static object TryGetReflected(object component, string memberName)
        {
            if (component == null) return null;

            Type type = component.GetType();
            FieldInfo field = type.GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                return field.GetValue(component);
            }

            PropertyInfo property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null && property.CanRead)
            {
                try { return property.GetValue(component); }
                catch { return null; }
            }

            return null;
        }

        private static bool TrySetReflected(object component, string memberName, object value)
        {
            if (component == null) return false;

            Type type = component.GetType();
            FieldInfo field = type.GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null && (value == null || field.FieldType.IsAssignableFrom(value.GetType())))
            {
                field.SetValue(component, value);
                return true;
            }

            PropertyInfo property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null && property.CanWrite && (value == null || property.PropertyType.IsAssignableFrom(value.GetType())))
            {
                property.SetValue(component, value);
                return true;
            }

            UnityEngine.Object unityObject = component as UnityEngine.Object;
            if (unityObject == null) return false;

            SerializedObject serializedObject = new SerializedObject(unityObject);
            SerializedProperty serializedProperty = serializedObject.FindProperty(memberName);
            if (serializedProperty == null) return false;

            if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference && value is UnityEngine.Object objectValue)
            {
                serializedProperty.objectReferenceValue = objectValue;
                serializedObject.ApplyModifiedProperties();
                return true;
            }

            if (serializedProperty.propertyType == SerializedPropertyType.Enum && value != null && value.GetType().IsEnum)
            {
                serializedProperty.enumValueIndex = Convert.ToInt32(value);
                serializedObject.ApplyModifiedProperties();
                return true;
            }

            return false;
        }

        internal struct RepairSummary
        {
            public int BoneProxyFixed;
            public int BoneProxySkipped;
            public int MenuInstallerFixed;
            public int EmptyInstallerRemoved;
        }
    }
}
