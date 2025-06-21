using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

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
            EditorGUILayout.LabelField("lilToon PCSS Extension", titleStyle);
            
            // バージョン情報
            var versionStyle = new GUIStyle(EditorStyles.miniLabel);
            versionStyle.normal.textColor = Color.gray;
            EditorGUILayout.LabelField("Version 1.0.0 - Advanced Soft Shadow System", versionStyle);
            
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
                    quality = EditorGUILayout.IntPopup("品質", quality, qualityLabels, qualityValues);
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
    }
}
#endif 