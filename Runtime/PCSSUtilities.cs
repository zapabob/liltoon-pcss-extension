using UnityEngine;

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
    }
} 