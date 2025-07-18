using UnityEditor;
using UnityEngine;
using System.Linq;

namespace lilToon.PCSS.Editor
{
    public class LilToonPCSSMaterialUpgrader2
    {
        private const string MenuRoot = "Tools/lilToon PCSS Extension/";
        private const string UtilitiesMenu = MenuRoot + "Utilities/";
        private const string LilToonMenu = UtilitiesMenu + "lilToon/";
        private const string UpgradeMenuPath = LilToonMenu + "Upgrade Selected Materials to PCSS Extension v2";

        private const string LILTOON_SHADER_NAME = "lilToon";
        private const string PCSS_EXTENSION_SHADER_NAME = "lilToon/PCSS Extension";

        [MenuItem(UpgradeMenuPath, true)]
        private static bool ValidateUpgradeMaterialsV2()
        {
            return Selection.GetFiltered<Material>(SelectionMode.Assets).Any(m => m.shader.name.Contains(LILTOON_SHADER_NAME) && m.shader.name != PCSS_EXTENSION_SHADER_NAME);
        }

        [MenuItem(UpgradeMenuPath)]
        private static void UpgradeMaterialsV2()
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
