using UnityEditor;

// FBX/ã¢ãE«ã¤ã³ããEãæã®èªåãEãEªã¢ã«åå²ãå½ã¦
public class AutoMaterialRemapper : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        ModelImporter modelImporter = assetImporter as ModelImporter;
        if (modelImporter != null)
        {
            // ãããªã¢ã«åã§ãã­ã¸ã§ã¯ãåEä½ããèEåæ¤ç´¢ã»åå²ãå½ã¦
            modelImporter.SearchAndRemapMaterials(
                ModelImporterMaterialName.BasedOnMaterialName,
                ModelImporterMaterialSearch.Everywhere
            );
        }
    }
} 
