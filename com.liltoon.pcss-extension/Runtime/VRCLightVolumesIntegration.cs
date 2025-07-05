using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;
#if VRCHAT_SDK_AVAILABLE
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
#endif

namespace lilToon.PCSS.Runtime
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
                
                // VRChatパフォーマンス監視
                MonitorVRChatPerformance();
            }
        }
        
        /// <summary>
        /// VRChatパフォーマンス監視
        /// </summary>
        private void MonitorVRChatPerformance()
        {
            float currentFPS = 1f / Time.deltaTime;
            
            // フレームレートが低下した場合の自動最適化
            if (currentFPS < 45f && !enableMobileOptimization)
            {
                Debug.LogWarning("VRC Light Volumes: Low framerate detected. Enabling mobile optimization.");
                enableMobileOptimization = true;
                ApplyMobileOptimizations();
            }
            
            // Quest環境での追加最適化
            if (IsQuestEnvironment() && lightVolumeIntensity > 0.5f)
            {
                lightVolumeIntensity = Mathf.Min(lightVolumeIntensity, 0.5f);
                Debug.Log("VRC Light Volumes: Quest optimization applied - intensity reduced.");
            }
        }
        
        /// <summary>
        /// Quest環境検出
        /// </summary>
        private bool IsQuestEnvironment()
        {
            return SystemInfo.deviceModel.Contains("Oculus") || 
                   Application.platform == RuntimePlatform.Android;
        }
        
        /// <summary>
        /// モバイル最適化適用
        /// </summary>
        private void ApplyMobileOptimizations()
        {
            maxLightVolumeDistance = Mathf.Min(maxLightVolumeDistance, 50);
            updateFrequency = Mathf.Max(updateFrequency, 0.2f);
            lightVolumeIntensity *= 0.8f;
            
            // VRChat Avatar Culling設定を考慮
            ApplyVRChatCullingOptimizations();
        }
        
        /// <summary>
        /// VRChat Avatar Culling最適化
        /// </summary>
        private void ApplyVRChatCullingOptimizations()
        {
            // VRChatの"Hide Avatars Beyond"設定を模倣
            int avatarCount = CountNearbyAvatars();
            
            if (avatarCount > 10)
            {
                // アバターが多い場合はLight Volume距離を短縮
                maxLightVolumeDistance = Mathf.Min(maxLightVolumeDistance, 30);
                lightVolumeIntensity *= 0.9f;
            }
        }
        
        /// <summary>
        /// 近くのアバター数をカウント
        /// </summary>
        private int CountNearbyAvatars()
        {
            var renderers = FindObjectsOfType<Renderer>();
            int count = 0;
            
            foreach (var renderer in renderers)
            {
                if (IsAvatarRenderer(renderer))
                {
                    float distance = Vector3.Distance(transform.position, renderer.transform.position);
                    if (distance <= maxLightVolumeDistance)
                    {
                        count++;
                    }
                }
            }
            
            return count;
        }
        
        /// <summary>
        /// アバターのレンダラーかチェック
        /// </summary>
        private bool IsAvatarRenderer(Renderer renderer)
        {
            return renderer.gameObject.layer != LayerMask.NameToLayer("UI") &&
                   !renderer.gameObject.name.Contains("World") &&
                   renderer.gameObject.GetComponent<Animator>() != null;
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

        public bool EnableVRCLightVolumes { get => enableVRCLightVolumes; set => enableVRCLightVolumes = value; }
        public bool AutoDetectLightVolumes { get => autoDetectLightVolumes; set => autoDetectLightVolumes = value; }
        public float LightVolumeIntensity { get => lightVolumeIntensity; set => lightVolumeIntensity = value; }
        public Color LightVolumeTint { get => lightVolumeTint; set => lightVolumeTint = value; }
        public bool EnableMobileOptimization { get => enableMobileOptimization; set => enableMobileOptimization = value; }
        public int MaxLightVolumeDistance { get => maxLightVolumeDistance; set => maxLightVolumeDistance = value; }
        public float UpdateFrequency { get => updateFrequency; set => updateFrequency = value; }
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