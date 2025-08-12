using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class VRChatMaterialTools
{
    [MenuItem("Tools/VRChat/All-in-One/1) Import PCSS UnityPackage...")]
    public static void ImportPCSSUnityPackage()
    {
        string path = EditorUtility.OpenFilePanel("Import lilToon PCSS Extension Package", "", "unitypackage");
        if (string.IsNullOrEmpty(path)) return;
        AssetDatabase.ImportPackage(path, false);
        Debug.Log($"[MaterialAuto] Imported package: {path}");
    }

    [MenuItem("Tools/VRChat/All-in-One/2) Apply lilToon PCSS to All Materials")]
    public static void ApplyPCSSAll()
    {
        ApplyToAllMaterials(preferPCSS: true);
    }

    [MenuItem("Tools/VRChat/Apply lilToon to Selected Materials")]
    public static void ApplyToSelectedMaterialsMenu()
    {
        var selectedMaterials = Selection.objects.OfType<Material>().ToArray();
        if (selectedMaterials.Length == 0)
        {
            Debug.LogWarning("[MaterialAuto] No materials selected.");
            return;
        }

        int changed = 0;
        foreach (var mat in selectedMaterials)
        {
            if (VRChatLilToonApplier.TryApplyLilToon(mat, force: true, out _))
            {
                EditorUtility.SetDirty(mat);
                changed++;
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"[MaterialAuto] Changed {changed}/{selectedMaterials.Length} materials.");
    }

    [MenuItem("Tools/VRChat/Apply lilToon to All Materials in Project")]
    public static void ApplyToAllMaterialsMenu()
    {
        ApplyToAllMaterials(preferPCSS: false);
    }

    private static void ApplyToAllMaterials(bool preferPCSS)
    {
        // preferPCSS は現在の実装では ShaderCandidates の順序で既にPCSS優先
        var guids = AssetDatabase.FindAssets("t:Material");
        int total = guids.Length;
        int changed = 0;
        for (int i = 0; i < guids.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;
            if (VRChatLilToonApplier.TryApplyLilToon(mat, force: false, out _))
            {
                EditorUtility.SetDirty(mat);
                changed++;
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"[MaterialAuto] Changed {changed}/{total} materials.");
    }

    [MenuItem("Tools/VRChat/Reimport Selected Models with External Materials")]
    public static void ReimportSelectedModels()
    {
        var selected = Selection.assetGUIDs
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => p.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase) || p.EndsWith(".obj", StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .ToArray();

        if (selected.Length == 0)
        {
            Debug.LogWarning("[MaterialAuto] No model assets selected (.fbx/.obj).");
            return;
        }

        int count = 0;
        foreach (var path in selected)
        {
            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null) continue;
            importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            importer.materialLocation = ModelImporterMaterialLocation.External;
            importer.materialSearch = ModelImporterMaterialSearch.Everywhere;
            importer.SaveAndReimport();
            count++;
        }
        Debug.Log($"[MaterialAuto] Reimported {count} model assets.");
    }
}


