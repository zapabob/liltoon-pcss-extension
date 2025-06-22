using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// VRChat最適化設定エディター
    /// </summary>
    public class VRChatOptimizationSettings : EditorWindow
    {
        private static VRChatOptimizationProfile currentProfile;
        private Vector2 scrollPosition;
        private bool showAdvancedSettings = false;
        private bool showPerformanceStats = false;
        
        [MenuItem("lilToon/PCSS Extension/VRChat Optimization Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<VRChatOptimizationSettings>("VRChat Optimization");
            window.minSize = new Vector2(450, 700);
            window.Show();
            
            if (currentProfile == null)
            {
                currentProfile = CreateInstance<VRChatOptimizationProfile>();
                window.LoadDefaultProfile();
            }
        }
        
        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("🎮 VRChat PCSS Optimization Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "VRChatのパフォーマンス設定に合わせてPCSS品質を最適化します。\n" +
                "Quest、PC VR、Desktopそれぞれに最適な設定を自動適用できます。",
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // プロファイル選択
            DrawProfileSelection();
            
            EditorGUILayout.Space();
            
            // VRChat品質マッピング
            DrawVRChatQualityMapping();
            
            EditorGUILayout.Space();
            
            // パフォーマンス設定
            DrawPerformanceSettings();
            
            EditorGUILayout.Space();
            
            // 高度な設定
            DrawAdvancedSettings();
            
            EditorGUILayout.Space();
            
            // パフォーマンス統計
            DrawPerformanceStats();
            
            EditorGUILayout.Space();
            
            // アクションボタン
            DrawActionButtons();
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawProfileSelection()
        {
            EditorGUILayout.LabelField("📋 最適化プロファイル", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Quest最適化", GUILayout.Height(30)))
            {
                LoadQuestProfile();
            }
            
            if (GUILayout.Button("PC VR最適化", GUILayout.Height(30)))
            {
                LoadPCVRProfile();
            }
            
            if (GUILayout.Button("Desktop最適化", GUILayout.Height(30)))
            {
                LoadDesktopProfile();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawVRChatQualityMapping()
        {
            EditorGUILayout.LabelField("🎚️ VRChat品質マッピング", EditorStyles.boldLabel);
            
            EditorGUI.indentLevel++;
            
            currentProfile.vrcUltraQuality = (lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality)EditorGUILayout.EnumPopup(
                "VRC Ultra → PCSS", currentProfile.vrcUltraQuality);
            
            currentProfile.vrcHighQuality = (lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality)EditorGUILayout.EnumPopup(
                "VRC High → PCSS", currentProfile.vrcHighQuality);
            
            currentProfile.vrcMediumQuality = (lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality)EditorGUILayout.EnumPopup(
                "VRC Medium → PCSS", currentProfile.vrcMediumQuality);
            
            currentProfile.vrcLowQuality = (lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality)EditorGUILayout.EnumPopup(
                "VRC Low → PCSS", currentProfile.vrcLowQuality);
            
            currentProfile.vrcMobileQuality = (lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality)EditorGUILayout.EnumPopup(
                "VRC Mobile → PCSS", currentProfile.vrcMobileQuality);
            
            EditorGUI.indentLevel--;
        }
        
        private void DrawPerformanceSettings()
        {
            EditorGUILayout.LabelField("⚡ パフォーマンス設定", EditorStyles.boldLabel);
            
            currentProfile.enableAutoOptimization = EditorGUILayout.Toggle(
                "自動最適化", currentProfile.enableAutoOptimization);
            
            currentProfile.targetFramerate = EditorGUILayout.FloatField(
                "目標フレームレート", currentProfile.targetFramerate);
            
            currentProfile.lowFramerateThreshold = EditorGUILayout.FloatField(
                "低フレームレート閾値", currentProfile.lowFramerateThreshold);
            
            currentProfile.maxAvatarsForHighQuality = EditorGUILayout.IntField(
                "高品質許可アバター数", currentProfile.maxAvatarsForHighQuality);
            
            currentProfile.avatarCullingDistance = EditorGUILayout.FloatField(
                "アバターカリング距離", currentProfile.avatarCullingDistance);
        }
        
        private void DrawAdvancedSettings()
        {
            showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "🔧 高度な設定");
            
            if (showAdvancedSettings)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.LabelField("VRC Light Volumes設定", EditorStyles.boldLabel);
                
                currentProfile.vrcLightVolumeMaxDistance = EditorGUILayout.IntField(
                    "最大距離", currentProfile.vrcLightVolumeMaxDistance);
                
                currentProfile.vrcLightVolumeUpdateFrequency = EditorGUILayout.FloatField(
                    "更新頻度", currentProfile.vrcLightVolumeUpdateFrequency);
                
                currentProfile.enableMobileOptimization = EditorGUILayout.Toggle(
                    "モバイル最適化", currentProfile.enableMobileOptimization);
                
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("シェーダーキーワード", EditorStyles.boldLabel);
                
                currentProfile.enableQuestOptimization = EditorGUILayout.Toggle(
                    "Quest最適化キーワード", currentProfile.enableQuestOptimization);
                
                currentProfile.enableVROptimization = EditorGUILayout.Toggle(
                    "VR最適化キーワード", currentProfile.enableVROptimization);
                
                EditorGUI.indentLevel--;
            }
        }
        
        private void DrawPerformanceStats()
        {
            showPerformanceStats = EditorGUILayout.Foldout(showPerformanceStats, "📊 パフォーマンス統計");
            
            if (showPerformanceStats)
            {
                EditorGUI.indentLevel++;
                
                var optimizer = FindObjectOfType<lilToon.PCSS.Runtime.VRChatPerformanceOptimizer>();
                if (optimizer != null)
                {
                    var stats = optimizer.GetPerformanceStats();
                    
                    EditorGUILayout.LabelField($"現在のFPS: {stats.CurrentFPS:F1}");
                    EditorGUILayout.LabelField($"アバター数: {stats.AvatarCount}");
                    EditorGUILayout.LabelField($"Quest環境: {(stats.IsQuest ? "Yes" : "No")}");
                    EditorGUILayout.LabelField($"VRモード: {(stats.IsVRMode ? "Yes" : "No")}");
                    EditorGUILayout.LabelField($"VRC品質: {stats.VRCQuality}");
                    EditorGUILayout.LabelField($"PCSS品質: {stats.CurrentPCSSQuality}");
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        "パフォーマンス統計を表示するには、シーンにVRChatPerformanceOptimizerを追加してください。",
                        MessageType.Info);
                    
                    if (GUILayout.Button("VRChatPerformanceOptimizerを追加"))
                    {
                        var go = new GameObject("VRChat Performance Optimizer");
                        go.AddComponent<lilToon.PCSS.Runtime.VRChatPerformanceOptimizer>();
                        Selection.activeGameObject = go;
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }
        
        private void DrawActionButtons()
        {
            EditorGUILayout.LabelField("🚀 アクション", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("設定を適用", GUILayout.Height(35)))
            {
                ApplySettings();
            }
            
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("プロファイル保存", GUILayout.Height(35)))
            {
                SaveProfile();
            }
            
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("プロファイル読み込み", GUILayout.Height(35)))
            {
                LoadProfile();
            }
            
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("シーン内のPCSS材質を検索"))
            {
                FindAndOptimizePCSSMaterials();
            }
            
            if (GUILayout.Button("VRChat設定を検証"))
            {
                ValidateVRChatSettings();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void LoadDefaultProfile()
        {
            currentProfile.vrcUltraQuality = lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality.Ultra;
            currentProfile.vrcHighQuality = lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality.High;
            currentProfile.vrcMediumQuality = lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality.Medium;
            currentProfile.vrcLowQuality = lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality.Low;
            currentProfile.vrcMobileQuality = lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality.Low;
            
            currentProfile.enableAutoOptimization = true;
            currentProfile.targetFramerate = 90f;
            currentProfile.lowFramerateThreshold = 45f;
            currentProfile.maxAvatarsForHighQuality = 10;
            currentProfile.avatarCullingDistance = 50f;
            
            currentProfile.vrcLightVolumeMaxDistance = 100;
            currentProfile.vrcLightVolumeUpdateFrequency = 0.1f;
            currentProfile.enableMobileOptimization = false;
        }
        
        private void LoadQuestProfile()
        {
            LoadDefaultProfile();
            
            // Quest用設定
            currentProfile.vrcUltraQuality = lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality.Medium;
            currentProfile.vrcHighQuality = lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality.Medium;
            currentProfile.vrcMediumQuality = lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality.Low;
            currentProfile.vrcLowQuality = lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality.Low;
            currentProfile.vrcMobileQuality = lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality.Low;
            
            currentProfile.targetFramerate = 72f;
            currentProfile.lowFramerateThreshold = 36f;
            currentProfile.maxAvatarsForHighQuality = 5;
            currentProfile.avatarCullingDistance = 25f;
            
            currentProfile.vrcLightVolumeMaxDistance = 50;
            currentProfile.vrcLightVolumeUpdateFrequency = 0.2f;
            currentProfile.enableMobileOptimization = true;
            currentProfile.enableQuestOptimization = true;
            
            Debug.Log("Quest最適化プロファイルを読み込みました");
        }
        
        private void LoadPCVRProfile()
        {
            LoadDefaultProfile();
            
            // PC VR用設定
            currentProfile.targetFramerate = 90f;
            currentProfile.lowFramerateThreshold = 45f;
            currentProfile.maxAvatarsForHighQuality = 8;
            currentProfile.avatarCullingDistance = 35f;
            
            currentProfile.vrcLightVolumeMaxDistance = 75;
            currentProfile.vrcLightVolumeUpdateFrequency = 0.15f;
            currentProfile.enableVROptimization = true;
            
            Debug.Log("PC VR最適化プロファイルを読み込みました");
        }
        
        private void LoadDesktopProfile()
        {
            LoadDefaultProfile();
            
            // Desktop用設定
            currentProfile.targetFramerate = 60f;
            currentProfile.lowFramerateThreshold = 30f;
            currentProfile.maxAvatarsForHighQuality = 15;
            currentProfile.avatarCullingDistance = 75f;
            
            currentProfile.vrcLightVolumeMaxDistance = 150;
            currentProfile.vrcLightVolumeUpdateFrequency = 0.05f;
            
            Debug.Log("Desktop最適化プロファイルを読み込みました");
        }
        
        private void ApplySettings()
        {
            // シーン内のlilToon.PCSS.Runtime.VRChatPerformanceOptimizerを更新
            var optimizers = FindObjectsOfType<lilToon.PCSS.Runtime.VRChatPerformanceOptimizer>();
            
            foreach (var optimizer in optimizers)
            {
                ApplyProfileToOptimizer(optimizer);
            }
            
            // シーン内のlilToon.PCSS.VRCLightVolumesIntegrationを更新
            var lightVolumeIntegrations = FindObjectsOfType<lilToon.PCSS.VRCLightVolumesIntegration>();
            
            foreach (var integration in lightVolumeIntegrations)
            {
                ApplyProfileToLightVolumeIntegration(integration);
            }
            
            // シェーダーキーワードを設定
            ApplyShaderKeywords();
            
            EditorUtility.DisplayDialog("設定適用完了", "VRChat最適化設定が適用されました。", "OK");
            Debug.Log("VRChat最適化設定を適用しました");
        }
        
        private void ApplyProfileToOptimizer(lilToon.PCSS.Runtime.VRChatPerformanceOptimizer optimizer)
        {
            var serializedObject = new SerializedObject(optimizer);
            
            serializedObject.FindProperty("enableAutoOptimization").boolValue = currentProfile.enableAutoOptimization;
            serializedObject.FindProperty("targetFramerate").floatValue = currentProfile.targetFramerate;
            serializedObject.FindProperty("lowFramerateThreshold").floatValue = currentProfile.lowFramerateThreshold;
            serializedObject.FindProperty("maxAvatarsForHighQuality").intValue = currentProfile.maxAvatarsForHighQuality;
            
            serializedObject.FindProperty("vrcUltraQuality").enumValueIndex = (int)currentProfile.vrcUltraQuality;
            serializedObject.FindProperty("vrcHighQuality").enumValueIndex = (int)currentProfile.vrcHighQuality;
            serializedObject.FindProperty("vrcMediumQuality").enumValueIndex = (int)currentProfile.vrcMediumQuality;
            serializedObject.FindProperty("vrcLowQuality").enumValueIndex = (int)currentProfile.vrcLowQuality;
            serializedObject.FindProperty("vrcMobileQuality").enumValueIndex = (int)currentProfile.vrcMobileQuality;
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void ApplyProfileToLightVolumeIntegration(lilToon.PCSS.VRCLightVolumesIntegration integration)
        {
            var serializedObject = new SerializedObject(integration);
            
            serializedObject.FindProperty("maxLightVolumeDistance").intValue = currentProfile.vrcLightVolumeMaxDistance;
            serializedObject.FindProperty("updateFrequency").floatValue = currentProfile.vrcLightVolumeUpdateFrequency;
            serializedObject.FindProperty("enableMobileOptimization").boolValue = currentProfile.enableMobileOptimization;
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void ApplyShaderKeywords()
        {
            if (currentProfile.enableQuestOptimization)
            {
                Shader.EnableKeyword("VRC_QUEST_OPTIMIZATION");
            }
            else
            {
                Shader.DisableKeyword("VRC_QUEST_OPTIMIZATION");
            }
            
            if (currentProfile.enableMobileOptimization)
            {
                Shader.EnableKeyword("VRC_LIGHT_VOLUMES_MOBILE");
            }
            else
            {
                Shader.DisableKeyword("VRC_LIGHT_VOLUMES_MOBILE");
            }
        }
        
        private void FindAndOptimizePCSSMaterials()
        {
            var renderers = FindObjectsOfType<Renderer>();
            var pcssMaterials = new List<Material>();
            
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    if (lilToon.PCSS.Runtime.PCSSUtilities.IsPCSSMaterial(material))
                    {
                        pcssMaterials.Add(material);
                    }
                }
            }
            
            if (pcssMaterials.Count > 0)
            {
                EditorUtility.DisplayDialog(
                    "PCSS材質検索完了",
                    $"{pcssMaterials.Count}個のPCSS対応材質が見つかりました。\n" +
                    "現在の設定で最適化しますか？",
                    "OK");
                
                Debug.Log($"PCSS材質を{pcssMaterials.Count}個検出しました");
            }
            else
            {
                EditorUtility.DisplayDialog("検索結果", "PCSS対応材質が見つかりませんでした。", "OK");
            }
        }
        
        private void ValidateVRChatSettings()
        {
            var issues = new List<string>();
            
            // lilToon.PCSS.Runtime.VRChatPerformanceOptimizerの存在確認
            if (FindObjectOfType<lilToon.PCSS.Runtime.VRChatPerformanceOptimizer>() == null)
            {
                issues.Add("lilToon.PCSS.Runtime.VRChatPerformanceOptimizerが見つかりません");
            }
            
            // lilToon.PCSS.VRCLightVolumesIntegrationの存在確認
            if (FindObjectOfType<lilToon.PCSS.VRCLightVolumesIntegration>() == null)
            {
                issues.Add("lilToon.PCSS.VRCLightVolumesIntegrationが見つかりません");
            }
            
            // 品質設定の妥当性確認
            if (currentProfile.targetFramerate < 30f)
            {
                issues.Add("目標フレームレートが低すぎます（30fps未満）");
            }
            
            if (currentProfile.lowFramerateThreshold >= currentProfile.targetFramerate)
            {
                issues.Add("低フレームレート閾値が目標フレームレート以上です");
            }
            
            // 結果表示
            if (issues.Count == 0)
            {
                EditorUtility.DisplayDialog(
                    "✅ 検証完了",
                    "VRChat設定に問題はありません。",
                    "OK");
            }
            else
            {
                string message = "以下の問題が見つかりました:\n\n" + string.Join("\n", issues);
                EditorUtility.DisplayDialog("❌ 設定エラー", message, "OK");
            }
        }
        
        private void SaveProfile()
        {
            string path = EditorUtility.SaveFilePanel(
                "VRChat最適化プロファイルを保存",
                "Assets",
                "VRChatOptimizationProfile",
                "asset");
            
            if (!string.IsNullOrEmpty(path))
            {
                path = FileUtil.GetProjectRelativePath(path);
                AssetDatabase.CreateAsset(currentProfile, path);
                AssetDatabase.SaveAssets();
                
                EditorUtility.DisplayDialog("保存完了", $"プロファイルを保存しました:\n{path}", "OK");
            }
        }
        
        private void LoadProfile()
        {
            string path = EditorUtility.OpenFilePanel(
                "VRChat最適化プロファイルを読み込み",
                "Assets",
                "asset");
            
            if (!string.IsNullOrEmpty(path))
            {
                path = FileUtil.GetProjectRelativePath(path);
                var profile = AssetDatabase.LoadAssetAtPath<VRChatOptimizationProfile>(path);
                
                if (profile != null)
                {
                    currentProfile = profile;
                    EditorUtility.DisplayDialog("読み込み完了", $"プロファイルを読み込みました:\n{path}", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("エラー", "プロファイルの読み込みに失敗しました。", "OK");
                }
            }
        }
    }
    
    /// <summary>
    /// VRChat最適化プロファイル
    /// </summary>
    [CreateAssetMenu(fileName = "VRChatOptimizationProfile", menuName = "lilToon/PCSS/VRChat Optimization Profile")]
    public class VRChatOptimizationProfile : ScriptableObject
    {
        [Header("VRChat Quality Mapping")]
        public lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality vrcUltraQuality = lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality.Ultra;
        public lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality vrcHighQuality = lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality.High;
        public lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality vrcMediumQuality = lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality.Medium;
        public lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality vrcLowQuality = lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality.Low;
        public lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality vrcMobileQuality = lilToon.PCSS.Runtime.PCSSUtilities.PCSSQuality.Low;
        
        [Header("Performance Settings")]
        public bool enableAutoOptimization = true;
        public float targetFramerate = 90f;
        public float lowFramerateThreshold = 45f;
        public int maxAvatarsForHighQuality = 10;
        public float avatarCullingDistance = 50f;
        
        [Header("VRC Light Volumes")]
        public int vrcLightVolumeMaxDistance = 100;
        public float vrcLightVolumeUpdateFrequency = 0.1f;
        public bool enableMobileOptimization = false;
        
        [Header("Shader Keywords")]
        public bool enableQuestOptimization = false;
        public bool enableVROptimization = false;
    }
} 