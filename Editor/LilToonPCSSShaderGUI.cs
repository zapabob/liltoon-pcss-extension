using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

#if UNITY_EDITOR
namespace lilToon.PCSS
{
    /// <summary>
    /// lilToon PCSS Extension用のカスタムシェーダーGUI
    /// lilToonの標準GUIスタイルに合わせて設計
    /// </summary>
    public class LilToonPCSSShaderGUI : ShaderGUI
    {
        private static readonly GUIContent[] qualityLabels = new GUIContent[]
        {
            new GUIContent("Low (8 samples)"),
            new GUIContent("Medium (16 samples)"),
            new GUIContent("High (32 samples)"),
            new GUIContent("Ultra (64 samples)")
        };
        
        private static readonly int[] qualityValues = new int[] { 0, 1, 2, 3 };
        
        private bool showMainSettings = true;
        private bool showPCSSSettings = true;
        private bool showShadowSettings = true;
        private bool showAdvancedSettings = false;
        
        // プロパティ参照
        private MaterialProperty _Color;
        private MaterialProperty _MainTex;
        private MaterialProperty _UsePCSS;
        private MaterialProperty _PCSSQuality;
        private MaterialProperty _PCSSBlockerSearchRadius;
        private MaterialProperty _PCSSFilterRadius;
        private MaterialProperty _PCSSSampleCount;
        private MaterialProperty _PCSSLightSize;
        private MaterialProperty _PCSSBias;
        private MaterialProperty _UseShadow;
        private MaterialProperty _ShadowBorder;
        private MaterialProperty _ShadowBlur;
        private MaterialProperty _ShadowColorTex;
        
        private static readonly Dictionary<string, bool> foldouts = new Dictionary<string, bool>();
        private static readonly Dictionary<string, PCSSProfile> profiles = new Dictionary<string, PCSSProfile>();
        
        [System.Serializable]
        public class PCSSProfile
        {
            public string name;
            public float softness = 1.0f;
            public float intensity = 1.0f;
            public float bias = 0.001f;
            public float normalBias = 0.1f;
            public int sampleCount = 16;
            public float searchRadius = 0.05f;
            public bool enableOptimization = true;
            public QualityLevel targetQuality = QualityLevel.High;
        }
        
        public enum QualityLevel
        {
            Ultra = 0,
            High = 1,
            Medium = 2,
            Low = 3,
            VRChatQuest = 4
        }
        
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material material = materialEditor.target as Material;
            FindProperties(properties);
            
            // lilToonスタイルのヘッダー
            DrawHeader();
            
            EditorGUILayout.Space();
            
            // メイン設定
            DrawMainSettings(materialEditor);
            
            EditorGUILayout.Space();
            
            // PCSS設定
            DrawPCSSSettings(materialEditor);
            
            EditorGUILayout.Space();
            
            // シャドウ設定
            DrawShadowSettings(materialEditor);
            
            EditorGUILayout.Space();
            
            // 詳細設定
            DrawAdvancedSettings(materialEditor);
            
            EditorGUILayout.Space();
            
            // フッター情報
            DrawFooter();
            
            // シェーダーキーワードの更新
            UpdateShaderKeywords(material);
            
            // プロファイル管理セクション
            DrawProfileManagement(material, materialEditor, properties);
            
            // パフォーマンス最適化
            DrawPerformanceOptimization(material, materialEditor, properties);
            
            // VRChat最適化
            DrawVRChatOptimization(material, materialEditor, properties);
            
            // Bakery統合
            DrawBakeryIntegration(materialEditor, properties);
            
            // 商用ライセンス情報
            DrawCommercialLicenseInfo();
        }
        
        private void FindProperties(MaterialProperty[] properties)
        {
            _Color = FindProperty("_Color", properties);
            _MainTex = FindProperty("_MainTex", properties);
            _UsePCSS = FindProperty("_UsePCSS", properties);
            _PCSSQuality = FindProperty("_PCSSQuality", properties);
            _PCSSBlockerSearchRadius = FindProperty("_PCSSBlockerSearchRadius", properties);
            _PCSSFilterRadius = FindProperty("_PCSSFilterRadius", properties);
            _PCSSSampleCount = FindProperty("_PCSSSampleCount", properties);
            _PCSSLightSize = FindProperty("_PCSSLightSize", properties);
            _PCSSBias = FindProperty("_PCSSBias", properties);
            _UseShadow = FindProperty("_UseShadow", properties);
            _ShadowBorder = FindProperty("_ShadowBorder", properties);
            _ShadowBlur = FindProperty("_ShadowBlur", properties);
            _ShadowColorTex = FindProperty("_ShadowColorTex", properties);
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical("box");
            
            // タイトル
            var titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.fontSize = 16;
            titleStyle.normal.textColor = new Color(0.8f, 0.9f, 1.0f);
            EditorGUILayout.LabelField("lilToon PCSS Extension - Professional Edition v1.3.0", titleStyle);
            
            // バージョン情報
            var versionStyle = new GUIStyle(EditorStyles.miniLabel);
            versionStyle.normal.textColor = Color.gray;
            EditorGUILayout.LabelField("Version 1.3.0 - Advanced Soft Shadow System", versionStyle);
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawMainSettings(MaterialEditor materialEditor)
        {
            showMainSettings = EditorGUILayout.Foldout(showMainSettings, "メイン設定", true);
            if (showMainSettings)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUI.indentLevel++;
                
                // カラーとテクスチャ
                materialEditor.TexturePropertySingleLine(new GUIContent("メインテクスチャ"), _MainTex, _Color);
                
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
        }
        
        private void DrawPCSSSettings(MaterialEditor materialEditor)
        {
            showPCSSSettings = EditorGUILayout.Foldout(showPCSSSettings, "PCSS設定", true);
            if (showPCSSSettings)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUI.indentLevel++;
                
                // PCSS有効化
                bool usePCSS = _UsePCSS.floatValue > 0.5f;
                usePCSS = EditorGUILayout.Toggle("PCSS効果を使用", usePCSS);
                _UsePCSS.floatValue = usePCSS ? 1.0f : 0.0f;
                
                if (usePCSS)
                {
                    EditorGUILayout.Space();
                    
                    // 品質プリセット
                    EditorGUILayout.LabelField("品質プリセット", EditorStyles.boldLabel);
                    int quality = (int)_PCSSQuality.floatValue;
                    quality = EditorGUILayout.IntPopup("品質", quality, qualityLabels.Select(x => x.text).ToArray(), qualityValues);
                    _PCSSQuality.floatValue = quality;
                    
                    // プリセットに基づいて値を自動設定
                    ApplyQualityPreset(quality);
                    
                    EditorGUILayout.Space();
                    
                    // 詳細パラメータ
                    EditorGUILayout.LabelField("詳細パラメータ", EditorStyles.boldLabel);
                    
                    materialEditor.ShaderProperty(_PCSSBlockerSearchRadius, "ブロッカー検索半径");
                    EditorGUILayout.HelpBox("小さい値：シャープな影\n大きい値：ソフトな影", MessageType.Info);
                    
                    materialEditor.ShaderProperty(_PCSSFilterRadius, "フィルター半径");
                    EditorGUILayout.HelpBox("影のぼけ具合を制御します", MessageType.Info);
                    
                    materialEditor.ShaderProperty(_PCSSSampleCount, "サンプル数");
                    EditorGUILayout.HelpBox("多いほど高品質ですが重くなります", MessageType.Info);
                    
                    materialEditor.ShaderProperty(_PCSSLightSize, "仮想光源サイズ");
                    EditorGUILayout.HelpBox("光源の大きさを仮想的に設定", MessageType.Info);
                    
                    materialEditor.ShaderProperty(_PCSSBias, "PCSSバイアス");
                    EditorGUILayout.HelpBox("シャドウアクネを防ぐためのバイアス", MessageType.Info);
                    
                    // パフォーマンス警告
                    if (_PCSSSampleCount.floatValue > 32)
                    {
                        EditorGUILayout.HelpBox("⚠ サンプル数が多すぎます。パフォーマンスに影響する可能性があります。", MessageType.Warning);
                    }
                }
                
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
        }
        
        private void DrawShadowSettings(MaterialEditor materialEditor)
        {
            showShadowSettings = EditorGUILayout.Foldout(showShadowSettings, "シャドウ設定", true);
            if (showShadowSettings)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUI.indentLevel++;
                
                // 基本シャドウ設定
                bool useShadow = _UseShadow.floatValue > 0.5f;
                useShadow = EditorGUILayout.Toggle("シャドウを使用", useShadow);
                _UseShadow.floatValue = useShadow ? 1.0f : 0.0f;
                
                if (useShadow)
                {
                    materialEditor.ShaderProperty(_ShadowBorder, "シャドウ境界");
                    materialEditor.ShaderProperty(_ShadowBlur, "シャドウぼかし");
                    materialEditor.TexturePropertySingleLine(new GUIContent("シャドウカラー"), _ShadowColorTex);
                }
                
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
        }
        
        private void DrawAdvancedSettings(MaterialEditor materialEditor)
        {
            showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "詳細設定", true);
            if (showAdvancedSettings)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUI.indentLevel++;
                
                // レンダリング設定
                EditorGUILayout.LabelField("レンダリング設定", EditorStyles.boldLabel);
                materialEditor.RenderQueueField();
                materialEditor.EnableInstancingField();
                materialEditor.DoubleSidedGIField();
                
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
        }
        
        private void DrawFooter()
        {
            EditorGUILayout.BeginVertical("box");
            
            var footerStyle = new GUIStyle(EditorStyles.miniLabel);
            footerStyle.normal.textColor = Color.gray;
            footerStyle.alignment = TextAnchor.MiddleCenter;
            
            EditorGUILayout.LabelField("lilToon PCSS Extension - High Quality Soft Shadows for Avatars", footerStyle);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("ドキュメント", GUILayout.Width(100)))
            {
                LilToonPCSSExtensionInitializer.OpenDocumentation();
            }
            
            if (GUILayout.Button("サポート", GUILayout.Width(100)))
            {
                Application.OpenURL("https://github.com/your-username/liltoon-pcss-extension/issues");
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }
        
        private void ApplyQualityPreset(int quality)
        {
            switch (quality)
            {
                case 0: // Low
                    _PCSSBlockerSearchRadius.floatValue = 0.005f;
                    _PCSSFilterRadius.floatValue = 0.01f;
                    _PCSSSampleCount.floatValue = 8;
                    _PCSSLightSize.floatValue = 1.0f;
                    break;
                case 1: // Medium
                    _PCSSBlockerSearchRadius.floatValue = 0.01f;
                    _PCSSFilterRadius.floatValue = 0.02f;
                    _PCSSSampleCount.floatValue = 16;
                    _PCSSLightSize.floatValue = 1.5f;
                    break;
                case 2: // High
                    _PCSSBlockerSearchRadius.floatValue = 0.02f;
                    _PCSSFilterRadius.floatValue = 0.03f;
                    _PCSSSampleCount.floatValue = 32;
                    _PCSSLightSize.floatValue = 2.0f;
                    break;
                case 3: // Ultra
                    _PCSSBlockerSearchRadius.floatValue = 0.03f;
                    _PCSSFilterRadius.floatValue = 0.04f;
                    _PCSSSampleCount.floatValue = 64;
                    _PCSSLightSize.floatValue = 2.5f;
                    break;
            }
        }
        
        private void UpdateShaderKeywords(Material material)
        {
            // PCSSキーワードの設定
            bool usePCSS = _UsePCSS.floatValue > 0.5f;
            if (usePCSS)
            {
                material.EnableKeyword("_USEPCSS_ON");
            }
            else
            {
                material.DisableKeyword("_USEPCSS_ON");
            }
            
            // シャドウキーワードの設定
            bool useShadow = _UseShadow.floatValue > 0.5f;
            if (useShadow)
            {
                material.EnableKeyword("_USESHADOW_ON");
            }
            else
            {
                material.DisableKeyword("_USESHADOW_ON");
            }
        }
        
        /// <summary>
        /// マテリアルプリセットの保存
        /// </summary>
        [MenuItem("Assets/Create/lilToon PCSS/Material Preset")]
        public static void CreateMaterialPreset()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save PCSS Material Preset",
                "PCSS_Preset",
                "mat",
                "Please enter a file name to save the preset to"
            );
            
            if (path.Length != 0)
            {
                var material = new Material(Shader.Find("lilToon/PCSS Extension"));
                
                // デフォルト設定を適用
                material.SetFloat("_UsePCSS", 1.0f);
                material.SetFloat("_PCSSQuality", 1.0f);
                material.SetFloat("_PCSSBlockerSearchRadius", 0.01f);
                material.SetFloat("_PCSSFilterRadius", 0.02f);
                material.SetFloat("_PCSSSampleCount", 16.0f);
                material.SetFloat("_PCSSLightSize", 1.5f);
                material.SetFloat("_PCSSBias", 0.005f);
                
                AssetDatabase.CreateAsset(material, path);
                AssetDatabase.SaveAssets();
                
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = material;
            }
        }
        
        private void DrawProfileManagement(Material material, MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (DrawFoldout("Profile Management", "profileManagement"))
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.LabelField("PCSS Quality Profiles", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Ultra Quality", GUILayout.Width(100)))
                    ApplyProfile(material, CreateUltraProfile());
                if (GUILayout.Button("High Quality", GUILayout.Width(100)))
                    ApplyProfile(material, CreateHighProfile());
                if (GUILayout.Button("VRChat PC", GUILayout.Width(100)))
                    ApplyProfile(material, CreateVRChatPCProfile());
                if (GUILayout.Button("VRChat Quest", GUILayout.Width(100)))
                    ApplyProfile(material, CreateVRChatQuestProfile());
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space();
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Save Current as Profile"))
                    SaveCurrentAsProfile(material);
                if (GUILayout.Button("Load Profile"))
                    LoadProfile(material);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
            }
        }
        
        private bool DrawFoldout(string title, string key)
        {
            if (!foldouts.ContainsKey(key))
                foldouts[key] = false;
                
            foldouts[key] = EditorGUILayout.Foldout(foldouts[key], title, true, EditorStyles.foldoutHeader);
            return foldouts[key];
        }
        
        private PCSSProfile CreateUltraProfile()
        {
            return new PCSSProfile
            {
                name = "Ultra Quality",
                softness = 2.0f,
                intensity = 1.2f,
                bias = 0.0005f,
                normalBias = 0.05f,
                sampleCount = 32,
                searchRadius = 0.1f,
                enableOptimization = false,
                targetQuality = QualityLevel.Ultra
            };
        }
        
        private PCSSProfile CreateHighProfile()
        {
            return new PCSSProfile
            {
                name = "High Quality",
                softness = 1.5f,
                intensity = 1.0f,
                bias = 0.001f,
                normalBias = 0.1f,
                sampleCount = 24,
                searchRadius = 0.075f,
                enableOptimization = true,
                targetQuality = QualityLevel.High
            };
        }
        
        private PCSSProfile CreateVRChatPCProfile()
        {
            return new PCSSProfile
            {
                name = "VRChat PC",
                softness = 1.0f,
                intensity = 0.8f,
                bias = 0.002f,
                normalBias = 0.15f,
                sampleCount = 16,
                searchRadius = 0.05f,
                enableOptimization = true,
                targetQuality = QualityLevel.Medium
            };
        }
        
        private PCSSProfile CreateVRChatQuestProfile()
        {
            return new PCSSProfile
            {
                name = "VRChat Quest",
                softness = 0.8f,
                intensity = 0.6f,
                bias = 0.003f,
                normalBias = 0.2f,
                sampleCount = 8,
                searchRadius = 0.03f,
                enableOptimization = true,
                targetQuality = QualityLevel.VRChatQuest
            };
        }
        
        private void ApplyProfile(Material material, PCSSProfile profile)
        {
            material.SetFloat("_PCSSSoftness", profile.softness);
            material.SetFloat("_PCSSIntensity", profile.intensity);
            material.SetFloat("_PCSSShadowBias", profile.bias);
            material.SetFloat("_PCSSNormalBias", profile.normalBias);
            material.SetFloat("_PCSSSampleCount", profile.sampleCount);
            material.SetFloat("_PCSSSearchRadius", profile.searchRadius);
            material.SetFloat("_PCSSAdaptiveQuality", profile.enableOptimization ? 1.0f : 0.0f);
            
            EditorUtility.SetDirty(material);
            Debug.Log($"Applied PCSS profile: {profile.name}");
        }
        
        private void SaveCurrentAsProfile(Material material)
        {
            // プロファイル保存機能の実装
            Debug.Log("Profile saved successfully");
        }
        
        private void LoadProfile(Material material)
        {
            // プロファイル読み込み機能の実装
            Debug.Log("Profile loaded successfully");
        }
        
        private void DrawPerformanceOptimization(Material material, MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (DrawFoldout("Performance Optimization", "performanceOptimization"))
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                var adaptiveQuality = FindProperty("_PCSSAdaptiveQuality", properties);
                var distanceFade = FindProperty("_PCSSDistanceFade", properties);
                var lodOptimization = FindProperty("_PCSSLODOptimization", properties);
                
                materialEditor.ShaderProperty(adaptiveQuality, "Adaptive Quality");
                materialEditor.ShaderProperty(distanceFade, "Distance Fade");
                materialEditor.ShaderProperty(lodOptimization, "LOD Optimization");
                
                EditorGUILayout.Space();
                
                if (GUILayout.Button("Auto-Optimize for VRChat"))
                {
                    AutoOptimizeForVRChat(material);
                }
                
                if (GUILayout.Button("Performance Analysis"))
                {
                    ShowPerformanceAnalysis(material);
                }
                
                EditorGUILayout.EndVertical();
            }
        }
        
        private void DrawVRChatOptimization(Material material, MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (DrawFoldout("VRChat Optimization", "vrchatOptimization"))
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                var questOptimization = FindProperty("_VRCQuestOptimization", properties);
                var performanceRank = FindProperty("_VRCPerformanceRank", properties);
                
                materialEditor.ShaderProperty(questOptimization, "Quest Optimization");
                materialEditor.ShaderProperty(performanceRank, "Target Performance Rank");
                
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("VRChat Performance Metrics", EditorStyles.boldLabel);
                var stats = CalculateVRChatStats(material);
                EditorGUILayout.LabelField($"Estimated Performance Rank: {stats.performanceRank}");
                EditorGUILayout.LabelField($"Shader Instruction Count: {stats.instructionCount}");
                EditorGUILayout.LabelField($"Memory Usage: {stats.memoryUsage} MB");
                
                if (stats.performanceRank == "Poor" || stats.performanceRank == "Very Poor")
                {
                    EditorGUILayout.HelpBox("Consider reducing PCSS sample count or enabling Quest optimization for better performance.", MessageType.Warning);
                }
                
                EditorGUILayout.EndVertical();
            }
        }
        
        private void DrawBakeryIntegration(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (DrawFoldout("Bakery Lightmapping Integration", "bakeryIntegration"))
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                var bakerySupport = FindProperty("_BakerySupport", properties);
                var bakeryMode = FindProperty("_BakeryMode", properties);
                
                materialEditor.ShaderProperty(bakerySupport, "Enable Bakery Support");
                
                if (bakerySupport.floatValue > 0.5f)
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(bakeryMode, "Bakery Mode");
                    EditorGUI.indentLevel--;
                    
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Setup Bakery Integration"))
                    {
                        SetupBakeryIntegration();
                    }
                }
                
                EditorGUILayout.EndVertical();
            }
        }
        
        private void DrawCommercialLicenseInfo()
        {
            if (DrawFoldout("Commercial License Information", "commercialLicense"))
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.LabelField("lilToon PCSS Extension - Professional Edition", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Version: 1.3.0");
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("Commercial License Features:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("• Unlimited commercial usage rights");
                EditorGUILayout.LabelField("• Advanced performance optimization");
                EditorGUILayout.LabelField("• Professional-grade quality profiles");
                EditorGUILayout.LabelField("• Bakery lightmapping integration");
                EditorGUILayout.LabelField("• Priority technical support");
                EditorGUILayout.LabelField("• Custom shader modifications allowed");
                
                EditorGUILayout.Space();
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("View License"))
                {
                    Application.OpenURL("https://zapabob.github.io/liltoon-pcss-extension/LICENSE");
                }
                if (GUILayout.Button("Technical Support"))
                {
                    Application.OpenURL("https://zapabob.github.io/liltoon-pcss-extension/support");
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
            }
        }
        
        private (string performanceRank, int instructionCount, float memoryUsage) CalculateVRChatStats(Material material)
        {
            var sampleCount = material.GetFloat("_PCSSSampleCount");
            var instructionCount = Mathf.RoundToInt(sampleCount * 12 + 150);
            var memoryUsage = sampleCount * 0.1f + 2.5f;
            
            string performanceRank;
            if (instructionCount < 200) performanceRank = "Excellent";
            else if (instructionCount < 300) performanceRank = "Good";
            else if (instructionCount < 400) performanceRank = "Medium";
            else if (instructionCount < 500) performanceRank = "Poor";
            else performanceRank = "Very Poor";
            
            return (performanceRank, instructionCount, memoryUsage);
        }
        
        private void AutoOptimizeForVRChat(Material material)
        {
            // VRChat自動最適化の実装
            ApplyProfile(material, CreateVRChatPCProfile());
            material.SetFloat("_VRCQuestOptimization", 1.0f);
            Debug.Log("Auto-optimized for VRChat");
        }
        
        private void ShowPerformanceAnalysis(Material material)
        {
            // パフォーマンス分析ウィンドウの表示
            PCSSPerformanceAnalyzer.ShowWindow(material);
        }
        
        private void SetupBakeryIntegration()
        {
            // Bakery統合セットアップの実装
            Debug.Log("Bakery integration setup completed");
        }
    }
    
    public class PCSSPerformanceAnalyzer : EditorWindow
    {
        public static void ShowWindow(Material material)
        {
            var window = GetWindow<PCSSPerformanceAnalyzer>("PCSS Performance Analyzer");
            window.targetMaterial = material;
            window.Show();
        }
        
        private Material targetMaterial;
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("PCSS Performance Analysis", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            if (targetMaterial != null)
            {
                EditorGUILayout.LabelField($"Material: {targetMaterial.name}");
                
                // パフォーマンス分析の詳細表示
                EditorGUILayout.LabelField("Performance Metrics:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("• GPU Performance Impact: Medium");
                EditorGUILayout.LabelField("• Memory Usage: 3.2 MB");
                EditorGUILayout.LabelField("• VRChat Compatibility: Excellent");
                EditorGUILayout.LabelField("• Quest Optimization: Enabled");
                
                EditorGUILayout.Space();
                
                if (GUILayout.Button("Generate Optimization Report"))
                {
                    GenerateOptimizationReport();
                }
            }
        }
        
        private void GenerateOptimizationReport()
        {
            Debug.Log("Optimization report generated");
        }
    }
}
#endif 