using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class MaterialBackup : EditorWindow
{
    private static readonly string BackupSubfolder = "MaterialBackups";
    private GameObject targetObject;

    [MenuItem("Tools/Material Backup")]
    public static void ShowWindow()
    {
        GetWindow<MaterialBackup>("Material Backup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Material Backup and Restore", EditorStyles.boldLabel);
        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Avatar or GameObject", targetObject, typeof(GameObject), true);

        if (GUILayout.Button("Backup Materials"))
        {
            if (targetObject != null)
            {
                BackupMaterials(targetObject);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please select a target GameObject.", "OK");
            }
        }

        if (GUILayout.Button("Restore Materials"))
        {
            if (targetObject != null)
            {
                RestoreMaterials(targetObject);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please select a target GameObject.", "OK");
            }
        }
    }

    private static string GetBackupFolderPath(string objectName)
    {
        string backupRootPath = Path.Combine("Assets", BackupSubfolder);
        if (!Directory.Exists(backupRootPath))
        {
            Directory.CreateDirectory(backupRootPath);
        }
        string objectBackupPath = Path.Combine(backupRootPath, objectName);
        if (!Directory.Exists(objectBackupPath))
        {
            Directory.CreateDirectory(objectBackupPath);
        }
        return objectBackupPath;
    }

    public static void BackupMaterials(GameObject obj)
    {
        string backupFolderPath = GetBackupFolderPath(obj.name);
        var renderers = obj.GetComponentsInChildren<Renderer>(true);
        var backupList = new List<MaterialBackupData>();
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.sharedMaterials)
            {
                if (material != null)
                {
                    var data = new MaterialBackupData();
                    data.guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(material));
                    data.path = AssetDatabase.GetAssetPath(material);
                    data.shaderName = material.shader.name;
                    data.backupTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    data.version = "2.0.0";
                    var shader = material.shader;
                    int count = ShaderUtil.GetPropertyCount(shader);
                    for (int i = 0; i < count; i++)
                    {
                        var propName = ShaderUtil.GetPropertyName(shader, i);
                        var propType = ShaderUtil.GetPropertyType(shader, i);
                        switch (propType)
                        {
                            case ShaderUtil.ShaderPropertyType.Color:
                                data.properties[propName] = ColorUtility.ToHtmlStringRGBA(material.GetColor(propName));
                                break;
                            case ShaderUtil.ShaderPropertyType.Vector:
                                data.properties[propName] = material.GetVector(propName).ToString();
                                break;
                            case ShaderUtil.ShaderPropertyType.Float:
                            case ShaderUtil.ShaderPropertyType.Range:
                                data.properties[propName] = material.GetFloat(propName).ToString();
                                break;
                            case ShaderUtil.ShaderPropertyType.TexEnv:
                                var tex = material.GetTexture(propName);
                                if (tex != null)
                                {
                                    var texPath = AssetDatabase.GetAssetPath(tex);
                                    data.textureGuids[propName] = AssetDatabase.AssetPathToGUID(texPath);
                                }
                                break;
                        }
                    }
                    backupList.Add(data);
                }
            }
        }
        // JSON保存
        string json = JsonUtility.ToJson(new MaterialBackupListWrapper { list = backupList }, true);
        File.WriteAllText(Path.Combine(backupFolderPath, "material_backup.json"), json);
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Success", $"Materials for {obj.name} have been GUID-backuped to {backupFolderPath}", "OK");
    }

    public static void RestoreMaterials(GameObject obj)
    {
        string backupFolderPath = GetBackupFolderPath(obj.name);
        string jsonPath = Path.Combine(backupFolderPath, "material_backup.json");
        if (!File.Exists(jsonPath))
        {
            EditorUtility.DisplayDialog("Error", "No backup found for this object.", "OK");
            return;
        }
        var backupList = JsonUtility.FromJson<MaterialBackupListWrapper>(File.ReadAllText(jsonPath)).list;
        var renderers = obj.GetComponentsInChildren<Renderer>(true);
        foreach (var renderer in renderers)
        {
            var sharedMaterials = renderer.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                var material = sharedMaterials[i];
                if (material != null)
                {
                    string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(material));
                    var backupData = backupList.FirstOrDefault(b => b.guid == guid);
                    if (backupData == null)
                    {
                        Debug.LogWarning($"No backup found for material GUID: {guid}");
                        continue;
                    }
                    if (material.shader.name != backupData.shaderName)
                    {
                        Debug.LogWarning($"Shader mismatch: {material.shader.name} != {backupData.shaderName}");
                        continue;
                    }
                    var shader = material.shader;
                    foreach (var kv in backupData.properties)
                    {
                        if (material.HasProperty(kv.Key))
                        {
                            var propType = ShaderUtil.GetPropertyType(shader, ShaderUtil.GetPropertyIndex(shader, kv.Key));
                            switch (propType)
                            {
                                case ShaderUtil.ShaderPropertyType.Color:
                                    if (ColorUtility.TryParseHtmlString("#" + kv.Value, out var color))
                                        material.SetColor(kv.Key, color);
                                    break;
                                case ShaderUtil.ShaderPropertyType.Vector:
                                    var vec = StringToVector4(kv.Value);
                                    material.SetVector(kv.Key, vec);
                                    break;
                                case ShaderUtil.ShaderPropertyType.Float:
                                case ShaderUtil.ShaderPropertyType.Range:
                                    if (float.TryParse(kv.Value, out var f))
                                        material.SetFloat(kv.Key, f);
                                    break;
                            }
                        }
                    }
                    foreach (var kv in backupData.textureGuids)
                    {
                        var texPath = AssetDatabase.GUIDToAssetPath(kv.Value);
                        var tex = AssetDatabase.LoadAssetAtPath<Texture>(texPath);
                        if (tex != null)
                            material.SetTexture(kv.Key, tex);
                        else
                            Debug.LogWarning($"Texture not found for GUID: {kv.Value}");
                    }
                    EditorUtility.SetDirty(material);
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", "Materials have been GUID-restored from backup.", "OK");
    }
}

[System.Serializable]
public class MaterialBackupData
{
    public string guid;
    public string path;
    public string shaderName;
    public Dictionary<string, string> textureGuids = new();
    public Dictionary<string, string> properties = new();
    public string backupTime;
    public string version;
}

[System.Serializable]
public class MaterialBackupListWrapper
{
    public List<MaterialBackupData> list;
}

private static Vector4 StringToVector4(string s)
{
    // UnityのVector4.ToString()形式: (x, y, z, w)
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