#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace lilToon.PCSS.Editor
{
    internal static class PCSSAvatarDescriptorGuard
    {
        private const string MenuRoot = "Tools/lilToon-PCSS-Extension/PCSS Hub/";
        private const string ControllerFolder = "Assets/PCSS/Controllers";
        private const string AutoRepairSessionKey = "lilToon.PCSS.AvatarDescriptorGuard.AutoRepairOpenScenes.v1";
        private static bool selectionRepairQueued;

        [InitializeOnLoadMethod]
        private static void AutoRepairOpenScenesAfterReload()
        {
            Selection.selectionChanged -= QueueSelectedAvatarRepair;
            Selection.selectionChanged += QueueSelectedAvatarRepair;

            if (SessionState.GetBool(AutoRepairSessionKey, false)) return;
            SessionState.SetBool(AutoRepairSessionKey, true);
            EditorApplication.delayCall += AutoRepairOpenScenesQuietly;
        }

        [MenuItem(MenuRoot + "Repair Avatar Descriptor for MA Outfit", false, 45)]
        public static void RepairSelectedAvatar()
        {
            if (!TryEnsureDescriptor(Selection.activeGameObject, useUndo: true, out RepairReport report))
            {
                EditorUtility.DisplayDialog("PCSS Avatar Descriptor Guard", report.ToDialogString(), "OK");
                return;
            }

            if (report.AvatarRoot != null)
            {
                Selection.activeGameObject = report.AvatarRoot;
                EditorGUIUtility.PingObject(report.AvatarRoot);
            }

            AssetDatabase.SaveAssets();
            Debug.Log(report.ToLogString());
            EditorUtility.DisplayDialog("PCSS Avatar Descriptor Guard", report.ToDialogString(), "OK");
        }

        [MenuItem(MenuRoot + "Repair All Scene Avatar Descriptors for MA Outfit", false, 46)]
        public static void RepairAllSceneAvatars()
        {
            AutoRepairReport report = RepairOpenScenes(useUndo: true);
            if (report.Repaired > 0 || report.ExpressionAssetsCreated > 0)
            {
                AssetDatabase.SaveAssets();
            }

            EditorUtility.DisplayDialog("PCSS Avatar Descriptor Guard", report.ToDialogString(), "OK");
        }

        [MenuItem(MenuRoot + "Repair Avatar Descriptor for MA Outfit", true)]
        private static bool CanRepairSelectedAvatar()
        {
            return Selection.activeGameObject != null;
        }

        internal static bool TryEnsureDescriptor(GameObject selected, bool useUndo, out RepairReport report)
        {
            report = new RepairReport
            {
                SelectedName = selected != null ? selected.name : string.Empty,
                SourceAssetPath = selected != null ? PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(selected) : string.Empty
            };

            if (selected == null)
            {
                report.Errors = "Select an avatar root or one of its children.";
                return false;
            }

            GameObject avatarRoot = ResolveAvatarRoot(selected);
            report.AvatarRoot = avatarRoot;
            report.AvatarRootName = avatarRoot != null ? avatarRoot.name : string.Empty;
            report.SourceAssetPath = avatarRoot != null
                ? PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(avatarRoot)
                : report.SourceAssetPath;

            if (avatarRoot == null)
            {
                report.Errors = "Could not resolve an avatar root from the current selection.";
                return false;
            }

            Animator animator = avatarRoot.GetComponent<Animator>();
            if (!IsUsableHumanoidAnimator(animator))
            {
                report.Errors = "The resolved root has no Humanoid Animator. Select the real avatar root, not an outfit-only object.";
                return false;
            }

            VRCAvatarDescriptor descriptor = avatarRoot.GetComponent<VRCAvatarDescriptor>();
            if (descriptor != null)
            {
                report.DescriptorAlreadyPresent = true;
                report.ExpressionAssetsCreated = EnsureExpressionAssets(descriptor, avatarRoot, useUndo);
                EditorUtility.SetDirty(descriptor);
                return true;
            }

            report.DescriptorAdded = true;
            descriptor = useUndo
                ? Undo.AddComponent<VRCAvatarDescriptor>(avatarRoot)
                : avatarRoot.AddComponent<VRCAvatarDescriptor>();

            VRCAvatarDescriptor reference = FindReferenceDescriptor(avatarRoot);
            if (reference != null)
            {
                CopySafeDescriptorSettings(reference, descriptor);
                report.CopiedFromReference = true;
                report.ReferenceName = reference.gameObject.name;
            }

            EnsureViewPosition(descriptor, animator, avatarRoot.transform);
            report.ExpressionAssetsCreated = EnsureExpressionAssets(descriptor, avatarRoot, useUndo);

            EditorUtility.SetDirty(descriptor);
            EditorUtility.SetDirty(avatarRoot);
            return true;
        }

        internal static AutoRepairReport RepairOpenScenes(bool useUndo)
        {
            AutoRepairReport report = new AutoRepairReport();

            for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIndex);
                if (!scene.isLoaded) continue;

                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    Animator[] animators = root.GetComponentsInChildren<Animator>(true);
                    foreach (Animator animator in animators)
                    {
                        if (!IsUsableHumanoidAnimator(animator)) continue;
                        report.CheckedHumanoidAnimators++;
                        if (!ShouldAutoRepairAnimator(animator)) continue;

                        if (!TryEnsureDescriptor(animator.gameObject, useUndo, out RepairReport repair))
                        {
                            report.Failed++;
                            report.AppendFailure(repair);
                            continue;
                        }

                        if (repair.DescriptorAdded)
                        {
                            report.Repaired++;
                            report.AppendRepair(repair);
                            EditorSceneManager.MarkSceneDirty(scene);
                        }
                        else if (repair.ExpressionAssetsCreated)
                        {
                            report.ExpressionAssetsCreated++;
                            EditorSceneManager.MarkSceneDirty(scene);
                        }
                        else
                        {
                            report.AlreadyReady++;
                        }
                    }
                }
            }

            return report;
        }

        private static void AutoRepairOpenScenesQuietly()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return;
            }

            AutoRepairReport report = RepairOpenScenes(useUndo: false);
            if (report.Repaired <= 0 && report.ExpressionAssetsCreated <= 0) return;

            AssetDatabase.SaveAssets();
            Debug.Log("[PCSS Avatar Descriptor Guard] " + report.ToLogString());
        }

        private static void QueueSelectedAvatarRepair()
        {
            if (selectionRepairQueued) return;
            selectionRepairQueued = true;
            EditorApplication.delayCall += AutoRepairSelectionQuietly;
        }

        private static void AutoRepairSelectionQuietly()
        {
            selectionRepairQueued = false;
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return;
            }

            GameObject selected = Selection.activeGameObject;
            GameObject avatarRoot = ResolveAvatarRoot(selected);
            Animator animator = avatarRoot != null ? avatarRoot.GetComponent<Animator>() : null;
            if (!ShouldAutoRepairAnimator(animator)) return;

            if (!TryEnsureDescriptor(avatarRoot, useUndo: false, out RepairReport repair))
            {
                Debug.LogWarning("[PCSS Avatar Descriptor Guard] Selection auto-repair skipped. " + repair.ToLogString());
                return;
            }

            if (!repair.DescriptorAdded && !repair.ExpressionAssetsCreated) return;

            EditorSceneManager.MarkSceneDirty(avatarRoot.scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[PCSS Avatar Descriptor Guard] Selection auto-repair applied. " + repair.ToLogString());
        }

        internal static GameObject ResolveAvatarRoot(GameObject selected)
        {
            if (selected == null) return null;

            VRCAvatarDescriptor descriptor = selected.GetComponentInParent<VRCAvatarDescriptor>(true);
            if (descriptor != null) return descriptor.gameObject;

            Animator animator = selected.GetComponent<Animator>();
            if (IsUsableHumanoidAnimator(animator)) return selected;

            Animator parentAnimator = selected.GetComponentInParent<Animator>(true);
            if (IsUsableHumanoidAnimator(parentAnimator)) return parentAnimator.gameObject;

            GameObject root = selected.transform.root != null ? selected.transform.root.gameObject : selected;
            Animator rootAnimator = root.GetComponent<Animator>();
            if (IsUsableHumanoidAnimator(rootAnimator)) return root;

            return selected;
        }

        private static bool ShouldAutoRepairAnimator(Animator animator)
        {
            if (!IsUsableHumanoidAnimator(animator)) return false;

            GameObject avatarRoot = animator.gameObject;
            if (avatarRoot == null || !avatarRoot.activeInHierarchy) return false;
            if (avatarRoot.GetComponent<VRCAvatarDescriptor>() != null) return false;
            if (avatarRoot.GetComponentInParent<VRCAvatarDescriptor>(true) != null) return false;

            string sourceAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(avatarRoot);
            if (string.IsNullOrEmpty(sourceAssetPath) || !sourceAssetPath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return FindReferenceDescriptor(avatarRoot) != null;
        }

        private static bool IsUsableHumanoidAnimator(Animator animator)
        {
            return animator != null && animator.avatar != null && animator.isHuman;
        }

        private static VRCAvatarDescriptor FindReferenceDescriptor(GameObject avatarRoot)
        {
            string baseName = StripCloneSuffix(avatarRoot.name);

            VRCAvatarDescriptor sceneReference = UnityEngine.Object.FindObjectsOfType<VRCAvatarDescriptor>(true)
                .Where(desc => desc != null && desc.gameObject != avatarRoot)
                .Where(desc => StripCloneSuffix(desc.gameObject.name) == baseName)
                .OrderByDescending(desc => desc.gameObject.activeInHierarchy)
                .ThenBy(desc => desc.gameObject.name.IndexOf("Backup", StringComparison.OrdinalIgnoreCase) >= 0)
                .FirstOrDefault();

            if (sceneReference != null) return sceneReference;

            string directPrefabPath = GuessAvatarPrefabPath(avatarRoot);
            if (!string.IsNullOrEmpty(directPrefabPath))
            {
                VRCAvatarDescriptor direct = LoadDescriptorFromPrefab(directPrefabPath);
                if (direct != null) return direct;
            }

            string[] prefabGuids = AssetDatabase.FindAssets(baseName + " t:Prefab");
            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                VRCAvatarDescriptor descriptor = LoadDescriptorFromPrefab(path);
                if (descriptor != null) return descriptor;
            }

            return null;
        }

        private static string GuessAvatarPrefabPath(GameObject avatarRoot)
        {
            string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(avatarRoot);
            if (string.IsNullOrEmpty(path) || !path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            string fileName = Path.GetFileNameWithoutExtension(path);
            string directory = Path.GetDirectoryName(path)?.Replace('\\', '/') ?? string.Empty;
            string prefabDirectory = directory.Replace("/FBX", "/Prefab");
            string candidate = prefabDirectory + "/" + fileName + ".prefab";
            return AssetDatabase.LoadAssetAtPath<GameObject>(candidate) != null ? candidate : string.Empty;
        }

        private static VRCAvatarDescriptor LoadDescriptorFromPrefab(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return prefab != null ? prefab.GetComponent<VRCAvatarDescriptor>() : null;
        }

        private static void CopySafeDescriptorSettings(VRCAvatarDescriptor source, VRCAvatarDescriptor destination)
        {
            SerializedObject sourceObject = new SerializedObject(source);
            SerializedObject destinationObject = new SerializedObject(destination);
            string[] propertyNames =
            {
                "ViewPosition",
                "customExpressions",
                "expressionsMenu",
                "expressionParameters",
                "baseAnimationLayers",
                "specialAnimationLayers"
            };

            foreach (string propertyName in propertyNames)
            {
                SerializedProperty sourceProperty = sourceObject.FindProperty(propertyName);
                if (sourceProperty == null || destinationObject.FindProperty(propertyName) == null) continue;
                destinationObject.CopyFromSerializedProperty(sourceProperty);
            }

            destinationObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureViewPosition(VRCAvatarDescriptor descriptor, Animator animator, Transform root)
        {
            SerializedObject serialized = new SerializedObject(descriptor);
            SerializedProperty viewPosition = serialized.FindProperty("ViewPosition");
            if (viewPosition == null || viewPosition.vector3Value != Vector3.zero)
            {
                return;
            }

            Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
            Vector3 local = head != null
                ? root.InverseTransformPoint(head.position) + new Vector3(0f, 0.02f, 0.06f)
                : new Vector3(0f, 1.35f, 0.08f);
            viewPosition.vector3Value = local;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static bool EnsureExpressionAssets(VRCAvatarDescriptor descriptor, GameObject avatarRoot, bool useUndo)
        {
            bool created = false;
            EnsureControllerFolder();
            string safeName = SanitizeFileName(StripCloneSuffix(avatarRoot.name));

            if (descriptor.expressionParameters == null)
            {
                VRCExpressionParameters parameters = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                parameters.name = safeName + "_ExpressionParameters";
                string path = AssetDatabase.GenerateUniqueAssetPath(ControllerFolder + "/" + parameters.name + ".asset");
                AssetDatabase.CreateAsset(parameters, path);
                if (useUndo) Undo.RegisterCreatedObjectUndo(parameters, "Create expression parameters");
                descriptor.expressionParameters = parameters;
                created = true;
            }

            if (descriptor.expressionsMenu == null)
            {
                VRCExpressionsMenu menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                menu.name = safeName + "_ExpressionsMenu";
                string path = AssetDatabase.GenerateUniqueAssetPath(ControllerFolder + "/" + menu.name + ".asset");
                AssetDatabase.CreateAsset(menu, path);
                if (useUndo) Undo.RegisterCreatedObjectUndo(menu, "Create expressions menu");
                descriptor.expressionsMenu = menu;
                created = true;
            }

            return created;
        }

        private static void EnsureControllerFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/PCSS"))
            {
                AssetDatabase.CreateFolder("Assets", "PCSS");
            }

            if (!AssetDatabase.IsValidFolder(ControllerFolder))
            {
                AssetDatabase.CreateFolder("Assets/PCSS", "Controllers");
            }
        }

        private static string StripCloneSuffix(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;
            string trimmed = name.Trim();
            int suffixStart = trimmed.LastIndexOf(" (", StringComparison.Ordinal);
            if (suffixStart > 0 && trimmed.EndsWith(")", StringComparison.Ordinal))
            {
                string suffix = trimmed.Substring(suffixStart + 2, trimmed.Length - suffixStart - 3);
                if (suffix.All(char.IsDigit))
                {
                    trimmed = trimmed.Substring(0, suffixStart);
                }
            }

            return trimmed;
        }

        private static string SanitizeFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Avatar";
            char[] invalid = Path.GetInvalidFileNameChars();
            StringBuilder builder = new StringBuilder(name.Length);
            foreach (char c in name)
            {
                builder.Append(invalid.Contains(c) ? '_' : c);
            }

            return builder.ToString();
        }

        internal struct RepairReport
        {
            public string SelectedName;
            public string AvatarRootName;
            public GameObject AvatarRoot;
            public string SourceAssetPath;
            public string ReferenceName;
            public bool DescriptorAlreadyPresent;
            public bool DescriptorAdded;
            public bool CopiedFromReference;
            public bool ExpressionAssetsCreated;
            public string Errors;

            public string StatusText
            {
                get
                {
                    if (!string.IsNullOrEmpty(Errors)) return "error";
                    if (DescriptorAdded && CopiedFromReference) return "added from " + ReferenceName;
                    if (DescriptorAdded) return "added";
                    if (DescriptorAlreadyPresent) return "already present";
                    return "checked";
                }
            }

            public string ToDialogString()
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(string.IsNullOrEmpty(Errors)
                    ? "Avatar Descriptor is ready for Modular Avatar outfit tools."
                    : "Avatar Descriptor repair could not finish.");
                builder.AppendLine();
                builder.AppendLine("Selected: " + SelectedName);
                builder.AppendLine("Avatar root: " + AvatarRootName);
                if (!string.IsNullOrEmpty(SourceAssetPath))
                {
                    builder.AppendLine("Source asset: " + SourceAssetPath);
                    if (SourceAssetPath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase))
                    {
                        builder.AppendLine("Note: this was an FBX instance, so a Descriptor shell was needed.");
                    }
                }

                builder.AppendLine("Descriptor: " + StatusText);
                if (ExpressionAssetsCreated)
                {
                    builder.AppendLine("Expression assets: created fallback assets.");
                }

                if (!string.IsNullOrEmpty(Errors))
                {
                    builder.AppendLine();
                    builder.AppendLine(Errors);
                }

                return builder.ToString();
            }

            public string ToLogString()
            {
                return "[PCSS Avatar Descriptor Guard] " + ToDialogString();
            }
        }

        internal struct AutoRepairReport
        {
            public int CheckedHumanoidAnimators;
            public int Repaired;
            public int AlreadyReady;
            public int ExpressionAssetsCreated;
            public int Failed;
            private StringBuilder repairs;
            private StringBuilder failures;

            public void AppendRepair(RepairReport report)
            {
                if (repairs == null) repairs = new StringBuilder();
                repairs.AppendLine("- " + report.AvatarRootName + " (" + report.StatusText + ")");
            }

            public void AppendFailure(RepairReport report)
            {
                if (failures == null) failures = new StringBuilder();
                failures.AppendLine("- " + report.SelectedName + ": " + report.Errors);
            }

            public string ToDialogString()
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(Repaired > 0
                    ? "Scene avatar descriptors are ready for Modular Avatar outfit tools."
                    : "No missing scene avatar descriptors needed repair.");
                builder.AppendLine();
                builder.AppendLine("Humanoid animators checked: " + CheckedHumanoidAnimators);
                builder.AppendLine("Descriptors added: " + Repaired);
                builder.AppendLine("Expression assets created: " + ExpressionAssetsCreated);
                builder.AppendLine("Already ready: " + AlreadyReady);
                builder.AppendLine("Failed: " + Failed);

                if (repairs != null && repairs.Length > 0)
                {
                    builder.AppendLine();
                    builder.AppendLine("Repaired:");
                    builder.Append(repairs);
                }

                if (failures != null && failures.Length > 0)
                {
                    builder.AppendLine();
                    builder.AppendLine("Failures:");
                    builder.Append(failures);
                }

                return builder.ToString();
            }

            public string ToLogString()
            {
                return ToDialogString();
            }
        }
    }
}
#endif
