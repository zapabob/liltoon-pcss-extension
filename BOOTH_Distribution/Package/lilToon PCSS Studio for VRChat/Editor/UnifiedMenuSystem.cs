using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// lilToon PCSS Extension 統合メニューシステム（製品版）
    /// シンプルでわかりやすく、高機能なメニューシステム
    /// </summary>
    public class UnifiedMenuSystem : EditorWindow
    {
        #region データ構造
        
        [System.Serializable]
        public class UnifiedMenuItemData
        {
            public string name;
            public string description;
            public string category;
            public string iconPath;
            public System.Action action;
            public bool isFavorite;
            public bool isEnabled = true;
            public string[] tags;
            public int priority;
            public bool requiresSelection;
            public string shortcut;
        }

        [System.Serializable]
        public class Category
        {
            public string name;
            public string displayName;
            public string description;
            public string iconPath;
            public Color categoryColor;
            public bool isExpanded = true;
            public int priority;
        }

        #endregion

        #region フィールド

        private List<UnifiedMenuItemData> allMenuItems = new List<UnifiedMenuItemData>();
        private List<Category> categories = new List<Category>();
        private List<UnifiedMenuItemData> favoriteItems = new List<UnifiedMenuItemData>();
        private List<UnifiedMenuItemData> recentItems = new List<UnifiedMenuItemData>();

        private string searchQuery = "";
        private string selectedCategory = "すべて";
        private int selectedTab = 0;
        private string[] tabNames = { "すべて", "お気に入り", "最近使用", "カテゴリ別" };

        private Vector2 scrollPosition;
        private bool showSearchBar = true;
        private bool showCategoryFilter = true;
        private bool showFavorites = true;
        private bool showRecent = true;

        private GUIStyle categoryHeaderStyle;
        private GUIStyle menuItemStyle;
        private GUIStyle searchBoxStyle;
        private GUIStyle favoriteButtonStyle;

        private float lastSearchUpdate = 0f;
        private const float SEARCH_UPDATE_INTERVAL = 0.3f;

        #endregion

        #region メニューアイテム

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/統合メニューシステム", false, 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<UnifiedMenuSystem>("lilToon PCSS 統合メニュー");
            window.minSize = new Vector2(500, 600);
            window.Show();
        }

        #endregion

        #region 初期化

        private void OnEnable()
        {
            InitializeCategories();
            InitializeMenuItems();
            LoadUserPreferences();
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            SaveUserPreferences();
            EditorApplication.update -= OnEditorUpdate;
        }

        private void InitializeCategories()
        {
            categories.Clear();
            categories.Add(new Category { name = "すべて", displayName = "すべて", description = "全ての機能", priority = 0 });
            categories.Add(new Category { name = "プリセット", displayName = "🎨 プリセット", description = "プリセット管理機能", categoryColor = new Color(0.2f, 0.6f, 1f), priority = 1 });
            categories.Add(new Category { name = "マテリアル", displayName = "🔧 マテリアル", description = "マテリアル関連機能", categoryColor = new Color(0.8f, 0.4f, 0.2f), priority = 2 });
            categories.Add(new Category { name = "ライト", displayName = "💡 ライト", description = "ライト関連機能", categoryColor = new Color(1f, 0.8f, 0.2f), priority = 3 });
            categories.Add(new Category { name = "パフォーマンス", displayName = "⚡ パフォーマンス", description = "パフォーマンス最適化", categoryColor = new Color(0.2f, 0.8f, 0.4f), priority = 4 });
            categories.Add(new Category { name = "バックアップ", displayName = "💾 バックアップ", description = "バックアップ機能", categoryColor = new Color(0.6f, 0.2f, 0.8f), priority = 5 });
            categories.Add(new Category { name = "エクスポート", displayName = "📦 エクスポート", description = "エクスポート機能", categoryColor = new Color(0.8f, 0.2f, 0.6f), priority = 6 });
        }

        private void InitializeMenuItems()
        {
            allMenuItems.Clear();

            // プリセット関連
            AddMenuItem("リアル風プリセット", "リアル風のPCSS設定を適用", "プリセット", 
                () => PCSSMenuSystem.ApplyRealisticSet(), tags: new[] { "プリセット", "リアル", "高品質" });
            AddMenuItem("アニメ風プリセット", "アニメ風のPCSS設定を適用", "プリセット", 
                () => PCSSMenuSystem.ApplyAnimeSet(), tags: new[] { "プリセット", "アニメ", "スタイル" });
            AddMenuItem("映画風プリセット", "映画風のPCSS設定を適用", "プリセット", 
                () => PCSSMenuSystem.ApplyCinematicSet(), tags: new[] { "プリセット", "映画", "ドラマチック" });
            AddMenuItem("ポートレート風プリセット", "ポートレート風のPCSS設定を適用", "プリセット", 
                () => PCSSMenuSystem.ApplyPortraitSet(), tags: new[] { "プリセット", "ポートレート", "美しい" });
            AddMenuItem("ゲーム風プリセット", "ゲーム風のPCSS設定を適用", "プリセット", 
                () => PCSSMenuSystem.ApplyGameSet(), tags: new[] { "プリセット", "ゲーム", "軽量" });

            // マテリアル関連
            AddMenuItem("マテリアルアップグレード", "lilToonシェーダーをPCSS Extensionにアップグレード", "マテリアル", 
                () => LilToonPCSSMaterialUpgrader.UpgradeMaterials(), tags: new[] { "マテリアル", "アップグレード", "変換" });

            // ライト関連
            AddMenuItem("外部ライト自動配置", "アバター身長から最適な位置にライトを自動配置", "ライト", 
                () => AutoLightPlacement.AutoPlaceExternalLights(), tags: new[] { "ライト", "自動配置", "最適化" });
            AddMenuItem("Hip基準ライト配置 (MA)", "Modular AvatarのBone ProxyでHip追従アンカーに3点ライト", "ライト",
                () => HipBasedLightPlacement.CreateHipBasedLights(Selection.activeGameObject), tags: new[] { "ライト", "ModularAvatar", "見栄え" }, requiresSelection: true);
            AddMenuItem("ライト管理", "外部ライトの管理と設定", "ライト", 
                () => ExternalLightManager.ShowWindow(), tags: new[] { "ライト", "管理", "設定" });

            // パフォーマンス関連
            AddMenuItem("パフォーマンス最適化", "VRChat向けパフォーマンス最適化", "パフォーマンス", 
                () => PerformanceOptimizerMenu.SetupPerformanceTuner(), tags: new[] { "パフォーマンス", "最適化", "VRChat" });

            // バックアップ関連
            AddMenuItem("Material Backup/Restore", "マテリアルのバックアップと復元", "バックアップ", 
                () => MaterialBackup.ShowWindow(), tags: new[] { "バックアップ", "マテリアル", "復元" });

            // エクスポート関連
            AddMenuItem("パッケージエクスポート", "PCSS Extensionパッケージをエクスポート", "エクスポート", 
                () => PackageExporter.ExportPackage(), tags: new[] { "エクスポート", "パッケージ", "配布" });
            AddMenuItem("手動パッケージエクスポート", "手動でのパッケージエクスポート", "エクスポート", 
                () => lilToonPCSS.Editor.ManualPackageExporter.ShowWindow(), tags: new[] { "エクスポート", "手動", "パッケージ" });
        }

        private void AddMenuItem(string name, string description, string category, System.Action action, 
            string[] tags = null, int priority = 0, bool requiresSelection = false, string shortcut = "")
        {
            allMenuItems.Add(new UnifiedMenuItemData
            {
                name = name,
                description = description,
                category = category,
                action = action,
                tags = tags ?? new string[0],
                priority = priority,
                requiresSelection = requiresSelection,
                shortcut = shortcut
            });
        }

        #endregion

        #region GUI描画

        private void OnGUI()
        {
            InitializeStyles();
            
            EditorGUILayout.BeginVertical();
            
            DrawHeader();
            DrawSearchAndFilters();
            DrawTabs();
            DrawContent();
            
            EditorGUILayout.EndVertical();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            GUILayout.Label("🌟 lilToon PCSS 統合メニュー", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("設定", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                ShowSettings();
            }
            
            if (GUILayout.Button("ヘルプ", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                ShowHelp();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSearchAndFilters()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // 検索バー
            if (showSearchBar)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("🔍", GUILayout.Width(20));
                searchQuery = EditorGUILayout.TextField(searchQuery, searchBoxStyle);
                
                if (GUILayout.Button("クリア", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    searchQuery = "";
                }
                EditorGUILayout.EndHorizontal();
            }
            
            // カテゴリフィルター
            if (showCategoryFilter)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("カテゴリ:", GUILayout.Width(60));
                
                var categoryNames = categories.Select(c => c.displayName).ToArray();
                var currentIndex = categories.FindIndex(c => c.displayName == selectedCategory);
                var newIndex = EditorGUILayout.Popup(currentIndex, categoryNames);
                
                if (newIndex != currentIndex && newIndex >= 0)
                {
                    selectedCategory = categories[newIndex].displayName;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawTabs()
        {
            selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
        }

        private void DrawContent()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            switch (selectedTab)
            {
                case 0: DrawAllItems(); break;
                case 1: DrawFavoriteItems(); break;
                case 2: DrawRecentItems(); break;
                case 3: DrawCategoryItems(); break;
            }
            
            EditorGUILayout.EndScrollView();
        }

        private void DrawAllItems()
        {
            var filteredItems = GetFilteredItems();
            
            if (filteredItems.Count == 0)
            {
                EditorGUILayout.HelpBox("該当する機能が見つかりません。", MessageType.Info);
                return;
            }
            
            foreach (var item in filteredItems)
            {
                DrawMenuItem(item);
            }
        }

        private void DrawFavoriteItems()
        {
            if (favoriteItems.Count == 0)
            {
                EditorGUILayout.HelpBox("お気に入りに登録された機能がありません。", MessageType.Info);
                return;
            }
            
            foreach (var item in favoriteItems)
            {
                DrawMenuItem(item);
            }
        }

        private void DrawRecentItems()
        {
            if (recentItems.Count == 0)
            {
                EditorGUILayout.HelpBox("最近使用した機能がありません。", MessageType.Info);
                return;
            }
            
            foreach (var item in recentItems)
            {
                DrawMenuItem(item);
            }
        }

        private void DrawCategoryItems()
        {
            var category = categories.FirstOrDefault(c => c.displayName == selectedCategory);
            if (category == null) return;
            
            var categoryItems = allMenuItems.Where(item => item.category == category.name).ToList();
            
            if (categoryItems.Count == 0)
            {
                EditorGUILayout.HelpBox($"「{category.displayName}」カテゴリに機能がありません。", MessageType.Info);
                return;
            }
            
            DrawCategoryHeader(category);
            
            foreach (var item in categoryItems)
            {
                DrawMenuItem(item);
            }
        }

        private void DrawCategoryHeader(Category category)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label(category.displayName, categoryHeaderStyle);
            GUILayout.Label(category.description, EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawMenuItem(UnifiedMenuItemData item)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.BeginHorizontal();
            
            // お気に入りボタン
            bool isFavorite = favoriteItems.Contains(item);
            bool newFavoriteState = GUILayout.Toggle(isFavorite, "★", favoriteButtonStyle, GUILayout.Width(25));
            
            if (newFavoriteState != isFavorite)
            {
                if (newFavoriteState)
                {
                    if (!favoriteItems.Contains(item))
                        favoriteItems.Add(item);
                }
                else
                {
                    favoriteItems.Remove(item);
                }
            }
            
            // メイン情報
            EditorGUILayout.BeginVertical();
            GUILayout.Label(item.name, menuItemStyle);
            GUILayout.Label(item.description, EditorStyles.miniLabel);
            
            // タグ表示
            if (item.tags != null && item.tags.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                foreach (var tag in item.tags)
                {
                    GUILayout.Label($"#{tag}", EditorStyles.miniLabel, GUILayout.Width(50));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            
            // 実行ボタン
            if (GUILayout.Button("実行", EditorStyles.miniButton, GUILayout.Width(60)))
            {
                ExecuteMenuItem(item);
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region ユーティリティ

        private List<UnifiedMenuItemData> GetFilteredItems()
        {
            var items = allMenuItems.AsEnumerable();
            
            // 検索フィルター
            if (!string.IsNullOrEmpty(searchQuery))
            {
                items = items.Where(item => 
                    item.name.ToLower().Contains(searchQuery.ToLower()) ||
                    item.description.ToLower().Contains(searchQuery.ToLower()) ||
                    (item.tags != null && item.tags.Any(tag => tag.ToLower().Contains(searchQuery.ToLower())))
                );
            }
            
            // カテゴリフィルター
            if (selectedCategory != "すべて")
            {
                items = items.Where(item => item.category == selectedCategory);
            }
            
            return items.OrderBy(item => item.priority).ThenBy(item => item.name).ToList();
        }

        private void ExecuteMenuItem(UnifiedMenuItemData item)
        {
            try
            {
                if (item.requiresSelection && Selection.gameObjects.Length == 0)
                {
                    EditorUtility.DisplayDialog("エラー", "オブジェクトを選択してください。", "OK");
                    return;
                }
                
                item.action?.Invoke();
                AddToRecentItems(item);
                
                Debug.Log($"✅ {item.name} を実行しました。");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ {item.name} の実行中にエラーが発生しました: {e.Message}");
                EditorUtility.DisplayDialog("エラー", $"{item.name} の実行中にエラーが発生しました。\n{e.Message}", "OK");
            }
        }

        private void AddToRecentItems(UnifiedMenuItemData item)
        {
            recentItems.Remove(item);
            recentItems.Insert(0, item);
            
            if (recentItems.Count > 10)
            {
                recentItems.RemoveAt(recentItems.Count - 1);
            }
        }

        private void ShowSettings()
        {
            UnifiedMenuSettings.ShowWindow();
        }

        private void ShowHelp()
        {
            EditorUtility.DisplayDialog("ヘルプ", 
                "🌟 lilToon PCSS 統合メニューシステム\n\n" +
                "【使用方法】\n" +
                "• 検索バーで機能を検索\n" +
                "• カテゴリで機能を絞り込み\n" +
                "• お気に入りに登録して素早くアクセス\n" +
                "• 最近使用した機能を確認\n\n" +
                "【ショートカット】\n" +
                "• Ctrl+Shift+M: 統合メニューを開く\n" +
                "• 各機能のショートカットキーも利用可能\n\n" +
                "【製品版機能】\n" +
                "• プリセット管理（5種類）\n" +
                "• マテリアルアップグレード\n" +
                "• 外部ライト自動配置\n" +
                "• パフォーマンス最適化\n" +
                "• バックアップ管理\n" +
                "• エクスポート機能", "OK");
        }

        #endregion

        #region スタイル初期化

        private void InitializeStyles()
        {
            if (categoryHeaderStyle == null)
            {
                categoryHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
                categoryHeaderStyle.fontSize = 14;
                categoryHeaderStyle.normal.textColor = Color.white;
            }
            
            if (menuItemStyle == null)
            {
                menuItemStyle = new GUIStyle(EditorStyles.boldLabel);
                menuItemStyle.fontSize = 12;
            }
            
            if (searchBoxStyle == null)
            {
                searchBoxStyle = new GUIStyle(EditorStyles.textField);
                searchBoxStyle.fontSize = 12;
            }
            
            if (favoriteButtonStyle == null)
            {
                favoriteButtonStyle = new GUIStyle(EditorStyles.toggle);
                favoriteButtonStyle.fontSize = 10;
                favoriteButtonStyle.normal.textColor = Color.yellow;
            }
        }

        #endregion

        #region 更新処理

        private void OnEditorUpdate()
        {
            if (Time.realtimeSinceStartup - lastSearchUpdate > SEARCH_UPDATE_INTERVAL)
            {
                lastSearchUpdate = Time.realtimeSinceStartup;
                Repaint();
            }
        }

        #endregion

        #region 設定管理

        private void LoadUserPreferences()
        {
            showSearchBar = EditorPrefs.GetBool("UnifiedMenu_ShowSearchBar", true);
            showCategoryFilter = EditorPrefs.GetBool("UnifiedMenu_ShowCategoryFilter", true);
            showFavorites = EditorPrefs.GetBool("UnifiedMenu_ShowFavorites", true);
            showRecent = EditorPrefs.GetBool("UnifiedMenu_ShowRecent", true);
            
            // お気に入りアイテムの読み込み
            string favoritesJson = EditorPrefs.GetString("UnifiedMenu_Favorites", "[]");
            // 簡易的な実装のため、お気に入りは名前で管理
        }

        private void SaveUserPreferences()
        {
            EditorPrefs.SetBool("UnifiedMenu_ShowSearchBar", showSearchBar);
            EditorPrefs.SetBool("UnifiedMenu_ShowCategoryFilter", showCategoryFilter);
            EditorPrefs.SetBool("UnifiedMenu_ShowFavorites", showFavorites);
            EditorPrefs.SetBool("UnifiedMenu_ShowRecent", showRecent);
        }

        #endregion
    }

    /// <summary>
    /// 統合メニュー設定ウィンドウ
    /// </summary>
    public class UnifiedMenuSettings : EditorWindow
    {
        private bool showSearchBar = true;
        private bool showCategoryFilter = true;
        private bool showFavorites = true;
        private bool showRecent = true;

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/統合メニュー設定", false, 2)]
        public static void ShowWindow()
        {
            var window = GetWindow<UnifiedMenuSettings>("統合メニュー設定");
            window.minSize = new Vector2(300, 200);
            window.Show();
        }

        private void OnEnable()
        {
            LoadSettings();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("統合メニュー設定", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            showSearchBar = EditorGUILayout.Toggle("検索バーを表示", showSearchBar);
            showCategoryFilter = EditorGUILayout.Toggle("カテゴリフィルターを表示", showCategoryFilter);
            showFavorites = EditorGUILayout.Toggle("お気に入り機能を表示", showFavorites);
            showRecent = EditorGUILayout.Toggle("最近使用機能を表示", showRecent);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("設定を保存"))
            {
                SaveSettings();
            }
            
            if (GUILayout.Button("デフォルトに戻す"))
            {
                ResetToDefault();
            }
        }

        private void LoadSettings()
        {
            showSearchBar = EditorPrefs.GetBool("UnifiedMenu_ShowSearchBar", true);
            showCategoryFilter = EditorPrefs.GetBool("UnifiedMenu_ShowCategoryFilter", true);
            showFavorites = EditorPrefs.GetBool("UnifiedMenu_ShowFavorites", true);
            showRecent = EditorPrefs.GetBool("UnifiedMenu_ShowRecent", true);
        }

        private void SaveSettings()
        {
            EditorPrefs.SetBool("UnifiedMenu_ShowSearchBar", showSearchBar);
            EditorPrefs.SetBool("UnifiedMenu_ShowCategoryFilter", showCategoryFilter);
            EditorPrefs.SetBool("UnifiedMenu_ShowFavorites", showFavorites);
            EditorPrefs.SetBool("UnifiedMenu_ShowRecent", showRecent);
            
            EditorUtility.DisplayDialog("完了", "設定を保存しました。", "OK");
        }

        private void ResetToDefault()
        {
            showSearchBar = true;
            showCategoryFilter = true;
            showFavorites = true;
            showRecent = true;
            
            EditorUtility.DisplayDialog("完了", "設定をデフォルトに戻しました。", "OK");
        }
    }
}
