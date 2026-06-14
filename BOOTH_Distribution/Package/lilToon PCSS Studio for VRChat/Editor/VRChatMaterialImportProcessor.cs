using UnityEditor;
using UnityEngine;

public class VRChatMaterialImportProcessor : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        var importer = (ModelImporter)assetImporter;
        importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
        importer.materialLocation = ModelImporterMaterialLocation.External;
        importer.materialSearch = ModelImporterMaterialSearch.Everywhere; // reuse existing materials if found
    }

    void OnPostprocessMaterial(Material material)
    {
        if (VRChatLilToonApplier.TryApplyLilToon(material, force: false, out var shaderName) && !string.IsNullOrEmpty(shaderName))
        {
            Debug.Log($"[MaterialAuto] Applied shader '{shaderName}' to '{material.name}'");
        }
    }
}


