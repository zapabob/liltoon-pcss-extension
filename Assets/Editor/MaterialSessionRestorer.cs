using UnityEditor;

// FBX/モデルインポート時の自動マテリアル再割り当て
public class AutoMaterialRemapper : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        ModelImporter modelImporter = assetImporter as ModelImporter;
        if (modelImporter != null)
        {
            // マテリアル名でプロジェクト全体から自動検索・再割り当て
            modelImporter.SearchAndRemapMaterials(
                ModelImporterMaterialName.BasedOnMaterialName,
                ModelImporterMaterialSearch.Everywhere
            );
        }
    }
} 