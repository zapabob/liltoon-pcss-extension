using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Linq;

public class PackageExporter
{
    private const string ExportDir = "ExportedPackages";
    private const string Version = "1.5.6"; // 蠢・ｦ√↓蠢懊§縺ｦ閾ｪ蜍募叙蠕怜庄
    private static readonly string[] ExportPaths = {
        "Assets/Editor/MaterialSessionRestorer.cs",
        // 莉悶↓蜷ｫ繧√◆縺・ヵ繧｡繧､繝ｫ繧・ヵ繧ｩ繝ｫ繝縺後≠繧後・縺薙％縺ｫ霑ｽ蜉
    };

    [MenuItem("Tools/PCSS/繝代ャ繧ｱ繝ｼ繧ｸ閾ｪ蜍輔お繧ｯ繧ｹ繝昴・繝茨ｼ・Μ繝ｪ繝ｼ繧ｹ繝弱・繝育函謌・)]
    public static void ExportAndGenerateReleaseNotes()
    {
        if (!Directory.Exists(ExportDir)) Directory.CreateDirectory(ExportDir);
        string unitypackage = $"com.liltoon.pcss-extension-{Version}.unitypackage";
        string exportPath = Path.Combine(ExportDir, unitypackage);

        // 繝代ャ繧ｱ繝ｼ繧ｸ繧ｨ繧ｯ繧ｹ繝昴・繝・
        AssetDatabase.ExportPackage(ExportPaths, exportPath, ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);
        Debug.Log($"[PCSS] 繝代ャ繧ｱ繝ｼ繧ｸ繧ｨ繧ｯ繧ｹ繝昴・繝亥ｮ御ｺ・ {exportPath}");

        // 繝ｪ繝ｪ繝ｼ繧ｹ繝弱・繝育函謌・
        string changelogPath = "CHANGELOG.md";
        string releaseNotePath = Path.Combine(ExportDir, $"release_notes_{Version}.txt");
        string notes = "";
        if (File.Exists(changelogPath))
        {
            // CHANGELOG.md縺九ｉ隧ｲ蠖薙ヰ繝ｼ繧ｸ繝ｧ繝ｳ縺ｮ繝ｪ繝ｪ繝ｼ繧ｹ繝弱・繝医ｒ謚ｽ蜃ｺ
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
            // git log縺九ｉ逶ｴ霑・0莉ｶ繧貞叙蠕暦ｼ郁ｦ“it繧ｳ繝槭Φ繝会ｼ・
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
        Debug.Log($"[PCSS] 繝ｪ繝ｪ繝ｼ繧ｹ繝弱・繝育函謌仙ｮ御ｺ・ {releaseNotePath}");
        EditorUtility.RevealInFinder(exportPath);
    }
} 
