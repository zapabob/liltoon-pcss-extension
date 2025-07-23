using UnityEditor;

// FBX/繝｢繝・Ν繧､繝ｳ繝昴・繝域凾縺ｮ閾ｪ蜍輔・繝・Μ繧｢繝ｫ蜀榊牡繧雁ｽ薙※
public class AutoMaterialRemapper : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        ModelImporter modelImporter = assetImporter as ModelImporter;
        if (modelImporter != null)
        {
            // 繝槭ユ繝ｪ繧｢繝ｫ蜷阪〒繝励Ο繧ｸ繧ｧ繧ｯ繝亥・菴薙°繧芽・蜍墓､懃ｴ｢繝ｻ蜀榊牡繧雁ｽ薙※
            modelImporter.SearchAndRemapMaterials(
                ModelImporterMaterialName.BasedOnMaterialName,
                ModelImporterMaterialSearch.Everywhere
            );
        }
    }
} 
