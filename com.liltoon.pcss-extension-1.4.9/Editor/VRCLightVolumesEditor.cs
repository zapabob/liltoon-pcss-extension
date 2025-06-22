using UnityEngine;
using UnityEditor;
using System.Linq;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// VRC Light Volumes統合エディタ
    /// </summary>
    [CustomEditor(typeof(VRCLightVolumesIntegration))]
    public class VRCLightVolumesEditor : UnityEditor.Editor
    {
        private VRCLightVolumesIntegration integration;
        private bool showAdvancedSettings = false;
        private bool showStats = true;
        
        private void OnEnable()
        {
            integration = (VRCLightVolumesIntegration)target;
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("🌟 VRC Light Volumes Integration", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "VRC Light Volumes統合システム\n" +
                "REDSIMのVRC Light VolumesとlilToon PCSS Extensionを統合します。",
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            // 基本設定
            DrawBasicSettings();
            
            EditorGUILayout.Space();
            
            // パフォーマンス設定
            DrawPerformanceSettings();
            
            EditorGUILayout.Space();
            
            // 高度な設定
            DrawAdvancedSettings();
            
            EditorGUILayout.Space();
            
            // 統計情報
            if (Application.isPlaying)
            {
                DrawStats();
            }
            
            EditorGUILayout.Space();
            
            // アクションボタン
            DrawActionButtons();
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawBasicSettings()
        {
            EditorGUILayout.LabelField("基本設定", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enableVRCLightVolumes"), 
                new GUIContent("VRC Light Volumes有効", "VRC Light Volumes統合を有効にします"));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoDetectLightVolumes"), 
                new GUIContent("自動検出", "シーン内のLight Volume Managerを自動検出します"));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lightVolumeIntensity"), 
                new GUIContent("Light Volume強度", "Light Volumeライティングの強度"));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lightVolumeTint"), 
                new GUIContent("Light Volume色調", "Light Volumeライティングの色調"));
        }
        
        private void DrawPerformanceSettings()
        {
            EditorGUILayout.LabelField("パフォーマンス設定", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enableMobileOptimization"), 
                new GUIContent("モバイル最適化", "モバイルプラットフォーム向けの最適化を有効にします"));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxLightVolumeDistance"), 
                new GUIContent("最大距離", "Light Volumeが影響する最大距離"));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("updateFrequency"), 
                new GUIContent("更新頻度", "Light Volume更新の間隔（秒）"));
        }
        
        private void DrawAdvancedSettings()
        {
            showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "高度な設定", true);
            
            if (showAdvancedSettings)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.HelpBox(
                    "高度な設定は経験豊富なユーザー向けです。\n" +
                    "変更する前に設定をバックアップしてください。",
                    MessageType.Warning);
                
                // シェーダーキーワード管理
                EditorGUILayout.LabelField("シェーダーキーワード", EditorStyles.miniBoldLabel);
                
                bool vrcLightVolumesEnabled = Shader.IsKeywordEnabled("VRC_LIGHT_VOLUMES_ENABLED");
                bool newVrcLightVolumesEnabled = EditorGUILayout.Toggle("VRC_LIGHT_VOLUMES_ENABLED", vrcLightVolumesEnabled);
                if (newVrcLightVolumesEnabled != vrcLightVolumesEnabled)
                {
                    if (newVrcLightVolumesEnabled)
                        Shader.EnableKeyword("VRC_LIGHT_VOLUMES_ENABLED");
                    else
                        Shader.DisableKeyword("VRC_LIGHT_VOLUMES_ENABLED");
                }
                
                bool vrcLightVolumesMobile = Shader.IsKeywordEnabled("VRC_LIGHT_VOLUMES_MOBILE");
                bool newVrcLightVolumesMobile = EditorGUILayout.Toggle("VRC_LIGHT_VOLUMES_MOBILE", vrcLightVolumesMobile);
                if (newVrcLightVolumesMobile != vrcLightVolumesMobile)
                {
                    if (newVrcLightVolumesMobile)
                        Shader.EnableKeyword("VRC_LIGHT_VOLUMES_MOBILE");
                    else
                        Shader.DisableKeyword("VRC_LIGHT_VOLUMES_MOBILE");
                }
                
                EditorGUI.indentLevel--;
            }
        }
        
        private void DrawStats()
        {
            showStats = EditorGUILayout.Foldout(showStats, "統計情報", true);
            
            if (showStats)
            {
                EditorGUI.indentLevel++;
                
                var stats = integration.GetStats();
                
                EditorGUILayout.LabelField("Light Volumes有効", stats.EnabledLightVolumes.ToString());
                EditorGUILayout.LabelField("検出されたPCSSマテリアル", stats.DetectedPCSSMaterials.ToString());
                EditorGUILayout.LabelField("対象レンダラー", stats.TargetRenderers.ToString());
                EditorGUILayout.LabelField("Light Volume強度", stats.LightVolumeIntensity.ToString("F2"));
                EditorGUILayout.LabelField("モバイル最適化", stats.IsMobileOptimized.ToString());
                
                EditorGUI.indentLevel--;
            }
        }
        
        private void DrawActionButtons()
        {
            EditorGUILayout.LabelField("アクション", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("🔍 Light Volumes検出"))
            {
                integration.DetectAndInitialize();
                EditorUtility.DisplayDialog("検出完了", "Light Volumesの検出が完了しました。", "OK");
            }
            
            if (GUILayout.Button("🔧 PCSS材質を更新"))
            {
                UpdatePCSSMaterials();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("📋 統計をコピー"))
            {
                CopyStatsToClipboard();
            }
            
            if (GUILayout.Button("❓ ヘルプ"))
            {
                ShowHelpDialog();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void UpdatePCSSMaterials()
        {
            var allMaterials = Resources.FindObjectsOfTypeAll<Material>()
                .Where(m => m.shader != null && 
                           (m.shader.name.Contains("PCSS") || 
                            m.shader.name.Contains("lilToon") || 
                            m.shader.name.Contains("Poiyomi")))
                .ToArray();
            
            int updatedCount = 0;
            
            foreach (var material in allMaterials)
            {
                // VRC Light Volumes対応プロパティを設定
                if (material.HasProperty("_VRCLightVolumeIntensity"))
                {
                    material.SetFloat("_VRCLightVolumeIntensity", integration.GetStats().LightVolumeIntensity);
                    updatedCount++;
                }
            }
            
            EditorUtility.DisplayDialog("更新完了", 
                $"{updatedCount}個のPCSSマテリアルを更新しました。", "OK");
        }
        
        private void CopyStatsToClipboard()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("エラー", "統計情報はプレイモード中のみ利用可能です。", "OK");
                return;
            }
            
            var stats = integration.GetStats();
            string statsText = $"VRC Light Volumes統計情報\n" +
                              $"Light Volumes有効: {stats.EnabledLightVolumes}\n" +
                              $"検出されたPCSSマテリアル: {stats.DetectedPCSSMaterials}\n" +
                              $"対象レンダラー: {stats.TargetRenderers}\n" +
                              $"Light Volume強度: {stats.LightVolumeIntensity:F2}\n" +
                              $"モバイル最適化: {stats.IsMobileOptimized}";
            
            EditorGUIUtility.systemCopyBuffer = statsText;
            EditorUtility.DisplayDialog("コピー完了", "統計情報をクリップボードにコピーしました。", "OK");
        }
        
        private void ShowHelpDialog()
        {
            EditorUtility.DisplayDialog("VRC Light Volumes統合ヘルプ",
                "VRC Light Volumes統合システム\n\n" +
                "このシステムはREDSIMのVRC Light Volumesと\n" +
                "lilToon PCSS Extensionを統合します。\n\n" +
                "使用方法:\n" +
                "1. VRC Light Volumesをシーンに設置\n" +
                "2. 「Light Volumes検出」ボタンをクリック\n" +
                "3. PCSS対応シェーダーが自動的に統合されます\n\n" +
                "詳細はドキュメントを参照してください。",
                "OK");
        }
    }
    
    /// <summary>
    /// VRC Light Volumes統合メニュー
    /// </summary>
    public static class VRCLightVolumesMenu
    {
        [MenuItem("lilToon/PCSS Extension/VRC Light Volumes/Add Integration Component")]
        public static void AddIntegrationComponent()
        {
            var selected = Selection.activeGameObject;
            if (selected == null)
            {
                EditorUtility.DisplayDialog("エラー", "GameObjectを選択してください。", "OK");
                return;
            }
            
            if (selected.GetComponent<VRCLightVolumesIntegration>() != null)
            {
                EditorUtility.DisplayDialog("警告", "VRCLightVolumesIntegrationは既に追加されています。", "OK");
                return;
            }
            
            selected.AddComponent<VRCLightVolumesIntegration>();
            EditorUtility.DisplayDialog("完了", "VRCLightVolumesIntegrationを追加しました。", "OK");
        }
        
        [MenuItem("lilToon/PCSS Extension/VRC Light Volumes/Create Light Volume Manager")]
        public static void CreateLightVolumeManager()
        {
            var go = new GameObject("VRC Light Volumes Manager");
            go.transform.position = Vector3.zero;
            
            var integration = go.AddComponent<VRCLightVolumesIntegration>();
            
            Selection.activeGameObject = go;
            
            EditorUtility.DisplayDialog("完了", 
                "VRC Light Volumes Managerを作成しました。\n" +
                "シーンにVRC Light Volumesを設置してから\n" +
                "「Light Volumes検出」ボタンをクリックしてください。", "OK");
        }
        
        [MenuItem("lilToon/PCSS Extension/VRC Light Volumes/Enable Global Light Volumes")]
        public static void EnableGlobalLightVolumes()
        {
            Shader.EnableKeyword("VRC_LIGHT_VOLUMES_ENABLED");
            EditorUtility.DisplayDialog("完了", "グローバルVRC Light Volumesを有効にしました。", "OK");
        }
        
        [MenuItem("lilToon/PCSS Extension/VRC Light Volumes/Disable Global Light Volumes")]
        public static void DisableGlobalLightVolumes()
        {
            Shader.DisableKeyword("VRC_LIGHT_VOLUMES_ENABLED");
            Shader.DisableKeyword("VRC_LIGHT_VOLUMES_MOBILE");
            EditorUtility.DisplayDialog("完了", "グローバルVRC Light Volumesを無効にしました。", "OK");
        }
    }
}
