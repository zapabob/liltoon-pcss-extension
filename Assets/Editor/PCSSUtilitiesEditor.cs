#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Presets;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    public static class PCSSUtilitiesEditor
    {
        // ===== Utilities =====
        [MenuItem(PCSSConstants.MenuBase + "Utilities/Repair lilToon Settings JSON", priority = 49)]
        private static void RepairLilToonSettingsJson()
        {
            var targets = new[]
            {
                "Packages/jp.lilxyzw.liltoon",
                "Packages/com.lilxyzw.liltoon",
                "ProjectSettings",
                "Assets"
            };
            int repaired = 0;
            foreach (var root in targets)
            {
                if (!Directory.Exists(root)) continue;
                var files = Directory.GetFiles(root, "*.json", SearchOption.AllDirectories);
                foreach (var f in files)
                {
                    try
                    {
                        var name = Path.GetFileName(f).ToLowerInvariant();
                        if (!(name.Contains("lil") || name.Contains("toon") || name.Contains("setting") || name.Contains("config"))) continue;
                        string content = File.ReadAllText(f);
                        if (string.IsNullOrWhiteSpace(content) || content.Trim() == "[]")
                        {
                            File.WriteAllText(f, "{}");
                            repaired++;
                        }
                    }
                    catch { /* ignore per file */ }
                }
            }
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("PCSS Utilities", repaired > 0 ? $"Repaired {repaired} lilToon/setting JSON files." : "No repair needed.", "OK");
        }

        [MenuItem(PCSSConstants.MenuBase + "Utilities/Remove Missing Scripts", priority = 50)]
        private static void RemoveMissingScripts()
        {
            var selected = Selection.gameObjects;
            if (selected == null || selected.Length == 0)
            {
                EditorUtility.DisplayDialog("PCSS Utilities", "HierarchyでGameObjectを選択してください。", "OK");
                return;
            }
            var all = GetAllChildren(selected);
            int count = RemoveMissingScriptsFrom(all);
            EditorUtility.DisplayDialog("Remove Missing Scripts", $"Removed {count} missing scripts.", "OK");
        }

        private static int RemoveMissingScriptsFrom(GameObject[] objects)
        {
            int removed = 0;
            var toSave = new HashSet<GameObject>();
            foreach (var go in objects)
            {
                if (go == null) continue;
                int missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                if (missing == 0) continue;
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                EditorUtility.SetDirty(go);
                if (EditorUtility.IsPersistent(go) && PrefabUtility.IsAnyPrefabInstanceRoot(go))
                    toSave.Add(go);
                removed += missing;
            }
            foreach (var root in toSave) PrefabUtility.SavePrefabAsset(root);
            return removed;
        }

        private static GameObject[] GetAllChildren(GameObject[] roots)
        {
            var list = new List<GameObject>();
            foreach (var r in roots)
            {
                if (r == null) continue;
                list.AddRange(r.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject));
            }
            return list.Distinct().ToArray();
        }

        [MenuItem(PCSSConstants.MenuBase + "Utilities/Rename To Unique Names", priority = 51)]
        private static void RenameToUnique()
        {
            foreach (var go in Selection.gameObjects)
            {
                go.name = $"{go.name}_{go.GetInstanceID()}";
                EditorUtility.SetDirty(go);
            }
        }

        [MenuItem(PCSSConstants.MenuBase + "Utilities/Apply MeshRenderer Preset To Selection", priority = 52)]
        private static void ApplyMeshRendererPresetToSelection()
        {
            var preset = AssetDatabase.LoadAssetAtPath<Preset>(PCSSConstants.MeshRendererPresetPath);
            if (preset == null)
            {
                EditorUtility.DisplayDialog("PCSS Utilities", "MeshRenderer Preset が見つかりません。先に『プリセット導入』を実行してください。", "OK");
                return;
            }
            int count = 0;
            foreach (var go in Selection.gameObjects)
            {
                foreach (var mr in go.GetComponentsInChildren<MeshRenderer>(true))
                {
                    preset.ApplyTo(mr);
                    EditorUtility.SetDirty(mr);
                    count++;
                }
            }
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("PCSS Utilities", $"MeshRenderer Preset を {count} 個に適用しました。", "OK");
        }

        [MenuItem(PCSSConstants.MenuBase + "Utilities/Remove Missing Scripts (All Open Scenes)", priority = 53)]
        private static void RemoveMissingScriptsAllOpenScenes()
        {
            int totalRemoved = 0;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                var roots = scene.GetRootGameObjects();
                var all = GetAllChildren(roots);
                totalRemoved += RemoveMissingScriptsFrom(all);
                if (totalRemoved > 0) EditorSceneManager.MarkSceneDirty(scene);
            }
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Remove Missing Scripts", $"All Open Scenes: Removed {totalRemoved} missing scripts.", "OK");
        }

        [MenuItem(PCSSConstants.MenuBase + "Utilities/Sanitize 'PCSS_Controller' In Open Scenes", priority = 54)]
        private static void SanitizePCSSControllerInOpenScenes()
        {
            int processed = 0;
            int removedMissing = 0;
            int deleted = 0;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                foreach (var root in scene.GetRootGameObjects())
                {
                    foreach (var t in root.GetComponentsInChildren<Transform>(true))
                    {
                        var go = t.gameObject;
                        if (go == null) continue;
                        if (!go.name.Contains("PCSS_Controller")) continue;
                        processed++;
                        int before = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                        if (before > 0)
                        {
                            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                            removedMissing += before;
                            EditorUtility.SetDirty(go);
                        }
                        // もし有効なコンポーネントが Transform しかないなら削除
                        var comps = go.GetComponents<Component>();
                        bool hasValidNonTransform = comps.Any(c => c != null && !(c is Transform));
                        if (!hasValidNonTransform)
                        {
                            Object.DestroyImmediate(go);
                            deleted++;
                        }
                    }
                }
                if (removedMissing > 0 || deleted > 0) EditorSceneManager.MarkSceneDirty(scene);
            }
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("PCSS Utilities", $"PCSS_Controller Sanitize: processed={processed}, removedMissing={removedMissing}, deleted={deleted}", "OK");
        }

        // ===== FX Controller Builder (minimal safe skeleton) =====
        [MenuItem(PCSSConstants.MenuBase + "Build FX Controller (Selected Avatar)", priority = 20)]
        public static void BuildFxControllerForSelected()
        {
            if (Selection.activeGameObject == null)
            {
                EditorUtility.DisplayDialog("PCSS", "アバターを選択してください。", "OK");
                return;
            }
            var avatar = Selection.activeGameObject;

            var animator = avatar.GetComponent<Animator>();
            if (animator == null) animator = avatar.AddComponent<Animator>();

            string dir = "Assets/PCSS/Controllers";
            if (!AssetDatabase.IsValidFolder("Assets/PCSS")) AssetDatabase.CreateFolder("Assets", "PCSS");
            if (!AssetDatabase.IsValidFolder(dir)) AssetDatabase.CreateFolder("Assets/PCSS", "Controllers");

            string path = Path.Combine(dir, "PCSS_FX.controller");
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            }

            EnsureParameter(controller, PCSSConstants.ParamLightOn, AnimatorControllerParameterType.Bool);
            EnsureParameter(controller, PCSSConstants.ParamLightIntensity, AnimatorControllerParameterType.Float);

            // Ensure layer and basic state machine exist
            var layer = controller.layers.FirstOrDefault(l => l.name == "PCSS_Control");
            if (layer.name != "PCSS_Control")
            {
                var sm = new AnimatorStateMachine { name = "PCSS_Control_SM" };
                sm.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(sm, path);
                layer = new AnimatorControllerLayer
                {
                    name = "PCSS_Control",
                    defaultWeight = 1f,
                    stateMachine = sm
                };
                controller.AddLayer(layer);
            }

            animator.runtimeAnimatorController = controller;
            EditorUtility.SetDirty(controller);
            EditorUtility.SetDirty(animator);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("PCSS", "FXコントローラーを作成/更新し、アバターに割り当てました。", "OK");
        }

        private static void EnsureParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
        {
            if (controller.parameters.Any(p => p.name == name)) return;
            controller.AddParameter(name, type);
        }
    }
}
#endif


