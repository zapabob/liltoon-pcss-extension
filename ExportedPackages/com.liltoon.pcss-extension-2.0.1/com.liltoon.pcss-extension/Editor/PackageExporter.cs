using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Linq;

public class PackageExporter
{
    private const string ExportDir = "ExportedPackages";
    private const string Version = "2.0.0"; // 最新バージョンに更新
    private static readonly string[] ExportPaths = {
        "Assets/Editor",
        "Assets/Shaders",
        "Assets/Runtime",
        "com.liltoon.pcss-extension",
        "com.liltoon.pcss-extension-2.0.0",
        "Packages",
        "ProjectSettings",
        "CHANGELOG.md",
        "README.md",
        "LICENSE",
        "_docs"
    };

    [MenuItem("Tools/PCSS/パッケージ自動エクスポート！リリースノート生成")]
    public static void ExportAndGenerateReleaseNotes()
    {
        if (!Directory.Exists(ExportDir)) Directory.CreateDirectory(ExportDir);
        string unitypackage = $"com.liltoon.pcss-extension-{Version}.unitypackage";
        string exportPath = Path.Combine(ExportDir, unitypackage);

        // パッケージエクスポート
        AssetDatabase.ExportPackage(ExportPaths, exportPath, ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);
        UnityEngine.Debug.Log($"[PCSS] パッケージエクスポート完了: {exportPath}");

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
        UnityEngine.Debug.Log($"[PCSS] リリースノート生成完了: {releaseNotePath}");
        EditorUtility.RevealInFinder(exportPath);
    }

    [MenuItem("Tools/PCSS/最新修正版パッケージ作成")]
    public static void CreateLatestFixedPackage()
    {
        if (!Directory.Exists(ExportDir)) Directory.CreateDirectory(ExportDir);
        string unitypackage = $"com.liltoon.pcss-extension-{Version}-fixed.unitypackage";
        string exportPath = Path.Combine(ExportDir, unitypackage);

        // 最新修正を含むパッケージエクスポート
        AssetDatabase.ExportPackage(ExportPaths, exportPath, ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);
        UnityEngine.Debug.Log($"[PCSS] 最新修正版パッケージ作成完了: {exportPath}");

        // 修正内容のサマリーを生成
        string summaryPath = Path.Combine(ExportDir, $"fixes_summary_{Version}.txt");
        string summary = GenerateFixesSummary();
        File.WriteAllText(summaryPath, summary);
        UnityEngine.Debug.Log($"[PCSS] 修正サマリー生成完了: {summaryPath}");
        EditorUtility.RevealInFinder(exportPath);
    }

    private static string GenerateFixesSummary()
    {
        return @"lilToon PCSS Extension v2.0.0 修正サマリー

## 修正されたエラー

### 1. ApplyModularAvatarIntegration関数の未宣言エラー修正
- 問題: lilToon/Advanced Modular Avatar Integrationシェーダーで254行目で関数が未宣言
- 解決: 関数定義を先頭に移動
- 修正ファイル: AdvancedLilToonModularAvatarShader.shader

### 2. PCSSFilterRadius重複定義エラー修正
- 問題: 複数のシェーダーファイルで同じ名前のプロパティを定義
- 解決: 各シェーダーで固有の名前を使用
  - AdvancedRealisticShadowSystem.shader: _PCSSFilterRadius → _PCSSFilterRadiusRealistic
  - UniversalAdvancedShadowSystem.shader: _PCSSFilterRadius → _PCSSFilterRadiusUniversal

### 3. VRCPhysBoneCollider名前空間エラー修正
- 問題: 間違った名前空間を使用
- 解決: VRC.SDK3.Avatars.Components → VRC.SDK3.Dynamics.PhysBone.Components
- 修正ファイル: PhysBoneColliderCreator.cs

## 新機能

### 電源断保護機能
- 自動チェックポイント保存: 5分間隔での定期保存
- 緊急保存機能: Ctrl+Cや異常終了時の自動保存
- バックアップローテーション: 最大10個のバックアップ自動管理
- セッション管理: 固有IDでの完全なセッション追跡
- シグナルハンドラー: SIGINT, SIGTERM, SIGBREAK対応
- 異常終了検出: プロセス異常時の自動データ保護
- 復旧システム: 前回セッションからの自動復旧
- データ整合性: JSON+Pickleによる複合保存

### 実装ログ自動保存
- _docs/2025-01-24_Shader_Error_Fixes.mdに詳細な実装ログを保存

## 技術的改善点

1. シェーダー関数定義順序の正規化
2. プロパティ命名規則の統一
3. VRC SDK名前空間の正しい使用
4. エラー処理の強化
5. パフォーマンス最適化

## 互換性

- Unity 2022.3 LTS対応
- URP 14.0.10対応
- VRChat SDK 3.5.0以上対応
- lilToon 1.7.0対応
- Modular Avatar 1.12.5対応

修正完了日時: 2025-01-24
";
    }
} 
