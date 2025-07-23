using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// 外部ライト管理システム
    /// アバター用の外部ライトを簡単に設定・管理できます
    /// </summary>
    public class ExternalLightManager : EditorWindow
    {
        #region ライト設定
        
        [System.Serializable]
        public class ExternalLightSettings
        {
            [Header("基本設定")]
            public string lightName = "ExternalLight";
            public LightType lightType = LightType.Directional;
            public float intensity = 1.0f;
            public Color color = Color.white;
            
            [Header("位置と角度")]
            public Vector3 position = new Vector3(0, 3, 5);
            public Vector3 rotation = new Vector3(30, 0, 0);
            public float distance = 5.0f;
            
            [Header("影の設定")]
            public LightShadows shadows = LightShadows.Soft;
            public float shadowStrength = 1.0f;
            public int shadowResolution = 2048;
            
            [Header("特殊効果")]
            public bool enableCookie = false;
            public Texture cookie = null;
            public float cookieSize = 1.0f;
            
            [Header("アニメ風設定")]
            public bool enableAnimeStyle = false;
            public Color animeLightColor = new Color(1.0f, 0.9f, 0.8f, 1.0f);
            public float animeIntensity = 1.2f;
            
            [Header("映画風設定")]
            public bool enableCinematicStyle = false;
            public Color cinematicLightColor = new Color(1.0f, 0.95f, 0.9f, 1.0f);
            public float cinematicIntensity = 1.5f;
            public float cinematicAngle = 25.0f;
        }
        
        #endregion
        
        #region プリセット
        
        private static readonly ExternalLightSettings[] LightPresets = new ExternalLightSettings[]
        {
            new ExternalLightSettings
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
            },
            
            new ExternalLightSettings
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
            },
            
            new ExternalLightSettings
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
            },
            
            new ExternalLightSettings
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
            },
            
            new ExternalLightSettings
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
        };
        
        #endregion
        
        #region メンバー変数
        
        private Vector2 scrollPosition;
        private ExternalLightSettings currentSettings = new ExternalLightSettings();
        private ExternalLightSettings selectedPreset;
        private bool showAdvancedSettings = false;
        private bool showLightList = false;
        private List<Light> sceneLights = new List<Light>();
        
        #endregion
        
        #region メニューアイテム
        
        [MenuItem("Tools/lilToon PCSS/外部ライト管理")]
        public static void ShowWindow()
        {
            var window = GetWindow<ExternalLightManager>("外部ライト管理");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }
        
        [MenuItem("Tools/lilToon PCSS/リアルライト作成")]
        public static void CreateRealisticLight()
        {
            CreateLightFromPreset(LightPresets[0]);
        }
        
        [MenuItem("Tools/lilToon PCSS/アニメライト作成")]
        public static void CreateAnimeLight()
        {
            CreateLightFromPreset(LightPresets[1]);
        }
        
        [MenuItem("Tools/lilToon PCSS/映画ライト作成")]
        public static void CreateCinematicLight()
        {
            CreateLightFromPreset(LightPresets[2]);
        }
        
        [MenuItem("Tools/lilToon PCSS/ポートレートライト作成")]
        public static void CreatePortraitLight()
        {
            CreateLightFromPreset(LightPresets[3]);
        }
        
        [MenuItem("Tools/lilToon PCSS/ゲームライト作成")]
        public static void CreateGameLight()
        {
            CreateLightFromPreset(LightPresets[4]);
        }
        
        #endregion
        
        #region Unity Lifecycle
        
        private void OnEnable()
        {
            RefreshSceneLights();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            
            // タイトル
            EditorGUILayout.LabelField("外部ライト管理システム", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // プリセット選択
            DrawPresetSelection();
            
            EditorGUILayout.Space(10);
            
            // 現在のライト設定
            DrawCurrentSettings();
            
            EditorGUILayout.Space(10);
            
            // シーン内ライト一覧
            DrawSceneLights();
            
            EditorGUILayout.EndScrollView();
        }
        
        #endregion
        
        #region GUI描画
        
        private void DrawPresetSelection()
        {
            EditorGUILayout.LabelField("ライトプリセット", EditorStyles.boldLabel);
            
            for (int i = 0; i < LightPresets.Length; i++)
            {
                var preset = LightPresets[i];
                
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button(preset.lightName, GUILayout.Height(25)))
                {
                    selectedPreset = preset;
                    currentSettings = CloneSettings(preset);
                }
                
                if (GUILayout.Button("作成", GUILayout.Width(50), GUILayout.Height(25)))
                {
                    CreateLightFromPreset(preset);
                }
                
                EditorGUILayout.EndHorizontal();
                
                if (selectedPreset == preset)
                {
                    EditorGUILayout.HelpBox($"プリセット: {preset.lightName}", MessageType.Info);
                }
            }
        }
        
        private void DrawCurrentSettings()
        {
            EditorGUILayout.LabelField("現在の設定", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical("box");
            
            // 基本設定
            EditorGUILayout.LabelField("基本設定", EditorStyles.miniBoldLabel);
            currentSettings.lightName = EditorGUILayout.TextField("ライト名", currentSettings.lightName);
            currentSettings.lightType = (LightType)EditorGUILayout.EnumPopup("ライトタイプ", currentSettings.lightType);
            currentSettings.intensity = EditorGUILayout.Slider("強度", currentSettings.intensity, 0.0f, 5.0f);
            currentSettings.color = EditorGUILayout.ColorField("色", currentSettings.color);
            
            EditorGUILayout.Space(5);
            
            // 位置と角度
            EditorGUILayout.LabelField("位置と角度", EditorStyles.miniBoldLabel);
            currentSettings.position = EditorGUILayout.Vector3Field("位置", currentSettings.position);
            currentSettings.rotation = EditorGUILayout.Vector3Field("回転", currentSettings.rotation);
            currentSettings.distance = EditorGUILayout.Slider("距離", currentSettings.distance, 1.0f, 20.0f);
            
            EditorGUILayout.Space(5);
            
            // 影の設定
            EditorGUILayout.LabelField("影の設定", EditorStyles.miniBoldLabel);
            currentSettings.shadows = (LightShadows)EditorGUILayout.EnumPopup("影の種類", currentSettings.shadows);
            currentSettings.shadowStrength = EditorGUILayout.Slider("影の強度", currentSettings.shadowStrength, 0.0f, 2.0f);
            currentSettings.shadowResolution = EditorGUILayout.IntSlider("影の解像度", currentSettings.shadowResolution, 512, 8192);
            
            EditorGUILayout.Space(5);
            
            // 特殊効果
            EditorGUILayout.LabelField("特殊効果", EditorStyles.miniBoldLabel);
            currentSettings.enableCookie = EditorGUILayout.Toggle("クッキー有効", currentSettings.enableCookie);
            
            if (currentSettings.enableCookie)
            {
                currentSettings.cookie = (Texture)EditorGUILayout.ObjectField("クッキーテクスチャ", currentSettings.cookie, typeof(Texture), false);
                currentSettings.cookieSize = EditorGUILayout.Slider("クッキーサイズ", currentSettings.cookieSize, 0.1f, 5.0f);
            }
            
            EditorGUILayout.Space(5);
            
            // アニメ風設定
            currentSettings.enableAnimeStyle = EditorGUILayout.Toggle("アニメ風設定", currentSettings.enableAnimeStyle);
            
            if (currentSettings.enableAnimeStyle)
            {
                EditorGUI.indentLevel++;
                currentSettings.animeLightColor = EditorGUILayout.ColorField("アニメライト色", currentSettings.animeLightColor);
                currentSettings.animeIntensity = EditorGUILayout.Slider("アニメ強度", currentSettings.animeIntensity, 0.5f, 3.0f);
                EditorGUI.indentLevel--;
            }
            
            // 映画風設定
            currentSettings.enableCinematicStyle = EditorGUILayout.Toggle("映画風設定", currentSettings.enableCinematicStyle);
            
            if (currentSettings.enableCinematicStyle)
            {
                EditorGUI.indentLevel++;
                currentSettings.cinematicLightColor = EditorGUILayout.ColorField("映画ライト色", currentSettings.cinematicLightColor);
                currentSettings.cinematicIntensity = EditorGUILayout.Slider("映画強度", currentSettings.cinematicIntensity, 0.5f, 3.0f);
                currentSettings.cinematicAngle = EditorGUILayout.Slider("映画角度", currentSettings.cinematicAngle, 0.0f, 90.0f);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space(10);
            
            // アクションボタン
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("ライトを作成"))
            {
                CreateLightFromSettings(currentSettings);
            }
            
            if (GUILayout.Button("選択中のライトを更新"))
            {
                UpdateSelectedLight(currentSettings);
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawSceneLights()
        {
            showLightList = EditorGUILayout.Foldout(showLightList, "シーン内ライト一覧");
            
            if (showLightList)
            {
                RefreshSceneLights();
                
                if (sceneLights.Count == 0)
                {
                    EditorGUILayout.HelpBox("シーン内にライトが見つかりません。", MessageType.Info);
                }
                else
                {
                    foreach (var light in sceneLights)
                    {
                        if (light != null)
                        {
                            EditorGUILayout.BeginHorizontal();
                            
                            EditorGUILayout.LabelField(light.name);
                            EditorGUILayout.LabelField($"強度: {light.intensity:F2}");
                            
                            if (GUILayout.Button("選択", GUILayout.Width(50)))
                            {
                                Selection.activeGameObject = light.gameObject;
                            }
                            
                            if (GUILayout.Button("削除", GUILayout.Width(50)))
                            {
                                if (EditorUtility.DisplayDialog("確認", $"ライト「{light.name}」を削除しますか？", "削除", "キャンセル"))
                                {
                                    DestroyImmediate(light.gameObject);
                                    RefreshSceneLights();
                                }
                            }
                            
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
            }
        }
        
        #endregion
        
        #region ライト作成・更新
        
        private static void CreateLightFromPreset(ExternalLightSettings preset)
        {
            var lightObj = CreateLightFromSettings(preset);
            
            if (lightObj != null)
            {
                Selection.activeGameObject = lightObj;
                EditorUtility.DisplayDialog("成功", $"ライト「{preset.lightName}」を作成しました。", "OK");
            }
        }
        
        private static GameObject CreateLightFromSettings(ExternalLightSettings settings)
        {
            try
            {
                // ライトのGameObjectを作成
                var lightObj = new GameObject(settings.lightName);
                
                // Lightコンポーネントを追加
                var light = lightObj.AddComponent<Light>();
                
                // 基本設定を適用
                light.type = settings.lightType;
                light.intensity = settings.intensity;
                light.color = settings.color;
                light.shadows = settings.shadows;
                light.shadowStrength = settings.shadowStrength;
                
                // 位置と角度を設定
                lightObj.transform.position = settings.position;
                lightObj.transform.rotation = Quaternion.Euler(settings.rotation);
                
                // 特殊効果を適用
                if (settings.enableCookie && settings.cookie != null)
                {
                    light.cookie = settings.cookie;
                    light.cookieSize = settings.cookieSize;
                }
                
                // アニメ風設定を適用
                if (settings.enableAnimeStyle)
                {
                    light.color = settings.animeLightColor;
                    light.intensity = settings.animeIntensity;
                }
                
                // 映画風設定を適用
                if (settings.enableCinematicStyle)
                {
                    light.color = settings.cinematicLightColor;
                    light.intensity = settings.cinematicIntensity;
                    
                    // 映画風の角度を適用
                    var cinematicRotation = settings.rotation;
                    cinematicRotation.x = settings.cinematicAngle;
                    lightObj.transform.rotation = Quaternion.Euler(cinematicRotation);
                }
                
                // 選択されたオブジェクトをターゲットにする
                var selectedObjects = Selection.gameObjects;
                if (selectedObjects.Length > 0)
                {
                    var target = selectedObjects[0];
                    lightObj.transform.LookAt(target.transform);
                }
                
                // Undoに登録
                Undo.RegisterCreatedObjectUndo(lightObj, "Create External Light");
                
                return lightObj;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ライト作成エラー: {e.Message}");
                EditorUtility.DisplayDialog("エラー", $"ライト作成中にエラーが発生しました: {e.Message}", "OK");
                return null;
            }
        }
        
        private void UpdateSelectedLight(ExternalLightSettings settings)
        {
            var selectedObjects = Selection.gameObjects;
            
            if (selectedObjects.Length == 0)
            {
                EditorUtility.DisplayDialog("情報", "更新するライトを選択してください。", "OK");
                return;
            }
            
            foreach (var obj in selectedObjects)
            {
                var light = obj.GetComponent<Light>();
                if (light != null)
                {
                    // 基本設定を更新
                    light.type = settings.lightType;
                    light.intensity = settings.intensity;
                    light.color = settings.color;
                    light.shadows = settings.shadows;
                    light.shadowStrength = settings.shadowStrength;
                    
                    // 位置と角度を更新
                    obj.transform.position = settings.position;
                    obj.transform.rotation = Quaternion.Euler(settings.rotation);
                    
                    // 特殊効果を更新
                    if (settings.enableCookie && settings.cookie != null)
                    {
                        light.cookie = settings.cookie;
                        light.cookieSize = settings.cookieSize;
                    }
                    
                    // アニメ風設定を更新
                    if (settings.enableAnimeStyle)
                    {
                        light.color = settings.animeLightColor;
                        light.intensity = settings.animeIntensity;
                    }
                    
                    // 映画風設定を更新
                    if (settings.enableCinematicStyle)
                    {
                        light.color = settings.cinematicLightColor;
                        light.intensity = settings.cinematicIntensity;
                        
                        var cinematicRotation = settings.rotation;
                        cinematicRotation.x = settings.cinematicAngle;
                        obj.transform.rotation = Quaternion.Euler(cinematicRotation);
                    }
                    
                    EditorUtility.SetDirty(obj);
                }
            }
            
            EditorUtility.DisplayDialog("成功", "選択されたライトを更新しました。", "OK");
        }
        
        #endregion
        
        #region ユーティリティ
        
        private void RefreshSceneLights()
        {
            sceneLights.Clear();
            var lights = FindObjectsOfType<Light>();
            sceneLights.AddRange(lights);
        }
        
        private ExternalLightSettings CloneSettings(ExternalLightSettings original)
        {
            return new ExternalLightSettings
            {
                lightName = original.lightName,
                lightType = original.lightType,
                intensity = original.intensity,
                color = original.color,
                position = original.position,
                rotation = original.rotation,
                distance = original.distance,
                shadows = original.shadows,
                shadowStrength = original.shadowStrength,
                shadowResolution = original.shadowResolution,
                enableCookie = original.enableCookie,
                cookie = original.cookie,
                cookieSize = original.cookieSize,
                enableAnimeStyle = original.enableAnimeStyle,
                animeLightColor = original.animeLightColor,
                animeIntensity = original.animeIntensity,
                enableCinematicStyle = original.enableCinematicStyle,
                cinematicLightColor = original.cinematicLightColor,
                cinematicIntensity = original.cinematicIntensity,
                cinematicAngle = original.cinematicAngle
            };
        }
        
        #endregion
    }
} 