using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDK3.Editor;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// VRChatアップロード時の自動バックアップ機能
    /// なんｊ風に言うと「これでVRChatアップロード時のマテリアル消失を完全に防げるぜ！」💪🔥
    /// </summary>
    [InitializeOnLoad]
    public class VRChatUploadAutoBackup
    {
        private static readonly string BackupSubfolder = "VRChatMaterialBackups";
        private static readonly string AutoBackupFileName = "vrchat_auto_backup.json";
        private static readonly string BackupVersion = "2.2.0";
        
        private static bool isUploading = false;
        private static GameObject lastUploadedAvatar = null;
        private static DateTime lastBackupTime = DateTime.MinValue;
        
        static VRChatUploadAutoBackup()
        {
            // VRChat SDKのイベントをフック
            EditorApplication.update += CheckForVRChatUpload;
            
            // アップロード前の自動バックアップ
            VRCUpload.OnUploadStart += OnVRChatUploadStart;
            VRCUpload.OnUploadComplete += OnVRChatUploadComplete;
            VRCUpload.OnUploadError += OnVRChatUploadError;
            
            Debug.Log("[VRChatUploadAutoBackup] Auto backup system initialized");
        }
        
        private static void CheckForVRChatUpload()
        {
            // VRChat SDKのアップロード状態を監視
            if (VRCUpload.IsUploading && !isUploading)
            {
                isUploading = true;
                OnVRChatUploadStart();
            }
            else if (!VRCUpload.IsUploading && isUploading)
            {
                isUploading = false;
                OnVRChatUploadComplete();
            }
        }
        
        private static void OnVRChatUploadStart()
        {
            try
            {
                Debug.Log("[VRChatUploadAutoBackup] VRChat upload started - creating automatic backup");
                
                // アップロード対象のアバターを検索
                var avatars = FindObjectsOfType<VRCAvatarDescriptor>();
                if (avatars.Length > 0)
                {
                    var avatar = avatars[0];
                    lastUploadedAvatar = avatar.gameObject;
                    
                    // 自動バックアップを作成
                    CreateAutomaticBackup(avatar.gameObject);
                    
                    // バックアップ時間を記録
                    lastBackupTime = DateTime.Now;
                    
                    Debug.Log($"[VRChatUploadAutoBackup] Automatic backup created for {avatar.name}");
                }
                else
                {
                    Debug.LogWarning("[VRChatUploadAutoBackup] No VRChat avatar found in scene");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[VRChatUploadAutoBackup] Error during upload start backup: {e.Message}");
            }
        }
        
        private static void OnVRChatUploadComplete()
        {
            try
            {
                Debug.Log("[VRChatUploadAutoBackup] VRChat upload completed");
                
                if (lastUploadedAvatar != null)
                {
                    // アップロード完了後のマテリアル状態をチェック
                    CheckMaterialStateAfterUpload(lastUploadedAvatar);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[VRChatUploadAutoBackup] Error during upload complete: {e.Message}");
            }
        }
        
        private static void OnVRChatUploadError()
        {
            try
            {
                Debug.LogWarning("[VRChatUploadAutoBackup] VRChat upload failed");
                
                if (lastUploadedAvatar != null)
                {
                    // アップロード失敗時の復元オプションを提供
                    ShowRestoreOption(lastUploadedAvatar);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[VRChatUploadAutoBackup] Error during upload error handling: {e.Message}");
            }
        }
        
        private static void CreateAutomaticBackup(GameObject avatar)
        {
            try
            {
                string backupPath = GetBackupFolderPath(avatar.name);
                var backupData = new VRChatAutoBackupData
                {
                    avatarName = avatar.name,
                    backupTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    version = BackupVersion,
                    uploadType = "automatic",
                    materials = new List<MaterialBackupEntry>()
                };
                
                // すべてのRendererを取得
                var renderers = avatar.GetComponentsInChildren<Renderer>(true);
                int totalMaterials = 0;
                int backedUpMaterials = 0;
                
                foreach (var renderer in renderers)
                {
                    for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                    {
                        totalMaterials++;
                        var material = renderer.sharedMaterials[i];
                        
                        if (material != null)
                        {
                            var entry = CreateMaterialBackupEntry(material, renderer, i);
                            backupData.materials.Add(entry);
                            backedUpMaterials++;
                        }
                    }
                }
                
                // JSONとして保存
                string json = JsonUtility.ToJson(backupData, true);
                string filePath = Path.Combine(backupPath, AutoBackupFileName);
                File.WriteAllText(filePath, json);
                
                AssetDatabase.SaveAssets();
                
                Debug.Log($"[VRChatUploadAutoBackup] Automatic backup created: {backedUpMaterials}/{totalMaterials} materials");
                
                // 成功通知
                EditorUtility.DisplayDialog("Auto Backup Created", 
                    $"Automatic material backup created before VRChat upload!\n" +
                    $"Avatar: {avatar.name}\n" +
                    $"Materials backed up: {backedUpMaterials}/{totalMaterials}", "OK");
            }
            catch (Exception e)
            {
                Debug.LogError($"[VRChatUploadAutoBackup] Error creating automatic backup: {e.Message}");
            }
        }
        
        private static MaterialBackupEntry CreateMaterialBackupEntry(Material material, Renderer renderer, int materialIndex)
        {
            var entry = new MaterialBackupEntry
            {
                materialName = material.name,
                materialGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(material)),
                materialPath = AssetDatabase.GetAssetPath(material),
                shaderName = material.shader.name,
                shaderGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(material.shader)),
                rendererName = renderer.name,
                rendererPath = GetGameObjectPath(renderer.gameObject),
                materialIndex = materialIndex,
                properties = new Dictionary<string, string>(),
                textureGUIDs = new Dictionary<string, string>()
            };
            
            // シェーダープロパティをバックアップ
            var shader = material.shader;
            int propertyCount = ShaderUtil.GetPropertyCount(shader);
            
            for (int i = 0; i < propertyCount; i++)
            {
                string propertyName = ShaderUtil.GetPropertyName(shader, i);
                var propertyType = ShaderUtil.GetPropertyType(shader, i);
                
                switch (propertyType)
                {
                    case ShaderUtil.ShaderPropertyType.Color:
                        entry.properties[propertyName] = ColorUtility.ToHtmlStringRGBA(material.GetColor(propertyName));
                        break;
                    case ShaderUtil.ShaderPropertyType.Vector:
                        entry.properties[propertyName] = material.GetVector(propertyName).ToString();
                        break;
                    case ShaderUtil.ShaderPropertyType.Float:
                    case ShaderUtil.ShaderPropertyType.Range:
                        entry.properties[propertyName] = material.GetFloat(propertyName).ToString();
                        break;
                    case ShaderUtil.ShaderPropertyType.TexEnv:
                        var texture = material.GetTexture(propertyName);
                        if (texture != null)
                        {
                            string texturePath = AssetDatabase.GetAssetPath(texture);
                            entry.textureGUIDs[propertyName] = AssetDatabase.AssetPathToGUID(texturePath);
                        }
                        break;
                }
            }
            
            return entry;
        }
        
        private static void CheckMaterialStateAfterUpload(GameObject avatar)
        {
            try
            {
                var missingMaterials = new List<string>();
                var renderers = avatar.GetComponentsInChildren<Renderer>(true);
                
                foreach (var renderer in renderers)
                {
                    for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                    {
                        if (renderer.sharedMaterials[i] == null)
                        {
                            missingMaterials.Add($"{renderer.name} (Material {i})");
                        }
                    }
                }
                
                if (missingMaterials.Count > 0)
                {
                    string message = $"Upload completed, but {missingMaterials.Count} materials are missing:\n\n" + 
                                   string.Join("\n", missingMaterials) + 
                                   "\n\nWould you like to restore them from backup?";
                    
                    if (EditorUtility.DisplayDialog("Missing Materials After Upload", message, "Restore", "Cancel"))
                    {
                        RestoreMaterialsFromAutoBackup(avatar);
                    }
                }
                else
                {
                    Debug.Log("[VRChatUploadAutoBackup] All materials intact after upload");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[VRChatUploadAutoBackup] Error checking material state: {e.Message}");
            }
        }
        
        private static void ShowRestoreOption(GameObject avatar)
        {
            string message = "VRChat upload failed. Would you like to restore materials from the automatic backup?";
            
            if (EditorUtility.DisplayDialog("Upload Failed - Restore Materials?", message, "Restore", "Cancel"))
            {
                RestoreMaterialsFromAutoBackup(avatar);
            }
        }
        
        private static void RestoreMaterialsFromAutoBackup(GameObject avatar)
        {
            try
            {
                string backupPath = GetBackupFolderPath(avatar.name);
                string filePath = Path.Combine(backupPath, AutoBackupFileName);
                
                if (!File.Exists(filePath))
                {
                    EditorUtility.DisplayDialog("Error", "No automatic backup found for this avatar", "OK");
                    return;
                }
                
                string json = File.ReadAllText(filePath);
                var backupData = JsonUtility.FromJson<VRChatAutoBackupData>(json);
                
                int restoredCount = 0;
                int totalMaterials = 0;
                
                var renderers = avatar.GetComponentsInChildren<Renderer>(true);
                
                foreach (var renderer in renderers)
                {
                    var materials = renderer.sharedMaterials;
                    bool changed = false;
                    
                    for (int i = 0; i < materials.Length; i++)
                    {
                        totalMaterials++;
                        
                        if (materials[i] == null)
                        {
                            // Missing material - try to restore
                            var backupEntry = backupData.materials.FirstOrDefault(m => 
                                m.rendererPath == GetGameObjectPath(renderer.gameObject) && 
                                m.materialIndex == i);
                            
                            if (backupEntry != null)
                            {
                                var restoredMaterial = RestoreMaterialFromBackup(backupEntry);
                                if (restoredMaterial != null)
                                {
                                    materials[i] = restoredMaterial;
                                    changed = true;
                                    restoredCount++;
                                    Debug.Log($"[VRChatUploadAutoBackup] Restored material: {backupEntry.materialName} on {renderer.name}");
                                }
                            }
                        }
                    }
                    
                    if (changed)
                    {
                        Undo.RecordObject(renderer, "Restore Materials from Auto Backup");
                        renderer.sharedMaterials = materials;
                        EditorUtility.SetDirty(renderer);
                    }
                }
                
                AssetDatabase.SaveAssets();
                
                EditorUtility.DisplayDialog("Restore Complete", 
                    $"Materials restored from automatic backup!\n" +
                    $"Materials restored: {restoredCount}/{totalMaterials}", "OK");
                
                Debug.Log($"[VRChatUploadAutoBackup] Restored {restoredCount}/{totalMaterials} materials from auto backup");
            }
            catch (Exception e)
            {
                Debug.LogError($"[VRChatUploadAutoBackup] Error restoring materials: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to restore materials: {e.Message}", "OK");
            }
        }
        
        private static Material RestoreMaterialFromBackup(MaterialBackupEntry backupEntry)
        {
            // GUIDからマテリアルを復元
            string materialPath = AssetDatabase.GUIDToAssetPath(backupEntry.materialGUID);
            if (!string.IsNullOrEmpty(materialPath))
            {
                var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                if (material != null)
                {
                    // シェーダーを復元
                    string shaderPath = AssetDatabase.GUIDToAssetPath(backupEntry.shaderGUID);
                    if (!string.IsNullOrEmpty(shaderPath))
                    {
                        var shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
                        if (shader != null)
                        {
                            material.shader = shader;
                        }
                    }
                    
                    // プロパティを復元
                    foreach (var kvp in backupEntry.properties)
                    {
                        if (kvp.Key.StartsWith("_Color"))
                        {
                            if (ColorUtility.TryParseHtmlString(kvp.Value, out Color color))
                            {
                                material.SetColor(kvp.Key, color);
                            }
                        }
                        else if (kvp.Key.StartsWith("_Vector"))
                        {
                            material.SetVector(kvp.Key, StringToVector4(kvp.Value));
                        }
                        else if (kvp.Key.StartsWith("_Float") || kvp.Key.StartsWith("_Range"))
                        {
                            if (float.TryParse(kvp.Value, out float value))
                            {
                                material.SetFloat(kvp.Key, value);
                            }
                        }
                    }
                    
                    // テクスチャを復元
                    foreach (var kvp in backupEntry.textureGUIDs)
                    {
                        string texturePath = AssetDatabase.GUIDToAssetPath(kvp.Value);
                        if (!string.IsNullOrEmpty(texturePath))
                        {
                            var texture = AssetDatabase.LoadAssetAtPath<Texture>(texturePath);
                            if (texture != null)
                            {
                                material.SetTexture(kvp.Key, texture);
                            }
                        }
                    }
                    
                    return material;
                }
            }
            
            return null;
        }
        
        private static string GetBackupFolderPath(string avatarName)
        {
            string backupRootPath = Path.Combine("Assets", BackupSubfolder);
            if (!Directory.Exists(backupRootPath))
            {
                Directory.CreateDirectory(backupRootPath);
            }
            
            string avatarBackupPath = Path.Combine(backupRootPath, avatarName);
            if (!Directory.Exists(avatarBackupPath))
            {
                Directory.CreateDirectory(avatarBackupPath);
            }
            
            return avatarBackupPath;
        }
        
        private static string GetGameObjectPath(GameObject obj)
        {
            string path = obj.name;
            Transform parent = obj.transform.parent;
            
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            
            return path;
        }
        
        private static Vector4 StringToVector4(string s)
        {
            s = s.Trim('(', ')');
            var parts = s.Split(',');
            if (parts.Length == 4)
            {
                float.TryParse(parts[0], out float x);
                float.TryParse(parts[1], out float y);
                float.TryParse(parts[2], out float z);
                float.TryParse(parts[3], out float w);
                return new Vector4(x, y, z, w);
            }
            return Vector4.zero;
        }
    }
    
    [System.Serializable]
    public class VRChatAutoBackupData
    {
        public string avatarName;
        public string backupTime;
        public string version;
        public string uploadType;
        public List<MaterialBackupEntry> materials;
    }
} 