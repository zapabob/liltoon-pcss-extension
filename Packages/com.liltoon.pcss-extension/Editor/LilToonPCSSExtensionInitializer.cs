using UnityEditor;
using UnityEngine;

public class LilToonPCSSExtensionInitializer : Editor
{
    private static string PCSS_EXTENSION_SHADER_NAME = "LilToonPCSS";

    [MenuItem("Tools/LilToon PCSS Extension/Initialize")]
    public static void Initialize()
    {
        // プロジェクト内のマテリアルも修復
        string[] materialGuids = AssetDatabase.FindAssets("t:Material");
        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null) continue;
            bool needsFix = false;
            if (material.shader == null)
            {
                needsFix = true;
            }
            else if (material.shader.name != PCSS_EXTENSION_SHADER_NAME)
            {
                if (material.shader.name.Contains("lilToon"))
                    needsFix = true;
            }
            else if (material.shader.name == PCSS_EXTENSION_SHADER_NAME)
            {
                needsFix = true;
            }
            if (needsFix && pcssShader != null)
            {
                material.shader = pcssShader;
                EnsureRequiredProperties(material);
                SetupShaderKeywords(material);
                if (!fixedMaterials.Contains(material))
                    fixedMaterials.Add(material);
                EditorUtility.SetDirty(material);
            }
        }
    }

    private static void EnsureRequiredProperties(Material material)
    {
        // Implementation of EnsureRequiredProperties method
    }

    private static void SetupShaderKeywords(Material material)
    {
        // Implementation of SetupShaderKeywords method
    }

    private static Material pcssShader;
    private static List<Material> fixedMaterials = new List<Material>();
} 