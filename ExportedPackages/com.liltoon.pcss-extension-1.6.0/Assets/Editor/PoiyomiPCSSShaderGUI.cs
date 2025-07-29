using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace lilToon.PCSS
{
    public class PoiyomiPCSSShaderGUI : ShaderGUI
    {
        private static readonly Dictionary<string, bool> foldouts = new Dictionary<string, bool>();
        private MaterialEditor materialEditor;
        private MaterialProperty[] properties;
        
        // Property cache
        private MaterialProperty pcssEnabled;
        private MaterialProperty pcssBlockerSearchRadius;
        private MaterialProperty pcssFilterRadius;
        private MaterialProperty pcssSampleCount;
        private MaterialProperty pcssLightSize;
        private MaterialProperty pcssShadowBias;
        private MaterialProperty pcssQuality;
        
        // VRChat Expression Properties
        private MaterialProperty vrchatExpression;
        private MaterialProperty pcssToggleParam;
        private MaterialProperty pcssQualityParam;
        
        // Quality presets
        private static readonly PCSSQualityPreset[] qualityPresets = new PCSSQualityPreset[]
        {
            new PCSSQualityPreset("Low", 8, 0.005f, 0.005f, 0.05f, 0.0005f),
            new PCSSQualityPreset("Medium", 16, 0.01f, 0.01f, 0.1f, 0.001f),
            new PCSSQualityPreset("High", 32, 0.015f, 0.015f, 0.15f, 0.0015f),
            new PCSSQualityPreset("Ultra", 64, 0.02f, 0.02f, 0.2f, 0.002f)
        };
        
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            this.materialEditor = materialEditor;
            this.properties = properties;
            
            CacheProperties();
            
            Material material = materialEditor.target as Material;
            
            // Header
            EditorGUILayout.Space();
            GUILayout.Label("✦ Poiyomi PCSS Extension v1.0 ✦", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space();
            
            // Main Properties
            if (DrawFoldout("Main", true))
            {
                EditorGUI.indentLevel++;
                materialEditor.TexturePropertySingleLine(new GUIContent("Main Texture"), FindProperty("_MainTex"), FindProperty("_Color"));
                materialEditor.ShaderProperty(FindProperty("_Cutoff"), "Alpha Cutoff");
                EditorGUI.indentLevel--;
            }
            
            // PCSS Properties
            if (DrawFoldout("PCSS Soft Shadows", true))
            {
                EditorGUI.indentLevel++;
                
                // Enable toggle
                materialEditor.ShaderProperty(pcssEnabled, "Enable PCSS");
                
                if (pcssEnabled.floatValue > 0.5f)
                {
                    EditorGUILayout.Space();
                    
                    // Quality preset selector
                    EditorGUI.BeginChangeCheck();
                    int currentQuality = (int)pcssQuality.floatValue;
                    int newQuality = EditorGUILayout.Popup("Quality Preset", currentQuality, 
                        new string[] { "Low", "Medium", "High", "Ultra" });
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        pcssQuality.floatValue = newQuality;
                        ApplyQualityPreset(newQuality);
                    }
                    
                    // Performance warning
                    if (newQuality >= 2)
                    {
                        EditorGUILayout.HelpBox(
                            newQuality == 2 ? "High quality: May impact performance on lower-end devices" :
                                            "Ultra quality: Recommended for high-end PCs only",
                            newQuality == 2 ? MessageType.Warning : MessageType.Error);
                    }
                    
                    EditorGUILayout.Space();
                    
                    // Manual parameters
                    if (DrawFoldout("Manual Parameters", false))
                    {
                        EditorGUI.indentLevel++;
                        materialEditor.ShaderProperty(pcssBlockerSearchRadius, "Blocker Search Radius");
                        materialEditor.ShaderProperty(pcssFilterRadius, "Filter Radius");
                        materialEditor.ShaderProperty(pcssSampleCount, "Sample Count");
                        materialEditor.ShaderProperty(pcssLightSize, "Light Size");
                        materialEditor.ShaderProperty(pcssShadowBias, "Shadow Bias");
                        EditorGUI.indentLevel--;
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            
            // VRChat Expression Control
            if (DrawFoldout("VRChat Expression Control", false))
            {
                EditorGUI.indentLevel++;
                
                materialEditor.ShaderProperty(vrchatExpression, "Enable VRChat Expression Control");
                
                if (vrchatExpression.floatValue > 0.5f)
                {
                    EditorGUILayout.HelpBox(
                        "VRChat Expression Control is enabled. PCSS will be controlled by avatar parameters:\n" +
                        "• PCSS_Enable: Toggle PCSS on/off\n" +
                        "• PCSS_Quality: Control quality (0-1 range)",
                        MessageType.Info);
                    
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.LabelField("Expression Parameters (Read-only):");
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginDisabledGroup(true);
                    materialEditor.ShaderProperty(pcssToggleParam, "PCSS Toggle Parameter");
                    materialEditor.ShaderProperty(pcssQualityParam, "PCSS Quality Parameter");
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.indentLevel--;
                    
                    if (GUILayout.Button("Setup VRChat Expression Menu"))
                    {
                        SetupVRChatExpressionMenu(material);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            
            // Poiyomi Base Properties
            if (DrawFoldout("Poiyomi Base", false))
            {
                EditorGUI.indentLevel++;
                materialEditor.TexturePropertySingleLine(new GUIContent("Alpha Mask"), FindProperty("_AlphaMask"));
                EditorGUI.indentLevel--;
            }
            
            // Shading
            if (DrawFoldout("Shading", false))
            {
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(FindProperty("_ShadowStrength"), "Shadow Strength");
                materialEditor.ShaderProperty(FindProperty("_ShadowOffset"), "Shadow Offset");
                EditorGUI.indentLevel--;
            }
            
            // Rim Lighting
            if (DrawFoldout("Rim Lighting", false))
            {
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(FindProperty("_EnableRimLighting"), "Enable Rim Lighting");
                
                if (FindProperty("_EnableRimLighting").floatValue > 0.5f)
                {
                    materialEditor.ShaderProperty(FindProperty("_RimLightColor"), "Rim Light Color");
                    materialEditor.ShaderProperty(FindProperty("_RimPower"), "Rim Power");
                    materialEditor.ShaderProperty(FindProperty("_RimStrength"), "Rim Strength");
                }
                EditorGUI.indentLevel--;
            }
            
            // Outline
            if (DrawFoldout("Outline", false))
            {
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(FindProperty("_EnableOutline"), "Enable Outline");
                
                if (FindProperty("_EnableOutline").floatValue > 0.5f)
                {
                    materialEditor.ShaderProperty(FindProperty("_OutlineWidth"), "Outline Width");
                    materialEditor.ShaderProperty(FindProperty("_OutlineColor"), "Outline Color");
                }
                EditorGUI.indentLevel--;
            }
            
            // Rendering
            if (DrawFoldout("Rendering", false))
            {
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(FindProperty("_Cull"), "Cull");
                materialEditor.ShaderProperty(FindProperty("_SrcBlend"), "Source Blend");
                materialEditor.ShaderProperty(FindProperty("_DstBlend"), "Destination Blend");
                materialEditor.ShaderProperty(FindProperty("_ZWrite"), "ZWrite");
                materialEditor.ShaderProperty(FindProperty("_ZTest"), "ZTest");
                EditorGUI.indentLevel--;
            }
            
            // Stencil
            if (DrawFoldout("Stencil", false))
            {
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(FindProperty("_StencilRef"), "Stencil Reference");
                materialEditor.ShaderProperty(FindProperty("_StencilReadMask"), "Stencil ReadMask");
                materialEditor.ShaderProperty(FindProperty("_StencilWriteMask"), "Stencil WriteMask");
                materialEditor.ShaderProperty(FindProperty("_StencilComp"), "Stencil Compare");
                materialEditor.ShaderProperty(FindProperty("_StencilOp"), "Stencil Op");
                materialEditor.ShaderProperty(FindProperty("_StencilFail"), "Stencil Fail");
                materialEditor.ShaderProperty(FindProperty("_StencilZFail"), "Stencil ZFail");
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
            
            // Footer
            GUILayout.Label("Poiyomi PCSS Extension - Compatible with Poiyomi Toon Shader", EditorStyles.centeredGreyMiniLabel);
        }
        
        private void CacheProperties()
        {
            pcssEnabled = FindProperty("_PCSSEnabled");
            pcssBlockerSearchRadius = FindProperty("_PCSSBlockerSearchRadius");
            pcssFilterRadius = FindProperty("_PCSSFilterRadius");
            pcssSampleCount = FindProperty("_PCSSSampleCount");
            pcssLightSize = FindProperty("_PCSSLightSize");
            pcssShadowBias = FindProperty("_PCSSShadowBias");
            pcssQuality = FindProperty("_PCSSQuality");
            
            vrchatExpression = FindProperty("_VRChatExpression");
            pcssToggleParam = FindProperty("_PCSSToggleParam");
            pcssQualityParam = FindProperty("_PCSSQualityParam");
        }
        
        private MaterialProperty FindProperty(string name)
        {
            return FindProperty(name, properties);
        }
        
        private bool DrawFoldout(string title, bool defaultState = false)
        {
            string key = $"PoiyomiPCSS_{title}";
            if (!foldouts.ContainsKey(key))
                foldouts[key] = defaultState;
            
            bool state = foldouts[key];
            bool newState = EditorGUILayout.Foldout(state, title, true);
            
            if (newState != state)
                foldouts[key] = newState;
            
            return newState;
        }
        
        private void ApplyQualityPreset(int qualityIndex)
        {
            if (qualityIndex < 0 || qualityIndex >= qualityPresets.Length) return;
            
            var preset = qualityPresets[qualityIndex];
            pcssSampleCount.floatValue = preset.sampleCount;
            pcssBlockerSearchRadius.floatValue = preset.blockerSearchRadius;
            pcssFilterRadius.floatValue = preset.filterRadius;
            pcssLightSize.floatValue = preset.lightSize;
            pcssShadowBias.floatValue = preset.shadowBias;
        }
        
        private void SetupVRChatExpressionMenu(Material material)
        {
            // Create VRChat expression menu setup
            var window = EditorWindow.GetWindow<VRChatExpressionSetupWindow>();
            window.targetMaterial = material;
            window.Show();
        }
        
        private struct PCSSQualityPreset
        {
            public string name;
            public int sampleCount;
            public float blockerSearchRadius;
            public float filterRadius;
            public float lightSize;
            public float shadowBias;
            
            public PCSSQualityPreset(string name, int sampleCount, float blockerSearchRadius, 
                                   float filterRadius, float lightSize, float shadowBias)
            {
                this.name = name;
                this.sampleCount = sampleCount;
                this.blockerSearchRadius = blockerSearchRadius;
                this.filterRadius = filterRadius;
                this.lightSize = lightSize;
                this.shadowBias = shadowBias;
            }
        }
    }
    
    public class VRChatExpressionSetupWindow : EditorWindow
    {
        public Material targetMaterial;
        private bool useBaseLayer = true;
        private string parameterPrefix = "PCSS";
        
        private void OnGUI()
        {
            titleContent = new GUIContent("VRChat Expression Setup");
            
            EditorGUILayout.LabelField("VRChat Expression Menu Setup", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            if (targetMaterial == null)
            {
                EditorGUILayout.HelpBox("No target material selected.", MessageType.Error);
                return;
            }
            
            EditorGUILayout.LabelField($"Target Material: {targetMaterial.name}");
            EditorGUILayout.Space();
            
            useBaseLayer = EditorGUILayout.Toggle("Use Base Layer (Recommended)", useBaseLayer);
            parameterPrefix = EditorGUILayout.TextField("Parameter Prefix", parameterPrefix);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                $"This will create the following parameters:\n" +
                $"• {parameterPrefix}_Enable (Bool) - Toggle PCSS on/off\n" +
                $"• {parameterPrefix}_Quality (Float 0-1) - Control quality level\n\n" +
                $"Layer: {(useBaseLayer ? "Base" : "FX")} Animator Controller",
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Create Expression Menu"))
            {
                CreateVRChatExpressionMenu();
            }
            
            if (GUILayout.Button("Close"))
            {
                Close();
            }
        }
        
        private void CreateVRChatExpressionMenu()
        {
            try
            {
                VRChatExpressionMenuCreator.CreatePCSSExpressionMenu(targetMaterial, useBaseLayer, parameterPrefix);
                EditorUtility.DisplayDialog("Success", "VRChat Expression Menu created successfully!", "OK");
                Close();
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to create expression menu: {e.Message}", "OK");
            }
        }
    }
} 
