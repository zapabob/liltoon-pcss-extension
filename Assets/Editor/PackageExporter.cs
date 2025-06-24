using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Linq;

public class PackageExporter
{
    private const string ExportDir = "ExportedPackages";
    private const string Version = "1.5.6"; // 必要に応じて自動取得可
    private static readonly string[] ExportPaths = {
        "Assets/Editor/MaterialSessionRestorer.cs",
        // 他に含めたいファイルやフォルダがあればここに追加
    };

    [MenuItem("Tools/PCSS/パッケージ自動エクスポート＆リリースノート生成")]
    public static void ExportAndGenerateReleaseNotes()
    {
        if (!Directory.Exists(ExportDir)) Directory.CreateDirectory(ExportDir);
        string unitypackage = $"com.liltoon.pcss-extension-{Version}.unitypackage";
        string exportPath = Path.Combine(ExportDir, unitypackage);

        // パッケージエクスポート
        AssetDatabase.ExportPackage(ExportPaths, exportPath, ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);
        Debug.Log($"[PCSS] パッケージエクスポート完了: {exportPath}");

        // リリースノート生成
        string changelogPath = "CHANGELOG.md";
        string releaseNotePath = Path.Combine(ExportDir, $"release_notes_{Version}.txt");
        string notes = "";
        if (File.Exists(changelogPath))
        {
            // CHANGELOG.mdから該当バージョンのリリースノートを抽出
            var lines = File.ReadAllLines(changelogPath);
            bool inSection = false;
            foreach (var line in lines)
            {
                if (line.Contains(Version)) inSection = true;
                else if (inSection && line.StartsWith("#")) break;
                if (inSection) notes += line + "\n";
            }
        }
        else
        {
            // git logから直近10件を取得（要gitコマンド）
            try
            {
                var psi = new ProcessStartInfo("git", "log -10 --oneline")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                var proc = Process.Start(psi);
                notes = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
            }
            catch { notes = "No changelog or git log found."; }
        }
        File.WriteAllText(releaseNotePath, notes);
        Debug.Log($"[PCSS] リリースノート生成完了: {releaseNotePath}");
        EditorUtility.RevealInFinder(exportPath);
    }
} 