using UnityEngine;
using System.Collections.Generic;

// ModularAvatar依存関係を削除し、リフレクションベ�Eスの実裁E��変更

#if VRCHAT_SDK_AVAILABLE
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
#endif

namespace lilToon.PCSS.Runtime
{
    /// <summary>
    /// PCSS関連のユーチE��リチE��機�Eを提供するクラス
    /// </summary>
    public static class PCSSUtilities
    {
        /// <summary>
        /// PCSS対応シェーダーの一覧
        /// </summary>
        public static readonly string[] SupportedShaders = {
            "lilToon/PCSS Extension",
            "Poiyomi/Toon/PCSS Extension",
            "lilToon Pro/PCSS Extension",
            "Poiyomi Pro/PCSS Extension"
        };

        /// <summary>
        /// 利用可能なシェーダータイチE
        /// </summary>
        public enum ShaderType
        {
            Unknown = 0,
            LilToon = 1,
            Poiyomi = 2,
            Both = 3
        }

        /// <summary>
        /// 現在利用可能なシェーダータイプを検�E
        /// </summary>
        public static ShaderType DetectAvailableShaders()
        {
            bool hasLilToon = false;
            bool hasPoiyomi = false;

            // lilToon検�E
            var lilToonShader = Shader.Find("lilToon");
            var lilToonPCSSShader = Shader.Find("lilToon/PCSS Extension");
            hasLilToon = (lilToonShader != null || lilToonPCSSShader != null);

            // Poiyomi検�E
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
        /// プリセチE��定義
        /// </summary>
        public enum PCSSPreset
        {
            Realistic = 0,    // リアル影
            Anime = 1,        // アニメ風
            Cinematic = 2,    // 映画風
            Custom = 3        // カスタム
        }

        /// <summary>
        /// PCSS品質設宁E
        /// </summary>
        public enum PCSSQuality
        {
            Low = 0,          // 低品質 (モバイル向け)
            Medium = 1,       // 中品質 (標溁E
            High = 2,         // 高品質 (PC向け)
            Ultra = 3         // 最高品質 (ハイエンドPC向け)
        }

        /// <summary>
        /// プリセチE��パラメータを取征E
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
        /// 品質設定パラメータを取征E
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
        /// マテリアルがPCSS対応かどぁE��をチェチE���E�エラー耐性付き�E�E
        /// </summary>
        public static bool IsPCSSCompatible(Material material)
        {
            if (material == null || material.shader == null) return false;

            try
            {
                string shaderName = material.shader.name;
                
                // 利用可能なシェーダータイプを検�E
                ShaderType availableShaders = DetectAvailableShaders();
                
                // シェーダータイプに応じた柔軟な判宁E
                foreach (string supportedShader in SupportedShaders)
                {
                    if (shaderName.Contains(supportedShader.Replace("/", "").Replace(" ", "")))
                    {
                        // シェーダーが実際に利用可能かチェチE��
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
                // エラーが発生した場合�Efalseを返してログ出劁E
                Debug.LogWarning($"[PCSSUtilities] Error checking PCSS compatibility: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 持E��されたシェーダーが利用可能なタイプに含まれるかチェチE��
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
                    // 不�Eな場合�E寛容に判定（スタンドアロンモード！E
                    return true;
            }
        }

        /// <summary>
        /// GameObjectからPCSS対応�EチE��アルを検索
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
        /// プリセチE��を�EチE��アルに適用
        /// </summary>
        public static void ApplyPresetToMaterial(Material material, PCSSPreset preset, Vector4? customParams = null)
        {
            if (!IsPCSSCompatible(material)) return;

            Vector4 parameters = customParams ?? GetPresetParameters(preset);

            // プリセチE��モードを設宁E
            if (material.HasProperty("_PCSSPresetMode"))
            {
                material.SetFloat("_PCSSPresetMode", (int)preset);
            }

            // 個別パラメータを設宁E
            if (material.HasProperty("_PCSSFilterRadius"))
                material.SetFloat("_PCSSFilterRadius", parameters.x);
            
            if (material.HasProperty("_PCSSLightSize"))
                material.SetFloat("_PCSSLightSize", parameters.y);
            
            if (material.HasProperty("_PCSSBias"))
                material.SetFloat("_PCSSBias", parameters.z);
            
            if (material.HasProperty("_PCSSIntensity"))
                material.SetFloat("_PCSSIntensity", parameters.w);

            // PCSS機�Eを有効匁E
            if (material.HasProperty("_PCSSEnabled"))
                material.SetFloat("_PCSSEnabled", 1.0f);
        }

        /// <summary>
        /// アバター全体にプリセチE��を適用
        /// </summary>
        public static int ApplyPresetToAvatar(GameObject avatar, PCSSPreset preset, Vector4? customParams = null)
        {
            if (avatar == null) return 0;

            var pcssMaterials = FindPCSSMaterials(avatar);
            foreach (var material in pcssMaterials)
            {
                ApplyPresetToMaterial(material, preset, customParams);
            }

            // ModularAvatarコンポ�Eネントが存在する場合、情報を記録�E�リフレクション使用�E�E
            try
            {
                var modularAvatarType = System.Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarInformation, ModularAvatar.Core");
                if (modularAvatarType != null)
                {
                    var maInfo = avatar.GetComponent(modularAvatarType);
                    if (maInfo != null)
                    {
                        // プリセチE��惁E��をコンポ�Eネント名に記録
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
        /// VRChatアバターかどうかをチェック
        /// </summary>
        public static bool IsVRChatAvatar(GameObject gameObject)
        {
#if VRCHAT_SDK_AVAILABLE
            return gameObject.GetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>() != null;
#else
            // VRChat SDKが利用できない場合は常にfalseを返す
            return false;
#endif
        }

        /// <summary>
        /// ModularAvatarコンポ�Eネントが存在するかチェチE���E�リフレクション使用�E�E
        /// </summary>
        public static bool HasModularAvatar(GameObject gameObject)
        {
            if (gameObject == null) return false;

            try
            {
                // リフレクションを使用してModularAvatarの存在をチェチE��
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
        /// ModularAvatarを使用してPCSSコンポ�Eネントを追加�E�リフレクション使用�E�E
        /// </summary>
        public static UnityEngine.Component AddPCSSModularAvatarComponent(GameObject avatar, PCSSPreset preset)
        {
            if (avatar == null) return null;

            try
            {
                // リフレクションを使用してModularAvatarコンポ�Eネントを追加
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
        /// パフォーマンス統計を取征E
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
    /// PCSS パフォーマンス統計情報
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
