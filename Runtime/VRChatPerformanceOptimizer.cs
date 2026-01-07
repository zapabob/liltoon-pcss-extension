using System;
using UnityEngine;

namespace lilToon.PCSS.Runtime
{
    /// <summary>
    /// VRChat/VR環境向けの簡易PCSS自動最適化（ランタイム/プレビュー用）
    /// - XR有効やAndroid(Quest)を検出し、PCSSサンプル数等を自動調整
    /// - 素材に安全に適用できる場合のみ反映（プロパティ存在チェック）
    /// </summary>
    public class VRChatPerformanceOptimizer : MonoBehaviour
    {
        public enum QualityProfile
        {
            Maximum,
            High,
            Medium,
            Low,
            Quest
        }

        public struct QualityParameters
        {
            public int sampleCount;
            public float presetMode; // PCSSPreset を 0..3 で対応（Realistic/Anime/Cinematic/Custom）
        }

        [Tooltip("VR/VRChat 環境を検出したら自動でPCSS品質を最適化します")]
        public bool enableVROptimization = true;

        [Tooltip("Quest など超省リソース端末向けに Low を強制します")]
        public bool forceMobileLowOnAndroid = true;

        [Tooltip("自動品質決定に用いる三角形数閾値（これを大きく超えると品質を落とす）")]
        public int triangleHeavyThreshold = 200_000;

        private void Start()
        {
            if (!enableVROptimization) return;

            bool isVr = DetectVREnvironment();
            if (!isVr) return;

            var renderer = GetComponent<Renderer>();
            if (renderer == null || renderer.material == null) return;

            ApplyVROptimizations(renderer.material);
        }

        private bool DetectVREnvironment()
        {
            // 1) Android(Quest) は常にVR扱い
            if (Application.platform == RuntimePlatform.Android)
                return true;

            // 2) XRSettings.enabled を反射で参照（XR Management がない環境でも安全）
            try
            {
                var xrSettingsType = Type.GetType("UnityEngine.XR.XRSettings, UnityEngine.XR");
                if (xrSettingsType != null)
                {
                    var enabledProp = xrSettingsType.GetProperty("enabled");
                    if (enabledProp != null)
                    {
                        bool enabled = (bool)enabledProp.GetValue(null);
                        if (enabled) return true;
                    }
                }
            }
            catch { /* ignore */ }

            // 3) その他ヒューリスティック（OpenVR/D3D11 + HMD など）→ 簡易に false
            return false;
        }

        private void ApplyVROptimizations(Material material)
        {
            var qp = GetAutoQualityParameters();

            // サンプル数適用（存在チェック）
            if (material.HasProperty("_LocalPCSSSamples"))
                material.SetFloat("_LocalPCSSSamples", qp.sampleCount);

            // プリセットモード（存在すれば）
            if (material.HasProperty("_PCSSPresetMode"))
                material.SetFloat("_PCSSPresetMode", qp.presetMode);

            // 代表的なキーワードも適宜（存在しなくても安全）
            TrySetKeyword(material, "_USEPCSS_ON", true);
            TrySetKeyword(material, "_USESHADOW_ON", true);
        }

        private QualityParameters GetAutoQualityParameters()
        {
            if (forceMobileLowOnAndroid && Application.platform == RuntimePlatform.Android)
            {
                return GetQualityParameters(QualityProfile.Quest);
            }

            float score = CalculatePerformanceScore();
            if (score > 0.9f) return GetQualityParameters(QualityProfile.Maximum);
            if (score > 0.75f) return GetQualityParameters(QualityProfile.High);
            if (score > 0.5f) return GetQualityParameters(QualityProfile.Medium);
            return GetQualityParameters(QualityProfile.Low);
        }

        private float CalculatePerformanceScore()
        {
            // 簡易スコア: デスクトップ=高スコア, モバイル=低スコア。三角形数で減点。
            float baseScore = Application.platform == RuntimePlatform.Android ? 0.4f : 0.85f;

            try
            {
                var stats = PCSSUtilities.GetPerformanceStats(gameObject);
                float triangleFactor = Mathf.Clamp01((float)stats.TotalTriangleCount / triangleHeavyThreshold);
                baseScore -= triangleFactor * 0.4f; // 重いほどスコア減少
            }
            catch { /* ignore */ }

            return Mathf.Clamp01(baseScore);
        }

        private QualityParameters GetQualityParameters(QualityProfile profile)
        {
            // サンプル数は PCSSUtilities の品質にだいたい合わせる
            switch (profile)
            {
                case QualityProfile.Maximum:
                    return new QualityParameters { sampleCount = 64, presetMode = (float)PCSSUtilities.PCSSPreset.Cinematic };
                case QualityProfile.High:
                    return new QualityParameters { sampleCount = 32, presetMode = (float)PCSSUtilities.PCSSPreset.Realistic };
                case QualityProfile.Medium:
                    return new QualityParameters { sampleCount = 16, presetMode = (float)PCSSUtilities.PCSSPreset.Anime };
                case QualityProfile.Quest:
                case QualityProfile.Low:
                default:
                    return new QualityParameters { sampleCount = 8, presetMode = (float)PCSSUtilities.PCSSPreset.Anime };
            }
        }

        private static void TrySetKeyword(Material material, string keyword, bool state)
        {
            if (state) material.EnableKeyword(keyword); else material.DisableKeyword(keyword);
        }
    }
}
