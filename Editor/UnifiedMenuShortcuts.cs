using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// 統合メニューシステムのショートカットキー機能
    /// </summary>
    public static class UnifiedMenuShortcuts
    {
        #region ショートカットキー定義
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/統合メニューシステム (Shortcut) %#m", false, 0)]
        public static void OpenUnifiedMenu()
        {
            UnifiedMenuSystem.ShowWindow();
        }
        
        #if PCSS_DEV
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/プリセット管理 %#p", false, 10)]
        public static void OpenPresetManager()
        {
            PCSSMenuSystem.ShowWindow();
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/高度プリセット管理 %#h", false, 11)]
        public static void OpenAdvancedPresetManager()
        {
            AdvancedPCSSMenuSystem.ShowWindow();
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/ライト管理 %#l", false, 20)]
        public static void OpenLightManager()
        {
            ExternalLightManager.ShowWindow();
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/マテリアルアップグレード %#u", false, 30)]
        public static void OpenMaterialUpgrader()
        {
            LilToonPCSSMaterialUpgrader.UpgradeMaterials();
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/パフォーマンス最適化 %#o", false, 40)]
        public static void OpenPerformanceOptimizer()
        {
            PerformanceOptimizer.ShowWindow();
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/VRChat最適化 %#v", false, 50)]
        public static void OpenVRChatOptimization()
        {
            VRChatOptimizationSettings.ShowWindow();
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/バックアップ管理 %#b", false, 60)]
        public static void OpenBackupManager()
        {
            MaterialBackup.ShowWindow();
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/エクスポート %#e", false, 70)]
        public static void OpenExporter()
        {
            PackageExporter.ExportPackage();
        }
        #endif
        
        #endregion
        
        #region クイックアクセスメニュー
        
        #if PCSS_DEV
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/クイックアクセス/リアル風セット適用", false, 100)]
        public static void QuickApplyRealisticSet()
        {
            PCSSMenuSystem.ApplyRealisticSet();
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/クイックアクセス/アニメ風セット適用", false, 101)]
        public static void QuickApplyAnimeSet()
        {
            PCSSMenuSystem.ApplyAnimeSet();
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/クイックアクセス/映画風セット適用", false, 102)]
        public static void QuickApplyCinematicSet()
        {
            PCSSMenuSystem.ApplyCinematicSet();
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/クイックアクセス/ポートレート風セット適用", false, 103)]
        public static void QuickApplyPortraitSet()
        {
            PCSSMenuSystem.ApplyPortraitSet();
        }
        
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/クイックアクセス/ゲーム風セット適用", false, 104)]
        public static void QuickApplyGameSet()
        {
            PCSSMenuSystem.ApplyGameSet();
        }
        #endif
        
        #endregion
        
        #region コンテキストメニュー
        
        #if PCSS_DEV
        [MenuItem("GameObject/Legacy/lilToon PCSS/プリセット適用/リアル風", false, 10)]
        public static void ContextApplyRealisticSet()
        {
            if (Selection.gameObjects.Length > 0)
            {
                PCSSMenuSystem.ApplyRealisticSet();
            }
        }
        
        [MenuItem("GameObject/Legacy/lilToon PCSS/プリセット適用/アニメ風", false, 11)]
        public static void ContextApplyAnimeSet()
        {
            if (Selection.gameObjects.Length > 0)
            {
                PCSSMenuSystem.ApplyAnimeSet();
            }
        }
        
        [MenuItem("GameObject/Legacy/lilToon PCSS/プリセット適用/映画風", false, 12)]
        public static void ContextApplyCinematicSet()
        {
            if (Selection.gameObjects.Length > 0)
            {
                PCSSMenuSystem.ApplyCinematicSet();
            }
        }
        
        [MenuItem("GameObject/Legacy/lilToon PCSS/マテリアルアップグレード", false, 20)]
        public static void ContextMaterialUpgrade()
        {
            if (Selection.gameObjects.Length > 0)
            {
                LilToonPCSSMaterialUpgrader.UpgradeMaterials();
            }
        }
        
        [MenuItem("GameObject/Legacy/lilToon PCSS/パフォーマンス最適化", false, 21)]
        public static void ContextPerformanceOptimization()
        {
            if (Selection.gameObjects.Length > 0)
            {
                PerformanceOptimizer.ShowWindow();
            }
        }
        #endif
        
        #endregion
        
        #region アセットメニュー
        
        #if PCSS_DEV
        [MenuItem("Assets/Legacy/lilToon PCSS/マテリアルアップグレード", false, 10)]
        public static void AssetMaterialUpgrade()
        {
            var selectedAssets = Selection.GetFiltered<Object>(SelectionMode.Assets);
            if (selectedAssets.Length > 0)
            {
                LilToonPCSSMaterialUpgrader.UpgradeMaterials();
            }
        }
        
        [MenuItem("Assets/Legacy/lilToon PCSS/バックアップ作成", false, 11)]
        public static void AssetBackup()
        {
            var selectedAssets = Selection.GetFiltered<Object>(SelectionMode.Assets);
            if (selectedAssets.Length > 0)
            {
                MaterialBackup.ShowWindow();
            }
        }
        
        [MenuItem("Assets/Legacy/lilToon PCSS/エクスポート", false, 12)]
        public static void AssetExport()
        {
            var selectedAssets = Selection.GetFiltered<Object>(SelectionMode.Assets);
            if (selectedAssets.Length > 0)
            {
                PackageExporter.ExportPackage();
            }
        }
        #endif
        #endregion
        
        #region バリデーション
        
        [MenuItem("GameObject/Legacy/lilToon PCSS/プリセット適用/リアル風", true)]
        [MenuItem("GameObject/Legacy/lilToon PCSS/プリセット適用/アニメ風", true)]
        [MenuItem("GameObject/Legacy/lilToon PCSS/プリセット適用/映画風", true)]
        [MenuItem("GameObject/Legacy/lilToon PCSS/マテリアルアップグレード", true)]
        [MenuItem("GameObject/Legacy/lilToon PCSS/パフォーマンス最適化", true)]
        public static bool ValidateGameObjectMenu()
        {
            #if PCSS_DEV
            return Selection.gameObjects.Length > 0;
            #else
            return false;
            #endif
        }
        
        [MenuItem("Assets/Legacy/lilToon PCSS/マテリアルアップグレード", true)]
        [MenuItem("Assets/Legacy/lilToon PCSS/バックアップ作成", true)]
        [MenuItem("Assets/Legacy/lilToon PCSS/エクスポート", true)]
        public static bool ValidateAssetMenu()
        {
            #if PCSS_DEV
            return Selection.GetFiltered<Object>(SelectionMode.Assets).Length > 0;
            #else
            return false;
            #endif
        }
        
        #endregion
    }
    
    /// <summary>
    /// ショートカットキーの説明ウィンドウ
    /// </summary>
    public class ShortcutHelpWindow : EditorWindow
    {
        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/ショートカットヘルプ", false, 3)]
        public static void ShowWindow()
        {
            var window = GetWindow<ShortcutHelpWindow>("ショートカットヘルプ");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("lilToon PCSS ショートカットキー", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("メイン機能", EditorStyles.boldLabel);
            DrawShortcut("Ctrl+Shift+M", "統合メニューシステムを開く");
            DrawShortcut("Ctrl+Shift+P", "プリセット管理を開く");
            DrawShortcut("Ctrl+Shift+H", "高度プリセット管理を開く");
            DrawShortcut("Ctrl+Shift+L", "ライト管理を開く");
            DrawShortcut("Ctrl+Shift+U", "マテリアルアップグレードを開く");
            DrawShortcut("Ctrl+Shift+O", "パフォーマンス最適化を開く");
            DrawShortcut("Ctrl+Shift+V", "VRChat最適化を開く");
            DrawShortcut("Ctrl+Shift+B", "バックアップ管理を開く");
            DrawShortcut("Ctrl+Shift+E", "エクスポートを開く");
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("コンテキストメニュー", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("GameObjectを右クリック → lilToon PCSS", EditorStyles.miniLabel);
            EditorGUILayout.LabelField("Assetsを右クリック → lilToon PCSS", EditorStyles.miniLabel);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("クイックアクセス", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Tools → lilToon PCSS → クイックアクセス", EditorStyles.miniLabel);
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("💡 ヒント: 統合メニューシステムからも全ての機能にアクセスできます", MessageType.Info);
        }
        
        private void DrawShortcut(string shortcut, string description)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(shortcut, EditorStyles.boldLabel, GUILayout.Width(120));
            GUILayout.Label(description);
            EditorGUILayout.EndHorizontal();
        }
    }
}
