#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    internal static class LegacyPCSSV220Compatibility
    {
        private static readonly string[] LegacyComponentTypeNames =
        {
            "PCSS.Core.PCSSLightPlugin",
            "VASES.Core.VRChatSensualExpressionSystem",
            "PCSSShader.Utils.SetCameraDepthMode"
        };

        [MenuItem("Tools/lilToon-PCSS-Extension/Repair/Legacy PCSS v2.2.0 Intake", false, 92)]
        public static void RepairSelectedAvatar()
        {
            GameObject avatarRoot = ResolveAvatarRoot(Selection.activeGameObject);
            if (avatarRoot == null)
            {
                EditorUtility.DisplayDialog("PCSS", "Select the avatar root or a child object first.", "OK");
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo(avatarRoot, "Intake Legacy PCSS v2.2.0");

            IntakeSummary intake = Apply(avatarRoot, useUndo: true);
            PCSSAvatarPerformanceGuard.PerformanceSummary guard = PCSSAvatarPerformanceGuard.Apply(
                avatarRoot,
                useUndo: true,
                removeGeneratedLightControls: true,
                tuneMaterials: true);

            EditorUtility.SetDirty(avatarRoot);
            if (avatarRoot.scene.IsValid())
            {
                EditorSceneManager.MarkSceneDirty(avatarRoot.scene);
            }

            EditorUtility.DisplayDialog(
                "Legacy PCSS v2.2.0 Intake",
                $"Legacy materials migrated: {intake.MigratedMaterials}\n" +
                $"Legacy runtime components removed: {intake.RemovedLegacyComponents}\n" +
                $"PCSS generated Lights removed: {guard.RemovedLightObjects}\n" +
                $"Other avatar Light components removed: {guard.RemovedOtherLightComponents}\n" +
                $"Remaining avatar Lights: {guard.RemainingLights}",
                "OK");
        }

        internal static IntakeSummary Apply(GameObject avatarRoot, bool useUndo)
        {
            IntakeSummary summary = new IntakeSummary();
            if (avatarRoot == null) return summary;

            summary.MigratedMaterials = MigrateLegacyMaterials(avatarRoot, useUndo);
            summary.RemovedLegacyComponents = RemoveLegacyRuntimeComponents(avatarRoot, useUndo);

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

        private static int MigrateLegacyMaterials(GameObject avatarRoot, bool useUndo)
        {
            int changed = 0;
            HashSet<Material> materials = new HashSet<Material>();
            Renderer[] renderers = avatarRoot.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null || renderer.sharedMaterials == null) continue;
                foreach (Material material in renderer.sharedMaterials)
                {
                    if (IsLegacyPCSSMaterial(material))
                    {
                        materials.Add(material);
                    }
                }
            }

            foreach (Material material in materials)
            {
                if (useUndo)
                {
                    Undo.RecordObject(material, "Migrate Legacy PCSS v2.2.0 Material");
                }

                bool materialChanged = false;
                float oldEnabled = GetFloat(material, "_PCSSEnabled", 1.0f);
                float oldIntensity = GetFloat(material, "_PCSSIntensity", 1.0f);
                float oldSoftness = GetFloat(material, "_PCSSoftness", 0.75f);
                float oldSampleRadius = GetFloat(material, "_PCSSSampleRadius", 0.045f);
                float oldBlockerSamples = GetFloat(material, "_PCSSBlockerSampleCount", 16.0f);
                float oldPcfSamples = GetFloat(material, "_PCSSPCFSampleCount", 16.0f);
                float oldStaticBias = GetFloat(material, "_PCSSMaxStaticGradientBias", 0.05f);

                float sampleCount = Mathf.Clamp(Mathf.Max(oldBlockerSamples, oldPcfSamples), 8.0f, 16.0f);
                float filterRadius = Mathf.Lerp(0.0065f, 0.0145f, Mathf.InverseLerp(0.01f, 0.10f, oldSampleRadius));
                float lightSize = Mathf.Lerp(0.060f, 0.160f, Mathf.Clamp01(oldSoftness));
                float bias = Mathf.Clamp(0.00045f + oldStaticBias * 0.006f, 0.00045f, 0.0012f);

                materialChanged |= SetFloatIfExists(material, "_UsePCSS", oldEnabled > 0.5f ? 1.0f : 0.0f);
                materialChanged |= SetFloatIfExists(material, "_PCSSEnabled", oldEnabled > 0.5f ? 1.0f : 0.0f);
                materialChanged |= SetFloatIfExists(material, "_PCSSPresetMode", 6.0f);
                materialChanged |= SetFloatIfExists(material, "_PCSSQualityLevel", 2.0f);
                materialChanged |= SetFloatIfExists(material, "_LocalPCSSSamples", sampleCount);
                materialChanged |= SetFloatIfExists(material, "_PCSSSamples", sampleCount);
                materialChanged |= SetFloatIfExists(material, "_LocalPCSSFilterRadius", filterRadius);
                materialChanged |= SetFloatIfExists(material, "_PCSSFilterRadius", filterRadius);
                materialChanged |= SetFloatIfExists(material, "_LocalPCSSLightSize", lightSize);
                materialChanged |= SetFloatIfExists(material, "_PCSSLightSize", lightSize);
                materialChanged |= SetFloatIfExists(material, "_LocalPCSSBias", bias);
                materialChanged |= SetFloatIfExists(material, "_PCSSBias", bias);
                materialChanged |= SetFloatIfExists(material, "_PCSSIntensity", Mathf.Clamp(oldIntensity, 0.85f, 1.30f));
                materialChanged |= SetFloatIfExists(material, "_UsePCSSOptimization", 1.0f);
                materialChanged |= SetFloatIfExists(material, "_UseVRChatPerformanceGate", 1.0f);
                materialChanged |= SetFloatIfExists(material, "_PCSSMaxDistance", 10.0f);
                materialChanged |= SetFloatIfExists(material, "_PCSSDistanceFade", 3.0f);
                materialChanged |= SetFloatIfExists(material, "_UseRealisticShadow", 1.0f);
                materialChanged |= SetFloatIfExists(material, "_RealisticShadowSoftness", Mathf.Clamp(oldSoftness, 0.50f, 0.78f));
                materialChanged |= SetFloatIfExists(material, "_RealisticShadowIntensity", 0.78f);
                materialChanged |= SetFloatIfExists(material, "_UseNoLightPCSSBoost", 1.0f);
                materialChanged |= SetFloatIfExists(material, "_NoLightPCSSBoostStrength", 0.72f);
                materialChanged |= SetFloatIfExists(material, "_NoLightPCSSBoostSoftness", 0.70f);
                materialChanged |= SetFloatIfExists(material, "_NoLightPCSSBoostRim", 0.55f);
                materialChanged |= SetColorIfExists(material, "_NoLightPCSSHighlightTint", new Color(0.72f, 0.70f, 0.76f, 1.0f));

                SetKeyword(material, "_USEPCSS_ON", oldEnabled > 0.5f);
                SetKeyword(material, "_USEPCSSOPTIMIZATION_ON", true);
                SetKeyword(material, "_USEVRCHATPERFORMANCEGATE_ON", true);
                SetKeyword(material, "_USEREALISTICSHADOW_ON", true);
                SetKeyword(material, "_USENOLIGHTPCSSBOOST_ON", true);

                if (!materialChanged) continue;
                EditorUtility.SetDirty(material);
                changed++;
            }

            return changed;
        }

        private static int RemoveLegacyRuntimeComponents(GameObject avatarRoot, bool useUndo)
        {
            int removed = 0;
            Component[] components = avatarRoot.GetComponentsInChildren<Component>(true);
            foreach (Component component in components)
            {
                if (component == null) continue;
                Type type = component.GetType();
                string typeName = type.FullName ?? type.Name;
                if (!IsLegacyComponentType(typeName)) continue;

                if (useUndo)
                {
                    Undo.DestroyObjectImmediate(component);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(component);
                }
                removed++;
            }

            return removed;
        }

        private static bool IsLegacyPCSSMaterial(Material material)
        {
            if (material == null) return false;
            string shaderName = material.shader != null ? material.shader.name : string.Empty;
            return shaderName.IndexOf("PCSSLiltoonComplete", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   material.HasProperty("_PCSSBlockerSampleCount") ||
                   material.HasProperty("_PCSSPCFSampleCount") ||
                   material.HasProperty("_PCSSSampleRadius") ||
                   material.HasProperty("_PCSSoftness");
        }

        private static bool IsLegacyComponentType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return false;
            foreach (string legacyTypeName in LegacyComponentTypeNames)
            {
                if (typeName.IndexOf(legacyTypeName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static GameObject ResolveAvatarRoot(GameObject selected)
        {
            if (selected == null) return null;
            Transform current = selected.transform;
            while (current != null)
            {
                if (current.GetComponent<Animator>() != null)
                {
                    return current.gameObject;
                }

                current = current.parent;
            }

            return selected;
        }

        private static float GetFloat(Material material, string propertyName, float fallback)
        {
            return material != null && material.HasProperty(propertyName) ? material.GetFloat(propertyName) : fallback;
        }

        private static bool SetFloatIfExists(Material material, string propertyName, float value)
        {
            if (material == null || !material.HasProperty(propertyName)) return false;
            if (Mathf.Approximately(material.GetFloat(propertyName), value)) return false;
            material.SetFloat(propertyName, value);
            return true;
        }

        private static bool SetColorIfExists(Material material, string propertyName, Color value)
        {
            if (material == null || !material.HasProperty(propertyName)) return false;
            if (material.GetColor(propertyName) == value) return false;
            material.SetColor(propertyName, value);
            return true;
        }

        private static void SetKeyword(Material material, string keyword, bool enabled)
        {
            if (material == null || string.IsNullOrEmpty(keyword)) return;
            if (enabled)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }

        internal struct IntakeSummary
        {
            public int MigratedMaterials;
            public int RemovedLegacyComponents;

            public int TotalChanges => MigratedMaterials + RemovedLegacyComponents;
        }
    }
}
#endif
