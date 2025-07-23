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
            }
        };
        
        #endregion
        
        #region メンバー変数
        
        private Vector2 scrollPosition;
        private PCSSPreset selectedPreset;
        private List<PCSSPreset> customPresets = new List<PCSSPreset>();
        private bool showAdvancedSettings = false;
        private bool showCustomPresets = false;
        private string newPresetName = "";
        private string newPresetDescription = "";
        
        #endregion
        
        #region メニューアイテム
        
        [MenuItem("Tools/lilToon PCSS/プリセット管理")]
        public static void ShowWindow()
        {
            var window = GetWindow<PCSSPresetManager>("PCSSプリセット管理");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }
        
        [MenuItem("Tools/lilToon PCSS/リアル影プリセット適用")]
        public static void ApplyRealisticPreset()
        {
            ApplyPreset(DefaultPresets[0]);
        }
        
        [MenuItem("Tools/lilToon PCSS/アニメ風プリセット適用")]
        public static void ApplyAnimePreset()
        {
            ApplyPreset(DefaultPresets[1]);
        }
        
        [MenuItem("Tools/lilToon PCSS/映画風プリセット適用")]
        public static void ApplyCinematicPreset()
        {
            ApplyPreset(DefaultPresets[2]);
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