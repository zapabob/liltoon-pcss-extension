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
    /// lilToonとの完�E互換性を管琁E��るクラス
    /// 自動バージョン検�E、�EチE��アル同期、�Eロパティマッピングを提侁E
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
        
        // lilToonプロパティマッピング
        private static readonly Dictionary<string, string> PropertyMappings = new Dictionary<string, string>
        {
            // 基本色プロパティ
            {"_Color", "_MainColor"},
            {"_MainTex", "_MainTex"},
            {"_MainTex_ST", "_MainTex_ST"},
            
            // 影プロパティ
            {"_ShadowColor", "_ShadowColor"},
            {"_ShadowColorTex", "_ShadowColorTex"},
            {"_ShadowBorder", "_ShadowBorder"},
            {"_ShadowBlur", "_ShadowBlur"},
            
            // アウトライン
            {"_OutlineColor", "_OutlineColor"},
            {"_OutlineWidth", "_OutlineWidth"},
            {"_OutlineTex", "_OutlineTex"},
            
            // 法線�EチE�E
            {"_BumpMap", "_BumpMap"},
            {"_BumpScale", "_BumpScale"},
            
            // エミッション
            {"_EmissionColor", "_EmissionColor"},
            {"_EmissionMap", "_EmissionMap"},
            
            // PCSS固有�Eロパティ
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
        /// lilToonのバ�Eジョンを�E動検�E
        /// </summary>
        public void DetectLilToonVersion()
        {
            try
            {
                // lilToonのアセンブリを検索
                var lilToonAssembly = System.AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name.Contains("lilToon"));
                
                if (lilToonAssembly != null)
                {
                    // バ�Eジョン惁E��を取征E
                    var versionAttribute = lilToonAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
                    if (versionAttribute != null)
                    {
                        detectedLilToonVersion = versionAttribute.Version;
                        isCompatible = CheckCompatibility(detectedLilToonVersion);
                    }
                    else
                    {
                        // フォールバック: シェーダーから検�E
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
        /// シェーダーからバ�Eジョンを検�E�E�フォールバック�E�E
        /// </summary>
        private void DetectVersionFromShader()
        {
            var lilToonShader = Shader.Find("lilToon");
            if (lilToonShader != null)
            {
#if UNITY_EDITOR
                // エチE��ター環墁E��のみシェーダープロパティを詳細チェチE��
                var propertyCount = ShaderUtil.GetPropertyCount(lilToonShader);
                
                // v1.7.0以降�E特徴皁E�EロパティをチェチE��
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
                // ランタイム環墁E��は基本皁E��検�Eのみ
                detectedLilToonVersion = "1.7.0+ (Runtime Detection)";
                isCompatible = true; // lilToonが見つかった場合�E互換性ありと仮宁E
#endif
            }
            else
            {
                detectedLilToonVersion = "Shader Not Found";
                isCompatible = false;
            }
        }
        
        /// <summary>
        /// バ�Eジョン互換性をチェチE��
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
                    
                    // lilToon 1.7.0以降をサポ�EチE
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
        /// 全てのマテリアルを同朁E
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
        /// 個別マテリアルの同期
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
            
            // プロパティ値を保孁E
            var propertyValues = new Dictionary<string, object>();
            
            // 允E�Eプロパティを保孁E
            foreach (var mapping in PropertyMappings)
            {
                if (material.HasProperty(mapping.Key))
                {
                    SavePropertyValue(material, mapping.Key, propertyValues);
                }
            }
            
            // シェーダーを変更
            material.shader = pcssShader;
            
            // プロパティを復允E
            foreach (var mapping in PropertyMappings)
            {
                if (propertyValues.ContainsKey(mapping.Key))
                {
                    RestorePropertyValue(material, mapping.Value, propertyValues[mapping.Key]);
                }
            }
            
            // PCSS固有�E初期設宁E
            SetupPCSSDefaults(material);
            
            Debug.Log($"Material '{material.name}' synchronized with lilToon compatibility");
        }
        
        /// <summary>
        /// プロパティ値を保孁E
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
            // ランタイムでは型を推測して保孁E
            try
            {
                if (material.HasProperty(propertyName))
                {
                    // 最も一般皁E��プロパティタイプを試衁E
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
        /// プロパティ値を復允E
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
        /// PCSS固有�E初期設宁E
        /// </summary>
        private void SetupPCSSDefaults(Material material)
        {
            // PCSS基本設宁E
            if (material.HasProperty("_PCSSEnabled"))
                material.SetFloat("_PCSSEnabled", 1.0f);
            
            if (material.HasProperty("_PCSSLightSize"))
                material.SetFloat("_PCSSLightSize", 0.5f);
            
            if (material.HasProperty("_PCSSMaxDistance"))
                material.SetFloat("_PCSSMaxDistance", 10.0f);
            
            if (material.HasProperty("_PCSSFilterRadius"))
                material.SetFloat("_PCSSFilterRadius", 3.0f);
            
            // VRChat最適化設宁E
            if (material.HasProperty("_VRChatOptimization"))
                material.SetFloat("_VRChatOptimization", 1.0f);
            
            // Quest対応設宁E
            if (material.HasProperty("_QuestCompatible"))
                material.SetFloat("_QuestCompatible", 1.0f);
        }
        
        /// <summary>
        /// マテリアルを管琁E��ストに追加
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
        /// 自動アチE�EチE�Eト機�E
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
