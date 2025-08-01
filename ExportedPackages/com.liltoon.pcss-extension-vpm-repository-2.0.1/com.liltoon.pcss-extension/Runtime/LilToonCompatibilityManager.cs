using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LilToon.PCSS.Runtime
{
    /// <summary>
    /// lilToon縺ｨ縺ｮ螳悟・莠呈鋤諤ｧ繧堤ｮ｡逅・☆繧九け繝ｩ繧ｹ
    /// 閾ｪ蜍輔ヰ繝ｼ繧ｸ繝ｧ繝ｳ讀懷・縲√・繝・Μ繧｢繝ｫ蜷梧悄縲√・繝ｭ繝代ユ繧｣繝槭ャ繝斐Φ繧ｰ繧呈署萓・
    /// </summary>
    public class LilToonCompatibilityManager : MonoBehaviour
    {
        [Header("lilToon Version Detection")]
        public string detectedLilToonVersion = "Not Detected";
        public bool isCompatible = false;
        
        [Header("Material Synchronization")]
        public bool autoSyncMaterials = true;
        public bool preserveOriginalColors = true;
        public bool autoUpdateOnVersionChange = true;
        
        [Header("Compatibility Settings")]
        public List<Material> managedMaterials = new List<Material>();
        
        // lilToon繝励Ο繝代ユ繧｣繝槭ャ繝斐Φ繧ｰ
        private static readonly Dictionary<string, string> PropertyMappings = new Dictionary<string, string>
        {
            // 蝓ｺ譛ｬ濶ｲ繝励Ο繝代ユ繧｣
            {"_Color", "_MainColor"},
            {"_MainTex", "_MainTex"},
            {"_MainTex_ST", "_MainTex_ST"},
            
            // 蠖ｱ繝励Ο繝代ユ繧｣
            {"_ShadowColor", "_ShadowColor"},
            {"_ShadowColorTex", "_ShadowColorTex"},
            {"_ShadowBorder", "_ShadowBorder"},
            {"_ShadowBlur", "_ShadowBlur"},
            
            // 繧｢繧ｦ繝医Λ繧､繝ｳ
            {"_OutlineColor", "_OutlineColor"},
            {"_OutlineWidth", "_OutlineWidth"},
            {"_OutlineTex", "_OutlineTex"},
            
            // 豕慕ｷ壹・繝・・
            {"_BumpMap", "_BumpMap"},
            {"_BumpScale", "_BumpScale"},
            
            // 繧ｨ繝溘ャ繧ｷ繝ｧ繝ｳ
            {"_EmissionColor", "_EmissionColor"},
            {"_EmissionMap", "_EmissionMap"},
            
            // PCSS蝗ｺ譛峨・繝ｭ繝代ユ繧｣
            {"_PCSSEnabled", "_PCSSEnabled"},
            {"_PCSSLightSize", "_PCSSLightSize"},
            {"_PCSSMaxDistance", "_PCSSMaxDistance"},
            {"_PCSSFilterRadius", "_PCSSFilterRadius"}
        };
        
        private void Start()
        {
            DetectLilToonVersion();
            if (autoSyncMaterials && isCompatible)
            {
                SynchronizeAllMaterials();
            }
        }
        
        /// <summary>
        /// lilToon縺ｮ繝舌・繧ｸ繝ｧ繝ｳ繧定・蜍墓､懷・
        /// </summary>
        public void DetectLilToonVersion()
        {
            try
            {
                // lilToon縺ｮ繧｢繧ｻ繝ｳ繝悶Μ繧呈､懃ｴ｢
                var lilToonAssembly = System.AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name.Contains("lilToon"));
                
                if (lilToonAssembly != null)
                {
                    // 繝舌・繧ｸ繝ｧ繝ｳ諠・ｱ繧貞叙蠕・
                    var versionAttribute = lilToonAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
                    if (versionAttribute != null)
                    {
                        detectedLilToonVersion = versionAttribute.Version;
                        isCompatible = CheckCompatibility(detectedLilToonVersion);
                    }
                    else
                    {
                        // 繝輔か繝ｼ繝ｫ繝舌ャ繧ｯ: 繧ｷ繧ｧ繝ｼ繝繝ｼ縺九ｉ讀懷・
                        DetectVersionFromShader();
                    }
                }
                else
                {
                    detectedLilToonVersion = "Not Found";
                    isCompatible = false;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"lilToon version detection failed: {ex.Message}");
                DetectVersionFromShader();
            }
        }
        
        /// <summary>
        /// 繧ｷ繧ｧ繝ｼ繝繝ｼ縺九ｉ繝舌・繧ｸ繝ｧ繝ｳ繧呈､懷・・医ヵ繧ｩ繝ｼ繝ｫ繝舌ャ繧ｯ・・
        /// </summary>
        private void DetectVersionFromShader()
        {
            var lilToonShader = Shader.Find("lilToon");
            if (lilToonShader != null)
            {
#if UNITY_EDITOR
                // 繧ｨ繝・ぅ繧ｿ繝ｼ迺ｰ蠅・〒縺ｮ縺ｿ繧ｷ繧ｧ繝ｼ繝繝ｼ繝励Ο繝代ユ繧｣繧定ｩｳ邏ｰ繝√ぉ繝・け
                var propertyCount = ShaderUtil.GetPropertyCount(lilToonShader);
                
                // v1.7.0莉･髯阪・迚ｹ蠕ｴ逧・・繝ｭ繝代ユ繧｣繧偵メ繧ｧ繝・け
                bool hasNewProperties = false;
                for (int i = 0; i < propertyCount; i++)
                {
                    var propName = ShaderUtil.GetPropertyName(lilToonShader, i);
                    if (propName.Contains("_lilToonVersion") || propName.Contains("_ShadowBorderRange"))
                    {
                        hasNewProperties = true;
                        break;
                    }
                }
                
                detectedLilToonVersion = hasNewProperties ? "1.7.0+" : "1.6.x or older";
                isCompatible = hasNewProperties;
#else
                // 繝ｩ繝ｳ繧ｿ繧､繝迺ｰ蠅・〒縺ｯ蝓ｺ譛ｬ逧・↑讀懷・縺ｮ縺ｿ
                detectedLilToonVersion = "1.7.0+ (Runtime Detection)";
                isCompatible = true; // lilToon縺瑚ｦ九▽縺九▲縺溷ｴ蜷医・莠呈鋤諤ｧ縺ゅｊ縺ｨ莉ｮ螳・
#endif
            }
            else
            {
                detectedLilToonVersion = "Shader Not Found";
                isCompatible = false;
            }
        }
        
        /// <summary>
        /// 繝舌・繧ｸ繝ｧ繝ｳ莠呈鋤諤ｧ繧偵メ繧ｧ繝・け
        /// </summary>
        private bool CheckCompatibility(string version)
        {
            try
            {
                var versionParts = version.Split('.');
                if (versionParts.Length >= 2)
                {
                    int major = int.Parse(versionParts[0]);
                    int minor = int.Parse(versionParts[1]);
                    
                    // lilToon 1.7.0莉･髯阪ｒ繧ｵ繝昴・繝・
                    return (major > 1) || (major == 1 && minor >= 7);
                }
            }
            catch
            {
                return false;
            }
            
            return false;
        }
        
        /// <summary>
        /// 蜈ｨ縺ｦ縺ｮ繝槭ユ繝ｪ繧｢繝ｫ繧貞酔譛・
        /// </summary>
        public void SynchronizeAllMaterials()
        {
            foreach (var material in managedMaterials)
            {
                if (material != null)
                {
                    SynchronizeMaterial(material);
                }
            }
        }
        
        /// <summary>
        /// 蛟句挨繝槭ユ繝ｪ繧｢繝ｫ縺ｮ蜷梧悄
        /// </summary>
        public void SynchronizeMaterial(Material material)
        {
            if (material == null || !isCompatible) return;
            
            var originalShader = material.shader;
            var pcssShader = Shader.Find("lilToon/PCSS Extension");
            
            if (pcssShader == null)
            {
                Debug.LogError("PCSS Extension shader not found!");
                return;
            }
            
            // 繝励Ο繝代ユ繧｣蛟､繧剃ｿ晏ｭ・
            var propertyValues = new Dictionary<string, object>();
            
            // 蜈・・繝励Ο繝代ユ繧｣繧剃ｿ晏ｭ・
            foreach (var mapping in PropertyMappings)
            {
                if (material.HasProperty(mapping.Key))
                {
                    SavePropertyValue(material, mapping.Key, propertyValues);
                }
            }
            
            // 繧ｷ繧ｧ繝ｼ繝繝ｼ繧貞､画峩
            material.shader = pcssShader;
            
            // 繝励Ο繝代ユ繧｣繧貞ｾｩ蜈・
            foreach (var mapping in PropertyMappings)
            {
                if (propertyValues.ContainsKey(mapping.Key))
                {
                    RestorePropertyValue(material, mapping.Value, propertyValues[mapping.Key]);
                }
            }
            
            // PCSS蝗ｺ譛峨・蛻晄悄險ｭ螳・
            SetupPCSSDefaults(material);
            
            Debug.Log($"Material '{material.name}' synchronized with lilToon compatibility");
        }
        
        /// <summary>
        /// 繝励Ο繝代ユ繧｣蛟､繧剃ｿ晏ｭ・
        /// </summary>
        private void SavePropertyValue(Material material, string propertyName, Dictionary<string, object> storage)
        {
#if UNITY_EDITOR
            var propertyType = material.shader.GetPropertyType(material.shader.FindPropertyIndex(propertyName));
            
            switch (propertyType)
            {
                case UnityEngine.Rendering.ShaderPropertyType.Color:
                    storage[propertyName] = material.GetColor(propertyName);
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Vector:
                    storage[propertyName] = material.GetVector(propertyName);
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Float:
                case UnityEngine.Rendering.ShaderPropertyType.Range:
                    storage[propertyName] = material.GetFloat(propertyName);
                    break;
                case UnityEngine.Rendering.ShaderPropertyType.Texture:
                    storage[propertyName] = material.GetTexture(propertyName);
                    break;
            }
#else
            // 繝ｩ繝ｳ繧ｿ繧､繝縺ｧ縺ｯ蝙九ｒ謗ｨ貂ｬ縺励※菫晏ｭ・
            try
            {
                if (material.HasProperty(propertyName))
                {
                    // 譛繧ゆｸ闊ｬ逧・↑繝励Ο繝代ユ繧｣繧ｿ繧､繝励ｒ隧ｦ陦・
                    if (propertyName.ToLower().Contains("color"))
                        storage[propertyName] = material.GetColor(propertyName);
                    else if (propertyName.ToLower().Contains("tex"))
                        storage[propertyName] = material.GetTexture(propertyName);
                    else
                        storage[propertyName] = material.GetFloat(propertyName);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Failed to save property {propertyName}: {ex.Message}");
            }
#endif
        }
        
        /// <summary>
        /// 繝励Ο繝代ユ繧｣蛟､繧貞ｾｩ蜈・
        /// </summary>
        private void RestorePropertyValue(Material material, string propertyName, object value)
        {
            if (!material.HasProperty(propertyName)) return;
            
            try
            {
                switch (value)
                {
                    case Color colorValue:
                        material.SetColor(propertyName, colorValue);
                        break;
                    case Vector4 vectorValue:
                        material.SetVector(propertyName, vectorValue);
                        break;
                    case float floatValue:
                        material.SetFloat(propertyName, floatValue);
                        break;
                    case Texture textureValue:
                        material.SetTexture(propertyName, textureValue);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Failed to restore property {propertyName}: {ex.Message}");
            }
        }
        
        /// <summary>
        /// PCSS蝗ｺ譛峨・蛻晄悄險ｭ螳・
        /// </summary>
        private void SetupPCSSDefaults(Material material)
        {
            // PCSS蝓ｺ譛ｬ險ｭ螳・
            if (material.HasProperty("_PCSSEnabled"))
                material.SetFloat("_PCSSEnabled", 1.0f);
            
            if (material.HasProperty("_PCSSLightSize"))
                material.SetFloat("_PCSSLightSize", 0.5f);
            
            if (material.HasProperty("_PCSSMaxDistance"))
                material.SetFloat("_PCSSMaxDistance", 10.0f);
            
            if (material.HasProperty("_PCSSFilterRadius"))
                material.SetFloat("_PCSSFilterRadius", 3.0f);
            
            // VRChat譛驕ｩ蛹冶ｨｭ螳・
            if (material.HasProperty("_VRChatOptimization"))
                material.SetFloat("_VRChatOptimization", 1.0f);
            
            // Quest蟇ｾ蠢懆ｨｭ螳・
            if (material.HasProperty("_QuestCompatible"))
                material.SetFloat("_QuestCompatible", 1.0f);
        }
        
        /// <summary>
        /// 繝槭ユ繝ｪ繧｢繝ｫ繧堤ｮ｡逅・Μ繧ｹ繝医↓霑ｽ蜉
        /// </summary>
        public void AddMaterialToManagement(Material material)
        {
            if (material != null && !managedMaterials.Contains(material))
            {
                managedMaterials.Add(material);
                if (autoSyncMaterials && isCompatible)
                {
                    SynchronizeMaterial(material);
                }
            }
        }
        
        /// <summary>
        /// 閾ｪ蜍輔い繝・・繝・・繝域ｩ溯・
        /// </summary>
        public void CheckForUpdates()
        {
            var currentVersion = detectedLilToonVersion;
            DetectLilToonVersion();
            
            if (currentVersion != detectedLilToonVersion && autoUpdateOnVersionChange)
            {
                Debug.Log($"lilToon version changed: {currentVersion} -> {detectedLilToonVersion}");
                SynchronizeAllMaterials();
            }
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                DetectLilToonVersion();
            }
        }
    }
} 
