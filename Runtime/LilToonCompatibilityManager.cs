using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LilToon.PCSS.Runtime
{
    /// <summary>
    /// lilToonとの完全互換性を管理するクラス
    /// 自動バージョン検出、マテリアル同期、プロパティマッピングを提供
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
            
            // 法線マップ
            {"_BumpMap", "_BumpMap"},
            {"_BumpScale", "_BumpScale"},
            
            // エミッション
            {"_EmissionColor", "_EmissionColor"},
            {"_EmissionMap", "_EmissionMap"},
            
            // PCSS固有プロパティ
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
        /// lilToonのバージョンを自動検出
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
                    // バージョン情報を取得
                    var versionAttribute = lilToonAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
                    if (versionAttribute != null)
                    {
                        detectedLilToonVersion = versionAttribute.Version;
                        isCompatible = CheckCompatibility(detectedLilToonVersion);
                    }
                    else
                    {
                        // フォールバック: シェーダーから検出
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
        /// シェーダーからバージョンを検出（フォールバック）
        /// </summary>
        private void DetectVersionFromShader()
        {
            var lilToonShader = Shader.Find("lilToon");
            if (lilToonShader != null)
            {
                // シェーダーのプロパティから推測
                var propertyCount = ShaderUtil.GetPropertyCount(lilToonShader);
                
                // v1.7.0以降の特徴的プロパティをチェック
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
            }
            else
            {
                detectedLilToonVersion = "Shader Not Found";
                isCompatible = false;
            }
        }
        
        /// <summary>
        /// バージョン互換性をチェック
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
                    
                    // lilToon 1.7.0以降をサポート
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
        /// 全てのマテリアルを同期
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
            
            // プロパティ値を保存
            var propertyValues = new Dictionary<string, object>();
            
            // 元のプロパティを保存
            foreach (var mapping in PropertyMappings)
            {
                if (material.HasProperty(mapping.Key))
                {
                    SavePropertyValue(material, mapping.Key, propertyValues);
                }
            }
            
            // シェーダーを変更
            material.shader = pcssShader;
            
            // プロパティを復元
            foreach (var mapping in PropertyMappings)
            {
                if (propertyValues.ContainsKey(mapping.Key))
                {
                    RestorePropertyValue(material, mapping.Value, propertyValues[mapping.Key]);
                }
            }
            
            // PCSS固有の初期設定
            SetupPCSSDefaults(material);
            
            Debug.Log($"Material '{material.name}' synchronized with lilToon compatibility");
        }
        
        /// <summary>
        /// プロパティ値を保存
        /// </summary>
        private void SavePropertyValue(Material material, string propertyName, Dictionary<string, object> storage)
        {
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
        }
        
        /// <summary>
        /// プロパティ値を復元
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
        /// PCSS固有の初期設定
        /// </summary>
        private void SetupPCSSDefaults(Material material)
        {
            // PCSS基本設定
            if (material.HasProperty("_PCSSEnabled"))
                material.SetFloat("_PCSSEnabled", 1.0f);
            
            if (material.HasProperty("_PCSSLightSize"))
                material.SetFloat("_PCSSLightSize", 0.5f);
            
            if (material.HasProperty("_PCSSMaxDistance"))
                material.SetFloat("_PCSSMaxDistance", 10.0f);
            
            if (material.HasProperty("_PCSSFilterRadius"))
                material.SetFloat("_PCSSFilterRadius", 3.0f);
            
            // VRChat最適化設定
            if (material.HasProperty("_VRChatOptimization"))
                material.SetFloat("_VRChatOptimization", 1.0f);
            
            // Quest対応設定
            if (material.HasProperty("_QuestCompatible"))
                material.SetFloat("_QuestCompatible", 1.0f);
        }
        
        /// <summary>
        /// マテリアルを管理リストに追加
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
        /// 自動アップデート機能
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