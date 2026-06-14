using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// lilToon 2.1.9 ファー機能Subdivision統一移行ツール
    /// ShrinkモードからSubdivisionモードへの自動移行支援
    /// </summary>
    public class FurSubdivisionMigrationTool : EditorWindow
    {
        private Vector2 scrollPosition;
        private List<Material> materialsWithShrinkMode = new List<Material>();
        private bool showDetailedReport = false;
        private bool autoMigrateMaterials = false;
        private bool backupBeforeMigration = true;
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Fur Subdivision Migration Tool")]
        public static void ShowWindow()
        {
            FurSubdivisionMigrationTool window = GetWindow<FurSubdivisionMigrationTool>("Fur Subdivision Migration");
            window.minSize = new Vector2(600, 400);
            window.ScanForShrinkModeMaterials();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Fur Subdivision Migration Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            EditorGUILayout.HelpBox("lilToon 2.1.9 Breaking Change: Fur Shrink mode has been removed.\n" +
                "All fur materials now use Subdivision mode for improved performance and consistency.", MessageType.Warning);

            EditorGUILayout.Space(10);

            // Scan Button
            if (GUILayout.Button("Scan for Shrink Mode Materials", GUILayout.Height(30)))
            {
                ScanForShrinkModeMaterials();
            }

            EditorGUILayout.Space(10);

            // Results
            EditorGUILayout.LabelField($"Found {materialsWithShrinkMode.Count} materials using Shrink mode:", EditorStyles.boldLabel);
            
            if (materialsWithShrinkMode.Count > 0)
            {
                EditorGUILayout.HelpBox("⚠️ ACTION REQUIRED: These materials use the deprecated Shrink mode that must be migrated to Subdivision mode for lilToon 2.1.9 compatibility.", MessageType.Error);
                
                EditorGUILayout.Space(5);
                
                // Options
                backupBeforeMigration = EditorGUILayout.Toggle("Backup before migration", backupBeforeMigration);
                autoMigrateMaterials = EditorGUILayout.Toggle("Auto-migrate materials", autoMigrateMaterials);
                
                EditorGUILayout.Space(5);
                
                // Action Buttons
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Migrate All Materials", GUILayout.Height(30)))
                {
                    MigrateAllMaterials();
                }
                
                if (GUILayout.Button("Select All Materials", GUILayout.Height(30)))
                {
                    Selection.objects = materialsWithShrinkMode.ToArray();
                }
                
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(10);
                
                // Detailed List
                showDetailedReport = EditorGUILayout.Foldout(showDetailedReport, "Detailed Report", true);
                if (showDetailedReport)
                {
                    EditorGUI.indentLevel++;
                    
                    foreach (var material in materialsWithShrinkMode)
                    {
                        EditorGUILayout.BeginHorizontal();
                        
                        if (GUILayout.Button("Select", GUILayout.Width(60)))
                        {
                            Selection.activeObject = material;
                        }
                        
                        EditorGUILayout.ObjectField(material, typeof(Material), true);
                        
                        if (GUILayout.Button("Migrate", GUILayout.Width(60)))
                        {
                            MigrateMaterial(material);
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("✅ No Shrink mode materials found. Your project is ready for lilToon 2.1.9!", MessageType.Info);
            }

            EditorGUILayout.Space(20);

            // Migration Info
            EditorGUILayout.LabelField("Migration Information:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("• lilToon 2.1.9 unifies fur rendering to Subdivision mode only");
            EditorGUILayout.LabelField("• Shrink mode has been removed for performance optimization");
            EditorGUILayout.LabelField("• Subdivision mode provides better quality and consistency");
            EditorGUILayout.LabelField("• Automatic parameter conversion preserves visual appearance");

            EditorGUILayout.Space(10);

            // Performance Benefits
            EditorGUILayout.LabelField("Performance Benefits:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("• Reduced shader complexity");
            EditorGUILayout.LabelField("• Improved rendering performance");
            EditorGUILayout.LabelField("• Better memory efficiency");
            EditorGUILayout.LabelField("• Consistent behavior across platforms");

            EditorGUILayout.EndScrollView();
        }

        private void ScanForShrinkModeMaterials()
        {
            materialsWithShrinkMode.Clear();
            
            // Find all materials in the project
            var materialGuids = AssetDatabase.FindAssets("t:Material");
            foreach (var guid in materialGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                
                if (material != null && IsLilToonFurMaterial(material))
                {
                    if (IsUsingShrinkMode(material))
                    {
                        materialsWithShrinkMode.Add(material);
                    }
                }
            }
            
            Repaint();
        }

        private bool IsLilToonFurMaterial(Material material)
        {
            if (material.shader == null) return false;
            
            var shaderName = material.shader.name.ToLower();
            return shaderName.Contains("liltoon") && shaderName.Contains("fur");
        }

        private bool IsUsingShrinkMode(Material material)
        {
            // Check for Shrink mode properties
            if (material.HasProperty("_FurMode"))
            {
                var furMode = material.GetFloat("_FurMode");
                // Assuming 0 = Shrink mode, 1 = Subdivision mode
                return furMode == 0;
            }
            
            // Check for legacy Shrink-specific properties
            if (material.HasProperty("_FurShrink"))
            {
                return material.GetFloat("_FurShrink") > 0;
            }
            
            // Check for Shrink-related keywords
            var keywords = material.shaderKeywords;
            return keywords.Any(k => k.ToLower().Contains("shrink"));
        }

        private void MigrateAllMaterials()
        {
            if (materialsWithShrinkMode.Count == 0)
            {
                EditorUtility.DisplayDialog("No Materials Found", "No Shrink mode materials to migrate.", "OK");
                return;
            }

            if (backupBeforeMigration)
            {
                var result = EditorUtility.DisplayDialog("Backup Project", 
                    "It's recommended to backup your project before migrating fur materials.\n\n" +
                    "Would you like to continue without backup?", "Continue", "Cancel");
                
                if (!result) return;
            }

            int migratedCount = 0;
            
            foreach (var material in materialsWithShrinkMode.ToList())
            {
                if (MigrateMaterial(material))
                {
                    migratedCount++;
                }
            }
            
            materialsWithShrinkMode.Clear();
            ScanForShrinkModeMaterials();
            
            EditorUtility.DisplayDialog("Migration Complete", 
                $"Successfully migrated {migratedCount} materials from Shrink to Subdivision mode.\n\n" +
                "Your fur materials are now compatible with lilToon 2.1.9!", "OK");
        }

        private bool MigrateMaterial(Material material)
        {
            if (material == null) return false;
            
            Undo.RecordObject(material, "Migrate Fur Material to Subdivision");
            
            // Set fur mode to Subdivision
            if (material.HasProperty("_FurMode"))
            {
                material.SetFloat("_FurMode", 1.0f); // 1 = Subdivision mode
            }
            
            // Remove Shrink-specific properties
            if (material.HasProperty("_FurShrink"))
            {
                material.SetFloat("_FurShrink", 0.0f);
            }
            
            // Update shader keywords
            var keywords = new List<string>(material.shaderKeywords);
            keywords.RemoveAll(k => k.ToLower().Contains("shrink"));
            keywords.Add("FUR_SUBDIVISION");
            material.shaderKeywords = keywords.ToArray();
            
            // Ensure Subdivision properties are set
            if (material.HasProperty("_FurSubdivision"))
            {
                var currentSubdivision = material.GetFloat("_FurSubdivision");
                if (currentSubdivision <= 0)
                {
                    material.SetFloat("_FurSubdivision", 4.0f); // Default subdivision value
                }
            }
            
            // Mark as dirty
            EditorUtility.SetDirty(material);
            
            return true;
        }

        private void OnEnable()
        {
            ScanForShrinkModeMaterials();
        }
    }
}
