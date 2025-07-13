using UnityEngine;
using System.Collections.Generic;

// ModularAvatar萓晏ｭ倬未菫ゅｒ蜑企勁縺励√Μ繝輔Ξ繧ｯ繧ｷ繝ｧ繝ｳ繝吶・繧ｹ縺ｮ螳溯｣・↓螟画峩

#if VRCHAT_SDK_AVAILABLE
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
#endif

namespace lilToon.PCSS.Runtime
{
    /// <summary>
    /// PCSS髢｢騾｣縺ｮ繝ｦ繝ｼ繝・ぅ繝ｪ繝・ぅ讖溯・繧呈署萓帙☆繧九け繝ｩ繧ｹ
    /// </summary>
    public static class PCSSUtilities
    {
        /// <summary>
        /// PCSS蟇ｾ蠢懊す繧ｧ繝ｼ繝繝ｼ縺ｮ荳隕ｧ
        /// </summary>
        public static readonly string[] SupportedShaders = {
            "lilToon/PCSS Extension",
            "Poiyomi/Toon/PCSS Extension",
            "lilToon Pro/PCSS Extension",
            "Poiyomi Pro/PCSS Extension"
        };

        /// <summary>
        /// 蛻ｩ逕ｨ蜿ｯ閭ｽ縺ｪ繧ｷ繧ｧ繝ｼ繝繝ｼ繧ｿ繧､繝・
        /// </summary>
        public enum ShaderType
        {
            Unknown = 0,
            LilToon = 1,
            Poiyomi = 2,
            Both = 3
        }

        /// <summary>
        /// 迴ｾ蝨ｨ蛻ｩ逕ｨ蜿ｯ閭ｽ縺ｪ繧ｷ繧ｧ繝ｼ繝繝ｼ繧ｿ繧､繝励ｒ讀懷・
        /// </summary>
        public static ShaderType DetectAvailableShaders()
        {
            bool hasLilToon = false;
            bool hasPoiyomi = false;

            // lilToon讀懷・
            var lilToonShader = Shader.Find("lilToon");
            var lilToonPCSSShader = Shader.Find("lilToon/PCSS Extension");
            hasLilToon = (lilToonShader != null || lilToonPCSSShader != null);

            // Poiyomi讀懷・
            var poiyomiShader = Shader.Find("Poiyomi/Toon");
            var poiyomiPCSSShader = Shader.Find("Poiyomi/Toon/PCSS Extension");
            hasPoiyomi = (poiyomiShader != null || poiyomiPCSSShader != null);

            if (hasLilToon && hasPoiyomi)
                return ShaderType.Both;
            else if (hasLilToon)
                return ShaderType.LilToon;
            else if (hasPoiyomi)
                return ShaderType.Poiyomi;
            else
                return ShaderType.Unknown;
        }

        /// <summary>
        /// 繝励Μ繧ｻ繝・ヨ螳夂ｾｩ
        /// </summary>
        public enum PCSSPreset
        {
            Realistic = 0,    // 繝ｪ繧｢繝ｫ蠖ｱ
            Anime = 1,        // 繧｢繝九Γ鬚ｨ
            Cinematic = 2,    // 譏逕ｻ鬚ｨ
            Custom = 3        // 繧ｫ繧ｹ繧ｿ繝
        }

        /// <summary>
        /// PCSS蜩∬ｳｪ險ｭ螳・
        /// </summary>
        public enum PCSSQuality
        {
            Low = 0,          // 菴主刀雉ｪ (繝｢繝舌う繝ｫ蜷代￠)
            Medium = 1,       // 荳ｭ蜩∬ｳｪ (讓呎ｺ・
            High = 2,         // 鬮伜刀雉ｪ (PC蜷代￠)
            Ultra = 3         // 譛鬮伜刀雉ｪ (繝上う繧ｨ繝ｳ繝臼C蜷代￠)
        }

        /// <summary>
        /// 繝励Μ繧ｻ繝・ヨ繝代Λ繝｡繝ｼ繧ｿ繧貞叙蠕・
        /// </summary>
        public static Vector4 GetPresetParameters(PCSSPreset preset)
        {
            switch (preset)
            {
                case PCSSPreset.Realistic:
                    return new Vector4(0.005f, 0.05f, 0.0005f, 1.0f);
                case PCSSPreset.Anime:
                    return new Vector4(0.015f, 0.1f, 0.001f, 0.8f);
                case PCSSPreset.Cinematic:
                    return new Vector4(0.025f, 0.2f, 0.002f, 1.2f);
                default:
                    return new Vector4(0.01f, 0.1f, 0.001f, 1.0f);
            }
        }

        /// <summary>
        /// 蜩∬ｳｪ險ｭ螳壹ヱ繝ｩ繝｡繝ｼ繧ｿ繧貞叙蠕・
        /// </summary>
        public static Vector3 GetQualityParameters(PCSSQuality quality)
        {
            switch (quality)
            {
                case PCSSQuality.Low:
                    return new Vector3(8, 4, 0.5f);    // SampleCount, BlockerSamples, FilterScale
                case PCSSQuality.Medium:
                    return new Vector3(16, 8, 1.0f);   // SampleCount, BlockerSamples, FilterScale
                case PCSSQuality.High:
                    return new Vector3(32, 16, 1.5f);  // SampleCount, BlockerSamples, FilterScale
                case PCSSQuality.Ultra:
                    return new Vector3(64, 32, 2.0f);  // SampleCount, BlockerSamples, FilterScale
                default:
                    return new Vector3(16, 8, 1.0f);   // Default to Medium
            }
        }

        /// <summary>
        /// 繝槭ユ繝ｪ繧｢繝ｫ縺訓CSS蟇ｾ蠢懊°縺ｩ縺・°繧偵メ繧ｧ繝・け・医お繝ｩ繝ｼ閠先ｧ莉倥″・・
        /// </summary>
        public static bool IsPCSSCompatible(Material material)
        {
            if (material == null || material.shader == null) return false;

            try
            {
                string shaderName = material.shader.name;
                
                // 蛻ｩ逕ｨ蜿ｯ閭ｽ縺ｪ繧ｷ繧ｧ繝ｼ繝繝ｼ繧ｿ繧､繝励ｒ讀懷・
                ShaderType availableShaders = DetectAvailableShaders();
                
                // 繧ｷ繧ｧ繝ｼ繝繝ｼ繧ｿ繧､繝励↓蠢懊§縺滓沐霆溘↑蛻､螳・
                foreach (string supportedShader in SupportedShaders)
                {
                    if (shaderName.Contains(supportedShader.Replace("/", "").Replace(" ", "")))
                    {
                        // 繧ｷ繧ｧ繝ｼ繝繝ｼ縺悟ｮ滄圀縺ｫ蛻ｩ逕ｨ蜿ｯ閭ｽ縺九メ繧ｧ繝・け
                        if (IsShaderTypeAvailable(shaderName, availableShaders))
                        {
                            return true;
                        }
                    }
                }
                
                return false;
            }
            catch (System.Exception ex)
            {
                // 繧ｨ繝ｩ繝ｼ縺檎匱逕溘＠縺溷ｴ蜷医・false繧定ｿ斐＠縺ｦ繝ｭ繧ｰ蜃ｺ蜉・
                Debug.LogWarning($"[PCSSUtilities] Error checking PCSS compatibility: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 謖・ｮ壹＆繧後◆繧ｷ繧ｧ繝ｼ繝繝ｼ縺悟茜逕ｨ蜿ｯ閭ｽ縺ｪ繧ｿ繧､繝励↓蜷ｫ縺ｾ繧後ｋ縺九メ繧ｧ繝・け
        /// </summary>
        private static bool IsShaderTypeAvailable(string shaderName, ShaderType availableShaders)
        {
            bool isLilToon = shaderName.Contains("lilToon") || shaderName.Contains("lil");
            bool isPoiyomi = shaderName.Contains("Poiyomi") || shaderName.Contains("poi");

            switch (availableShaders)
            {
                case ShaderType.LilToon:
                    return isLilToon;
                case ShaderType.Poiyomi:
                    return isPoiyomi;
                case ShaderType.Both:
                    return isLilToon || isPoiyomi;
                case ShaderType.Unknown:
                default:
                    // 荳肴・縺ｪ蝣ｴ蜷医・蟇帛ｮｹ縺ｫ蛻､螳夲ｼ医せ繧ｿ繝ｳ繝峨い繝ｭ繝ｳ繝｢繝ｼ繝会ｼ・
                    return true;
            }
        }

        /// <summary>
        /// GameObject縺九ｉPCSS蟇ｾ蠢懊・繝・Μ繧｢繝ｫ繧呈､懃ｴ｢
        /// </summary>
        public static List<Material> FindPCSSMaterials(GameObject target)
        {
            List<Material> pcssMaterials = new List<Material>();
            
            if (target == null) return pcssMaterials;

            Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (IsPCSSCompatible(material))
                    {
                        pcssMaterials.Add(material);
                    }
                }
            }

            return pcssMaterials;
        }

        /// <summary>
        /// 繝励Μ繧ｻ繝・ヨ繧偵・繝・Μ繧｢繝ｫ縺ｫ驕ｩ逕ｨ
        /// </summary>
        public static void ApplyPresetToMaterial(Material material, PCSSPreset preset, Vector4? customParams = null)
        {
            if (!IsPCSSCompatible(material)) return;

            Vector4 parameters = customParams ?? GetPresetParameters(preset);

            // 繝励Μ繧ｻ繝・ヨ繝｢繝ｼ繝峨ｒ險ｭ螳・
            if (material.HasProperty("_PCSSPresetMode"))
            {
                material.SetFloat("_PCSSPresetMode", (int)preset);
            }

            // 蛟句挨繝代Λ繝｡繝ｼ繧ｿ繧定ｨｭ螳・
            if (material.HasProperty("_PCSSFilterRadius"))
                material.SetFloat("_PCSSFilterRadius", parameters.x);
            
            if (material.HasProperty("_PCSSLightSize"))
                material.SetFloat("_PCSSLightSize", parameters.y);
            
            if (material.HasProperty("_PCSSBias"))
                material.SetFloat("_PCSSBias", parameters.z);
            
            if (material.HasProperty("_PCSSIntensity"))
                material.SetFloat("_PCSSIntensity", parameters.w);

            // PCSS讖溯・繧呈怏蜉ｹ蛹・
            if (material.HasProperty("_PCSSEnabled"))
                material.SetFloat("_PCSSEnabled", 1.0f);
        }

        /// <summary>
        /// 繧｢繝舌ち繝ｼ蜈ｨ菴薙↓繝励Μ繧ｻ繝・ヨ繧帝←逕ｨ
        /// </summary>
        public static int ApplyPresetToAvatar(GameObject avatar, PCSSPreset preset, Vector4? customParams = null)
        {
            if (avatar == null) return 0;

            var pcssMaterials = FindPCSSMaterials(avatar);
            foreach (var material in pcssMaterials)
            {
                ApplyPresetToMaterial(material, preset, customParams);
            }

            // ModularAvatar繧ｳ繝ｳ繝昴・繝阪Φ繝医′蟄伜惠縺吶ｋ蝣ｴ蜷医∵ュ蝣ｱ繧定ｨ倬鹸・医Μ繝輔Ξ繧ｯ繧ｷ繝ｧ繝ｳ菴ｿ逕ｨ・・
            try
            {
                var modularAvatarType = System.Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarInformation, ModularAvatar.Core");
                if (modularAvatarType != null)
                {
                    var maInfo = avatar.GetComponent(modularAvatarType);
                    if (maInfo != null)
                    {
                        // 繝励Μ繧ｻ繝・ヨ諠・ｱ繧偵さ繝ｳ繝昴・繝阪Φ繝亥錐縺ｫ險倬鹸
                        maInfo.name = $"PCSS_{preset}_Applied";
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[PCSSUtilities] Failed to update ModularAvatar info: {ex.Message}");
            }

            return pcssMaterials.Count;
        }

        /// <summary>
        /// VRChat繧｢繝舌ち繝ｼ縺九←縺・°繧偵メ繧ｧ繝・け
        /// </summary>
        public static bool IsVRChatAvatar(GameObject gameObject)
        {
            return gameObject.GetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>() != null;
        }

        /// <summary>
        /// ModularAvatar繧ｳ繝ｳ繝昴・繝阪Φ繝医′蟄伜惠縺吶ｋ縺九メ繧ｧ繝・け・医Μ繝輔Ξ繧ｯ繧ｷ繝ｧ繝ｳ菴ｿ逕ｨ・・
        /// </summary>
        public static bool HasModularAvatar(GameObject gameObject)
        {
            if (gameObject == null) return false;

            try
            {
                // 繝ｪ繝輔Ξ繧ｯ繧ｷ繝ｧ繝ｳ繧剃ｽｿ逕ｨ縺励※ModularAvatar縺ｮ蟄伜惠繧偵メ繧ｧ繝・け
                var modularAvatarType = System.Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarInformation, ModularAvatar.Core");
                if (modularAvatarType != null)
                {
                    var component = gameObject.GetComponent(modularAvatarType);
                    return component != null;
                }
                return false;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[PCSSUtilities] ModularAvatar check failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// ModularAvatar繧剃ｽｿ逕ｨ縺励※PCSS繧ｳ繝ｳ繝昴・繝阪Φ繝医ｒ霑ｽ蜉・医Μ繝輔Ξ繧ｯ繧ｷ繝ｧ繝ｳ菴ｿ逕ｨ・・
        /// </summary>
        public static UnityEngine.Component AddPCSSModularAvatarComponent(GameObject avatar, PCSSPreset preset)
        {
            if (avatar == null) return null;

            try
            {
                // 繝ｪ繝輔Ξ繧ｯ繧ｷ繝ｧ繝ｳ繧剃ｽｿ逕ｨ縺励※ModularAvatar繧ｳ繝ｳ繝昴・繝阪Φ繝医ｒ霑ｽ蜉
                var modularAvatarType = System.Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarInformation, ModularAvatar.Core");
                if (modularAvatarType != null)
                {
                    var maInfo = avatar.GetComponent(modularAvatarType);
                    if (maInfo == null)
                    {
                        maInfo = avatar.AddComponent(modularAvatarType);
                    }
                    
                    maInfo.name = $"lilToon_PCSS_{preset}_Component";
                    return maInfo;
                }
                return null;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[PCSSUtilities] Failed to add ModularAvatar component: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 繝代ヵ繧ｩ繝ｼ繝槭Φ繧ｹ邨ｱ險医ｒ蜿門ｾ・
        /// </summary>
        public static PCSSPerformanceStats GetPerformanceStats(GameObject avatar)
        {
            var stats = new PCSSPerformanceStats();
            
            if (avatar == null) return stats;

            var pcssMaterials = FindPCSSMaterials(avatar);
            stats.PCSSMaterialCount = pcssMaterials.Count;
            
            var renderers = avatar.GetComponentsInChildren<Renderer>();
            stats.TotalRendererCount = renderers.Length;
            
            int totalVertices = 0;
            int totalTriangles = 0;
            
            foreach (var renderer in renderers)
            {
                if (renderer is MeshRenderer meshRenderer)
                {
                    var meshFilter = meshRenderer.GetComponent<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        totalVertices += meshFilter.sharedMesh.vertexCount;
                        totalTriangles += meshFilter.sharedMesh.triangles.Length / 3;
                    }
                }
                else if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    if (skinnedMeshRenderer.sharedMesh != null)
                    {
                        totalVertices += skinnedMeshRenderer.sharedMesh.vertexCount;
                        totalTriangles += skinnedMeshRenderer.sharedMesh.triangles.Length / 3;
                    }
                }
            }
            
            stats.TotalVertexCount = totalVertices;
            stats.TotalTriangleCount = totalTriangles;
            
            return stats;
        }
    }

    /// <summary>
    /// PCSS 繝代ヵ繧ｩ繝ｼ繝槭Φ繧ｹ邨ｱ險域ュ蝣ｱ
    /// </summary>
    [System.Serializable]
    public class PCSSPerformanceStats
    {
        public int PCSSMaterialCount;
        public int TotalRendererCount;
        public int TotalVertexCount;
        public int TotalTriangleCount;
        
        public string GetSummary()
        {
            return $"PCSS Materials: {PCSSMaterialCount}, " +
                   $"Renderers: {TotalRendererCount}, " +
                   $"Vertices: {TotalVertexCount:N0}, " +
                   $"Triangles: {TotalTriangleCount:N0}";
        }
    }
} 
