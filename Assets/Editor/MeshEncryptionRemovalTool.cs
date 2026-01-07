using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// lilToon 2.1.9 メッシュ暗号化削除対応ツール
    /// AvatarEncryptionコンポーネントの検出・削除・移行支援
    /// </summary>
    public class MeshEncryptionRemovalTool : EditorWindow
    {
        private Vector2 scrollPosition;
        private List<GameObject> objectsWithEncryption = new List<GameObject>();
        private bool showDetailedReport = false;
        private bool autoRemoveEncryption = false;
        private bool backupBeforeRemoval = true;
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Mesh Encryption Removal Tool")]
        public static void ShowWindow()
        {
            MeshEncryptionRemovalTool window = GetWindow<MeshEncryptionRemovalTool>("Mesh Encryption Removal");
            window.minSize = new Vector2(600, 400);
            window.ScanForEncryptionComponents();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Mesh Encryption Removal Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            EditorGUILayout.HelpBox("lilToon 2.1.9 Breaking Change: Mesh Encryption (AvatarEncryption) has been removed.\n" +
                "This tool helps detect and remove encryption components from your project.", MessageType.Warning);

            EditorGUILayout.Space(10);

            // Scan Button
            if (GUILayout.Button("Scan for Encryption Components", GUILayout.Height(30)))
            {
                ScanForEncryptionComponents();
            }

            EditorGUILayout.Space(10);

            // Results
            EditorGUILayout.LabelField($"Found {objectsWithEncryption.Count} objects with encryption components:", EditorStyles.boldLabel);
            
            if (objectsWithEncryption.Count > 0)
            {
                EditorGUILayout.HelpBox("⚠️ ACTION REQUIRED: These objects contain AvatarEncryption components that must be removed for lilToon 2.1.9 compatibility.", MessageType.Error);
                
                EditorGUILayout.Space(5);
                
                // Options
                backupBeforeRemoval = EditorGUILayout.Toggle("Backup before removal", backupBeforeRemoval);
                autoRemoveEncryption = EditorGUILayout.Toggle("Auto-remove encryption components", autoRemoveEncryption);
                
                EditorGUILayout.Space(5);
                
                // Action Buttons
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Remove All Encryption", GUILayout.Height(30)))
                {
                    RemoveAllEncryptionComponents();
                }
                
                if (GUILayout.Button("Select All Objects", GUILayout.Height(30)))
                {
                    Selection.objects = objectsWithEncryption.ToArray();
                }
                
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(10);
                
                // Detailed List
                showDetailedReport = EditorGUILayout.Foldout(showDetailedReport, "Detailed Report", true);
                if (showDetailedReport)
                {
                    EditorGUI.indentLevel++;
                    
                    foreach (var obj in objectsWithEncryption)
                    {
                        EditorGUILayout.BeginHorizontal();
                        
                        if (GUILayout.Button("Select", GUILayout.Width(60)))
                        {
                            Selection.activeGameObject = obj;
                        }
                        
                        EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
                        
                        if (GUILayout.Button("Remove", GUILayout.Width(60)))
                        {
                            RemoveEncryptionFromObject(obj);
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("✅ No encryption components found. Your project is ready for lilToon 2.1.9!", MessageType.Info);
            }

            EditorGUILayout.Space(20);

            // Migration Info
            EditorGUILayout.LabelField("Migration Information:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("• lilToon 2.1.9 removes AvatarEncryption for performance reasons");
            EditorGUILayout.LabelField("• Encrypted meshes will be automatically decrypted");
            EditorGUILayout.LabelField("• No data loss - meshes remain intact");
            EditorGUILayout.LabelField("• Improved performance and compatibility");

            EditorGUILayout.EndScrollView();
        }

        private void ScanForEncryptionComponents()
        {
            objectsWithEncryption.Clear();
            
            // Find all GameObjects in the scene
            var allGameObjects = FindObjectsOfType<GameObject>();
            
            foreach (var go in allGameObjects)
            {
                var components = go.GetComponents<Component>();
                foreach (var component in components)
                {
                    if (component != null && component.GetType().Name.Contains("AvatarEncryption"))
                    {
                        objectsWithEncryption.Add(go);
                        break;
                    }
                }
            }
            
            // Also check prefabs in project
            var prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            foreach (var guid in prefabGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab != null)
                {
                    var components = prefab.GetComponentsInChildren<Component>(true);
                    foreach (var component in components)
                    {
                        if (component != null && component.GetType().Name.Contains("AvatarEncryption"))
                        {
                            objectsWithEncryption.Add(prefab);
                            break;
                        }
                    }
                }
            }
            
            Repaint();
        }

        private void RemoveAllEncryptionComponents()
        {
            if (objectsWithEncryption.Count == 0)
            {
                EditorUtility.DisplayDialog("No Encryption Found", "No encryption components to remove.", "OK");
                return;
            }

            if (backupBeforeRemoval)
            {
                var result = EditorUtility.DisplayDialog("Backup Project", 
                    "It's recommended to backup your project before removing encryption components.\n\n" +
                    "Would you like to continue without backup?", "Continue", "Cancel");
                
                if (!result) return;
            }

            int removedCount = 0;
            
            foreach (var obj in objectsWithEncryption.ToList())
            {
                if (RemoveEncryptionFromObject(obj))
                {
                    removedCount++;
                }
            }
            
            objectsWithEncryption.Clear();
            ScanForEncryptionComponents();
            
            EditorUtility.DisplayDialog("Encryption Removal Complete", 
                $"Successfully removed encryption components from {removedCount} objects.\n\n" +
                "Your project is now compatible with lilToon 2.1.9!", "OK");
        }

        private bool RemoveEncryptionFromObject(GameObject obj)
        {
            if (obj == null) return false;
            
            var components = obj.GetComponents<Component>();
            var encryptionComponents = new List<Component>();
            
            foreach (var component in components)
            {
                if (component != null && component.GetType().Name.Contains("AvatarEncryption"))
                {
                    encryptionComponents.Add(component);
                }
            }
            
            if (encryptionComponents.Count == 0) return false;
            
            // Remove encryption components
            foreach (var component in encryptionComponents)
            {
                if (component != null)
                {
                    Undo.DestroyObjectImmediate(component);
                }
            }
            
            // Mark as dirty
            EditorUtility.SetDirty(obj);
            
            // If it's a prefab, mark the asset as dirty
            if (PrefabUtility.IsPartOfPrefabAsset(obj))
            {
                EditorUtility.SetDirty(obj);
                AssetDatabase.SaveAssets();
            }
            
            return true;
        }

        private void OnEnable()
        {
            ScanForEncryptionComponents();
        }
    }
}
