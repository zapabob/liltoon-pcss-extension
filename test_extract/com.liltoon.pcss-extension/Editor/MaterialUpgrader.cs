 using UnityEditor;
using UnityEngine;
using System.Linq;

namespace lilToon.PCSS.Editor
{
    public class LilToonPCSSMaterialUpgrader
    {
        private const string LILTOON_SHADER_NAME = "lilToon";
        private const string PCSS_EXTENSION_SHADER_NAME = "lilToon/PCSS Extension";

        [MenuItem("Assets/lilToon/Upgrade Selected Materials to PCSS Extension", true)]
        private static bool ValidateUpgradeMaterials()
        {
            return Selection.GetFiltered<Material>(SelectionMode.Assets).Any(m => m.shader.name.Contains(LILTOON_SHADER_NAME) && m.shader.name != PCSS_EXTENSION_SHADER_NAME);
        }

        [MenuItem("Assets/lilToon/Upgrade Selected Materials to PCSS Extension")]
        private static void UpgradeMaterials()
        {
            Shader pcssShader = Shader.Find(PCSS_EXTENSION_SHADER_NAME);
            if (pcssShader == null)
            {
                EditorUtility.DisplayDialog("Error", $"Shader '{PCSS_EXTENSION_SHADER_NAME}' not found. Please ensure the PCSS Extension is installed correctly.", "OK");
                return;
            }

            var materialsToUpgrade = Selection.GetFiltered<Material>(SelectionMode.Assets)
                                              .Where(m => m.shader.name.Contains(LILTOON_SHADER_NAME) && m.shader.name != PCSS_EXTENSION_SHADER_NAME)
                                              .ToList();

            if (materialsToUpgrade.Count == 0)
            {
                EditorUtility.DisplayDialog("No Materials to Upgrade", "No selected materials are using a standard lilToon shader.", "OK");
                return;
            }
            
            Undo.RecordObjects(materialsToUpgrade.ToArray(), "Upgrade to PCSS Material");

            int upgradedCount = 0;
            foreach (var material in materialsToUpgrade)
            {
                Debug.Log($"Upgrading material '{material.name}' from '{material.shader.name}' to '{PCSS_EXTENSION_SHADER_NAME}'.");
                material.shader = pcssShader;
                upgradedCount++;
                EditorUtility.SetDirty(material);
            }
            
            AssetDatabase.SaveAssets();
            
            EditorUtility.DisplayDialog("Upgrade Complete", $"{upgradedCount} material(s) have been upgraded to use the lilToon PCSS Extension shader.", "OK");
        }
    }
} 