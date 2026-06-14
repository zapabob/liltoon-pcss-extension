using System;
using System.Collections.Generic;
using UnityEngine;

#if VRC_SDK_VRCSDK3 || VRCHAT_SDK_AVAILABLE
using VRC.SDK3.Avatars.Components;
#endif

namespace lilToon.PCSS.Runtime
{
    public static class PCSSUtilities
    {
        public static readonly string[] SupportedShaders =
        {
            "lilToon/PCSS Extension",
            "Poiyomi/Toon/PCSS Extension",
            "lilToon Pro/PCSS Extension",
            "Poiyomi Pro/PCSS Extension"
        };

        public enum ShaderType
        {
            Unknown = 0,
            LilToon = 1,
            Poiyomi = 2,
            Both = 3
        }

        public enum PCSSPreset
        {
            Realistic = 0,
            Anime = 1,
            Cinematic = 2,
            Custom = 3,
            DewySkin = 4,
            SoftFlushSkin = 5,
            StudioBoost = 6,
            ExcitedTone = 7
        }

        public enum PCSSQuality
        {
            Low = 0,
            Medium = 1,
            High = 2,
            Ultra = 3
        }

        public static ShaderType DetectAvailableShaders()
        {
            bool hasLilToon = Shader.Find("lilToon") != null || Shader.Find("lilToon/PCSS Extension") != null;
            bool hasPoiyomi = Shader.Find("Poiyomi/Toon") != null || Shader.Find("Poiyomi/Toon/PCSS Extension") != null;

            if (hasLilToon && hasPoiyomi) return ShaderType.Both;
            if (hasLilToon) return ShaderType.LilToon;
            if (hasPoiyomi) return ShaderType.Poiyomi;
            return ShaderType.Unknown;
        }

        public static Vector4 GetPresetParameters(PCSSPreset preset)
        {
            switch (preset)
            {
                case PCSSPreset.Realistic:
                    return new Vector4(0.0065f, 0.060f, 0.00055f, 1.05f);
                case PCSSPreset.Anime:
                    return new Vector4(0.0120f, 0.085f, 0.00120f, 0.88f);
                case PCSSPreset.Cinematic:
                    return new Vector4(0.0200f, 0.180f, 0.00150f, 1.15f);
                case PCSSPreset.DewySkin:
                    return new Vector4(0.0090f, 0.095f, 0.00080f, 0.96f);
                case PCSSPreset.SoftFlushSkin:
                    return new Vector4(0.0105f, 0.105f, 0.00090f, 0.93f);
                case PCSSPreset.StudioBoost:
                    return new Vector4(0.0125f, 0.160f, 0.00065f, 1.30f);
                case PCSSPreset.ExcitedTone:
                    return new Vector4(0.0110f, 0.115f, 0.00085f, 1.02f);
                default:
                    return new Vector4(0.0100f, 0.100f, 0.00100f, 1.00f);
            }
        }

        public static Vector3 GetQualityParameters(PCSSQuality quality)
        {
            switch (quality)
            {
                case PCSSQuality.Low:
                    return new Vector3(4f, 2f, 0.55f);
                case PCSSQuality.Medium:
                    return new Vector3(6f, 3f, 0.75f);
                case PCSSQuality.High:
                    return new Vector3(10f, 5f, 0.95f);
                case PCSSQuality.Ultra:
                    return new Vector3(16f, 8f, 1.10f);
                default:
                    return new Vector3(6f, 3f, 0.75f);
            }
        }

        public static bool IsPCSSCompatible(Material material)
        {
            if (material == null || material.shader == null) return false;

            try
            {
                string shaderName = material.shader.name ?? string.Empty;
                string normalizedShaderName = NormalizeShaderName(shaderName);

                foreach (string supportedShader in SupportedShaders)
                {
                    if (normalizedShaderName.Contains(NormalizeShaderName(supportedShader)))
                    {
                        return IsShaderTypeAvailable(shaderName, DetectAvailableShaders());
                    }
                }

                return material.HasProperty("_UsePCSS")
                    || material.HasProperty("_PCSSEnabled")
                    || material.HasProperty("_LocalPCSSFilterRadius")
                    || material.HasProperty("_PCSSFilterRadius");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[PCSSUtilities] PCSS compatibility check failed: {ex.Message}");
                return false;
            }
        }

        public static List<Material> FindPCSSMaterials(GameObject target)
        {
            List<Material> pcssMaterials = new List<Material>();
            if (target == null) return pcssMaterials;

            Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null || renderer.sharedMaterials == null) continue;

                foreach (Material material in renderer.sharedMaterials)
                {
                    if (IsPCSSCompatible(material) && !pcssMaterials.Contains(material))
                    {
                        pcssMaterials.Add(material);
                    }
                }
            }

            return pcssMaterials;
        }

        public static void ApplyPresetToMaterial(Material material, PCSSPreset preset, Vector4? customParams = null)
        {
            if (!IsPCSSCompatible(material)) return;

            Vector4 parameters = customParams ?? GetPresetParameters(preset);
            PCSSQuality quality = preset == PCSSPreset.Cinematic ? PCSSQuality.High : PCSSQuality.Medium;
            if (preset == PCSSPreset.Realistic) quality = PCSSQuality.High;
            if (preset == PCSSPreset.DewySkin) quality = PCSSQuality.High;
            if (preset == PCSSPreset.SoftFlushSkin) quality = PCSSQuality.High;
            if (preset == PCSSPreset.StudioBoost) quality = PCSSQuality.Ultra;
            if (preset == PCSSPreset.ExcitedTone) quality = PCSSQuality.High;
            Vector3 qualityParams = GetQualityParameters(quality);

            SetFloatIfExists(material, "_PCSSPresetMode", (float)preset);
            SetFloatIfExists(material, "_PCSSQualityLevel", preset == PCSSPreset.Anime ? 1f : 2f);

            SetFloatIfExists(material, "_LocalPCSSFilterRadius", parameters.x);
            SetFloatIfExists(material, "_PCSSFilterRadius", parameters.x);
            SetFloatIfExists(material, "_LocalPCSSLightSize", parameters.y);
            SetFloatIfExists(material, "_PCSSLightSize", parameters.y);
            SetFloatIfExists(material, "_LocalPCSSBias", parameters.z);
            SetFloatIfExists(material, "_PCSSBias", parameters.z);
            SetFloatIfExists(material, "_PCSSIntensity", parameters.w);
            SetFloatIfExists(material, "_LocalPCSSSamples", qualityParams.x);
            SetFloatIfExists(material, "_PCSSSamples", qualityParams.x);
            SetFloatIfExists(material, "_BlockerSamples", qualityParams.y);
            SetFloatIfExists(material, "_FilterScale", qualityParams.z);

            SetFloatIfExists(material, "_UsePCSS", 1f);
            SetFloatIfExists(material, "_PCSSEnabled", 1f);
            SetFloatIfExists(material, "_UsePCSSOptimization", 1f);
            SetFloatIfExists(material, "_PCSSOptimizationLevel", preset == PCSSPreset.StudioBoost ? 0f : 1f);
            SetFloatIfExists(material, "_UseVRChatPerformanceGate", 1f);
            SetFloatIfExists(material, "_PCSSMaxDistance", 10f);
            SetFloatIfExists(material, "_PCSSDistanceFade", preset == PCSSPreset.Anime ? 2.4f : preset == PCSSPreset.StudioBoost ? 3.2f : preset == PCSSPreset.DewySkin ? 3.0f : preset == PCSSPreset.SoftFlushSkin ? 2.8f : preset == PCSSPreset.ExcitedTone ? 3.0f : 3.0f);

            ApplyGlossPreset(material, preset);
            ApplyRealisticShadowPreset(material, preset);
            ApplyNoLightBoostPreset(material, preset);
            UpdateKeywords(material);
        }

        public static int ApplyPresetToAvatar(GameObject avatar, PCSSPreset preset, Vector4? customParams = null)
        {
            if (avatar == null) return 0;

            List<Material> pcssMaterials = FindPCSSMaterials(avatar);
            foreach (Material material in pcssMaterials)
            {
                ApplyPresetToMaterial(material, preset, customParams);
            }

            TryTagModularAvatarObject(avatar, $"PCSS_{preset}_Applied");
            return pcssMaterials.Count;
        }

        public static bool IsVRChatAvatar(GameObject gameObject)
        {
#if VRC_SDK_VRCSDK3 || VRCHAT_SDK_AVAILABLE
            return gameObject != null && gameObject.GetComponent<VRCAvatarDescriptor>() != null;
#else
            return false;
#endif
        }

        public static bool HasModularAvatar(GameObject gameObject)
        {
            if (gameObject == null) return false;

            try
            {
                Type modularAvatarType = Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarInformation, ModularAvatar.Core");
                return modularAvatarType != null && gameObject.GetComponent(modularAvatarType) != null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[PCSSUtilities] Modular Avatar check failed: {ex.Message}");
                return false;
            }
        }

        public static Component AddPCSSModularAvatarComponent(GameObject avatar, PCSSPreset preset)
        {
            if (avatar == null) return null;

            try
            {
                Type modularAvatarType = Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarInformation, ModularAvatar.Core");
                if (modularAvatarType == null) return null;

                Component component = avatar.GetComponent(modularAvatarType);
                if (component == null)
                {
                    component = avatar.AddComponent(modularAvatarType);
                }

                component.name = $"lilToon_PCSS_{preset}_Component";
                return component;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[PCSSUtilities] Failed to add Modular Avatar component: {ex.Message}");
                return null;
            }
        }

        public static PCSSPerformanceStats GetPerformanceStats(GameObject avatar)
        {
            PCSSPerformanceStats stats = new PCSSPerformanceStats();
            if (avatar == null) return stats;

            stats.PCSSMaterialCount = FindPCSSMaterials(avatar).Count;

            Renderer[] renderers = avatar.GetComponentsInChildren<Renderer>(true);
            stats.TotalRendererCount = renderers.Length;

            foreach (Renderer renderer in renderers)
            {
                Mesh mesh = GetRendererMesh(renderer);
                if (mesh == null) continue;

                stats.TotalVertexCount += mesh.vertexCount;
                stats.TotalTriangleCount += mesh.triangles.Length / 3;
            }

            return stats;
        }

        private static void ApplyGlossPreset(Material material, PCSSPreset preset)
        {
            float coherence = 0.55f;
            float boost = 0.35f;
            float suppression = 0.45f;
            float rim = 0.35f;
            float smoothness = 0.72f;

            if (preset == PCSSPreset.Anime)
            {
                coherence = 0.48f;
                boost = 0.25f;
                suppression = 0.35f;
                rim = 0.25f;
                smoothness = 0.64f;
            }
            else if (preset == PCSSPreset.Cinematic)
            {
                coherence = 0.72f;
                boost = 0.55f;
                suppression = 0.55f;
                rim = 0.55f;
                smoothness = 0.82f;
            }
            else if (preset == PCSSPreset.DewySkin)
            {
                coherence = 0.76f;
                boost = 0.66f;
                suppression = 0.40f;
                rim = 0.48f;
                smoothness = 0.88f;
            }
            else if (preset == PCSSPreset.SoftFlushSkin)
            {
                coherence = 0.68f;
                boost = 0.42f;
                suppression = 0.46f;
                rim = 0.34f;
                smoothness = 0.78f;
            }
            else if (preset == PCSSPreset.StudioBoost)
            {
                coherence = 0.86f;
                boost = 0.88f;
                suppression = 0.30f;
                rim = 0.72f;
                smoothness = 0.92f;
            }
            else if (preset == PCSSPreset.ExcitedTone)
            {
                coherence = 0.72f;
                boost = 0.50f;
                suppression = 0.42f;
                rim = 0.42f;
                smoothness = 0.82f;
            }

            SetFloatIfExists(material, "_UseGlossShadowCoherence", 1f);
            SetFloatIfExists(material, "_GlossShadowCoherence", coherence);
            SetFloatIfExists(material, "_GlossShadowBoost", boost);
            SetFloatIfExists(material, "_GlossShadowSuppression", suppression);
            SetFloatIfExists(material, "_GlossRimStrength", rim);
            SetFloatIfExists(material, "_GlossSmoothness", smoothness);
            if (preset == PCSSPreset.DewySkin)
            {
                SetFloatIfExists(material, "_Translucency", 0.55f);
            }
            if (preset == PCSSPreset.SoftFlushSkin)
            {
                SetFloatIfExists(material, "_Translucency", 0.50f);
                SetFloatIfExists(material, "_UseSoftFlush", 1f);
                SetColorIfExists(material, "_SoftFlushColor", new Color(1.0f, 0.40f, 0.36f, 1.0f));
                SetFloatIfExists(material, "_SoftFlushStrength", 0.34f);
                SetFloatIfExists(material, "_SoftFlushWidth", 0.56f);
                SetFloatIfExists(material, "_SoftFlushVerticalBias", 0.46f);
                SetColorIfExists(material, "_RealisticShadowColor", new Color(0.23f, 0.13f, 0.14f, 0.76f));
                SetFloatIfExists(material, "_UseRimShade", 1f);
                SetColorIfExists(material, "_RimShadeColor", new Color(1.0f, 0.45f, 0.40f, 1.0f));
                SetFloatIfExists(material, "_RimShadeIntensity", 0.06f);
                SetFloatIfExists(material, "_RimShadeWidth", 0.78f);
            }
            if (preset == PCSSPreset.ExcitedTone)
            {
                SetFloatIfExists(material, "_Translucency", 0.52f);
                SetFloatIfExists(material, "_UseSoftFlush", 1f);
                SetColorIfExists(material, "_SoftFlushColor", new Color(1.0f, 0.40f, 0.34f, 1.0f));
                SetFloatIfExists(material, "_SoftFlushStrength", 0.26f);
                SetFloatIfExists(material, "_SoftFlushWidth", 0.60f);
                SetFloatIfExists(material, "_SoftFlushVerticalBias", 0.48f);
                SetFloatIfExists(material, "_UseExcitedTone", 1f);
                SetColorIfExists(material, "_ExcitedToneColor", new Color(1.0f, 0.48f, 0.34f, 1.0f));
                SetFloatIfExists(material, "_ExcitedToneStrength", 0.28f);
                SetFloatIfExists(material, "_ExcitedToneBreath", 0.0f);
                SetFloatIfExists(material, "_ExcitedToneUpperBias", 0.58f);
                SetColorIfExists(material, "_RealisticShadowColor", new Color(0.24f, 0.12f, 0.12f, 0.74f));
                SetFloatIfExists(material, "_UseRimShade", 1f);
                SetColorIfExists(material, "_RimShadeColor", new Color(1.0f, 0.52f, 0.42f, 1.0f));
                SetFloatIfExists(material, "_RimShadeIntensity", 0.10f);
                SetFloatIfExists(material, "_RimShadeWidth", 0.74f);
            }
            if (preset == PCSSPreset.StudioBoost)
            {
                SetFloatIfExists(material, "_Translucency", 0.48f);
                SetFloatIfExists(material, "_UseRimShade", 1f);
                SetColorIfExists(material, "_RimShadeColor", new Color(0.95f, 0.92f, 1.0f, 1.0f));
                SetFloatIfExists(material, "_RimShadeIntensity", 0.16f);
                SetFloatIfExists(material, "_RimShadeWidth", 0.64f);
            }
        }

        private static void ApplyRealisticShadowPreset(Material material, PCSSPreset preset)
        {
            SetFloatIfExists(material, "_UseRealisticShadow", 1f);
            SetFloatIfExists(material, "_RealisticShadowIntensity", preset == PCSSPreset.Anime ? 0.78f : preset == PCSSPreset.StudioBoost ? 0.78f : preset == PCSSPreset.DewySkin ? 0.62f : preset == PCSSPreset.SoftFlushSkin ? 0.58f : preset == PCSSPreset.ExcitedTone ? 0.60f : 0.92f);
            SetFloatIfExists(material, "_RealisticShadowSoftness", preset == PCSSPreset.Cinematic ? 0.70f : preset == PCSSPreset.StudioBoost ? 0.62f : preset == PCSSPreset.DewySkin ? 0.68f : preset == PCSSPreset.SoftFlushSkin ? 0.74f : preset == PCSSPreset.ExcitedTone ? 0.72f : 0.55f);
            SetFloatIfExists(material, "_UseShadow", 1f);
            SetFloatIfExists(material, "_UseShadow2", 1f);
            SetFloatIfExists(material, "_UseShadow3", preset == PCSSPreset.Anime ? 0f : 1f);
        }

        private static void ApplyNoLightBoostPreset(Material material, PCSSPreset preset)
        {
            bool enabled = preset == PCSSPreset.DewySkin || preset == PCSSPreset.SoftFlushSkin || preset == PCSSPreset.StudioBoost || preset == PCSSPreset.ExcitedTone;
            SetFloatIfExists(material, "_UseLightDirectionOverride", enabled ? 1f : 0f);
            SetVectorIfExists(material, "_LightDirectionOverride", preset == PCSSPreset.StudioBoost
                ? new Vector4(0.28f, 0.82f, 0.50f, 0f)
                : preset == PCSSPreset.ExcitedTone
                    ? new Vector4(0.20f, 0.86f, 0.44f, 0f)
                : preset == PCSSPreset.SoftFlushSkin
                    ? new Vector4(0.18f, 0.88f, 0.42f, 0f)
                    : new Vector4(0.22f, 0.86f, 0.46f, 0f));
            SetFloatIfExists(material, "_UseNoLightPCSSBoost", enabled ? 1f : 0f);
            SetFloatIfExists(material, "_NoLightPCSSBoostStrength", preset == PCSSPreset.StudioBoost ? 0.72f : preset == PCSSPreset.ExcitedTone ? 0.56f : preset == PCSSPreset.SoftFlushSkin ? 0.52f : 0.48f);
            SetFloatIfExists(material, "_NoLightPCSSBoostSoftness", preset == PCSSPreset.StudioBoost ? 0.70f : 0.66f);
            SetFloatIfExists(material, "_NoLightPCSSBoostRim", preset == PCSSPreset.StudioBoost ? 0.55f : preset == PCSSPreset.ExcitedTone ? 0.40f : preset == PCSSPreset.DewySkin ? 0.42f : 0.32f);
            SetColorIfExists(material, "_NoLightPCSSHighlightTint", preset == PCSSPreset.StudioBoost
                ? new Color(0.72f, 0.70f, 0.76f, 1f)
                : preset == PCSSPreset.ExcitedTone
                    ? new Color(0.64f, 0.50f, 0.46f, 1f)
                : new Color(0.58f, 0.52f, 0.50f, 1f));
        }

        private static void UpdateKeywords(Material material)
        {
            if (material == null) return;

            SetKeyword(material, "_USEPCSS_ON", GetFloat(material, "_UsePCSS", "_PCSSEnabled") > 0.5f);
            SetKeyword(material, "_USEPCSSOPTIMIZATION_ON", GetFloat(material, "_UsePCSSOptimization") > 0.5f);
            SetKeyword(material, "_USEVRCHATPERFORMANCEGATE_ON", GetFloat(material, "_UseVRChatPerformanceGate") > 0.5f);
            SetKeyword(material, "_USEGLOSSSHADOWCOHERENCE_ON", GetFloat(material, "_UseGlossShadowCoherence") > 0.5f);
            SetKeyword(material, "_USEREALISTICSHADOW_ON", GetFloat(material, "_UseRealisticShadow") > 0.5f);
            SetKeyword(material, "_USENOLIGHTPCSSBOOST_ON", GetFloat(material, "_UseNoLightPCSSBoost") > 0.5f);
            SetKeyword(material, "_USESOFTFLUSH_ON", GetFloat(material, "_UseSoftFlush") > 0.5f);
            SetKeyword(material, "_USEEXCITEDTONE_ON", GetFloat(material, "_UseExcitedTone") > 0.5f);
            SetKeyword(material, "_USERIMSHADE_ON", GetFloat(material, "_UseRimShade") > 0.5f);
            SetKeyword(material, "_USESHADOW_ON", GetFloat(material, "_UseShadow") > 0.5f);
            SetKeyword(material, "_USESHADOW2_ON", GetFloat(material, "_UseShadow2") > 0.5f);
            SetKeyword(material, "_USESHADOW3_ON", GetFloat(material, "_UseShadow3") > 0.5f);
        }

        private static bool IsShaderTypeAvailable(string shaderName, ShaderType availableShaders)
        {
            string lowerName = shaderName.ToLowerInvariant();
            bool isLilToon = lowerName.Contains("liltoon") || lowerName.Contains("lil");
            bool isPoiyomi = lowerName.Contains("poiyomi") || lowerName.Contains("poi");

            switch (availableShaders)
            {
                case ShaderType.LilToon:
                    return isLilToon;
                case ShaderType.Poiyomi:
                    return isPoiyomi;
                case ShaderType.Both:
                    return isLilToon || isPoiyomi;
                default:
                    return true;
            }
        }

        private static string NormalizeShaderName(string shaderName)
        {
            return (shaderName ?? string.Empty)
                .Replace("/", string.Empty)
                .Replace(" ", string.Empty)
                .Replace("_", string.Empty)
                .Replace("-", string.Empty)
                .ToLowerInvariant();
        }

        private static Mesh GetRendererMesh(Renderer renderer)
        {
            MeshRenderer meshRenderer = renderer as MeshRenderer;
            MeshFilter meshFilter = meshRenderer != null ? meshRenderer.GetComponent<MeshFilter>() : null;
            if (meshFilter != null) return meshFilter.sharedMesh;

            SkinnedMeshRenderer skinned = renderer as SkinnedMeshRenderer;
            return skinned != null ? skinned.sharedMesh : null;
        }

        private static void TryTagModularAvatarObject(GameObject avatar, string componentName)
        {
            try
            {
                Type modularAvatarType = Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarInformation, ModularAvatar.Core");
                if (modularAvatarType == null) return;

                Component component = avatar.GetComponent(modularAvatarType);
                if (component != null)
                {
                    component.name = componentName;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[PCSSUtilities] Failed to tag Modular Avatar object: {ex.Message}");
            }
        }

        private static float GetFloat(Material material, params string[] names)
        {
            foreach (string name in names)
            {
                if (material.HasProperty(name))
                {
                    return material.GetFloat(name);
                }
            }

            return 0f;
        }

        private static void SetFloatIfExists(Material material, string name, float value)
        {
            if (material != null && material.HasProperty(name))
            {
                material.SetFloat(name, value);
            }
        }

        private static void SetColorIfExists(Material material, string name, Color value)
        {
            if (material != null && material.HasProperty(name))
            {
                material.SetColor(name, value);
            }
        }

        private static void SetVectorIfExists(Material material, string name, Vector4 value)
        {
            if (material != null && material.HasProperty(name))
            {
                material.SetVector(name, value);
            }
        }

        private static void SetKeyword(Material material, string keyword, bool enabled)
        {
            if (enabled)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }
    }

    [Serializable]
    public class PCSSPerformanceStats
    {
        public int PCSSMaterialCount;
        public int TotalRendererCount;
        public int TotalVertexCount;
        public int TotalTriangleCount;

        public string GetSummary()
        {
            return $"PCSS Materials: {PCSSMaterialCount}, Renderers: {TotalRendererCount}, Vertices: {TotalVertexCount:N0}, Triangles: {TotalTriangleCount:N0}";
        }
    }
}
