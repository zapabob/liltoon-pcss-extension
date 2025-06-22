using UnityEngine;
using System.Collections.Generic;
#if VRCHAT_SDK_AVAILABLE
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
#endif

// ModularAvatar依存関係の完全な条件付き対応 - エラー耐性を向上
#if UNITY_EDITOR
    // ModularAvatarが利用可能かどうかを実行時に判定
    #if MODULAR_AVATAR_AVAILABLE
        using nadena.dev.modular_avatar.core;
        #define LIL_PCSS_MODULAR_AVATAR_ENABLED
    #endif
#endif

namespace lilToon.PCSS.Runtime
{
    /// <summary>
    /// PCSS関連のユーティリティ機能を提供するクラス
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
        /// 利用可能なシェーダータイプ
        /// </summary>
        public enum ShaderType
        {
            Unknown = 0,
            LilToon = 1,
            Poiyomi = 2,
            Both = 3
        }

        /// <summary>
        /// 現在利用可能なシェーダータイプを検出
        /// </summary>
        public static ShaderType DetectAvailableShaders()
        {
            bool hasLilToon = false;
            bool hasPoiyomi = false;

            // lilToon検出
            var lilToonShader = Shader.Find("lilToon");
            var lilToonPCSSShader = Shader.Find("lilToon/PCSS Extension");
            hasLilToon = (lilToonShader != null || lilToonPCSSShader != null);

            // Poiyomi検出
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
        /// プリセット定義
        /// </summary>
        public enum PCSSPreset
        {
            Realistic = 0,    // リアル影
            Anime = 1,        // アニメ風
            Cinematic = 2,    // 映画風
            Custom = 3        // カスタム
        }

        /// <summary>
        /// PCSS品質設定
        /// </summary>
        public enum PCSSQuality
        {
            Low = 0,          // 低品質 (モバイル向け)
            Medium = 1,       // 中品質 (標準)
            High = 2,         // 高品質 (PC向け)
            Ultra = 3         // 最高品質 (ハイエンドPC向け)
        }

        /// <summary>
        /// プリセットパラメータを取得
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
        /// 品質設定パラメータを取得
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
        /// マテリアルがPCSS対応かどうかをチェック（エラー耐性付き）
        /// </summary>
        public static bool IsPCSSCompatible(Material material)
        {
            if (material == null || material.shader == null) return false;

            try
            {
                string shaderName = material.shader.name;
                
                // 利用可能なシェーダータイプを検出
                ShaderType availableShaders = DetectAvailableShaders();
                
                // シェーダータイプに応じた柔軟な判定
                foreach (string supportedShader in SupportedShaders)
                {
                    if (shaderName.Contains(supportedShader.Replace("/", "").Replace(" ", "")))
                    {
                        // シェーダーが実際に利用可能かチェック
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
                // エラーが発生した場合はfalseを返してログ出力
                Debug.LogWarning($"[PCSSUtilities] Error checking PCSS compatibility: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 指定されたシェーダーが利用可能なタイプに含まれるかチェック
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
                    // 不明な場合は寛容に判定（スタンドアロンモード）
                    return true;
            }
        }

        /// <summary>
        /// GameObjectからPCSS対応マテリアルを検索
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
        /// プリセットをマテリアルに適用
        /// </summary>
        public static void ApplyPresetToMaterial(Material material, PCSSPreset preset, Vector4? customParams = null)
        {
            if (!IsPCSSCompatible(material)) return;

            Vector4 parameters = customParams ?? GetPresetParameters(preset);

            // プリセットモードを設定
            if (material.HasProperty("_PCSSPresetMode"))
            {
                material.SetFloat("_PCSSPresetMode", (int)preset);
            }

            // 個別パラメータを設定
            if (material.HasProperty("_PCSSFilterRadius"))
                material.SetFloat("_PCSSFilterRadius", parameters.x);
            
            if (material.HasProperty("_PCSSLightSize"))
                material.SetFloat("_PCSSLightSize", parameters.y);
            
            if (material.HasProperty("_PCSSBias"))
                material.SetFloat("_PCSSBias", parameters.z);
            
            if (material.HasProperty("_PCSSIntensity"))
                material.SetFloat("_PCSSIntensity", parameters.w);

            // PCSS機能を有効化
            if (material.HasProperty("_PCSSEnabled"))
                material.SetFloat("_PCSSEnabled", 1.0f);
        }

        /// <summary>
        /// アバター全体にプリセットを適用
        /// </summary>
        public static int ApplyPresetToAvatar(GameObject avatar, PCSSPreset preset, Vector4? customParams = null)
        {
            if (avatar == null) return 0;

            var pcssMaterials = FindPCSSMaterials(avatar);
            foreach (var material in pcssMaterials)
            {
                ApplyPresetToMaterial(material, preset, customParams);
            }

#if LIL_PCSS_MODULAR_AVATAR_ENABLED
            // ModularAvatarコンポーネントが存在する場合、情報を記録（エラー耐性付き）
            try
            {
                var maInfo = avatar.GetComponent<ModularAvatarInformation>();
                if (maInfo != null)
                {
                    // プリセット情報をコンポーネント名に記録
                    maInfo.name = $"PCSS_{preset}_Applied";
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[PCSSUtilities] Failed to update ModularAvatar info: {ex.Message}");
            }
#endif

            return pcssMaterials.Count;
        }

        /// <summary>
        /// VRChatアバターかどうかをチェック
        /// </summary>
        public static bool IsVRChatAvatar(GameObject gameObject)
        {
            return gameObject.GetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>() != null;
        }

        /// <summary>
        /// ModularAvatarコンポーネントが存在するかチェック（エラー耐性付き）
        /// </summary>
        public static bool HasModularAvatar(GameObject gameObject)
        {
            if (gameObject == null) return false;

#if LIL_PCSS_MODULAR_AVATAR_ENABLED
            try
            {
                return gameObject.GetComponent<ModularAvatarInformation>() != null;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[PCSSUtilities] ModularAvatar check failed: {ex.Message}");
                return false;
            }
#else
            return false;
#endif
        }

        /// <summary>
        /// ModularAvatarを使用してPCSSコンポーネントを追加（エラー耐性付き）
        /// </summary>
        public static UnityEngine.Component AddPCSSModularAvatarComponent(GameObject avatar, PCSSPreset preset)
        {
            if (avatar == null) return null;

#if LIL_PCSS_MODULAR_AVATAR_ENABLED
            try
            {
                var maInfo = avatar.GetComponent<ModularAvatarInformation>();
                if (maInfo == null)
                {
                    maInfo = avatar.AddComponent<ModularAvatarInformation>();
                }
                
                maInfo.name = $"lilToon_PCSS_{preset}_Component";
                return maInfo;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[PCSSUtilities] Failed to add ModularAvatar component: {ex.Message}");
                return null;
            }
#else
            return null;
#endif
        }

        /// <summary>
        /// パフォーマンス統計を取得
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