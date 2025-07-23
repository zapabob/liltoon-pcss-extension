using UnityEditor;

// FBX/モチE��インポ�Eト時の自動�EチE��アル再割り当て
public class AutoMaterialRemapper : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        ModelImporter modelImporter = assetImporter as ModelImporter;
        if (modelImporter != null)
        {
            // マテリアル名でプロジェクト�E体から�E動検索・再割り当て
            modelImporter.SearchAndRemapMaterials(
                ModelImporterMaterialName.BasedOnMaterialName,
                ModelImporterMaterialSearch.Everywhere
            );
        }
    }
} 
