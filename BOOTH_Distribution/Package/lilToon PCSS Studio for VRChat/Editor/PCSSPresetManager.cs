using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// PCSS効果のプリセット管理システム
    /// リアル影、アニメ風、映画風のプリセットを提供
    /// </summary>
    public class PCSSPresetManager : EditorWindow
    {
        #region プリセット定義
        
        [System.Serializable]
        public class PCSSPreset
        {
            public string name;
            public string description;
            public PCSSSettings settings;
            public Texture2D previewTexture;
            public bool isCustom;
        }
        
        [System.Serializable]
        public class PCSSSettings
        {
            [Header("影の基本設定")]
            [Range(0.1f, 10.0f)]
            public float shadowDistance = 2.0f;
            
            [Range(0.01f, 1.0f)]
            public float shadowSoftness = 0.5f;
            
            [Range(0.1f, 5.0f)]
            public float shadowIntensity = 1.0f;
            
            [Header("PCSS詳細設定")]
            [Range(1, 64)]
            public int shadowSamples = 16;
            
            [Range(0.1f, 2.0f)]
            public float filterRadius = 1.0f;
            
            [Range(0.0f, 1.0f)]
            public float shadowBias = 0.05f;
            
            [Header("色と効果")]
            public Color shadowColor = Color.black;
            
            [Range(0.0f, 1.0f)]
            public float ambientOcclusion = 0.3f;
            
            [Header("外部ライト設定")]
            public bool enableExternalLights = true;
            
            [Range(0.1f, 5.0f)]
            public float externalLightIntensity = 1.0f;
            
            public Color externalLightColor = Color.white;
            
            [Range(0.0f, 360.0f)]
            public float externalLightAngle = 45.0f;
            
            [Header("アニメ風設定")]
            public bool enableAnimeStyle = false;
            
            [Range(0.0f, 1.0f)]
            public float celShadingThreshold = 0.5f;
            
            [Range(0.0f, 1.0f)]
            public float celShadingSmoothness = 0.1f;
            
            [Header("映画風設定")]
            public bool enableCinematicStyle = false;
            
            [Range(0.0f, 1.0f)]
            public float filmGrain = 0.1f;
            
            [Range(0.0f, 1.0f)]
            public float vignette = 0.2f;
            
            [Range(0.0f, 1.0f)]
            public float bloom = 0.3f;

            [Header("艶の設定")]
            public bool enableDewyGloss = false;

            [Range(0.0f, 1.0f)]
            public float glossCoherence = 0.55f;

            [Range(0.0f, 2.0f)]
            public float glossBoost = 0.35f;

            [Range(0.0f, 1.0f)]
            public float glossShadowSuppression = 0.45f;

            [Range(0.0f, 2.0f)]
            public float glossRimStrength = 0.35f;

            [Range(0.0f, 1.0f)]
            public float glossSmoothness = 0.72f;

            [Header("Soft Flush")]
            public bool enableSoftFlush = false;
            public bool enableStudioBoost = false;
            public bool enableExcitedTone = false;

            public Color softFlushColor = new Color(1.0f, 0.40f, 0.36f, 1.0f);

            [Range(0.0f, 1.0f)]
            public float softFlushStrength = 0.34f;

            [Range(0.0f, 1.0f)]
            public float softFlushWidth = 0.56f;

            [Range(0.0f, 1.0f)]
            public float softFlushVerticalBias = 0.46f;

            public Color excitedToneColor = new Color(1.0f, 0.48f, 0.34f, 1.0f);

            [Range(0.0f, 1.0f)]
            public float excitedToneStrength = 0.28f;

            [Range(0.0f, 1.0f)]
            public float excitedToneBreath = 0.0f;

            [Range(0.0f, 1.0f)]
            public float excitedToneUpperBias = 0.58f;
        }
        
        #endregion
        
        #region プリセットデータ
        
        private static readonly PCSSPreset[] DefaultPresets = new PCSSPreset[]
        {
            new PCSSPreset
            {
                name = "リアル影",
                description = "現実的な影の表現。自然な光の反射と影の柔らかさを再現します。",
                settings = new PCSSSettings
                {
                    shadowDistance = 3.0f,
                    shadowSoftness = 0.7f,
                    shadowIntensity = 1.2f,
                    shadowSamples = 32,
                    filterRadius = 1.5f,
                    shadowBias = 0.03f,
                    shadowColor = new Color(0.1f, 0.1f, 0.15f, 1.0f),
                    ambientOcclusion = 0.4f,
                    enableExternalLights = true,
                    externalLightIntensity = 1.5f,
                    externalLightColor = new Color(1.0f, 0.95f, 0.9f, 1.0f),
                    externalLightAngle = 30.0f,
                    enableAnimeStyle = false,
                    enableCinematicStyle = false
                }
            },
            
            new PCSSPreset
            {
                name = "アニメ風",
                description = "アニメやマンガ風の影の表現。セルシェーディングとクリアな影を特徴とします。",
                settings = new PCSSSettings
                {
                    shadowDistance = 1.5f,
                    shadowSoftness = 0.3f,
                    shadowIntensity = 0.8f,
                    shadowSamples = 16,
                    filterRadius = 0.8f,
                    shadowBias = 0.08f,
                    shadowColor = new Color(0.2f, 0.2f, 0.3f, 1.0f),
                    ambientOcclusion = 0.2f,
                    enableExternalLights = true,
                    externalLightIntensity = 1.2f,
                    externalLightColor = new Color(1.0f, 1.0f, 1.0f, 1.0f),
                    externalLightAngle = 60.0f,
                    enableAnimeStyle = true,
                    celShadingThreshold = 0.6f,
                    celShadingSmoothness = 0.05f,
                    enableCinematicStyle = false
                }
            },
            
            new PCSSPreset
            {
                name = "映画風",
                description = "映画やドラマ風の影の表現。ドラマチックな光と影のコントラストを特徴とします。",
                settings = new PCSSSettings
                {
                    shadowDistance = 4.0f,
                    shadowSoftness = 0.9f,
                    shadowIntensity = 1.5f,
                    shadowSamples = 48,
                    filterRadius = 2.0f,
                    shadowBias = 0.02f,
                    shadowColor = new Color(0.05f, 0.05f, 0.1f, 1.0f),
                    ambientOcclusion = 0.6f,
                    enableExternalLights = true,
                    externalLightIntensity = 2.0f,
                    externalLightColor = new Color(1.0f, 0.9f, 0.8f, 1.0f),
                    externalLightAngle = 25.0f,
                    enableAnimeStyle = false,
                    enableCinematicStyle = true,
                    filmGrain = 0.15f,
                    vignette = 0.3f,
                    bloom = 0.4f
                }
            },
            
            new PCSSPreset
            {
                name = "ゲーム風",
                description = "ゲーム向けの最適化された影の表現。パフォーマンスと視覚的品質のバランスを重視します。",
                settings = new PCSSSettings
                {
                    shadowDistance = 2.0f,
                    shadowSoftness = 0.5f,
                    shadowIntensity = 1.0f,
                    shadowSamples = 16,
                    filterRadius = 1.0f,
                    shadowBias = 0.05f,
                    shadowColor = new Color(0.15f, 0.15f, 0.2f, 1.0f),
                    ambientOcclusion = 0.25f,
                    enableExternalLights = true,
                    externalLightIntensity = 1.0f,
                    externalLightColor = new Color(1.0f, 1.0f, 1.0f, 1.0f),
                    externalLightAngle = 45.0f,
                    enableAnimeStyle = false,
                    enableCinematicStyle = false
                }
            },
            
            new PCSSPreset
            {
                name = "ポートレート風",
                description = "ポートレート撮影風の影の表現。美しい肌の質感と自然な光を再現します。",
                settings = new PCSSSettings
                {
                    shadowDistance = 2.5f,
                    shadowSoftness = 0.8f,
                    shadowIntensity = 1.1f,
                    shadowSamples = 24,
                    filterRadius = 1.3f,
                    shadowBias = 0.04f,
                    shadowColor = new Color(0.12f, 0.12f, 0.18f, 1.0f),
                    ambientOcclusion = 0.35f,
                    enableExternalLights = true,
                    externalLightIntensity = 1.8f,
                    externalLightColor = new Color(1.0f, 0.98f, 0.95f, 1.0f),
                    externalLightAngle = 35.0f,
                    enableAnimeStyle = false,
                    enableCinematicStyle = true,
                    filmGrain = 0.05f,
                    vignette = 0.15f,
                    bloom = 0.2f
                }
            },
            
            new PCSSPreset
            {
                name = "うるみ艶肌",
                description = "汗ばみのような細いハイライトと、しっとりした肌の艶を加える健全な撮影向けプリセットです。",
                settings = new PCSSSettings
                {
                    shadowDistance = 2.2f,
                    shadowSoftness = 0.72f,
                    shadowIntensity = 0.95f,
                    shadowSamples = 24,
                    filterRadius = 1.15f,
                    shadowBias = 0.035f,
                    shadowColor = new Color(0.11f, 0.10f, 0.13f, 1.0f),
                    ambientOcclusion = 0.30f,
                    enableExternalLights = false,
                    externalLightIntensity = 1.0f,
                    externalLightColor = new Color(1.0f, 0.96f, 0.92f, 1.0f),
                    externalLightAngle = 35.0f,
                    enableAnimeStyle = false,
                    enableCinematicStyle = true,
                    filmGrain = 0.02f,
                    vignette = 0.10f,
                    bloom = 0.18f,
                    enableDewyGloss = true,
                    glossCoherence = 0.76f,
                    glossBoost = 0.66f,
                    glossShadowSuppression = 0.40f,
                    glossRimStrength = 0.48f,
                    glossSmoothness = 0.88f
                }
            },

            new PCSSPreset
            {
                name = "Soft Flush Skin",
                description = "Healthy warm cheek flush for portrait avatars, using soft shadows and restrained skin gloss without adding avatar lights.",
                settings = new PCSSSettings
                {
                    shadowDistance = 2.0f,
                    shadowSoftness = 0.78f,
                    shadowIntensity = 0.92f,
                    shadowSamples = 24,
                    filterRadius = 1.20f,
                    shadowBias = 0.035f,
                    shadowColor = new Color(0.16f, 0.10f, 0.12f, 1.0f),
                    ambientOcclusion = 0.26f,
                    enableExternalLights = false,
                    externalLightIntensity = 0.9f,
                    externalLightColor = new Color(1.0f, 0.93f, 0.90f, 1.0f),
                    externalLightAngle = 38.0f,
                    enableAnimeStyle = false,
                    enableCinematicStyle = true,
                    filmGrain = 0.01f,
                    vignette = 0.08f,
                    bloom = 0.12f,
                    enableDewyGloss = true,
                    glossCoherence = 0.68f,
                    glossBoost = 0.42f,
                    glossShadowSuppression = 0.46f,
                    glossRimStrength = 0.34f,
                    glossSmoothness = 0.78f,
                    enableSoftFlush = true,
                    softFlushColor = new Color(1.0f, 0.40f, 0.36f, 1.0f),
                    softFlushStrength = 0.34f,
                    softFlushWidth = 0.56f,
                    softFlushVerticalBias = 0.46f
                }
            },

            new PCSSPreset
            {
                name = "Excited Tone",
                description = "Healthy high-energy warm tone for portrait avatars, replacing adult wording with an ethical excited-color workflow.",
                settings = new PCSSSettings
                {
                    shadowDistance = 2.15f,
                    shadowSoftness = 0.76f,
                    shadowIntensity = 1.02f,
                    shadowSamples = 24,
                    filterRadius = 1.22f,
                    shadowBias = 0.034f,
                    shadowColor = new Color(0.18f, 0.10f, 0.11f, 1.0f),
                    ambientOcclusion = 0.28f,
                    enableExternalLights = false,
                    externalLightIntensity = 0.95f,
                    externalLightColor = new Color(1.0f, 0.94f, 0.90f, 1.0f),
                    externalLightAngle = 36.0f,
                    enableAnimeStyle = false,
                    enableCinematicStyle = true,
                    filmGrain = 0.01f,
                    vignette = 0.08f,
                    bloom = 0.14f,
                    enableDewyGloss = true,
                    glossCoherence = 0.72f,
                    glossBoost = 0.50f,
                    glossShadowSuppression = 0.42f,
                    glossRimStrength = 0.42f,
                    glossSmoothness = 0.82f,
                    enableSoftFlush = true,
                    softFlushColor = new Color(1.0f, 0.40f, 0.34f, 1.0f),
                    softFlushStrength = 0.30f,
                    softFlushWidth = 0.60f,
                    softFlushVerticalBias = 0.48f,
                    enableExcitedTone = true,
                    excitedToneColor = new Color(1.0f, 0.48f, 0.34f, 1.0f),
                    excitedToneStrength = 0.28f,
                    excitedToneBreath = 0.0f,
                    excitedToneUpperBias = 0.58f
                }
            },

            new PCSSPreset
            {
                name = "Studio Boost",
                description = "Maximum PCSS and shader boost for no-light AAO workflows and lit PC scenes.",
                settings = new PCSSSettings
                {
                    shadowDistance = 3.2f,
                    shadowSoftness = 0.86f,
                    shadowIntensity = 1.35f,
                    shadowSamples = 32,
                    filterRadius = 1.55f,
                    shadowBias = 0.026f,
                    shadowColor = new Color(0.08f, 0.08f, 0.13f, 1.0f),
                    ambientOcclusion = 0.45f,
                    enableExternalLights = false,
                    externalLightIntensity = 1.6f,
                    externalLightColor = new Color(1.0f, 0.96f, 0.92f, 1.0f),
                    externalLightAngle = 32.0f,
                    enableAnimeStyle = false,
                    enableCinematicStyle = true,
                    filmGrain = 0.01f,
                    vignette = 0.12f,
                    bloom = 0.20f,
                    enableDewyGloss = true,
                    glossCoherence = 0.86f,
                    glossBoost = 0.88f,
                    glossShadowSuppression = 0.30f,
                    glossRimStrength = 0.72f,
                    glossSmoothness = 0.92f,
                    enableStudioBoost = true
                }
            }
        };
        
        #endregion
        
        #region メンバー変数
        
        private Vector2 scrollPosition;
        private PCSSPreset selectedPreset;
        private List<PCSSPreset> customPresets = new List<PCSSPreset>();
        // 未使用のため一旦停止（GUI拡張時に復帰）
        // private bool showAdvancedSettings = false;
        private bool showCustomPresets = false;
        private string newPresetName = "";
        private string newPresetDescription = "";
        
        #endregion
        
        #region メニューアイテム
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/プリセット管理")]
        public static void ShowWindow()
        {
            var window = GetWindow<PCSSPresetManager>("PCSSプリセット管理");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/リアル影プリセット適用")]
        public static void ApplyRealisticPreset()
        {
            ApplyPreset(DefaultPresets[0]);
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/アニメ風プリセット適用")]
        public static void ApplyAnimePreset()
        {
            ApplyPreset(DefaultPresets[1]);
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/映画風プリセット適用")]
        public static void ApplyCinematicPreset()
        {
            ApplyPreset(DefaultPresets[2]);
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/うるみ艶肌プリセット適用")]
        public static void ApplyDewySkinPreset()
        {
            ApplyPreset(DefaultPresets[5]);
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Soft Flush Skin Preset")]
        public static void ApplySoftFlushSkinPreset()
        {
            ApplyPreset(DefaultPresets[6]);
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Studio Boost Preset")]
        public static void ApplyStudioBoostPreset()
        {
            ApplyPreset(DefaultPresets[8]);
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Excited Tone Preset")]
        public static void ApplyExcitedTonePreset()
        {
            ApplyPreset(DefaultPresets[7]);
        }

        #endregion
        
        #region Unity Lifecycle
        
        private void OnEnable()
        {
            LoadCustomPresets();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            
            // タイトル
            EditorGUILayout.LabelField("PCSS効果プリセット管理", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // プリセット選択
            DrawPresetSelection();
            
            EditorGUILayout.Space(10);
            
            // プリセット詳細
            if (selectedPreset != null)
            {
                DrawPresetDetails();
            }
            
            EditorGUILayout.Space(10);
            
            // カスタムプリセット管理
            DrawCustomPresetManagement();
            
            EditorGUILayout.EndScrollView();
        }
        
        #endregion
        
        #region GUI描画
        
        private void DrawPresetSelection()
        {
            EditorGUILayout.LabelField("プリセット選択", EditorStyles.boldLabel);
            
            // デフォルトプリセット
            EditorGUILayout.LabelField("デフォルトプリセット", EditorStyles.miniBoldLabel);
            
            for (int i = 0; i < DefaultPresets.Length; i++)
            {
                var preset = DefaultPresets[i];
                var isSelected = selectedPreset == preset;
                
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button(preset.name, isSelected ? EditorStyles.boldLabel : EditorStyles.label, GUILayout.Height(25)))
                {
                    selectedPreset = preset;
                }
                
                if (GUILayout.Button("適用", GUILayout.Width(50), GUILayout.Height(25)))
                {
                    ApplyPreset(preset);
                }
                
                EditorGUILayout.EndHorizontal();
                
                if (isSelected)
                {
                    EditorGUILayout.HelpBox(preset.description, MessageType.Info);
                }
            }
            
            EditorGUILayout.Space(5);
            
            // カスタムプリセット
            showCustomPresets = EditorGUILayout.Foldout(showCustomPresets, "カスタムプリセット");
            
            if (showCustomPresets)
            {
                for (int i = 0; i < customPresets.Count; i++)
                {
                    var preset = customPresets[i];
                    var isSelected = selectedPreset == preset;
                    
                    EditorGUILayout.BeginHorizontal();
                    
                    if (GUILayout.Button(preset.name, isSelected ? EditorStyles.boldLabel : EditorStyles.label, GUILayout.Height(25)))
                    {
                        selectedPreset = preset;
                    }
                    
                    if (GUILayout.Button("適用", GUILayout.Width(50), GUILayout.Height(25)))
                    {
                        ApplyPreset(preset);
                    }
                    
                    if (GUILayout.Button("削除", GUILayout.Width(50), GUILayout.Height(25)))
                    {
                        customPresets.RemoveAt(i);
                        SaveCustomPresets();
                        break;
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    
                    if (isSelected)
                    {
                        EditorGUILayout.HelpBox(preset.description, MessageType.Info);
                    }
                }
            }
        }
        
        private void DrawPresetDetails()
        {
            EditorGUILayout.LabelField("プリセット詳細", EditorStyles.boldLabel);
            
            if (selectedPreset == null) return;
            
            // プリセット情報
            EditorGUILayout.LabelField($"名前: {selectedPreset.name}");
            EditorGUILayout.LabelField($"説明: {selectedPreset.description}");
            
            EditorGUILayout.Space(5);
            
            // 設定値の表示（読み取り専用）
            var settings = selectedPreset.settings;
            
            EditorGUILayout.LabelField("影の基本設定", EditorStyles.miniBoldLabel);
            EditorGUILayout.LabelField($"影の距離: {settings.shadowDistance:F2}");
            EditorGUILayout.LabelField($"影の柔らかさ: {settings.shadowSoftness:F2}");
            EditorGUILayout.LabelField($"影の強度: {settings.shadowIntensity:F2}");
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("PCSS詳細設定", EditorStyles.miniBoldLabel);
            EditorGUILayout.LabelField($"影のサンプル数: {settings.shadowSamples}");
            EditorGUILayout.LabelField($"フィルター半径: {settings.filterRadius:F2}");
            EditorGUILayout.LabelField($"影のバイアス: {settings.shadowBias:F2}");
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("外部ライト設定", EditorStyles.miniBoldLabel);
            EditorGUILayout.LabelField($"外部ライト有効: {settings.enableExternalLights}");
            EditorGUILayout.LabelField($"外部ライト強度: {settings.externalLightIntensity:F2}");
            EditorGUILayout.LabelField($"外部ライト角度: {settings.externalLightAngle:F1}°");
            
            if (settings.enableAnimeStyle)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("アニメ風設定", EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField($"セルシェーディング閾値: {settings.celShadingThreshold:F2}");
                EditorGUILayout.LabelField($"セルシェーディング滑らかさ: {settings.celShadingSmoothness:F2}");
            }
            
            if (settings.enableCinematicStyle)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("映画風設定", EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField($"フィルムグレイン: {settings.filmGrain:F2}");
                EditorGUILayout.LabelField($"ビネット: {settings.vignette:F2}");
                EditorGUILayout.LabelField($"ブルーム: {settings.bloom:F2}");
            }

            if (settings.enableDewyGloss)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("うるみ艶設定", EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField($"艶のまとまり: {settings.glossCoherence:F2}");
                EditorGUILayout.LabelField($"明部ハイライト: {settings.glossBoost:F2}");
                EditorGUILayout.LabelField($"影側の抑制: {settings.glossShadowSuppression:F2}");
                EditorGUILayout.LabelField($"輪郭ハイライト: {settings.glossRimStrength:F2}");
                EditorGUILayout.LabelField($"艶の細かさ: {settings.glossSmoothness:F2}");
            }

            if (settings.enableSoftFlush)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Soft Flush", EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField($"Strength: {settings.softFlushStrength:F2}");
                EditorGUILayout.LabelField($"Width: {settings.softFlushWidth:F2}");
                EditorGUILayout.LabelField($"Vertical: {settings.softFlushVerticalBias:F2}");
            }

            if (settings.enableExcitedTone)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Excited Tone", EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField($"Strength: {settings.excitedToneStrength:F2}");
                EditorGUILayout.LabelField($"Breath: {settings.excitedToneBreath:F2}");
                EditorGUILayout.LabelField($"Upper Bias: {settings.excitedToneUpperBias:F2}");
            }
        }

        private void DrawCustomPresetManagement()
        {
            EditorGUILayout.LabelField("カスタムプリセット管理", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical("box");
            
            newPresetName = EditorGUILayout.TextField("プリセット名", newPresetName);
            newPresetDescription = EditorGUILayout.TextField("説明", newPresetDescription);
            
            EditorGUILayout.Space(5);
            
            if (GUILayout.Button("現在の設定をカスタムプリセットとして保存"))
            {
                if (!string.IsNullOrEmpty(newPresetName))
                {
                    SaveCurrentSettingsAsPreset();
                }
                else
                {
                    EditorUtility.DisplayDialog("エラー", "プリセット名を入力してください。", "OK");
                }
            }
            
            EditorGUILayout.EndVertical();
        }
        
        #endregion
        
        #region プリセット適用
        
        private static void ApplyPreset(PCSSPreset preset)
        {
            if (preset == null) return;
            
            try
            {
                // 選択されたオブジェクトにPCSS設定を適用
                var selectedObjects = Selection.gameObjects;
                
                if (selectedObjects.Length == 0)
                {
                    EditorUtility.DisplayDialog("情報", "適用するオブジェクトを選択してください。", "OK");
                    return;
                }
                
                foreach (var obj in selectedObjects)
                {
                    ApplyPCSSSettingsToObject(obj, preset.settings);
                }
                
                EditorUtility.DisplayDialog("成功", $"プリセット「{preset.name}」を適用しました。", "OK");
                
                // シーンを更新
                EditorUtility.SetDirty(selectedObjects[0]);
                SceneView.RepaintAll();
            }
            catch (Exception e)
            {
                Debug.LogError($"プリセット適用エラー: {e.Message}");
                EditorUtility.DisplayDialog("エラー", $"プリセット適用中にエラーが発生しました: {e.Message}", "OK");
            }
        }
        
        private static void ApplyPCSSSettingsToObject(GameObject obj, PCSSSettings settings)
        {
            // マテリアルにPCSS設定を適用
            var renderers = obj.GetComponentsInChildren<Renderer>();
            
            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials;
                
                foreach (var material in materials)
                {
                    if (material != null && material.shader != null)
                    {
                        ApplyPCSSSettingsToMaterial(material, settings);
                    }
                }
            }
            
            // 外部ライトの設定
            if (settings.enableExternalLights)
            {
                SetupExternalLights(obj, settings);
            }
        }
        
        private static void ApplyPCSSSettingsToMaterial(Material material, PCSSSettings settings)
        {
            // PCSS関連のプロパティを設定
            if (material.HasProperty("_PCSSShadowDistance"))
                material.SetFloat("_PCSSShadowDistance", settings.shadowDistance);
            
            if (material.HasProperty("_PCSSShadowSoftness"))
                material.SetFloat("_PCSSShadowSoftness", settings.shadowSoftness);
            
            if (material.HasProperty("_PCSSShadowIntensity"))
                material.SetFloat("_PCSSShadowIntensity", settings.shadowIntensity);
            
            if (material.HasProperty("_PCSSShadowSamples"))
                material.SetInt("_PCSSShadowSamples", settings.shadowSamples);
            
            if (material.HasProperty("_PCSSFilterRadius"))
                material.SetFloat("_PCSSFilterRadius", settings.filterRadius);
            
            if (material.HasProperty("_PCSSShadowBias"))
                material.SetFloat("_PCSSShadowBias", settings.shadowBias);
            
            if (material.HasProperty("_PCSSShadowColor"))
                material.SetColor("_PCSSShadowColor", settings.shadowColor);
            
            if (material.HasProperty("_PCSSAmbientOcclusion"))
                material.SetFloat("_PCSSAmbientOcclusion", settings.ambientOcclusion);
            
            // アニメ風設定
            if (settings.enableAnimeStyle)
            {
                if (material.HasProperty("_CelShadingThreshold"))
                    material.SetFloat("_CelShadingThreshold", settings.celShadingThreshold);
                
                if (material.HasProperty("_CelShadingSmoothness"))
                    material.SetFloat("_CelShadingSmoothness", settings.celShadingSmoothness);
            }
            
            // 映画風設定
            if (settings.enableCinematicStyle)
            {
                if (material.HasProperty("_FilmGrain"))
                    material.SetFloat("_FilmGrain", settings.filmGrain);
                
                if (material.HasProperty("_Vignette"))
                    material.SetFloat("_Vignette", settings.vignette);
                
                if (material.HasProperty("_Bloom"))
                    material.SetFloat("_Bloom", settings.bloom);
            }

            if (settings.enableDewyGloss)
            {
                SetFloatIfExists(material, "_UsePCSS", 1.0f);
                SetFloatIfExists(material, "_PCSSEnabled", 1.0f);
                SetFloatIfExists(material, "_PCSSPresetMode", 4.0f);
                SetFloatIfExists(material, "_UseGlossShadowCoherence", 1.0f);
                SetFloatIfExists(material, "_GlossShadowCoherence", settings.glossCoherence);
                SetFloatIfExists(material, "_GlossShadowBoost", settings.glossBoost);
                SetFloatIfExists(material, "_GlossShadowSuppression", settings.glossShadowSuppression);
                SetFloatIfExists(material, "_GlossRimStrength", settings.glossRimStrength);
                SetFloatIfExists(material, "_GlossSmoothness", settings.glossSmoothness);
                SetFloatIfExists(material, "_UseRealisticShadow", 1.0f);
                SetFloatIfExists(material, "_RealisticShadowIntensity", 0.62f);
                SetFloatIfExists(material, "_RealisticShadowSoftness", 0.68f);
                SetFloatIfExists(material, "_UseVRChatPerformanceGate", 1.0f);
                SetFloatIfExists(material, "_PCSSMaxDistance", 10.0f);
                SetFloatIfExists(material, "_PCSSDistanceFade", 3.0f);
                SetFloatIfExists(material, "_Translucency", 0.55f);
                SetFloatIfExists(material, "_UseLightDirectionOverride", 1.0f);
                SetVectorIfExists(material, "_LightDirectionOverride", new Vector4(0.22f, 0.86f, 0.46f, 0.0f));
                SetFloatIfExists(material, "_UseNoLightPCSSBoost", 1.0f);
                SetFloatIfExists(material, "_NoLightPCSSBoostStrength", 0.48f);
                SetFloatIfExists(material, "_NoLightPCSSBoostSoftness", 0.66f);
                SetFloatIfExists(material, "_NoLightPCSSBoostRim", 0.42f);
                SetColorIfExists(material, "_NoLightPCSSHighlightTint", new Color(0.58f, 0.52f, 0.50f, 1.0f));
                SetKeyword(material, "_USEPCSS_ON", true);
                SetKeyword(material, "_USEGLOSSSHADOWCOHERENCE_ON", true);
                SetKeyword(material, "_USEREALISTICSHADOW_ON", true);
                SetKeyword(material, "_USEVRCHATPERFORMANCEGATE_ON", true);
                SetKeyword(material, "_USELIGHTDIRECTIONOVERRIDE_ON", true);
                SetKeyword(material, "_USENOLIGHTPCSSBOOST_ON", true);
            }

            if (settings.enableSoftFlush)
            {
                bool isFace = IsLikelyFaceMaterial(material);
                bool isSkin = IsLikelySkinMaterial(material);
                SetFloatIfExists(material, "_UsePCSS", 1.0f);
                SetFloatIfExists(material, "_PCSSEnabled", 1.0f);
                SetFloatIfExists(material, "_PCSSPresetMode", 5.0f);
                SetFloatIfExists(material, "_PCSSQualityLevel", 2.0f);
                SetFloatIfExists(material, "_LocalPCSSSamples", 12.0f);
                SetFloatIfExists(material, "_LocalPCSSFilterRadius", 0.0105f);
                SetFloatIfExists(material, "_LocalPCSSLightSize", 0.105f);
                SetFloatIfExists(material, "_LocalPCSSBias", 0.0009f);
                SetFloatIfExists(material, "_PCSSIntensity", 0.93f);
                SetFloatIfExists(material, "_PCSSMaxDistance", 10.0f);
                SetFloatIfExists(material, "_PCSSDistanceFade", 2.8f);
                SetFloatIfExists(material, "_UseGlossShadowCoherence", 1.0f);
                SetFloatIfExists(material, "_GlossShadowCoherence", settings.glossCoherence);
                SetFloatIfExists(material, "_GlossShadowBoost", settings.glossBoost);
                SetFloatIfExists(material, "_GlossShadowSuppression", settings.glossShadowSuppression);
                SetFloatIfExists(material, "_GlossRimStrength", settings.glossRimStrength);
                SetFloatIfExists(material, "_GlossSmoothness", settings.glossSmoothness);
                SetFloatIfExists(material, "_UseRealisticShadow", 1.0f);
                SetFloatIfExists(material, "_UseVRChatPerformanceGate", 1.0f);
                SetColorIfExists(material, "_RealisticShadowColor", isSkin
                    ? new Color(0.23f, 0.13f, 0.14f, 0.76f)
                    : new Color(0.14f, 0.13f, 0.15f, 0.76f));
                SetFloatIfExists(material, "_RealisticShadowIntensity", isSkin ? 0.58f : 0.72f);
                SetFloatIfExists(material, "_RealisticShadowSoftness", isSkin ? 0.74f : 0.56f);
                SetFloatIfExists(material, "_Translucency", isSkin ? 0.50f : 0.42f);
                SetFloatIfExists(material, "_UseSoftFlush", isFace ? 1.0f : 0.0f);
                SetColorIfExists(material, "_SoftFlushColor", settings.softFlushColor);
                SetFloatIfExists(material, "_SoftFlushStrength", isFace ? settings.softFlushStrength : 0.0f);
                SetFloatIfExists(material, "_SoftFlushWidth", settings.softFlushWidth);
                SetFloatIfExists(material, "_SoftFlushVerticalBias", settings.softFlushVerticalBias);
                SetFloatIfExists(material, "_UseRimShade", isSkin ? 1.0f : 0.0f);
                SetColorIfExists(material, "_RimShadeColor", new Color(1.0f, 0.45f, 0.40f, 1.0f));
                SetFloatIfExists(material, "_RimShadeIntensity", isFace ? 0.06f : isSkin ? 0.035f : 0.0f);
                SetFloatIfExists(material, "_RimShadeWidth", 0.78f);
                SetFloatIfExists(material, "_UseLightDirectionOverride", 1.0f);
                SetVectorIfExists(material, "_LightDirectionOverride", new Vector4(0.18f, 0.88f, 0.42f, 0.0f));
                SetFloatIfExists(material, "_UseNoLightPCSSBoost", 1.0f);
                SetFloatIfExists(material, "_NoLightPCSSBoostStrength", 0.52f);
                SetFloatIfExists(material, "_NoLightPCSSBoostSoftness", 0.66f);
                SetFloatIfExists(material, "_NoLightPCSSBoostRim", 0.32f);
                SetColorIfExists(material, "_NoLightPCSSHighlightTint", new Color(0.58f, 0.52f, 0.50f, 1.0f));
                SetKeyword(material, "_USEPCSS_ON", true);
                SetKeyword(material, "_USEGLOSSSHADOWCOHERENCE_ON", true);
                SetKeyword(material, "_USEREALISTICSHADOW_ON", true);
                SetKeyword(material, "_USEVRCHATPERFORMANCEGATE_ON", true);
                SetKeyword(material, "_USELIGHTDIRECTIONOVERRIDE_ON", true);
                SetKeyword(material, "_USENOLIGHTPCSSBOOST_ON", true);
                SetKeyword(material, "_USESOFTFLUSH_ON", isFace);
                SetKeyword(material, "_USERIMSHADE_ON", isSkin);
            }

            if (settings.enableExcitedTone)
            {
                bool isFace = IsLikelyFaceMaterial(material);
                bool isSkin = IsLikelySkinMaterial(material);
                SetFloatIfExists(material, "_UsePCSS", 1.0f);
                SetFloatIfExists(material, "_PCSSEnabled", 1.0f);
                SetFloatIfExists(material, "_PCSSPresetMode", 7.0f);
                SetFloatIfExists(material, "_PCSSQualityLevel", 2.0f);
                SetFloatIfExists(material, "_LocalPCSSSamples", 14.0f);
                SetFloatIfExists(material, "_LocalPCSSFilterRadius", 0.0110f);
                SetFloatIfExists(material, "_LocalPCSSLightSize", 0.115f);
                SetFloatIfExists(material, "_LocalPCSSBias", 0.00085f);
                SetFloatIfExists(material, "_PCSSIntensity", 1.02f);
                SetFloatIfExists(material, "_PCSSMaxDistance", 10.0f);
                SetFloatIfExists(material, "_PCSSDistanceFade", 3.0f);
                SetFloatIfExists(material, "_UseGlossShadowCoherence", 1.0f);
                SetFloatIfExists(material, "_GlossShadowCoherence", settings.glossCoherence);
                SetFloatIfExists(material, "_GlossShadowBoost", settings.glossBoost);
                SetFloatIfExists(material, "_GlossShadowSuppression", settings.glossShadowSuppression);
                SetFloatIfExists(material, "_GlossRimStrength", settings.glossRimStrength);
                SetFloatIfExists(material, "_GlossSmoothness", settings.glossSmoothness);
                SetFloatIfExists(material, "_UseRealisticShadow", 1.0f);
                SetColorIfExists(material, "_RealisticShadowColor", isSkin
                    ? new Color(0.24f, 0.12f, 0.12f, 0.74f)
                    : new Color(0.14f, 0.13f, 0.15f, 0.76f));
                SetFloatIfExists(material, "_RealisticShadowIntensity", isSkin ? 0.60f : 0.72f);
                SetFloatIfExists(material, "_RealisticShadowSoftness", isSkin ? 0.72f : 0.56f);
                SetFloatIfExists(material, "_UseVRChatPerformanceGate", 1.0f);
                SetFloatIfExists(material, "_Translucency", isSkin ? 0.52f : 0.42f);
                SetFloatIfExists(material, "_UseSoftFlush", isFace ? 1.0f : 0.0f);
                SetColorIfExists(material, "_SoftFlushColor", settings.softFlushColor);
                SetFloatIfExists(material, "_SoftFlushStrength", isFace ? settings.softFlushStrength : 0.0f);
                SetFloatIfExists(material, "_SoftFlushWidth", settings.softFlushWidth);
                SetFloatIfExists(material, "_SoftFlushVerticalBias", settings.softFlushVerticalBias);
                SetFloatIfExists(material, "_UseExcitedTone", isSkin ? 1.0f : 0.0f);
                SetColorIfExists(material, "_ExcitedToneColor", settings.excitedToneColor);
                SetFloatIfExists(material, "_ExcitedToneStrength", isFace ? settings.excitedToneStrength : isSkin ? settings.excitedToneStrength * 0.60f : 0.0f);
                SetFloatIfExists(material, "_ExcitedToneBreath", settings.excitedToneBreath);
                SetFloatIfExists(material, "_ExcitedToneUpperBias", settings.excitedToneUpperBias);
                SetFloatIfExists(material, "_UseRimShade", isSkin ? 1.0f : 0.0f);
                SetColorIfExists(material, "_RimShadeColor", new Color(1.0f, 0.52f, 0.42f, 1.0f));
                SetFloatIfExists(material, "_RimShadeIntensity", isFace ? 0.10f : isSkin ? 0.045f : 0.0f);
                SetFloatIfExists(material, "_RimShadeWidth", 0.74f);
                SetFloatIfExists(material, "_UseLightDirectionOverride", 1.0f);
                SetVectorIfExists(material, "_LightDirectionOverride", new Vector4(0.20f, 0.86f, 0.44f, 0.0f));
                SetFloatIfExists(material, "_UseNoLightPCSSBoost", 1.0f);
                SetFloatIfExists(material, "_NoLightPCSSBoostStrength", 0.56f);
                SetFloatIfExists(material, "_NoLightPCSSBoostSoftness", 0.66f);
                SetFloatIfExists(material, "_NoLightPCSSBoostRim", 0.40f);
                SetColorIfExists(material, "_NoLightPCSSHighlightTint", new Color(0.64f, 0.50f, 0.46f, 1.0f));
                SetKeyword(material, "_USEPCSS_ON", true);
                SetKeyword(material, "_USEGLOSSSHADOWCOHERENCE_ON", true);
                SetKeyword(material, "_USEREALISTICSHADOW_ON", true);
                SetKeyword(material, "_USEVRCHATPERFORMANCEGATE_ON", true);
                SetKeyword(material, "_USELIGHTDIRECTIONOVERRIDE_ON", true);
                SetKeyword(material, "_USENOLIGHTPCSSBOOST_ON", true);
                SetKeyword(material, "_USESOFTFLUSH_ON", isFace);
                SetKeyword(material, "_USEEXCITEDTONE_ON", isSkin);
                SetKeyword(material, "_USERIMSHADE_ON", isSkin);
            }

            if (settings.enableStudioBoost)
            {
                SetFloatIfExists(material, "_UsePCSS", 1.0f);
                SetFloatIfExists(material, "_PCSSEnabled", 1.0f);
                SetFloatIfExists(material, "_PCSSPresetMode", 6.0f);
                SetFloatIfExists(material, "_PCSSQualityLevel", 2.0f);
                SetFloatIfExists(material, "_LocalPCSSSamples", 16.0f);
                SetFloatIfExists(material, "_LocalPCSSFilterRadius", 0.0125f);
                SetFloatIfExists(material, "_LocalPCSSLightSize", 0.160f);
                SetFloatIfExists(material, "_LocalPCSSBias", 0.00065f);
                SetFloatIfExists(material, "_PCSSIntensity", 1.30f);
                SetFloatIfExists(material, "_UsePCSSOptimization", 1.0f);
                SetFloatIfExists(material, "_PCSSOptimizationLevel", 0.0f);
                SetFloatIfExists(material, "_UseVRChatPerformanceGate", 1.0f);
                SetFloatIfExists(material, "_PCSSMaxDistance", 10.0f);
                SetFloatIfExists(material, "_PCSSDistanceFade", 3.2f);
                SetFloatIfExists(material, "_UseGlossShadowCoherence", 1.0f);
                SetFloatIfExists(material, "_GlossShadowCoherence", 0.86f);
                SetFloatIfExists(material, "_GlossShadowBoost", 0.88f);
                SetFloatIfExists(material, "_GlossShadowSuppression", 0.30f);
                SetFloatIfExists(material, "_GlossRimStrength", 0.72f);
                SetFloatIfExists(material, "_GlossSmoothness", 0.92f);
                SetFloatIfExists(material, "_UseRealisticShadow", 1.0f);
                SetFloatIfExists(material, "_RealisticShadowIntensity", 0.78f);
                SetFloatIfExists(material, "_RealisticShadowSoftness", 0.62f);
                SetFloatIfExists(material, "_Translucency", 0.48f);
                SetFloatIfExists(material, "_UseRimShade", 1.0f);
                SetColorIfExists(material, "_RimShadeColor", new Color(0.95f, 0.92f, 1.0f, 1.0f));
                SetFloatIfExists(material, "_RimShadeIntensity", 0.16f);
                SetFloatIfExists(material, "_RimShadeWidth", 0.64f);
                SetFloatIfExists(material, "_UseLightDirectionOverride", 1.0f);
                SetVectorIfExists(material, "_LightDirectionOverride", new Vector4(0.28f, 0.82f, 0.50f, 0.0f));
                SetFloatIfExists(material, "_UseNoLightPCSSBoost", 1.0f);
                SetFloatIfExists(material, "_NoLightPCSSBoostStrength", 0.72f);
                SetFloatIfExists(material, "_NoLightPCSSBoostSoftness", 0.70f);
                SetFloatIfExists(material, "_NoLightPCSSBoostRim", 0.55f);
                SetColorIfExists(material, "_NoLightPCSSHighlightTint", new Color(0.72f, 0.70f, 0.76f, 1.0f));
                SetKeyword(material, "_USEPCSS_ON", true);
                SetKeyword(material, "_USEPCSSOPTIMIZATION_ON", true);
                SetKeyword(material, "_USEGLOSSSHADOWCOHERENCE_ON", true);
                SetKeyword(material, "_USEREALISTICSHADOW_ON", true);
                SetKeyword(material, "_USEVRCHATPERFORMANCEGATE_ON", true);
                SetKeyword(material, "_USELIGHTDIRECTIONOVERRIDE_ON", true);
                SetKeyword(material, "_USENOLIGHTPCSSBOOST_ON", true);
                SetKeyword(material, "_USERIMSHADE_ON", true);
            }
        }

        private static void SetFloatIfExists(Material material, string name, float value)
        {
            if (material != null && material.HasProperty(name))
            {
                material.SetFloat(name, value);
            }
        }

        private static void SetColorIfExists(Material material, string name, Color value)
        {
            if (material != null && material.HasProperty(name))
            {
                material.SetColor(name, value);
            }
        }

        private static void SetVectorIfExists(Material material, string name, Vector4 value)
        {
            if (material != null && material.HasProperty(name))
            {
                material.SetVector(name, value);
            }
        }

        private static bool IsLikelyFaceMaterial(Material material)
        {
            if (material == null) return false;
            string name = (material.name ?? string.Empty).ToLowerInvariant();
            return name.Contains("face") ||
                   name.Contains("head") ||
                   name.Contains("cheek") ||
                   name.Contains("blush") ||
                   name.Contains("makeup") ||
                   name.Contains("make");
        }

        private static bool IsLikelySkinMaterial(Material material)
        {
            if (material == null) return false;
            string name = (material.name ?? string.Empty).ToLowerInvariant();
            return IsLikelyFaceMaterial(material) ||
                   name.Contains("skin") ||
                   name.Contains("body") ||
                   name.Contains("hand") ||
                   name.Contains("arm") ||
                   name.Contains("leg") ||
                   name.Contains("torso") ||
                   name.Contains("hada");
        }

        private static void SetKeyword(Material material, string keyword, bool enabled)
        {
            if (material == null) return;
            if (enabled)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }
        
        private static void SetupExternalLights(GameObject obj, PCSSSettings settings)
        {
            // 外部ライトのGameObjectを作成または取得
            var lightObj = GameObject.Find("ExternalLight");
            
            if (lightObj == null)
            {
                lightObj = new GameObject("ExternalLight");
            }
            
            // Lightコンポーネントを設定
            var light = lightObj.GetComponent<Light>();
            if (light == null)
            {
                light = lightObj.AddComponent<Light>();
            }
            
            light.type = LightType.Directional;
            light.intensity = settings.externalLightIntensity;
            light.color = settings.externalLightColor;
            light.shadows = LightShadows.Soft;
            
            // ライトの位置と角度を設定
            var angle = settings.externalLightAngle * Mathf.Deg2Rad;
            lightObj.transform.position = obj.transform.position + new Vector3(
                Mathf.Cos(angle) * 5f,
                3f,
                Mathf.Sin(angle) * 5f
            );
            
            lightObj.transform.LookAt(obj.transform);
        }
        
        #endregion
        
        #region カスタムプリセット管理
        
        private void SaveCurrentSettingsAsPreset()
        {
            var newPreset = new PCSSPreset
            {
                name = newPresetName,
                description = newPresetDescription,
                settings = GetCurrentSettings(),
                isCustom = true
            };
            
            customPresets.Add(newPreset);
            SaveCustomPresets();
            
            newPresetName = "";
            newPresetDescription = "";
            
            EditorUtility.DisplayDialog("成功", $"カスタムプリセット「{newPreset.name}」を保存しました。", "OK");
        }
        
        private PCSSSettings GetCurrentSettings()
        {
            // 現在のシーン設定からPCSS設定を取得
            var settings = new PCSSSettings();
            
            // 選択されたオブジェクトから設定を取得
            var selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length > 0)
            {
                var renderers = selectedObjects[0].GetComponentsInChildren<Renderer>();
                if (renderers.Length > 0)
                {
                    var material = renderers[0].sharedMaterial;
                    if (material != null)
                    {
                        settings.shadowDistance = material.GetFloat("_PCSSShadowDistance");
                        settings.shadowSoftness = material.GetFloat("_PCSSShadowSoftness");
                        settings.shadowIntensity = material.GetFloat("_PCSSShadowIntensity");
                        settings.shadowSamples = material.GetInt("_PCSSShadowSamples");
                        settings.filterRadius = material.GetFloat("_PCSSFilterRadius");
                        settings.shadowBias = material.GetFloat("_PCSSShadowBias");
                        settings.shadowColor = material.GetColor("_PCSSShadowColor");
                        settings.ambientOcclusion = material.GetFloat("_PCSSAmbientOcclusion");
                    }
                }
            }
            
            return settings;
        }
        
        private void LoadCustomPresets()
        {
            var path = Path.Combine(Application.dataPath, "Editor/PCSSPresets.json");
            
            if (File.Exists(path))
            {
                try
                {
                    var json = File.ReadAllText(path);
                    var data = JsonUtility.FromJson<CustomPresetData>(json);
                    customPresets = data.presets;
                }
                catch (Exception e)
                {
                    Debug.LogError($"カスタムプリセット読み込みエラー: {e.Message}");
                }
            }
        }
        
        private void SaveCustomPresets()
        {
            var path = Path.Combine(Application.dataPath, "Editor/PCSSPresets.json");
            
            try
            {
                var data = new CustomPresetData { presets = customPresets };
                var json = JsonUtility.ToJson(data, true);
                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"カスタムプリセット保存エラー: {e.Message}");
            }
        }
        
        [System.Serializable]
        private class CustomPresetData
        {
            public List<PCSSPreset> presets = new List<PCSSPreset>();
        }
        
        #endregion
    }
} 
