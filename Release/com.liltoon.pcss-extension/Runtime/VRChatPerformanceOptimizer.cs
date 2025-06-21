using UnityEngine;
using UnityEngine.Rendering;
#if VRCHAT_SDK_AVAILABLE
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
#endif
#if MODULAR_AVATAR_AVAILABLE
using nadena.dev.modular_avatar.core;
#endif

namespace lilToon.PCSS
{
    /// <summary>
    /// VRChat品質設定連動最適化システム
    /// VRChatのGraphics Quality設定に応じてPCSS品質を自動調整
    /// </summary>
    public class VRChatPerformanceOptimizer : MonoBehaviour
    {
        [Header("VRChat Performance Integration")]
        [SerializeField] private bool enableAutoOptimization = true;
        [SerializeField] private bool respectVRChatSettings = true;
        [SerializeField] private bool enableMobileDetection = true;
        
        [Header("Performance Thresholds")]
        [SerializeField] private float targetFramerate = 90f;
        [SerializeField] private float lowFramerateThreshold = 45f;
        [SerializeField] private int maxAvatarsForHighQuality = 10;
        
        [Header("VRChat Quality Mapping")]
        [SerializeField] private PCSSQuality vrcUltraQuality = PCSSQuality.Ultra;
        [SerializeField] private PCSSQuality vrcHighQuality = PCSSQuality.High;
        [SerializeField] private PCSSQuality vrcMediumQuality = PCSSQuality.Medium;
        [SerializeField] private PCSSQuality vrcLowQuality = PCSSQuality.Low;
        [SerializeField] private PCSSQuality vrcMobileQuality = PCSSQuality.Low;
        
        private float frameRateHistory = 90f;
        private int currentAvatarCount = 0;
        private bool isQuest = false;
        private bool isVRMode = false;
        
        private void Start()
        {
            DetectPlatform();
            if (enableAutoOptimization)
            {
                InvokeRepeating(nameof(UpdatePerformanceOptimization), 1f, 2f);
            }
        }
        
        private void DetectPlatform()
        {
            // Quest検出
            isQuest = SystemInfo.deviceModel.Contains("Oculus") || 
                     Application.platform == RuntimePlatform.Android;
            
            // VRモード検出
            isVRMode = UnityEngine.XR.XRSettings.enabled;
            
            // 初期最適化適用
            if (isQuest)
            {
                ApplyQuestOptimizations();
            }
        }
        
        private void UpdatePerformanceOptimization()
        {
            if (!enableAutoOptimization) return;
            
            // フレームレート監視
            float currentFPS = 1f / Time.deltaTime;
            frameRateHistory = Mathf.Lerp(frameRateHistory, currentFPS, 0.1f);
            
            // アバター数取得（VRChatのAvatar Culling設定を考慮）
            UpdateAvatarCount();
            
            // VRChat品質設定取得
            var vrcQuality = GetVRChatQualityLevel();
            
            // 最適化実行
            OptimizePCSSSettings(vrcQuality);
        }
        
        private void UpdateAvatarCount()
        {
            // VRChatのMaximum Shown Avatarsを考慮
            var renderers = FindObjectsOfType<Renderer>();
            currentAvatarCount = 0;
            
            foreach (var renderer in renderers)
            {
                // アバターのレンダラーかチェック
                if (IsAvatarRenderer(renderer))
                {
                    currentAvatarCount++;
                }
            }
        }
        
        private bool IsAvatarRenderer(Renderer renderer)
        {
            // VRChatのAvatar Culling範囲内かチェック
            float distance = Vector3.Distance(transform.position, renderer.transform.position);
            
            // VRChatの"Hide Avatars Beyond"設定を模倣
            float maxAvatarDistance = GetVRChatAvatarCullingDistance();
            
            return distance <= maxAvatarDistance && 
                   renderer.gameObject.layer != LayerMask.NameToLayer("UI") &&
                   !renderer.gameObject.name.Contains("World");
        }
        
        private float GetVRChatAvatarCullingDistance()
        {
            // VRChatのHide Avatars Beyond設定を模倣
            // デフォルト値やプラットフォームに応じて調整
            if (isQuest)
                return 15f; // Quest用短距離
            else if (isVRMode)
                return 25f; // VR用中距離
            else
                return 50f; // Desktop用長距離
        }
        
        private string GetVRChatQualityLevel()
        {
            // Unity品質設定からVRChat品質レベルを推定
            string currentQuality = QualitySettings.names[QualitySettings.GetQualityLevel()];
            
            // VRChatの品質設定マッピング
            if (currentQuality.Contains("Ultra") || currentQuality.Contains("VRC Ultra"))
                return "VRC Ultra";
            else if (currentQuality.Contains("High") || currentQuality.Contains("VRC High"))
                return "VRC High";
            else if (currentQuality.Contains("Medium") || currentQuality.Contains("VRC Medium"))
                return "VRC Medium";
            else if (currentQuality.Contains("Low") || currentQuality.Contains("VRC Low"))
                return "VRC Low";
            else if (isQuest)
                return "VRC Mobile";
            
            return "VRC Medium"; // デフォルト
        }
        
        private void OptimizePCSSSettings(string vrcQuality)
        {
            PCSSQuality targetQuality = GetPCSSQualityForVRCQuality(vrcQuality);
            
            // フレームレートベース調整
            if (frameRateHistory < lowFramerateThreshold)
            {
                targetQuality = DowngradeQuality(targetQuality);
            }
            
            // アバター数ベース調整
            if (currentAvatarCount > maxAvatarsForHighQuality)
            {
                targetQuality = DowngradeQuality(targetQuality);
            }
            
            // VRC Light Volumes最適化
            OptimizeVRCLightVolumes(targetQuality);
            
            // PCSS設定適用
            ApplyPCSSQuality(targetQuality);
        }
        
        private PCSSQuality GetPCSSQualityForVRCQuality(string vrcQuality)
        {
            return vrcQuality switch
            {
                "VRC Ultra" => vrcUltraQuality,
                "VRC High" => vrcHighQuality,
                "VRC Medium" => vrcMediumQuality,
                "VRC Low" => vrcLowQuality,
                "VRC Mobile" => vrcMobileQuality,
                _ => PCSSQuality.Medium
            };
        }
        
        private PCSSQuality DowngradeQuality(PCSSQuality current)
        {
            return current switch
            {
                PCSSQuality.Ultra => PCSSQuality.High,
                PCSSQuality.High => PCSSQuality.Medium,
                PCSSQuality.Medium => PCSSQuality.Low,
                PCSSQuality.Low => PCSSQuality.Low,
                _ => PCSSQuality.Low
            };
        }
        
        private void OptimizeVRCLightVolumes(PCSSQuality quality)
        {
            var lightVolumeIntegration = GetComponent<VRCLightVolumesIntegration>();
            if (lightVolumeIntegration == null) return;
            
            // VRC Light Volumes最適化設定
            switch (quality)
            {
                case PCSSQuality.Ultra:
                    lightVolumeIntegration.maxLightVolumeDistance = 150;
                    lightVolumeIntegration.updateFrequency = 0.05f;
                    lightVolumeIntegration.enableMobileOptimization = false;
                    break;
                
                case PCSSQuality.High:
                    lightVolumeIntegration.maxLightVolumeDistance = 100;
                    lightVolumeIntegration.updateFrequency = 0.1f;
                    lightVolumeIntegration.enableMobileOptimization = false;
                    break;
                
                case PCSSQuality.Medium:
                    lightVolumeIntegration.maxLightVolumeDistance = 75;
                    lightVolumeIntegration.updateFrequency = 0.15f;
                    lightVolumeIntegration.enableMobileOptimization = true;
                    break;
                
                case PCSSQuality.Low:
                    lightVolumeIntegration.maxLightVolumeDistance = 50;
                    lightVolumeIntegration.updateFrequency = 0.2f;
                    lightVolumeIntegration.enableMobileOptimization = true;
                    break;
            }
        }
        
        private void ApplyPCSSQuality(PCSSQuality quality)
        {
            // シーン内のすべてのPCSSマテリアルに適用
            var materials = FindPCSSMaterials();
            
            foreach (var material in materials)
            {
                PCSSUtilities.ApplyQualitySettings(material, quality);
                
                // VRChat特有の最適化
                ApplyVRChatSpecificOptimizations(material, quality);
            }
        }
        
        private Material[] FindPCSSMaterials()
        {
            var renderers = FindObjectsOfType<Renderer>();
            var pcssMaterials = new System.Collections.Generic.List<Material>();
            
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    if (PCSSUtilities.IsPCSSMaterial(material))
                    {
                        pcssMaterials.Add(material);
                    }
                }
            }
            
            return pcssMaterials.ToArray();
        }
        
        private void ApplyVRChatSpecificOptimizations(Material material, PCSSQuality quality)
        {
            // VRChatのShadow Quality設定を考慮
            var shadowQuality = QualitySettings.shadowResolution;
            
            switch (shadowQuality)
            {
                case ShadowResolution.Low:
                    material.SetFloat("_PCSSBias", 0.01f);
                    break;
                case ShadowResolution.Medium:
                    material.SetFloat("_PCSSBias", 0.005f);
                    break;
                case ShadowResolution.High:
                    material.SetFloat("_PCSSBias", 0.002f);
                    break;
                case ShadowResolution.VeryHigh:
                    material.SetFloat("_PCSSBias", 0.001f);
                    break;
            }
            
            // VRChatのLOD Quality設定を考慮
            float lodBias = QualitySettings.lodBias;
            if (lodBias < 1.5f) // Low LOD Quality
            {
                // LOD品質が低い場合はPCSSも控えめに
                float currentRadius = material.GetFloat("_PCSSFilterRadius");
                material.SetFloat("_PCSSFilterRadius", currentRadius * 0.8f);
            }
        }
        
        private void ApplyQuestOptimizations()
        {
            // Quest専用最適化
            targetFramerate = 72f; // Quest 2の標準フレームレート
            lowFramerateThreshold = 36f; // Quest用低フレームレート閾値
            maxAvatarsForHighQuality = 5; // Quest用アバター数制限
            
            // Quest用品質マッピング
            vrcUltraQuality = PCSSQuality.Medium;
            vrcHighQuality = PCSSQuality.Medium;
            vrcMediumQuality = PCSSQuality.Low;
            vrcLowQuality = PCSSQuality.Low;
            vrcMobileQuality = PCSSQuality.Low;
        }
        
        /// <summary>
        /// パフォーマンス統計取得
        /// </summary>
        public PerformanceStats GetPerformanceStats()
        {
            return new PerformanceStats
            {
                CurrentFPS = frameRateHistory,
                AvatarCount = currentAvatarCount,
                IsQuest = isQuest,
                IsVRMode = isVRMode,
                VRCQuality = GetVRChatQualityLevel(),
                CurrentPCSSQuality = GetCurrentPCSSQuality()
            };
        }
        
        private PCSSQuality GetCurrentPCSSQuality()
        {
            var materials = FindPCSSMaterials();
            if (materials.Length > 0)
            {
                return PCSSUtilities.GetMaterialQuality(materials[0]);
            }
            return PCSSQuality.Medium;
        }
        
        /// <summary>
        /// 手動最適化実行
        /// </summary>
        [ContextMenu("Force Optimization")]
        public void ForceOptimization()
        {
            UpdatePerformanceOptimization();
        }
    }
    
    [System.Serializable]
    public struct PerformanceStats
    {
        public float CurrentFPS;
        public int AvatarCount;
        public bool IsQuest;
        public bool IsVRMode;
        public string VRCQuality;
        public PCSSQuality CurrentPCSSQuality;
    }
} 