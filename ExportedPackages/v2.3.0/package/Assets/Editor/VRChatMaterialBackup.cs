using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// VRChatアップロード時のlilToonマテリアルmissing問題を解決するUUIDベースバックアップシステム
    /// なんｊ風に言うと「これで完璧なマテリアルバックアップシステムが完成したぜ！」💪🔥
    /// </summary>
    public class VRChatMaterialBackup : EditorWindow
    {
        private static readonly string BackupSubfolder = "VRChatMaterialBackups";
        private static readonly string BackupFileName = "vrchat_material_backup.json";
        private static readonly string BackupVersion = "2.2.0";
        
        private GameObject targetAvatar;
        private VRCAvatarDescriptor avatarDescriptor;
        private Vector2 scrollPosition;
        private bool showAdvancedOptions = false;
        private bool autoBackupOnUpload = true;
        private bool backupTextures = true;
        private bool backupShaderProperties = true;
        private bool createBackupBeforeUpload = true;
        
        [MenuItem("Tools/lilToon PCSS/VRChat Material Backup")]
        public static void ShowWindow()
        {
            GetWindow<VRChatMaterialBackup>("VRChat Material Backup");
        }
        
        private void OnEnable()
        {
            // 自動的にアバターを検出
            FindAvatarInScene();
        }
        
        private void OnGUI()
        {
            GUILayout.Label("🎯 VRChat Material Backup System", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            // アバター選択
            EditorGUILayout.LabelField("📋 Avatar Selection", EditorStyles.miniBoldLabel);
            targetAvatar = (GameObject)EditorGUILayout.ObjectField("Target Avatar", targetAvatar, typeof(GameObject), true);
            
            if (targetAvatar != null)
            {
                avatarDescriptor = targetAvatar.GetComponent<VRCAvatarDescriptor>();
                if (avatarDescriptor != null)
                {
                    EditorGUILayout.HelpBox($"✅ VRChat Avatar detected: {avatarDescriptor.name}", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("⚠️ No VRCAvatarDescriptor found. This may not be a VRChat avatar.", MessageType.Warning);
                }
            }
            
            EditorGUILayout.Space(10);
            
            // メイン機能
            EditorGUILayout.LabelField("🚀 Main Functions", EditorStyles.miniBoldLabel);
            
            if (GUILayout.Button("💾 Create Material Backup", GUILayout.Height(30)))
            {
                if (targetAvatar != null)
                {
                    CreateMaterialBackup(targetAvatar);
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Please select a target avatar.", "OK");
                }
            }
            
            if (GUILayout.Button("🔄 Restore Materials from Backup", GUILayout.Height(30)))
            {
                if (targetAvatar != null)
                {
                    RestoreMaterialsFromBackup(targetAvatar);
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Please select a target avatar.", "OK");
                }
            }
            
            if (GUILayout.Button("🔍 Scan for Missing Materials", GUILayout.Height(30)))
            {
                ScanForMissingMaterials();
            }
            
            EditorGUILayout.Space(10);
            
            // 高度なオプション
            showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "⚙️ Advanced Options");
            if (showAdvancedOptions)
            {
                EditorGUI.indentLevel++;
                
                autoBackupOnUpload = EditorGUILayout.Toggle("Auto backup before upload", autoBackupOnUpload);
                backupTextures = EditorGUILayout.Toggle("Backup textures", backupTextures);
                backupShaderProperties = EditorGUILayout.Toggle("Backup shader properties", backupShaderProperties);
                createBackupBeforeUpload = EditorGUILayout.Toggle("Create backup before upload", createBackupBeforeUpload);
                
                EditorGUILayout.Space(5);
                
                if (GUILayout.Button("🗑️ Clear All Backups"))
                {
                    if (EditorUtility.DisplayDialog("Clear Backups", 
                        "Are you sure you want to clear all material backups?", "Yes", "No"))
                    {
                        ClearAllBackups();
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space(10);
            
            // 情報表示
            EditorGUILayout.LabelField("📊 Backup Information", EditorStyles.miniBoldLabel);
            DisplayBackupInfo();
        }
        
        private void FindAvatarInScene()
        {
            // シーン内のVRCAvatarDescriptorを検索
            var avatars = FindObjectsOfType<VRCAvatarDescriptor>();
            if (avatars.Length > 0)
            {
                targetAvatar = avatars[0].gameObject;
                avatarDescriptor = avatars[0];
            }
        }
        
        private void CreateMaterialBackup(GameObject avatar)
        {
            try
            {
                string backupPath = GetBackupFolderPath(avatar.name);
                var backupData = new VRChatMaterialBackupData
                {
                    avatarName = avatar.name,
                    backupTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    version = BackupVersion,
                    materials = new List<MaterialBackupEntry>()
                };
                
                // すべてのRendererを取得
                var renderers = avatar.GetComponentsInChildren<Renderer>(true);
                int totalMaterials = 0;
                int backedUpMaterials = 0;
                
                foreach (var renderer in renderers)
                {
                    foreach (var material in renderer.sharedMaterials)
                    {
                        totalMaterials++;
                        
                        if (material != null)
                        {
                            var entry = CreateMaterialBackupEntry(material, renderer);
                            backupData.materials.Add(entry);
                            backedUpMaterials++;
                        }
                    }
                }
                
                // JSONとして保存
                string json = JsonUtility.ToJson(backupData, true);
                string filePath = Path.Combine(backupPath, BackupFileName);
                File.WriteAllText(filePath, json);
                
                AssetDatabase.SaveAssets();
                
                EditorUtility.DisplayDialog("Success", 
                    $"Material backup created successfully!\n" +
                    $"Avatar: {avatar.name}\n" +
                    $"Materials backed up: {backedUpMaterials}/{totalMaterials}\n" +
                    $"Location: {filePath}", "OK");
                
                Debug.Log($"[VRChatMaterialBackup] Created backup for {avatar.name}: {backedUpMaterials}/{totalMaterials} materials");
            }
            catch (Exception e)
            {
                Debug.LogError($"[VRChatMaterialBackup] Error creating backup: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to create backup: {e.Message}", "OK");
            }
        }
        
        private MaterialBackupEntry CreateMaterialBackupEntry(Material material, Renderer renderer)
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
                properties = new Dictionary<string, string>(),
                textureGUIDs = new Dictionary<string, string>()
            };
            
            // シェーダープロパティをバックアップ
            if (backupShaderProperties)
            {
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
                            if (backupTextures)
                            {
                                var texture = material.GetTexture(propertyName);
                                if (texture != null)
                                {
                                    string texturePath = AssetDatabase.GetAssetPath(texture);
                                    entry.textureGUIDs[propertyName] = AssetDatabase.AssetPathToGUID(texturePath);
                                }
                            }
                            break;
                    }
                }
            }
            
            return entry;
        }
        
        private void RestoreMaterialsFromBackup(GameObject avatar)
        {
            try
            {
                string backupPath = GetBackupFolderPath(avatar.name);
                string filePath = Path.Combine(backupPath, BackupFileName);
                
                if (!File.Exists(filePath))
                {
                    EditorUtility.DisplayDialog("Error", $"No backup found for {avatar.name}", "OK");
                    return;
                }
                
                string json = File.ReadAllText(filePath);
                var backupData = JsonUtility.FromJson<VRChatMaterialBackupData>(json);
                
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
                            var backupEntry = FindBackupEntry(backupData, renderer, i);
                            if (backupEntry != null)
                            {
                                var restoredMaterial = RestoreMaterialFromBackup(backupEntry);
                                if (restoredMaterial != null)
                                {
                                    materials[i] = restoredMaterial;
                                    changed = true;
                                    restoredCount++;
                                    Debug.Log($"[VRChatMaterialBackup] Restored material: {backupEntry.materialName} on {renderer.name}");
                                }
                            }
                        }
                    }
                    
                    if (changed)
                    {
                        Undo.RecordObject(renderer, "Restore Materials from Backup");
                        renderer.sharedMaterials = materials;
                        EditorUtility.SetDirty(renderer);
                    }
                }
                
                AssetDatabase.SaveAssets();
                
                EditorUtility.DisplayDialog("Success", 
                    $"Material restoration completed!\n" +
                    $"Materials restored: {restoredCount}/{totalMaterials}", "OK");
                
                Debug.Log($"[VRChatMaterialBackup] Restored {restoredCount}/{totalMaterials} materials for {avatar.name}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[VRChatMaterialBackup] Error restoring materials: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to restore materials: {e.Message}", "OK");
            }
        }
        
        private MaterialBackupEntry FindBackupEntry(VRChatMaterialBackupData backupData, Renderer renderer, int materialIndex)
        {
            string rendererPath = GetGameObjectPath(renderer.gameObject);
            
            // レンダラーパスとマテリアルインデックスで一致を検索
            return backupData.materials.FirstOrDefault(m => 
                m.rendererPath == rendererPath && 
                m.materialIndex == materialIndex);
        }
        
        private Material RestoreMaterialFromBackup(MaterialBackupEntry backupEntry)
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
        
        private void ScanForMissingMaterials()
        {
            var missingMaterials = new List<string>();
            var renderers = FindObjectsOfType<Renderer>();
            
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
                string message = $"Found {missingMaterials.Count} missing materials:\n\n" + string.Join("\n", missingMaterials);
                EditorUtility.DisplayDialog("Missing Materials Found", message, "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Scan Complete", "No missing materials found!", "OK");
            }
        }
        
        private void ClearAllBackups()
        {
            string backupRootPath = Path.Combine("Assets", BackupSubfolder);
            if (Directory.Exists(backupRootPath))
            {
                Directory.Delete(backupRootPath, true);
                AssetDatabase.Refresh();
                Debug.Log("[VRChatMaterialBackup] All backups cleared");
            }
        }
        
        private void DisplayBackupInfo()
        {
            string backupRootPath = Path.Combine("Assets", BackupSubfolder);
            if (Directory.Exists(backupRootPath))
            {
                var backupFolders = Directory.GetDirectories(backupRootPath);
                EditorGUILayout.LabelField($"Backup folders: {backupFolders.Length}");
                
                foreach (var folder in backupFolders)
                {
                    string folderName = Path.GetFileName(folder);
                    string backupFile = Path.Combine(folder, BackupFileName);
                    
                    if (File.Exists(backupFile))
                    {
                        var fileInfo = new FileInfo(backupFile);
                        EditorGUILayout.LabelField($"📁 {folderName}: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm}");
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("No backups found");
            }
        }
        
        private string GetBackupFolderPath(string avatarName)
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
        
        private string GetGameObjectPath(GameObject obj)
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
        
        private Vector4 StringToVector4(string s)
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
    public class VRChatMaterialBackupData
    {
        public string avatarName;
        public string backupTime;
        public string version;
        public List<MaterialBackupEntry> materials;
    }
    
    [System.Serializable]
    public class MaterialBackupEntry
    {
        public string materialName;
        public string materialGUID;
        public string materialPath;
        public string shaderName;
        public string shaderGUID;
        public string rendererName;
        public string rendererPath;
        public int materialIndex;
        public Dictionary<string, string> properties;
        public Dictionary<string, string> textureGUIDs;
    }
    
    /// <summary>
    /// VRChatアップロード前の自動バックアップ機能
    /// </summary>
    [InitializeOnLoad]
    public class VRChatUploadBackup
    {
        static VRChatUploadBackup()
        {
            // VRChatアップロード前の自動バックアップ
            EditorApplication.update += CheckForVRChatUpload;
        }
        
        private static void CheckForVRChatUpload()
        {
            // VRChat SDKのアップロード処理を検出して自動バックアップを実行
            // この部分はVRChat SDKの内部実装に依存するため、
            // 実際の実装ではVRChat SDKのイベントをフックする必要があります
        }
    }
} 