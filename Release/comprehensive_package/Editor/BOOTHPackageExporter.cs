using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace lilToon.PCSS
{
    /// <summary>
    /// BOOTH販売用UnityPackage生成ツール
    /// </summary>
    public class BOOTHPackageExporter : EditorWindow
    {
        private static string packageVersion = "1.2.0";
        private static string packageName = "lilToon_PCSS_Extension";
        private static bool includeDocumentation = true;
        private static bool includeSamples = true;
        private static bool includeSourceCode = true;
        
        private Vector2 scrollPosition;
        private bool showAdvancedOptions = false;
        
        [MenuItem("lilToon/PCSS Extension/Export BOOTH Package")]
        public static void ShowWindow()
        {
            var window = GetWindow<BOOTHPackageExporter>("BOOTH Package Exporter");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("📦 BOOTH Package Exporter", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "BOOTH販売用のUnityPackageを生成します。\n" +
                "生成されたパッケージには必要なファイルのみが含まれます。",
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // 基本設定
            EditorGUILayout.LabelField("⚙️ 基本設定", EditorStyles.boldLabel);
            packageVersion = EditorGUILayout.TextField("Package Version", packageVersion);
            packageName = EditorGUILayout.TextField("Package Name", packageName);
            
            EditorGUILayout.Space();
            
            // インクルード設定
            EditorGUILayout.LabelField("📋 インクルード設定", EditorStyles.boldLabel);
            includeDocumentation = EditorGUILayout.Toggle("Documentation", includeDocumentation);
            includeSamples = EditorGUILayout.Toggle("Sample Files", includeSamples);
            includeSourceCode = EditorGUILayout.Toggle("Source Code", includeSourceCode);
            
            EditorGUILayout.Space();
            
            // 高度な設定
            showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "🔧 高度な設定");
            if (showAdvancedOptions)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox(
                    "高度な設定では、パッケージに含めるファイルを詳細に制御できます。",
                    MessageType.Info);
                
                EditorGUILayout.LabelField("含まれるファイル:");
                EditorGUI.indentLevel++;
                
                var filesToInclude = GetFilesToInclude();
                foreach (var file in filesToInclude.Take(10)) // 最初の10個のみ表示
                {
                    EditorGUILayout.LabelField($"• {file}", EditorStyles.miniLabel);
                }
                
                if (filesToInclude.Count > 10)
                {
                    EditorGUILayout.LabelField($"... and {filesToInclude.Count - 10} more files", EditorStyles.miniLabel);
                }
                
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
            
            // パッケージ情報
            EditorGUILayout.LabelField("📊 パッケージ情報", EditorStyles.boldLabel);
            var filesToInclude2 = GetFilesToInclude();
            EditorGUILayout.LabelField($"ファイル数: {filesToInclude2.Count}");
            EditorGUILayout.LabelField($"推定サイズ: {CalculatePackageSize(filesToInclude2)} KB");
            
            EditorGUILayout.Space();
            
            // エクスポートボタン
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("📦 UnityPackageをエクスポート", GUILayout.Height(40)))
            {
                ExportPackage();
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space();
            
            // 情報表示
            EditorGUILayout.LabelField("ℹ️ 注意事項", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "• エクスポート前にプロジェクトを保存してください\n" +
                "• 生成されたパッケージはBOOTH販売用に最適化されています\n" +
                "• ライセンス情報が自動的に含まれます\n" +
                "• テスト用ファイルは除外されます",
                MessageType.Warning);
            
            EditorGUILayout.EndScrollView();
        }
        
        private List<string> GetFilesToInclude()
        {
            var files = new List<string>();
            
            // 必須ファイル
            files.Add("package.json");
            files.Add("README.md");
            files.Add("LICENSE");
            
            // Runtime ファイル
            if (Directory.Exists("Runtime"))
            {
                files.AddRange(Directory.GetFiles("Runtime", "*.cs", SearchOption.AllDirectories)
                    .Where(f => !f.Contains("Test") && !f.Contains(".meta")));
                files.AddRange(Directory.GetFiles("Runtime", "*.asmdef", SearchOption.AllDirectories)
                    .Where(f => !f.Contains(".meta")));
            }
            
            // Editor ファイル
            if (Directory.Exists("Editor"))
            {
                files.AddRange(Directory.GetFiles("Editor", "*.cs", SearchOption.AllDirectories)
                    .Where(f => !f.Contains("Test") && !f.Contains(".meta")));
                files.AddRange(Directory.GetFiles("Editor", "*.asmdef", SearchOption.AllDirectories)
                    .Where(f => !f.Contains(".meta")));
            }
            
            // Shader ファイル
            if (Directory.Exists("Shaders"))
            {
                files.AddRange(Directory.GetFiles("Shaders", "*.shader", SearchOption.AllDirectories)
                    .Where(f => !f.Contains(".meta")));
                files.AddRange(Directory.GetFiles("Shaders", "*.hlsl", SearchOption.AllDirectories)
                    .Where(f => !f.Contains(".meta")));
            }
            
            // ドキュメント
            if (includeDocumentation)
            {
                if (File.Exists("VRC_Light_Volumes_Integration_Guide.md"))
                    files.Add("VRC_Light_Volumes_Integration_Guide.md");
                if (File.Exists("PCSS_Poiyomi_VRChat_Guide.md"))
                    files.Add("PCSS_Poiyomi_VRChat_Guide.md");
                if (Directory.Exists("_docs"))
                {
                    files.AddRange(Directory.GetFiles("_docs", "*.md", SearchOption.AllDirectories)
                        .Where(f => !f.Contains(".meta")));
                }
            }
            
            // サンプル
            if (includeSamples && Directory.Exists("Samples"))
            {
                files.AddRange(Directory.GetFiles("Samples", "*", SearchOption.AllDirectories)
                    .Where(f => !f.Contains(".meta") && !f.Contains("Test")));
            }
            
            return files.Where(File.Exists).ToList();
        }
        
        private long CalculatePackageSize(List<string> files)
        {
            long totalSize = 0;
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    totalSize += new FileInfo(file).Length;
                }
            }
            return totalSize / 1024; // KB単位
        }
        
        private void ExportPackage()
        {
            try
            {
                var filesToInclude = GetFilesToInclude();
                
                if (filesToInclude.Count == 0)
                {
                    EditorUtility.DisplayDialog("エラー", "エクスポートするファイルが見つかりません。", "OK");
                    return;
                }
                
                // エクスポートパス
                string exportPath = EditorUtility.SaveFilePanel(
                    "UnityPackageをエクスポート",
                    "",
                    $"{packageName}_v{packageVersion}.unitypackage",
                    "unitypackage");
                
                if (string.IsNullOrEmpty(exportPath))
                    return;
                
                // パッケージ情報ファイルを一時的に作成
                CreatePackageInfoFile();
                
                // UnityPackageをエクスポート
                AssetDatabase.ExportPackage(
                    filesToInclude.ToArray(),
                    exportPath,
                    ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
                
                // 一時ファイルを削除
                CleanupTempFiles();
                
                EditorUtility.DisplayDialog(
                    "エクスポート完了",
                    $"UnityPackageが正常にエクスポートされました。\n\n" +
                    $"ファイル: {Path.GetFileName(exportPath)}\n" +
                    $"場所: {Path.GetDirectoryName(exportPath)}\n" +
                    $"サイズ: {new FileInfo(exportPath).Length / 1024} KB",
                    "OK");
                
                // エクスプローラーで表示
                EditorUtility.RevealInFinder(exportPath);
                
                Debug.Log($"BOOTH Package exported: {exportPath}");
            }
            catch (System.Exception e)
            {
            {  
using UnityEngine;\nusing UnityEditor;\nusing System.IO;\nusing System.Collections.Generic;\nusing System.Linq;\n\nnamespace lilToon.PCSS\n{\n    /// <summary>\n    /// BOOTH販売用UnityPackage生成ツール\n    /// </summary>\n    public class BOOTHPackageExporter : EditorWindow\n    {\n        private static string packageVersion = \"1.2.0\";\n        private static string packageName = \"lilToon_PCSS_Extension\";\n        private static bool includeDocumentation = true;\n        private static bool includeSamples = true;\n        private static bool includeSourceCode = true;\n        \n        private Vector2 scrollPosition;\n        private bool showAdvancedOptions = false;\n        \n        [MenuItem(\"lilToon/PCSS Extension/Export BOOTH Package\")]\n        public static void ShowWindow()\n        {\n            var window = GetWindow<BOOTHPackageExporter>(\"BOOTH Package Exporter\");\n            window.minSize = new Vector2(400, 600);\n            window.Show();\n        }\n        \n        private void OnGUI()\n        {\n            EditorGUILayout.Space();\n            EditorGUILayout.LabelField(\"📦 BOOTH Package Exporter\", EditorStyles.boldLabel);\n            EditorGUILayout.Space();\n            \n            EditorGUILayout.HelpBox(\n                \"BOOTH販売用のUnityPackageを生成します。\\n\" +\n                \"生成されたパッケージには必要なファイルのみが含まれます。\",\n                MessageType.Info);\n            \n            EditorGUILayout.Space();\n            \n            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);\n            \n            // 基本設定\n            EditorGUILayout.LabelField(\"⚙️ 基本設定\", EditorStyles.boldLabel);\n            packageVersion = EditorGUILayout.TextField(\"Package Version\", packageVersion);\n            packageName = EditorGUILayout.TextField(\"Package Name\", packageName);\n            \n            EditorGUILayout.Space();\n            \n            // インクルード設定\n            EditorGUILayout.LabelField(\"📋 インクルード設定\", EditorStyles.boldLabel);\n            includeDocumentation = EditorGUILayout.Toggle(\"Documentation\", includeDocumentation);\n            includeSamples = EditorGUILayout.Toggle(\"Sample Files\", includeSamples);\n            includeSourceCode = EditorGUILayout.Toggle(\"Source Code\", includeSourceCode);\n            \n            EditorGUILayout.Space();\n            \n            // 高度な設定\n            showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, \"🔧 高度な設定\");\n            if (showAdvancedOptions)\n            {\n                EditorGUI.indentLevel++;\n                EditorGUILayout.HelpBox(\n                    \"高度な設定では、パッケージに含めるファイルを詳細に制御できます。\",\n                    MessageType.Info);\n                \n                EditorGUILayout.LabelField(\"含まれるファイル:\");\n                EditorGUI.indentLevel++;\n                \n                var filesToInclude = GetFilesToInclude();\n                foreach (var file in filesToInclude.Take(10)) // 最初の10個のみ表示\n                {\n                    EditorGUILayout.LabelField($\"• {file}\", EditorStyles.miniLabel);\n                }\n                \n                if (filesToInclude.Count > 10)\n                {\n                    EditorGUILayout.LabelField($\"... and {filesToInclude.Count - 10} more files\", EditorStyles.miniLabel);\n                }\n                \n                EditorGUI.indentLevel--;\n                EditorGUI.indentLevel--;\n            }\n            \n            EditorGUILayout.Space();\n            \n            // パッケージ情報\n            EditorGUILayout.LabelField(\"📊 パッケージ情報\", EditorStyles.boldLabel);\n            var filesToInclude2 = GetFilesToInclude();\n            EditorGUILayout.LabelField($\"ファイル数: {filesToInclude2.Count}\");\n            EditorGUILayout.LabelField($\"推定サイズ: {CalculatePackageSize(filesToInclude2)} KB\");\n            \n            EditorGUILayout.Space();\n            \n            // エクスポートボタン\n            GUI.backgroundColor = Color.green;\n            if (GUILayout.Button(\"📦 UnityPackageをエクスポート\", GUILayout.Height(40)))\n            {\n                ExportPackage();\n            }\n            GUI.backgroundColor = Color.white;\n            \n            EditorGUILayout.Space();\n            \n            // 情報表示\n            EditorGUILayout.LabelField(\"ℹ️ 注意事項\", EditorStyles.boldLabel);\n            EditorGUILayout.HelpBox(\n                \"• エクスポート前にプロジェクトを保存してください\\n\" +\n                \"• 生成されたパッケージはBOOTH販売用に最適化されています\\n\" +\n                \"• ライセンス情報が自動的に含まれます\\n\" +\n                \"• テスト用ファイルは除外されます\",\n                MessageType.Warning);\n            \n            EditorGUILayout.EndScrollView();\n        }\n        \n        private List<string> GetFilesToInclude()\n        {\n            var files = new List<string>();\n            \n            // 必須ファイル\n            files.Add(\"package.json\");\n            files.Add(\"README.md\");\n            files.Add(\"LICENSE\");\n            \n            // Runtime ファイル\n            files.AddRange(Directory.GetFiles(\"Runtime\", \"*.cs\", SearchOption.AllDirectories)\n                .Where(f => !f.Contains(\"Test\") && !f.Contains(\".meta\")));\n            files.AddRange(Directory.GetFiles(\"Runtime\", \"*.asmdef\", SearchOption.AllDirectories)\n                .Where(f => !f.Contains(\".meta\")));\n            \n            // Editor ファイル\n            files.AddRange(Directory.GetFiles(\"Editor\", \"*.cs\", SearchOption.AllDirectories)\n                .Where(f => !f.Contains(\"Test\") && !f.Contains(\".meta\")));\n            files.AddRange(Directory.GetFiles(\"Editor\", \"*.asmdef\", SearchOption.AllDirectories)\n                .Where(f => !f.Contains(\".meta\")));\n            \n            // Shader ファイル\n            files.AddRange(Directory.GetFiles(\"Shaders\", \"*.shader\", SearchOption.AllDirectories)\n                .Where(f => !f.Contains(\".meta\")));\n            files.AddRange(Directory.GetFiles(\"Shaders\", \"*.hlsl\", SearchOption.AllDirectories)\n                .Where(f => !f.Contains(\".meta\")));\n            \n            // ドキュメント\n            if (includeDocumentation)\n            {\n                if (File.Exists(\"VRC_Light_Volumes_Integration_Guide.md\"))\n                    files.Add(\"VRC_Light_Volumes_Integration_Guide.md\");\n                if (File.Exists(\"PCSS_Poiyomi_VRChat_Guide.md\"))\n                    files.Add(\"PCSS_Poiyomi_VRChat_Guide.md\");\n                if (Directory.Exists(\"_docs\"))\n                {\n                    files.AddRange(Directory.GetFiles(\"_docs\", \"*.md\", SearchOption.AllDirectories)\n                        .Where(f => !f.Contains(\".meta\")));\n                }\n            }\n            \n            // サンプル\n            if (includeSamples && Directory.Exists(\"Samples\"))\n            {\n                files.AddRange(Directory.GetFiles(\"Samples\", \"*\", SearchOption.AllDirectories)\n                    .Where(f => !f.Contains(\".meta\") && !f.Contains(\"Test\")));\n            }\n            \n            return files.Where(File.Exists).ToList();\n        }\n        \n        private long CalculatePackageSize(List<string> files)\n        {\n            long totalSize = 0;\n            foreach (var file in files)\n            {\n                if (File.Exists(file))\n                {\n                    totalSize += new FileInfo(file).Length;\n                }\n            }\n            return totalSize / 1024; // KB単位\n        }\n        \n        private void ExportPackage()\n        {\n            try\n            {\n                var filesToInclude = GetFilesToInclude();\n                \n                if (filesToInclude.Count == 0)\n                {\n                    EditorUtility.DisplayDialog(\"エラー\", \"エクスポートするファイルが見つかりません。\", \"OK\");\n                    return;\n                }\n                \n                // エクスポートパス\n                string exportPath = EditorUtility.SaveFilePanel(\n                    \"UnityPackageをエクスポート\",\n                    \"\",\n                    $\"{packageName}_v{packageVersion}.unitypackage\",\n                    \"unitypackage\");\n                \n                if (string.IsNullOrEmpty(exportPath))\n                    return;\n                \n                // パッケージ情報ファイルを一時的に作成\n                CreatePackageInfoFile();\n                \n                // UnityPackageをエクスポート\n                AssetDatabase.ExportPackage(\n                    filesToInclude.ToArray(),\n                    exportPath,\n                    ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);\n                \n                // 一時ファイルを削除\n                CleanupTempFiles();\n                \n                EditorUtility.DisplayDialog(\n                    \"エクスポート完了\",\n                    $\"UnityPackageが正常にエクスポートされました。\\n\\n\" +\n                    $\"ファイル: {Path.GetFileName(exportPath)}\\n\" +\n                    $\"場所: {Path.GetDirectoryName(exportPath)}\\n\" +\n                    $\"サイズ: {new FileInfo(exportPath).Length / 1024} KB\",\n                    \"OK\");\n                \n                // エクスプローラーで表示\n                EditorUtility.RevealInFinder(exportPath);\n                \n                Debug.Log($\"BOOTH Package exported: {exportPath}\");\n            }\n            catch (System.Exception e)\n            {\n                EditorUtility.DisplayDialog(\"エラー\", $\"エクスポート中にエラーが発生しました:\\n{e.Message}\", \"OK\");\n                Debug.LogError($\"Package export failed: {e}\");\n            }\n        }\n        \n        private void CreatePackageInfoFile()\n        {\n            var packageInfo = new\n            {\n                name = packageName,\n                version = packageVersion,\n                exportDate = System.DateTime.Now.ToString(\"yyyy-MM-dd HH:mm:ss\"),\n                unity = Application.unityVersion,\n                features = new string[]\n                {\n                    \"PCSS (Percentage-Closer Soft Shadows)\",\n                    \"lilToon Integration\",\n                    \"Poiyomi Integration\",\n                    \"VRChat Expression Control\",\n                    \"VRC Light Volumes Support\",\n                    \"ModularAvatar Support\"\n                }\n            };\n            \n            string json = JsonUtility.ToJson(packageInfo, true);\n            File.WriteAllText(\"PackageInfo.json\", json);\n        }\n        \n        private void CleanupTempFiles()\n        {\n            if (File.Exists(\"PackageInfo.json\"))\n            {\n                File.Delete(\"PackageInfo.json\");\n            }\n            if (File.Exists(\"PackageInfo.json.meta\"))\n            {\n                File.Delete(\"PackageInfo.json.meta\");\n            }\n        }\n        \n        [MenuItem(\"lilToon/PCSS Extension/Export BOOTH Package\", true)]\n        public static bool ValidateExportPackage()\n        {\n            return Directory.Exists(\"Runtime\") && Directory.Exists(\"Editor\") && Directory.Exists(\"Shaders\");\n        }\n        \n        [MenuItem(\"lilToon/PCSS Extension/Validate Package\")]\n        public static void ValidatePackage()\n        {\n            var issues = new List<string>();\n            \n            // 必須ファイルのチェック\n            if (!File.Exists(\"package.json\")) issues.Add(\"package.json が見つかりません\");\n            if (!File.Exists(\"README.md\")) issues.Add(\"README.md が見つかりません\");\n            if (!File.Exists(\"LICENSE\")) issues.Add(\"LICENSE が見つかりません\");\n            \n            // ディレクトリのチェック\n            if (!Directory.Exists(\"Runtime\")) issues.Add(\"Runtime ディレクトリが見つかりません\");\n            if (!Directory.Exists(\"Editor\")) issues.Add(\"Editor ディレクトリが見つかりません\");\n            if (!Directory.Exists(\"Shaders\")) issues.Add(\"Shaders ディレクトリが見つかりません\");\n            \n            // シェーダーファイルのチェック\n            if (!File.Exists(\"Shaders/lilToon_PCSS_Extension.shader\"))\n                issues.Add(\"lilToon PCSS シェーダーが見つかりません\");\n            if (!File.Exists(\"Shaders/Poiyomi_PCSS_Extension.shader\"))\n                issues.Add(\"Poiyomi PCSS シェーダーが見つかりません\");\n            \n            // 結果表示\n            if (issues.Count == 0)\n            {\n                EditorUtility.DisplayDialog(\n                    \"✅ パッケージ検証完了\",\n                    \"すべての必須ファイルが揃っています。\\nBOOTH販売の準備が完了しました！\",\n                    \"OK\");\n            }\n            else\n            {\n                string message = \"以下の問題が見つかりました:\\n\\n\" + string.Join(\"\\n\", issues);\n                EditorUtility.DisplayDialog(\"❌ パッケージ検証エラー\", message, \"OK\");\n            }\n        }\n    }\n} 