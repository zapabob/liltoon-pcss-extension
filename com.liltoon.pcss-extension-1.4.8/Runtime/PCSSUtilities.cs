using UnityEngine;
#if VRCHAT_SDK_AVAILABLE
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
#endif
#if UNITY_EDITOR && MODULAR_AVATAR_AVAILABLE
using nadena.dev.modular_avatar.core;
#endif

namespace lilToon.PCSS
{
    /// <summary>
    /// PCSS機能のランタイムユーティリティ
    /// </summary>
    public static class PCSSUtilities
    {
        /// <summary>
        /// PCSS品質設定
        /// </summary>
        public enum PCSSQuality
        {
            Low = 0,      // 8 samples
            Medium = 1,   // 16 samples  
            High = 2,     // 32 samples
            Ultra = 3     // 64 samples
        }
        
        /// <summary>
        /// 品質設定に基づいてマテリアルのPCSSパラメータを設定
        /// </summary>
        /// <param name="material">対象のマテリアル</param>
        /// <param name="quality">品質設定</param>
        public static void ApplyQualitySettings(Material material, PCSSQuality quality)
        {
            if (material == null) return;
            
            switch (quality)
            {
                case PCSSQuality.Low:
                    material.SetFloat("_PCSSBlockerSearchRadius", 0.005f);
                    material.SetFloat("_PCSSFilterRadius", 0.01f);
                    material.SetFloat("_PCSSSampleCount", 8f);
                    material.SetFloat("_PCSSLightSize", 1.0f);
                    break;
                    
                case PCSSQuality.Medium:
                    material.SetFloat("_PCSSBlockerSearchRadius", 0.01f);
                    material.SetFloat("_PCSSFilterRadius", 0.02f);
                    material.SetFloat("_PCSSSampleCount", 16f);
                    material.SetFloat("_PCSSLightSize", 1.5f);
                    break;
                    
                case PCSSQuality.High:
                    material.SetFloat("_PCSSBlockerSearchRadius", 0.02f);
                    material.SetFloat("_PCSSFilterRadius", 0.03f);
                    material.SetFloat("_PCSSSampleCount", 32f);
                    material.SetFloat("_PCSSLightSize", 2.0f);
                    break;
                    
                case PCSSQuality.Ultra:
                    material.SetFloat("_PCSSBlockerSearchRadius", 0.03f);
                    material.SetFloat("_PCSSFilterRadius", 0.04f);
                    material.SetFloat("_PCSSSampleCount", 64f);
                    material.SetFloat("_PCSSLightSize", 2.5f);
                    break;
            }
            
            material.SetFloat("_PCSSQuality", (float)quality);
        }
        
        /// <summary>
        /// マテリアルでPCSSを有効化
        /// </summary>
        /// <param name="material">対象のマテリアル</param>
        /// <param name="enable">有効化するかどうか</param>
        public static void EnablePCSS(Material material, bool enable)
        {
            if (material == null) return;
            
            material.SetFloat("_UsePCSS", enable ? 1.0f : 0.0f);
            
            if (enable)
            {
                material.EnableKeyword("_USEPCSS_ON");
            }
            else
            {
                material.DisableKeyword("_USEPCSS_ON");
            }
        }
        
        /// <summary>
        /// アバターの全マテリアルにPCSS設定を適用
        /// </summary>
        /// <param name="avatar">対象のアバター</param>
        /// <param name="quality">品質設定</param>
        /// <param name="enablePCSS">PCSSを有効化するかどうか</param>
        public static void ApplyToAvatar(GameObject avatar, PCSSQuality quality, bool enablePCSS = true)
        {
            if (avatar == null) return;
            
            var renderers = avatar.GetComponentsInChildren<Renderer>();
            
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    if (material != null && IsPCSSMaterial(material))
                    {
                        EnablePCSS(material, enablePCSS);
                        if (enablePCSS)
                        {
                            ApplyQualitySettings(material, quality);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// マテリアルがPCSS対応かどうかを確認
        /// </summary>
        /// <param name="material">確認するマテリアル</param>
        /// <returns>PCSS対応の場合true</returns>
        public static bool IsPCSSMaterial(Material material)
        {
            if (material == null || material.shader == null) return false;
            
            return material.shader.name.Contains("lilToon/PCSS") ||
                   material.HasProperty("_UsePCSS");
        }
        
        /// <summary>
        /// PCSS設定の検証
        /// </summary>
        /// <param name="material">検証するマテリアル</param>
        /// <returns>設定が適切な場合true</returns>
        public static bool ValidatePCSSSettings(Material material)
        {
            if (!IsPCSSMaterial(material)) return false;
            
            // 必要なプロパティが存在するかチェック
            string[] requiredProperties = {
                "_UsePCSS",
                "_PCSSBlockerSearchRadius",
                "_PCSSFilterRadius",
                "_PCSSSampleCount",
                "_PCSSLightSize"
            };
            
            foreach (string prop in requiredProperties)
            {
                if (!material.HasProperty(prop))
                {
                    Debug.LogWarning($"Material {material.name} is missing required PCSS property: {prop}");
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// パフォーマンス警告の確認
        /// </summary>
        /// <param name="material">確認するマテリアル</param>
        /// <returns>警告メッセージ（問題がない場合は空文字）</returns>
        public static string GetPerformanceWarning(Material material)
        {
            if (!IsPCSSMaterial(material)) return "";
            
            float sampleCount = material.GetFloat("_PCSSSampleCount");
            
            if (sampleCount > 32)
            {
                return "サンプル数が32を超えています。パフォーマンスに影響する可能性があります。";
            }
            else if (sampleCount > 16)
            {
                return "サンプル数が多めです。低スペック環境では注意が必要です。";
            }
            
            return "";
        }
        
        /// <summary>
        /// VRChat最適化設定を適用
        /// </summary>
        /// <param name="material">対象マテリアル</param>
        /// <param name="isQuest">Quest環境かどうか</param>
        /// <param name="isVR">VR環境かどうか</param>
        /// <param name="avatarCount">現在のアバター数</param>
        public static void ApplyVRChatOptimizations(Material material, bool isQuest, bool isVR, int avatarCount)
        {
            if (!IsPCSSMaterial(material)) return;
            
            // Quest最適化
            if (isQuest)
            {
                ApplyQuestOptimizations(material);
            }
            
            // VR最適化
            if (isVR)
            {
                ApplyVROptimizations(material);
            }
            
            // アバター密度最適化
            ApplyAvatarDensityOptimizations(material, avatarCount);
        }
        
        /// <summary>
        /// Quest専用最適化
        /// </summary>
        private static void ApplyQuestOptimizations(Material material)
        {
            // Quest用低負荷設定
            float currentSamples = material.GetFloat("_PCSSSampleCount");
            material.SetFloat("_PCSSSampleCount", Mathf.Min(currentSamples, 16f));
            
            float currentRadius = material.GetFloat("_PCSSFilterRadius");
            material.SetFloat("_PCSSFilterRadius", currentRadius * 0.7f);
            
            material.SetFloat("_PCSSBlockerSearchRadius", 
                Mathf.Min(material.GetFloat("_PCSSBlockerSearchRadius"), 0.01f));
            
            // Quest用シェーダーキーワード
            material.EnableKeyword("VRC_QUEST_OPTIMIZATION");
        }
        
        /// <summary>
        /// VR最適化
        /// </summary>
        private static void ApplyVROptimizations(Material material)
        {
            // VR用フレームレート最適化
            float currentRadius = material.GetFloat("_PCSSFilterRadius");
            material.SetFloat("_PCSSFilterRadius", currentRadius * 0.9f);
            
            // VR用サンプル数調整
            float currentSamples = material.GetFloat("_PCSSSampleCount");
            if (currentSamples > 32f)
            {
                material.SetFloat("_PCSSSampleCount", 32f);
            }
        }
        
        /// <summary>
        /// アバター密度に応じた最適化
        /// </summary>
        private static void ApplyAvatarDensityOptimizations(Material material, int avatarCount)
        {
            // アバター数に応じて品質を動的調整
            float densityFactor = Mathf.Clamp01(1.0f - (avatarCount - 5) * 0.05f);
            
            if (densityFactor < 1.0f)
            {
                float currentRadius = material.GetFloat("_PCSSFilterRadius");
                material.SetFloat("_PCSSFilterRadius", currentRadius * densityFactor);
                
                float currentSamples = material.GetFloat("_PCSSSampleCount");
                material.SetFloat("_PCSSSampleCount", 
                    Mathf.Max(8f, currentSamples * densityFactor));
            }
        }
        
        /// <summary>
        /// マテリアルの現在の品質レベルを取得
        /// </summary>
        public static PCSSQuality GetMaterialQuality(Material material)
        {
            if (!IsPCSSMaterial(material)) return PCSSQuality.Medium;
            
            float samples = material.GetFloat("_PCSSSampleCount");
            return samples switch
            {
                >= 48f => PCSSQuality.Ultra,
                >= 24f => PCSSQuality.High,
                >= 12f => PCSSQuality.Medium,
                _ => PCSSQuality.Low
            };
        }
        
        /// <summary>
        /// VRChat品質設定に基づく推奨PCSS品質を取得
        /// </summary>
        public static PCSSQuality GetRecommendedQualityForVRChat(string vrcQuality, bool isQuest, int avatarCount)
        {
            // Quest環境では常に低品質
            if (isQuest)
            {
                return avatarCount > 8 ? PCSSQuality.Low : PCSSQuality.Medium;
            }
            
            // VRChat品質設定マッピング
            PCSSQuality baseQuality = vrcQuality switch
            {
                "VRC Ultra" => PCSSQuality.Ultra,
                "VRC High" => PCSSQuality.High,
                "VRC Medium" => PCSSQuality.Medium,
                "VRC Low" => PCSSQuality.Low,
                _ => PCSSQuality.Medium
            };
            
            // アバター数による調整
            if (avatarCount > 15)
            {
                baseQuality = (PCSSQuality)Mathf.Max(0, (int)baseQuality - 2);
            }
            else if (avatarCount > 10)
            {
                baseQuality = (PCSSQuality)Mathf.Max(0, (int)baseQuality - 1);
            }
            
            return baseQuality;
        }
    }
} 