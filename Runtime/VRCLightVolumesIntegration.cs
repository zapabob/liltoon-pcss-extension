using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace lilToon.PCSS
{
    /// <summary>
    /// VRC Light Volumes統合システム
    /// REDSIMのVRC Light Volumesシステムとの統合を提供
    /// </summary>
    public class VRCLightVolumesIntegration : MonoBehaviour
    {
        [Header("VRC Light Volumes Integration")]
        [SerializeField] private bool enableVRCLightVolumes = true;
        [SerializeField] private bool autoDetectLightVolumes = true;
        [SerializeField] private float lightVolumeIntensity = 1.0f;
        [SerializeField] private Color lightVolumeTint = Color.white;
        
        [Header("Performance Settings")]
        [SerializeField] private bool enableMobileOptimization = true;
        [SerializeField] private int maxLightVolumeDistance = 100;
        [SerializeField] private float updateFrequency = 0.1f;
        
        private static readonly int _VRCLightVolumeTexture = Shader.PropertyToID("_VRCLightVolumeTexture");
        private static readonly int _VRCLightVolumeParams = Shader.PropertyToID("_VRCLightVolumeParams");
        private static readonly int _VRCLightVolumeWorldToLocal = Shader.PropertyToID("_VRCLightVolumeWorldToLocal");
        private static readonly int _VRCLightVolumeIntensity = Shader.PropertyToID("_VRCLightVolumeIntensity");
        private static readonly int _VRCLightVolumeTint = Shader.PropertyToID("_VRCLightVolumeTint");
        
        private List<Renderer> targetRenderers = new List<Renderer>();
        private List<Material> pcssMaterials = new List<Material>();
        private float lastUpdateTime;
        private Camera mainCamera;
        
        // VRC Light Volumes検出用のキーワード
        private static readonly string[] LIGHT_VOLUME_KEYWORDS = {
            "LIGHT_VOLUMES",
            "VRC_LIGHT_VOLUMES",
            "BAKERY_VOLUMES"
        };
        
        private void Start()
        {
            mainCamera = Camera.main ?? FindObjectOfType<Camera>();
            DetectAndInitialize();
        }
        
        private void Update()
        {
            if (!enableVRCLightVolumes) return;
            
            if (Time.time - lastUpdateTime > updateFrequency)
            {
                UpdateLightVolumes();
                lastUpdateTime = Time.time;
            }
        }
        
        /// <summary>
        /// VRC Light Volumesの自動検出と初期化
        /// </summary>
        public void DetectAndInitialize()
        {
            if (!autoDetectLightVolumes) return;
            
            // シーン内のLight Volume Managerを検索
            var lightVolumeManagers = FindObjectsOfType<MonoBehaviour>()
                .Where(mb => mb.GetType().Name.Contains("LightVolumeManager") || 
                            mb.GetType().Name.Contains("VRCLightVolume"))
                .ToArray();
            
            if (lightVolumeManagers.Length > 0)
            {
                Debug.Log($"[VRC Light Volumes] {lightVolumeManagers.Length} Light Volume Manager(s) detected");
                InitializeLightVolumeSupport();
            }
            
            // PCSS対応マテリアルを検索
            DetectPCSSMaterials();
        }
        
        /// <summary>
        /// PCSS対応マテリアルの検出
        /// </summary>
        private void DetectPCSSMaterials()
        {
            targetRenderers.Clear();
            pcssMaterials.Clear();
            
            var allRenderers = FindObjectsOfType<Renderer>();
            
            foreach (var renderer in allRenderers)
            {
                foreach (var material in renderer.materials)
                {
                    if (IsPCSSMaterial(material))
                    {
                        targetRenderers.Add(renderer);
                        pcssMaterials.Add(material);
                    }
                }
            }
            
            Debug.Log($"[VRC Light Volumes] {pcssMaterials.Count} PCSS materials detected");
        }
        
        /// <summary>
        /// マテリアルがPCSS対応かチェック
        /// </summary>
        private bool IsPCSSMaterial(Material material)
        {
            if (material == null || material.shader == null) return false;
            
            string shaderName = material.shader.name.ToLower();
            return shaderName.Contains("pcss") || 
                   shaderName.Contains("liltoon") || 
                   shaderName.Contains("poiyomi");
        }
        
        /// <summary>
        /// Light Volume サポートの初期化
        /// </summary>
        private void InitializeLightVolumeSupport()
        {
            // グローバルシェーダーキーワードを有効化
            Shader.EnableKeyword("VRC_LIGHT_VOLUMES_ENABLED");
            
            // 品質設定に基づく最適化
            if (enableMobileOptimization && IsMobilePlatform())
            {
                Shader.EnableKeyword("VRC_LIGHT_VOLUMES_MOBILE");
                lightVolumeIntensity *= 0.7f; // モバイルでは強度を下げる
            }
            
            Debug.Log("[VRC Light Volumes] Light Volume support initialized");
        }
        
        /// <summary>
        /// Light Volumesの更新
        /// </summary>
        private void UpdateLightVolumes()
        {
            if (pcssMaterials.Count == 0) return;
            
            // カメラ位置に基づく距離カリング
            Vector3 cameraPos = mainCamera != null ? mainCamera.transform.position : Vector3.zero;
            
            foreach (var material in pcssMaterials)
            {
                if (material == null) continue;
                
                // VRC Light Volume パラメータを設定
                material.SetFloat(_VRCLightVolumeIntensity, lightVolumeIntensity);
                material.SetColor(_VRCLightVolumeTint, lightVolumeTint);
                
                // 距離ベースの最適化
                var renderer = targetRenderers.FirstOrDefault(r => r.materials.Contains(material));
                if (renderer != null)
                {
                    float distance = Vector3.Distance(cameraPos, renderer.transform.position);
                    float distanceFactor = Mathf.Clamp01(1.0f - (distance / maxLightVolumeDistance));
                    
                    material.SetFloat("_VRCLightVolumeDistanceFactor", distanceFactor);
                }
            }
        }
        
        /// <summary>
        /// モバイルプラットフォームの判定
        /// </summary>
        private bool IsMobilePlatform()
        {
            return Application.isMobilePlatform || 
                   SystemInfo.deviceType == DeviceType.Handheld ||
                   QualitySettings.GetQualityLevel() <= 2;
        }
        
        /// <summary>
        /// VRC Light Volumes機能の有効/無効切り替え
        /// </summary>
        public void SetVRCLightVolumesEnabled(bool enabled)
        {
            enableVRCLightVolumes = enabled;
            
            if (enabled)
            {
                Shader.EnableKeyword("VRC_LIGHT_VOLUMES_ENABLED");
            }
            else
            {
                Shader.DisableKeyword("VRC_LIGHT_VOLUMES_ENABLED");
            }
            
            Debug.Log($"[VRC Light Volumes] VRC Light Volumes {(enabled ? "enabled" : "disabled")}");
        }
        
        /// <summary>
        /// Light Volume強度の設定
        /// </summary>
        public void SetLightVolumeIntensity(float intensity)
        {
            lightVolumeIntensity = Mathf.Clamp(intensity, 0f, 2f);
            
            foreach (var material in pcssMaterials)
            {
                if (material != null)
                {
                    material.SetFloat(_VRCLightVolumeIntensity, lightVolumeIntensity);
                }
            }
        }
        
        /// <summary>
        /// Light Volume色調の設定
        /// </summary>
        public void SetLightVolumeTint(Color tint)
        {
            lightVolumeTint = tint;
            
            foreach (var material in pcssMaterials)
            {
                if (material != null)
                {
                    material.SetColor(_VRCLightVolumeTint, lightVolumeTint);
                }
            }
        }
        
        /// <summary>
        /// 統計情報の取得
        /// </summary>
        public VRCLightVolumeStats GetStats()
        {
            return new VRCLightVolumeStats
            {
                EnabledLightVolumes = enableVRCLightVolumes,
                DetectedPCSSMaterials = pcssMaterials.Count,
                TargetRenderers = targetRenderers.Count,
                LightVolumeIntensity = lightVolumeIntensity,
                IsMobileOptimized = enableMobileOptimization && IsMobilePlatform()
            };
        }
        
        private void OnDestroy()
        {
            // クリーンアップ
            Shader.DisableKeyword("VRC_LIGHT_VOLUMES_ENABLED");
            Shader.DisableKeyword("VRC_LIGHT_VOLUMES_MOBILE");
        }
    }
    
    /// <summary>
    /// VRC Light Volumes統計情報
    /// </summary>
    [System.Serializable]
    public struct VRCLightVolumeStats
    {
        public bool EnabledLightVolumes;
        public int DetectedPCSSMaterials;
        public int TargetRenderers;
        public float LightVolumeIntensity;
        public bool IsMobileOptimized;
    }
} 