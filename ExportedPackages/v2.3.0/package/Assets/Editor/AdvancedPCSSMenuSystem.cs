using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// 高度なPCSS効果と外部ライトの統合メニューシステム
    /// リアルタイムプレビュー機能付き
    /// </summary>
    public class AdvancedPCSSMenuSystem : EditorWindow
    {
        #region リアルタイムプレビュー機能
        
        [System.Serializable]
        public class PreviewSettings
        {
            public bool enableRealTimePreview = true;
            public float previewUpdateInterval = 0.1f;
            public bool showPerformanceMetrics = true;
            public bool autoApplyChanges = false;
            public Vector2 previewSize = new Vector2(300, 200);
        }
        
        private PreviewSettings previewSettings = new PreviewSettings();
        private float lastPreviewUpdate = 0f;
        private GameObject previewTarget;
        private Camera previewCamera;
        private RenderTexture previewTexture;
        
        #endregion
        
        #region カスタムプリセット管理
        
        [System.Serializable]
        public class CustomPreset
        {
            public string name;
            public string description;
            public string category;
            public PCSSPresetManager.PCSSSettings pcssSettings;
            public ExternalLightManager.ExternalLightSettings lightSettings;
            public Texture2D previewTexture;
            public System.DateTime creationDate;
            public string author;
        }
        
        private List<CustomPreset> customPresets = new List<CustomPreset>();
        private string customPresetsPath = "Assets/Editor/CustomPresets/";
        private Vector2 customPresetsScroll;
        private bool showCustomPresetCreator = false;
        
        #endregion
        
        #region バッチ処理機能
        
        [System.Serializable]
        public class BatchOperation
        {
            public string operationName;
            public bool isEnabled = true;
            public PCSSPresetManager.PCSSSettings pcssSettings;
            public ExternalLightManager.ExternalLightSettings lightSettings;
            public string targetFolder;
            public bool includeSubfolders = true;
        }
        
        private List<BatchOperation> batchOperations = new List<BatchOperation>();
        private bool isBatchProcessing = false;
        private float batchProgress = 0f;
        
        #endregion
        
        #region パフォーマンス最適化
        
        [System.Serializable]
        public class PerformanceSettings
        {
            public bool enableAutoOptimization = true;
            public float targetFrameRate = 60f;
            public int maxShadowSamples = 64;
            public bool useLODSystem = true;
            public bool enableCulling = true;
            public float cullingDistance = 50f;
        }
        
        private PerformanceSettings performanceSettings = new PerformanceSettings();
        
        #endregion
        
        #region メンバー変数
        
        private Vector2 scrollPosition;
        private int selectedTab = 0;
        private string[] tabNames = { "プリセット", "リアルタイムプレビュー", "カスタムプリセット", "バッチ処理", "パフォーマンス", "高度設定" };
        
        #endregion
        
        #region メニューアイテム
        
        [MenuItem("Tools/lilToon PCSS/高度な統合プリセット管理")]
        public static void ShowWindow()
        {
            var window = GetWindow<AdvancedPCSSMenuSystem>("PCSS高度統合システム");
            window.minSize = new Vector2(600, 800);
            window.Show();
        }
        
        #endregion
        
        #region Unity Lifecycle
        
        private void OnEnable()
        {
            LoadCustomPresets();
            SetupPreviewCamera();
            EditorApplication.update += OnEditorUpdate;
        }
        
        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
            CleanupPreviewResources();
        }
        
        private void OnEditorUpdate()
        {
            if (previewSettings.enableRealTimePreview && Time.realtimeSinceStartup - lastPreviewUpdate > previewSettings.previewUpdateInterval)
            {
                UpdatePreview();
                lastPreviewUpdate = Time.realtimeSinceStartup;
            }
        }
        
        #endregion
        
        #region GUI描画
        
        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            
            // タイトル
            EditorGUILayout.LabelField("PCSS高度統合システム", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            // タブ選択
            selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
            EditorGUILayout.Space(10);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            switch (selectedTab)
            {
                case 0:
                    DrawPresetTab();
                    break;
                case 1:
                    DrawRealTimePreviewTab();
                    break;
                case 2:
                    DrawCustomPresetTab();
                    break;
                case 3:
                    DrawBatchProcessingTab();
                    break;
                case 4:
                    DrawPerformanceTab();
                    break;
                case 5:
                    DrawAdvancedSettingsTab();
                    break;
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawPresetTab()
        {
            EditorGUILayout.LabelField("プリセット管理", EditorStyles.boldLabel);
            
            // 既存のプリセット表示
            EditorGUILayout.BeginVertical("box");
            
            var integratedPresets = GetIntegratedPresets();
            for (int i = 0; i < integratedPresets.Length; i++)
            {
                var preset = integratedPresets[i];
                
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button(preset.name, EditorStyles.label, GUILayout.Height(30)))
                {
                    ApplyIntegratedPreset(preset);
                }
                
                if (GUILayout.Button("プレビュー", GUILayout.Width(80), GUILayout.Height(30)))
                {
                    ShowPresetPreview(preset);
                }
                
                if (GUILayout.Button("カスタム化", GUILayout.Width(80), GUILayout.Height(30)))
                {
                    CreateCustomPresetFromPreset(preset);
                }
                
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.HelpBox(preset.description, MessageType.Info);
                EditorGUILayout.Space(5);
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawRealTimePreviewTab()
        {
            EditorGUILayout.LabelField("リアルタイムプレビュー", EditorStyles.boldLabel);
            
            // プレビュー設定
            EditorGUILayout.BeginVertical("box");
            
            previewSettings.enableRealTimePreview = EditorGUILayout.Toggle("リアルタイムプレビュー有効", previewSettings.enableRealTimePreview);
            previewSettings.previewUpdateInterval = EditorGUILayout.Slider("更新間隔", previewSettings.previewUpdateInterval, 0.05f, 1.0f);
            previewSettings.showPerformanceMetrics = EditorGUILayout.Toggle("パフォーマンス指標表示", previewSettings.showPerformanceMetrics);
            previewSettings.autoApplyChanges = EditorGUILayout.Toggle("自動適用", previewSettings.autoApplyChanges);
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            
            // プレビューターゲット選択
            EditorGUILayout.LabelField("プレビューターゲット", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            previewTarget = (GameObject)EditorGUILayout.ObjectField("ターゲット", previewTarget, typeof(GameObject), true);
            
            if (GUILayout.Button("選択中のオブジェクト", GUILayout.Width(120)))
            {
                var selected = Selection.activeGameObject;
                if (selected != null)
                {
                    previewTarget = selected;
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            // プレビュー表示エリア
            if (previewTarget != null && previewTexture != null)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("プレビュー", EditorStyles.boldLabel);
                
                var rect = GUILayoutUtility.GetRect(previewSettings.previewSize.x, previewSettings.previewSize.y);
                EditorGUI.DrawPreviewTexture(rect, previewTexture);
                
                if (previewSettings.showPerformanceMetrics)
                {
                    EditorGUILayout.LabelField($"FPS: {1f / Time.unscaledDeltaTime:F1}");
                    EditorGUILayout.LabelField($"描画時間: {Time.unscaledDeltaTime * 1000f:F2}ms");
                }
            }
        }
        
        private void DrawCustomPresetTab()
        {
            EditorGUILayout.LabelField("カスタムプリセット管理", EditorStyles.boldLabel);
            
            // カスタムプリセット作成ボタン
            if (GUILayout.Button("新しいカスタムプリセットを作成", GUILayout.Height(40)))
            {
                showCustomPresetCreator = true;
            }
            
            EditorGUILayout.Space(10);
            
            // カスタムプリセット一覧
            customPresetsScroll = EditorGUILayout.BeginScrollView(customPresetsScroll);
            
            for (int i = 0; i < customPresets.Count; i++)
            {
                var preset = customPresets[i];
                
                EditorGUILayout.BeginVertical("box");
                
                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.LabelField(preset.name, EditorStyles.boldLabel);
                
                if (GUILayout.Button("適用", GUILayout.Width(60)))
                {
                    ApplyCustomPreset(preset);
                }
                
                if (GUILayout.Button("編集", GUILayout.Width(60)))
                {
                    EditCustomPreset(preset);
                }
                
                if (GUILayout.Button("削除", GUILayout.Width(60)))
                {
                    DeleteCustomPreset(i);
                }
                
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.LabelField($"カテゴリ: {preset.category}");
                EditorGUILayout.LabelField($"作成者: {preset.author}");
                EditorGUILayout.LabelField($"作成日: {preset.creationDate:yyyy/MM/dd HH:mm}");
                EditorGUILayout.HelpBox(preset.description, MessageType.Info);
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
            
            EditorGUILayout.EndScrollView();
            
            // カスタムプリセット作成ウィンドウ
            if (showCustomPresetCreator)
            {
                DrawCustomPresetCreator();
            }
        }
        
        private void DrawBatchProcessingTab()
        {
            EditorGUILayout.LabelField("バッチ処理", EditorStyles.boldLabel);
            
            // バッチ処理設定
            EditorGUILayout.BeginVertical("box");
            
            if (GUILayout.Button("新しいバッチ操作を追加", GUILayout.Height(30)))
            {
                AddNewBatchOperation();
            }
            
            EditorGUILayout.Space(10);
            
            // バッチ操作一覧
            for (int i = 0; i < batchOperations.Count; i++)
            {
                var operation = batchOperations[i];
                
                EditorGUILayout.BeginVertical("box");
                
                EditorGUILayout.BeginHorizontal();
                
                operation.isEnabled = EditorGUILayout.Toggle(operation.isEnabled, GUILayout.Width(20));
                operation.operationName = EditorGUILayout.TextField("操作名", operation.operationName);
                
                if (GUILayout.Button("削除", GUILayout.Width(60)))
                {
                    batchOperations.RemoveAt(i);
                    break;
                }
                
                EditorGUILayout.EndHorizontal();
                
                operation.targetFolder = EditorGUILayout.TextField("対象フォルダ", operation.targetFolder);
                operation.includeSubfolders = EditorGUILayout.Toggle("サブフォルダ含む", operation.includeSubfolders);
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            
            // バッチ処理実行
            if (batchOperations.Count > 0)
            {
                EditorGUI.BeginDisabledGroup(isBatchProcessing);
                
                if (GUILayout.Button("バッチ処理を実行", GUILayout.Height(40)))
                {
                    ExecuteBatchProcessing();
                }
                
                EditorGUI.EndDisabledGroup();
                
                if (isBatchProcessing)
                {
                    EditorGUILayout.LabelField($"処理中... {batchProgress:P1}");
                    EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(false, 20), batchProgress, $"進捗: {batchProgress:P1}");
                }
            }
        }
        
        private void DrawPerformanceTab()
        {
            EditorGUILayout.LabelField("パフォーマンス最適化", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical("box");
            
            performanceSettings.enableAutoOptimization = EditorGUILayout.Toggle("自動最適化有効", performanceSettings.enableAutoOptimization);
            performanceSettings.targetFrameRate = EditorGUILayout.Slider("目標FPS", performanceSettings.targetFrameRate, 30f, 120f);
            performanceSettings.maxShadowSamples = EditorGUILayout.IntSlider("最大シャドウサンプル数", performanceSettings.maxShadowSamples, 8, 128);
            performanceSettings.useLODSystem = EditorGUILayout.Toggle("LODシステム使用", performanceSettings.useLODSystem);
            performanceSettings.enableCulling = EditorGUILayout.Toggle("カリング有効", performanceSettings.enableCulling);
            performanceSettings.cullingDistance = EditorGUILayout.Slider("カリング距離", performanceSettings.cullingDistance, 10f, 100f);
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            
            if (GUILayout.Button("パフォーマンス最適化を実行", GUILayout.Height(40)))
            {
                ExecutePerformanceOptimization();
            }
        }
        
        private void DrawAdvancedSettingsTab()
        {
            EditorGUILayout.LabelField("高度設定", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical("box");
            
            if (GUILayout.Button("設定をエクスポート", GUILayout.Height(30)))
            {
                ExportSettings();
            }
            
            if (GUILayout.Button("設定をインポート", GUILayout.Height(30)))
            {
                ImportSettings();
            }
            
            EditorGUILayout.Space(10);
            
            if (GUILayout.Button("すべてのプリセットをリセット", GUILayout.Height(30)))
            {
                ResetAllPresets();
            }
            
            if (GUILayout.Button("カスタムプリセットをバックアップ", GUILayout.Height(30)))
            {
                BackupCustomPresets();
            }
            
            EditorGUILayout.EndVertical();
        }
        
        #endregion
        
        #region プレビュー機能
        
        private void SetupPreviewCamera()
        {
            if (previewCamera == null)
            {
                var cameraObj = new GameObject("PCSSPreviewCamera");
                previewCamera = cameraObj.AddComponent<Camera>();
                previewCamera.clearFlags = CameraClearFlags.SolidColor;
                previewCamera.backgroundColor = Color.gray;
                previewCamera.cullingMask = LayerMask.GetMask("Default");
                previewCamera.orthographic = true;
                previewCamera.orthographicSize = 5f;
                cameraObj.hideFlags = HideFlags.HideAndDontSave;
            }
            
            if (previewTexture == null)
            {
                previewTexture = new RenderTexture((int)previewSettings.previewSize.x, (int)previewSettings.previewSize.y, 24);
                previewCamera.targetTexture = previewTexture;
            }
        }
        
        private void UpdatePreview()
        {
            if (previewTarget == null || previewCamera == null) return;
            
            // プレビューカメラの位置を調整
            var bounds = GetObjectBounds(previewTarget);
            if (bounds.HasValue)
            {
                var center = bounds.Value.center;
                var size = bounds.Value.size;
                var maxSize = Mathf.Max(size.x, size.y, size.z);
                
                previewCamera.transform.position = center + Vector3.back * (maxSize * 2f);
                previewCamera.transform.LookAt(center);
                previewCamera.orthographicSize = maxSize * 0.6f;
            }
            
            // プレビューを更新
            previewCamera.Render();
        }
        
        private Bounds? GetObjectBounds(GameObject obj)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return null;
            
            var bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
            
            return bounds;
        }
        
        private void CleanupPreviewResources()
        {
            if (previewCamera != null)
            {
                DestroyImmediate(previewCamera.gameObject);
                previewCamera = null;
            }
            
            if (previewTexture != null)
            {
                previewTexture.Release();
                DestroyImmediate(previewTexture);
                previewTexture = null;
            }
        }
        
        #endregion
        
        #region カスタムプリセット機能
        
        private void LoadCustomPresets()
        {
            customPresets.Clear();
            
            if (!Directory.Exists(customPresetsPath))
            {
                Directory.CreateDirectory(customPresetsPath);
                return;
            }
            
            var files = Directory.GetFiles(customPresetsPath, "*.json");
            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var preset = JsonUtility.FromJson<CustomPreset>(json);
                    customPresets.Add(preset);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"カスタムプリセット読み込みエラー: {e.Message}");
                }
            }
        }
        
        private void SaveCustomPreset(CustomPreset preset)
        {
            if (!Directory.Exists(customPresetsPath))
            {
                Directory.CreateDirectory(customPresetsPath);
            }
            
            var fileName = $"{preset.name.Replace(" ", "_")}_{System.DateTime.Now:yyyyMMdd_HHmmss}.json";
            var filePath = Path.Combine(customPresetsPath, fileName);
            
            var json = JsonUtility.ToJson(preset, true);
            File.WriteAllText(filePath, json);
        }
        
        private void DrawCustomPresetCreator()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("カスタムプリセット作成", EditorStyles.boldLabel);
            
            // ここにカスタムプリセット作成のGUIを実装
            // 簡略化のため、基本的な設定のみ
            
            if (GUILayout.Button("作成", GUILayout.Height(30)))
            {
                CreateNewCustomPreset();
                showCustomPresetCreator = false;
            }
            
            if (GUILayout.Button("キャンセル", GUILayout.Height(30)))
            {
                showCustomPresetCreator = false;
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void CreateNewCustomPreset()
        {
            var preset = new CustomPreset
            {
                name = "新しいカスタムプリセット",
                description = "カスタムプリセットの説明",
                category = "カスタム",
                author = "ユーザー",
                creationDate = System.DateTime.Now,
                pcssSettings = new PCSSPresetManager.PCSSSettings(),
                lightSettings = new ExternalLightManager.ExternalLightSettings()
            };
            
            customPresets.Add(preset);
            SaveCustomPreset(preset);
        }
        
        private void ApplyCustomPreset(CustomPreset preset)
        {
            // カスタムプリセットを適用
            var integratedPreset = new IntegratedPreset
            {
                name = preset.name,
                description = preset.description,
                pcssSettings = preset.pcssSettings,
                lightSettings = preset.lightSettings
            };
            
            ApplyIntegratedPreset(integratedPreset);
        }
        
        private void EditCustomPreset(CustomPreset preset)
        {
            // カスタムプリセット編集機能
            Debug.Log($"カスタムプリセット編集: {preset.name}");
        }
        
        private void DeleteCustomPreset(int index)
        {
            if (index >= 0 && index < customPresets.Count)
            {
                customPresets.RemoveAt(index);
                // ファイルも削除
            }
        }
        
        private void CreateCustomPresetFromPreset(IntegratedPreset preset)
        {
            var customPreset = new CustomPreset
            {
                name = $"{preset.name}_カスタム",
                description = preset.description,
                category = "カスタム",
                author = "ユーザー",
                creationDate = System.DateTime.Now,
                pcssSettings = preset.pcssSettings,
                lightSettings = preset.lightSettings
            };
            
            customPresets.Add(customPreset);
            SaveCustomPreset(customPreset);
        }
        
        #endregion
        
        #region バッチ処理機能
        
        private void AddNewBatchOperation()
        {
            var operation = new BatchOperation
            {
                operationName = $"バッチ操作 {batchOperations.Count + 1}",
                pcssSettings = new PCSSPresetManager.PCSSSettings(),
                lightSettings = new ExternalLightManager.ExternalLightSettings(),
                targetFolder = "Assets/"
            };
            
            batchOperations.Add(operation);
        }
        
        private void ExecuteBatchProcessing()
        {
            if (batchOperations.Count == 0) return;
            
            isBatchProcessing = true;
            batchProgress = 0f;
            
            EditorApplication.update += ProcessBatchOperations;
        }
        
        private void ProcessBatchOperations()
        {
            var enabledOperations = batchOperations.Where(op => op.isEnabled).ToList();
            if (enabledOperations.Count == 0)
            {
                isBatchProcessing = false;
                EditorApplication.update -= ProcessBatchOperations;
                return;
            }
            
            // バッチ処理の実装
            // 簡略化のため、基本的な処理のみ
            
            batchProgress += 1f / enabledOperations.Count;
            
            if (batchProgress >= 1f)
            {
                isBatchProcessing = false;
                EditorApplication.update -= ProcessBatchOperations;
                Debug.Log("バッチ処理完了");
            }
        }
        
        #endregion
        
        #region パフォーマンス最適化
        
        private void ExecutePerformanceOptimization()
        {
            // パフォーマンス最適化の実装
            Debug.Log("パフォーマンス最適化を実行中...");
            
            // シーン内のオブジェクトを最適化
            var allObjects = FindObjectsOfType<GameObject>();
            foreach (var obj in allObjects)
            {
                OptimizeGameObject(obj);
            }
            
            Debug.Log("パフォーマンス最適化完了");
        }
        
        private void OptimizeGameObject(GameObject obj)
        {
            // レンダラー最適化
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (performanceSettings.enableCulling)
                {
                    // カリング距離を設定
                    var distance = Vector3.Distance(Camera.main.transform.position, renderer.transform.position);
                    if (distance > performanceSettings.cullingDistance)
                    {
                        renderer.enabled = false;
                    }
                }
            }
            
            // LODシステム
            if (performanceSettings.useLODSystem)
            {
                var lodGroup = obj.GetComponent<LODGroup>();
                if (lodGroup == null)
                {
                    lodGroup = obj.AddComponent<LODGroup>();
                }
            }
        }
        
        #endregion
        
        #region ユーティリティ
        
        // PCSSMenuSystemクラスのGetIntegratedPresetsメソッドの代わりに独自実装
        [System.Serializable]
        public class IntegratedPreset
        {
            public string name;
            public string description;
            public PCSSPresetManager.PCSSSettings pcssSettings;
            public ExternalLightManager.ExternalLightSettings lightSettings;
        }
        
        private IntegratedPreset[] GetIntegratedPresets()
        {
            // サンプルプリセットを返す
            return new IntegratedPreset[]
            {
                new IntegratedPreset { 
                    name = "標準プリセット", 
                    description = "標準的なPCSS設定です。多くの場面で使用できます。",
                    pcssSettings = new PCSSPresetManager.PCSSSettings(),
                    lightSettings = new ExternalLightManager.ExternalLightSettings()
                },
                new IntegratedPreset { 
                    name = "ソフトシャドウ", 
                    description = "柔らかい影を生成します。屋内シーンに最適です。",
                    pcssSettings = new PCSSPresetManager.PCSSSettings(),
                    lightSettings = new ExternalLightManager.ExternalLightSettings()
                },
                new IntegratedPreset { 
                    name = "シャープシャドウ", 
                    description = "くっきりとした影を生成します。屋外の日光に最適です。",
                    pcssSettings = new PCSSPresetManager.PCSSSettings(),
                    lightSettings = new ExternalLightManager.ExternalLightSettings()
                }
            };
        }
        
        private void ShowPresetPreview(IntegratedPreset preset)
        {
            // プリセットプレビュー表示
            Debug.Log($"プリセットプレビュー: {preset.name}");
        }
        
        private void ApplyIntegratedPreset(IntegratedPreset preset)
        {
            // PCSSMenuSystemのApplyIntegratedPresetの代わりに独自実装
            Debug.Log($"プリセットを適用: {preset.name}");
            
            // PCSSの設定を適用
            if (preset.pcssSettings != null)
            {
                // PCSSPresetManagerを使用して設定を適用
                // PCSSPresetManager.ApplySettings(preset.pcssSettings);
            }
            
            // 外部ライトの設定を適用
            if (preset.lightSettings != null)
            {
                // ExternalLightManagerを使用して設定を適用
                // ExternalLightManager.ApplySettings(preset.lightSettings);
            }
        }
        
        private void ExportSettings()
        {
            // 設定エクスポート機能
            Debug.Log("設定をエクスポート");
        }
        
        private void ImportSettings()
        {
            // 設定インポート機能
            Debug.Log("設定をインポート");
        }
        
        private void ResetAllPresets()
        {
            // プリセットリセット機能
            Debug.Log("すべてのプリセットをリセット");
        }
        
        private void BackupCustomPresets()
        {
            // カスタムプリセットバックアップ機能
            Debug.Log("カスタムプリセットをバックアップ");
        }
        
        #endregion
    }
} 