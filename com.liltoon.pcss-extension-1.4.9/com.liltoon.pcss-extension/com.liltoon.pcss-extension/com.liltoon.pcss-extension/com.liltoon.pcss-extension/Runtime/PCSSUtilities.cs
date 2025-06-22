using UnityEngine;
using System.Collections.Generic;
#if VRCHAT_SDK_AVAILABLE
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
#endif
#if UNITY_EDITOR && MODULAR_AVATAR_AVAILABLE
using nadena.dev.modular_avatar.core;
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
        /// マテリアルがPCSS対応かどうかをチェック
        /// </summary>
        public static bool IsPCSSCompatible(Material material)
        {
            if (material == null || material.shader == null) return false;

            string shaderName = material.shader.name;
            foreach (string supportedShader in SupportedShaders)
            {
                if (shaderName.Contains(supportedShader.Replace("/", "").Replace(" ", "")))
                {
                    return true;
                }
            }
            return false;
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

#if UNITY_EDITOR && MODULAR_AVATAR_AVAILABLE
            // ModularAvatarコンポーネントが存在する場合、情報を記録
            var maInfo = avatar.GetComponent<ModularAvatarInformation>();
            if (maInfo != null)
            {
                // プリセット情報をコンポーネント名に記録
                maInfo.name = $"PCSS_{preset}_Applied";
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

#if UNITY_EDITOR && MODULAR_AVATAR_AVAILABLE
        /// <summary>
        /// ModularAvatarコンポーネントが存在するかチェック
        /// </summary>
        public static bool HasModularAvatar(GameObject gameObject)
        {
            return gameObject.GetComponent<ModularAvatarInformation>() != null;
        }

        /// <summary>
        /// ModularAvatarを使用してPCSSコンポーネントを追加
        /// </summary>
        public static ModularAvatarInformation AddPCSSModularAvatarComponent(GameObject avatar, PCSSPreset preset)
        {
            var maInfo = avatar.GetComponent<ModularAvatarInformation>();
            if (maInfo == null)
            {
                maInfo = avatar.AddComponent<ModularAvatarInformation>();
            }
            
            maInfo.name = $"lilToon_PCSS_{preset}_Component";
            return maInfo;
        }
#endif

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