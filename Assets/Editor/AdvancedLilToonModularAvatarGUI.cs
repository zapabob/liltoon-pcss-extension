#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using VRC.SDK3.Avatars.Components;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// lilToon Advanced Modular Avatar統合GUI
    /// なんｊ風に言うと「これで完璧なModular Avatar統合システムが完成したぜ！」💪🔥
    /// </summary>
    public class AdvancedLilToonModularAvatarGUI : ShaderGUI
    {
        private const string ADVANCED_MODULAR_AVATAR_SHADER_NAME = "lilToon/Advanced Modular Avatar Integration";
        private const string LILTOON_SHADER_NAME = "lilToon/lilToon";

        // プロパティのマッピング定義
        private static readonly Dictionary<string, string> PropertyMapping = new Dictionary<string, string>()
        {
            // 基本プロパティ
            { "_MainTex", "_MainTex" },
            { "_Color", "_Color" },
            { "_Cutoff", "_Cutoff" },
            
            // Modular Avatar統合プロパティ
            { "_UseModularAvatarIntegration", "_UseModularAvatarIntegration" },
            { "_ModularAvatarQuality", "_ModularAvatarQuality" },
            { "_ModularAvatarAdvancedLighting", "_ModularAvatarAdvancedLighting" },
            { "_ModularAvatarPerformanceMode", "_ModularAvatarPerformanceMode" },
            
            // 高度なライティングプロパティ
            { "_UseAdvancedLighting", "_UseAdvancedLighting" },
            { "_AdvancedLightingQuality", "_AdvancedLightingQuality" },
            { "_DynamicLightingIntensity", "_DynamicLightingIntensity" },
            { "_RealTimeLightingUpdate", "_RealTimeLightingUpdate" },
            
            // 動的マテリアルプロパティ
            { "_UseDynamicMaterials", "_UseDynamicMaterials" },
            { "_DynamicMaterialQuality", "_DynamicMaterialQuality" },
            { "_MaterialUpdateFrequency", "_MaterialUpdateFrequency" },
            { "_RealTimeMaterialUpdate", "_RealTimeMaterialUpdate" },
            
            // 高度なシェーダープロパティ
            { "_UseAdvancedShaders", "_UseAdvancedShaders" },
            { "_AdvancedShaderQuality", "_AdvancedShaderQuality" },
            { "_ShaderCompilationOptimization", "_ShaderCompilationOptimization" },
            { "_RealTimeShaderUpdate", "_RealTimeShaderUpdate" },
            
            // パフォーマンス最適化プロパティ
            { "_UsePerformanceOptimization", "_UsePerformanceOptimization" },
            { "_PerformanceMode", "_PerformanceMode" },
            { "_MemoryOptimization", "_MemoryOptimization" },
            { "_CPUOptimization", "_CPUOptimization" },
            { "_GPUOptimization", "_GPUOptimization" },
            
            // シャドウプロパティ
            { "_UseAdvancedShadows", "_UseAdvancedShadows" },
            { "_ShadowQuality", "_ShadowQuality" },
            { "_ShadowDistance", "_ShadowDistance" },
            { "_ShadowCascadeCount", "_ShadowCascadeCount" },
            
            // PCSSプロパティ
            { "_UseRealTimePCSS", "_UseRealTimePCSS" },
            { "_PCSSQuality", "_PCSSQuality" },
            { "_PCSSSamples", "_PCSSSamples" },
            { "_PCSSFilterRadius", "_PCSSFilterRadius" },
            { "_PCSSLightSize", "_PCSSLightSize" },
            { "_PCSSBias", "_PCSSBias" },
            { "_PCSSIntensity", "_PCSSIntensity" },
            
            // ボリュメトリックプロパティ
            { "_UseVolumetricShadows", "_UseVolumetricShadows" },
            { "_VolumetricQuality", "_VolumetricQuality" },
            { "_VolumetricSamples", "_VolumetricSamples" },
            { "_VolumetricDensity", "_VolumetricDensity" },
            
            // 反射プロパティ
            { "_UseRealTimeReflection", "_UseRealTimeReflection" },
            { "_ReflectionQuality", "_ReflectionQuality" },
            { "_ReflectionIntensity", "_ReflectionIntensity" },
            { "_ReflectionRoughness", "_ReflectionRoughness" },
            
            // サブサーフェススキャタリングプロパティ
            { "_UseSubsurfaceScattering", "_UseSubsurfaceScattering" },
            { "_SubsurfaceQuality", "_SubsurfaceQuality" },
            { "_SubsurfaceColor", "_SubsurfaceColor" },
            { "_SubsurfaceRadius", "_SubsurfaceRadius" },
            
            // 品質設定プロパティ
            { "_QualityLevel", "_QualityLevel" },
            { "_QualityPreset", "_QualityPreset" },
            { "_QualityOptimization", "_QualityOptimization" },
            
            // VRC Light Volumeプロパティ
            { "_UseVRCLightVolumes", "_UseVRCLightVolumes" },
            { "_VRCLightVolumeIntensity", "_VRCLightVolumeIntensity" },
            { "_VRCLightVolumeTint", "_VRCLightVolumeTint" },
            { "_VRCLightVolumeDistanceFactor", "_VRCLightVolumeDistanceFactor" },
            
            // アニメシャドウプロパティ
            { "_UseAnimeShadow", "_UseAnimeShadow" },
            { "_AnimeShadowIntensity", "_AnimeShadowIntensity" },
            { "_AnimeShadowColor", "_AnimeShadowColor" },
            { "_AnimeShadowBlur", "_AnimeShadowBlur" },
            
            // シネマティックシャドウプロパティ
            { "_UseCinematicShadow", "_UseCinematicShadow" },
            { "_CinematicShadowQuality", "_CinematicShadowQuality" },
            { "_CinematicShadowIntensity", "_CinematicShadowIntensity" },
            { "_CinematicShadowColor", "_CinematicShadowColor" },
            
            // レンダリング設定プロパティ
            { "_Cull", "_Cull" },
            { "_ZWrite", "_ZWrite" },
            { "_ZTest", "_ZTest" },
            { "_SrcBlend", "_SrcBlend" },
            { "_DstBlend", "_DstBlend" },
            
            // ステンシル設定プロパティ
            { "_StencilRef", "_StencilRef" },
            { "_StencilReadMask", "_StencilReadMask" },
            { "_StencilWriteMask", "_StencilWriteMask" },
            { "_StencilComp", "_StencilComp" },
            { "_StencilPass", "_StencilPass" },
            { "_StencilFail", "_StencilFail" },
            { "_StencilZFail", "_StencilZFail" }
        };

        private static readonly string[] TextureProperties = {
            "_MainTex",
            "_ShadowColorTex",
            "_NormalMap",
            "_EmissionMap",
            "_RimColorTex",
            "_OutlineTex",
            "_MatCapTex",
            "_ReflectionColorTex",
            "_ReflectionCubeTex",
            "_RefractionColorTex",
            "_FurTex",
            "_FurMask",
            "_FurLengthMask",
            "_FurVectorTex",
            "_FurGravity",
            "_FurAO",
            "_FurSubsurfaceColor",
            "_FurSubsurfaceTex",
            "_FurSubsurfaceMask",
            "_FurSubsurfaceAO",
            "_FurSubsurfaceAOTex",
            "_FurSubsurfaceAOMask",
            "_FurSubsurfaceAOScale",
            "_FurSubsurfaceAOPower",
            "_FurSubsurfaceAOBias",
            "_FurSubsurfaceAOSaturation",
            "_FurSubsurfaceAOContrast",
            "_FurSubsurfaceAOGamma",
            "_FurSubsurfaceAOHue",
            "_FurSubsurfaceAOSaturation",
            "_FurSubsurfaceAOValue",
            "_FurSubsurfaceAOLuminance",
            "_FurSubsurfaceAOLuminanceTex",
            "_FurSubsurfaceAOLuminanceMask",
            "_FurSubsurfaceAOLuminanceScale",
            "_FurSubsurfaceAOLuminancePower",
            "_FurSubsurfaceAOLuminanceBias",
            "_FurSubsurfaceAOLuminanceSaturation",
            "_FurSubsurfaceAOLuminanceContrast",
            "_FurSubsurfaceAOLuminanceGamma",
            "_FurSubsurfaceAOLuminanceHue",
            "_FurSubsurfaceAOLuminanceValue"
        };

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
            Material material = materialEditor.target as Material;
            if (material == null) return;

            currentMaterialEditor = materialEditor;
            var props = properties.ToDictionary(p => p.name);

            EditorGUILayout.Space(10);
            GUILayout.Label("🎯 Advanced lilToon Modular Avatar Integration", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // 基本設定
            showBasicSettings = EditorGUILayout.Foldout(showBasicSettings, "⚙️ Basic Settings");
            if (showBasicSettings)
            {
                EditorGUI.indentLevel++;
                DrawProperty(props, "_MainTex", "Main Texture");
                DrawProperty(props, "_Color", "Color");
                DrawProperty(props, "_Cutoff", "Cutoff");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // Modular Avatar設定
            showModularAvatarSettings = EditorGUILayout.Foldout(showModularAvatarSettings, "🔗 Modular Avatar Settings");
            if (showModularAvatarSettings)
            {
                EditorGUI.indentLevel++;
                DrawToggleProperty(props, "_UseModularAvatarIntegration", "Use Modular Avatar Integration", "_MODULARAVATAR_INTEGRATION");
                DrawProperty(props, "_ModularAvatarQuality", "Modular Avatar Quality");
                DrawToggleProperty(props, "_ModularAvatarAdvancedLighting", "Advanced Lighting", "_MODULARAVATAR_ADVANCED_LIGHTING");
                DrawToggleProperty(props, "_ModularAvatarPerformanceMode", "Performance Mode", "_MODULARAVATAR_PERFORMANCE_MODE");
                
                if (GUILayout.Button("🔧 Setup Modular Avatar Integration", GUILayout.Height(25)))
                {
                    SetupModularAvatarIntegration(material);
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // 高度なライティング設定
            showAdvancedLightingSettings = EditorGUILayout.Foldout(showAdvancedLightingSettings, "💡 Advanced Lighting Settings");
            if (showAdvancedLightingSettings)
            {
                EditorGUI.indentLevel++;
                DrawToggleProperty(props, "_UseAdvancedLighting", "Use Advanced Lighting", "_ADVANCED_LIGHTING");
                DrawProperty(props, "_AdvancedLightingQuality", "Advanced Lighting Quality");
                DrawProperty(props, "_DynamicLightingIntensity", "Dynamic Lighting Intensity");
                DrawToggleProperty(props, "_RealTimeLightingUpdate", "Real-time Lighting Update", "_REALTIME_LIGHTING_UPDATE");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // 動的マテリアル設定
            showDynamicMaterialSettings = EditorGUILayout.Foldout(showDynamicMaterialSettings, "🔄 Dynamic Material Settings");
            if (showDynamicMaterialSettings)
            {
                EditorGUI.indentLevel++;
                DrawToggleProperty(props, "_UseDynamicMaterials", "Use Dynamic Materials", "_DYNAMIC_MATERIALS");
                DrawProperty(props, "_DynamicMaterialQuality", "Dynamic Material Quality");
                DrawProperty(props, "_MaterialUpdateFrequency", "Material Update Frequency");
                DrawToggleProperty(props, "_RealTimeMaterialUpdate", "Real-time Material Update", "_REALTIME_MATERIAL_UPDATE");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // 高度なシェーダー設定
            showAdvancedShaderSettings = EditorGUILayout.Foldout(showAdvancedShaderSettings, "🎨 Advanced Shader Settings");
            if (showAdvancedShaderSettings)
            {
                EditorGUI.indentLevel++;
                DrawToggleProperty(props, "_UseAdvancedShaders", "Use Advanced Shaders", "_ADVANCED_SHADERS");
                DrawProperty(props, "_AdvancedShaderQuality", "Advanced Shader Quality");
                DrawToggleProperty(props, "_ShaderCompilationOptimization", "Shader Compilation Optimization", "_SHADER_COMPILATION_OPTIMIZATION");
                DrawToggleProperty(props, "_RealTimeShaderUpdate", "Real-time Shader Update", "_REALTIME_SHADER_UPDATE");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // パフォーマンス最適化設定
            showPerformanceOptimizationSettings = EditorGUILayout.Foldout(showPerformanceOptimizationSettings, "⚡ Performance Optimization Settings");
            if (showPerformanceOptimizationSettings)
            {
                EditorGUI.indentLevel++;
                DrawToggleProperty(props, "_UsePerformanceOptimization", "Use Performance Optimization", "_PERFORMANCE_OPTIMIZATION");
                DrawToggleProperty(props, "_PerformanceMode", "Performance Mode", "_PERFORMANCE_MODE");
                DrawToggleProperty(props, "_MemoryOptimization", "Memory Optimization", "_MEMORY_OPTIMIZATION");
                DrawToggleProperty(props, "_CPUOptimization", "CPU Optimization", "_CPU_OPTIMIZATION");
                DrawToggleProperty(props, "_GPUOptimization", "GPU Optimization", "_GPU_OPTIMIZATION");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // シャドウ設定
            showShadowSettings = EditorGUILayout.Foldout(showShadowSettings, "🌑 Shadow Settings");
            if (showShadowSettings)
            {
                EditorGUI.indentLevel++;
                DrawToggleProperty(props, "_UseAdvancedShadows", "Use Advanced Shadows", "_ADVANCED_SHADOWS");
                DrawProperty(props, "_ShadowQuality", "Shadow Quality");
                DrawProperty(props, "_ShadowDistance", "Shadow Distance");
                DrawProperty(props, "_ShadowCascadeCount", "Shadow Cascade Count");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // PCSS設定
            showPCSSSettings = EditorGUILayout.Foldout(showPCSSSettings, "🎯 PCSS Settings");
            if (showPCSSSettings)
            {
                EditorGUI.indentLevel++;
                DrawToggleProperty(props, "_UseRealTimePCSS", "Use Real-time PCSS", "_REALTIME_PCSS");
                DrawProperty(props, "_PCSSQuality", "PCSS Quality");
                DrawProperty(props, "_PCSSSamples", "PCSS Samples");
                DrawProperty(props, "_PCSSFilterRadius", "PCSS Filter Radius");
                DrawProperty(props, "_PCSSLightSize", "PCSS Light Size");
                DrawProperty(props, "_PCSSBias", "PCSS Bias");
                DrawProperty(props, "_PCSSIntensity", "PCSS Intensity");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // ボリュメトリック設定
            showVolumetricSettings = EditorGUILayout.Foldout(showVolumetricSettings, "🌫️ Volumetric Settings");
            if (showVolumetricSettings)
            {
                EditorGUI.indentLevel++;
                DrawToggleProperty(props, "_UseVolumetricShadows", "Use Volumetric Shadows", "_VOLUMETRIC_SHADOWS");
                DrawProperty(props, "_VolumetricQuality", "Volumetric Quality");
                DrawProperty(props, "_VolumetricSamples", "Volumetric Samples");
                DrawProperty(props, "_VolumetricDensity", "Volumetric Density");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // 反射設定
            showReflectionSettings = EditorGUILayout.Foldout(showReflectionSettings, "✨ Reflection Settings");
            if (showReflectionSettings)
            {
                EditorGUI.indentLevel++;
                DrawToggleProperty(props, "_UseRealTimeReflection", "Use Real-time Reflection", "_REALTIME_REFLECTION");
                DrawProperty(props, "_ReflectionQuality", "Reflection Quality");
                DrawProperty(props, "_ReflectionIntensity", "Reflection Intensity");
                DrawProperty(props, "_ReflectionRoughness", "Reflection Roughness");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // サブサーフェススキャタリング設定
            showSubsurfaceSettings = EditorGUILayout.Foldout(showSubsurfaceSettings, "🔴 Subsurface Scattering Settings");
            if (showSubsurfaceSettings)
            {
                EditorGUI.indentLevel++;
                DrawToggleProperty(props, "_UseSubsurfaceScattering", "Use Subsurface Scattering", "_SUBSURFACE_SCATTERING");
                DrawProperty(props, "_SubsurfaceQuality", "Subsurface Quality");
                DrawProperty(props, "_SubsurfaceColor", "Subsurface Color");
                DrawProperty(props, "_SubsurfaceRadius", "Subsurface Radius");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // 品質設定
            showQualitySettings = EditorGUILayout.Foldout(showQualitySettings, "📊 Quality Settings");
            if (showQualitySettings)
            {
                EditorGUI.indentLevel++;
                DrawProperty(props, "_QualityLevel", "Quality Level");
                DrawProperty(props, "_QualityPreset", "Quality Preset");
                DrawToggleProperty(props, "_QualityOptimization", "Quality Optimization", "_QUALITY_OPTIMIZATION");
                
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Quality Presets", EditorStyles.miniBoldLabel);
                
                if (GUILayout.Button("🎨 Cinematic", GUILayout.Height(25)))
                {
                    ApplyQualityPreset(material, "Cinematic");
                }
                
                if (GUILayout.Button("⚡ Performance", GUILayout.Height(25)))
                {
                    ApplyQualityPreset(material, "Performance");
                }
                
                if (GUILayout.Button("🔧 Balanced", GUILayout.Height(25)))
                {
                    ApplyQualityPreset(material, "Balanced");
                }
                
                if (GUILayout.Button("�� Ultra", GUILayout.Height(25)))
                {
                    ApplyQualityPreset(material, "Ultra");
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // VRC Light Volume設定
            showVRCLightVolumeSettings = EditorGUILayout.Foldout(showVRCLightVolumeSettings, "💡 VRC Light Volume Settings");
            if (showVRCLightVolumeSettings)
            {
                EditorGUI.indentLevel++;
                DrawToggleProperty(props, "_UseVRCLightVolumes", "Use VRC Light Volumes", "_VRC_LIGHT_VOLUMES");
                DrawProperty(props, "_VRCLightVolumeIntensity", "VRC Light Volume Intensity");
                DrawProperty(props, "_VRCLightVolumeTint", "VRC Light Volume Tint");
                DrawProperty(props, "_VRCLightVolumeDistanceFactor", "VRC Light Volume Distance Factor");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // アニメシャドウ設定
            showAnimeSettings = EditorGUILayout.Foldout(showAnimeSettings, "🎭 Anime Shadow Settings");
            if (showAnimeSettings)
            {
                EditorGUI.indentLevel++;
                DrawToggleProperty(props, "_UseAnimeShadow", "Use Anime Shadow", "_ANIME_SHADOW");
                DrawProperty(props, "_AnimeShadowIntensity", "Anime Shadow Intensity");
                DrawProperty(props, "_AnimeShadowColor", "Anime Shadow Color");
                DrawProperty(props, "_AnimeShadowBlur", "Anime Shadow Blur");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // シネマティックシャドウ設定
            showCinematicSettings = EditorGUILayout.Foldout(showCinematicSettings, "🎬 Cinematic Shadow Settings");
            if (showCinematicSettings)
            {
                EditorGUI.indentLevel++;
                DrawToggleProperty(props, "_UseCinematicShadow", "Use Cinematic Shadow", "_CINEMATIC_SHADOW");
                DrawProperty(props, "_CinematicShadowQuality", "Cinematic Shadow Quality");
                DrawProperty(props, "_CinematicShadowIntensity", "Cinematic Shadow Intensity");
                DrawProperty(props, "_CinematicShadowColor", "Cinematic Shadow Color");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // レンダリング設定
            showRenderingSettings = EditorGUILayout.Foldout(showRenderingSettings, "�� Rendering Settings");
            if (showRenderingSettings)
            {
                EditorGUI.indentLevel++;
                DrawProperty(props, "_Cull", "Cull");
                DrawProperty(props, "_ZWrite", "ZWrite");
                DrawProperty(props, "_ZTest", "ZTest");
                DrawProperty(props, "_SrcBlend", "SrcBlend");
                DrawProperty(props, "_DstBlend", "DstBlend");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // ステンシル設定
            showStencilSettings = EditorGUILayout.Foldout(showStencilSettings, "🔧 Stencil Settings");
            if (showStencilSettings)
            {
                EditorGUI.indentLevel++;
                DrawProperty(props, "_StencilRef", "StencilRef");
                DrawProperty(props, "_StencilReadMask", "StencilReadMask");
                DrawProperty(props, "_StencilWriteMask", "StencilWriteMask");
                DrawProperty(props, "_StencilComp", "StencilComp");
                DrawProperty(props, "_StencilPass", "StencilPass");
                DrawProperty(props, "_StencilFail", "StencilFail");
                DrawProperty(props, "_StencilZFail", "StencilZFail");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);

            // アクションボタン
            EditorGUILayout.LabelField("🚀 Actions", EditorStyles.miniBoldLabel);
            
            if (GUILayout.Button("🔄 Upgrade to Advanced Modular Avatar Integration", GUILayout.Height(30)))
            {
                UpgradeToAdvancedModularAvatarIntegration(material);
            }
            
            if (GUILayout.Button("⚙️ Set Default Values", GUILayout.Height(30)))
            {
                SetDefaultValues(material);
            }
            
            if (GUILayout.Button("✅ Enable All Features", GUILayout.Height(30)))
            {
                EnableAllFeatures(material);
            }
            
            if (GUILayout.Button("❌ Disable All Features", GUILayout.Height(30)))
            {
                DisableAllFeatures(material);
            }
        }

        private MaterialEditor currentMaterialEditor;

        private void DrawProperty(Dictionary<string, MaterialProperty> props, string propertyName, string displayName)
        {
            if (props.TryGetValue(propertyName, out var prop))
            {
                currentMaterialEditor.ShaderProperty(prop, displayName);
            }
        }

        private void DrawToggleProperty(Dictionary<string, MaterialProperty> props, string propertyName, string displayName, string keyword = "")
        {
            if (props.TryGetValue(propertyName, out var prop))
            {
                bool value = prop.floatValue > 0.5f;
                bool newValue = EditorGUILayout.Toggle(displayName, value);
                
                if (newValue != value)
                {
                    prop.floatValue = newValue ? 1.0f : 0.0f;
                    
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        Material material = currentMaterialEditor.target as Material;
                        if (material != null)
                        {
                            if (newValue)
                                material.EnableKeyword(keyword);
                            else
                                material.DisableKeyword(keyword);
                        }
                    }
                }
            }
        }

        private void UpgradeToAdvancedModularAvatarIntegration(Material material)
        {
            if (material.shader.name != ADVANCED_MODULAR_AVATAR_SHADER_NAME)
            {
                Shader advancedShader = Shader.Find(ADVANCED_MODULAR_AVATAR_SHADER_NAME);
                if (advancedShader != null)
                {
                    material.shader = advancedShader;
                    SetDefaultValues(material);
                    EditorUtility.DisplayDialog("Upgrade Complete", "Material upgraded to Advanced Modular Avatar Integration shader.", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Advanced Modular Avatar Integration shader not found.", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Info", "Material is already using Advanced Modular Avatar Integration shader.", "OK");
            }
        }

        private void SetDefaultValues(Material material)
        {
            // 基本プロパティのデフォルト値設定
            material.SetFloat("_UseModularAvatarIntegration", 1.0f);
            material.SetFloat("_ModularAvatarQuality", 1.0f);
            material.SetFloat("_ModularAvatarAdvancedLighting", 1.0f);
            material.SetFloat("_ModularAvatarPerformanceMode", 0.0f);
            
            material.SetFloat("_UseAdvancedLighting", 1.0f);
            material.SetFloat("_AdvancedLightingQuality", 1.0f);
            material.SetFloat("_DynamicLightingIntensity", 1.0f);
            material.SetFloat("_RealTimeLightingUpdate", 1.0f);
            
            material.SetFloat("_UseDynamicMaterials", 1.0f);
            material.SetFloat("_DynamicMaterialQuality", 1.0f);
            material.SetFloat("_MaterialUpdateFrequency", 1.0f);
            material.SetFloat("_RealTimeMaterialUpdate", 1.0f);
            
            material.SetFloat("_UseAdvancedShaders", 1.0f);
            material.SetFloat("_AdvancedShaderQuality", 1.0f);
            material.SetFloat("_ShaderCompilationOptimization", 1.0f);
            material.SetFloat("_RealTimeShaderUpdate", 1.0f);
            
            material.SetFloat("_UsePerformanceOptimization", 1.0f);
            material.SetFloat("_PerformanceMode", 0.0f);
            material.SetFloat("_MemoryOptimization", 1.0f);
            material.SetFloat("_CPUOptimization", 1.0f);
            material.SetFloat("_GPUOptimization", 1.0f);
            
            material.SetFloat("_UseAdvancedShadows", 1.0f);
            material.SetFloat("_ShadowQuality", 1.0f);
            material.SetFloat("_ShadowDistance", 50.0f);
            material.SetFloat("_ShadowCascadeCount", 4.0f);
            
            material.SetFloat("_UseRealTimePCSS", 1.0f);
            material.SetFloat("_PCSSQuality", 1.0f);
            material.SetFloat("_PCSSSamples", 16.0f);
            material.SetFloat("_PCSSFilterRadius", 0.01f);
            material.SetFloat("_PCSSLightSize", 0.1f);
            material.SetFloat("_PCSSBias", 0.001f);
            material.SetFloat("_PCSSIntensity", 1.0f);
            
            material.SetFloat("_UseVolumetricShadows", 1.0f);
            material.SetFloat("_VolumetricQuality", 1.0f);
            material.SetFloat("_VolumetricSamples", 16.0f);
            material.SetFloat("_VolumetricDensity", 1.0f);
            
            material.SetFloat("_UseRealTimeReflection", 1.0f);
            material.SetFloat("_ReflectionQuality", 1.0f);
            material.SetFloat("_ReflectionIntensity", 1.0f);
            material.SetFloat("_ReflectionRoughness", 0.5f);
            
            material.SetFloat("_UseSubsurfaceScattering", 1.0f);
            material.SetFloat("_SubsurfaceQuality", 1.0f);
            material.SetColor("_SubsurfaceColor", Color.red);
            material.SetFloat("_SubsurfaceRadius", 1.0f);
            
            material.SetFloat("_QualityLevel", 1.0f);
            material.SetFloat("_QualityPreset", 1.0f);
            material.SetFloat("_QualityOptimization", 1.0f);
            
            material.SetFloat("_UseVRCLightVolumes", 1.0f);
            material.SetFloat("_VRCLightVolumeIntensity", 1.0f);
            material.SetColor("_VRCLightVolumeTint", Color.white);
            material.SetFloat("_VRCLightVolumeDistanceFactor", 0.1f);
            
            material.SetFloat("_UseAnimeShadow", 1.0f);
            material.SetFloat("_AnimeShadowIntensity", 1.0f);
            material.SetColor("_AnimeShadowColor", Color.black);
            material.SetFloat("_AnimeShadowBlur", 0.5f);
            
            material.SetFloat("_UseCinematicShadow", 1.0f);
            material.SetFloat("_CinematicShadowQuality", 1.0f);
            material.SetFloat("_CinematicShadowIntensity", 1.0f);
            material.SetColor("_CinematicShadowColor", Color.black);
            
            EditorUtility.SetDirty(material);
        }

        private void ApplyQualityPreset(Material material, string presetName)
        {
            switch (presetName)
            {
                case "Cinematic":
                    material.SetFloat("_ModularAvatarQuality", 2.0f);
                    material.SetFloat("_AdvancedLightingQuality", 2.0f);
                    material.SetFloat("_DynamicMaterialQuality", 2.0f);
                    material.SetFloat("_AdvancedShaderQuality", 2.0f);
                    material.SetFloat("_PCSSSamples", 32.0f);
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
                    
                case "Balanced":
                    material.SetFloat("_ModularAvatarQuality", 1.0f);
                    material.SetFloat("_AdvancedLightingQuality", 1.0f);
                    material.SetFloat("_DynamicMaterialQuality", 1.0f);
                    material.SetFloat("_AdvancedShaderQuality", 1.0f);
                    material.SetFloat("_PCSSSamples", 16.0f);
                    material.SetFloat("_UseVolumetricShadows", 1.0f);
                    material.SetFloat("_UseRealTimeReflection", 1.0f);
                    material.SetFloat("_UseSubsurfaceScattering", 1.0f);
                    break;
                    
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
            GameObject avatarRoot = GetAvatarRoot(material);
            if (avatarRoot == null)
            {
                EditorUtility.DisplayDialog("エラー", "アバターのルートオブジェクトが見つかりません。", "OK");
                return;
            }

            Undo.RecordObject(avatarRoot, "Setup Modular Avatar Integration");

            // ModularAvatarPCSSControllerコンポーネントの追加
            if (avatarRoot.GetComponent<ModularAvatarPCSSController>() == null)
            {
                avatarRoot.AddComponent<ModularAvatarPCSSController>();
            }

            material.SetFloat("_UseModularAvatarIntegration", 1.0f);
            material.SetFloat("_ModularAvatarQuality", 1.0f);
            material.SetFloat("_ModularAvatarAdvancedLighting", 1.0f);
            material.SetFloat("_ModularAvatarPerformanceMode", 0.0f);
            
            EditorUtility.SetDirty(material);
            EditorUtility.DisplayDialog("完了", "Modular Avatar統合をセットアップしました。", "OK");
        }

        private GameObject GetAvatarRoot(Material material)
        {
            if (Selection.activeGameObject != null)
            {
                return Selection.activeGameObject;
            }
            return null;
        }
    }

    /// <summary>
    /// Modular Avatar PCSS制御コンポーネント
    /// </summary>
    public class ModularAvatarPCSSController : MonoBehaviour
    {
        [Header("PCSS Settings")]
        public bool usePCSS = true;
        public float pcssQuality = 1.0f;
        public int pcssSamples = 16;
        public float pcssFilterRadius = 0.01f;
        public float pcssLightSize = 0.1f;
        public float pcssBias = 0.001f;
        public float pcssIntensity = 1.0f;

        [Header("Performance Settings")]
        public bool usePerformanceMode = false;
        public bool useMemoryOptimization = true;
        public bool useCPUOptimization = true;
        public bool useGPUOptimization = true;

        [Header("Advanced Settings")]
        public bool useAdvancedLighting = true;
        public bool useDynamicMaterials = true;
        public bool useAdvancedShaders = true;
        public bool useVolumetricShadows = true;
        public bool useRealTimeReflection = true;
        public bool useSubsurfaceScattering = true;

        private void Start()
        {
            ApplySettings();
        }

        private void ApplySettings()
        {
            var materials = GetComponentsInChildren<Renderer>()
                .SelectMany(r => r.sharedMaterials)
                .Where(m => m != null && m.shader.name.Contains("lilToon"))
                .ToArray();

            foreach (var material in materials)
            {
                material.SetFloat("_UseRealTimePCSS", usePCSS ? 1.0f : 0.0f);
                material.SetFloat("_PCSSQuality", pcssQuality);
                material.SetFloat("_PCSSSamples", pcssSamples);
                material.SetFloat("_PCSSFilterRadius", pcssFilterRadius);
                material.SetFloat("_PCSSLightSize", pcssLightSize);
                material.SetFloat("_PCSSBias", pcssBias);
                material.SetFloat("_PCSSIntensity", pcssIntensity);

                material.SetFloat("_PerformanceMode", usePerformanceMode ? 1.0f : 0.0f);
                material.SetFloat("_MemoryOptimization", useMemoryOptimization ? 1.0f : 0.0f);
                material.SetFloat("_CPUOptimization", useCPUOptimization ? 1.0f : 0.0f);
                material.SetFloat("_GPUOptimization", useGPUOptimization ? 1.0f : 0.0f);

                material.SetFloat("_UseAdvancedLighting", useAdvancedLighting ? 1.0f : 0.0f);
                material.SetFloat("_UseDynamicMaterials", useDynamicMaterials ? 1.0f : 0.0f);
                material.SetFloat("_UseAdvancedShaders", useAdvancedShaders ? 1.0f : 0.0f);
                material.SetFloat("_UseVolumetricShadows", useVolumetricShadows ? 1.0f : 0.0f);
                material.SetFloat("_UseRealTimeReflection", useRealTimeReflection ? 1.0f : 0.0f);
                material.SetFloat("_UseSubsurfaceScattering", useSubsurfaceScattering ? 1.0f : 0.0f);
            }
        }
    }
}
#endif