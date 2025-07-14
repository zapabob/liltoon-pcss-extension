using UnityEngine;
using UnityEditor;
using System.Linq;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// VRC Light Volumes統合エチE��タ
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
                "VRC Light Volumes統合シスチE��\n" +
                "REDSIMのVRC Light VolumesとlilToon PCSS Extensionを統合します、E,
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            // 基本設宁E
            DrawBasicSettings();
            
            EditorGUILayout.Space();
            
            // パフォーマンス設宁E
            DrawPerformanceSettings();
            
            EditorGUILayout.Space();
            
            // 高度な設宁E
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
            EditorGUILayout.LabelField("基本設宁E, EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enableVRCLightVolumes"), 
                new GUIContent("VRC Light Volumes有効", "VRC Light Volumes統合を有効にしまぁE));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoDetectLightVolumes"), 
                new GUIContent("自動検�E", "シーン冁E�ELight Volume Managerを�E動検�EしまぁE));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lightVolumeIntensity"), 
                new GUIContent("Light Volume強度", "Light VolumeライチE��ングの強度"));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lightVolumeTint"), 
                new GUIContent("Light Volume色調", "Light VolumeライチE��ングの色調"));
        }
        
        private void DrawPerformanceSettings()
        {
            EditorGUILayout.LabelField("パフォーマンス設宁E, EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enableMobileOptimization"), 
                new GUIContent("モバイル最適匁E, "モバイルプラチE��フォーム向けの最適化を有効にしまぁE));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxLightVolumeDistance"), 
                new GUIContent("最大距離", "Light Volumeが影響する最大距離"));
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("updateFrequency"), 
                new GUIContent("更新頻度", "Light Volume更新の間隔�E�秒！E));
        }
        
        private void DrawAdvancedSettings()
        {
            showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "高度な設宁E, true);
            
            if (showAdvancedSettings)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.HelpBox(
                    "高度な設定�E経験豊富なユーザー向けです、En" +
                    "変更する前に設定をバックアチE�Eしてください、E,
                    MessageType.Warning);
                
                // シェーダーキーワード管琁E
                EditorGUILayout.LabelField("シェーダーキーワーチE, EditorStyles.miniBoldLabel);
                
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
                EditorGUILayout.LabelField("検�EされたPCSSマテリアル", stats.DetectedPCSSMaterials.ToString());
                EditorGUILayout.LabelField("対象レンダラー", stats.TargetRenderers.ToString());
                EditorGUILayout.LabelField("Light Volume強度", stats.LightVolumeIntensity.ToString("F2"));
                EditorGUILayout.LabelField("モバイル最適匁E, stats.IsMobileOptimized.ToString());
                
                EditorGUI.indentLevel--;
            }
        }
        
        private void DrawActionButtons()
        {
            EditorGUILayout.LabelField("アクション", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("🔍 Light Volumes検�E"))
            {
                integration.DetectAndInitialize();
                EditorUtility.DisplayDialog("検�E完亁E, "Light Volumesの検�Eが完亁E��ました、E, "OK");
            }
            
            if (GUILayout.Button("🔧 PCSS材質を更新"))
            {
                UpdatePCSSMaterials();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("📋 統計をコピ�E"))
            {
                CopyStatsToClipboard();
            }
            
            if (GUILayout.Button("❁EヘルチE))
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
                // VRC Light Volumes対応�Eロパティを設宁E
                if (material.HasProperty("_VRCLightVolumeIntensity"))
                {
                    material.SetFloat("_VRCLightVolumeIntensity", integration.GetStats().LightVolumeIntensity);
                    updatedCount++;
                }
            }
            
            EditorUtility.DisplayDialog("更新完亁E, 
                $"{updatedCount}個�EPCSSマテリアルを更新しました、E, "OK");
        }
        
        private void CopyStatsToClipboard()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("エラー", "統計情報はプレイモード中のみ利用可能です、E, "OK");
                return;
            }
            
            var stats = integration.GetStats();
            string statsText = $"VRC Light Volumes統計情報\n" +
                              $"Light Volumes有効: {stats.EnabledLightVolumes}\n" +
                              $"検�EされたPCSSマテリアル: {stats.DetectedPCSSMaterials}\n" +
                              $"対象レンダラー: {stats.TargetRenderers}\n" +
                              $"Light Volume強度: {stats.LightVolumeIntensity:F2}\n" +
                              $"モバイル最適匁E {stats.IsMobileOptimized}";
            
            EditorGUIUtility.systemCopyBuffer = statsText;
            EditorUtility.DisplayDialog("コピ�E完亁E, "統計情報をクリチE�Eボ�Eドにコピ�Eしました、E, "OK");
        }
        
        private void ShowHelpDialog()
        {
            EditorUtility.DisplayDialog("VRC Light Volumes統合�EルチE,
                "VRC Light Volumes統合シスチE��\n\n" +
                "こ�EシスチE��はREDSIMのVRC Light Volumesと\n" +
                "lilToon PCSS Extensionを統合します、En\n" +
                "使用方況E\n" +
                "1. VRC Light Volumesをシーンに設置\n" +
                "2. 「Light Volumes検�E」�EタンをクリチE��\n" +
                "3. PCSS対応シェーダーが�E動的に統合されます\n\n" +
                "詳細はドキュメントを参�Eしてください、E,
                "OK");
        }
    }
    
    /// <summary>
    /// VRC Light Volumes統合メニュー
    /// </summary>
    public static class VRCLightVolumesMenu
    {
        private const string MenuRoot = "Tools/lilToon PCSS Extension/";
        private const string UtilitiesMenu = MenuRoot + "Utilities/";
        private const string PCSSMenu = UtilitiesMenu + "PCSS Extension/";
        private const string LightVolumesMenu = PCSSMenu + "VRC Light Volumes/";
        private const string AddIntegrationComponentMenu = LightVolumesMenu + "Add Integration Component";
        private const string CreateManagerMenu = LightVolumesMenu + "Create Light Volume Manager";
        private const string EnableGlobalMenu = LightVolumesMenu + "Enable Global Light Volumes";
        private const string DisableGlobalMenu = LightVolumesMenu + "Disable Global Light Volumes";
        
        [MenuItem(AddIntegrationComponentMenu)]
        public static void AddIntegrationComponent()
        {
            var selected = Selection.activeGameObject;
            if (selected == null)
            {
                EditorUtility.DisplayDialog("エラー", "GameObjectを選択してください、E, "OK");
                return;
            }
            
            if (selected.GetComponent<VRCLightVolumesIntegration>() != null)
            {
                EditorUtility.DisplayDialog("警呁E, "VRCLightVolumesIntegrationは既に追加されてぁE��す、E, "OK");
                return;
            }
            
            selected.AddComponent<VRCLightVolumesIntegration>();
            EditorUtility.DisplayDialog("完亁E, "VRCLightVolumesIntegrationを追加しました、E, "OK");
        }
        
        [MenuItem(CreateManagerMenu)]
        public static void CreateLightVolumeManager()
        {
            var go = new GameObject("VRC Light Volumes Manager");
            go.transform.position = Vector3.zero;
            
            var integration = go.AddComponent<VRCLightVolumesIntegration>();
            
            Selection.activeGameObject = go;
            
            EditorUtility.DisplayDialog("完亁E, 
                "VRC Light Volumes Managerを作�Eしました、En" +
                "シーンにVRC Light Volumesを設置してから\n" +
                "「Light Volumes検�E」�EタンをクリチE��してください、E, "OK");
        }
        
        [MenuItem(EnableGlobalMenu)]
        public static void EnableGlobalLightVolumes()
        {
            Shader.EnableKeyword("VRC_LIGHT_VOLUMES_ENABLED");
            EditorUtility.DisplayDialog("完亁E, "グローバルVRC Light Volumesを有効にしました、E, "OK");
        }
        
        [MenuItem(DisableGlobalMenu)]
        public static void DisableGlobalLightVolumes()
        {
            Shader.DisableKeyword("VRC_LIGHT_VOLUMES_ENABLED");
            Shader.DisableKeyword("VRC_LIGHT_VOLUMES_MOBILE");
            EditorUtility.DisplayDialog("完亁E, "グローバルVRC Light Volumesを無効にしました、E, "OK");
        }
    }
}
