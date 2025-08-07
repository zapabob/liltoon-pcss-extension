using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;
#if VRCHAT_SDK_AVAILABLE
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
#endif

namespace lilToon.PCSS
{
    /// <summary>
    /// VRC Light Volumes邨ｱ蜷医す繧ｹ繝・Β
    /// REDSIM縺ｮVRC Light Volumes繧ｷ繧ｹ繝・Β縺ｨ縺ｮ邨ｱ蜷医ｒ謠蝉ｾ・
    /// </summary>
    public class VRCLightVolumesIntegration : MonoBehaviour
    {
        [Header("VRC Light Volumes Integration")]
        [SerializeField] private bool enableVRCLightVolumes = true;
        [SerializeField] private bool autoDetectLightVolumes = true;
        [SerializeField] private float lightVolumeIntensity = 1.0f;
        [SerializeField] private Color lightVolumeTint = Color.white;
        
        [Header("Performance Settings")]
        [SerializeField] public bool enableMobileOptimization = true;
        [SerializeField] public int maxLightVolumeDistance = 100;
        [SerializeField] public float updateFrequency = 0.1f;
        
        private static readonly int _VRCLightVolumeTexture = Shader.PropertyToID("_VRCLightVolumeTexture");
        private static readonly int _VRCLightVolumeParams = Shader.PropertyToID("_VRCLightVolumeParams");
        private static readonly int _VRCLightVolumeWorldToLocal = Shader.PropertyToID("_VRCLightVolumeWorldToLocal");
        private static readonly int _VRCLightVolumeIntensity = Shader.PropertyToID("_VRCLightVolumeIntensity");
        private static readonly int _VRCLightVolumeTint = Shader.PropertyToID("_VRCLightVolumeTint");
        
        private List<Renderer> targetRenderers = new List<Renderer>();
        private List<Material> pcssMaterials = new List<Material>();
        private float lastUpdateTime;
        private Camera mainCamera;
        
        // VRC Light Volumes讀懷・逕ｨ縺ｮ繧ｭ繝ｼ繝ｯ繝ｼ繝・
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
                
                // VRChat繝代ヵ繧ｩ繝ｼ繝槭Φ繧ｹ逶｣隕・
                MonitorVRChatPerformance();
            }
        }
        
        /// <summary>
        /// VRChat繝代ヵ繧ｩ繝ｼ繝槭Φ繧ｹ逶｣隕・
        /// </summary>
        private void MonitorVRChatPerformance()
        {
            float currentFPS = 1f / Time.deltaTime;
            
            // 繝輔Ξ繝ｼ繝繝ｬ繝ｼ繝医′菴惹ｸ九＠縺溷ｴ蜷医・閾ｪ蜍墓怙驕ｩ蛹・
            if (currentFPS < 45f && !enableMobileOptimization)
            {
                Debug.LogWarning("VRC Light Volumes: Low framerate detected. Enabling mobile optimization.");
                enableMobileOptimization = true;
                ApplyMobileOptimizations();
            }
            
            // Quest迺ｰ蠅・〒縺ｮ霑ｽ蜉譛驕ｩ蛹・
            if (IsQuestEnvironment() && lightVolumeIntensity > 0.5f)
            {
                lightVolumeIntensity = Mathf.Min(lightVolumeIntensity, 0.5f);
                Debug.Log("VRC Light Volumes: Quest optimization applied - intensity reduced.");
            }
        }
        
        /// <summary>
        /// Quest迺ｰ蠅・､懷・
        /// </summary>
        private bool IsQuestEnvironment()
        {
            return SystemInfo.deviceModel.Contains("Oculus") || 
                   Application.platform == RuntimePlatform.Android;
        }
        
        /// <summary>
        /// 繝｢繝舌う繝ｫ譛驕ｩ蛹夜←逕ｨ
        /// </summary>
        private void ApplyMobileOptimizations()
        {
            maxLightVolumeDistance = Mathf.Min(maxLightVolumeDistance, 50);
            updateFrequency = Mathf.Max(updateFrequency, 0.2f);
            lightVolumeIntensity *= 0.8f;
            
            // VRChat Avatar Culling險ｭ螳壹ｒ閠・・
            ApplyVRChatCullingOptimizations();
        }
        
        /// <summary>
        /// VRChat Avatar Culling譛驕ｩ蛹・
        /// </summary>
        private void ApplyVRChatCullingOptimizations()
        {
            // VRChat縺ｮ"Hide Avatars Beyond"險ｭ螳壹ｒ讓｡蛟｣
            int avatarCount = CountNearbyAvatars();
            
            if (avatarCount > 10)
            {
                // 繧｢繝舌ち繝ｼ縺悟､壹＞蝣ｴ蜷医・Light Volume霍晞屬繧堤洒邵ｮ
                maxLightVolumeDistance = Mathf.Min(maxLightVolumeDistance, 30);
                lightVolumeIntensity *= 0.9f;
            }
        }
        
        /// <summary>
        /// 霑代￥縺ｮ繧｢繝舌ち繝ｼ謨ｰ繧偵き繧ｦ繝ｳ繝・
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
        /// 繧｢繝舌ち繝ｼ縺ｮ繝ｬ繝ｳ繝繝ｩ繝ｼ縺九メ繧ｧ繝・け
        /// </summary>
        private bool IsAvatarRenderer(Renderer renderer)
        {
            return renderer.gameObject.layer != LayerMask.NameToLayer("UI") &&
                   !renderer.gameObject.name.Contains("World") &&
                   renderer.gameObject.GetComponent<Animator>() != null;
        }
        
        /// <summary>
        /// VRC Light Volumes縺ｮ閾ｪ蜍墓､懷・縺ｨ蛻晄悄蛹・
        /// </summary>
        public void DetectAndInitialize()
        {
            if (!autoDetectLightVolumes) return;
            
            // 繧ｷ繝ｼ繝ｳ蜀・・Light Volume Manager繧呈､懃ｴ｢
            var lightVolumeManagers = FindObjectsOfType<MonoBehaviour>()
                .Where(mb => mb.GetType().Name.Contains("LightVolumeManager") || 
                            mb.GetType().Name.Contains("VRCLightVolume"))
                .ToArray();
            
            if (lightVolumeManagers.Length > 0)
            {
                Debug.Log($"[VRC Light Volumes] {lightVolumeManagers.Length} Light Volume Manager(s) detected");
                InitializeLightVolumeSupport();
            }
            
            // PCSS蟇ｾ蠢懊・繝・Μ繧｢繝ｫ繧呈､懃ｴ｢
            DetectPCSSMaterials();
        }
        
        /// <summary>
        /// PCSS蟇ｾ蠢懊・繝・Μ繧｢繝ｫ縺ｮ讀懷・
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
        /// 繝槭ユ繝ｪ繧｢繝ｫ縺訓CSS蟇ｾ蠢懊°繝√ぉ繝・け
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
        /// Light Volume 繧ｵ繝昴・繝医・蛻晄悄蛹・
        /// </summary>
        private void InitializeLightVolumeSupport()
        {
            // 繧ｰ繝ｭ繝ｼ繝舌Ν繧ｷ繧ｧ繝ｼ繝繝ｼ繧ｭ繝ｼ繝ｯ繝ｼ繝峨ｒ譛牙柑蛹・
            Shader.EnableKeyword("VRC_LIGHT_VOLUMES_ENABLED");
            
            // 蜩∬ｳｪ險ｭ螳壹↓蝓ｺ縺･縺乗怙驕ｩ蛹・
            if (enableMobileOptimization && IsMobilePlatform())
            {
                Shader.EnableKeyword("VRC_LIGHT_VOLUMES_MOBILE");
                lightVolumeIntensity *= 0.7f; // 繝｢繝舌う繝ｫ縺ｧ縺ｯ蠑ｷ蠎ｦ繧剃ｸ九￡繧・
            }
            
            Debug.Log("[VRC Light Volumes] Light Volume support initialized");
        }
        
        /// <summary>
        /// Light Volumes縺ｮ譖ｴ譁ｰ
        /// </summary>
        private void UpdateLightVolumes()
        {
            if (pcssMaterials.Count == 0) return;
            
            // 繧ｫ繝｡繝ｩ菴咲ｽｮ縺ｫ蝓ｺ縺･縺剰ｷ晞屬繧ｫ繝ｪ繝ｳ繧ｰ
            Vector3 cameraPos = mainCamera != null ? mainCamera.transform.position : Vector3.zero;
            
            foreach (var material in pcssMaterials)
            {
                if (material == null) continue;
                
                // VRC Light Volume 繝代Λ繝｡繝ｼ繧ｿ繧定ｨｭ螳・
                material.SetFloat(_VRCLightVolumeIntensity, lightVolumeIntensity);
                material.SetColor(_VRCLightVolumeTint, lightVolumeTint);
                
                // 霍晞屬繝吶・繧ｹ縺ｮ譛驕ｩ蛹・
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
        /// 繝｢繝舌う繝ｫ繝励Λ繝・ヨ繝輔か繝ｼ繝縺ｮ蛻､螳・
        /// </summary>
        private bool IsMobilePlatform()
        {
            return Application.isMobilePlatform || 
                   SystemInfo.deviceType == DeviceType.Handheld ||
                   QualitySettings.GetQualityLevel() <= 2;
        }
        
        /// <summary>
        /// VRC Light Volumes讖溯・縺ｮ譛牙柑/辟｡蜉ｹ蛻・ｊ譖ｿ縺・
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
        /// Light Volume蠑ｷ蠎ｦ縺ｮ險ｭ螳・
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
        /// Light Volume濶ｲ隱ｿ縺ｮ險ｭ螳・
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
        /// 邨ｱ險域ュ蝣ｱ縺ｮ蜿門ｾ・
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
            // 繧ｯ繝ｪ繝ｼ繝ｳ繧｢繝・・
            Shader.DisableKeyword("VRC_LIGHT_VOLUMES_ENABLED");
            Shader.DisableKeyword("VRC_LIGHT_VOLUMES_MOBILE");
        }
    }
    
    /// <summary>
    /// VRC Light Volumes邨ｱ險域ュ蝣ｱ
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
