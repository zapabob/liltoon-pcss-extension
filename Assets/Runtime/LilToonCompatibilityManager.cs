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
    /// lilToonã¨ã®å®åEäºææ§ãç®¡çEãã¯ã©ã¹
    /// èªåãã¼ã¸ã§ã³æ¤åEããEãEªã¢ã«åæããEã­ããã£ãããã³ã°ãæä¾E
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
        
        // lilToonãã­ããã£ãããã³ã°
        private static readonly Dictionary<string, string> PropertyMappings = new Dictionary<string, string>
        {
            // åºæ¬è²ãã­ããã£
            {"_Color", "_MainColor"},
            {"_MainTex", "_MainTex"},
            {"_MainTex_ST", "_MainTex_ST"},
            
            // å½±ãã­ããã£
            {"_ShadowColor", "_ShadowColor"},
            {"_ShadowColorTex", "_ShadowColorTex"},
            {"_ShadowBorder", "_ShadowBorder"},
            {"_ShadowBlur", "_ShadowBlur"},
            
            // ã¢ã¦ãã©ã¤ã³
            {"_OutlineColor", "_OutlineColor"},
            {"_OutlineWidth", "_OutlineWidth"},
            {"_OutlineTex", "_OutlineTex"},
            
            // æ³ç·ãEãEE
            {"_BumpMap", "_BumpMap"},
            {"_BumpScale", "_BumpScale"},
            
            // ã¨ããã·ã§ã³
            {"_EmissionColor", "_EmissionColor"},
            {"_EmissionMap", "_EmissionMap"},
            
            // PCSSåºæãEã­ããã£
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
        /// lilToonã®ããEã¸ã§ã³ãèEåæ¤åE
        /// </summary>
        public void DetectLilToonVersion()
        {
            try
            {
                // lilToonã®ã¢ã»ã³ããªãæ¤ç´¢
                var lilToonAssembly = System.AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name.Contains("lilToon"));
                
                if (lilToonAssembly != null)
                {
                    // ããEã¸ã§ã³æE ±ãåå¾E
                    var versionAttribute = lilToonAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
                    if (versionAttribute != null)
                    {
                        detectedLilToonVersion = versionAttribute.Version;
                        isCompatible = CheckCompatibility(detectedLilToonVersion);
                    }
                    else
                    {
                        // ãã©ã¼ã«ããã¯: ã·ã§ã¼ãã¼ããæ¤åE
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
        /// ã·ã§ã¼ãã¼ããããEã¸ã§ã³ãæ¤åEEãã©ã¼ã«ããã¯EE
        /// </summary>
        private void DetectVersionFromShader()
        {
            var lilToonShader = Shader.Find("lilToon");
            if (lilToonShader != null)
            {
#if UNITY_EDITOR
                // ã¨ãE£ã¿ã¼ç°å¢E§ã®ã¿ã·ã§ã¼ãã¼ãã­ããã£ãè©³ç´°ãã§ãE¯
                var propertyCount = ShaderUtil.GetPropertyCount(lilToonShader);
                
                // v1.7.0ä»¥éãEç¹å¾´çEEã­ããã£ããã§ãE¯
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
                // ã©ã³ã¿ã¤ã ç°å¢E§ã¯åºæ¬çEªæ¤åEã®ã¿
                detectedLilToonVersion = "1.7.0+ (Runtime Detection)";
                isCompatible = true; // lilToonãè¦ã¤ãã£ãå ´åãEäºææ§ããã¨ä»®å®E
#endif
            }
            else
            {
                detectedLilToonVersion = "Shader Not Found";
                isCompatible = false;
            }
        }
        
        /// <summary>
        /// ããEã¸ã§ã³äºææ§ããã§ãE¯
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
                    
                    // lilToon 1.7.0ä»¥éããµããEãE
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
        /// å¨ã¦ã®ãããªã¢ã«ãåæE
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
        /// åå¥ãããªã¢ã«ã®åæ
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
            
            // ãã­ããã£å¤ãä¿å­E
            var propertyValues = new Dictionary<string, object>();
            
            // åEEãã­ããã£ãä¿å­E
            foreach (var mapping in PropertyMappings)
            {
                if (material.HasProperty(mapping.Key))
                {
                    SavePropertyValue(material, mapping.Key, propertyValues);
                }
            }
            
            // ã·ã§ã¼ãã¼ãå¤æ´
            material.shader = pcssShader;
            
            // ãã­ããã£ãå¾©åE
            foreach (var mapping in PropertyMappings)
            {
                if (propertyValues.ContainsKey(mapping.Key))
                {
                    RestorePropertyValue(material, mapping.Value, propertyValues[mapping.Key]);
                }
            }
            
            // PCSSåºæãEåæè¨­å®E
            SetupPCSSDefaults(material);
            
            Debug.Log($"Material '{material.name}' synchronized with lilToon compatibility");
        }
        
        /// <summary>
        /// ãã­ããã£å¤ãä¿å­E
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
            // ã©ã³ã¿ã¤ã ã§ã¯åãæ¨æ¸¬ãã¦ä¿å­E
            try
            {
                if (material.HasProperty(propertyName))
                {
                    // æãä¸è¬çEªãã­ããã£ã¿ã¤ããè©¦è¡E
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
        /// ãã­ããã£å¤ãå¾©åE
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
        /// PCSSåºæãEåæè¨­å®E
        /// </summary>
        private void SetupPCSSDefaults(Material material)
        {
            // PCSSåºæ¬è¨­å®E
            if (material.HasProperty("_PCSSEnabled"))
                material.SetFloat("_PCSSEnabled", 1.0f);
            
            if (material.HasProperty("_PCSSLightSize"))
                material.SetFloat("_PCSSLightSize", 0.5f);
            
            if (material.HasProperty("_PCSSMaxDistance"))
                material.SetFloat("_PCSSMaxDistance", 10.0f);
            
            if (material.HasProperty("_PCSSFilterRadius"))
                material.SetFloat("_PCSSFilterRadius", 3.0f);
            
            // VRChatæé©åè¨­å®E
            if (material.HasProperty("_VRChatOptimization"))
                material.SetFloat("_VRChatOptimization", 1.0f);
            
            // Questå¯¾å¿è¨­å®E
            if (material.HasProperty("_QuestCompatible"))
                material.SetFloat("_QuestCompatible", 1.0f);
        }
        
        /// <summary>
        /// ãããªã¢ã«ãç®¡çEªã¹ãã«è¿½å 
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
        /// èªåã¢ãEEãEEãæ©èE
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
