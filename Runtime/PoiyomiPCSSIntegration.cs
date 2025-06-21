using UnityEngine;
using System.Collections.Generic;

namespace lilToon.PCSS
{
    [System.Serializable]
    public class PoiyomiPCSSSettings
    {
        [Header("PCSS Configuration")]
        public bool enablePCSS = true;
        public PCSSUtilities.PCSSQuality quality = PCSSUtilities.PCSSQuality.Medium;
        public float blockerSearchRadius = 0.01f;
        public float filterRadius = 0.01f;
        public int sampleCount = 16;
        public float lightSize = 0.1f;
        public float shadowBias = 0.001f;
        
        [Header("VRChat Expression")]
        public bool enableVRChatExpression = false;
        public bool useBaseLayer = true;
        public string parameterPrefix = "PCSS";
        
        [Header("Poiyomi Compatibility")]
        public bool maintainPoiyomiFeatures = true;
        public bool autoDetectPoiyomiShader = true;
    }
    
    // PCSSQuality enum is now defined in PCSSUtilities class
    // Use PCSSUtilities.PCSSQuality instead
    
    /// <summary>
    /// Poiyomi PCSS Integration Component
    /// Provides runtime control and VRChat expression integration for Poiyomi PCSS Extension
    /// </summary>
    [AddComponentMenu("lilToon PCSS/Poiyomi PCSS Integration")]
    public class PoiyomiPCSSIntegration : MonoBehaviour
    {
        [SerializeField] private PoiyomiPCSSSettings settings = new PoiyomiPCSSSettings();
        [SerializeField] private List<Material> targetMaterials = new List<Material>();
        [SerializeField] private List<Renderer> targetRenderers = new List<Renderer>();
        
        // VRChat Expression Parameters
        private Dictionary<string, float> expressionParameters = new Dictionary<string, float>();
        
        // Quality presets
        private static readonly Dictionary<PCSSUtilities.PCSSQuality, PCSSQualityData> qualityPresets = 
            new Dictionary<PCSSUtilities.PCSSQuality, PCSSQualityData>
        {
            { PCSSUtilities.PCSSQuality.Low, new PCSSQualityData(8, 0.005f, 0.005f, 0.05f, 0.0005f) },
            { PCSSUtilities.PCSSQuality.Medium, new PCSSQualityData(16, 0.01f, 0.01f, 0.1f, 0.001f) },
            { PCSSUtilities.PCSSQuality.High, new PCSSQualityData(32, 0.015f, 0.015f, 0.15f, 0.0015f) },
            { PCSSUtilities.PCSSQuality.Ultra, new PCSSQualityData(64, 0.02f, 0.02f, 0.2f, 0.002f) }
        };
        
        public PoiyomiPCSSSettings Settings => settings;
        public List<Material> TargetMaterials => targetMaterials;
        public List<Renderer> TargetRenderers => targetRenderers;
        
        private void Start()
        {
            InitializePCSSIntegration();
        }
        
        private void Update()
        {
            if (settings.enableVRChatExpression)
            {
                UpdateVRChatExpressionParameters();
            }
        }
        
        /// <summary>
        /// Initialize PCSS integration
        /// </summary>
        public void InitializePCSSIntegration()
        {
            if (settings.autoDetectPoiyomiShader)
            {
                AutoDetectPoiyomiMaterials();
            }
            
            ApplyPCSSSettings();
            
            if (settings.enableVRChatExpression)
            {
                InitializeVRChatExpressionParameters();
            }
        }
        
        /// <summary>
        /// Auto-detect Poiyomi materials with PCSS support
        /// </summary>
        public void AutoDetectPoiyomiMaterials()
        {
            targetMaterials.Clear();
            targetRenderers.Clear();
            
            // Get all renderers in this GameObject and children
            var renderers = GetComponentsInChildren<Renderer>(true);
            
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    if (IsPoiyomiPCSSMaterial(material))
                    {
                        if (!targetMaterials.Contains(material))
                        {
                            targetMaterials.Add(material);
                        }
                        
                        if (!targetRenderers.Contains(renderer))
                        {
                            targetRenderers.Add(renderer);
                        }
                    }
                }
            }
            
            Debug.Log($"[PoiyomiPCSSIntegration] Auto-detected {targetMaterials.Count} PCSS materials on {targetRenderers.Count} renderers");
        }
        
        /// <summary>
        /// Check if material is a Poiyomi PCSS material
        /// </summary>
        private bool IsPoiyomiPCSSMaterial(Material material)
        {
            if (material == null || material.shader == null) return false;
            
            string shaderName = material.shader.name;
            return shaderName.Contains("Poiyomi") && shaderName.Contains("PCSS");
        }
        
        /// <summary>
        /// Apply PCSS settings to all target materials
        /// </summary>
        public void ApplyPCSSSettings()
        {
            foreach (var material in targetMaterials)
            {
                if (material == null) continue;
                
                ApplyPCSSSettingsToMaterial(material);
            }
        }
        
        /// <summary>
        /// Apply PCSS settings to a specific material
        /// </summary>
        public void ApplyPCSSSettingsToMaterial(Material material)
        {
            if (material == null) return;
            
            // Apply basic settings
            material.SetFloat("_PCSSEnabled", settings.enablePCSS ? 1f : 0f);
            material.SetFloat("_PCSSQuality", (float)settings.quality);
            
            // Apply quality preset or manual settings
            if (qualityPresets.ContainsKey(settings.quality))
            {
                var preset = qualityPresets[settings.quality];
                material.SetFloat("_PCSSSampleCount", preset.sampleCount);
                material.SetFloat("_PCSSBlockerSearchRadius", preset.blockerSearchRadius);
                material.SetFloat("_PCSSFilterRadius", preset.filterRadius);
                material.SetFloat("_PCSSLightSize", preset.lightSize);
                material.SetFloat("_PCSSShadowBias", preset.shadowBias);
            }
            else
            {
                // Apply manual settings
                material.SetFloat("_PCSSSampleCount", settings.sampleCount);
                material.SetFloat("_PCSSBlockerSearchRadius", settings.blockerSearchRadius);
                material.SetFloat("_PCSSFilterRadius", settings.filterRadius);
                material.SetFloat("_PCSSLightSize", settings.lightSize);
                material.SetFloat("_PCSSShadowBias", settings.shadowBias);
            }
            
            // VRChat Expression settings
            material.SetFloat("_VRChatExpression", settings.enableVRChatExpression ? 1f : 0f);
        }
        
        /// <summary>
        /// Initialize VRChat expression parameters
        /// </summary>
        private void InitializeVRChatExpressionParameters()
        {
            expressionParameters.Clear();
            
            // Initialize default values
            expressionParameters[$"{settings.parameterPrefix}_Enable"] = settings.enablePCSS ? 1f : 0f;
            expressionParameters[$"{settings.parameterPrefix}_Quality"] = (float)settings.quality / 3f; // Normalize to 0-1
            
            Debug.Log($"[PoiyomiPCSSIntegration] Initialized VRChat expression parameters with prefix: {settings.parameterPrefix}");
        }
        
        /// <summary>
        /// Update VRChat expression parameters
        /// </summary>
        private void UpdateVRChatExpressionParameters()
        {
            // This would integrate with VRChat's parameter system
            // For now, we simulate parameter updates
            
            bool enableParam = GetVRChatParameter($"{settings.parameterPrefix}_Enable", settings.enablePCSS ? 1f : 0f) > 0.5f;
            float qualityParam = GetVRChatParameter($"{settings.parameterPrefix}_Quality", (float)settings.quality / 3f);
            
            // Update materials with expression parameters
            foreach (var material in targetMaterials)
            {
                if (material == null) continue;
                
                material.SetFloat("_PCSSToggleParam", enableParam ? 1f : 0f);
                material.SetFloat("_PCSSQualityParam", qualityParam);
            }
        }
        
        /// <summary>
        /// Get VRChat parameter value (placeholder for actual VRChat integration)
        /// </summary>
        private float GetVRChatParameter(string parameterName, float defaultValue)
        {
            // In actual VRChat integration, this would get the parameter from the avatar
            // For now, return the stored value or default
            return expressionParameters.ContainsKey(parameterName) ? 
                   expressionParameters[parameterName] : defaultValue;
        }
        
        /// <summary>
        /// Set VRChat parameter value (for testing/simulation)
        /// </summary>
        public void SetVRChatParameter(string parameterName, float value)
        {
            expressionParameters[parameterName] = value;
        }
        
        /// <summary>
        /// Toggle PCSS on/off
        /// </summary>
        public void TogglePCSS()
        {
            settings.enablePCSS = !settings.enablePCSS;
            ApplyPCSSSettings();
            
            if (settings.enableVRChatExpression)
            {
                SetVRChatParameter($"{settings.parameterPrefix}_Enable", settings.enablePCSS ? 1f : 0f);
            }
        }
        
        /// <summary>
        /// Set PCSS quality
        /// </summary>
        public void SetPCSSQuality(PCSSUtilities.PCSSQuality quality)
        {
            settings.quality = quality;
            ApplyPCSSSettings();
            
            if (settings.enableVRChatExpression)
            {
                SetVRChatParameter($"{settings.parameterPrefix}_Quality", (float)quality / 3f);
            }
        }
        
        /// <summary>
        /// Get current PCSS status
        /// </summary>
        public bool IsPCSSEnabled()
        {
            return settings.enablePCSS;
        }
        
        /// <summary>
        /// Get current PCSS quality
        /// </summary>
        public PCSSUtilities.PCSSQuality GetPCSSQuality()
        {
            return settings.quality;
        }
        
        /// <summary>
        /// Add material to target list
        /// </summary>
        public void AddTargetMaterial(Material material)
        {
            if (material != null && !targetMaterials.Contains(material))
            {
                targetMaterials.Add(material);
                ApplyPCSSSettingsToMaterial(material);
            }
        }
        
        /// <summary>
        /// Remove material from target list
        /// </summary>
        public void RemoveTargetMaterial(Material material)
        {
            targetMaterials.Remove(material);
        }
        
        /// <summary>
        /// Clear all target materials
        /// </summary>
        public void ClearTargetMaterials()
        {
            targetMaterials.Clear();
            targetRenderers.Clear();
        }
        
        // Unity Editor integration
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/lilToon PCSS/Add Poiyomi PCSS Integration", false, 10)]
        private static void AddPoiyomiPCSSIntegration()
        {
            var selected = UnityEditor.Selection.activeGameObject;
            if (selected != null)
            {
                var integration = selected.GetComponent<PoiyomiPCSSIntegration>();
                if (integration == null)
                {
                    integration = selected.AddComponent<PoiyomiPCSSIntegration>();
                    integration.InitializePCSSIntegration();
                    
                    UnityEditor.EditorUtility.SetDirty(selected);
                    Debug.Log($"Added Poiyomi PCSS Integration to {selected.name}");
                }
                else
                {
                    Debug.LogWarning($"Poiyomi PCSS Integration already exists on {selected.name}");
                }
            }
        }
        
        [UnityEditor.MenuItem("GameObject/lilToon PCSS/Add Poiyomi PCSS Integration", true)]
        private static bool ValidateAddPoiyomiPCSSIntegration()
        {
            return UnityEditor.Selection.activeGameObject != null;
        }
        #endif
    }
    
    /// <summary>
    /// PCSS Quality data structure
    /// </summary>
    [System.Serializable]
    public struct PCSSQualityData
    {
        public int sampleCount;
        public float blockerSearchRadius;
        public float filterRadius;
        public float lightSize;
        public float shadowBias;
        
        public PCSSQualityData(int sampleCount, float blockerSearchRadius, float filterRadius, 
                              float lightSize, float shadowBias)
        {
            this.sampleCount = sampleCount;
            this.blockerSearchRadius = blockerSearchRadius;
            this.filterRadius = filterRadius;
            this.lightSize = lightSize;
            this.shadowBias = shadowBias;
        }
    }
} 