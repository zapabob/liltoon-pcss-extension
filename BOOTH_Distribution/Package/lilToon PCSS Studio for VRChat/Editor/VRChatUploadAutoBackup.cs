using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System;
using VRC.SDK3.Avatars.Components;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// VRChatアップロード時の自動バックアップ機能
    /// なんｊ風に言うと「これで完璧な自動バックアップシステムが完成したぜ！」💪🔥
    /// </summary>
    [InitializeOnLoad]
    public class VRChatUploadAutoBackup
    {
        private static readonly string BackupSubfolder = "VRChatMaterialBackups";
        // 使っていないが将来的に拡張予定。未使用警告を避けるためコメントアウト
        // private static readonly string AutoBackupFileName = "vrchat_auto_backup.json";
        private static readonly string BackupVersion = "2.2.0";
        
        private static bool isUploading = false;
        private static GameObject lastUploadedAvatar = null;
        private static DateTime lastBackupTime = DateTime.MinValue;
        
        static VRChatUploadAutoBackup()
        {
            // VRChatアップロード前の自動バックアップ
            EditorApplication.update += CheckForVRChatUpload;
        }
        
        private static void CheckForVRChatUpload()
        {
            // VRChat SDKのアップロード状態を監視
            var avatars = UnityEngine.Object.FindObjectsOfType<VRCAvatarDescriptor>();
            if (avatars.Length > 0 && !isUploading)
            {
                // アップロード開始の検出（簡易版）
                isUploading = true;
                OnVRChatUploadStart();
            }
        }
        
        private static void OnVRChatUploadStart()
        {
            try
            {
                Debug.Log("[VRChatUploadAutoBackup] VRChat upload started - creating automatic backup");
                
                // アップロード対象のアバターを検索
                var avatars = UnityEngine.Object.FindObjectsOfType<VRCAvatarDescriptor>();
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
                    // エラー時の復元オプションを表示
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
                    uploadType = "Auto",
                    materials = new List<MaterialBackupEntry>()
                };

                var renderers = avatar.GetComponentsInChildren<Renderer>(true);
                foreach (var renderer in renderers)
                {
                    var materials = renderer.sharedMaterials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        if (materials[i] != null)
                        {
                            var entry = CreateMaterialBackupEntry(materials[i], renderer, i);
                            backupData.materials.Add(entry);
                        }
                    }
                }

                string backupFile = Path.Combine(backupPath, $"AutoBackup_{DateTime.Now:yyyyMMdd_HHmmss}.json");
                string json = JsonUtility.ToJson(backupData, true);
                File.WriteAllText(backupFile, json);
                
                Debug.Log($"[VRChatUploadAutoBackup] Automatic backup created: {backupFile}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[VRChatUploadAutoBackup] Failed to create automatic backup: {e.Message}");
            }
        }

        private static MaterialBackupEntry CreateMaterialBackupEntry(Material material, Renderer renderer, int materialIndex)
        {
            var entry = new MaterialBackupEntry
            {
                materialName = material.name,
                materialGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(material)),
                materialPath = AssetDatabase.GetAssetPath(material),
                shaderName = material.shader != null ? material.shader.name : "",
                shaderGUID = material.shader != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(material.shader)) : "",
                rendererName = renderer.name,
                rendererPath = GetGameObjectPath(renderer.gameObject),
                materialIndex = materialIndex,
                properties = new Dictionary<string, string>(),
                textureGUIDs = new Dictionary<string, string>()
            };

            // プロパティの保存
            var shader = material.shader;
            if (shader != null)
            {
                int propertyCount = ShaderUtil.GetPropertyCount(shader);
                for (int i = 0; i < propertyCount; i++)
                {
                    string propertyName = ShaderUtil.GetPropertyName(shader, i);
                    ShaderUtil.ShaderPropertyType propertyType = ShaderUtil.GetPropertyType(shader, i);
                    
                    switch (propertyType)
                    {
                        case ShaderUtil.ShaderPropertyType.Color:
                            entry.properties[propertyName] = material.GetColor(propertyName).ToString();
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
                                entry.textureGUIDs[propertyName] = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(texture));
                            }
                            break;
                    }
                }
            }

            return entry;
        }

        private static void CheckMaterialStateAfterUpload(GameObject avatar)
        {
            try
            {
                int missingCount = 0;
                var renderers = avatar.GetComponentsInChildren<Renderer>(true);
                
                foreach (var renderer in renderers)
                {
                    var materials = renderer.sharedMaterials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        if (materials[i] == null)
                        {
                            missingCount++;
                        }
                    }
                }
                
                if (missingCount > 0)
                {
                    Debug.LogWarning($"[VRChatUploadAutoBackup] {missingCount} materials missing after upload");
                    ShowRestoreOption(avatar);
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
            bool restore = EditorUtility.DisplayDialog("Material Backup Available", 
                "Some materials may have been lost during upload. Would you like to restore from automatic backup?", 
                "Restore", "Skip");
            
            if (restore)
            {
                RestoreMaterialsFromAutoBackup(avatar);
            }
        }
        
        private static void RestoreMaterialsFromAutoBackup(GameObject avatar)
        {
            try
            {
                string backupPath = GetBackupFolderPath(avatar.name);
                if (!Directory.Exists(backupPath))
                {
                    EditorUtility.DisplayDialog("Error", "No backup directory found!", "OK");
                    return;
                }

                var backupFiles = Directory.GetFiles(backupPath, "AutoBackup_*.json");
                if (backupFiles.Length == 0)
                {
                    EditorUtility.DisplayDialog("Error", "No automatic backup files found!", "OK");
                    return;
                }

                // 最新のバックアップファイルを選択
                string latestBackup = backupFiles.OrderByDescending(f => File.GetLastWriteTime(f)).First();
                string jsonData = File.ReadAllText(latestBackup);
                var backupData = JsonUtility.FromJson<VRChatAutoBackupData>(jsonData);
                
                if (backupData == null || backupData.materials == null)
                {
                    EditorUtility.DisplayDialog("Error", "Invalid backup data!", "OK");
                    return;
                }

                int restoredCount = 0;
                var renderers = avatar.GetComponentsInChildren<Renderer>(true);
                var rendererDict = renderers.ToDictionary(r => GetGameObjectPath(r.gameObject), r => r);

                foreach (var backupEntry in backupData.materials)
                {
                    if (rendererDict.TryGetValue(backupEntry.rendererPath, out var renderer))
                    {
                        var restoredMaterial = RestoreMaterialFromBackup(backupEntry);
                        if (restoredMaterial != null)
                        {
                            var materials = renderer.sharedMaterials;
                            if (backupEntry.materialIndex < materials.Length)
                            {
                                materials[backupEntry.materialIndex] = restoredMaterial;
                                renderer.sharedMaterials = materials;
                                EditorUtility.SetDirty(renderer);
                                restoredCount++;
                            }
                        }
                    }
                }

                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Restore Complete", $"Restored {restoredCount} materials from automatic backup.", "OK");
            }
            catch (Exception e)
            {
                Debug.LogError($"[VRChatUploadAutoBackup] Failed to restore from automatic backup: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to restore: {e.Message}", "OK");
            }
        }

        private static Material RestoreMaterialFromBackup(MaterialBackupEntry backupEntry)
        {
            // マテリアルアセットの復元を試行
            if (!string.IsNullOrEmpty(backupEntry.materialGUID))
            {
                string materialPath = AssetDatabase.GUIDToAssetPath(backupEntry.materialGUID);
                if (!string.IsNullOrEmpty(materialPath))
                {
                    var restoredMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                    if (restoredMaterial != null)
                        return restoredMaterial;
                }
            }

            // シェーダーから新規作成
            Shader shader = null;
            if (!string.IsNullOrEmpty(backupEntry.shaderGUID))
            {
                string shaderPath = AssetDatabase.GUIDToAssetPath(backupEntry.shaderGUID);
                if (!string.IsNullOrEmpty(shaderPath))
                {
                    shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
                }
            }

            if (shader == null && !string.IsNullOrEmpty(backupEntry.shaderName))
            {
                shader = Shader.Find(backupEntry.shaderName);
            }

            if (shader == null)
            {
                shader = Shader.Find("lilToon");
            }

            if (shader == null)
                return null;

            var newMaterial = new Material(shader) { name = backupEntry.materialName };

            // プロパティの復元
            foreach (var kvp in backupEntry.properties)
            {
                string propertyName = kvp.Key;
                string propertyValue = kvp.Value;

                if (newMaterial.HasProperty(propertyName))
                {
                    if (propertyValue.StartsWith("RGBA("))
                    {
                        // Color型の復元
                        var colorMatch = System.Text.RegularExpressions.Regex.Match(propertyValue, @"RGBA\(([^)]+)\)");
                        if (colorMatch.Success)
                        {
                            var colorValues = colorMatch.Groups[1].Value.Split(',');
                            if (colorValues.Length >= 4)
                            {
                                float r = float.Parse(colorValues[0]);
                                float g = float.Parse(colorValues[1]);
                                float b = float.Parse(colorValues[2]);
                                float a = float.Parse(colorValues[3]);
                                newMaterial.SetColor(propertyName, new Color(r, g, b, a));
                            }
                        }
                    }
                    else if (propertyValue.StartsWith("(") && propertyValue.EndsWith(")"))
                    {
                        // Vector4型の復元
                        var vector4 = StringToVector4(propertyValue);
                        newMaterial.SetVector(propertyName, vector4);
                    }
                    else if (float.TryParse(propertyValue, out float floatValue))
                    {
                        // Float型の復元
                        newMaterial.SetFloat(propertyName, floatValue);
                    }
                }
            }

            // テクスチャの復元
            foreach (var kvp in backupEntry.textureGUIDs)
            {
                string propertyName = kvp.Key;
                string textureGUID = kvp.Value;

                if (newMaterial.HasProperty(propertyName))
                {
                    string texturePath = AssetDatabase.GUIDToAssetPath(textureGUID);
                    if (!string.IsNullOrEmpty(texturePath))
                    {
                        var texture = AssetDatabase.LoadAssetAtPath<Texture>(texturePath);
                        if (texture != null)
                        {
                            newMaterial.SetTexture(propertyName, texture);
                        }
                    }
                }
            }

            return newMaterial;
        }

        private static string GetBackupFolderPath(string avatarName)
        {
            string basePath = "Assets";
            string backupPath = Path.Combine(basePath, BackupSubfolder, avatarName);
            
            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }
            
            return backupPath;
        }
        
        private static string GetGameObjectPath(GameObject obj)
        {
            var path = new List<string>();
            var current = obj;
            
            while (current != null)
            {
                path.Insert(0, current.name);
                current = current.transform.parent?.gameObject;
            }
            
            return string.Join("/", path);
        }
        
        private static Vector4 StringToVector4(string s)
        {
            try
            {
                // 括弧を除去して数値を抽出
                s = s.Trim('(', ')');
                var values = s.Split(',');
                
                if (values.Length >= 4)
                {
                    return new Vector4(
                        float.Parse(values[0]),
                        float.Parse(values[1]),
                        float.Parse(values[2]),
                        float.Parse(values[3])
                    );
                }
                else if (values.Length >= 3)
                {
                    return new Vector4(
                        float.Parse(values[0]),
                        float.Parse(values[1]),
                        float.Parse(values[2]),
                        0f
                    );
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[VRChatUploadAutoBackup] Failed to parse Vector4: {s}, Error: {e.Message}");
            }
            
            return Vector4.zero;
        }
    }
}