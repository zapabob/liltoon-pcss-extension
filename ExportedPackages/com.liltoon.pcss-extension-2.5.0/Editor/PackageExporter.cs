using UnityEngine;
using UnityEditor;
using System.IO;

namespace lilToonPCSS.Editor
{
    public class PackageExporter : EditorWindow
    {
        private const string PACKAGE_NAME = "com.liltoon.pcss-extension";
        private const string DISPLAY_NAME = "lilToon PCSS Extension";
        private const string VERSION = "2.4.0";
        
        [MenuItem("Tools/lilToon PCSS Extension/Export Package")]
        public static void ExportPackage()
        {
            Debug.Log("🎯 lilToon PCSS Extension パッケージエクスポート開始...");
            
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
            string outputPath = $"../{PACKAGE_NAME}-{VERSION}.unitypackage";
            
            // パッケージをエクスポート
            AssetDatabase.ExportPackage(assetPaths, outputPath, ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
            
            Debug.Log($"✅ パッケージエクスポート完了: {outputPath}");
            
            // エクスポート完了後にファイルを開く
            if (File.Exists(outputPath))
            {
                EditorUtility.RevealInFinder(outputPath);
                Debug.Log($"📁 エクスポートされたファイルを開きました: {Path.GetFullPath(outputPath)}");
            }
        }
        
        [MenuItem("Tools/lilToon PCSS Extension/Export Package (Clean)")]
        public static void ExportPackageClean()
        {
            Debug.Log("🧹 lilToon PCSS Extension クリーンパッケージエクスポート開始...");
            
            // クリーンエクスポート用のアセットパス（不要なファイルを除外）
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
            string outputPath = $"../{PACKAGE_NAME}-{VERSION}-clean.unitypackage";
            
            // クリーンパッケージをエクスポート
            AssetDatabase.ExportPackage(assetPaths, outputPath, ExportPackageOptions.Recurse);
            
            Debug.Log($"✅ クリーンパッケージエクスポート完了: {outputPath}");
            
            // エクスポート完了後にファイルを開く
            if (File.Exists(outputPath))
            {
                EditorUtility.RevealInFinder(outputPath);
                Debug.Log($"📁 エクスポートされたファイルを開きました: {Path.GetFullPath(outputPath)}");
            }
        }
        
        [MenuItem("Tools/lilToon PCSS Extension/Export Package (Full)")]
        public static void ExportPackageFull()
        {
            Debug.Log("🚀 lilToon PCSS Extension フルパッケージエクスポート開始...");
            
            // フルエクスポート用のアセットパス（全てのアセットを含む）
            string[] assetPaths = {
                "Assets/"
            };
            
            // 出力パスを設定
            string outputPath = $"../{PACKAGE_NAME}-{VERSION}-full.unitypackage";
            
            // フルパッケージをエクスポート
            AssetDatabase.ExportPackage(assetPaths, outputPath, ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
            
            Debug.Log($"✅ フルパッケージエクスポート完了: {outputPath}");
            
            // エクスポート完了後にファイルを開く
            if (File.Exists(outputPath))
            {
                EditorUtility.RevealInFinder(outputPath);
                Debug.Log($"📁 エクスポートされたファイルを開きました: {Path.GetFullPath(outputPath)}");
            }
        }
        
        [MenuItem("Tools/lilToon PCSS Extension/Export Package (VPM)")]
        public static void ExportPackageVPM()
        {
            Debug.Log("📦 lilToon PCSS Extension VPMパッケージエクスポート開始...");
            
            // VPM用のアセットパス（VRChat World SDK対応）
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
            string outputPath = $"../{PACKAGE_NAME}-{VERSION}-vpm.zip";
            
            // VPMパッケージをエクスポート
            AssetDatabase.ExportPackage(assetPaths, outputPath, ExportPackageOptions.Recurse);
            
            Debug.Log($"✅ VPMパッケージエクスポート完了: {outputPath}");
            
            // エクスポート完了後にファイルを開く
            if (File.Exists(outputPath))
            {
                EditorUtility.RevealInFinder(outputPath);
                Debug.Log($"📁 エクスポートされたファイルを開きました: {Path.GetFullPath(outputPath)}");
            }
        }
    }
} 
