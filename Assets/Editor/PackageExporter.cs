using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Linq;

public class PackageExporter
{
    private const string ExportDir = "ExportedPackages";
    private const string Version = "1.5.6"; // 忁E��に応じて自動取得可
    private static readonly string[] ExportPaths = {
        "Assets/Editor/MaterialSessionRestorer.cs",
        // 他に含めたぁE��ァイルめE��ォルダがあれ�Eここに追加
    };

    [MenuItem("Tools/PCSS/パッケージ自動エクスポ�Eト！E��リースノ�Eト生戁E)]
    public static void ExportAndGenerateReleaseNotes()
    {
        if (!Directory.Exists(ExportDir)) Directory.CreateDirectory(ExportDir);
        string unitypackage = $"com.liltoon.pcss-extension-{Version}.unitypackage";
        string exportPath = Path.Combine(ExportDir, unitypackage);

        // パッケージエクスポ�EチE
        AssetDatabase.ExportPackage(ExportPaths, exportPath, ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);
        Debug.Log($"[PCSS] パッケージエクスポ�Eト完亁E {exportPath}");

        // リリースノ�Eト生戁E
        string changelogPath = "CHANGELOG.md";
        string releaseNotePath = Path.Combine(ExportDir, $"release_notes_{Version}.txt");
        string notes = "";
        if (File.Exists(changelogPath))
        {
            // CHANGELOG.mdから該当バージョンのリリースノ�Eトを抽出
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
            // git logから直迁E0件を取得（要gitコマンド！E
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
        Debug.Log($"[PCSS] リリースノ�Eト生成完亁E {releaseNotePath}");
        EditorUtility.RevealInFinder(exportPath);
    }
} 
