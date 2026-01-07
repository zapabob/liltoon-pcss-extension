using UnityEditor;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// lilToon PCSS ExtensionシェーダーのGUIエディター
    /// リアル影作成機能統合版
    /// </summary>
    public class LilToonPCSSShaderGUI : ShaderGUI
    {
        private MaterialProperty mainTex;
        private MaterialProperty color;
        private MaterialProperty cutoff;
        
        // 影システム
        private MaterialProperty useShadow;
        private MaterialProperty shadowColorTex;
        private MaterialProperty shadowBorder;
        private MaterialProperty shadowBlur;
        private MaterialProperty useShadow2;
        private MaterialProperty shadow2ColorTex;
        private MaterialProperty shadow2Border;
        private MaterialProperty shadow2Blur;
        private MaterialProperty useShadow3;
        private MaterialProperty shadow3ColorTex;
        private MaterialProperty shadow3Border;
        private MaterialProperty shadow3Blur;
        
        // SDF Face Shadow
        private MaterialProperty useSDFFaceShadow;
        private MaterialProperty sdfFaceShadowTex;
        private MaterialProperty sdfFaceShadowIntensity;
        private MaterialProperty sdfFaceShadowSoftness;
        
        // PCSS
        private MaterialProperty usePCSS;
        private MaterialProperty pcssPresetMode;
        private MaterialProperty localPCSSFilterRadius;
        private MaterialProperty localPCSSLightSize;
        private MaterialProperty localPCSSBias;
        private MaterialProperty pcssIntensity;
        private MaterialProperty pcssQualityLevel;
        private MaterialProperty localPCSSSamples;
        private MaterialProperty useShadowMask;
        private MaterialProperty shadowMaskTex;
        private MaterialProperty shadowMaskStrength;
        
        // PCSS最適化
        private MaterialProperty usePCSSOptimization;
        private MaterialProperty pcssOptimizationLevel;
        private MaterialProperty usePCSSMobileOptimization;
        
        // VRC Light Volumes
        private MaterialProperty useVRCLightVolumes;
        private MaterialProperty vrcLightVolumeIntensity;
        private MaterialProperty vrcLightVolumeTint;
        private MaterialProperty vrcLightVolumeDistanceFactor;
        private MaterialProperty useVRCLVOptimization;
        private MaterialProperty vrcLVOptimizationLevel;
        
        // ファー機能
        private MaterialProperty useFur;
        private MaterialProperty furTex;
        private MaterialProperty furLength;
        private MaterialProperty furDensity;
        private MaterialProperty furSubdivision;
        private MaterialProperty furGravity;
        private MaterialProperty furWind;
        private MaterialProperty furWindSpeed;
        private MaterialProperty furAO;
        private MaterialProperty furShadow;
        private MaterialProperty useFurOptimization;
        private MaterialProperty furOptimizationLevel;
        
        // リアル影作成機能 (統合)
        private MaterialProperty useRealisticShadow;
        private MaterialProperty realisticShadowColor;
        private MaterialProperty realisticShadowIntensity;
        private MaterialProperty realisticShadowSoftness;
        private MaterialProperty realisticShadowOffset;
        private MaterialProperty realisticShadowScale;
        
        // RimShade機能 (統合)
        private MaterialProperty useRimShade;
        private MaterialProperty rimShadeColor;
        private MaterialProperty rimShadeIntensity;
        private MaterialProperty rimShadeWidth;
        
        // レンダリング設定
        private MaterialProperty cull;
        private MaterialProperty zWrite;
        private MaterialProperty zTest;
        private MaterialProperty srcBlend;
        private MaterialProperty dstBlend;
        
        // ステンシル設定
        private MaterialProperty stencilRef;
        private MaterialProperty stencilReadMask;
        private MaterialProperty stencilWriteMask;
        private MaterialProperty stencilComp;
        private MaterialProperty stencilPass;
        private MaterialProperty stencilFail;
        private MaterialProperty stencilZFail;
        
        // フォールドアウト状態
        private bool showMainSettings = true;
        private bool showShadowSettings = true;
        private bool showPCSSSettings = true;
        private bool showVRCLightVolumesSettings = false;
        private bool showFurSettings = false;
        private bool showRealisticShadowSettings = true;
        private bool showRenderingSettings = false;
        
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material material = materialEditor.target as Material;
            
            // プロパティの取得
            FindProperties(properties);
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("lilToon PCSS Extension + Realistic Shadow", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            EditorGUILayout.HelpBox("VRChatリアル影作成機能統合版: PCSS + Fake Shadow + RimShade", MessageType.Info);
            
            EditorGUILayout.Space(10);
            
            // メイン設定
            showMainSettings = EditorGUILayout.Foldout(showMainSettings, "Main Settings", true);
            if (showMainSettings)
            {
                EditorGUI.indentLevel++;
                
                if (mainTex != null) materialEditor.TexturePropertySingleLine(new GUIContent("Main Texture"), mainTex);
                if (color != null) materialEditor.ShaderProperty(color, "Color");
                if (cutoff != null) materialEditor.ShaderProperty(cutoff, "Alpha Cutoff");
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space(10);
            
            // リアル影作成設定 (統合)
            showRealisticShadowSettings = EditorGUILayout.Foldout(showRealisticShadowSettings, "Realistic Shadow Settings", true);
            if (showRealisticShadowSettings)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.HelpBox("リアル影作成機能: Fake Shadow + RimShade統合", MessageType.Info);
                
                // Fake Shadow設定
                EditorGUILayout.LabelField("Fake Shadow", EditorStyles.boldLabel);
                if (useRealisticShadow != null) materialEditor.ShaderProperty(useRealisticShadow, "Use Realistic Shadow");
                if (useRealisticShadow != null && useRealisticShadow.floatValue > 0.5f)
                {
                    if (realisticShadowColor != null) materialEditor.ShaderProperty(realisticShadowColor, "Realistic Shadow Color");
                    if (realisticShadowIntensity != null) materialEditor.ShaderProperty(realisticShadowIntensity, "Realistic Shadow Intensity");
                    if (realisticShadowSoftness != null) materialEditor.ShaderProperty(realisticShadowSoftness, "Realistic Shadow Softness");
                    if (realisticShadowOffset != null) materialEditor.ShaderProperty(realisticShadowOffset, "Realistic Shadow Offset");
                    if (realisticShadowScale != null) materialEditor.ShaderProperty(realisticShadowScale, "Realistic Shadow Scale");
                }
                
                EditorGUILayout.Space(5);
                
                // RimShade設定
                EditorGUILayout.LabelField("RimShade", EditorStyles.boldLabel);
                if (useRimShade != null) materialEditor.ShaderProperty(useRimShade, "Use RimShade");
                if (useRimShade != null && useRimShade.floatValue > 0.5f)
                {
                    if (rimShadeColor != null) materialEditor.ShaderProperty(rimShadeColor, "RimShade Color");
                    if (rimShadeIntensity != null) materialEditor.ShaderProperty(rimShadeIntensity, "RimShade Intensity");
                    if (rimShadeWidth != null) materialEditor.ShaderProperty(rimShadeWidth, "RimShade Width");
                }
                
                EditorGUILayout.Space(10);
                
                // プリセットボタン
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Apply Hair Shadow Preset"))
                {
                    ApplyHairShadowPreset(material);
                }
                
                if (GUILayout.Button("Apply Face Shadow Preset"))
                {
                    ApplyFaceShadowPreset(material);
                }
                
                EditorGUILayout.EndHorizontal();
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space(10);
            
            // 影設定
            showShadowSettings = EditorGUILayout.Foldout(showShadowSettings, "Shadow Settings", true);
            if (showShadowSettings)
            {
                EditorGUI.indentLevel++;
                
                // 3影システム
                EditorGUILayout.LabelField("3-Shadow System", EditorStyles.boldLabel);
                if (useShadow != null) materialEditor.ShaderProperty(useShadow, "Use Shadow");
                if (useShadow != null && useShadow.floatValue > 0.5f)
                {
                    if (shadowColorTex != null) materialEditor.TexturePropertySingleLine(new GUIContent("Shadow Color"), shadowColorTex);
                    if (shadowBorder != null) materialEditor.ShaderProperty(shadowBorder, "Shadow Border");
                    if (shadowBlur != null) materialEditor.ShaderProperty(shadowBlur, "Shadow Blur");
                }
                
                if (useShadow2 != null) materialEditor.ShaderProperty(useShadow2, "Use 2nd Shadow");
                if (useShadow2 != null && useShadow2.floatValue > 0.5f)
                {
                    if (shadow2ColorTex != null) materialEditor.TexturePropertySingleLine(new GUIContent("2nd Shadow Color"), shadow2ColorTex);
                    if (shadow2Border != null) materialEditor.ShaderProperty(shadow2Border, "2nd Shadow Border");
                    if (shadow2Blur != null) materialEditor.ShaderProperty(shadow2Blur, "2nd Shadow Blur");
                }
                
                if (useShadow3 != null) materialEditor.ShaderProperty(useShadow3, "Use 3rd Shadow");
                if (useShadow3 != null && useShadow3.floatValue > 0.5f)
                {
                    if (shadow3ColorTex != null) materialEditor.TexturePropertySingleLine(new GUIContent("3rd Shadow Color"), shadow3ColorTex);
                    if (shadow3Border != null) materialEditor.ShaderProperty(shadow3Border, "3rd Shadow Border");
                    if (shadow3Blur != null) materialEditor.ShaderProperty(shadow3Blur, "3rd Shadow Blur");
                }
                
                // SDF Face Shadow
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("SDF Face Shadow", EditorStyles.boldLabel);
                if (useSDFFaceShadow != null) materialEditor.ShaderProperty(useSDFFaceShadow, "Use SDF Face Shadow");
                if (useSDFFaceShadow != null && useSDFFaceShadow.floatValue > 0.5f)
                {
                    if (sdfFaceShadowTex != null) materialEditor.TexturePropertySingleLine(new GUIContent("SDF Face Shadow Texture"), sdfFaceShadowTex);
                    if (sdfFaceShadowIntensity != null) materialEditor.ShaderProperty(sdfFaceShadowIntensity, "SDF Face Shadow Intensity");
                    if (sdfFaceShadowSoftness != null) materialEditor.ShaderProperty(sdfFaceShadowSoftness, "SDF Face Shadow Softness");
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space(10);
            
            // PCSS設定
            showPCSSSettings = EditorGUILayout.Foldout(showPCSSSettings, "PCSS Settings", true);
            if (showPCSSSettings)
            {
                EditorGUI.indentLevel++;
                
                if (usePCSS != null) materialEditor.ShaderProperty(usePCSS, "Use PCSS");
                if (usePCSS != null && usePCSS.floatValue > 0.5f)
                {
                    if (pcssPresetMode != null) materialEditor.ShaderProperty(pcssPresetMode, "PCSS Preset");
                    if (localPCSSFilterRadius != null) materialEditor.ShaderProperty(localPCSSFilterRadius, "PCSS Filter Radius");
                    if (localPCSSLightSize != null) materialEditor.ShaderProperty(localPCSSLightSize, "PCSS Light Size");
                    if (localPCSSBias != null) materialEditor.ShaderProperty(localPCSSBias, "PCSS Bias");
                    if (pcssIntensity != null) materialEditor.ShaderProperty(pcssIntensity, "PCSS Intensity");
                    if (pcssQualityLevel != null) materialEditor.ShaderProperty(pcssQualityLevel, "PCSS Quality");
                    if (localPCSSSamples != null) materialEditor.ShaderProperty(localPCSSSamples, "PCSS Samples");
                    
                    // PCSS最適化
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("PCSS Optimization", EditorStyles.boldLabel);
                    if (usePCSSOptimization != null) materialEditor.ShaderProperty(usePCSSOptimization, "Use PCSS Optimization");
                    if (usePCSSOptimization != null && usePCSSOptimization.floatValue > 0.5f)
                    {
                        if (pcssOptimizationLevel != null) materialEditor.ShaderProperty(pcssOptimizationLevel, "PCSS Optimization Level");
                    }
                    if (usePCSSMobileOptimization != null) materialEditor.ShaderProperty(usePCSSMobileOptimization, "Use PCSS Mobile Optimization");
                }
                
                // Shadow Mask
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Shadow Mask", EditorStyles.boldLabel);
                if (useShadowMask != null) materialEditor.ShaderProperty(useShadowMask, "Use Shadow Mask");
                if (useShadowMask != null && useShadowMask.floatValue > 0.5f)
                {
                    if (shadowMaskTex != null) materialEditor.TexturePropertySingleLine(new GUIContent("Shadow Mask"), shadowMaskTex);
                    if (shadowMaskStrength != null) materialEditor.ShaderProperty(shadowMaskStrength, "Shadow Mask Strength");
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space(10);
            
            // VRC Light Volumes設定
            showVRCLightVolumesSettings = EditorGUILayout.Foldout(showVRCLightVolumesSettings, "VRC Light Volumes Settings", true);
            if (showVRCLightVolumesSettings)
            {
                EditorGUI.indentLevel++;
                
                if (useVRCLightVolumes != null) materialEditor.ShaderProperty(useVRCLightVolumes, "Use VRC Light Volumes");
                if (useVRCLightVolumes != null && useVRCLightVolumes.floatValue > 0.5f)
                {
                    if (vrcLightVolumeIntensity != null) materialEditor.ShaderProperty(vrcLightVolumeIntensity, "VRC Light Volume Intensity");
                    if (vrcLightVolumeTint != null) materialEditor.ShaderProperty(vrcLightVolumeTint, "VRC Light Volume Tint");
                    if (vrcLightVolumeDistanceFactor != null) materialEditor.ShaderProperty(vrcLightVolumeDistanceFactor, "VRC Light Volume Distance Factor");
                    
                    // VRC Light Volumes最適化
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("VRC Light Volumes Optimization", EditorStyles.boldLabel);
                    if (useVRCLVOptimization != null) materialEditor.ShaderProperty(useVRCLVOptimization, "Use VRC LV Optimization");
                    if (useVRCLVOptimization != null && useVRCLVOptimization.floatValue > 0.5f)
                    {
                        if (vrcLVOptimizationLevel != null) materialEditor.ShaderProperty(vrcLVOptimizationLevel, "VRC LV Optimization Level");
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space(10);
            
            // ファー設定
            showFurSettings = EditorGUILayout.Foldout(showFurSettings, "Fur Settings", true);
            if (showFurSettings)
            {
                EditorGUI.indentLevel++;
                
                if (useFur != null) materialEditor.ShaderProperty(useFur, "Use Fur");
                if (useFur != null && useFur.floatValue > 0.5f)
                {
                    if (furTex != null) materialEditor.TexturePropertySingleLine(new GUIContent("Fur Texture"), furTex);
                    if (furLength != null) materialEditor.ShaderProperty(furLength, "Fur Length");
                    if (furDensity != null) materialEditor.ShaderProperty(furDensity, "Fur Density");
                    if (furSubdivision != null) materialEditor.ShaderProperty(furSubdivision, "Fur Subdivision");
                    if (furGravity != null) materialEditor.ShaderProperty(furGravity, "Fur Gravity");
                    if (furWind != null) materialEditor.ShaderProperty(furWind, "Fur Wind");
                    if (furWindSpeed != null) materialEditor.ShaderProperty(furWindSpeed, "Fur Wind Speed");
                    if (furAO != null) materialEditor.ShaderProperty(furAO, "Fur Ambient Occlusion");
                    if (furShadow != null) materialEditor.ShaderProperty(furShadow, "Fur Shadow");
                    
                    // ファー最適化
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Fur Optimization", EditorStyles.boldLabel);
                    if (useFurOptimization != null) materialEditor.ShaderProperty(useFurOptimization, "Use Fur Optimization");
                    if (useFurOptimization != null && useFurOptimization.floatValue > 0.5f)
                    {
                        if (furOptimizationLevel != null) materialEditor.ShaderProperty(furOptimizationLevel, "Fur Optimization Level");
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space(10);
            
            // レンダリング設定
            showRenderingSettings = EditorGUILayout.Foldout(showRenderingSettings, "Rendering Settings", true);
            if (showRenderingSettings)
            {
                EditorGUI.indentLevel++;
                
                if (cull != null) materialEditor.ShaderProperty(cull, "Cull Mode");
                if (zWrite != null) materialEditor.ShaderProperty(zWrite, "ZWrite");
                if (zTest != null) materialEditor.ShaderProperty(zTest, "ZTest");
                if (srcBlend != null) materialEditor.ShaderProperty(srcBlend, "Src Blend");
                if (dstBlend != null) materialEditor.ShaderProperty(dstBlend, "Dst Blend");
                
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Stencil Settings", EditorStyles.boldLabel);
                if (stencilRef != null) materialEditor.ShaderProperty(stencilRef, "Stencil Reference");
                if (stencilReadMask != null) materialEditor.ShaderProperty(stencilReadMask, "Stencil Read Mask");
                if (stencilWriteMask != null) materialEditor.ShaderProperty(stencilWriteMask, "Stencil Write Mask");
                if (stencilComp != null) materialEditor.ShaderProperty(stencilComp, "Stencil Compare");
                if (stencilPass != null) materialEditor.ShaderProperty(stencilPass, "Stencil Pass");
                if (stencilFail != null) materialEditor.ShaderProperty(stencilFail, "Stencil Fail");
                if (stencilZFail != null) materialEditor.ShaderProperty(stencilZFail, "Stencil ZFail");
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space(20);
            
            // ヘルプ情報
            EditorGUILayout.HelpBox("統合機能:\n" +
                "• PCSS: 高品質な影表現\n" +
                "• Fake Shadow: 前髪など特定部位に疑似的な影を追加\n" +
                "• RimShade: モデルの輪郭に影を付けて立体感を演出\n" +
                "• VRC Light Volumes: よりリアルなライティング表現\n" +
                "• ファー機能: 自然な毛皮表現", MessageType.Info);
        }
        
        private void FindProperties(MaterialProperty[] properties)
        {
            // lilToon 2.2.1対応: メインテクスチャとカラー
            mainTex = FindProperty("_MainTex", properties, false) ?? FindProperty("_BaseMap", properties, false);
            color = FindProperty("_Color", properties, false) ?? FindProperty("_BaseColor", properties, false);
            cutoff = FindProperty("_Cutoff", properties, false) ?? FindProperty("_AlphaClip", properties, false);
            
            // lilToon 2.2.1対応: 影システム（新しいプロパティ名）
            useShadow = FindProperty("_UseShadow", properties, false) ?? FindProperty("_ShadowReceive", properties, false);
            shadowColorTex = FindProperty("_ShadowColorTex", properties, false) ?? FindProperty("_ShadowColor", properties, false);
            shadowBorder = FindProperty("_ShadowBorder", properties, false) ?? FindProperty("_ShadowBorderRange", properties, false);
            shadowBlur = FindProperty("_ShadowBlur", properties, false) ?? FindProperty("_ShadowBlurMask", properties, false);
            useShadow2 = FindProperty("_UseShadow2", properties, false) ?? FindProperty("_Shadow2ndReceive", properties, false);
            shadow2ColorTex = FindProperty("_Shadow2ColorTex", properties, false) ?? FindProperty("_Shadow2ndColor", properties, false);
            shadow2Border = FindProperty("_Shadow2Border", properties, false) ?? FindProperty("_Shadow2ndBorderRange", properties, false);
            shadow2Blur = FindProperty("_Shadow2Blur", properties, false) ?? FindProperty("_Shadow2ndBlurMask", properties, false);
            useShadow3 = FindProperty("_UseShadow3", properties, false) ?? FindProperty("_Shadow3rdReceive", properties, false);
            shadow3ColorTex = FindProperty("_Shadow3ColorTex", properties, false) ?? FindProperty("_Shadow3rdColor", properties, false);
            shadow3Border = FindProperty("_Shadow3Border", properties, false) ?? FindProperty("_Shadow3rdBorderRange", properties, false);
            shadow3Blur = FindProperty("_Shadow3Blur", properties, false) ?? FindProperty("_Shadow3rdBlurMask", properties, false);
            
            // SDF Face Shadow
            useSDFFaceShadow = FindProperty("_UseSDFFaceShadow", properties, false);
            sdfFaceShadowTex = FindProperty("_SDFFaceShadowTex", properties, false);
            sdfFaceShadowIntensity = FindProperty("_SDFFaceShadowIntensity", properties, false);
            sdfFaceShadowSoftness = FindProperty("_SDFFaceShadowSoftness", properties, false);
            
            // PCSS
            usePCSS = FindProperty("_UsePCSS", properties, false);
            pcssPresetMode = FindProperty("_PCSSPresetMode", properties, false);
            localPCSSFilterRadius = FindProperty("_LocalPCSSFilterRadius", properties, false);
            localPCSSLightSize = FindProperty("_LocalPCSSLightSize", properties, false);
            localPCSSBias = FindProperty("_LocalPCSSBias", properties, false);
            pcssIntensity = FindProperty("_PCSSIntensity", properties, false);
            pcssQualityLevel = FindProperty("_PCSSQualityLevel", properties, false);
            localPCSSSamples = FindProperty("_LocalPCSSSamples", properties, false);
            useShadowMask = FindProperty("_UseShadowMask", properties, false);
            shadowMaskTex = FindProperty("_ShadowMaskTex", properties, false);
            shadowMaskStrength = FindProperty("_ShadowMaskStrength", properties, false);
            
            // PCSS最適化
            usePCSSOptimization = FindProperty("_UsePCSSOptimization", properties, false);
            pcssOptimizationLevel = FindProperty("_PCSSOptimizationLevel", properties, false);
            usePCSSMobileOptimization = FindProperty("_UsePCSSMobileOptimization", properties, false);
            
            // VRLightVolumes 2.0.1対応: 新しいプロパティ名
            useVRCLightVolumes = FindProperty("_UseVRCLightVolumes", properties, false) ?? FindProperty("_VRCLightVolumesEnabled", properties, false);
            vrcLightVolumeIntensity = FindProperty("_VRCLightVolumeIntensity", properties, false) ?? FindProperty("_VRCLVIntensity", properties, false);
            vrcLightVolumeTint = FindProperty("_VRCLightVolumeTint", properties, false) ?? FindProperty("_VRCLVTintColor", properties, false);
            vrcLightVolumeDistanceFactor = FindProperty("_VRCLightVolumeDistanceFactor", properties, false) ?? FindProperty("_VRCLVDistanceAttenuation", properties, false);
            useVRCLVOptimization = FindProperty("_UseVRCLVOptimization", properties, false) ?? FindProperty("_VRCLVOptimizationEnabled", properties, false);
            vrcLVOptimizationLevel = FindProperty("_VRCLVOptimizationLevel", properties, false) ?? FindProperty("_VRCLVOptimizationMode", properties, false);
            
            // lilToon 2.2.1対応: Fur機能の新しいプロパティ名
            useFur = FindProperty("_UseFur", properties, false) ?? FindProperty("_FurEnabled", properties, false);
            furTex = FindProperty("_FurTex", properties, false) ?? FindProperty("_FurNoiseMask", properties, false);
            furLength = FindProperty("_FurLength", properties, false) ?? FindProperty("_FurLengthMask", properties, false);
            furDensity = FindProperty("_FurDensity", properties, false) ?? FindProperty("_FurRandomize", properties, false);
            furSubdivision = FindProperty("_FurSubdivision", properties, false) ?? FindProperty("_FurLayerNum", properties, false);
            furGravity = FindProperty("_FurGravity", properties, false) ?? FindProperty("_FurGravityMask", properties, false);
            furWind = FindProperty("_FurWind", properties, false) ?? FindProperty("_FurWindMask", properties, false);
            furWindSpeed = FindProperty("_FurWindSpeed", properties, false) ?? FindProperty("_FurWindFreq", properties, false);
            furAO = FindProperty("_FurAO", properties, false) ?? FindProperty("_FurAOMask", properties, false);
            furShadow = FindProperty("_FurShadow", properties, false) ?? FindProperty("_FurMesh", properties, false);
            useFurOptimization = FindProperty("_UseFurOptimization", properties, false) ?? FindProperty("_FurOptimizationEnabled", properties, false);
            furOptimizationLevel = FindProperty("_FurOptimizationLevel", properties, false) ?? FindProperty("_FurOptimizationMode", properties, false);
            
            // リアル影作成機能 (統合)
            useRealisticShadow = FindProperty("_UseRealisticShadow", properties, false);
            realisticShadowColor = FindProperty("_RealisticShadowColor", properties, false);
            realisticShadowIntensity = FindProperty("_RealisticShadowIntensity", properties, false);
            realisticShadowSoftness = FindProperty("_RealisticShadowSoftness", properties, false);
            realisticShadowOffset = FindProperty("_RealisticShadowOffset", properties, false);
            realisticShadowScale = FindProperty("_RealisticShadowScale", properties, false);
            
            // RimShade機能 (統合)
            useRimShade = FindProperty("_UseRimShade", properties, false);
            rimShadeColor = FindProperty("_RimShadeColor", properties, false);
            rimShadeIntensity = FindProperty("_RimShadeIntensity", properties, false);
            rimShadeWidth = FindProperty("_RimShadeWidth", properties, false);
            
            // レンダリング設定
            cull = FindProperty("_Cull", properties, false);
            zWrite = FindProperty("_ZWrite", properties, false);
            zTest = FindProperty("_ZTest", properties, false);
            srcBlend = FindProperty("_SrcBlend", properties, false);
            dstBlend = FindProperty("_DstBlend", properties, false);
            
            // ステンシル設定
            stencilRef = FindProperty("_StencilRef", properties, false);
            stencilReadMask = FindProperty("_StencilReadMask", properties, false);
            stencilWriteMask = FindProperty("_StencilWriteMask", properties, false);
            stencilComp = FindProperty("_StencilComp", properties, false);
            stencilPass = FindProperty("_StencilPass", properties, false);
            stencilFail = FindProperty("_StencilFail", properties, false);
            stencilZFail = FindProperty("_StencilZFail", properties, false);
        }
        
        private void ApplyHairShadowPreset(Material material)
        {
            // 前髪用のプリセット設定
            material.SetFloat("_UseRealisticShadow", 1.0f);
            material.SetColor("_RealisticShadowColor", new Color(0.2f, 0.2f, 0.2f, 0.8f));
            material.SetFloat("_RealisticShadowIntensity", 0.6f);
            material.SetFloat("_RealisticShadowSoftness", 0.4f);
            material.SetVector("_RealisticShadowOffset", new Vector4(0, -0.02f, 0, 0));
            material.SetVector("_RealisticShadowScale", new Vector4(1.05f, 1.05f, 1.05f, 1));
            
            // RimShade設定
            material.SetFloat("_UseRimShade", 1.0f);
            material.SetColor("_RimShadeColor", new Color(0.3f, 0.3f, 0.3f, 1.0f));
            material.SetFloat("_RimShadeIntensity", 0.3f);
            material.SetFloat("_RimShadeWidth", 0.6f);
            
            EditorUtility.DisplayDialog("Preset Applied", "Hair Shadow preset applied successfully!", "OK");
        }
        
        private void ApplyFaceShadowPreset(Material material)
        {
            // 顔用のプリセット設定
            material.SetFloat("_UseRealisticShadow", 1.0f);
            material.SetColor("_RealisticShadowColor", new Color(0.15f, 0.15f, 0.15f, 0.6f));
            material.SetFloat("_RealisticShadowIntensity", 0.4f);
            material.SetFloat("_RealisticShadowSoftness", 0.6f);
            material.SetVector("_RealisticShadowOffset", new Vector4(0, -0.01f, 0, 0));
            material.SetVector("_RealisticShadowScale", new Vector4(1.02f, 1.02f, 1.02f, 1));
            
            // RimShade設定
            material.SetFloat("_UseRimShade", 1.0f);
            material.SetColor("_RimShadeColor", new Color(0.25f, 0.25f, 0.25f, 1.0f));
            material.SetFloat("_RimShadeIntensity", 0.2f);
            material.SetFloat("_RimShadeWidth", 0.7f);
            
            EditorUtility.DisplayDialog("Preset Applied", "Face Shadow preset applied successfully!", "OK");
        }
    }
}


