using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// メニューの整理とクリーンアップシステム
    /// 古いメニューを非表示化し、統合メニューシステムに移行
    /// </summary>
    public static class MenuCleanupSystem
    {
        #region 非表示化するメニューリスト
        
        private static readonly string[] HiddenMenus = new string[]
        {
            // 個別のプリセット適用メニュー（統合メニューに移行）
            "Tools/lilToon-PCSS-Extension/リアル風セット適用",
            "Tools/lilToon-PCSS-Extension/アニメ風セット適用", 
            "Tools/lilToon-PCSS-Extension/映画風セット適用",
            "Tools/lilToon-PCSS-Extension/ポートレート風セット適用",
            "Tools/lilToon-PCSS-Extension/ゲーム風セット適用",
            
            // 個別の管理メニュー（統合メニューに移行）
            "Tools/lilToon-PCSS-Extension/統合プリセット管理",
            "Tools/lilToon-PCSS-Extension/高度な統合プリセット管理",
            
            // 重複するメニュー
            "Tools/lilToon-PCSS-Extension/外部ライト管理",
            "Tools/lilToon-PCSS-Extension/ModularAvatarライト制御",
            "Tools/lilToon-PCSS-Extension/ライトトグルメニュー",
            "Tools/lilToon-PCSS-Extension/マテリアルアップグレーダー",
            "Tools/lilToon-PCSS-Extension/Material Backup/Restore",
            "Tools/lilToon-PCSS-Extension/マテリアル自動修復",
            "Tools/lilToon-PCSS-Extension/シェーダー自動変換",
            "Tools/lilToon-PCSS-Extension/パフォーマンス最適化",
            "Tools/lilToon-PCSS-Extension/パフォーマンス最適化メニュー",
            "Tools/lilToon-PCSS-Extension/シェーダーコンパイル最適化",
            "Tools/lilToon-PCSS-Extension/VRChat最適化設定",
            "Tools/lilToon-PCSS-Extension/VRChat式メニュー作成",
            "Tools/lilToon-PCSS-Extension/VRChatMaterial Backup/Restore",
            "Tools/lilToon-PCSS-Extension/VRChatアップロード自動バックアップ",
            "Tools/lilToon-PCSS-Extension/自動Material Backup/Restore",
            "Tools/lilToon-PCSS-Extension/セッション復元",
            "Tools/lilToon-PCSS-Extension/パッケージエクスポーター",
            "Tools/lilToon-PCSS-Extension/手動パッケージエクスポーター",
            "Tools/lilToon-PCSS-Extension/ワンクリックセットアップ",
            "Tools/lilToon-PCSS-Extension/競合機能実装",
            "Tools/lilToon-PCSS-Extension/競合セットアップウィザード",
            "Tools/lilToon-PCSS-Extension/アバター選択メニュー",
            "Tools/lilToon-PCSS-Extension/高度マスクシステム",
            "Tools/lilToon-PCSS-Extension/PhysBoneコライダー作成",
            "Tools/lilToon-PCSS-Extension/VRChatライトボリューム",
            "Tools/lilToon-PCSS-Extension/VRChatリムライトGUI",
            "Tools/lilToon-PCSS-Extension/エラーハンドラー",
            "Tools/lilToon-PCSS-Extension/コンパイルチェッカー",
            "Tools/lilToon-PCSS-Extension/メニュー名リファクターツール"
        };
        
        #endregion
        
        #region メニュー整理
        
        [MenuItem("Tools/lilToon-PCSS-Extension/メニュー整理/古いメニューを非表示化", false, 100)]
        public static void HideOldMenus()
        {
            var hiddenCount = 0;
            
            foreach (var menuPath in HiddenMenus)
            {
                if (MenuExists(menuPath))
                {
                    Menu.SetChecked(menuPath, false);
                    hiddenCount++;
                }
            }
            
            EditorUtility.DisplayDialog("メニュー整理", 
                $"{hiddenCount}個の古いメニューを非表示化しました。\n統合メニューシステムをご利用ください。", "OK");
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/メニュー整理/統合メニューを推奨", false, 101)]
        public static void ShowUnifiedMenuRecommendation()
        {
            EditorUtility.DisplayDialog("統合メニューシステム推奨",
                "🔧 新しい統合メニューシステムが利用可能です！\n\n" +
                "✨ 機能:\n" +
                "• 検索機能付き\n" +
                "• カテゴリ別整理\n" +
                "• お気に入り機能\n" +
                "• 最近使用履歴\n" +
                "• ショートカットキー\n\n" +
                "📝 アクセス方法:\n" +
                "• Tools → lilToon PCSS → 統合メニューシステム\n" +
                "• ショートカット: Ctrl+Shift+M\n\n" +
                "古いメニューは段階的に非表示化されます。", "OK");
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/メニュー整理/メニュー状態を確認", false, 102)]
        public static void CheckMenuStatus()
        {
            var report = new System.Text.StringBuilder();
            report.AppendLine("=== メニュー状態レポート ===");
            report.AppendLine();
            
            var activeMenus = 0;
            var hiddenMenus = 0;
            
            foreach (var menuPath in HiddenMenus)
            {
                if (MenuExists(menuPath))
                {
                    if (Menu.GetChecked(menuPath))
                    {
                        activeMenus++;
                        report.AppendLine($"✅ アクティブ: {menuPath}");
                    }
                    else
                    {
                        hiddenMenus++;
                        report.AppendLine($"❌ 非表示: {menuPath}");
                    }
                }
                else
                {
                    report.AppendLine($"⚠️  存在しない: {menuPath}");
                }
            }
            
            report.AppendLine();
            report.AppendLine($"総数: {HiddenMenus.Length}");
            report.AppendLine($"アクティブ: {activeMenus}");
            report.AppendLine($"非表示: {hiddenMenus}");
            
            EditorUtility.DisplayDialog("メニュー状態", report.ToString(), "OK");
        }
        
        #endregion
        
        #region メニュー存在確認
        
        private static bool MenuExists(string menuPath)
        {
            try
            {
                return Menu.GetEnabled(menuPath);
            }
            catch
            {
                return false;
            }
        }
        
        #endregion
        
        #region 自動メニュー整理
        
        [InitializeOnLoadMethod]
        private static void InitializeMenuCleanup()
        {
            // 初回起動時に統合メニューシステムを推奨
            if (!EditorPrefs.GetBool("UnifiedMenu_RecommendationShown", false))
            {
                EditorApplication.delayCall += () =>
                {
                    if (EditorUtility.DisplayDialog("統合メニューシステム",
                        "新しい統合メニューシステムが利用可能です！\n\n" +
                        "機能:\n" +
                        "• 検索機能付き\n" +
                        "• カテゴリ別整理\n" +
                        "• お気に入り機能\n" +
                        "• ショートカットキー\n\n" +
                        "ショートカット: Ctrl+Shift+M\n\n" +
                        "詳細を表示しますか？", "詳細を見る", "後で"))
                    {
                        ShowUnifiedMenuRecommendation();
                    }
                    
                    EditorPrefs.SetBool("UnifiedMenu_RecommendationShown", true);
                };
            }
        }
        
        #endregion
    }
    
    /// <summary>
    /// メニュー移行アシスタント
    /// </summary>
    public class MenuMigrationAssistant : EditorWindow
    {
        [MenuItem("Tools/lilToon-PCSS-Extension/メニュー移行アシスタント", false, 103)]
        public static void ShowWindow()
        {
            var window = GetWindow<MenuMigrationAssistant>("メニュー移行アシスタント");
            window.minSize = new Vector2(500, 400);
            window.Show();
        }
        
        private Vector2 scrollPosition;
        private bool showMigrationGuide = true;
        private bool showShortcutGuide = true;
        private bool showCategoryGuide = true;
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("メニュー移行アシスタント", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            DrawMigrationGuide();
            DrawShortcutGuide();
            DrawCategoryGuide();
            DrawQuickActions();
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawMigrationGuide()
        {
            showMigrationGuide = EditorGUILayout.Foldout(showMigrationGuide, "📋 移行ガイド");
            if (showMigrationGuide)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.LabelField("古いメニュー → 新しい統合メニュー", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                
                DrawMigrationItem("リアル風セット適用", "プリセット → リアル風セット適用");
                DrawMigrationItem("アニメ風セット適用", "プリセット → アニメ風セット適用");
                DrawMigrationItem("映画風セット適用", "プリセット → 映画風セット適用");
                DrawMigrationItem("外部ライト管理", "ライト → 外部ライト管理");
                DrawMigrationItem("マテリアルアップグレーダー", "マテリアル → マテリアルアップグレーダー");
                DrawMigrationItem("パフォーマンス最適化", "パフォーマンス → パフォーマンス最適化");
                DrawMigrationItem("VRChat最適化設定", "VRChat → VRChat最適化設定");
                DrawMigrationItem("バックアップ管理", "バックアップ → 自動Material Backup/Restore");
                DrawMigrationItem("エクスポート", "エクスポート → パッケージエクスポーター");
                
                EditorGUILayout.EndVertical();
            }
        }
        
        private void DrawShortcutGuide()
        {
            showShortcutGuide = EditorGUILayout.Foldout(showShortcutGuide, "⌨️ ショートカットガイド");
            if (showShortcutGuide)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.LabelField("主要ショートカット", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                
                DrawShortcutItem("Ctrl+Shift+M", "統合メニューシステム");
                DrawShortcutItem("Ctrl+Shift+P", "プリセット管理");
                DrawShortcutItem("Ctrl+Shift+L", "ライト管理");
                DrawShortcutItem("Ctrl+Shift+U", "マテリアルアップグレード");
                DrawShortcutItem("Ctrl+Shift+O", "パフォーマンス最適化");
                DrawShortcutItem("Ctrl+Shift+V", "VRChat最適化");
                DrawShortcutItem("Ctrl+Shift+B", "バックアップ管理");
                DrawShortcutItem("Ctrl+Shift+E", "エクスポート");
                
                EditorGUILayout.EndVertical();
            }
        }
        
        private void DrawCategoryGuide()
        {
            showCategoryGuide = EditorGUILayout.Foldout(showCategoryGuide, "📁 カテゴリガイド");
            if (showCategoryGuide)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.LabelField("機能別カテゴリ", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                
                DrawCategoryItem("プリセット", "PCSSプリセットの管理と適用");
                DrawCategoryItem("ライト", "外部ライトの設定と管理");
                DrawCategoryItem("マテリアル", "マテリアルの変換と最適化");
                DrawCategoryItem("パフォーマンス", "パフォーマンスの最適化と監視");
                DrawCategoryItem("VRChat", "VRChat用の最適化と設定");
                DrawCategoryItem("バックアップ", "データのバックアップと復元");
                DrawCategoryItem("エクスポート", "パッケージのエクスポート機能");
                DrawCategoryItem("高度設定", "高度な設定とカスタマイズ");
                
                EditorGUILayout.EndVertical();
            }
        }
        
        private void DrawQuickActions()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("🚀 クイックアクション", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            if (GUILayout.Button("統合メニューシステムを開く"))
            {
                UnifiedMenuSystem.ShowWindow();
            }
            
            if (GUILayout.Button("ショートカットヘルプを開く"))
            {
                ShortcutHelpWindow.ShowWindow();
            }
            
            if (GUILayout.Button("メニュー状態を確認"))
            {
                MenuCleanupSystem.CheckMenuStatus();
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawMigrationItem(string oldMenu, string newLocation)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"❌ {oldMenu}", EditorStyles.miniLabel, GUILayout.Width(200));
            GUILayout.Label("→", EditorStyles.miniLabel, GUILayout.Width(20));
            GUILayout.Label($"✅ {newLocation}", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawShortcutItem(string shortcut, string description)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(shortcut, EditorStyles.boldLabel, GUILayout.Width(120));
            GUILayout.Label(description, EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawCategoryItem(string category, string description)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"📁 {category}", EditorStyles.boldLabel, GUILayout.Width(100));
            GUILayout.Label(description, EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
        }
    }
}
