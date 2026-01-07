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
    /// VRChatアップロード時のlilToonマテリアルmissing問題を解決する強化版AutoFixer
    /// なんｊ風に言うと「これで完璧なマテリアル修復システムが完成したぜ！」💪🔥
    /// </summary>
    public class MissingMaterialAutoFixer : EditorWindow
    {
        private static readonly string BackupSubfolder = "VRChatMaterialBackups";
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Missing Material AutoFixer")]
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
            
            // バックアップ情報表示
            EditorGUILayout.LabelField(" Backup Information", EditorStyles.miniBoldLabel);
            DisplayBackupInfo();
            
            EditorGUILayout.EndScrollView();
        }

        private int ScanAndReport()
        {
            int count = 0;
            var renderers = FindObjectsOfType<Renderer>();
            
            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] == null)
                    {
                        count++;
                    }
                }
            }
            
            return count;
        }

        private int AutoFix()
        {
            int fixedCount = 0;
            var renderers = FindObjectsOfType<Renderer>();
            
            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials;
                bool hasChanges = false;
                
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] == null)
                    {
                        // デフォルトのlilToonマテリアルを作成
                        var newMaterial = new Material(Shader.Find("lilToon"));
                        newMaterial.name = $"AutoFixed_Material_{i}";
                        
                        materials[i] = newMaterial;
                        hasChanges = true;
                        fixedCount++;
                    }
                }
                
                if (hasChanges)
                {
                    renderer.sharedMaterials = materials;
                    EditorUtility.SetDirty(renderer);
                }
            }
            
            AssetDatabase.SaveAssets();
            return fixedCount;
        }

        private void CreateBackupBeforeFix()
        {
            try
            {
                var avatars = FindObjectsOfType<VRCAvatarDescriptor>();
                if (avatars.Length == 0)
                {
                    EditorUtility.DisplayDialog("Error", "No VRChat avatar found in scene!", "OK");
                    return;
                }

                var avatar = avatars[0];
                string backupPath = GetBackupFolderPath(avatar.name);
                var backupData = new VRChatMaterialBackupData
                {
                    avatarName = avatar.name,
                    backupTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    version = "2.2.0",
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

                string backupFile = Path.Combine(backupPath, $"PreFixBackup_{DateTime.Now:yyyyMMdd_HHmmss}.json");
                string json = JsonUtility.ToJson(backupData, true);
                File.WriteAllText(backupFile, json);
                
                EditorUtility.DisplayDialog("Backup Created", $"Backup saved to: {backupFile}", "OK");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create backup: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to create backup: {e.Message}", "OK");
            }
        }

        private void RestoreFromBackup()
        {
            try
            {
                var avatars = FindObjectsOfType<VRCAvatarDescriptor>();
                if (avatars.Length == 0)
                {
                    EditorUtility.DisplayDialog("Error", "No VRChat avatar found in scene!", "OK");
                    return;
                }

                var avatar = avatars[0];
                int restoredCount = RestoreFromBackupSilent(avatar.name);
                EditorUtility.DisplayDialog("Restore Complete", $"Restored {restoredCount} materials from backup.", "OK");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to restore from backup: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to restore: {e.Message}", "OK");
            }
        }

        private int RestoreFromBackupSilent(string avatarName)
        {
            try
            {
                string backupPath = GetBackupFolderPath(avatarName);
                if (!Directory.Exists(backupPath))
                {
                    return 0;
                }

                var backupFiles = Directory.GetFiles(backupPath, "PreFixBackup_*.json");
                if (backupFiles.Length == 0)
                {
                    return 0;
                }

                // 最新のバックアップファイルを選択
                string latestBackup = backupFiles.OrderByDescending(f => File.GetLastWriteTime(f)).First();
                string jsonData = File.ReadAllText(latestBackup);
                var backupData = JsonUtility.FromJson<VRChatMaterialBackupData>(jsonData);
                
                if (backupData == null || backupData.materials == null)
                {
                    return 0;
                }

                int restoredCount = 0;
                var avatars = FindObjectsOfType<VRCAvatarDescriptor>();
                if (avatars.Length > 0)
                {
                    var avatar = avatars[0];
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
                }

                return restoredCount;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to restore from backup: {e.Message}");
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

        private Material RestoreMaterialFromBackup(MaterialBackupEntry backupEntry)
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

        private void DisplayBackupInfo()
        {
            try
            {
                var avatars = FindObjectsOfType<VRCAvatarDescriptor>();
                if (avatars.Length == 0)
                {
                    EditorGUILayout.HelpBox("No VRChat avatar found in scene", MessageType.Info);
                    return;
                }

                var avatar = avatars[0];
                string backupPath = GetBackupFolderPath(avatar.name);
                
                if (Directory.Exists(backupPath))
                {
                    var backupFiles = Directory.GetFiles(backupPath, "PreFixBackup_*.json");
                    EditorGUILayout.LabelField($"Backup files found: {backupFiles.Length}");
                    
                    if (backupFiles.Length > 0)
                    {
                        var latestBackup = backupFiles.OrderByDescending(f => File.GetLastWriteTime(f)).First();
                        var fileInfo = new FileInfo(latestBackup);
                        EditorGUILayout.LabelField($"Latest backup: {fileInfo.Name}");
                        EditorGUILayout.LabelField($"Created: {fileInfo.CreationTime}");
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No backup directory found", MessageType.Warning);
                }
            }
            catch (Exception e)
            {
                EditorGUILayout.HelpBox($"Error displaying backup info: {e.Message}", MessageType.Error);
            }
        }

        private string GetBackupFolderPath(string avatarName)
        {
            string basePath = "Assets";
            string backupPath = Path.Combine(basePath, BackupSubfolder, avatarName);
            
            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }
            
            return backupPath;
        }
        
        private string GetGameObjectPath(GameObject obj)
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
        
        private Vector4 StringToVector4(string s)
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
                Debug.LogError($"Failed to parse Vector4: {s}, Error: {e.Message}");
            }
            
            return Vector4.zero;
        }
    }

    /// <summary>
    /// lilToonマテリアルリマッパー
    /// なんｊ風に言うと「これで完璧なマテリアルリマッピングシステムが完成したぜ！」💪🔥
    /// </summary>
    public class LilToonMaterialRemapper : AssetPostprocessor
    {
        void OnPostprocessModel(GameObject g)
        {
            // モデルインポート時の自動マテリアルリマッピング
            var renderers = g.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] != null && materials[i].shader != null)
                    {
                        // シェーダー名に基づいてlilToonシェーダーにリマップ
                        string shaderName = materials[i].shader.name;
                        if (shaderName.Contains("Standard") || shaderName.Contains("Legacy"))
                        {
                            var lilToonShader = Shader.Find("lilToon");
                            if (lilToonShader != null)
                            {
                                materials[i].shader = lilToonShader;
                                EditorUtility.SetDirty(materials[i]);
                            }
                        }
                    }
                }
            }
        }
    }
}