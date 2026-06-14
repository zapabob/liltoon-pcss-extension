using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// Runtime と Shaders/Runtime に重複配置された C# を検出・整理するユーティリティ
    /// </summary>
    public static class DuplicateRuntimeCleanup
    {
        private static readonly string PrimaryRuntime = "Assets/New Folder/Runtime";
        private static readonly string ShadowRuntime = "Assets/New Folder/Shaders/Runtime";

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/ビルド/重複Runtimeスクリプトのクリーンアップ", false, 9100)]
        public static void CleanupDuplicateRuntime()
        {
            if (!Directory.Exists(PrimaryRuntime) || !Directory.Exists(ShadowRuntime))
            {
                EditorUtility.DisplayDialog("Cleanup", "必要なフォルダが見つかりませんでした。", "OK");
                return;
            }

            var primaryFiles = Directory.GetFiles(PrimaryRuntime, "*.cs", SearchOption.TopDirectoryOnly)
                                        .Select(Path.GetFileName)
                                        .ToHashSet();
            var shadowFiles = Directory.GetFiles(ShadowRuntime, "*.cs", SearchOption.TopDirectoryOnly);

            int removed = 0;
            foreach (var shadowPath in shadowFiles)
            {
                var name = Path.GetFileName(shadowPath);
                if (primaryFiles.Contains(name))
                {
                    // 同名の影ファイルは削除（既に Primary に実体がある）
                    AssetDatabase.DeleteAsset(ToAssetPath(shadowPath));
                    removed++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Cleanup", $"重複Runtimeスクリプトを {removed} 件削除しました。", "OK");
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/ビルド/重複Runtimeスクリプトのクリーンアップ", true)]
        private static bool ValidateCleanupDuplicateRuntime()
        {
            return Directory.Exists(PrimaryRuntime) && Directory.Exists(ShadowRuntime);
        }

        private static string ToAssetPath(string absolutePath)
        {
            var projectPath = Application.dataPath.Replace("/Assets", "");
            return absolutePath.Replace(projectPath + Path.DirectorySeparatorChar, string.Empty).Replace('\\', '/');
        }
    }
}


