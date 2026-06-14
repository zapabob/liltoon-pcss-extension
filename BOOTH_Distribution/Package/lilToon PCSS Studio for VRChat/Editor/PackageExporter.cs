using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// lilToon PCSS Extension パッケージエクスポーター
    /// なんｊ風に言うと「これで完璧なパッケージエクスポートシステムが完成したぜ！」💪🔥
    /// </summary>
    public class PackageExporter
    {
        private static readonly string PackageName = "lilToon-PCSS-Extension";
        private static readonly string PackageVersion = "2.2.0";
        private static readonly string PackageDisplayName = "lilToon PCSS Extension";
        private static readonly string PackageDescription = "Enhanced PCSS (Percentage Closer Soft Shadows) extension for lilToon shader with advanced features and optimizations.";
        private static readonly string PackageAuthor = "lilToon PCSS Team";
        private static readonly string PackageUnityVersion = "2022.3.0f1";
        private static readonly string PackageKeywords = "lilToon, PCSS, Shadows, VRChat, Avatar, Shader";

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Package/Export Package")]
        public static void ExportPackage()
        {
            try
            {
                string packagePath = ExportAndGenerateReleaseNotes();
                EditorUtility.DisplayDialog("Export Complete", $"Package exported successfully to:\n{packagePath}", "OK");
                Debug.Log($"Package exported to: {packagePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Package export failed: {e.Message}");
                EditorUtility.DisplayDialog("Export Failed", $"Package export failed:\n{e.Message}", "OK");
            }
        }

        public static string ExportAndGenerateReleaseNotes()
        {
            // 1. パッケージマニフェストの作成
            CreatePackageManifest();

            // 2. リリースノートの生成
            GenerateReleaseNotes();

            // 3. パッケージのエクスポート
            string packagePath = ExportPackageFiles();

            // 4. ドキュメントの生成
            GenerateDocumentation();

            return packagePath;
        }

        private static void CreatePackageManifest()
        {
            var manifest = new Dictionary<string, object>
            {
                ["name"] = PackageName,
                ["version"] = PackageVersion,
                ["displayName"] = PackageDisplayName,
                ["description"] = PackageDescription,
                ["unity"] = PackageUnityVersion,
                ["keywords"] = PackageKeywords.Split(','),
                ["author"] = new Dictionary<string, string>
                {
                    ["name"] = PackageAuthor,
                    ["email"] = "support@liltoon-pcss.com",
                    ["url"] = "https://github.com/liltoon-pcss"
                },
                ["dependencies"] = new Dictionary<string, string>
                {
                    ["com.unity.render-pipelines.universal"] = "14.0.0",
                    ["com.vrchat.avatars"] = "3.1.0"
                },
                ["samples"] = new object[]
                {
                    new Dictionary<string, object>
                    {
                        ["displayName"] = "Basic PCSS Setup",
                        ["description"] = "Basic PCSS setup example",
                        ["path"] = "Samples~/BasicPCSSSetup"
                    },
                    new Dictionary<string, object>
                    {
                        ["displayName"] = "Advanced PCSS Features",
                        ["description"] = "Advanced PCSS features and optimizations",
                        ["path"] = "Samples~/AdvancedPCSSFeatures"
                    }
                }
            };

            string manifestJson = JsonUtility.ToJson(manifest, true);
            string manifestPath = "Assets/New Folder/package.json";
            File.WriteAllText(manifestPath, manifestJson);
            AssetDatabase.Refresh();
        }

        private static void GenerateReleaseNotes()
        {
            var releaseNotes = new List<string>
            {
                "# lilToon PCSS Extension v2.2.0",
                "",
                "## 🚀 New Features",
                "- Enhanced PCSS (Percentage Closer Soft Shadows) implementation",
                "- Advanced shadow quality controls",
                "- VRChat optimization features",
                "- Modular Avatar integration",
                "- Automatic material backup system",
                "- Performance optimization tools",
                "",
                "## 🔧 Improvements",
                "- Improved shadow filtering algorithms",
                "- Better memory management",
                "- Enhanced UI/UX for all tools",
                "- Comprehensive documentation",
                "",
                "## 🐛 Bug Fixes",
                "- Fixed shadow artifacts in certain lighting conditions",
                "- Resolved compatibility issues with VRChat SDK",
                "- Fixed material backup restoration issues",
                "",
                "## 📦 Installation",
                "1. Import the package into your Unity project",
                "2. Ensure VRChat SDK is installed",
                "3. Use the provided tools to set up PCSS",
                "",
                "## 🎯 Usage",
                "1. Open Tools > lilToon PCSS > Setup Wizard",
                "2. Follow the guided setup process",
                "3. Configure your materials with PCSS",
                "4. Use the optimization tools for VRChat",
                "",
                "## 📚 Documentation",
                "See the included documentation for detailed usage instructions.",
                "",
                "## 🤝 Support",
                "For support, visit: https://github.com/liltoon-pcss/support",
                "",
                "## 📄 License",
                "MIT License - see LICENSE file for details."
            };

            string releaseNotesPath = "Assets/New Folder/CHANGELOG.md";
            File.WriteAllLines(releaseNotesPath, releaseNotes);
            AssetDatabase.Refresh();
        }

        private static string ExportPackageFiles()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string packageFileName = $"{PackageName}_v{PackageVersion}_{timestamp}.unitypackage";
            string packagePath = Path.Combine(Application.dataPath, "..", "Exports", packageFileName);

            // エクスポートディレクトリの作成
            string exportDir = Path.GetDirectoryName(packagePath);
            if (!Directory.Exists(exportDir))
            {
                Directory.CreateDirectory(exportDir);
            }

            // エクスポートするアセットのリスト
            var assetsToExport = new List<string>
            {
                "Assets/New Folder/Editor",
                "Assets/New Folder/Runtime",
                "Assets/New Folder/Documentation",
                "Assets/New Folder/Samples",
                "Assets/New Folder/package.json",
                "Assets/New Folder/CHANGELOG.md",
                "Assets/New Folder/README.md"
            };

            // パッケージのエクスポート
            AssetDatabase.ExportPackage(assetsToExport.ToArray(), packagePath, ExportPackageOptions.Recurse);

            return packagePath;
        }

        private static void GenerateDocumentation()
        {
            var documentation = new List<string>
            {
                "# lilToon PCSS Extension Documentation",
                "",
                "## Overview",
                "The lilToon PCSS Extension provides enhanced Percentage Closer Soft Shadows (PCSS) implementation for lilToon shader, optimized for VRChat avatars.",
                "",
                "## Features",
                "",
                "### 🎯 Core Features",
                "- **Enhanced PCSS**: Advanced shadow filtering for realistic soft shadows",
                "- **VRChat Optimization**: Specialized optimizations for VRChat platform",
                "- **Modular Avatar Integration**: Seamless integration with Modular Avatar system",
                "- **Material Backup System**: Automatic backup and restoration of materials",
                "- **Performance Tools**: Comprehensive performance optimization tools",
                "",
                "### 🔧 Tools Included",
                "- **Setup Wizard**: Guided setup process for beginners",
                "- **Material AutoFixer**: Automatic fixing of missing materials",
                "- **PCSS Menu System**: Advanced menu system for PCSS controls",
                "- **Performance Optimizer**: Performance analysis and optimization",
                "- **Package Exporter**: Easy package export with release notes",
                "",
                "## Installation",
                "",
                "### Prerequisites",
                "- Unity 2022.3.0f1 or later",
                "- VRChat SDK 3.1.0 or later",
                "- lilToon shader",
                "",
                "### Installation Steps",
                "1. Import the package into your Unity project",
                "2. Ensure all prerequisites are installed",
                "3. Run the Setup Wizard from Tools > lilToon PCSS > Setup Wizard",
                "4. Follow the guided setup process",
                "",
                "## Usage",
                "",
                "### Basic Setup",
                "1. Open the Setup Wizard",
                "2. Select your avatar",
                "3. Configure PCSS settings",
                "4. Apply the configuration",
                "",
                "### Advanced Features",
                "- Use the PCSS Menu System for advanced controls",
                "- Utilize the Performance Optimizer for VRChat optimization",
                "- Use the Material AutoFixer for missing material issues",
                "",
                "## Configuration",
                "",
                "### PCSS Settings",
                "- **Filter Radius**: Controls the softness of shadows",
                "- **Light Size**: Determines the size of the light source",
                "- **Bias**: Adjusts shadow bias to prevent artifacts",
                "- **Intensity**: Controls shadow intensity",
                "- **Quality**: Sets the quality level (Low/Medium/High)",
                "",
                "### VRChat Optimization",
                "- **Performance Mode**: Optimizes for VRChat performance",
                "- **Memory Management**: Efficient memory usage",
                "- **Shader Compilation**: Optimized shader compilation",
                "",
                "## Troubleshooting",
                "",
                "### Common Issues",
                "1. **Missing Materials**: Use the Material AutoFixer tool",
                "2. **Performance Issues**: Run the Performance Optimizer",
                "3. **Shader Errors**: Check shader compilation in the console",
                "",
                "### Support",
                "For additional support, visit: https://github.com/liltoon-pcss/support",
                "",
                "## License",
                "MIT License - see LICENSE file for details."
            };

            string documentationPath = "Assets/New Folder/README.md";
            File.WriteAllLines(documentationPath, documentation);
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Package/Generate Documentation")]
        public static void GenerateDocumentationOnly()
        {
            GenerateDocumentation();
            EditorUtility.DisplayDialog("Documentation Generated", "Documentation has been generated successfully.", "OK");
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Package/Create Package Manifest")]
        public static void CreatePackageManifestOnly()
        {
            CreatePackageManifest();
            EditorUtility.DisplayDialog("Package Manifest Created", "Package manifest has been created successfully.", "OK");
        }
    }
}