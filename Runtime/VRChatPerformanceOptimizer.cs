using UnityEngine;
using UnityEngine.Rendering;
#if VRCHAT_SDK_AVAILABLE
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
#endif

// ModularAvatar依存関係の完全な条件付き対応
#if UNITY_EDITOR && MODULAR_AVATAR_AVAILABLE
    using nadena.dev.modular_avatar.core;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace lilToon.PCSS.Runtime
{
    /// <summary>
    /// VRChat環境でのPCSSパフォーマンス最適化システム
    /// </summary>
    public class VRChatPerformanceOptimizer : MonoBehaviour
    {
        [Header("🎯 PCSS Performance Settings")]
        [SerializeField] private bool enableAutoOptimization = true;
        [SerializeField] private QualityProfile currentProfile = QualityProfile.Auto;
        [SerializeField] private bool enableFrameRateMonitoring = true;
        [SerializeField] private float targetFrameRate = 90f;

        [Header("📊 Runtime Statistics")]
        [SerializeField] private int currentAvatarCount = 0;
        [SerializeField] private float currentFrameRate = 0f;
        [SerializeField] private int pcssMaterialCount = 0;

        [Header("⚙️ Advanced Settings")]
        [SerializeField] private bool enableQuestOptimization = true;
        [SerializeField] private bool enableVROptimization = true;
        [SerializeField] private bool enableDynamicQuality = true;

        // パフォーマンス品質プロファイル
        public enum QualityProfile
        {
            Auto = 0,           // 自動調整
            Maximum = 1,        // 最高品質
            High = 2,           // 高品質
            Medium = 3,         // 中品質
            Low = 4,            // 低品質
            Quest = 5           // Quest専用
        }

        // 最適化統計
        [System.Serializable]
        public class OptimizationStats
        {
            public int optimizedMaterials;
            public int totalMaterials;
            public float averageFrameRate;
            public float optimizationLevel;
            public string qualityProfile;
            public bool isQuestMode;
        }

        private OptimizationStats stats = new OptimizationStats();
        private List<Material> trackedMaterials = new List<Material>();
        private float frameRateSum = 0f;
        private int frameRateCount = 0;

        void Start()
        {
            InitializeOptimizer();
        }

        void Update()
        {
            if (enableAutoOptimization)
            {
                UpdatePerformanceMetrics();
                CheckAndOptimize();
            }
        }

        /// <summary>
        /// オプティマイザーの初期化
        /// </summary>
        private void InitializeOptimizer()
        {
            // VRChat環境の検出
            bool isVRChat = DetectVRChatEnvironment();
            bool isQuest = DetectQuestPlatform();

            if (isQuest)
            {
                currentProfile = QualityProfile.Quest;
                enableQuestOptimization = true;
            }

            // 初期最適化の実行
            OptimizeAllMaterials();

            Debug.Log($"🎯 PCSS Performance Optimizer initialized - Profile: {currentProfile}, Quest: {isQuest}");
        }

        /// <summary>
        /// パフォーマンスメトリクスの更新
        /// </summary>
        private void UpdatePerformanceMetrics()
        {
            if (enableFrameRateMonitoring)
            {
                currentFrameRate = 1f / Time.unscaledDeltaTime;
                frameRateSum += currentFrameRate;
                frameRateCount++;

                if (frameRateCount >= 60) // 1秒間の平均を計算
                {
                    stats.averageFrameRate = frameRateSum / frameRateCount;
                    frameRateSum = 0f;
                    frameRateCount = 0;
                }
            }

            // アバター数の更新
            UpdateAvatarCount();
        }

        /// <summary>
        /// 最適化チェックと実行
        /// </summary>
        private void CheckAndOptimize()
        {
            if (!enableDynamicQuality) return;

            // フレームレートベースの最適化
            if (stats.averageFrameRate < targetFrameRate * 0.8f)
            {
                // パフォーマンス低下時の最適化
                OptimizeForLowPerformance();
            }
            else if (stats.averageFrameRate > targetFrameRate * 1.1f)
            {
                // パフォーマンス余裕時の品質向上
                OptimizeForHighPerformance();
            }
        }

        /// <summary>
        /// 全マテリアルの最適化
        /// </summary>
        public void OptimizeAllMaterials()
        {
            trackedMaterials.Clear();
            
            // シーン内のすべてのPCSS対応マテリアルを検索
            var allRenderers = FindObjectsOfType<Renderer>();
            
            foreach (var renderer in allRenderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null && PCSSUtilities.IsPCSSCompatible(material))
                    {
                        trackedMaterials.Add(material);
                        OptimizeMaterial(material);
                    }
                }
            }

            pcssMaterialCount = trackedMaterials.Count;
            stats.optimizedMaterials = trackedMaterials.Count;
            stats.totalMaterials = allRenderers.SelectMany(r => r.sharedMaterials).Count(m => m != null);
            stats.qualityProfile = currentProfile.ToString();
            stats.isQuestMode = enableQuestOptimization;

            Debug.Log($"📊 Optimized {stats.optimizedMaterials} PCSS materials out of {stats.totalMaterials} total materials");
        }

        /// <summary>
        /// 個別マテリアルの最適化
        /// </summary>
        private void OptimizeMaterial(Material material)
        {
            if (material == null) return;

            var qualityParams = GetQualityParameters(currentProfile);
            
            // プリセットモードを設定
            if (material.HasProperty("_PCSSPresetMode"))
            {
                material.SetFloat("_PCSSPresetMode", qualityParams.presetMode);
            }

            // 品質パラメータを適用
            ApplyQualityParameters(material, qualityParams);

            // プラットフォーム固有の最適化
            if (enableQuestOptimization && DetectQuestPlatform())
            {
                ApplyQuestOptimizations(material);
            }

            if (enableVROptimization && DetectVREnvironment())
            {
                ApplyVROptimizations(material);
            }
        }

        /// <summary>
        /// 品質プロファイルのパラメータを取得
        /// </summary>
        private QualityParameters GetQualityParameters(QualityProfile profile)
        {
            switch (profile)
            {
                case QualityProfile.Maximum:
                    return new QualityParameters
                    {
                        filterRadius = 0.03f,
                        lightSize = 0.25f,
                        bias = 0.0005f,
                        intensity = 1.2f,
                        sampleCount = 32,
                        presetMode = 0 // リアル
                    };

                case QualityProfile.High:
                    return new QualityParameters
                    {
                        filterRadius = 0.02f,
                        lightSize = 0.15f,
                        bias = 0.001f,
                        intensity = 1.0f,
                        sampleCount = 24,
                        presetMode = 0 // リアル
                    };

                case QualityProfile.Medium:
                    return new QualityParameters
                    {
                        filterRadius = 0.015f,
                        lightSize = 0.1f,
                        bias = 0.001f,
                        intensity = 0.8f,
                        sampleCount = 16,
                        presetMode = 1 // アニメ
                    };

                case QualityProfile.Low:
                    return new QualityParameters
                    {
                        filterRadius = 0.01f,
                        lightSize = 0.05f,
                        bias = 0.002f,
                        intensity = 0.6f,
                        sampleCount = 8,
                        presetMode = 1 // アニメ
                    };

                case QualityProfile.Quest:
                    return new QualityParameters
                    {
                        filterRadius = 0.005f,
                        lightSize = 0.03f,
                        bias = 0.003f,
                        intensity = 0.5f,
                        sampleCount = 6,
                        presetMode = 1 // アニメ
                    };

                case QualityProfile.Auto:
                default:
                    return GetAutoQualityParameters();
            }
        }

        /// <summary>
        /// 自動品質パラメータの決定
        /// </summary>
        private QualityParameters GetAutoQualityParameters()
        {
            // システム性能とアバター数に基づいて自動決定
            bool isQuest = DetectQuestPlatform();
            bool isVR = DetectVREnvironment();
            
            if (isQuest)
            {
                return GetQualityParameters(QualityProfile.Quest);
            }
            
            if (currentAvatarCount > 15)
            {
                return GetQualityParameters(QualityProfile.Low);
            }
            else if (currentAvatarCount > 8)
            {
                return GetQualityParameters(QualityProfile.Medium);
            }
            else if (isVR)
            {
                return GetQualityParameters(QualityProfile.High);
            }
            else
            {
                return GetQualityParameters(QualityProfile.Maximum);
            }
        }

        /// <summary>
        /// 品質パラメータをマテリアルに適用
        /// </summary>
        private void ApplyQualityParameters(Material material, QualityParameters parameters)
        {
            if (material.HasProperty("_PCSSFilterRadius"))
                material.SetFloat("_PCSSFilterRadius", parameters.filterRadius);
            
            if (material.HasProperty("_PCSSLightSize"))
                material.SetFloat("_PCSSLightSize", parameters.lightSize);
            
            if (material.HasProperty("_PCSSBias"))
                material.SetFloat("_PCSSBias", parameters.bias);
            
            if (material.HasProperty("_PCSSIntensity"))
                material.SetFloat("_PCSSIntensity", parameters.intensity);

            if (material.HasProperty("_PCSSSampleCount"))
                material.SetFloat("_PCSSSampleCount", parameters.sampleCount);

            // PCSS機能を有効化
            if (material.HasProperty("_PCSSEnabled"))
                material.SetFloat("_PCSSEnabled", 1.0f);
        }

        /// <summary>
        /// Quest専用最適化
        /// </summary>
        private void ApplyQuestOptimizations(Material material)
        {
            // Quest用の超軽量設定
            if (material.HasProperty("_PCSSFilterRadius"))
                material.SetFloat("_PCSSFilterRadius", Mathf.Min(material.GetFloat("_PCSSFilterRadius"), 0.005f));
            
            if (material.HasProperty("_PCSSSampleCount"))
                material.SetFloat("_PCSSSampleCount", Mathf.Min(material.GetFloat("_PCSSSampleCount"), 6f));

            // Quest用シェーダーキーワード
            if (material.shader.name.Contains("PCSS"))
            {
                material.EnableKeyword("QUEST_OPTIMIZATION");
            }
        }

        /// <summary>
        /// VR専用最適化
        /// </summary>
        private void ApplyVROptimizations(Material material)
        {
            // VR用フレームレート優先設定
            if (material.HasProperty("_PCSSFilterRadius"))
            {
                float currentRadius = material.GetFloat("_PCSSFilterRadius");
                material.SetFloat("_PCSSFilterRadius", currentRadius * 0.85f);
            }
            
            if (material.HasProperty("_PCSSSampleCount"))
            {
                float currentSamples = material.GetFloat("_PCSSSampleCount");
                material.SetFloat("_PCSSSampleCount", Mathf.Min(currentSamples, 20f));
            }
        }

        /// <summary>
        /// 低パフォーマンス時の最適化
        /// </summary>
        private void OptimizeForLowPerformance()
        {
            // 品質プロファイルを一段階下げる
            if (currentProfile > QualityProfile.Quest)
            {
                currentProfile = (QualityProfile)((int)currentProfile + 1);
                OptimizeAllMaterials();
                Debug.Log($"⚡ Performance optimization: Reduced quality to {currentProfile}");
            }
        }

        /// <summary>
        /// 高パフォーマンス時の品質向上
        /// </summary>
        private void OptimizeForHighPerformance()
        {
            // 品質プロファイルを一段階上げる
            if (currentProfile > QualityProfile.Maximum && currentProfile < QualityProfile.Quest)
            {
                currentProfile = (QualityProfile)((int)currentProfile - 1);
                OptimizeAllMaterials();
                Debug.Log($"✨ Performance optimization: Increased quality to {currentProfile}");
            }
        }

        /// <summary>
        /// アバター数の更新
        /// </summary>
        private void UpdateAvatarCount()
        {
            var avatarDescriptors = FindObjectsOfType<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>();
            currentAvatarCount = avatarDescriptors.Length;
        }

        /// <summary>
        /// VRChat環境の検出
        /// </summary>
        private bool DetectVRChatEnvironment()
        {
            return FindObjectOfType<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>() != null;
        }

        /// <summary>
        /// Quest プラットフォームの検出
        /// </summary>
        private bool DetectQuestPlatform()
        {
            return SystemInfo.deviceModel.Contains("Oculus") || 
                   Application.platform == RuntimePlatform.Android;
        }

        /// <summary>
        /// VR環境の検出
        /// </summary>
        private bool DetectVREnvironment()
        {
            return UnityEngine.XR.XRSettings.enabled;
        }

        /// <summary>
        /// 統計情報の取得
        /// </summary>
        public OptimizationStats GetStats()
        {
            stats.optimizationLevel = ((int)QualityProfile.Quest - (int)currentProfile) / (float)(int)QualityProfile.Quest;
            return stats;
        }

        /// <summary>
        /// 手動で品質プロファイルを設定
        /// </summary>
        public void SetQualityProfile(QualityProfile profile)
        {
            currentProfile = profile;
            OptimizeAllMaterials();
            Debug.Log($"🎯 Manual quality profile set to: {profile}");
        }

        /// <summary>
        /// 最適化のリセット
        /// </summary>
        public void ResetOptimization()
        {
            currentProfile = QualityProfile.Auto;
            OptimizeAllMaterials();
            Debug.Log("🔄 PCSS optimization reset to Auto");
        }

        // 品質パラメータ構造体
        [System.Serializable]
        private struct QualityParameters
        {
            public float filterRadius;
            public float lightSize;
            public float bias;
            public float intensity;
            public int sampleCount;
            public int presetMode;
        }

        // デバッグ用GUI表示
        void OnGUI()
        {
            if (!Application.isEditor) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("🎯 PCSS Performance Monitor", EditorStyles.boldLabel);
            GUILayout.Label($"Profile: {currentProfile}");
            GUILayout.Label($"FPS: {currentFrameRate:F1} (Target: {targetFrameRate})");
            GUILayout.Label($"Avatars: {currentAvatarCount}");
            GUILayout.Label($"PCSS Materials: {pcssMaterialCount}");
            GUILayout.Label($"Optimization Level: {stats.optimizationLevel:P0}");
            
            if (GUILayout.Button("Force Optimize"))
            {
                OptimizeAllMaterials();
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
} 