using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class MaterialBackup : EditorWindow
{
    private static readonly string BackupSubfolder = "MaterialBackups";
    private GameObject targetObject;

    [MenuItem("Tools/Material Backup")]
    public static void ShowWindow()
    {
        PCSSSetupCenterWindow.ShowWindow();
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
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.sharedMaterials)
            {
                if (material != null)
                {
                    MaterialBackupData backupData = CreateInstance<MaterialBackupData>();
                    backupData.shaderName = material.shader.name;
                    
                    string backupPath = Path.Combine(backupFolderPath, material.name + ".asset");
                    AssetDatabase.CreateAsset(backupData, backupPath);
                }
            }
        }
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Success", $"Materials for {obj.name} have been backed up to {backupFolderPath}", "OK");
    }

    public static void RestoreMaterials(GameObject obj)
    {
        string backupFolderPath = GetBackupFolderPath(obj.name);
        if (!Directory.Exists(backupFolderPath))
        {
            EditorUtility.DisplayDialog("Error", "No backup found for this object.", "OK");
            return;
        }

        var renderers = obj.GetComponentsInChildren<Renderer>(true);
        foreach (var renderer in renderers)
        {
            var sharedMaterials = renderer.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                var material = sharedMaterials[i];
                if (material != null)
                {
                    string backupPath = Path.Combine(backupFolderPath, material.name + ".asset");
                    if (File.Exists(backupPath))
                    {
                        MaterialBackupData backupData = AssetDatabase.LoadAssetAtPath<MaterialBackupData>(backupPath);
                        if (backupData != null)
                        {
                            Shader shader = Shader.Find(backupData.shaderName);
                            if (shader != null)
                            {
                                material.shader = shader;
                                EditorUtility.SetDirty(material);
                            }
                            else
                            {
                                Debug.LogWarning($"Shader '{backupData.shaderName}' not found for material '{material.name}'.");
                            }
                        }
                    }
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", "Materials have been restored from backup.", "OK");
    }
}

public class MaterialBackupData : ScriptableObject
{
    public string shaderName;
}