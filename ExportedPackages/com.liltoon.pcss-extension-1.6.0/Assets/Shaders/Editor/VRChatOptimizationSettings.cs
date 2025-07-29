using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using lilToon.PCSS.Runtime;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// VRChat最適化設定エチE��ター
    /// </summary>
    public class VRChatOptimizationSettings : EditorWindow
    {
        private static VRChatOptimizationProfile currentProfile;
        private Vector2 scrollPosition;
        private bool showAdvancedSettings = false;
        private bool showPerformanceStats = false;
        
        [MenuItem("Tools/lilToon PCSS Extension/Utilities/PCSS Extension/VRChat Optimization Settings")]
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
                "VRChatのパフォーマンス設定に合わせてPCSS品質を最適化します、En" +
                "Quest、PC VR、Desktopそれぞれに最適な設定を自動適用できます、E,
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // プロファイル選抁E
            DrawProfileSelection();
            
            EditorGUILayout.Space();
            
            // VRChat品質マッピング
            DrawVRChatQualityMapping();
            
            EditorGUILayout.Space();
            
            // パフォーマンス設宁E
            DrawPerformanceSettings();
            
            EditorGUILayout.Space();
            
            // 高度な設宁E
            DrawAdvancedSettings();
            
            EditorGUILayout.Space();
            
            // パフォーマンス統訁E
            DrawPerformanceStats();
            
            EditorGUILayout.Space();
            
            // アクションボタン
            DrawActionButtons();
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawProfileSelection()
        {
            EditorGUILayout.LabelField("📋 最適化�Eロファイル", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Quest最適匁E, GUILayout.Height(30)))
            {
                LoadQuestProfile();
            }
            
            if (GUILayout.Button("PC VR最適匁E, GUILayout.Height(30)))
            {
                LoadPCVRProfile();
            }
            
            if (GUILayout.Button("Desktop最適匁E, GUILayout.Height(30)))
            {
                LoadDesktopProfile();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawVRChatQualityMapping()
        {
            EditorGUILayout.LabelField("🎚�E�EVRChat品質マッピング", EditorStyles.boldLabel);
            
            EditorGUI.indentLevel++;
            
            currentProfile.vrcUltraQuality = (PCSSUtilities.PCSSQuality)EditorGUILayout.EnumPopup(
                "VRC Ultra ↁEPCSS", currentProfile.vrcUltraQuality);
            
            currentProfile.vrcHighQuality = (PCSSUtilities.PCSSQuality)EditorGUILayout.EnumPopup(
                "VRC High ↁEPCSS", currentProfile.vrcHighQuality);
            
            currentProfile.vrcMediumQuality = (PCSSUtilities.PCSSQuality)EditorGUILayout.EnumPopup(
                "VRC Medium ↁEPCSS", currentProfile.vrcMediumQuality);
            
            currentProfile.vrcLowQuality = (PCSSUtilities.PCSSQuality)EditorGUILayout.EnumPopup(
                "VRC Low ↁEPCSS", currentProfile.vrcLowQuality);
            
            currentProfile.vrcMobileQuality = (PCSSUtilities.PCSSQuality)EditorGUILayout.EnumPopup(
                "VRC Mobile ↁEPCSS", currentProfile.vrcMobileQuality);
            
            EditorGUI.indentLevel--;
        }
        
        private void DrawPerformanceSettings()
        {
            EditorGUILayout.LabelField("⚡ パフォーマンス設宁E, EditorStyles.boldLabel);
            
            currentProfile.enableAutoOptimization = EditorGUILayout.Toggle(
                "自動最適匁E, currentProfile.enableAutoOptimization);
            
            currentProfile.targetFramerate = EditorGUILayout.FloatField(
                "目標フレームレーチE, currentProfile.targetFramerate);
            
            currentProfile.lowFramerateThreshold = EditorGUILayout.FloatField(
                "低フレームレート閾値", currentProfile.lowFramerateThreshold);
            
            currentProfile.maxAvatarsForHighQuality = EditorGUILayout.IntField(
                "高品質許可アバター数", currentProfile.maxAvatarsForHighQuality);
            
            currentProfile.avatarCullingDistance = EditorGUILayout.FloatField(
                "アバターカリング距離", currentProfile.avatarCullingDistance);
        }
        
        private void DrawAdvancedSettings()
        {
            showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "🔧 高度な設宁E);
            
            if (showAdvancedSettings)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.LabelField("VRC Light Volumes設宁E, EditorStyles.boldLabel);
                
                currentProfile.vrcLightVolumeMaxDistance = EditorGUILayout.IntField(
                    "最大距離", currentProfile.vrcLightVolumeMaxDistance);
                
                currentProfile.vrcLightVolumeUpdateFrequency = EditorGUILayout.FloatField(
                    "更新頻度", currentProfile.vrcLightVolumeUpdateFrequency);
                
                currentProfile.enableMobileOptimization = EditorGUILayout.Toggle(
                    "モバイル最適匁E, currentProfile.enableMobileOptimization);
                
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("シェーダーキーワーチE, EditorStyles.boldLabel);
                
                currentProfile.enableQuestOptimization = EditorGUILayout.Toggle(
                    "Quest最適化キーワーチE, currentProfile.enableQuestOptimization);
                
                currentProfile.enableVROptimization = EditorGUILayout.Toggle(
                    "VR最適化キーワーチE, currentProfile.enableVROptimization);
                
                EditorGUI.indentLevel--;
            }
        }
        
        private void DrawPerformanceStats()
        {
            // 統計機�Eは不要�Eため無効匁E
            EditorGUILayout.HelpBox("パフォーマンス統計機�Eは無効化されてぁE��す、E, MessageType.Info);
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
            if (GUILayout.Button("プロファイル保孁E, GUILayout.Height(35)))
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
            
            if (GUILayout.Button("シーン冁E�EPCSS材質を検索"))
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
            currentProfile.vrcUltraQuality = PCSSUtilities.PCSSQuality.Ultra;
            currentProfile.vrcHighQuality = PCSSUtilities.PCSSQuality.High;
            currentProfile.vrcMediumQuality = PCSSUtilities.PCSSQuality.Medium;
            currentProfile.vrcLowQuality = PCSSUtilities.PCSSQuality.Low;
            currentProfile.vrcMobileQuality = PCSSUtilities.PCSSQuality.Low;
            
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
            
            // Quest用設宁E
            currentProfile.vrcUltraQuality = PCSSUtilities.PCSSQuality.Medium;
            currentProfile.vrcHighQuality = PCSSUtilities.PCSSQuality.Medium;
            currentProfile.vrcMediumQuality = PCSSUtilities.PCSSQuality.Low;
            currentProfile.vrcLowQuality = PCSSUtilities.PCSSQuality.Low;
            currentProfile.vrcMobileQuality = PCSSUtilities.PCSSQuality.Low;
            
            currentProfile.targetFramerate = 72f;
            currentProfile.lowFramerateThreshold = 36f;
            currentProfile.maxAvatarsForHighQuality = 5;
            currentProfile.avatarCullingDistance = 25f;
            
            currentProfile.vrcLightVolumeMaxDistance = 50;
            currentProfile.vrcLightVolumeUpdateFrequency = 0.2f;
            currentProfile.enableMobileOptimization = true;
            currentProfile.enableQuestOptimization = true;
            
            Debug.Log("Quest最適化�Eロファイルを読み込みました");
        }
        
        private void LoadPCVRProfile()
        {
            LoadDefaultProfile();
            
            // PC VR用設宁E
            currentProfile.targetFramerate = 90f;
            currentProfile.lowFramerateThreshold = 45f;
            currentProfile.maxAvatarsForHighQuality = 8;
            currentProfile.avatarCullingDistance = 35f;
            
            currentProfile.vrcLightVolumeMaxDistance = 75;
            currentProfile.vrcLightVolumeUpdateFrequency = 0.15f;
            currentProfile.enableVROptimization = true;
            
            Debug.Log("PC VR最適化�Eロファイルを読み込みました");
        }
        
        private void LoadDesktopProfile()
        {
            LoadDefaultProfile();
            
            // Desktop用設宁E
            currentProfile.targetFramerate = 60f;
            currentProfile.lowFramerateThreshold = 30f;
            currentProfile.maxAvatarsForHighQuality = 15;
            currentProfile.avatarCullingDistance = 75f;
            
            currentProfile.vrcLightVolumeMaxDistance = 150;
            currentProfile.vrcLightVolumeUpdateFrequency = 0.05f;
            
            Debug.Log("Desktop最適化�Eロファイルを読み込みました");
        }
        
        private void ApplySettings()
        {
            // シーン冁E�EVRChatPerformanceOptimizerを更新
            var optimizers = FindObjectsOfType<VRChatPerformanceOptimizer>();
            
            foreach (var optimizer in optimizers)
            {
                ApplyProfileToOptimizer(optimizer);
            }
            
            // シーン冁E�EVRCLightVolumesIntegrationを更新
            var lightVolumeIntegrations = FindObjectsOfType<VRCLightVolumesIntegration>();
            
            foreach (var integration in lightVolumeIntegrations)
            {
                ApplyProfileToLightVolumeIntegration(integration);
            }
            
            // シェーダーキーワードを設宁E
            ApplyShaderKeywords();
            
            EditorUtility.DisplayDialog("設定適用完亁E, "VRChat最適化設定が適用されました、E, "OK");
            Debug.Log("VRChat最適化設定を適用しました");
        }
        
        private void ApplyProfileToOptimizer(VRChatPerformanceOptimizer optimizer)
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
        
        private void ApplyProfileToLightVolumeIntegration(VRCLightVolumesIntegration integration)
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
                    if (PCSSUtilities.IsPCSSCompatible(material))
                    {
                        pcssMaterials.Add(material);
                    }
                }
            }
            
            if (pcssMaterials.Count > 0)
            {
                EditorUtility.DisplayDialog(
                    "PCSS材質検索完亁E,
                    $"{pcssMaterials.Count}個�EPCSS対応材質が見つかりました、En" +
                    "現在の設定で最適化しますか�E�E,
                    "OK");
                
                Debug.Log($"PCSS材質を{pcssMaterials.Count}個検�Eしました");
            }
            else
            {
                EditorUtility.DisplayDialog("検索結果", "PCSS対応材質が見つかりませんでした、E, "OK");
            }
        }
        
        private void ValidateVRChatSettings()
        {
            var issues = new List<string>();
            
            // VRChatPerformanceOptimizerの存在確誁E
            if (FindObjectOfType<VRChatPerformanceOptimizer>() == null)
            {
                issues.Add("VRChatPerformanceOptimizerが見つかりません");
            }
            
            // VRCLightVolumesIntegrationの存在確誁E
            if (FindObjectOfType<VRCLightVolumesIntegration>() == null)
            {
                issues.Add("VRCLightVolumesIntegrationが見つかりません");
            }
            
            // 品質設定�E妥当性確誁E
            if (currentProfile.targetFramerate < 30f)
            {
                issues.Add("目標フレームレートが低すぎます！E0fps未満�E�E);
            }
            
            if (currentProfile.lowFramerateThreshold >= currentProfile.targetFramerate)
            {
                issues.Add("低フレームレート閾値が目標フレームレート以上でぁE);
            }
            
            // 結果表示
            if (issues.Count == 0)
            {
                EditorUtility.DisplayDialog(
                    "✁E検証完亁E,
                    "VRChat設定に問題�Eありません、E,
                    "OK");
            }
            else
            {
                string message = "以下�E問題が見つかりました:\n\n" + string.Join("\n", issues);
                EditorUtility.DisplayDialog("❁E設定エラー", message, "OK");
            }
        }
        
        private void SaveProfile()
        {
            string path = EditorUtility.SaveFilePanel(
                "VRChat最適化�Eロファイルを保孁E,
                "Assets",
                "VRChatOptimizationProfile",
                "asset");
            
            if (!string.IsNullOrEmpty(path))
            {
                path = FileUtil.GetProjectRelativePath(path);
                AssetDatabase.CreateAsset(currentProfile, path);
                AssetDatabase.SaveAssets();
                
                EditorUtility.DisplayDialog("保存完亁E, $"プロファイルを保存しました:\n{path}", "OK");
            }
        }
        
        private void LoadProfile()
        {
            string path = EditorUtility.OpenFilePanel(
                "VRChat最適化�Eロファイルを読み込み",
                "Assets",
                "asset");
            
            if (!string.IsNullOrEmpty(path))
            {
                path = FileUtil.GetProjectRelativePath(path);
                var profile = AssetDatabase.LoadAssetAtPath<VRChatOptimizationProfile>(path);
                
                if (profile != null)
                {
                    currentProfile = profile;
                    EditorUtility.DisplayDialog("読み込み完亁E, $"プロファイルを読み込みました:\n{path}", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("エラー", "プロファイルの読み込みに失敗しました、E, "OK");
                }
            }
        }
    }
    
    /// <summary>
    /// VRChat最適化�Eロファイル
    /// </summary>
    [CreateAssetMenu(fileName = "VRChatOptimizationProfile", menuName = "lilToon/PCSS/VRChat Optimization Profile")]
    public class VRChatOptimizationProfile : ScriptableObject
    {
        [Header("VRChat Quality Mapping")]
        public PCSSUtilities.PCSSQuality vrcUltraQuality = PCSSUtilities.PCSSQuality.Ultra;
        public PCSSUtilities.PCSSQuality vrcHighQuality = PCSSUtilities.PCSSQuality.High;
        public PCSSUtilities.PCSSQuality vrcMediumQuality = PCSSUtilities.PCSSQuality.Medium;
        public PCSSUtilities.PCSSQuality vrcLowQuality = PCSSUtilities.PCSSQuality.Low;
        public PCSSUtilities.PCSSQuality vrcMobileQuality = PCSSUtilities.PCSSQuality.Low;
        
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
