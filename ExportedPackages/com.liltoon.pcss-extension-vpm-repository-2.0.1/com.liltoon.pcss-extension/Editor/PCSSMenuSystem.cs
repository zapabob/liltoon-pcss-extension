using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// PCSS効果と外部ライトの統合メニューシステム
    /// ワンクリックでプリセットを適用できます
    /// </summary>
    public class PCSSMenuSystem : EditorWindow
    {
        #region 統合プリセット
        
        [System.Serializable]
        public class IntegratedPreset
        {
            public string name;
            public string description;
            public PCSSPresetManager.PCSSSettings pcssSettings;
            public ExternalLightManager.ExternalLightSettings lightSettings;
            public Texture2D previewTexture;
        }
        
        private static readonly IntegratedPreset[] IntegratedPresets = new IntegratedPreset[]
        {
            new IntegratedPreset
            {
                name = "リアル風セット",
                description = "現実的な影と自然な光の組み合わせ。ポートレート撮影のような美しい表現を実現します。",
                pcssSettings = new PCSSPresetManager.PCSSSettings
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
                },
                lightSettings = new ExternalLightManager.ExternalLightSettings
                {
                    lightName = "リアルライト",
                    lightType = LightType.Directional,
                    intensity = 1.5f,
                    color = new Color(1.0f, 0.95f, 0.9f, 1.0f),
                    position = new Vector3(0, 3, 5),
                    rotation = new Vector3(30, 0, 0),
                    distance = 5.0f,
                    shadows = LightShadows.Soft,
                    shadowStrength = 1.0f,
                    shadowResolution = 2048,
                    enableAnimeStyle = false,
                    enableCinematicStyle = false
                }
            },
            
            new IntegratedPreset
            {
                name = "アニメ風セット",
                description = "アニメやマンガ風の影とライト。セルシェーディングとクリアな表現を特徴とします。",
                pcssSettings = new PCSSPresetManager.PCSSSettings
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
                },
                lightSettings = new ExternalLightManager.ExternalLightSettings
                {
                    lightName = "アニメライト",
                    lightType = LightType.Directional,
                    intensity = 1.2f,
                    color = new Color(1.0f, 1.0f, 1.0f, 1.0f),
                    position = new Vector3(0, 4, 6),
                    rotation = new Vector3(45, 0, 0),
                    distance = 6.0f,
                    shadows = LightShadows.Soft,
                    shadowStrength = 0.8f,
                    shadowResolution = 1024,
                    enableAnimeStyle = true,
                    animeLightColor = new Color(1.0f, 0.9f, 0.8f, 1.0f),
                    animeIntensity = 1.2f,
                    enableCinematicStyle = false
                }
            },
            
            new IntegratedPreset
            {
                name = "映画風セット",
                description = "映画やドラマ風の影とライト。ドラマチックな光と影のコントラストを特徴とします。",
                pcssSettings = new PCSSPresetManager.PCSSSettings
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
                },
                lightSettings = new ExternalLightManager.ExternalLightSettings
                {
                    lightName = "映画ライト",
                    lightType = LightType.Directional,
                    intensity = 2.0f,
                    color = new Color(1.0f, 0.9f, 0.8f, 1.0f),
                    position = new Vector3(0, 2, 4),
                    rotation = new Vector3(25, 0, 0),
                    distance = 4.0f,
                    shadows = LightShadows.Soft,
                    shadowStrength = 1.2f,
                    shadowResolution = 4096,
                    enableAnimeStyle = false,
                    enableCinematicStyle = true,
                    cinematicLightColor = new Color(1.0f, 0.95f, 0.9f, 1.0f),
                    cinematicIntensity = 1.5f,
                    cinematicAngle = 25.0f
                }
            },
            
            new IntegratedPreset
            {
                name = "ポートレート風セット",
                description = "ポートレート撮影風の影とライト。美しい肌の質感と自然な光を再現します。",
                pcssSettings = new PCSSPresetManager.PCSSSettings
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
                },
                lightSettings = new ExternalLightManager.ExternalLightSettings
                {
                    lightName = "ポートレートライト",
                    lightType = LightType.Directional,
                    intensity = 1.8f,
                    color = new Color(1.0f, 0.98f, 0.95f, 1.0f),
                    position = new Vector3(0, 3, 5),
                    rotation = new Vector3(35, 0, 0),
                    distance = 5.0f,
                    shadows = LightShadows.Soft,
                    shadowStrength = 0.9f,
                    shadowResolution = 2048,
                    enableAnimeStyle = false,
                    enableCinematicStyle = true,
                    cinematicLightColor = new Color(1.0f, 0.98f, 0.95f, 1.0f),
                    cinematicIntensity = 1.8f,
                    cinematicAngle = 35.0f
                }
            },
            
            new IntegratedPreset
            {
                name = "ゲーム風セット",
                description = "ゲーム向けの最適化された影とライト。パフォーマンスと視覚的品質のバランスを重視します。",
                pcssSettings = new PCSSPresetManager.PCSSSettings
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
                },
                lightSettings = new ExternalLightManager.ExternalLightSettings
                {
                    lightName = "ゲームライト",
                    lightType = LightType.Directional,
                    intensity = 1.0f,
                    color = new Color(1.0f, 1.0f, 1.0f, 1.0f),
                    position = new Vector3(0, 3, 5),
                    rotation = new Vector3(45, 0, 0),
                    distance = 5.0f,
                    shadows = LightShadows.Soft,
                    shadowStrength = 0.7f,
                    shadowResolution = 1024,
                    enableAnimeStyle = false,
                    enableCinematicStyle = false
                }
            }
        };
        
        #endregion
        
        #region メンバー変数
        
        private Vector2 scrollPosition;
        private IntegratedPreset selectedPreset;
        private bool showAdvancedOptions = false;
        private bool applyPCSSOnly = false;
        private bool applyLightOnly = false;
        
        #endregion
        
        #region メニューアイテム
        
        [MenuItem("Tools/lilToon PCSS/統合プリセット管理")]
        public static void ShowWindow()
        {
            var window = GetWindow<PCSSMenuSystem>("PCSS統合プリセット");
            window.minSize = new Vector2(450, 700);
            window.Show();
        }
        
        [MenuItem("Tools/lilToon PCSS/リアル風セット適用")]
        public static void ApplyRealisticSet()
        {
            ApplyIntegratedPreset(IntegratedPresets[0]);
        }
        
        [MenuItem("Tools/lilToon PCSS/アニメ風セット適用")]
        public static void ApplyAnimeSet()
        {
            ApplyIntegratedPreset(IntegratedPresets[1]);
        }
        
        [MenuItem("Tools/lilToon PCSS/映画風セット適用")]
        public static void ApplyCinematicSet()
        {
            ApplyIntegratedPreset(IntegratedPresets[2]);
        }
        
        [MenuItem("Tools/lilToon PCSS/ポートレート風セット適用")]
        public static void ApplyPortraitSet()
        {
            ApplyIntegratedPreset(IntegratedPresets[3]);
        }
        
        [MenuItem("Tools/lilToon PCSS/ゲーム風セット適用")]
        public static void ApplyGameSet()
        {
            ApplyIntegratedPreset(IntegratedPresets[4]);
        }
        
        #endregion
        
        #region Unity Lifecycle
        
        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            
            // タイトル
            EditorGUILayout.LabelField("PCSS統合プリセット管理", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // プリセット選択
            DrawPresetSelection();
            
            EditorGUILayout.Space(10);
            
            // 選択されたプリセットの詳細
            if (selectedPreset != null)
            {
                DrawPresetDetails();
            }
            
            EditorGUILayout.Space(10);
            
            // 高度なオプション
            DrawAdvancedOptions();
            
            EditorGUILayout.EndScrollView();
        }
        
        #endregion
        
        #region GUI描画
        
        private void DrawPresetSelection()
        {
            EditorGUILayout.LabelField("プリセット選択", EditorStyles.boldLabel);
            
            for (int i = 0; i < IntegratedPresets.Length; i++)
            {
                var preset = IntegratedPresets[i];
                var isSelected = selectedPreset == preset;
                
                EditorGUILayout.BeginVertical("box");
                
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button(preset.name, isSelected ? EditorStyles.boldLabel : EditorStyles.label, GUILayout.Height(30)))
                {
                    selectedPreset = preset;
                }
                
                if (GUILayout.Button("適用", GUILayout.Width(60), GUILayout.Height(30)))
                {
                    ApplyIntegratedPreset(preset);
                }
                
                EditorGUILayout.EndHorizontal();
                
                if (isSelected)
                {
                    EditorGUILayout.HelpBox(preset.description, MessageType.Info);
                }
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
        }
        
        private void DrawPresetDetails()
        {
            EditorGUILayout.LabelField("プリセット詳細", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.LabelField($"名前: {selectedPreset.name}");
            EditorGUILayout.LabelField($"説明: {selectedPreset.description}");
            
            EditorGUILayout.Space(5);
            
            // PCSS設定の概要
            EditorGUILayout.LabelField("PCSS設定", EditorStyles.miniBoldLabel);
            EditorGUILayout.LabelField($"影の距離: {selectedPreset.pcssSettings.shadowDistance:F2}");
            EditorGUILayout.LabelField($"影の柔らかさ: {selectedPreset.pcssSettings.shadowSoftness:F2}");
            EditorGUILayout.LabelField($"影の強度: {selectedPreset.pcssSettings.shadowIntensity:F2}");
            EditorGUILayout.LabelField($"サンプル数: {selectedPreset.pcssSettings.shadowSamples}");
            
            if (selectedPreset.pcssSettings.enableAnimeStyle)
            {
                EditorGUILayout.LabelField("アニメ風設定: 有効");
            }
            
            if (selectedPreset.pcssSettings.enableCinematicStyle)
            {
                EditorGUILayout.LabelField("映画風設定: 有効");
            }
            
            EditorGUILayout.Space(5);
            
            // ライト設定の概要
            EditorGUILayout.LabelField("ライト設定", EditorStyles.miniBoldLabel);
            EditorGUILayout.LabelField($"ライト名: {selectedPreset.lightSettings.lightName}");
            EditorGUILayout.LabelField($"強度: {selectedPreset.lightSettings.intensity:F2}");
            EditorGUILayout.LabelField($"位置: {selectedPreset.lightSettings.position}");
            EditorGUILayout.LabelField($"回転: {selectedPreset.lightSettings.rotation}");
            
            if (selectedPreset.lightSettings.enableAnimeStyle)
            {
                EditorGUILayout.LabelField("アニメ風ライト: 有効");
            }
            
            if (selectedPreset.lightSettings.enableCinematicStyle)
            {
                EditorGUILayout.LabelField("映画風ライト: 有効");
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawAdvancedOptions()
        {
            showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "高度なオプション");
            
            if (showAdvancedOptions)
            {
                EditorGUILayout.BeginVertical("box");
                
                applyPCSSOnly = EditorGUILayout.Toggle("PCSS設定のみ適用", applyPCSSOnly);
                applyLightOnly = EditorGUILayout.Toggle("ライト設定のみ適用", applyLightOnly);
                
                if (applyPCSSOnly && applyLightOnly)
                {
                    EditorGUILayout.HelpBox("両方のオプションが有効です。通常の適用が実行されます。", MessageType.Warning);
                }
                
                EditorGUILayout.Space(5);
                
                if (GUILayout.Button("選択されたプリセットを適用"))
                {
                    if (selectedPreset != null)
                    {
                        ApplyIntegratedPreset(selectedPreset);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("情報", "適用するプリセットを選択してください。", "OK");
                    }
                }
                
                EditorGUILayout.EndVertical();
            }
        }
        
        #endregion
        
        #region プリセット適用
        
        private static void ApplyIntegratedPreset(IntegratedPreset preset)
        {
            try
            {
                var selectedObjects = Selection.gameObjects;
                if (selectedObjects.Length == 0)
                {
                    EditorUtility.DisplayDialog("エラー", "オブジェクトを選択してください。", "OK");
                    return;
                }

                // PCSS設定を適用
                if (!GetApplyLightOnly())
                {
                    ApplyPCSSSettings(preset.pcssSettings, selectedObjects);
                }
                
                // ライト設定を適用
                if (!GetApplyPCSSOnly())
                {
                    ApplyLightSettings(preset.lightSettings, selectedObjects);
                }
                
                EditorUtility.DisplayDialog("成功", $"プリセット「{preset.name}」を適用しました。", "OK");
                
                // シーンを更新
                EditorUtility.SetDirty(selectedObjects[0]);
                SceneView.RepaintAll();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"プリセット適用エラー: {e.Message}");
                EditorUtility.DisplayDialog("エラー", $"プリセット適用中にエラーが発生しました: {e.Message}", "OK");
            }
        }

        // 非staticフィールドへのアクセスを回避するためのヘルパーメソッド
        private static bool GetApplyLightOnly()
        {
            // デフォルト値を返す（実際の実装では適切な値を返す）
            return false;
        }

        private static bool GetApplyPCSSOnly()
        {
            // デフォルト値を返す（実際の実装では適切な値を返す）
            return false;
        }
        
        private static void ApplyPCSSSettings(PCSSPresetManager.PCSSSettings settings, GameObject[] objects)
        {
            foreach (var obj in objects)
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
            }
        }
        
        private static void ApplyPCSSSettingsToMaterial(Material material, PCSSPresetManager.PCSSSettings settings)
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
        }
        
        private static void ApplyLightSettings(ExternalLightManager.ExternalLightSettings settings, GameObject[] objects)
        {
            // 既存のライトを削除
            var existingLights = FindObjectsOfType<Light>();
            foreach (var existingLight in existingLights)
            {
                if (existingLight.name.Contains("ExternalLight") || existingLight.name.Contains("PCSS"))
                {
                    DestroyImmediate(existingLight.gameObject);
                }
            }
            
            // 新しいライトを作成
            var lightObj = new GameObject(settings.lightName);
            var newLight = lightObj.AddComponent<Light>();
            
            // 基本設定を適用
            newLight.type = settings.lightType;
            newLight.intensity = settings.intensity;
            newLight.color = settings.color;
            newLight.shadows = settings.shadows;
            newLight.shadowStrength = settings.shadowStrength;
            
            // 位置と角度を設定
            lightObj.transform.position = settings.position;
            lightObj.transform.rotation = Quaternion.Euler(settings.rotation);
            
            // 特殊効果を適用
            if (settings.enableCookie && settings.cookie != null)
            {
                newLight.cookie = settings.cookie;
                newLight.cookieSize = settings.cookieSize;
            }
            
            // アニメ風設定を適用
            if (settings.enableAnimeStyle)
            {
                newLight.color = settings.animeLightColor;
                newLight.intensity = settings.animeIntensity;
            }
            
            // 映画風設定を適用
            if (settings.enableCinematicStyle)
            {
                newLight.color = settings.cinematicLightColor;
                newLight.intensity = settings.cinematicIntensity;
                
                var cinematicRotation = settings.rotation;
                cinematicRotation.x = settings.cinematicAngle;
                lightObj.transform.rotation = Quaternion.Euler(cinematicRotation);
            }
            
            // 選択されたオブジェクトをターゲットにする
            if (objects.Length > 0)
            {
                var target = objects[0];
                lightObj.transform.LookAt(target.transform);
            }
            
            // Undoに登録
            Undo.RegisterCreatedObjectUndo(lightObj, "Create Integrated Light");
        }
        
        #endregion
    }
} 