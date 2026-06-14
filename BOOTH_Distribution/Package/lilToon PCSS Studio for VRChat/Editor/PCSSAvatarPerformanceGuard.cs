#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    internal static class PCSSAvatarPerformanceGuard
    {
        private const string HipLightGroupName = "PCSS Hip Lights (MA)";
        private const string AutoLightGroupName = "PCSS External Lights (Auto)";
        private const string LightControlsName = "PCSS Light Controls (MA)";
        private const string AllowAvatarLightsMarkerName = "PCSS Allow Avatar Lights";
        private const string ControllerPath = "Assets/PCSS/Controllers/PCSS_LightControl.controller";

        [MenuItem("Tools/lilToon-PCSS-Extension/Repair/AAO Performance Guard (0 Avatar Lights)", false, 91)]
        public static void RepairSelectedAvatar()
        {
            GameObject avatarRoot = ResolveAvatarRoot(Selection.activeGameObject);
            if (avatarRoot == null)
            {
                EditorUtility.DisplayDialog("PCSS", "Select the avatar root or a child object first.", "OK");
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo(avatarRoot, "Apply PCSS AAO Performance Guard");
            PerformanceSummary summary = Apply(avatarRoot, useUndo: true, removeGeneratedLightControls: true, tuneMaterials: true);
            EditorSceneManager.MarkSceneDirty(avatarRoot.scene);

            EditorUtility.DisplayDialog(
                "PCSS AAO Performance Guard",
                $"PCSS generated Lights removed: {summary.RemovedLightObjects}\n" +
                $"Other avatar Light components removed: {summary.RemovedOtherLightComponents}\n" +
                $"PCSS light control roots removed: {summary.RemovedControlObjects}\n" +
                $"PCSS MA merge animators removed: {summary.RemovedMergeAnimators}\n" +
                $"PCSS materials tuned: {summary.TunedMaterials}\n" +
                $"Remaining avatar Lights: {summary.RemainingLights}",
                "OK");
        }

        internal static PerformanceSummary Apply(
            GameObject avatarRoot,
            bool useUndo,
            bool removeGeneratedLightControls,
            bool tuneMaterials)
        {
            PerformanceSummary summary = new PerformanceSummary();
            if (avatarRoot == null) return summary;

            if (tuneMaterials)
            {
                summary.TunedMaterials = ApplyMaterialSafeDefaults(avatarRoot, useUndo);
            }

            summary.RemovedLightObjects = RemoveGeneratedLightObjects(avatarRoot, useUndo);
            summary.RemovedOtherLightComponents = RemoveRemainingAvatarLightComponents(avatarRoot, useUndo);

            if (removeGeneratedLightControls)
            {
                summary.RemovedControlObjects = RemoveGeneratedLightControls(avatarRoot, useUndo);
                summary.RemovedMergeAnimators = RemoveGeneratedMergeAnimators(avatarRoot, useUndo);
            }

            summary.RemainingLights = avatarRoot.GetComponentsInChildren<Light>(true).Length;

            if (summary.TotalChanges > 0)
            {
                EditorUtility.SetDirty(avatarRoot);
                if (avatarRoot.scene.IsValid())
                {
                    EditorSceneManager.MarkSceneDirty(avatarRoot.scene);
                }
            }

            return summary;
        }

        internal static bool HasAllowAvatarLightsMarker(GameObject avatarRoot)
        {
            return avatarRoot != null && avatarRoot.transform.Find(AllowAvatarLightsMarkerName) != null;
        }

        internal static void SetAllowAvatarLightsMarker(GameObject avatarRoot, bool allow, bool useUndo)
        {
            if (avatarRoot == null) return;

            Transform existing = avatarRoot.transform.Find(AllowAvatarLightsMarkerName);
            if (allow)
            {
                if (existing != null) return;

                GameObject marker = new GameObject(AllowAvatarLightsMarkerName);
                if (useUndo)
                {
                    Undo.RegisterCreatedObjectUndo(marker, "Create PCSS Allow Avatar Lights Marker");
                }

                marker.transform.SetParent(avatarRoot.transform, false);
                marker.transform.localPosition = Vector3.zero;
                marker.transform.localRotation = Quaternion.identity;
                marker.transform.localScale = Vector3.one;
                EditorUtility.SetDirty(avatarRoot);
                return;
            }

            if (existing != null)
            {
                DestroyObject(existing.gameObject, useUndo);
                EditorUtility.SetDirty(avatarRoot);
            }
        }

        private static int ApplyMaterialSafeDefaults(GameObject avatarRoot, bool useUndo)
        {
            int changed = 0;
            HashSet<Material> materials = new HashSet<Material>();
            Renderer[] renderers = avatarRoot.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null || renderer.sharedMaterials == null) continue;
                foreach (Material material in renderer.sharedMaterials)
                {
                    if (IsPCSSMaterial(material))
                    {
                        materials.Add(material);
                    }
                }
            }

            foreach (Material material in materials)
            {
                if (useUndo)
                {
                    Undo.RecordObject(material, "Apply PCSS AAO Safe Material Defaults");
                }
                bool materialChanged = false;
                materialChanged |= SetFloatIfExists(material, "_UsePCSS", 1.0f);
                materialChanged |= SetFloatIfExists(material, "_PCSSEnabled", 1.0f);
                materialChanged |= SetFloatIfExists(material, "_PCSSPresetMode", 6.0f);
                materialChanged |= SetFloatIfExists(material, "_UseVRChatPerformanceGate", 1.0f);
                materialChanged |= SetFloatIfExists(material, "_PCSSMaxDistance", 10.0f);
                materialChanged |= SetFloatIfExists(material, "_PCSSDistanceFade", 3.0f);
                materialChanged |= SetFloatIfExists(material, "_LocalPCSSFilterRadius", 0.0100f);
                materialChanged |= SetFloatIfExists(material, "_PCSSFilterRadius", 0.0100f);
                materialChanged |= SetFloatIfExists(material, "_LocalPCSSLightSize", 0.105f);
                materialChanged |= SetFloatIfExists(material, "_PCSSLightSize", 0.105f);
                materialChanged |= SetFloatIfExists(material, "_LocalPCSSBias", 0.0009f);
                materialChanged |= SetFloatIfExists(material, "_PCSSBias", 0.0009f);
                materialChanged |= SetFloatIfExists(material, "_LocalPCSSSamples", 6.0f);
                materialChanged |= SetFloatIfExists(material, "_PCSSSamples", 6.0f);
                materialChanged |= SetFloatIfExists(material, "_PCSSQualityLevel", 1.0f);
                materialChanged |= SetFloatIfExists(material, "_PCSSIntensity", 0.92f);
                materialChanged |= SetFloatIfExists(material, "_UseGlossShadowCoherence", 1.0f);
                materialChanged |= SetFloatIfExists(material, "_GlossShadowCoherence", 0.68f);
                materialChanged |= SetFloatIfExists(material, "_GlossShadowBoost", 0.52f);
                materialChanged |= SetFloatIfExists(material, "_GlossShadowSuppression", 0.42f);
                materialChanged |= SetFloatIfExists(material, "_GlossRimStrength", 0.38f);
                materialChanged |= SetFloatIfExists(material, "_GlossSmoothness", 0.78f);
                materialChanged |= SetFloatIfExists(material, "_UseRealisticShadow", 1.0f);
                materialChanged |= SetFloatIfExists(material, "_RealisticShadowIntensity", 0.56f);
                materialChanged |= SetFloatIfExists(material, "_RealisticShadowSoftness", 0.70f);
                materialChanged |= SetFloatIfExists(material, "_UseLightDirectionOverride", 1.0f);
                materialChanged |= SetVectorIfExists(material, "_LightDirectionOverride", new Vector4(0.24f, 0.84f, 0.48f, 0.0f));
                materialChanged |= SetFloatIfExists(material, "_UseNoLightPCSSBoost", 1.0f);
                materialChanged |= SetFloatIfExists(material, "_NoLightPCSSBoostStrength", 0.62f);
                materialChanged |= SetFloatIfExists(material, "_NoLightPCSSBoostSoftness", 0.68f);
                materialChanged |= SetFloatIfExists(material, "_NoLightPCSSBoostRim", 0.44f);
                materialChanged |= SetColorIfExists(material, "_NoLightPCSSHighlightTint", new Color(0.66f, 0.60f, 0.58f, 1.0f));
                materialChanged |= SetFloatIfExists(material, "_UseVRCLightVolumes", 0.0f);
                materialChanged |= SetFloatIfExists(material, "_VRCLightVolumesEnabled", 0.0f);
                materialChanged |= SetFloatIfExists(material, "_UseVRCLVRimLight", 0.0f);

                SetKeyword(material, "_USEPCSS_ON", true);
                SetKeyword(material, "_USEPCSSOPTIMIZATION_ON", true);
                SetKeyword(material, "_USEVRCHATPERFORMANCEGATE_ON", true);
                SetKeyword(material, "_USEGLOSSSHADOWCOHERENCE_ON", true);
                SetKeyword(material, "_USEREALISTICSHADOW_ON", true);
                if (material.HasProperty("_UseLightDirectionOverride"))
                {
                    SetKeyword(material, "_USELIGHTDIRECTIONOVERRIDE_ON", true);
                }
                if (material.HasProperty("_UseNoLightPCSSBoost"))
                {
                    SetKeyword(material, "_USENOLIGHTPCSSBOOST_ON", true);
                }

                if (!materialChanged) continue;
                EditorUtility.SetDirty(material);
                changed++;
            }

            return changed;
        }

        private static int RemoveGeneratedLightObjects(GameObject avatarRoot, bool useUndo)
        {
            int removed = 0;
            HashSet<GameObject> toRemove = new HashSet<GameObject>();

            foreach (string groupName in new[] { HipLightGroupName, AutoLightGroupName })
            {
                Transform group = avatarRoot.transform.Find(groupName);
                if (group != null && group.GetComponentsInChildren<Light>(true).Length > 0)
                {
                    toRemove.Add(group.gameObject);
                }
            }

            Light[] lights = avatarRoot.GetComponentsInChildren<Light>(true);
            foreach (Light light in lights)
            {
                if (light == null) continue;
                if (IsGeneratedPCSSLight(light.transform))
                {
                    toRemove.Add(FindGeneratedRemovalRoot(light.transform, avatarRoot.transform).gameObject);
                }
            }

            foreach (GameObject go in toRemove.Where(go => go != null).ToList())
            {
                DestroyObject(go, useUndo);
                removed++;
            }

            return removed;
        }

        private static int RemoveRemainingAvatarLightComponents(GameObject avatarRoot, bool useUndo)
        {
            int removed = 0;
            Light[] lights = avatarRoot.GetComponentsInChildren<Light>(true);
            foreach (Light light in lights)
            {
                if (light == null) continue;
                if (DestroyLightObjectOrComponent(light, avatarRoot.transform, useUndo))
                {
                    removed++;
                }
            }

            return removed;
        }

        internal static int RemoveNonPCSSAvatarLightComponents(GameObject avatarRoot, bool useUndo)
        {
            int removed = 0;
            if (avatarRoot == null) return removed;

            Light[] lights = avatarRoot.GetComponentsInChildren<Light>(true);
            foreach (Light light in lights)
            {
                if (light == null || IsGeneratedPCSSLight(light.transform)) continue;
                if (DestroyLightObjectOrComponent(light, avatarRoot.transform, useUndo))
                {
                    removed++;
                }
            }

            if (removed > 0)
            {
                EditorUtility.SetDirty(avatarRoot);
                if (avatarRoot.scene.IsValid())
                {
                    EditorSceneManager.MarkSceneDirty(avatarRoot.scene);
                }
            }

            return removed;
        }

        private static int RemoveGeneratedLightControls(GameObject avatarRoot, bool useUndo)
        {
            int removed = 0;
            Transform controls = avatarRoot.transform.Find(LightControlsName);
            if (controls != null)
            {
                DestroyObject(controls.gameObject, useUndo);
                removed++;
            }

            return removed;
        }

        private static int RemoveGeneratedMergeAnimators(GameObject avatarRoot, bool useUndo)
        {
            Type mergeAnimatorType = FindType("nadena.dev.modular_avatar.core.ModularAvatarMergeAnimator");
            if (mergeAnimatorType == null) return 0;

            int removed = 0;
            Component[] components = avatarRoot.GetComponents(mergeAnimatorType);
            foreach (Component component in components)
            {
                if (component == null) continue;
                object animator = TryGetReflected(component, "animator") ?? TryGetReflected(component, "Animator");
                UnityEngine.Object animatorObject = animator as UnityEngine.Object;
                string assetPath = animatorObject != null ? AssetDatabase.GetAssetPath(animatorObject) : string.Empty;
                string objectName = animatorObject != null ? animatorObject.name : string.Empty;
                if (assetPath != ControllerPath && objectName.IndexOf("PCSS_LightControl", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                DestroyObject(component, useUndo);
                removed++;
            }

            return removed;
        }

        private static Transform FindGeneratedRemovalRoot(Transform lightTransform, Transform avatarRoot)
        {
            Transform current = lightTransform;
            Transform best = lightTransform;

            while (current != null && current != avatarRoot)
            {
                if (current.name == HipLightGroupName || current.name == AutoLightGroupName)
                {
                    return current;
                }

                if (current.name.IndexOf("PCSS", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    best = current;
                }

                current = current.parent;
            }

            return best;
        }

        private static bool IsGeneratedPCSSLight(Transform transform)
        {
            if (transform == null) return false;
            return transform.name.IndexOf("PCSS", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   HasAncestor(transform, HipLightGroupName) ||
                   HasAncestor(transform, AutoLightGroupName);
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

        private static bool DestroyLightObjectOrComponent(Light light, Transform avatarRoot, bool useUndo)
        {
            if (light == null) return false;

            GameObject owner = light.gameObject;
            if (IsLightOnlyObject(owner, avatarRoot))
            {
                DestroyObject(owner, useUndo);
                return true;
            }

            DestroyObject(light, useUndo);
            return true;
        }

        private static bool IsLightOnlyObject(GameObject owner, Transform avatarRoot)
        {
            if (owner == null || avatarRoot == null) return false;
            if (owner.transform == avatarRoot) return false;
            if (owner.transform.childCount > 0) return false;

            Component[] components = owner.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component == null) continue;
                if (component is Transform || component is Light) continue;
                return false;
            }

            return true;
        }

        private static GameObject ResolveAvatarRoot(GameObject selected)
        {
            return PCSSAvatarDescriptorGuard.ResolveAvatarRoot(selected);
        }

        private static bool IsPCSSMaterial(Material material)
        {
            if (material == null || material.shader == null) return false;
            string shaderName = material.shader.name ?? string.Empty;
            return shaderName.IndexOf("PCSS", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   material.HasProperty("_UsePCSS") ||
                   material.HasProperty("_PCSSEnabled") ||
                   material.HasProperty("_LocalPCSSFilterRadius") ||
                   material.HasProperty("_PCSSFilterRadius");
        }

        private static bool SetFloatIfExists(Material material, string name, float value)
        {
            if (material == null || !material.HasProperty(name)) return false;
            if (Mathf.Approximately(material.GetFloat(name), value)) return false;
            material.SetFloat(name, value);
            return true;
        }

        private static bool SetVectorIfExists(Material material, string name, Vector4 value)
        {
            if (material == null || !material.HasProperty(name)) return false;
            if (material.GetVector(name) == value) return false;
            material.SetVector(name, value);
            return true;
        }

        private static bool SetColorIfExists(Material material, string name, Color value)
        {
            if (material == null || !material.HasProperty(name)) return false;
            if (material.GetColor(name) == value) return false;
            material.SetColor(name, value);
            return true;
        }

        private static float GetFloat(Material material, string name, float defaultValue)
        {
            if (material == null || !material.HasProperty(name)) return defaultValue;
            return material.GetFloat(name);
        }

        private static void SetKeyword(Material material, string keyword, bool enabled)
        {
            if (material == null) return;

            if (enabled)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
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

        private static void DestroyObject(UnityEngine.Object obj, bool useUndo)
        {
            if (obj == null) return;
            if (useUndo)
            {
                Undo.DestroyObjectImmediate(obj);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
        }

        internal struct PerformanceSummary
        {
            public int RemovedLightObjects;
            public int RemovedOtherLightComponents;
            public int RemovedControlObjects;
            public int RemovedMergeAnimators;
            public int TunedMaterials;
            public int RemainingLights;

            public int TotalChanges => RemovedLightObjects + RemovedOtherLightComponents + RemovedControlObjects + RemovedMergeAnimators + TunedMaterials;
        }
    }
}
#endif
