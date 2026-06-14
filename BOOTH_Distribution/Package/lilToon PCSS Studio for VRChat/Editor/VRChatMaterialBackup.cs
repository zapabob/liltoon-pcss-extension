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
    /// VRChatアップロード前の自動バックアップ機能
    /// </summary>
    #if PCSS_DEV
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
    #endif

    /// <summary>
    /// VRChatMaterial Backup/Restoreツール
    /// なんｊ風に言うと「これで完璧なバックアップシステムが完成したぜ！」💪🔥
    /// </summary>
    public class VRChatMaterialBackup : EditorWindow
    {
        private static readonly string BackupSubfolder = "VRChatMaterialBackups";
        // 既定のファイル名（未使用警告を避けつつ既定値を明示）
        // 未使用のためコメントアウト（将来の互換維持のため値は保持）
        // private static readonly string BackupFileName = "vrchat_material_backup.json";
        private static readonly string BackupVersion = "2.2.0";
        
        private GameObject targetAvatar;
        private VRCAvatarDescriptor avatarDescriptor;
        private Vector2 scrollPosition;
        // 将来的な拡張用。現状未使用のため警告回避でコメントアウト
        // private bool showAdvancedOptions = false;
        private bool autoBackupOnUpload = true;
        private bool backupTextures = true;
        private bool backupShaderProperties = true;
        private bool createBackupBeforeUpload = true;

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/VRChat Material Backup")]
        public static void ShowWindow()
        {
            GetWindow<VRChatMaterialBackup>("VRChat Material Backup");
        }

        private void OnEnable()
        {
            FindAvatarInScene();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            GUILayout.Label("🎯 VRChat Material Backup", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            // アバター選択
            EditorGUILayout.LabelField(" Avatar Selection", EditorStyles.miniBoldLabel);
            targetAvatar = (GameObject)EditorGUILayout.ObjectField("Target Avatar", targetAvatar, typeof(GameObject), true);
            
            if (targetAvatar != null)
            {
                avatarDescriptor = targetAvatar.GetComponent<VRCAvatarDescriptor>();
                if (avatarDescriptor != null)
                {
                    EditorGUILayout.LabelField($"Avatar Name: {targetAvatar.name}");
                    EditorGUILayout.LabelField($"Avatar ID: {avatarDescriptor.name}");
                }
            }
            
            EditorGUILayout.Space(10);
            
            // バックアップオプション
            EditorGUILayout.LabelField("⚙️ Backup Options", EditorStyles.miniBoldLabel);
            autoBackupOnUpload = EditorGUILayout.Toggle("Auto backup on upload", autoBackupOnUpload);
            backupTextures = EditorGUILayout.Toggle("Backup textures", backupTextures);
            backupShaderProperties = EditorGUILayout.Toggle("Backup shader properties", backupShaderProperties);
            createBackupBeforeUpload = EditorGUILayout.Toggle("Create backup before upload", createBackupBeforeUpload);
            
            EditorGUILayout.Space(10);
            
            // メイン機能
            EditorGUILayout.LabelField("🚀 Main Functions", EditorStyles.miniBoldLabel);
            
            if (GUILayout.Button("🔍 Find Avatar in Scene", GUILayout.Height(30)))
            {
                FindAvatarInScene();
            }
            
            if (GUILayout.Button("💾 Create Material Backup", GUILayout.Height(30)))
            {
                if (targetAvatar != null)
                {
                    CreateMaterialBackup(targetAvatar);
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Please select an avatar first", "OK");
                }
            }
            
            if (GUILayout.Button("🔄 Restore from Backup", GUILayout.Height(30)))
            {
                if (targetAvatar != null)
                {
                    RestoreMaterialsFromBackup(targetAvatar);
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Please select an avatar first", "OK");
                }
            }
            
            if (GUILayout.Button("🍞 Bake Materials for Upload (Fix Missing)", GUILayout.Height(34)))
            {
                if (targetAvatar != null)
                {
                    BakeMaterialsForUpload(targetAvatar, forcePCSSShader:false);
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Please select an avatar first", "OK");
                }
            }
            
            if (GUILayout.Button("🔍 Scan for Missing Materials", GUILayout.Height(30)))
            {
                if (targetAvatar != null)
                {
                    ScanForMissingMaterials();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Please select an avatar first", "OK");
                }
            }
            
            if (GUILayout.Button("️ Clear All Backups", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to clear all backups?", "Yes", "No"))
                {
                    ClearAllBackups();
                }
            }
            
            EditorGUILayout.Space(10);
            
            // バックアップ情報表示
            EditorGUILayout.LabelField(" Backup Information", EditorStyles.miniBoldLabel);
            DisplayBackupInfo();
            
            EditorGUILayout.EndScrollView();
        }

        private void FindAvatarInScene()
        {
            var avatars = FindObjectsOfType<VRCAvatarDescriptor>();
            if (avatars.Length > 0)
            {
                targetAvatar = avatars[0].gameObject;
                Debug.Log($"Found avatar: {targetAvatar.name}");
            }
            else
            {
                Debug.LogWarning("No VRChat avatar found in scene");
            }
        }

        private void CreateMaterialBackup(GameObject avatar)
        {
            try
            {
                var backupData = new VRChatMaterialBackupData
                {
                    avatarName = avatar.name,
                    backupTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    version = BackupVersion,
                    materials = new List<MaterialBackupEntry>()
                };

                var renderers = avatar.GetComponentsInChildren<Renderer>();
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

                string backupPath = GetBackupFolderPath(avatar.name);
                string backupFile = Path.Combine(backupPath, $"MaterialBackup_{DateTime.Now:yyyyMMdd_HHmmss}.json");
                
                string json = JsonUtility.ToJson(backupData, true);
                File.WriteAllText(backupFile, json);
                
                Debug.Log($"Material backup created: {backupFile}");
                EditorUtility.DisplayDialog("Success", $"Material backup created successfully!\nPath: {backupFile}", "OK");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create material backup: {ex.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to create backup: {ex.Message}", "OK");
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
            if (backupShaderProperties && material.shader != null)
            {
                var shader = material.shader;
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
                            if (backupTextures)
                            {
                                var texture = material.GetTexture(propertyName);
                                if (texture != null)
                                {
                                    entry.textureGUIDs[propertyName] = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(texture));
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
            if (avatar == null) return;

            string avatarName = avatar.name;
            string backupPath = GetBackupFolderPath(avatarName);
            
            if (!Directory.Exists(backupPath))
            {
                EditorUtility.DisplayDialog("Error", "No backup folder found!", "OK");
                return;
            }

            var backupFiles = Directory.GetFiles(backupPath, "MaterialBackup_*.json");
            if (backupFiles.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "No backup files found!", "OK");
                return;
            }

            // 最新のバックアップファイルを選択
            string latestBackup = backupFiles.OrderByDescending(f => File.GetLastWriteTime(f)).First();
            string jsonData = File.ReadAllText(latestBackup);
            var backupData = JsonUtility.FromJson<VRChatMaterialBackupData>(jsonData);
            
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
                    var restoredMat = RestoreMaterialFromBackup(backupEntry);
                    if (restoredMat != null)
                    {
                        var mats = renderer.sharedMaterials;
                        if (backupEntry.materialIndex < mats.Length)
                        {
                            mats[backupEntry.materialIndex] = restoredMat;
                            renderer.sharedMaterials = mats;
                            EditorUtility.SetDirty(renderer);
                            restoredCount++;
                        }
                    }
                }
            }

            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Restore Complete", $"Restored {restoredCount} materials from backup.", "OK");
        }

        private MaterialBackupEntry FindBackupEntry(VRChatMaterialBackupData backupData, Renderer renderer, int materialIndex)
        {
            string rendererPath = GetGameObjectPath(renderer.gameObject);
            return backupData.materials.FirstOrDefault(e => e.rendererPath == rendererPath && e.materialIndex == materialIndex);
        }

        private Material RestoreMaterialFromBackup(MaterialBackupEntry backupEntry)
        {
            // マテリアルアセットの復元を試行
            if (!string.IsNullOrEmpty(backupEntry.materialGUID))
            {
                string materialPath = AssetDatabase.GUIDToAssetPath(backupEntry.materialGUID);
                if (!string.IsNullOrEmpty(materialPath))
                {
                    var loadedMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                    if (loadedMaterial != null)
                        return loadedMaterial;
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
                            if (colorValues.Length == 4)
                            {
                                float.TryParse(colorValues[0], out float r);
                                float.TryParse(colorValues[1], out float g);
                                float.TryParse(colorValues[2], out float b);
                                float.TryParse(colorValues[3], out float a);
                                newMaterial.SetColor(propertyName, new Color(r, g, b, a));
                            }
                        }
                    }
                    else if (propertyValue.StartsWith("(") && propertyValue.EndsWith(")"))
                    {
                        // Vector4型の復元
                        newMaterial.SetVector(propertyName, StringToVector4(propertyValue));
                    }
                    else
                    {
                        // Float型の復元
                        if (float.TryParse(propertyValue, out float floatValue))
                        {
                            newMaterial.SetFloat(propertyName, floatValue);
                        }
                    }
                }
            }

            // テクスチャの復元
            foreach (var kvp in backupEntry.textureGUIDs)
            {
                string propertyName = kvp.Key;
                string textureGUID = kvp.Value;

                if (!string.IsNullOrEmpty(textureGUID))
                {
                    string texturePath = AssetDatabase.GUIDToAssetPath(textureGUID);
                    if (!string.IsNullOrEmpty(texturePath))
                    {
                        var texture = AssetDatabase.LoadAssetAtPath<Texture>(texturePath);
                        if (texture != null && newMaterial.HasProperty(propertyName))
                        {
                            newMaterial.SetTexture(propertyName, texture);
                        }
                    }
                }
            }

            return newMaterial;
        }

        private void ScanForMissingMaterials()
        {
            if (targetAvatar == null) return;

            int count = 0;
            var renderers = targetAvatar.GetComponentsInChildren<Renderer>();
            
            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] == null)
                    {
                        count++;
                        Debug.LogWarning($"Missing material found: {renderer.name} at index {i}");
                    }
                }
            }
            
            EditorUtility.DisplayDialog("Scan Result", $"Missing materials found: {count}", "OK");
        }

        private void ClearAllBackups()
        {
            string backupRootPath = Path.Combine("Assets", BackupSubfolder);
            if (Directory.Exists(backupRootPath))
            {
                Directory.Delete(backupRootPath, true);
                Debug.Log("All backups cleared");
            }
        }

        /// <summary>
        /// アップロード安全化用に、使用中マテリアルをAssets配下に複製し再割当。透明物の描画順/ZWriteも調整
        /// </summary>
        private void BakeMaterialsForUpload(GameObject avatar, bool forcePCSSShader)
        {
            try
            {
                string bakedRoot = Path.Combine("Assets", BackupSubfolder, "Baked", avatar.name);
                if (!Directory.Exists(bakedRoot)) Directory.CreateDirectory(bakedRoot);

                var renderers = avatar.GetComponentsInChildren<Renderer>(true);
                var originalToBaked = new Dictionary<Material, Material>();
                int reassigned = 0;

                Shader pcssShader = Shader.Find("lilToon/PCSS Extension");
                Shader lilToonShader = Shader.Find("lilToon");

                foreach (var renderer in renderers)
                {
                    if (renderer == null) continue;
                    var mats = renderer.sharedMaterials;
                    bool changed = false;
                    for (int i = 0; i < mats.Length; i++)
                    {
                        var src = mats[i];
                        if (src == null)
                        {
                            // 足りない場合はデフォルトlilToon/PCSSで埋める
                            var fallbackShader = pcssShader != null ? pcssShader : lilToonShader;
                            if (fallbackShader == null) continue;
                            var newMat = new Material(fallbackShader) { name = $"{renderer.name}_Mat_{i}" };
                            EnsureSafeRenderingSettings(newMat);
                            string path = Path.Combine(bakedRoot, $"{SanitizeFileName(newMat.name)}.mat");
                            AssetDatabase.CreateAsset(newMat, AssetDatabase.GenerateUniqueAssetPath(path));
                            mats[i] = newMat;
                            changed = true;
                            reassigned++;
                            continue;
                        }

                        if (!originalToBaked.TryGetValue(src, out var baked))
                        {
                            // 既存シェーダーが見つからない場合はフォールバック
                            Shader useShader = src.shader;
                            if (useShader == null)
                                useShader = forcePCSSShader && pcssShader != null ? pcssShader : (lilToonShader ?? pcssShader);

                            var dup = new Material(src);
                            if (forcePCSSShader && pcssShader != null) dup.shader = pcssShader;
                            if (dup.shader == null) dup.shader = useShader;
                            EnsureSafeRenderingSettings(dup);

                            string matName = SanitizeFileName(src.name);
                            string path = Path.Combine(bakedRoot, $"{matName}.mat");
                            path = AssetDatabase.GenerateUniqueAssetPath(path);
                            AssetDatabase.CreateAsset(dup, path);
                            originalToBaked[src] = dup;
                            baked = dup;
                        }

                        if (mats[i] != baked)
                        {
                            mats[i] = baked;
                            changed = true;
                            reassigned++;
                        }
                    }
                    if (changed)
                    {
                        renderer.sharedMaterials = mats;
                        EditorUtility.SetDirty(renderer);
                        if (PrefabUtility.IsPartOfPrefabInstance(renderer.gameObject))
                        {
                            PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
                        }
                    }
                }

                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Bake Complete", $"Baked and reassigned {originalToBaked.Count} unique materials. Reassigned slots: {reassigned}", "OK");
            }
            catch (Exception ex)
            {
                Debug.LogError($"BakeMaterialsForUpload failed: {ex.Message}\n{ex.StackTrace}");
                EditorUtility.DisplayDialog("Error", $"Bake failed: {ex.Message}", "OK");
            }
        }

        private static void EnsureSafeRenderingSettings(Material mat)
        {
            // 透明判定のヒューリスティクス
            bool isTransparent = mat.renderQueue >= 3000 ||
                                  mat.IsKeywordEnabled("_TRANSPARENT_ON") ||
                                  (mat.HasProperty("_Transparent") && mat.GetFloat("_Transparent") > 0.5f) ||
                                  (mat.HasProperty("_BlendMode") && mat.GetFloat("_BlendMode") >= 3.0f);

            bool isCutout = (!isTransparent) &&
                            (mat.HasProperty("_Cutoff") && mat.GetFloat("_Cutoff") > 0.0f ||
                             mat.IsKeywordEnabled("_ALPHATEST_ON"));

            if (isTransparent)
            {
                mat.renderQueue = 3000; // Transparent
                if (mat.HasProperty("_ZWrite")) mat.SetFloat("_ZWrite", 0f);
                // lilToon系の透明スイッチ類がある場合も合わせる
                if (mat.HasProperty("_AddBlendMode")) mat.SetFloat("_AddBlendMode", 0f);
            }
            else if (isCutout)
            {
                mat.renderQueue = 2450; // AlphaTest
                if (mat.HasProperty("_ZWrite")) mat.SetFloat("_ZWrite", 1f);
            }
            else
            {
                mat.renderQueue = 2000; // Opaque
                if (mat.HasProperty("_ZWrite")) mat.SetFloat("_ZWrite", 1f);
            }
        }

        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name;
        }

        private void DisplayBackupInfo()
        {
            if (targetAvatar == null)
            {
                EditorGUILayout.LabelField("No avatar selected");
                return;
            }

            string avatarName = targetAvatar.name;
            string backupPath = GetBackupFolderPath(avatarName);
            
            if (Directory.Exists(backupPath))
            {
                var backupFiles = Directory.GetFiles(backupPath, "MaterialBackup_*.json");
                EditorGUILayout.LabelField($"Backup files: {backupFiles.Length}");
                
                if (backupFiles.Length > 0)
                {
                    var latestBackup = backupFiles.OrderByDescending(f => File.GetLastWriteTime(f)).First();
                    EditorGUILayout.LabelField($"Latest backup: {Path.GetFileName(latestBackup)}");
                    EditorGUILayout.LabelField($"Last modified: {File.GetLastWriteTime(latestBackup)}");
                }
            }
            else
            {
                EditorGUILayout.LabelField("No backup folder found");
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
}