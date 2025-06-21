using UnityEngine;
using UnityEngine.Rendering;
#if VRCHAT_SDK_AVAILABLE
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
#endif
#if MODULAR_AVATAR_AVAILABLE
using nadena.dev.modular_avatar.core;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LilToonPCSS.Runtime
{
    /// <summary>
    /// Professional-grade VRChat performance optimizer for PCSS materials
    /// Provides dynamic quality adjustment, real-time monitoring, and commercial-ready optimization
    /// </summary>
    [System.Serializable]
    public class VRChatPerformanceOptimizer : MonoBehaviour, IEditorOnly
    {
        [Header("Professional Performance Optimization")]
        [SerializeField] private bool enableAdvancedOptimization = true;
        [SerializeField] private bool enableDynamicQualityAdjustment = true;
        [SerializeField] private bool enableRealtimeMonitoring = true;
        [SerializeField] private bool enableCommercialOptimizations = true;
        
        [Header("Quality Profiles")]
        [SerializeField] private QualityProfile ultraQualityProfile;
        [SerializeField] private QualityProfile highQualityProfile;
        [SerializeField] private QualityProfile mediumQualityProfile;
        [SerializeField] private QualityProfile questQualityProfile;
        
        [Header("Performance Thresholds")]
        [SerializeField] private float targetFrameRate = 90.0f;
        [SerializeField] private float performanceThreshold = 0.8f;
        [SerializeField] private float memoryThreshold = 0.75f;
        [SerializeField] private int maxShaderInstructions = 300;
        
        [Header("Dynamic Optimization")]
        [SerializeField] private float optimizationInterval = 1.0f;
        [SerializeField] private bool enableFrameRateBasedOptimization = true;
        [SerializeField] private bool enableMemoryBasedOptimization = true;
        [SerializeField] private bool enableDistanceBasedOptimization = true;
        
        [Header("Commercial Features")]
        [SerializeField] private bool enableProfessionalAnalytics = true;
        [SerializeField] private bool enableAdvancedProfiling = true;
        [SerializeField] private bool enableCustomOptimizationProfiles = true;
        
        private Material[] targetMaterials;
        private Dictionary<Material, QualityProfile> materialProfiles;
        private PerformanceMonitor performanceMonitor;
        private OptimizationEngine optimizationEngine;
        private bool isInitialized = false;
        
        [System.Serializable]
        public class QualityProfile
        {
            public string name;
            public float shadowSoftness = 1.0f;
            public float shadowIntensity = 1.0f;
            public float shadowBias = 0.001f;
            public float normalBias = 0.1f;
            public int sampleCount = 16;
            public float searchRadius = 0.05f;
            public bool enableOptimization = true;
            public float distanceFade = 50.0f;
            public bool enableLOD = true;
            public float qualityMultiplier = 1.0f;
        }
        
        public class PerformanceMonitor
        {
            public float currentFrameRate { get; private set; }
            public float averageFrameRate { get; private set; }
            public float memoryUsage { get; private set; }
            public int activeShaderInstructions { get; private set; }
            public string performanceRank { get; private set; }
            
            private Queue<float> frameRateHistory = new Queue<float>();
            private const int historySize = 60;
            
            public void Update()
            {
                currentFrameRate = 1.0f / Time.unscaledDeltaTime;
                
                frameRateHistory.Enqueue(currentFrameRate);
                if (frameRateHistory.Count > historySize)
                    frameRateHistory.Dequeue();
                
                averageFrameRate = frameRateHistory.Average();
                
                UpdateMemoryUsage();
                UpdatePerformanceRank();
            }
            
            private void UpdateMemoryUsage()
            {
                memoryUsage = (float)System.GC.GetTotalMemory(false) / (1024 * 1024);
            }
            
            private void UpdatePerformanceRank()
            {
                if (averageFrameRate > 80) performanceRank = "Excellent";
                else if (averageFrameRate > 60) performanceRank = "Good";
                else if (averageFrameRate > 45) performanceRank = "Medium";
                else if (averageFrameRate > 30) performanceRank = "Poor";
                else performanceRank = "Very Poor";
            }
        }
        
        public class OptimizationEngine
        {
            private VRChatPerformanceOptimizer optimizer;
            
            public OptimizationEngine(VRChatPerformanceOptimizer optimizer)
            {
                this.optimizer = optimizer;
            }
            
            public void OptimizeMaterials()
            {
                if (optimizer.materialProfiles == null) return;
                
                foreach (var kvp in optimizer.materialProfiles)
                {
                    var material = kvp.Key;
                    var profile = kvp.Value;
                    
                    OptimizeMaterial(material, profile);
                }
            }
            
            private void OptimizeMaterial(Material material, QualityProfile profile)
            {
                if (material == null || profile == null) return;
                
                // フレームレートベースの最適化
                if (optimizer.enableFrameRateBasedOptimization)
                {
                    ApplyFrameRateOptimization(material, profile);
                }
                
                // メモリベースの最適化
                if (optimizer.enableMemoryBasedOptimization)
                {
                    ApplyMemoryOptimization(material, profile);
                }
                
                // 距離ベースの最適化
                if (optimizer.enableDistanceBasedOptimization)
                {
                    ApplyDistanceOptimization(material, profile);
                }
            }
            
            private void ApplyFrameRateOptimization(Material material, QualityProfile profile)
            {
                var frameRate = optimizer.performanceMonitor.averageFrameRate;
                var targetFrameRate = optimizer.targetFrameRate;
                
                if (frameRate < targetFrameRate * optimizer.performanceThreshold)
                {
                    // パフォーマンスが閾値を下回った場合、品質を下げる
                    var qualityReduction = 1.0f - (frameRate / targetFrameRate);
                    
                    material.SetFloat("_PCSSSampleCount", 
                        Mathf.Max(4, profile.sampleCount * (1.0f - qualityReduction * 0.5f)));
                    material.SetFloat("_PCSSSearchRadius", 
                        profile.searchRadius * (1.0f - qualityReduction * 0.3f));
                }
            }
            
            private void ApplyMemoryOptimization(Material material, QualityProfile profile)
            {
                var memoryUsage = optimizer.performanceMonitor.memoryUsage;
                var memoryThreshold = optimizer.memoryThreshold * 1024; // MB to bytes
                
                if (memoryUsage > memoryThreshold)
                {
                    // メモリ使用量が閾値を超えた場合、最適化を適用
                    material.SetFloat("_PCSSAdaptiveQuality", 1.0f);
                    material.SetFloat("_PCSSLODOptimization", 1.0f);
                }
            }
            
            private void ApplyDistanceOptimization(Material material, QualityProfile profile)
            {
                // カメラからの距離に基づく最適化
                var camera = Camera.main;
                if (camera != null)
                {
                    var distance = Vector3.Distance(camera.transform.position, 
                        optimizer.transform.position);
                    
                    if (distance > profile.distanceFade)
                    {
                        var fadeAmount = Mathf.Clamp01((distance - profile.distanceFade) / profile.distanceFade);
                        material.SetFloat("_PCSSDistanceFade", fadeAmount);
                    }
                }
            }
        }
        
        private void Awake()
        {
            InitializeProfiles();
            InitializeOptimizer();
        }
        
        private void Start()
        {
            if (enableAdvancedOptimization)
            {
                StartCoroutine(OptimizationLoop());
            }
            
            if (enableRealtimeMonitoring)
            {
                StartCoroutine(MonitoringLoop());
            }
        }
        
        private void InitializeProfiles()
        {
            ultraQualityProfile = new QualityProfile
            {
                name = "Ultra Quality",
                shadowSoftness = 2.0f,
                shadowIntensity = 1.2f,
                shadowBias = 0.0005f,
                normalBias = 0.05f,
                sampleCount = 32,
                searchRadius = 0.1f,
                enableOptimization = false,
                distanceFade = 100.0f,
                enableLOD = false,
                qualityMultiplier = 1.5f
            };
            
            highQualityProfile = new QualityProfile
            {
                name = "High Quality",
                shadowSoftness = 1.5f,
                shadowIntensity = 1.0f,
                shadowBias = 0.001f,
                normalBias = 0.1f,
                sampleCount = 24,
                searchRadius = 0.075f,
                enableOptimization = true,
                distanceFade = 75.0f,
                enableLOD = true,
                qualityMultiplier = 1.2f
            };
            
            mediumQualityProfile = new QualityProfile
            {
                name = "Medium Quality",
                shadowSoftness = 1.0f,
                shadowIntensity = 0.8f,
                shadowBias = 0.002f,
                normalBias = 0.15f,
                sampleCount = 16,
                searchRadius = 0.05f,
                enableOptimization = true,
                distanceFade = 50.0f,
                enableLOD = true,
                qualityMultiplier = 1.0f
            };
            
            questQualityProfile = new QualityProfile
            {
                name = "Quest Quality",
                shadowSoftness = 0.8f,
                shadowIntensity = 0.6f,
                shadowBias = 0.003f,
                normalBias = 0.2f,
                sampleCount = 8,
                searchRadius = 0.03f,
                enableOptimization = true,
                distanceFade = 30.0f,
                enableLOD = true,
                qualityMultiplier = 0.7f
            };
        }
        
        private void InitializeOptimizer()
        {
            performanceMonitor = new PerformanceMonitor();
            optimizationEngine = new OptimizationEngine(this);
            
            // ターゲットマテリアルの検索
            var renderers = GetComponentsInChildren<Renderer>();
            var materials = new List<Material>();
            
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    if (material.shader.name.Contains("lilToon") && 
                        material.shader.name.Contains("PCSS"))
                    {
                        materials.Add(material);
                    }
                }
            }
            
            targetMaterials = materials.ToArray();
            materialProfiles = new Dictionary<Material, QualityProfile>();
            
            // デフォルトプロファイルの適用
            foreach (var material in targetMaterials)
            {
                materialProfiles[material] = GetOptimalProfile();
            }
            
            isInitialized = true;
        }
        
        private QualityProfile GetOptimalProfile()
        {
            // VRChatプラットフォーム検出
            if (IsVRChatQuest())
            {
                return questQualityProfile;
            }
            else if (IsVRChatPC())
            {
                return highQualityProfile;
            }
            else
            {
                return mediumQualityProfile;
            }
        }
        
        private bool IsVRChatQuest()
        {
            #if UNITY_ANDROID
            return true;
            #else
            return false;
            #endif
        }
        
        private bool IsVRChatPC()
        {
            #if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            return true;
            #else
            return false;
            #endif
        }
        
        private IEnumerator OptimizationLoop()
        {
            while (enabled)
            {
                yield return new WaitForSeconds(optimizationInterval);
                
                if (isInitialized && enableDynamicQualityAdjustment)
                {
                    optimizationEngine.OptimizeMaterials();
                }
            }
        }
        
        private IEnumerator MonitoringLoop()
        {
            while (enabled)
            {
                yield return new WaitForEndOfFrame();
                
                if (performanceMonitor != null)
                {
                    performanceMonitor.Update();
                    
                    if (enableProfessionalAnalytics)
                    {
                        LogPerformanceMetrics();
                    }
                }
            }
        }
        
        private void LogPerformanceMetrics()
        {
            if (Time.frameCount % 300 == 0) // 5秒間隔でログ出力
            {
                Debug.Log($"[PCSS Professional] Performance: {performanceMonitor.performanceRank} " +
                         $"(FPS: {performanceMonitor.averageFrameRate:F1}, " +
                         $"Memory: {performanceMonitor.memoryUsage:F1}MB)");
            }
        }
        
        /// <summary>
        /// 手動でプロファイルを適用
        /// </summary>
        public void ApplyProfile(QualityProfile profile)
        {
            if (!isInitialized || profile == null) return;
            
            foreach (var material in targetMaterials)
            {
                if (material != null)
                {
                    materialProfiles[material] = profile;
                    ApplyProfileToMaterial(material, profile);
                }
            }
        }
        
        private void ApplyProfileToMaterial(Material material, QualityProfile profile)
        {
            material.SetFloat("_PCSSSoftness", profile.shadowSoftness);
            material.SetFloat("_PCSSIntensity", profile.shadowIntensity);
            material.SetFloat("_PCSSShadowBias", profile.shadowBias);
            material.SetFloat("_PCSSNormalBias", profile.normalBias);
            material.SetFloat("_PCSSSampleCount", profile.sampleCount);
            material.SetFloat("_PCSSSearchRadius", profile.searchRadius);
            material.SetFloat("_PCSSAdaptiveQuality", profile.enableOptimization ? 1.0f : 0.0f);
            material.SetFloat("_PCSSDistanceFade", profile.distanceFade);
            material.SetFloat("_PCSSLODOptimization", profile.enableLOD ? 1.0f : 0.0f);
        }
        
        /// <summary>
        /// パフォーマンス統計の取得
        /// </summary>
        public PerformanceStats GetPerformanceStats()
        {
            if (performanceMonitor == null) return new PerformanceStats();
            
            return new PerformanceStats
            {
                frameRate = performanceMonitor.averageFrameRate,
                memoryUsage = performanceMonitor.memoryUsage,
                performanceRank = performanceMonitor.performanceRank,
                shaderInstructions = performanceMonitor.activeShaderInstructions,
                optimizationLevel = GetCurrentOptimizationLevel()
            };
        }
        
        private float GetCurrentOptimizationLevel()
        {
            if (materialProfiles == null || materialProfiles.Count == 0) return 1.0f;
            
            var totalQuality = materialProfiles.Values.Sum(p => p.qualityMultiplier);
            return totalQuality / materialProfiles.Count;
        }
        
        [System.Serializable]
        public struct PerformanceStats
        {
            public float frameRate;
            public float memoryUsage;
            public string performanceRank;
            public int shaderInstructions;
            public float optimizationLevel;
        }
        
        /// <summary>
        /// 商用版限定: 高度な最適化設定
        /// </summary>
        public void EnableProfessionalOptimizations()
        {
            if (!enableCommercialOptimizations) return;
            
            enableAdvancedOptimization = true;
            enableDynamicQualityAdjustment = true;
            enableRealtimeMonitoring = true;
            enableProfessionalAnalytics = true;
            enableAdvancedProfiling = true;
            
            // 商用版限定の高度な最適化を適用
            ApplyCommercialOptimizations();
        }
        
        private void ApplyCommercialOptimizations()
        {
            // 商用版限定の最適化ロジック
            foreach (var material in targetMaterials)
            {
                if (material != null)
                {
                    // 高度なシェーダー最適化
                    material.SetFloat("_PCSSCommercialOptimization", 1.0f);
                    material.SetFloat("_PCSSProfessionalQuality", 1.0f);
                    
                    // 商用版限定のキーワード有効化
                    material.EnableKeyword("PCSS_PROFESSIONAL_EDITION");
                    material.EnableKeyword("PCSS_COMMERCIAL_OPTIMIZATION");
                }
            }
            
            Debug.Log("[PCSS Professional] Commercial optimizations enabled");
        }
        
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying && isInitialized)
            {
                // エディタでの値変更を即座に反映
                optimizationEngine?.OptimizeMaterials();
            }
        }
        #endif
    }
} 