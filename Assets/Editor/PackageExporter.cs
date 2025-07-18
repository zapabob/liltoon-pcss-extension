using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Linq;

public class PackageExporter
{
    private const string ExportDir = "ExportedPackages";
    private const string Version = "1.5.6"; // å¿E¦ã«å¿ãã¦èªååå¾å¯
    private static readonly string[] ExportPaths = {
        "Assets/Editor/MaterialSessionRestorer.cs",
        // ä»ã«å«ãããEã¡ã¤ã«ãEã©ã«ãããããEããã«è¿½å 
    };

    [MenuItem("Tools/PCSS/ããã±ã¼ã¸èªåã¨ã¯ã¹ããEãï¼Eªãªã¼ã¹ããEãçæE)]
    public static void ExportAndGenerateReleaseNotes()
    {
        if (!Directory.Exists(ExportDir)) Directory.CreateDirectory(ExportDir);
        string unitypackage = $"com.liltoon.pcss-extension-{Version}.unitypackage";
        string exportPath = Path.Combine(ExportDir, unitypackage);

        // ããã±ã¼ã¸ã¨ã¯ã¹ããEãE
        AssetDatabase.ExportPackage(ExportPaths, exportPath, ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);
        Debug.Log($"[PCSS] ããã±ã¼ã¸ã¨ã¯ã¹ããEãå®äºE {exportPath}");

        // ãªãªã¼ã¹ããEãçæE
        string changelogPath = "CHANGELOG.md";
        string releaseNotePath = Path.Combine(ExportDir, $"release_notes_{Version}.txt");
        string notes = "";
        if (File.Exists(changelogPath))
        {
            // CHANGELOG.mdããè©²å½ãã¼ã¸ã§ã³ã®ãªãªã¼ã¹ããEããæ½åº
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
            // git logããç´è¿E0ä»¶ãåå¾ï¼è¦gitã³ãã³ãï¼E
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
        Debug.Log($"[PCSS] ãªãªã¼ã¹ããEãçæå®äºE {releaseNotePath}");
        EditorUtility.RevealInFinder(exportPath);
    }
} 
