using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// VRChat material backup data structure for comprehensive material restoration
    /// </summary>
    [System.Serializable]
    public class VRChatMaterialBackupData
    {
        public string avatarName;
        public string backupTime;
        public string version;
        public List<MaterialBackupEntry> materials;
    }

    /// <summary>
    /// Individual material backup entry with complete property serialization
    /// </summary>
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
    /// VRChatアップロード時のlilToonマテリアルmissing問題を解決する強化版AutoFixer
    /// なんｊ風に言うと「これで完璧なマテリアル修復システムが完成したぜ！」💪🔥
    /// </summary>
    public class MissingMaterialAutoFixer : EditorWindow
    {
        private static readonly string BackupSubfolder = "VRChatMaterialBackups";
        
        [MenuItem("Tools/lilToon PCSS/Missing Material AutoFixer")]
        public static void ShowWindow()
        {
            GetWindow<MissingMaterialAutoFixer>("Missing Material AutoFixer");
        }

        private int missingCount = 0;
        private int fixedCount = 0;
        private bool useBackupRestore = true;
        private bool createBackupBeforeFix = true;
        private Vector2 scrollPosition;

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            GUILayout.Label("🎯 Missing Material AutoFixer", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            // オプション設定
            EditorGUILayout.LabelField("⚙️ Options", EditorStyles.miniBoldLabel);
            useBackupRestore = EditorGUILayout.Toggle("Use backup restore", useBackupRestore);
            createBackupBeforeFix = EditorGUILayout.Toggle("Create backup before fix", createBackupBeforeFix);
            
            EditorGUILayout.Space(10);
            
            // メイン機能
            EditorGUILayout.LabelField("🚀 Main Functions", EditorStyles.miniBoldLabel);
            
            if (GUILayout.Button("🔍 Scan for Missing Materials", GUILayout.Height(30)))
            {
                missingCount = ScanAndReport();
                EditorUtility.DisplayDialog("Scan Result", $"Missing materials found: {missingCount}", "OK");
            }
            
            if (GUILayout.Button("🔧 Auto Fix Missing Materials", GUILayout.Height(30)))
            {
                fixedCount = AutoFix();
                EditorUtility.DisplayDialog("Auto Fix", $"Fixed {fixedCount} missing materials.", "OK");
            }
            
            if (GUILayout.Button("💾 Create Backup Before Fix", GUILayout.Height(30)))
            {
                CreateBackupBeforeFix();
            }
            
            if (GUILayout.Button("🔄 Restore from Backup", GUILayout.Height(30)))
            {
                RestoreFromBackup();
            }
            
            EditorGUILayout.Space(10);
            
            // 統計情報
            EditorGUILayout.LabelField("📊 Statistics", EditorStyles.miniBoldLabel);
            GUILayout.Label($"Last Scan: {missingCount} missing | Last Fix: {fixedCount} fixed", EditorStyles.boldLabel);
            
            // バックアップ情報
            DisplayBackupInfo();
            
            EditorGUILayout.EndScrollView();
        }

        private int ScanAndReport()
        {
            int count = 0;
            foreach (var renderer in FindObjectsOfType<Renderer>(true))
            {
                foreach (var mat in renderer.sharedMaterials)
                {
                    if (mat == null)
                    {
                        Debug.LogWarning($"[MissingMaterialAutoFixer] {renderer.name} has a missing material slot.", renderer);
                        count++;
                    }
                }
            }
            return count;
        }

        private int AutoFix()
        {
            var lilToonShader = Shader.Find("lilToon");
            if (lilToonShader == null)
            {
                EditorUtility.DisplayDialog("Error", "lilToon shader not found! Please install lilToon.", "OK");
                return 0;
            }
            
            int count = 0;
            
            // バックアップから復元を試行
            if (useBackupRestore)
            {
                count += RestoreFromBackupSilent();
            }
            
            // 残りのmissing materialを新規作成
            foreach (var renderer in FindObjectsOfType<Renderer>(true))
            {
                var mats = renderer.sharedMaterials;
                bool changed = false;
                for (int i = 0; i < mats.Length; i++)
                {
                    if (mats[i] == null)
                    {
                        Undo.RecordObject(renderer, "AutoFix Missing Material");
                        var newMat = new Material(lilToonShader) { name = $"{renderer.name}_AutoFixed_{i}" };
                        mats[i] = newMat;
                        count++;
                        changed = true;
                        Debug.Log($"[MissingMaterialAutoFixer] Fixed missing material on {renderer.name}", renderer);
                    }
                }
                if (changed)
                {
                    renderer.sharedMaterials = mats;
                    EditorUtility.SetDirty(renderer);
                }
            }
            AssetDatabase.SaveAssets();
            return count;
        }
        
        private void CreateBackupBeforeFix()
        {
            try
            {
                var avatars = FindObjectsOfType<VRCAvatarDescriptor>();
                if (avatars.Length > 0)
                {
                    var avatar = avatars[0];
                    string backupPath = GetBackupFolderPath(avatar.name);
                    var backupData = new VRChatMaterialBackupData
                    {
                        avatarName = avatar.name,
                        backupTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        version = "2.3.0",
                        materials = new List<MaterialBackupEntry>()
                    };
                    
                    var renderers = avatar.GetComponentsInChildren<Renderer>(true);
                    int backedUpMaterials = 0;
                    
                    foreach (var renderer in renderers)
                    {
                        for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                        {
                            var material = renderer.sharedMaterials[i];
                            if (material != null)
                            {
                                var entry = CreateMaterialBackupEntry(material, renderer, i);
                                backupData.materials.Add(entry);
                                backedUpMaterials++;
                            }
                        }
                    }
                    
                    string json = JsonUtility.ToJson(backupData, true);
                    string filePath = Path.Combine(backupPath, "pre_fix_backup.json");
                    File.WriteAllText(filePath, json);
                    
                    AssetDatabase.SaveAssets();
                    
                    EditorUtility.DisplayDialog("Backup Created", 
                        $"Pre-fix backup created successfully!\n" +
                        $"Materials backed up: {backedUpMaterials}", "OK");
                    
                    Debug.Log($"[MissingMaterialAutoFixer] Pre-fix backup created: {backedUpMaterials} materials");
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "No VRChat avatar found in scene", "OK");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[MissingMaterialAutoFixer] Error creating backup: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to create backup: {e.Message}", "OK");
            }
        }
        
        private void RestoreFromBackup()
        {
            try
            {
                var avatars = FindObjectsOfType<VRCAvatarDescriptor>();
                if (avatars.Length > 0)
                {
                    var avatar = avatars[0];
                    int restoredCount = RestoreFromBackupSilent();
                    
                    EditorUtility.DisplayDialog("Restore Complete", 
                        $"Materials restored from backup!\n" +
                        $"Materials restored: {restoredCount}", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "No VRChat avatar found in scene", "OK");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[MissingMaterialAutoFixer] Error restoring from backup: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to restore from backup: {e.Message}", "OK");
            }
        }
        
        private int RestoreFromBackupSilent()
        {
            try
            {
                var avatars = FindObjectsOfType<VRCAvatarDescriptor>();
                if (avatars.Length == 0) return 0;
                
                var avatar = avatars[0];
                string backupPath = GetBackupFolderPath(avatar.name);
                
                // 最新のバックアップファイルを検索
                string[] backupFiles = {
                    Path.Combine(backupPath, "vrchat_auto_backup.json"),
                    Path.Combine(backupPath, "vrchat_material_backup.json"),
                    Path.Combine(backupPath, "pre_fix_backup.json")
                };
                
                string backupFile = null;
                foreach (var file in backupFiles)
                {
                    if (File.Exists(file))
                    {
                        backupFile = file;
                        break;
                    }
                }
                
                if (backupFile == null) return 0;
                
                string json = File.ReadAllText(backupFile);
                var backupData = JsonUtility.FromJson<VRChatMaterialBackupData>(json);
                
                int restoredCount = 0;
                var renderers = avatar.GetComponentsInChildren<Renderer>(true);
                
                foreach (var renderer in renderers)
                {
                    var materials = renderer.sharedMaterials;
                    bool changed = false;
                    
                    for (int i = 0; i < materials.Length; i++)
                    {
                        if (materials[i] == null)
                        {
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
                                    Debug.Log($"[MissingMaterialAutoFixer] Restored material: {backupEntry.materialName} on {renderer.name}");
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
                return restoredCount;
            }
            catch (Exception e)
            {
                Debug.LogError($"[MissingMaterialAutoFixer] Error in silent restore: {e.Message}");
                return 0;
            }
        }
        
        private MaterialBackupEntry CreateMaterialBackupEntry(Material material, Renderer renderer, int materialIndex)
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
        
        private Material RestoreMaterialFromBackup(MaterialBackupEntry backupEntry)
        {
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
        
        private void DisplayBackupInfo()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("📁 Backup Information", EditorStyles.miniBoldLabel);
            
            string backupRootPath = Path.Combine("Assets", BackupSubfolder);
            if (Directory.Exists(backupRootPath))
            {
                var backupFolders = Directory.GetDirectories(backupRootPath);
                EditorGUILayout.LabelField($"Backup folders: {backupFolders.Length}");
                
                foreach (var folder in backupFolders)
                {
                    string folderName = Path.GetFileName(folder);
                    string[] backupFiles = {
                        Path.Combine(folder, "vrchat_auto_backup.json"),
                        Path.Combine(folder, "vrchat_material_backup.json"),
                        Path.Combine(folder, "pre_fix_backup.json")
                    };
                    
                    foreach (var file in backupFiles)
                    {
                        if (File.Exists(file))
                        {
                            var fileInfo = new FileInfo(file);
                            EditorGUILayout.LabelField($"📄 {folderName}: {Path.GetFileName(file)} ({fileInfo.LastWriteTime:yyyy-MM-dd HH:mm})");
                        }
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

    // バックアップデータ構造
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

    // FBX/Prefabインポート時の自動リマップ
    public class LilToonMaterialRemapper : AssetPostprocessor
    {
        void OnPostprocessModel(GameObject g)
        {
            var lilToonShader = Shader.Find("lilToon");
            if (lilToonShader == null) return;

            foreach (var renderer in g.GetComponentsInChildren<Renderer>(true))
            {
                var mats = renderer.sharedMaterials;
                bool changed = false;
                for (int i = 0; i < mats.Length; i++)
                {
                    if (mats[i] == null)
                    {
                        var newMat = new Material(lilToonShader) { name = $"{renderer.name}_AutoRemapped_{i}" };
                        mats[i] = newMat;
                        changed = true;
                        Debug.Log($"[LilToonMaterialRemapper] Auto-remapped missing material on {renderer.name}", renderer);
                    }
                }
                if (changed)
                {
                    renderer.sharedMaterials = mats;
                    EditorUtility.SetDirty(renderer);
                }
            }
        }
    }
} 
