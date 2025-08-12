using UnityEngine;
using UnityEditor;
using System.IO;

namespace lilToonPCSS.Editor
{
    public class ManualPackageExporter : EditorWindow
    {
        [MenuItem("Tools/lilToon-PCSS-Extension/Manual Export Package Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<ManualPackageExporter>("手動パッケージエクスポーター");
            window.Show();
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Manual Export Package")]
        public static void ManualExportPackage()
        {
            Debug.Log("🎯 手動パッケージエクスポート開始...");
            
            // エクスポート対象のアセットパスを定義
            string[] assetPaths = {
                "Assets/package.json",
                "Assets/README.md",
                "Assets/CHANGELOG.md",
                "Assets/Editor/",
                "Assets/Shaders/",
                "Assets/Runtime/",
                "Assets/Samples~/",
                "Assets/lilToonPCSS/"
            };
            
            // 出力パスを設定
            string outputPath = "../ExportedPackages/com.liltoon.pcss-extension-2.4.0.unitypackage";
            
            // パッケージをエクスポート
            AssetDatabase.ExportPackage(assetPaths, outputPath, ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
            
            Debug.Log($"✅ 手動パッケージエクスポート完了: {outputPath}");
            
            // エクスポート完了後にファイルを開く
            if (File.Exists(outputPath))
            {
                EditorUtility.RevealInFinder(outputPath);
                Debug.Log($"📁 エクスポートされたファイルを開きました: {Path.GetFullPath(outputPath)}");
            }
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Export All Package Types")]
        public static void ExportAllPackageTypes()
        {
            Debug.Log("🚀 全パッケージタイプのエクスポート開始...");
            
            // 1. 標準パッケージ
            ExportStandardPackage();
            
            // 2. クリーンパッケージ
            ExportCleanPackage();
            
            // 3. フルパッケージ
            ExportFullPackage();
            
            // 4. VPMパッケージ
            ExportVPMPackage();
            
            Debug.Log("🎉 全パッケージタイプのエクスポート完了！");
        }
        
        private static void ExportStandardPackage()
        {
            string[] assetPaths = {
                "Assets/package.json",
                "Assets/README.md",
                "Assets/CHANGELOG.md",
                "Assets/Editor/",
                "Assets/Shaders/",
                "Assets/Runtime/",
                "Assets/Samples~/",
                "Assets/lilToonPCSS/"
            };
            
            string outputPath = "../ExportedPackages/com.liltoon.pcss-extension-2.4.0.unitypackage";
            AssetDatabase.ExportPackage(assetPaths, outputPath, ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
            Debug.Log($"✅ 標準パッケージエクスポート完了: {outputPath}");
        }
        
        private static void ExportCleanPackage()
        {
            string[] assetPaths = {
                "Assets/package.json",
                "Assets/README.md",
                "Assets/CHANGELOG.md",
                "Assets/Editor/",
                "Assets/Shaders/",
                "Assets/Runtime/",
                "Assets/Samples~/",
                "Assets/lilToonPCSS/"
            };
            
            string outputPath = "../ExportedPackages/com.liltoon.pcss-extension-2.4.0-clean.unitypackage";
            AssetDatabase.ExportPackage(assetPaths, outputPath, ExportPackageOptions.Recurse);
            Debug.Log($"✅ クリーンパッケージエクスポート完了: {outputPath}");
        }
        
        private static void ExportFullPackage()
        {
            string[] assetPaths = {
                "Assets/"
            };
            
            string outputPath = "../ExportedPackages/com.liltoon.pcss-extension-2.4.0-full.unitypackage";
            AssetDatabase.ExportPackage(assetPaths, outputPath, ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
            Debug.Log($"✅ フルパッケージエクスポート完了: {outputPath}");
        }
        
        private static void ExportVPMPackage()
        {
            string[] assetPaths = {
                "Assets/package.json",
                "Assets/README.md",
                "Assets/CHANGELOG.md",
                "Assets/Editor/",
                "Assets/Shaders/",
                "Assets/Runtime/",
                "Assets/Samples~/",
                "Assets/lilToonPCSS/"
            };
            
            string outputPath = "../ExportedPackages/com.liltoon.pcss-extension-2.4.0-vpm.zip";
            AssetDatabase.ExportPackage(assetPaths, outputPath, ExportPackageOptions.Recurse);
            Debug.Log($"✅ VPMパッケージエクスポート完了: {outputPath}");
        }
    }
} 