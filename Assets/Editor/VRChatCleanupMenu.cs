using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor.Presets;
using UnityEditor.Experimental.SceneManagement;
using UnityEditorInternal;
using UnityEditor;
using System.Collections.Generic;
using System.Linq; // Added for .Where()

namespace lilToon.PCSS.Editor
{
    public static class VRChatCleanupMenu
    {
        [MenuItem("Tools/lilToon-PCSS-Extension/VRChat向けクリーンアップ/不許可コンポーネント削除", false, 200)]
        public static void CleanupDisallowedComponents()
        {
            var root = Selection.activeGameObject;
            if (root == null)
            {
                EditorUtility.DisplayDialog("PCSS", "クリーンアップ対象のアバター(ルート)を選択してください。", "OK");
                return;
            }

            int removed = 0;
            foreach (var comp in root.GetComponentsInChildren<Component>(true))
            {
                if (comp == null) continue;
                var t = comp.GetType();
                string full = t.FullName;
                if (full == null) continue;

                // Editor/開発用 or アップロードに不向きと想定されるものを削除
                if (full == "lilToon.PCSS.PhysBoneLightController" ||
                    full == "PhysBoneLightController" ||
                    full == "lilToon.PCSS.Runtime.ModularAvatarPCSSController" ||
                    full == "lilToon.PCSS.Editor.ModularAvatarPCSSController" ||
                    full == "lilToon.PCSS.VRCLightVolumesIntegration")
                {
                    Undo.DestroyObjectImmediate(comp);
                    removed++;
                }
            }

            // 自動配置ライトの塊を削除（旧/新）
            var groups = new List<Transform>();
            foreach (var tr in root.GetComponentsInChildren<Transform>(true))
            {
                if (tr.name == "PCSS External Lights (Auto)" || tr.name == "PCSS Hip Lights (MA)")
                {
                    groups.Add(tr);
                }
            }
            foreach (var g in groups)
            {
                Undo.DestroyObjectImmediate(g.gameObject);
                removed++;
            }

            EditorUtility.DisplayDialog("PCSS", $"クリーンアップ完了: {removed} 件を削除しました。", "OK");
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/VRChat向けクリーンアップ/VRC Light Volumes 無効化・除去", false, 201)]
        public static void RemoveVRCLightVolumesOnly()
        {
            var root = Selection.activeGameObject;
            if (root == null)
            {
                EditorUtility.DisplayDialog("PCSS", "対象のアバター(ルート)を選択してください。", "OK");
                return;
            }

            int removed = 0;
            foreach (var comp in root.GetComponentsInChildren<Component>(true))
            {
                if (comp == null) continue;
                var t = comp.GetType();
                string full = t.FullName;
                if (full == "lilToon.PCSS.VRCLightVolumesIntegration")
                {
                    Undo.DestroyObjectImmediate(comp);
                    removed++;
                }
            }

            // 併せてマテリアルのグローバルキーワードを無効化
            var renderers = root.GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers)
            {
                foreach (var m in r.sharedMaterials)
                {
                    if (m == null) continue;
                    m.DisableKeyword("VRC_LIGHT_VOLUMES_ENABLED");
                    m.DisableKeyword("VRC_LIGHT_VOLUMES_MOBILE");
                }
            }

            EditorUtility.DisplayDialog("PCSS", $"VRC Light Volumes コンポーネントを {removed} 件除去し、関連キーワードを無効化しました。", "OK");
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/VRChat向けクリーンアップ/PhysBoneをVRCPhysBoneに統一", false, 202)]
        public static void EnforceVRCPhysBoneOnly()
        {
            var root = Selection.activeGameObject;
            if (root == null)
            {
                EditorUtility.DisplayDialog("PCSS", "対象のアバター(ルート)を選択してください。", "OK");
                return;
            }

            int removed = 0;
            foreach (var comp in root.GetComponentsInChildren<Component>(true))
            {
                if (comp == null) continue;
                var t = comp.GetType();
                string full = t.FullName;
                if (string.IsNullOrEmpty(full)) continue;

                // VRC公式以外のPhysBone系を排除
                if (full.Contains("PhysBone") &&
                    full != "VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone" &&
                    full != "VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider")
                {
                    Undo.DestroyObjectImmediate(comp);
                    removed++;
                }
            }

            EditorUtility.DisplayDialog("PCSS", $"VRCPhysBone以外のPhysBone系コンポーネントを {removed} 件削除しました。", "OK");
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/VRChat向けクリーンアップ/シーン全体: 不許可コンポーネント削除", false, 203)]
        public static void CleanupDisallowedInActiveScene()
        {
            int removed = 0;
            foreach (var comp in Object.FindObjectsByType<Component>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID))
            {
                if (comp == null) continue;
                if (ShouldRemove(comp))
                {
                    Undo.DestroyObjectImmediate(comp);
                    removed++;
                }
            }
            EditorUtility.DisplayDialog("PCSS", $"シーン全体の不許可コンポーネントを {removed} 件削除しました。", "OK");
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/VRChat向けクリーンアップ/プロジェクト全Prefab: 不許可コンポーネント削除", false, 204)]
        public static void CleanupDisallowedInAllPrefabs()
        {
            var guids = AssetDatabase.FindAssets("t:Prefab");
            int removed = 0;
            int touched = 0;
            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var root = PrefabUtility.LoadPrefabContents(path);
                if (root == null) continue;
                try
                {
                    int before = removed;
                    foreach (var comp in root.GetComponentsInChildren<Component>(true))
                    {
                        if (comp == null) continue;
                        if (ShouldRemove(comp))
                        {
                            Object.DestroyImmediate(comp, true);
                            removed++;
                        }
                    }
                    if (removed > before)
                    {
                        PrefabUtility.SaveAsPrefabAsset(root, path);
                        touched++;
                    }
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }
            EditorUtility.DisplayDialog("PCSS", $"Prefab内の不許可コンポーネントを {removed} 件削除（{touched} Prefab更新）しました。", "OK");
        }

        private static bool ShouldRemove(Component comp)
        {
            var t = comp.GetType();
            string full = t.FullName;
            if (string.IsNullOrEmpty(full)) return false;

            // 直接名/名前空間含む両対応
            if (full == "lilToon.PCSS.PhysBoneLightController" || full == "PhysBoneLightController") return true;
            if (full.EndsWith("CompetitorSetupWizard+PhysBoneLightController")) return true;
            if (full.EndsWith(".PhysBoneLightController")) return true;
            if (full == "lilToon.PCSS.VRCLightVolumesIntegration" || full.EndsWith("VRCLightVolumesIntegration")) return true;

            // 非公式PhysBone系（公式2種以外は削除）
            if (full.Contains("PhysBone") &&
                full != "VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone" &&
                full != "VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider")
            {
                return true;
            }
            return false;
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/VRChat向けクリーンアップ/Missing Script削除(選択アバター)", false, 205)]
        public static void RemoveMissingScriptsOnSelection()
        {
            var root = Selection.activeGameObject;
            if (root == null)
            {
                EditorUtility.DisplayDialog("PCSS", "対象のアバター(ルート)を選択してください。", "OK");
                return;
            }
            int removed = RemoveMissingScriptsRecursive(root);
            EditorUtility.DisplayDialog("PCSS", $"Missing Script を {removed} 件削除しました。", "OK");
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/VRChat向けクリーンアップ/Missing Script削除(シーン全体)", false, 206)]
        public static void RemoveMissingScriptsInScene()
        {
            int removed = 0;
            foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID))
            {
                removed += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            }
            EditorUtility.DisplayDialog("PCSS", $"シーン内の Missing Script を {removed} 件削除しました。", "OK");
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/VRChat向けクリーンアップ/Missing Script削除(全Prefab)", false, 207)]
        public static void RemoveMissingScriptsInAllPrefabs()
        {
            var guids = AssetDatabase.FindAssets("t:Prefab");
            int removed = 0;
            int touched = 0;
            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var root = PrefabUtility.LoadPrefabContents(path);
                if (root == null) continue;
                try
                {
                    int before = removed;
                    removed += RemoveMissingScriptsRecursive(root);
                    if (removed > before)
                    {
                        PrefabUtility.SaveAsPrefabAsset(root, path);
                        touched++;
                    }
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }
            EditorUtility.DisplayDialog("PCSS", $"Prefab内の Missing Script を {removed} 件削除（{touched} Prefab更新）しました。", "OK");
        }

        private static int RemoveMissingScriptsRecursive(GameObject root)
        {
            int removed = 0;
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                removed += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
            }
            return removed;
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/VRChat向けクリーンアップ/ModularAvatarPCSSController削除", false, 302)]
        public static void RemoveModularAvatarPCSSController()
        {
            var go = Selection.activeGameObject;
            if (go == null)
            {
                EditorUtility.DisplayDialog("PCSS", "アバターのルートを選択してください。", "OK");
                return;
            }

            int removedCount = 0;

            // ModularAvatarPCSSControllerを検索して削除
            var allComponents = go.GetComponentsInChildren<Component>(true);
            foreach (var component in allComponents)
            {
                if (component != null && component.GetType().Name.Contains("ModularAvatarPCSSController"))
                {
                    Undo.DestroyObjectImmediate(component);
                    removedCount++;
                }
            }

            // ネストした型も検索
            var allGameObjects = go.GetComponentsInChildren<Transform>(true);
            foreach (var transform in allGameObjects)
            {
                var components = transform.GetComponents<Component>();
                foreach (var component in components)
                {
                    if (component != null && 
                        (component.GetType().Name.Contains("ModularAvatarPCSSController") ||
                         component.GetType().Name.Contains("PCSSController") ||
                         component.GetType().Name.Contains("ModularAvatar") && component.GetType().Name.Contains("PCSS")))
                    {
                        Undo.DestroyObjectImmediate(component);
                        removedCount++;
                    }
                }
            }

            if (removedCount > 0)
            {
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("PCSS", $"ModularAvatarPCSSControllerを{removedCount}個削除しました。\n\n" +
                    "✅ VRChat AutoFix Safe\n" +
                    "✅ カスタムランタイムコンポーネント削除完了", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("PCSS", "ModularAvatarPCSSControllerは見つかりませんでした。", "OK");
            }
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/VRChat向けクリーンアップ/全カスタムPCSSコンポーネント削除", false, 303)]
        public static void RemoveAllCustomPCSSComponents()
        {
            var go = Selection.activeGameObject;
            if (go == null)
            {
                EditorUtility.DisplayDialog("PCSS", "アバターのルートを選択してください。", "OK");
                return;
            }

            int removedCount = 0;

            // 全カスタムPCSSコンポーネントを検索して削除
            var allGameObjects = go.GetComponentsInChildren<Transform>(true);
            foreach (var transform in allGameObjects)
            {
                var components = transform.GetComponents<Component>();
                foreach (var component in components)
                {
                    if (component != null && 
                        (component.GetType().Name.Contains("PCSSController") ||
                         component.GetType().Name.Contains("ModularAvatarPCSS") ||
                         component.GetType().Name.Contains("PCSSLightController") ||
                         component.GetType().Name.Contains("PhysBoneLightController") ||
                         component.GetType().Name.Contains("VRCLightVolumesIntegration")))
                    {
                        Undo.DestroyObjectImmediate(component);
                        removedCount++;
                    }
                }
            }

            // Missing Scriptsも削除
            var missingScripts = go.GetComponentsInChildren<Component>(true)
                .Where(c => c == null)
                .ToArray();

            foreach (var missingScript in missingScripts)
            {
                if (missingScript != null)
                {
                    Undo.DestroyObjectImmediate(missingScript);
                    removedCount++;
                }
            }

            if (removedCount > 0)
            {
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("PCSS", $"カスタムPCSSコンポーネントを{removedCount}個削除しました。\n\n" +
                    "✅ VRChat AutoFix Safe\n" +
                    "✅ 全カスタムランタイムコンポーネント削除完了\n" +
                    "✅ Missing Scripts削除完了", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("PCSS", "カスタムPCSSコンポーネントは見つかりませんでした。", "OK");
            }
        }
    }
}


