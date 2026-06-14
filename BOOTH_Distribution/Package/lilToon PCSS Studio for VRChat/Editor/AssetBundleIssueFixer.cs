using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    public static class AssetBundleIssueFixer
    {
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/ビルド/重複アセット名スキャン", false, 300)]
        public static void ScanDuplicateAssetNames()
        {
            var guids = AssetDatabase.FindAssets("");
            var nameToPaths = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path) || Directory.Exists(path)) continue;
                var name = Path.GetFileName(path);
                if (!nameToPaths.TryGetValue(name, out var list))
                {
                    list = new List<string>();
                    nameToPaths[name] = list;
                }
                list.Add(path);
            }

            int dupCount = 0;
            foreach (var kv in nameToPaths.Where(kv => kv.Value.Count > 1))
            {
                dupCount++;
                Debug.Log($"[DuplicateName] {kv.Key}\n  - " + string.Join("\n  - ", kv.Value));
            }

            EditorUtility.DisplayDialog("PCSS", dupCount > 0
                ? $"重複ファイル名が {dupCount} 件見つかりました。Console を確認してください。\n（同名アセットはAssetBundleビルド失敗の原因になります）"
                : "重複ファイル名は見つかりませんでした。", "OK");
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/ビルド/重複HLSL(Shaders/Shaders/Includes)安全削除", false, 301)]
        public static void RemoveDuplicateHlslFolderIfIdentical()
        {
            // 候補フォルダを検索
            var includesA = FindFolderAssetPath("Shaders/Includes");
            var includesB = FindFolderAssetPath("Shaders/Shaders/Includes");

            if (string.IsNullOrEmpty(includesA) || string.IsNullOrEmpty(includesB))
            {
                EditorUtility.DisplayDialog("PCSS", "対象フォルダが見つかりませんでした。", "OK");
                return;
            }

            // 比較対象ファイル
            var files = new[] { "lil_pcss_common.hlsl", "lil_pcss_shadows.hlsl", "lil_pcss_shadows_optimized.hlsl" };
            foreach (var file in files)
            {
                var pathA = Path.Combine(includesA, file).Replace('\\', '/');
                var pathB = Path.Combine(includesB, file).Replace('\\', '/');
                if (!FileEquals(pathA, pathB))
                {
                    EditorUtility.DisplayDialog("PCSS", $"内容が一致しないファイルがあります:\n{file}\n安全削除を中止します。", "OK");
                    return;
                }
            }

            if (!EditorUtility.DisplayDialog("PCSS 確認", "重複フォルダ 'Shaders/Shaders/Includes' は 'Shaders/Includes' と同一内容です。\n安全に削除しますか？", "削除する", "やめる"))
            {
                return;
            }

            // includesB を削除
            if (AssetDatabase.DeleteAsset(includesB))
            {
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("PCSS", "重複HLSLフォルダを削除しました。再インポート後にビルドをお試しください。", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("PCSS", "削除に失敗しました。手動でご確認ください。", "OK");
            }
        }

        private static string FindFolderAssetPath(string endsWith)
        {
            // すべてのフォルダから末尾一致で検索
            var folders = AssetDatabase.GetSubFolders("Assets");
            var queue = new Queue<string>(folders);
            while (queue.Count > 0)
            {
                var f = queue.Dequeue();
                if (f.EndsWith(endsWith, StringComparison.OrdinalIgnoreCase)) return f;
                foreach (var sub in AssetDatabase.GetSubFolders(f)) queue.Enqueue(sub);
            }
            return null;
        }

        private static bool FileEquals(string assetPathA, string assetPathB)
        {
            var sysA = AssetPathToSystemPath(assetPathA);
            var sysB = AssetPathToSystemPath(assetPathB);
            if (!File.Exists(sysA) || !File.Exists(sysB)) return false;
            var a = File.ReadAllBytes(sysA);
            var b = File.ReadAllBytes(sysB);
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;
            return true;
        }

        private static string AssetPathToSystemPath(string assetPath)
        {
            if (assetPath.StartsWith("Assets"))
            {
                var root = Application.dataPath; // .../ProjectName/Assets
                var rel = assetPath.Substring("Assets".Length).Replace('/', Path.DirectorySeparatorChar);
                return Path.Combine(Path.GetDirectoryName(root) ?? string.Empty, "Assets" + rel);
            }
            return assetPath;
        }
    }
}


