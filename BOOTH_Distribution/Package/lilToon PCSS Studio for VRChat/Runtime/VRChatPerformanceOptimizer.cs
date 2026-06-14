using System;
using UnityEngine;

namespace lilToon.PCSS.Runtime
{
    /// <summary>
    /// Runtime PCSS quality guard for VRChat avatars.
    /// Keeps PCSS enabled, but clamps heavy sampling and distance in VR/mobile.
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
            public float presetMode;
            public float qualityLevel;
            public float maxDistance;
            public float distanceFade;
        }

        [Tooltip("Automatically reduce PCSS quality while running in VR or on mobile.")]
        public bool enableVROptimization = true;

        [Tooltip("Force the safest profile on Android/Quest.")]
        public bool forceMobileLowOnAndroid = true;

        [Tooltip("Triangle count where the optimizer starts reducing PCSS quality.")]
        public int triangleHeavyThreshold = 200_000;

        private void Start()
        {
            if (!enableVROptimization) return;
            if (!DetectVREnvironment()) return;

            Renderer renderer = GetComponent<Renderer>();
            if (renderer == null || renderer.material == null) return;

            ApplyVROptimizations(renderer.material);
        }

        private bool DetectVREnvironment()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return true;
            }

            try
            {
                Type xrSettingsType = Type.GetType("UnityEngine.XR.XRSettings, UnityEngine.XR");
                if (xrSettingsType != null)
                {
                    var enabledProperty = xrSettingsType.GetProperty("enabled");
                    if (enabledProperty != null && (bool)enabledProperty.GetValue(null))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // Optional XR packages vary by project; falling back to false is safest.
            }

            return false;
        }

        private void ApplyVROptimizations(Material material)
        {
            if (material == null) return;

            QualityParameters qp = GetAutoQualityParameters();

            float sampleCount = ClampDownExisting(material, qp.sampleCount, "_LocalPCSSSamples", "_PCSSSamples");
            float qualityLevel = ClampDownExisting(material, qp.qualityLevel, "_PCSSQualityLevel");
            float maxDistance = ClampDownExisting(material, Mathf.Min(qp.maxDistance, 10.0f), "_PCSSMaxDistance");

            SetFloatIfExists(material, "_LocalPCSSSamples", sampleCount);
            SetFloatIfExists(material, "_PCSSSamples", sampleCount);
            SetFloatIfExists(material, "_PCSSQualityLevel", qualityLevel);
            SetFloatIfExists(material, "_PCSSMaxDistance", maxDistance);
            SetFloatIfExists(material, "_PCSSDistanceFade", Mathf.Min(qp.distanceFade, 3.0f));
            SetFloatIfExists(material, "_PCSSPresetMode", qp.presetMode);
            SetFloatIfExists(material, "_UsePCSS", 1.0f);
            SetFloatIfExists(material, "_PCSSEnabled", 1.0f);
            SetFloatIfExists(material, "_UsePCSSOptimization", 1.0f);
            SetFloatIfExists(material, "_PCSSOptimizationLevel", 1.0f);
            SetFloatIfExists(material, "_UseVRChatPerformanceGate", 1.0f);
            SetFloatIfExists(material, "_UseNoLightPCSSBoost", 1.0f);

            TrySetKeyword(material, "_USEPCSS_ON", true);
            TrySetKeyword(material, "_USESHADOW_ON", true);
            TrySetKeyword(material, "_USEPCSSOPTIMIZATION_ON", true);
            TrySetKeyword(material, "_USEVRCHATPERFORMANCEGATE_ON", true);
            TrySetKeyword(material, "_USENOLIGHTPCSSBOOST_ON", material.HasProperty("_UseNoLightPCSSBoost"));
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
            float baseScore = Application.platform == RuntimePlatform.Android ? 0.35f : 0.62f;
            bool midrangePcVrGpu = false;

            if (Application.platform != RuntimePlatform.Android)
            {
                int vramMb = SystemInfo.graphicsMemorySize;
                string gpuName = (SystemInfo.graphicsDeviceName ?? string.Empty).ToLowerInvariant();
                midrangePcVrGpu = ContainsAny(
                    gpuName,
                    "rtx 3050",
                    "rtx 3060",
                    "rtx 4050",
                    "rtx 4060",
                    "rtx 2060",
                    "rtx 2070",
                    "gtx 1660",
                    "gtx 1070",
                    "gtx 1080",
                    "rx 5600",
                    "rx 6600",
                    "rx 7600");

                bool modernPcVrGpu = ContainsAny(
                    gpuName,
                    "rtx 20",
                    "rtx 30",
                    "rtx 40",
                    "rtx 50",
                    "rx 6",
                    "rx 7",
                    "rx 8",
                    "arc a7");

                if (modernPcVrGpu)
                {
                    baseScore += midrangePcVrGpu ? 0.06f : 0.10f;
                }

                if (vramMb >= 16_000)
                {
                    baseScore += 0.10f;
                }
                else if (vramMb >= 10_000)
                {
                    baseScore += 0.08f;
                }
                else if (vramMb >= 8_000)
                {
                    baseScore += 0.04f;
                }
                else if (vramMb >= 6_000)
                {
                    baseScore += 0.02f;
                }
                else if (!modernPcVrGpu && vramMb > 0 && vramMb < 5_000)
                {
                    baseScore -= 0.18f;
                }
            }

            try
            {
                PCSSPerformanceStats stats = PCSSUtilities.GetPerformanceStats(gameObject);
                float triangleFactor = Mathf.Clamp01((float)stats.TotalTriangleCount / triangleHeavyThreshold);
                baseScore -= triangleFactor * 0.4f;
            }
            catch
            {
                // Stats are best-effort; keep the runtime path resilient.
            }

            if (midrangePcVrGpu)
            {
                baseScore = Mathf.Min(baseScore, 0.70f);
            }

            return Mathf.Clamp01(baseScore);
        }

        private static bool ContainsAny(string text, params string[] fragments)
        {
            if (string.IsNullOrEmpty(text) || fragments == null) return false;
            foreach (string fragment in fragments)
            {
                if (!string.IsNullOrEmpty(fragment) && text.Contains(fragment))
                {
                    return true;
                }
            }

            return false;
        }

        private QualityParameters GetQualityParameters(QualityProfile profile)
        {
            switch (profile)
            {
                case QualityProfile.Maximum:
                    return new QualityParameters
                    {
                        sampleCount = 16,
                        presetMode = (float)PCSSUtilities.PCSSPreset.Cinematic,
                        qualityLevel = 2f,
                        maxDistance = 10.0f,
                        distanceFade = 3.0f
                    };
                case QualityProfile.High:
                    return new QualityParameters
                    {
                        sampleCount = 10,
                        presetMode = (float)PCSSUtilities.PCSSPreset.Realistic,
                        qualityLevel = 1f,
                        maxDistance = 10.0f,
                        distanceFade = 3.0f
                    };
                case QualityProfile.Medium:
                    return new QualityParameters
                    {
                        sampleCount = 6,
                        presetMode = (float)PCSSUtilities.PCSSPreset.Anime,
                        qualityLevel = 1f,
                        maxDistance = 10.0f,
                        distanceFade = 3.0f
                    };
                case QualityProfile.Quest:
                case QualityProfile.Low:
                default:
                    return new QualityParameters
                    {
                        sampleCount = 4,
                        presetMode = (float)PCSSUtilities.PCSSPreset.Anime,
                        qualityLevel = 1f,
                        maxDistance = 10.0f,
                        distanceFade = 3.0f
                    };
            }
        }

        private static void SetFloatIfExists(Material material, string propertyName, float value)
        {
            if (material != null && material.HasProperty(propertyName))
            {
                material.SetFloat(propertyName, value);
            }
        }

        private static float ClampDownExisting(Material material, float recommended, params string[] propertyNames)
        {
            if (material == null || propertyNames == null) return recommended;

            foreach (string propertyName in propertyNames)
            {
                if (material.HasProperty(propertyName))
                {
                    float existing = material.GetFloat(propertyName);
                    if (existing > 0.0f)
                    {
                        return Mathf.Min(existing, recommended);
                    }
                }
            }

            return recommended;
        }

        private static void TrySetKeyword(Material material, string keyword, bool state)
        {
            if (material == null) return;
            if (state)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }
    }
}
