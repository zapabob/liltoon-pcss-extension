 using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Collections.Generic;

namespace lilToon.PCSS
{
    public static class VRChatExpressionMenuCreator
    {
        private const string MENU_FOLDER = "Assets/PCSS_VRChat_Expressions";
        private const string ANIMATOR_FOLDER = "Assets/PCSS_VRChat_Animators";
        
        public static void CreatePCSSExpressionMenu(Material targetMaterial, bool useBaseLayer, string parameterPrefix)
        {
            // Create directories
            CreateDirectoryIfNotExists(MENU_FOLDER);
            CreateDirectoryIfNotExists(ANIMATOR_FOLDER);
            
            // Create expression parameters
            var expressionParameters = CreateExpressionParameters(parameterPrefix);
            
            // Create expression menu
            var expressionMenu = CreateExpressionMenu(targetMaterial, parameterPrefix);
            
            // Create animator controller
            var animatorController = CreateAnimatorController(targetMaterial, useBaseLayer, parameterPrefix);
            
            // Create material animations
            CreateMaterialAnimations(targetMaterial, parameterPrefix);
            
            // Save assets
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"VRChat Expression Menu created successfully for material: {targetMaterial.name}");
            Debug.Log($"Files created in: {MENU_FOLDER} and {ANIMATOR_FOLDER}");
        }
        
        private static void CreateDirectoryIfNotExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parentFolder = Path.GetDirectoryName(path);
                string folderName = Path.GetFileName(path);
                
                if (!AssetDatabase.IsValidFolder(parentFolder))
                {
                    AssetDatabase.CreateFolder("Assets", Path.GetFileName(parentFolder));
                }
                
                AssetDatabase.CreateFolder(parentFolder, folderName);
            }
        }
        
        private static ScriptableObject CreateExpressionParameters(string parameterPrefix)
        {
            // VRChat Expression Parameters用のScriptableObjectを作成
            // 実際のVRCSDKが無い場合の代替実装
            
            var parametersAsset = ScriptableObject.CreateInstance<ScriptableObject>();
            
            string assetPath = $"{MENU_FOLDER}/{parameterPrefix}_Parameters.asset";
            AssetDatabase.CreateAsset(parametersAsset, assetPath);
            
            return parametersAsset;
        }
        
        private static ScriptableObject CreateExpressionMenu(Material targetMaterial, string parameterPrefix)
        {
            // VRChat Expression Menu用のScriptableObjectを作成
            var menuAsset = ScriptableObject.CreateInstance<ScriptableObject>();
            
            string assetPath = $"{MENU_FOLDER}/{targetMaterial.name}_PCSS_Menu.asset";
            AssetDatabase.CreateAsset(menuAsset, assetPath);
            
            return menuAsset;
        }
        
        private static AnimatorController CreateAnimatorController(Material targetMaterial, bool useBaseLayer, string parameterPrefix)
        {
            string controllerName = useBaseLayer ? 
                $"{targetMaterial.name}_PCSS_Base.controller" : 
                $"{targetMaterial.name}_PCSS_FX.controller";
            
            string controllerPath = $"{ANIMATOR_FOLDER}/{controllerName}";
            
            // Create animator controller
            var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            
            // Add parameters
            controller.AddParameter($"{parameterPrefix}_Enable", AnimatorControllerParameterType.Bool);
            controller.AddParameter($"{parameterPrefix}_Quality", AnimatorControllerParameterType.Float);
            controller.AddParameter("VRC_LightVolume_Enable", AnimatorControllerParameterType.Bool);
            controller.AddParameter("VRC_LightVolume_Intensity", AnimatorControllerParameterType.Float);
            
            // Get or create base layer
            AnimatorControllerLayer baseLayer;
            if (controller.layers.Length > 0)
            {
                baseLayer = controller.layers[0];
                baseLayer.name = "PCSS Control";
            }
            else
            {
                baseLayer = new AnimatorControllerLayer
                {
                    name = "PCSS Control",
                    defaultWeight = 1f,
                    stateMachine = new AnimatorStateMachine()
                };
                controller.AddLayer(baseLayer);
            }
            
            // Create states
            CreatePCSSStates(controller, baseLayer, targetMaterial, parameterPrefix);
            
            return controller;
        }
        
        private static void CreatePCSSStates(AnimatorController controller, AnimatorControllerLayer layer, 
                                           Material targetMaterial, string parameterPrefix)
        {
            var stateMachine = layer.stateMachine;
            
            // Create states for PCSS toggle
            var pcssOffState = stateMachine.AddState("PCSS Off");
            var pcssOnState = stateMachine.AddState("PCSS On");
            
            // Create quality states
            var qualityLowState = stateMachine.AddState("Quality Low");
            var qualityMediumState = stateMachine.AddState("Quality Medium");
            var qualityHighState = stateMachine.AddState("Quality High");
            var qualityUltraState = stateMachine.AddState("Quality Ultra");
            
            // Load animations
            var pcssOffAnimation = LoadOrCreateAnimation($"{targetMaterial.name}_PCSS_Off");
            var pcssOnAnimation = LoadOrCreateAnimation($"{targetMaterial.name}_PCSS_On");
            var qualityLowAnimation = LoadOrCreateAnimation($"{targetMaterial.name}_Quality_Low");
            var qualityMediumAnimation = LoadOrCreateAnimation($"{targetMaterial.name}_Quality_Medium");
            var qualityHighAnimation = LoadOrCreateAnimation($"{targetMaterial.name}_Quality_High");
            var qualityUltraAnimation = LoadOrCreateAnimation($"{targetMaterial.name}_Quality_Ultra");
            
            // Assign animations to states
            pcssOffState.motion = pcssOffAnimation;
            pcssOnState.motion = pcssOnAnimation;
            qualityLowState.motion = qualityLowAnimation;
            qualityMediumState.motion = qualityMediumAnimation;
            qualityHighState.motion = qualityHighAnimation;
            qualityUltraState.motion = qualityUltraAnimation;
            
            // Create transitions for PCSS toggle
            var offToOnTransition = pcssOffState.AddTransition(pcssOnState);
            offToOnTransition.AddCondition(AnimatorConditionMode.If, 0, $"{parameterPrefix}_Enable");
            offToOnTransition.hasExitTime = false;
            offToOnTransition.duration = 0;
            
            var onToOffTransition = pcssOnState.AddTransition(pcssOffState);
            onToOffTransition.AddCondition(AnimatorConditionMode.IfNot, 0, $"{parameterPrefix}_Enable");
            onToOffTransition.hasExitTime = false;
            onToOffTransition.duration = 0;
            
            // Create quality transitions
            CreateQualityTransitions(qualityLowState, qualityMediumState, qualityHighState, qualityUltraState, parameterPrefix);
            
            // Set default state
            stateMachine.defaultState = pcssOnState;
        }
        
        private static void CreateQualityTransitions(AnimatorState lowState, AnimatorState mediumState, 
                                                   AnimatorState highState, AnimatorState ultraState, 
                                                   string parameterPrefix)
        {
            string qualityParam = $"{parameterPrefix}_Quality";
            
            // Low to Medium (0.25)
            var lowToMedium = lowState.AddTransition(mediumState);
            lowToMedium.AddCondition(AnimatorConditionMode.Greater, 0.25f, qualityParam);
            lowToMedium.hasExitTime = false;
            lowToMedium.duration = 0;
            
            // Medium to Low
            var mediumToLow = mediumState.AddTransition(lowState);
            mediumToLow.AddCondition(AnimatorConditionMode.Less, 0.25f, qualityParam);
            mediumToLow.hasExitTime = false;
            mediumToLow.duration = 0;
            
            // Medium to High (0.5)
            var mediumToHigh = mediumState.AddTransition(highState);
            mediumToHigh.AddCondition(AnimatorConditionMode.Greater, 0.5f, qualityParam);
            mediumToHigh.hasExitTime = false;
            mediumToHigh.duration = 0;
            
            // High to Medium
            var highToMedium = highState.AddTransition(mediumState);
            highToMedium.AddCondition(AnimatorConditionMode.Less, 0.5f, qualityParam);
            highToMedium.hasExitTime = false;
            highToMedium.duration = 0;
            
            // High to Ultra (0.75)
            var highToUltra = highState.AddTransition(ultraState);
            highToUltra.AddCondition(AnimatorConditionMode.Greater, 0.75f, qualityParam);
            highToUltra.hasExitTime = false;
            highToUltra.duration = 0;
            
            // Ultra to High
            var ultraToHigh = ultraState.AddTransition(highState);
            ultraToHigh.AddCondition(AnimatorConditionMode.Less, 0.75f, qualityParam);
            ultraToHigh.hasExitTime = false;
            ultraToHigh.duration = 0;
        }
        
        private static AnimationClip LoadOrCreateAnimation(string animationName)
        {
            string animationPath = $"{ANIMATOR_FOLDER}/{animationName}.anim";
            
            var existingClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animationPath);
            if (existingClip != null)
                return existingClip;
            
            var newClip = new AnimationClip();
            AssetDatabase.CreateAsset(newClip, animationPath);
            
            return newClip;
        }
        
        private static void CreateMaterialAnimations(Material targetMaterial, string parameterPrefix)
        {
            string materialPath = AssetDatabase.GetAssetPath(targetMaterial);
            string materialName = targetMaterial.name;
            
            // Create PCSS On/Off animations
            CreatePCSSToggleAnimation(materialPath, $"{materialName}_PCSS_Off", false);
            CreatePCSSToggleAnimation(materialPath, $"{materialName}_PCSS_On", true);
            
            // Create quality animations
            CreateQualityAnimation(materialPath, $"{materialName}_Quality_Low", 0);
            CreateQualityAnimation(materialPath, $"{materialName}_Quality_Medium", 1);
            CreateQualityAnimation(materialPath, $"{materialName}_Quality_High", 2);
            CreateQualityAnimation(materialPath, $"{materialName}_Quality_Ultra", 3);
        }
        
        private static void CreatePCSSToggleAnimation(string materialPath, string animationName, bool enabled)
        {
            var clip = LoadOrCreateAnimation(animationName);
            
            // Clear existing curves
            AnimationUtility.SetEditorCurves(clip, new EditorCurveBinding[0], new AnimationCurve[0]);
            
            // Create binding for PCSS enable property
            var binding = new EditorCurveBinding
            {
                path = "",
                type = typeof(Renderer),
                propertyName = $"material._PCSSEnabled"
            };
            
            // Create animation curve
            var curve = new AnimationCurve();
            curve.AddKey(0f, enabled ? 1f : 0f);
            
            // Set the curve
            AnimationUtility.SetEditorCurve(clip, binding, curve);
            
            // Also animate VRChat expression parameters
            var vrchatBinding = new EditorCurveBinding
            {
                path = "",
                type = typeof(Renderer),
                propertyName = $"material._PCSSToggleParam"
            };
            
            var vrchatCurve = new AnimationCurve();
            vrchatCurve.AddKey(0f, enabled ? 1f : 0f);
            AnimationUtility.SetEditorCurve(clip, vrchatBinding, vrchatCurve);
            
            EditorUtility.SetDirty(clip);
        }
        
        private static void CreateQualityAnimation(string materialPath, string animationName, int qualityLevel)
        {
            var clip = LoadOrCreateAnimation(animationName);
            
            // Clear existing curves
            AnimationUtility.SetEditorCurves(clip, new EditorCurveBinding[0], new AnimationCurve[0]);
            
            // Quality settings for each level
            var qualitySettings = new Dictionary<int, (int samples, float searchRadius, float filterRadius)>
            {
                { 0, (8, 0.005f, 0.005f) },   // Low
                { 1, (16, 0.01f, 0.01f) },    // Medium
                { 2, (32, 0.015f, 0.015f) },  // High
                { 3, (64, 0.02f, 0.02f) }     // Ultra
            };
            
            if (!qualitySettings.ContainsKey(qualityLevel)) return;
            
            var settings = qualitySettings[qualityLevel];
            
            // Create bindings and curves for quality parameters
            var bindings = new[]
            {
                new EditorCurveBinding { path = "", type = typeof(Renderer), propertyName = "material._PCSSSampleCount" },
                new EditorCurveBinding { path = "", type = typeof(Renderer), propertyName = "material._PCSSBlockerSearchRadius" },
                new EditorCurveBinding { path = "", type = typeof(Renderer), propertyName = "material._PCSSFilterRadius" },
                new EditorCurveBinding { path = "", type = typeof(Renderer), propertyName = "material._PCSSQualityParam" }
            };
            
            var values = new float[] 
            { 
                settings.samples, 
                settings.searchRadius, 
                settings.filterRadius, 
                qualityLevel / 3f // Normalize to 0-1 range
            };
            
            for (int i = 0; i < bindings.Length; i++)
            {
                var curve = new AnimationCurve();
                curve.AddKey(0f, values[i]);
                AnimationUtility.SetEditorCurve(clip, bindings[i], curve);
            }
            
            EditorUtility.SetDirty(clip);
        }
        
        [MenuItem("lilToon/PCSS Extension/Create VRChat Expression Menu")]
        public static void CreateVRChatExpressionMenuFromMenu()
        {
            var selectedMaterials = Selection.GetFiltered<Material>(SelectionMode.Assets);
            
            if (selectedMaterials.Length == 0)
            {
                EditorUtility.DisplayDialog("No Material Selected", 
                    "Please select a material with PCSS shader in the Project window.", "OK");
                return;
            }
            
            foreach (var material in selectedMaterials)
            {
                if (material.shader.name.Contains("PCSS"))
                {
                    CreatePCSSExpressionMenu(material, true, "PCSS");
                }
            }
        }
        
        [MenuItem("lilToon/PCSS Extension/Create VRChat Expression Menu", true)]
        public static bool ValidateCreateVRChatExpressionMenuFromMenu()
        {
            var selectedMaterials = Selection.GetFiltered<Material>(SelectionMode.Assets);
            return selectedMaterials.Length > 0;
        }
    }
} 