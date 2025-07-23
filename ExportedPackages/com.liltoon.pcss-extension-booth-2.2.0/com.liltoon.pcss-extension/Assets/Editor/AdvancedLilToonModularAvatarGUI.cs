#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// lilToonとModular Avatar AAOの高度な統合システムのカスタムエディタGUI
/// 競合製品を上回る機能を提供
/// </summary>
public class AdvancedLilToonModularAvatarGUI : ShaderGUI
{
    private const string ADVANCED_MODULAR_AVATAR_SHADER_NAME = "lilToon/Advanced Modular Avatar Integration";
    private const string LILTOON_SHADER_NAME = "lilToon/lilToon";

    // プロパティマッピング
    private static readonly Dictionary<string, string> PropertyMapping = new Dictionary<string, string>
    {
        // lilToon基本プロパティ
        {"_MainTex", "_MainTex"},
        {"_Color", "_Color"},
        {"_Cutoff", "_Cutoff"},
        
        // 影・シャドウ
        {"_UseShadow", "_UseShadow"},
        {"_ShadowColorTex", "_ShadowColorTex"},
        {"_ShadowBorder", "_ShadowBorder"},
        {"_ShadowBlur", "_ShadowBlur"},
        {"_ShadowStrength", "_ShadowStrength"},
        
        // PCSS拡張プロパティ
        {"_UseAdvancedShadows", "_UseAdvancedShadows"},
        {"_UseRealTimePCSS", "_UseRealTimePCSS"},
        {"_UseDynamicQuality", "_UseDynamicQuality"},
        {"_UseAdvancedLighting", "_UseAdvancedLighting"},
        {"_ShadowQualityMode", "_ShadowQualityMode"},
        {"_PCSSQualityLevel", "_PCSSQualityLevel"},
        {"_PCSSSamples", "_PCSSSamples"},
        {"_PCSSFilterRadius", "_PCSSFilterRadius"},
        {"_PCSSLightSize", "_PCSSLightSize"},
        {"_PCSSBias", "_PCSSBias"},
        {"_PCSSIntensity", "_PCSSIntensity"},
        
        // ボリュメトリック影
        {"_UseVolumetricShadows", "_UseVolumetricShadows"},
        {"_VolumetricShadowDensity", "_VolumetricShadowDensity"},
        {"_VolumetricShadowSteps", "_VolumetricShadowSteps"},
        
        // リアルタイム反射
        {"_UseRealTimeReflection", "_UseRealTimeReflection"},
        {"_ReflectionStrength", "_ReflectionStrength"},
        {"_ReflectionRoughness", "_ReflectionRoughness"},
        
        // サブサーフェススキャタリング
        {"_UseSubsurfaceScattering", "_UseSubsurfaceScattering"},
        {"_SubsurfaceStrength", "_SubsurfaceStrength"},
        {"_SubsurfaceColor", "_SubsurfaceColor"},
        
        // 距離ベース品質調整
        {"_UseDistanceBasedQuality", "_UseDistanceBasedQuality"},
        {"_DistanceBasedQuality", "_DistanceBasedQuality"},
        {"_QualityDistanceThreshold", "_QualityDistanceThreshold"},
        
        // パフォーマンス最適化
        {"_UsePerformanceMode", "_UsePerformanceMode"},
        {"_PerformanceMode", "_PerformanceMode"},
        
        // VRC Light Volumes
        {"_UseVRCLightVolumes", "_UseVRCLightVolumes"},
        {"_VRCLightVolumeIntensity", "_VRCLightVolumeIntensity"},
        {"_VRCLightVolumeTint", "_VRCLightVolumeTint"},
        {"_VRCLightVolumeDistanceFactor", "_VRCLightVolumeDistanceFactor"},
        
        // レンダリング設定
        {"_Cull", "_Cull"},
        {"_ZWrite", "_ZWrite"},
        {"_ZTest", "_ZTest"},
        {"_SrcBlend", "_SrcBlend"},
        {"_DstBlend", "_DstBlend"},
        
        // ステンシル設定
        {"_StencilRef", "_StencilRef"},
        {"_StencilReadMask", "_StencilReadMask"},
        {"_StencilWriteMask", "_StencilWriteMask"},
        {"_StencilComp", "_StencilComp"},
        {"_StencilPass", "_StencilPass"},
        {"_StencilFail", "_StencilFail"},
        {"_StencilZFail", "_StencilZFail"},
        
        // 高度な影設定
        {"_UseShadowClamp", "_UseShadowClamp"},
        {"_ShadowClamp", "_ShadowClamp"},
        {"_Translucency", "_Translucency"},
        
        // アニメーション影設定
        {"_UseAnimeShadow", "_UseAnimeShadow"},
        {"_AnimeShadowStrength", "_AnimeShadowStrength"},
        {"_AnimeShadowSmoothness", "_AnimeShadowSmoothness"},
        
        // 映画風影設定
        {"_UseCinematicShadow", "_UseCinematicShadow"},
        {"_CinematicShadowContrast", "_CinematicShadowContrast"},
        {"_CinematicShadowSaturation", "_CinematicShadowSaturation"}
    };

    // テクスチャプロパティ
    private static readonly string[] TextureProperties = {
        "_MainTex", "_ShadowColorTex"
    };

    // フォールドアウト状態
    private bool showBasicSettings = true;
    private bool showModularAvatarSettings = true;
    private bool showAdvancedLightingSettings = true;
    private bool showDynamicMaterialSettings = true;
    private bool showAdvancedShaderSettings = true;
    private bool showPerformanceOptimizationSettings = true;
    private bool showShadowSettings = true;
    private bool showPCSSSettings = true;
    private bool showVolumetricSettings = true;
    private bool showReflectionSettings = true;
    private bool showSubsurfaceSettings = true;
    private bool showQualitySettings = true;
    private bool showVRCLightVolumeSettings = true;
    private bool showAnimeSettings = true;
    private bool showCinematicSettings = true;
    private bool showRenderingSettings = true;
    private bool showStencilSettings = true;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        currentMaterialEditor = materialEditor;
        Material material = materialEditor.target as Material;
        if (material == null) return;

        if (material.shader.name != ADVANCED_MODULAR_AVATAR_SHADER_NAME)
        {
            EditorGUILayout.HelpBox("このマテリアルは高度なModular Avatar統合シェーダーを使用していません。", MessageType.Warning);
            if (GUILayout.Button("高度なModular Avatar統合シェーダーに変更"))
            {
                UpgradeToAdvancedModularAvatarIntegration(material);
            }
            return;
        }

        var props = new Dictionary<string, MaterialProperty>();
        foreach (var prop in properties)
        {
            props[prop.name] = prop;
        }

        // 基本設定
        showBasicSettings = EditorGUILayout.Foldout(showBasicSettings, "Basic Settings", true);
        if (showBasicSettings)
        {
            DrawProperty(props, "_MainTex", "Main Texture");
            DrawProperty(props, "_Color", "Color");
            DrawProperty(props, "_Cutoff", "Alpha Cutoff");
        }

        // Modular Avatar統合設定
        showModularAvatarSettings = EditorGUILayout.Foldout(showModularAvatarSettings, "Modular Avatar Integration", true);
        if (showModularAvatarSettings)
        {
            DrawToggleProperty(props, "_UseModularAvatarIntegration", "Use Modular Avatar Integration");
            DrawProperty(props, "_ModularAvatarQuality", "Modular Avatar Quality");
            DrawProperty(props, "_ModularAvatarShadowMode", "Modular Avatar Shadow Mode");
            DrawToggleProperty(props, "_ModularAvatarAdvancedLighting", "Modular Avatar Advanced Lighting");
            DrawToggleProperty(props, "_ModularAvatarPerformanceMode", "Modular Avatar Performance Mode");
            DrawToggleProperty(props, "_ModularAvatarVolumetricShadows", "Modular Avatar Volumetric Shadows");
            DrawToggleProperty(props, "_ModularAvatarRealTimeReflection", "Modular Avatar Real-time Reflection");
            DrawToggleProperty(props, "_ModularAvatarSubsurfaceScattering", "Modular Avatar Subsurface Scattering");
        }

        // 高度なライティング設定
        showAdvancedLightingSettings = EditorGUILayout.Foldout(showAdvancedLightingSettings, "Advanced Lighting System", true);
        if (showAdvancedLightingSettings)
        {
            DrawToggleProperty(props, "_UseAdvancedLighting", "Use Advanced Lighting");
            DrawProperty(props, "_AdvancedLightingIntensity", "Advanced Lighting Intensity");
            DrawProperty(props, "_AdvancedLightingQuality", "Advanced Lighting Quality");
            DrawProperty(props, "_AdvancedLightingColor", "Advanced Lighting Color");
        }

        // 動的マテリアル設定
        showDynamicMaterialSettings = EditorGUILayout.Foldout(showDynamicMaterialSettings, "Dynamic Material System", true);
        if (showDynamicMaterialSettings)
        {
            DrawToggleProperty(props, "_UseDynamicMaterials", "Use Dynamic Materials");
            DrawProperty(props, "_DynamicMaterialQuality", "Dynamic Material Quality");
            DrawProperty(props, "_DynamicMaterialTime", "Dynamic Material Time");
            DrawProperty(props, "_DynamicMaterialDistance", "Dynamic Material Distance");
        }

        // 高度なシェーダー設定
        showAdvancedShaderSettings = EditorGUILayout.Foldout(showAdvancedShaderSettings, "Advanced Shader System", true);
        if (showAdvancedShaderSettings)
        {
            DrawToggleProperty(props, "_UseAdvancedShaders", "Use Advanced Shaders");
            DrawProperty(props, "_AdvancedShaderQuality", "Advanced Shader Quality");
            DrawProperty(props, "_AdvancedShaderOptimization", "Advanced Shader Optimization");
        }

        // パフォーマンス最適化設定
        showPerformanceOptimizationSettings = EditorGUILayout.Foldout(showPerformanceOptimizationSettings, "Performance Optimization", true);
        if (showPerformanceOptimizationSettings)
        {
            DrawToggleProperty(props, "_UsePerformanceOptimization", "Use Performance Optimization");
            DrawProperty(props, "_PerformanceLODLevel", "Performance LOD Level");
            DrawProperty(props, "_PerformanceFrameRate", "Performance Frame Rate");
            DrawProperty(props, "_PerformanceMemoryUsage", "Performance Memory Usage");
            DrawProperty(props, "_PerformanceGPUUsage", "Performance GPU Usage");
        }

        // 影・シャドウ設定
        showShadowSettings = EditorGUILayout.Foldout(showShadowSettings, "Shadow Settings", true);
        if (showShadowSettings)
        {
            DrawToggleProperty(props, "_UseShadow", "Use Shadow");
            DrawProperty(props, "_ShadowColorTex", "Shadow Color");
            DrawProperty(props, "_ShadowBorder", "Shadow Border");
            DrawProperty(props, "_ShadowBlur", "Shadow Blur");
            DrawProperty(props, "_ShadowStrength", "Shadow Strength");
        }

        // PCSS設定
        showPCSSSettings = EditorGUILayout.Foldout(showPCSSSettings, "PCSS Settings", true);
        if (showPCSSSettings)
        {
            DrawToggleProperty(props, "_UseAdvancedShadows", "Use Advanced Shadows");
            DrawToggleProperty(props, "_UseRealTimePCSS", "Use Real-time PCSS");
            DrawToggleProperty(props, "_UseDynamicQuality", "Use Dynamic Quality");
            DrawToggleProperty(props, "_UseAdvancedLighting", "Use Advanced Lighting");
            DrawProperty(props, "_ShadowQualityMode", "Shadow Quality Mode");
            DrawProperty(props, "_PCSSQualityLevel", "PCSS Quality Level");
            DrawProperty(props, "_PCSSSamples", "PCSS Samples");
            DrawProperty(props, "_PCSSFilterRadius", "PCSS Filter Radius");
            DrawProperty(props, "_PCSSLightSize", "PCSS Light Size");
            DrawProperty(props, "_PCSSBias", "PCSS Bias");
            DrawProperty(props, "_PCSSIntensity", "PCSS Intensity");
        }

        // ボリュメトリック設定
        showVolumetricSettings = EditorGUILayout.Foldout(showVolumetricSettings, "Volumetric Settings", true);
        if (showVolumetricSettings)
        {
            DrawToggleProperty(props, "_UseVolumetricShadows", "Use Volumetric Shadows");
            DrawProperty(props, "_VolumetricShadowDensity", "Volumetric Shadow Density");
            DrawProperty(props, "_VolumetricShadowSteps", "Volumetric Shadow Steps");
        }

        // 反射設定
        showReflectionSettings = EditorGUILayout.Foldout(showReflectionSettings, "Reflection Settings", true);
        if (showReflectionSettings)
        {
            DrawToggleProperty(props, "_UseRealTimeReflection", "Use Real-time Reflection");
            DrawProperty(props, "_ReflectionStrength", "Reflection Strength");
            DrawProperty(props, "_ReflectionRoughness", "Reflection Roughness");
        }

        // サブサーフェス設定
        showSubsurfaceSettings = EditorGUILayout.Foldout(showSubsurfaceSettings, "Subsurface Scattering", true);
        if (showSubsurfaceSettings)
        {
            DrawToggleProperty(props, "_UseSubsurfaceScattering", "Use Subsurface Scattering");
            DrawProperty(props, "_SubsurfaceStrength", "Subsurface Strength");
            DrawProperty(props, "_SubsurfaceColor", "Subsurface Color");
        }

        // 品質設定
        showQualitySettings = EditorGUILayout.Foldout(showQualitySettings, "Quality Settings", true);
        if (showQualitySettings)
        {
            DrawToggleProperty(props, "_UseDistanceBasedQuality", "Use Distance-based Quality");
            DrawProperty(props, "_DistanceBasedQuality", "Distance Quality Factor");
            DrawProperty(props, "_QualityDistanceThreshold", "Quality Distance Threshold");
        }

        // VRC Light Volumes設定
        showVRCLightVolumeSettings = EditorGUILayout.Foldout(showVRCLightVolumeSettings, "VRC Light Volumes", true);
        if (showVRCLightVolumeSettings)
        {
            DrawToggleProperty(props, "_UseVRCLightVolumes", "Use VRC Light Volumes");
            DrawProperty(props, "_VRCLightVolumeIntensity", "VRC Light Volume Intensity");
            DrawProperty(props, "_VRCLightVolumeTint", "VRC Light Volume Tint");
            DrawProperty(props, "_VRCLightVolumeDistanceFactor", "VRC Light Volume Distance Factor");
        }

        // アニメーション設定
        showAnimeSettings = EditorGUILayout.Foldout(showAnimeSettings, "Anime Settings", true);
        if (showAnimeSettings)
        {
            DrawToggleProperty(props, "_UseAnimeShadow", "Use Anime Shadow");
            DrawProperty(props, "_AnimeShadowStrength", "Anime Shadow Strength");
            DrawProperty(props, "_AnimeShadowSmoothness", "Anime Shadow Smoothness");
        }

        // 映画風設定
        showCinematicSettings = EditorGUILayout.Foldout(showCinematicSettings, "Cinematic Settings", true);
        if (showCinematicSettings)
        {
            DrawToggleProperty(props, "_UseCinematicShadow", "Use Cinematic Shadow");
            DrawProperty(props, "_CinematicShadowContrast", "Cinematic Shadow Contrast");
            DrawProperty(props, "_CinematicShadowSaturation", "Cinematic Shadow Saturation");
        }

        // レンダリング設定
        showRenderingSettings = EditorGUILayout.Foldout(showRenderingSettings, "Rendering Settings", true);
        if (showRenderingSettings)
        {
            DrawProperty(props, "_Cull", "Cull Mode");
            DrawProperty(props, "_ZWrite", "ZWrite");
            DrawProperty(props, "_ZTest", "ZTest");
            DrawProperty(props, "_SrcBlend", "Src Blend");
            DrawProperty(props, "_DstBlend", "Dst Blend");
        }

        // ステンシル設定
        showStencilSettings = EditorGUILayout.Foldout(showStencilSettings, "Stencil Settings", true);
        if (showStencilSettings)
        {
            DrawProperty(props, "_StencilRef", "Stencil Reference");
            DrawProperty(props, "_StencilReadMask", "Stencil Read Mask");
            DrawProperty(props, "_StencilWriteMask", "Stencil Write Mask");
            DrawProperty(props, "_StencilComp", "Stencil Compare");
            DrawProperty(props, "_StencilPass", "Stencil Pass");
            DrawProperty(props, "_StencilFail", "Stencil Fail");
            DrawProperty(props, "_StencilZFail", "Stencil ZFail");
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Apply Ultra Quality Preset"))
        {
            ApplyQualityPreset(material, "Ultra");
        }
        if (GUILayout.Button("Apply Performance Preset"))
        {
            ApplyQualityPreset(material, "Performance");
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Enable All Features"))
        {
            EnableAllFeatures(material);
        }
        if (GUILayout.Button("Disable All Features"))
        {
            DisableAllFeatures(material);
        }
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Setup Modular Avatar Integration"))
        {
            SetupModularAvatarIntegration(material);
        }
    }

    /// <summary>
    /// プロパティを描画
    /// </summary>
    private MaterialEditor currentMaterialEditor;

    private void DrawProperty(Dictionary<string, MaterialProperty> props, string propertyName, string displayName)
    {
        if (props.ContainsKey(propertyName))
        {
            currentMaterialEditor.ShaderProperty(props[propertyName], displayName);
        }
    }

    /// <summary>
    /// トグルプロパティを描画
    /// </summary>
    private void DrawToggleProperty(Dictionary<string, MaterialProperty> props, string propertyName, string displayName)
    {
        if (props.ContainsKey(propertyName))
        {
            currentMaterialEditor.ShaderProperty(props[propertyName], displayName);
        }
    }

    /// <summary>
    /// 高度なModular Avatar統合シェーダーにアップグレード
    /// </summary>
    private void UpgradeToAdvancedModularAvatarIntegration(Material material)
    {
        Shader advancedShader = Shader.Find(ADVANCED_MODULAR_AVATAR_SHADER_NAME);
        if (advancedShader == null)
        {
            EditorUtility.DisplayDialog("エラー", "高度なModular Avatar統合シェーダーが見つかりません。", "OK");
            return;
        }

        Undo.RecordObject(material, "Upgrade to Advanced Modular Avatar Integration");
        material.shader = advancedShader;
        SetDefaultValues(material);
        EditorUtility.SetDirty(material);
    }

    /// <summary>
    /// デフォルト値を設定
    /// </summary>
    private void SetDefaultValues(Material material)
    {
        // Modular Avatar統合のデフォルト値
        material.SetFloat("_UseModularAvatarIntegration", 1.0f);
        material.SetFloat("_ModularAvatarQuality", 1.0f);
        material.SetFloat("_ModularAvatarShadowMode", 0.0f);
        material.SetFloat("_ModularAvatarAdvancedLighting", 1.0f);
        material.SetFloat("_ModularAvatarPerformanceMode", 0.0f);
        material.SetFloat("_ModularAvatarVolumetricShadows", 0.0f);
        material.SetFloat("_ModularAvatarRealTimeReflection", 0.0f);
        material.SetFloat("_ModularAvatarSubsurfaceScattering", 0.0f);

        // 高度なライティングシステムのデフォルト値
        material.SetFloat("_UseAdvancedLighting", 1.0f);
        material.SetFloat("_AdvancedLightingIntensity", 1.0f);
        material.SetFloat("_AdvancedLightingQuality", 1.0f);
        material.SetColor("_AdvancedLightingColor", Color.white);

        // 動的マテリアルシステムのデフォルト値
        material.SetFloat("_UseDynamicMaterials", 1.0f);
        material.SetFloat("_DynamicMaterialQuality", 1.0f);
        material.SetFloat("_DynamicMaterialTime", 0.0f);
        material.SetFloat("_DynamicMaterialDistance", 10.0f);

        // 高度なシェーダーシステムのデフォルト値
        material.SetFloat("_UseAdvancedShaders", 1.0f);
        material.SetFloat("_AdvancedShaderQuality", 1.0f);
        material.SetFloat("_AdvancedShaderOptimization", 0.5f);

        // パフォーマンス最適化システムのデフォルト値
        material.SetFloat("_UsePerformanceOptimization", 1.0f);
        material.SetFloat("_PerformanceLODLevel", 1.0f);
        material.SetFloat("_PerformanceFrameRate", 60.0f);
        material.SetFloat("_PerformanceMemoryUsage", 0.5f);
        material.SetFloat("_PerformanceGPUUsage", 0.5f);

        // 高度な影システムのデフォルト値
        material.SetFloat("_UseAdvancedShadows", 1.0f);
        material.SetFloat("_UseRealTimePCSS", 1.0f);
        material.SetFloat("_UseDynamicQuality", 1.0f);
        material.SetFloat("_UseAdvancedLighting", 1.0f);
        material.SetFloat("_ShadowQualityMode", 0.0f);
        material.SetFloat("_PCSSQualityLevel", 1.0f);
        material.SetFloat("_PCSSSamples", 32.0f);
        material.SetFloat("_PCSSFilterRadius", 0.01f);
        material.SetFloat("_PCSSLightSize", 0.1f);
        material.SetFloat("_PCSSBias", 0.001f);
        material.SetFloat("_PCSSIntensity", 1.0f);

        // ボリュメトリック影のデフォルト値
        material.SetFloat("_UseVolumetricShadows", 0.0f);
        material.SetFloat("_VolumetricShadowDensity", 1.0f);
        material.SetFloat("_VolumetricShadowSteps", 16.0f);

        // リアルタイム反射のデフォルト値
        material.SetFloat("_UseRealTimeReflection", 0.0f);
        material.SetFloat("_ReflectionStrength", 0.5f);
        material.SetFloat("_ReflectionRoughness", 0.1f);

        // サブサーフェススキャタリングのデフォルト値
        material.SetFloat("_UseSubsurfaceScattering", 0.0f);
        material.SetFloat("_SubsurfaceStrength", 1.0f);
        material.SetColor("_SubsurfaceColor", new Color(1.0f, 0.2f, 0.1f, 1.0f));

        // 距離ベース品質調整のデフォルト値
        material.SetFloat("_UseDistanceBasedQuality", 1.0f);
        material.SetFloat("_DistanceBasedQuality", 0.5f);
        material.SetFloat("_QualityDistanceThreshold", 10.0f);

        // パフォーマンス最適化のデフォルト値
        material.SetFloat("_UsePerformanceMode", 0.0f);
        material.SetFloat("_PerformanceMode", 0.5f);

        // VRC Light Volumesのデフォルト値
        material.SetFloat("_UseVRCLightVolumes", 0.0f);
        material.SetFloat("_VRCLightVolumeIntensity", 1.0f);
        material.SetColor("_VRCLightVolumeTint", Color.white);
        material.SetFloat("_VRCLightVolumeDistanceFactor", 0.1f);

        // アニメーション影のデフォルト値
        material.SetFloat("_UseAnimeShadow", 0.0f);
        material.SetFloat("_AnimeShadowStrength", 0.8f);
        material.SetFloat("_AnimeShadowSmoothness", 0.5f);

        // 映画風影のデフォルト値
        material.SetFloat("_UseCinematicShadow", 0.0f);
        material.SetFloat("_CinematicShadowContrast", 1.0f);
        material.SetFloat("_CinematicShadowSaturation", 1.0f);

        // その他のデフォルト値
        material.SetFloat("_UseShadowClamp", 0.0f);
        material.SetFloat("_ShadowClamp", 0.5f);
        material.SetFloat("_Translucency", 0.5f);
    }

    /// <summary>
    /// 品質プリセットを適用
    /// </summary>
    private void ApplyQualityPreset(Material material, string presetName)
    {
        switch (presetName)
        {
            case "Ultra":
                material.SetFloat("_ModularAvatarQuality", 3.0f);
                material.SetFloat("_AdvancedLightingQuality", 3.0f);
                material.SetFloat("_DynamicMaterialQuality", 3.0f);
                material.SetFloat("_AdvancedShaderQuality", 3.0f);
                material.SetFloat("_PCSSSamples", 64.0f);
                material.SetFloat("_UseVolumetricShadows", 1.0f);
                material.SetFloat("_UseRealTimeReflection", 1.0f);
                material.SetFloat("_UseSubsurfaceScattering", 1.0f);
                break;
                
            case "Performance":
                material.SetFloat("_ModularAvatarQuality", 0.0f);
                material.SetFloat("_AdvancedLightingQuality", 0.0f);
                material.SetFloat("_DynamicMaterialQuality", 0.0f);
                material.SetFloat("_AdvancedShaderQuality", 0.0f);
                material.SetFloat("_PCSSSamples", 8.0f);
                material.SetFloat("_UseVolumetricShadows", 0.0f);
                material.SetFloat("_UseRealTimeReflection", 0.0f);
                material.SetFloat("_UseSubsurfaceScattering", 0.0f);
                material.SetFloat("_UsePerformanceOptimization", 1.0f);
                material.SetFloat("_PerformanceMode", 1.0f);
                break;
        }
        
        EditorUtility.SetDirty(material);
    }

    /// <summary>
    /// 全ての機能を有効化
    /// </summary>
    private void EnableAllFeatures(Material material)
    {
        material.SetFloat("_UseModularAvatarIntegration", 1.0f);
        material.SetFloat("_UseAdvancedLighting", 1.0f);
        material.SetFloat("_UseDynamicMaterials", 1.0f);
        material.SetFloat("_UseAdvancedShaders", 1.0f);
        material.SetFloat("_UseAdvancedShadows", 1.0f);
        material.SetFloat("_UseRealTimePCSS", 1.0f);
        material.SetFloat("_UseVolumetricShadows", 1.0f);
        material.SetFloat("_UseRealTimeReflection", 1.0f);
        material.SetFloat("_UseSubsurfaceScattering", 1.0f);
        material.SetFloat("_UseVRCLightVolumes", 1.0f);
        material.SetFloat("_UseAnimeShadow", 1.0f);
        material.SetFloat("_UseCinematicShadow", 1.0f);
        
        EditorUtility.SetDirty(material);
    }

    /// <summary>
    /// 全ての機能を無効化
    /// </summary>
    private void DisableAllFeatures(Material material)
    {
        material.SetFloat("_UseModularAvatarIntegration", 0.0f);
        material.SetFloat("_UseAdvancedLighting", 0.0f);
        material.SetFloat("_UseDynamicMaterials", 0.0f);
        material.SetFloat("_UseAdvancedShaders", 0.0f);
        material.SetFloat("_UseAdvancedShadows", 0.0f);
        material.SetFloat("_UseRealTimePCSS", 0.0f);
        material.SetFloat("_UseVolumetricShadows", 0.0f);
        material.SetFloat("_UseRealTimeReflection", 0.0f);
        material.SetFloat("_UseSubsurfaceScattering", 0.0f);
        material.SetFloat("_UseVRCLightVolumes", 0.0f);
        material.SetFloat("_UseAnimeShadow", 0.0f);
        material.SetFloat("_UseCinematicShadow", 0.0f);
        
        EditorUtility.SetDirty(material);
    }

    /// <summary>
    /// Modular Avatar統合をセットアップ
    /// </summary>
    private void SetupModularAvatarIntegration(Material material)
    {
        material.SetFloat("_UseModularAvatarIntegration", 1.0f);
        material.SetFloat("_ModularAvatarQuality", 1.0f);
        material.SetFloat("_ModularAvatarAdvancedLighting", 1.0f);
        material.SetFloat("_ModularAvatarPerformanceMode", 0.0f);
        
        EditorUtility.SetDirty(material);
        EditorUtility.DisplayDialog("完了", "Modular Avatar統合をセットアップしました。", "OK");
    }
}
#endif 