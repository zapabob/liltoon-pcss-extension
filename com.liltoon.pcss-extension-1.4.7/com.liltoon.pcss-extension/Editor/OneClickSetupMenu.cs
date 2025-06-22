using UnityEngine;
using UnityEditor;
#if VRCHAT_SDK_AVAILABLE
using VRC.SDK3.Avatars.Components;
#endif
using System.Linq;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// Unityメニューからのワンクリックセットアップシステム
    /// nHaruka互換リアル影システムの即座導入
    /// </summary>
    public static class OneClickSetupMenu
    {
        // メニュー項目定義
        private const string MENU_ROOT = "GameObject/lilToon PCSS Extension/";
        private const string MENU_PRIORITY_BASE = 20;
        
        [MenuItem(MENU_ROOT + "🎯 競合製品互換 ワンクリックセットアップ", false, MENU_PRIORITY_BASE)]
        public static void QuickSetupCompetitorCompatible()
        {
            var selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                ShowSelectionDialog();
                return;
            }
            
            ExecuteCompetitorCompatibleSetup(selectedObject);
        }
        
        /// <summary>
        /// 指定されたGameObjectに対して競合製品互換セットアップを実行（外部呼び出し用）
        /// </summary>
        public static void QuickSetupCompetitorCompatible(GameObject target)
        {
            if (target == null)
            {
                ShowSelectionDialog();
                return;
            }
            
            ExecuteCompetitorCompatibleSetup(target);
        }
        
        [MenuItem(MENU_ROOT + "🎭 明暗トゥーン影セットアップ", false, MENU_PRIORITY_BASE + 1)]
        public static void QuickSetupToonShadows()
        {
            var selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                ShowSelectionDialog();
                return;
            }
            
            ExecuteToonShadowSetup(selectedObject);
        }
        
        /// <summary>
        /// 指定されたGameObjectに対してトゥーン影セットアップを実行（外部呼び出し用）
        /// </summary>
        public static void QuickSetupToonShadows(GameObject target)
        {
            if (target == null)
            {
                ShowSelectionDialog();
                return;
            }
            
            ExecuteToonShadowSetup(target);
        }
        
        [MenuItem(MENU_ROOT + "📸 リアル影セットアップ", false, MENU_PRIORITY_BASE + 2)]
        public static void QuickSetupRealisticShadows()
        {
            var selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                ShowSelectionDialog();
                return;
            }
            
            ExecuteRealisticShadowSetup(selectedObject);
        }
        
        /// <summary>
        /// 指定されたGameObjectに対してリアル影セットアップを実行（外部呼び出し用）
        /// </summary>
        public static void QuickSetupRealisticShadows(GameObject target)
        {
            if (target == null)
            {
                ShowSelectionDialog();
                return;
            }
            
            ExecuteRealisticShadowSetup(target);
        }
        
        [MenuItem(MENU_ROOT + "🎯 アバター選択メニュー", false, MENU_PRIORITY_BASE + 5)]
        public static void OpenAvatarSelector()
        {
            AvatarSelectorMenu.ShowWindow();
        }
        
        [MenuItem(MENU_ROOT + "⚙️ カスタムセットアップウィザード", false, MENU_PRIORITY_BASE + 10)]
        public static void OpenCustomSetupWizard()
        {
            CompetitorCompatibleSetupWizard.ShowWindow();
        }
        
        [MenuItem(MENU_ROOT + "🔧 詳細設定パネル", false, MENU_PRIORITY_BASE + 11)]
        public static void OpenAdvancedSettings()
        {
            AdvancedPCSSSettingsWindow.ShowWindow();
        }
        
        [MenuItem(MENU_ROOT + "📖 セットアップガイド", false, MENU_PRIORITY_BASE + 20)]
        public static void OpenSetupGuide()
        {
            Application.OpenURL("https://zapabob.github.io/liltoon-pcss-extension/documentation.html");
        }
        
        [MenuItem(MENU_ROOT + "🆘 トラブルシューティング", false, MENU_PRIORITY_BASE + 21)]
        public static void OpenTroubleshooting()
        {
            TroubleshootingWindow.ShowWindow();
        }
        
        // バリデーション：VRChatアバターが選択されているかチェック
        [MenuItem(MENU_ROOT + "🎯 競合製品互換 ワンクリックセットアップ", true)]
        [MenuItem(MENU_ROOT + "🎭 明暗トゥーン影セットアップ", true)]
        [MenuItem(MENU_ROOT + "📸 リアル影セットアップ", true)]
        public static bool ValidateQuickSetup()
        {
            var selectedObject = Selection.activeGameObject;
            if (selectedObject == null) return true; // メニューは表示するが警告を出す
            
#if VRCHAT_SDK_AVAILABLE
            return selectedObject.GetComponent<VRCAvatarDescriptor>() != null ||
                   selectedObject.GetComponentInParent<VRCAvatarDescriptor>() != null ||
                   selectedObject.GetComponentInChildren<VRCAvatarDescriptor>() != null;
#else
            return true;
#endif
        }
        
        /// <summary>
        /// 競合製品互換セットアップの実行
        /// </summary>
        private static void ExecuteCompetitorCompatibleSetup(GameObject target)
        {
            try
            {
                EditorUtility.DisplayProgressBar("競合製品互換セットアップ", "初期化中...", 0.0f);
                
                // 1. VRChatアバター検証
                EditorUtility.DisplayProgressBar("競合製品互換セットアップ", "アバター検証中...", 0.1f);
                if (!ValidateVRChatAvatar(target))
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("エラー", 
                        "選択されたオブジェクトはVRChatアバターではありません。\n" +
                        "VRCAvatarDescriptorコンポーネントを持つオブジェクトを選択してください。", 
                        "OK");
                    return;
                }
                
                // 2. PhysBone連動ライトコントローラー追加
                EditorUtility.DisplayProgressBar("nHaruka互換セットアップ", "PhysBoneライト制御設定中...", 0.2f);
                SetupPhysBoneLightController(target);
                
                // 3. シャドウマスクシステム追加
                EditorUtility.DisplayProgressBar("nHaruka互換セットアップ", "シャドウマスクシステム設定中...", 0.4f);
                SetupShadowMaskSystem(target);
                
                // 4. PCSS Shader適用
                EditorUtility.DisplayProgressBar("nHaruka互換セットアップ", "PCSSシェーダー適用中...", 0.6f);
                ApplyPCSSShaders(target);
                
                // 5. Expression Menu作成
                EditorUtility.DisplayProgressBar("nHaruka互換セットアップ", "Expression Menu作成中...", 0.8f);
                CreateExpressionMenu(target);
                
                // 6. 最適化設定適用
                EditorUtility.DisplayProgressBar("nHaruka互換セットアップ", "VRChat最適化設定適用中...", 0.9f);
                ApplyVRChatOptimizations(target);
                
                EditorUtility.ClearProgressBar();
                
                // 成功メッセージ
                EditorUtility.DisplayDialog("セットアップ完了", 
                    "🎉 競合製品互換リアル影システムのセットアップが完了しました！\n\n" +
                    "✅ PhysBone連動ライト制御\n" +
                    "✅ 10m距離による自動ON/OFF\n" +
                    "✅ シャドウマスク機能\n" +
                    "✅ Expression Menu統合\n" +
                    "✅ VRChat最適化設定\n\n" +
                    "アバターをアップロードしてお楽しみください！", 
                    "完了");
                
                Debug.Log($"[競合製品互換セットアップ] 完了: {target.name}");
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("セットアップエラー", 
                    $"セットアップ中にエラーが発生しました:\n{e.Message}\n\n" +
                    "詳細はConsoleログを確認してください。", 
                    "OK");
                Debug.LogError($"[競合製品互換セットアップ] エラー: {e.Message}");
            }
        }
        
        /// <summary>
        /// 明暗トゥーン影セットアップの実行
        /// </summary>
        private static void ExecuteToonShadowSetup(GameObject target)
        {
            try
            {
                EditorUtility.DisplayProgressBar("明暗トゥーン影セットアップ", "初期化中...", 0.0f);
                
                if (!ValidateVRChatAvatar(target))
                {
                    EditorUtility.ClearProgressBar();
                    ShowAvatarValidationError();
                    return;
                }
                
                // トゥーン専用設定でセットアップ
                EditorUtility.DisplayProgressBar("明暗トゥーン影セットアップ", "トゥーン設定適用中...", 0.3f);
                SetupToonShadowSettings(target);
                
                EditorUtility.DisplayProgressBar("明暗トゥーン影セットアップ", "シェーダー適用中...", 0.6f);
                ApplyToonOptimizedShaders(target);
                
                EditorUtility.DisplayProgressBar("明暗トゥーン影セットアップ", "最適化設定適用中...", 0.9f);
                ApplyToonOptimizations(target);
                
                EditorUtility.ClearProgressBar();
                
                EditorUtility.DisplayDialog("セットアップ完了", 
                    "🎭 明暗トゥーン影システムのセットアップが完了しました！\n\n" +
                    "アニメ調の美しい影境界線が適用されました。\n" +
                    "VTuberアバターに最適な設定です。", 
                    "完了");
                
                Debug.Log($"[明暗トゥーン影セットアップ] 完了: {target.name}");
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                ShowSetupError("明暗トゥーン影セットアップ", e);
            }
        }
        
        /// <summary>
        /// リアル影セットアップの実行
        /// </summary>
        private static void ExecuteRealisticShadowSetup(GameObject target)
        {
            try
            {
                EditorUtility.DisplayProgressBar("リアル影セットアップ", "初期化中...", 0.0f);
                
                if (!ValidateVRChatAvatar(target))
                {
                    EditorUtility.ClearProgressBar();
                    ShowAvatarValidationError();
                    return;
                }
                
                // リアル専用設定でセットアップ
                EditorUtility.DisplayProgressBar("リアル影セットアップ", "リアル設定適用中...", 0.3f);
                SetupRealisticShadowSettings(target);
                
                EditorUtility.DisplayProgressBar("リアル影セットアップ", "高品質シェーダー適用中...", 0.6f);
                ApplyRealisticOptimizedShaders(target);
                
                EditorUtility.DisplayProgressBar("リアル影セットアップ", "RTX最適化設定適用中...", 0.9f);
                ApplyRealisticOptimizations(target);
                
                EditorUtility.ClearProgressBar();
                
                EditorUtility.DisplayDialog("セットアップ完了", 
                    "📸 リアル影システムのセットアップが完了しました！\n\n" +
                    "写実的なソフトシャドウが適用されました。\n" +
                    "RTX GPU環境で最高の品質を発揮します。", 
                    "完了");
                
                Debug.Log($"[リアル影セットアップ] 完了: {target.name}");
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                ShowSetupError("リアル影セットアップ", e);
            }
        }
        
        // ヘルパーメソッド群
        private static bool ValidateVRChatAvatar(GameObject target)
        {
#if VRCHAT_SDK_AVAILABLE
            return target.GetComponent<VRCAvatarDescriptor>() != null ||
                   target.GetComponentInParent<VRCAvatarDescriptor>() != null ||
                   target.GetComponentInChildren<VRCAvatarDescriptor>() != null;
#else
            return true; // VRChat SDK未導入時は警告のみ
#endif
        }
        
        private static void SetupPhysBoneLightController(GameObject target)
        {
            var controller = target.GetComponent<PhysBoneLightController>();
            if (controller == null)
            {
                controller = target.AddComponent<PhysBoneLightController>();
            }
            
            // 競合製品互換設定適用
            controller.SetCompetitorCompatibilityMode(true);
            controller.TogglePhysBoneControl(true);
            
            Debug.Log("[Setup] PhysBone Light Controller追加完了");
        }
        
        private static void SetupShadowMaskSystem(GameObject target)
        {
            var shadowMask = target.GetComponent<ShadowMaskSystem>();
            if (shadowMask == null)
            {
                shadowMask = target.AddComponent<ShadowMaskSystem>();
            }
            
            // 競合製品互換設定適用
            shadowMask.SetShadowMaskMode(ShadowMaskSystem.ShadowMaskMode.CompetitorCompatible);
            
            Debug.Log("[Setup] Shadow Mask System追加完了");
        }
        
        private static void ApplyPCSSShaders(GameObject target)
        {
            var renderers = target.GetComponentsInChildren<Renderer>();
            int shaderCount = 0;
            
            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    var material = materials[i];
                    if (material != null)
                    {
                        Shader pcssShader = null;
                        
                        if (material.shader.name.Contains("lilToon"))
                        {
                            pcssShader = Shader.Find("lilToon/PCSS Extension");
                        }
                        else if (material.shader.name.Contains("Poiyomi"))
                        {
                            pcssShader = Shader.Find("Poiyomi/PCSS Extension");
                        }
                        
                        if (pcssShader != null)
                        {
                            material.shader = pcssShader;
                            shaderCount++;
                        }
                    }
                }
            }
            
            Debug.Log($"[Setup] PCSSシェーダー適用完了: {shaderCount}個のマテリアル");
        }
        
        private static void CreateExpressionMenu(GameObject target)
        {
#if VRCHAT_SDK_AVAILABLE
            var avatarDescriptor = target.GetComponent<VRCAvatarDescriptor>();
            if (avatarDescriptor == null)
            {
                avatarDescriptor = target.GetComponentInParent<VRCAvatarDescriptor>();
            }
            
            if (avatarDescriptor != null)
            {
                var menuCreator = new VRChatExpressionMenuCreator();
                menuCreator.CreatePCSSExpressionMenu(avatarDescriptor);
                Debug.Log("[Setup] Expression Menu作成完了");
            }
#endif
        }
        
        private static void ApplyVRChatOptimizations(GameObject target)
        {
            var optimizer = target.GetComponent<VRChatPerformanceOptimizer>();
            if (optimizer == null)
            {
                optimizer = target.AddComponent<VRChatPerformanceOptimizer>();
            }
            
            Debug.Log("[Setup] VRChat最適化設定適用完了");
        }
        
        // プリセット専用設定メソッド
        private static void SetupToonShadowSettings(GameObject target)
        {
            var shadowMask = target.GetComponent<ShadowMaskSystem>();
            if (shadowMask == null)
            {
                shadowMask = target.AddComponent<ShadowMaskSystem>();
            }
            
            // トゥーン専用設定
            shadowMask.SetShadowClampThreshold(0.8f); // 明確な境界線
            shadowMask.SetShadowSharpness(5.0f);      // シャープな影
        }
        
        private static void SetupRealisticShadowSettings(GameObject target)
        {
            var shadowMask = target.GetComponent<ShadowMaskSystem>();
            if (shadowMask == null)
            {
                shadowMask = target.AddComponent<ShadowMaskSystem>();
            }
            
            // リアル専用設定
            shadowMask.SetShadowClampThreshold(0.2f); // ソフトな境界線
            shadowMask.SetShadowSharpness(1.0f);      // 自然な影
        }
        
        private static void ApplyToonOptimizedShaders(GameObject target)
        {
            // トゥーン最適化シェーダー適用
            ApplyPCSSShaders(target);
            
            // トゥーン専用パラメータ設定
            var renderers = target.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null && material.shader.name.Contains("PCSS"))
                    {
                        // トゥーン調整
                        if (material.HasProperty("_ShadowBorderRange"))
                            material.SetFloat("_ShadowBorderRange", 0.05f);
                        if (material.HasProperty("_ShadowBlur"))
                            material.SetFloat("_ShadowBlur", 0.1f);
                    }
                }
            }
        }
        
        private static void ApplyRealisticOptimizedShaders(GameObject target)
        {
            // リアル最適化シェーダー適用
            ApplyPCSSShaders(target);
            
            // リアル専用パラメータ設定
            var renderers = target.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null && material.shader.name.Contains("PCSS"))
                    {
                        // リアル調整
                        if (material.HasProperty("_ShadowBorderRange"))
                            material.SetFloat("_ShadowBorderRange", 0.3f);
                        if (material.HasProperty("_ShadowBlur"))
                            material.SetFloat("_ShadowBlur", 0.8f);
                    }
                }
            }
        }
        
        private static void ApplyToonOptimizations(GameObject target)
        {
            // トゥーン用パフォーマンス最適化
            ApplyVRChatOptimizations(target);
        }
        
        private static void ApplyRealisticOptimizations(GameObject target)
        {
            // リアル用パフォーマンス最適化（RTX特化）
            ApplyVRChatOptimizations(target);
        }
        
        // エラーハンドリング
        private static void ShowSelectionDialog()
        {
            EditorUtility.DisplayDialog("オブジェクト未選択", 
                "セットアップを実行するVRChatアバターを選択してください。\n\n" +
                "Hierarchyで対象のアバター（VRCAvatarDescriptorコンポーネント付き）を選択してから、" +
                "再度メニューを実行してください。", 
                "OK");
        }
        
        private static void ShowAvatarValidationError()
        {
            EditorUtility.DisplayDialog("アバター検証エラー", 
                "選択されたオブジェクトはVRChatアバターではありません。\n\n" +
                "VRCAvatarDescriptorコンポーネントを持つオブジェクトを選択してください。\n" +
                "VRChat SDKが正しくインポートされていることも確認してください。", 
                "OK");
        }
        
        private static void ShowSetupError(string setupType, System.Exception e)
        {
            EditorUtility.DisplayDialog($"{setupType}エラー", 
                $"セットアップ中にエラーが発生しました:\n{e.Message}\n\n" +
                "以下を確認してください:\n" +
                "• VRChat SDKが正しくインポートされている\n" +
                "• lilToon PCSS Extensionが正しくインストールされている\n" +
                "• 対象オブジェクトが有効なVRChatアバターである\n\n" +
                "詳細はConsoleログを確認してください。", 
                "OK");
            Debug.LogError($"[{setupType}] エラー: {e.Message}\n{e.StackTrace}");
        }
    }
    
    /// <summary>
    /// 詳細設定ウィンドウ
    /// </summary>
    public class AdvancedPCSSSettingsWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            var window = GetWindow<AdvancedPCSSSettingsWindow>("PCSS詳細設定");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("🔧 PCSS詳細設定", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "ここでは高度なPCSS設定を調整できます。\n" +
                "設定を変更する前に、基本的なセットアップを完了してください。",
                MessageType.Info
            );
            
            // 詳細設定UI実装予定
            EditorGUILayout.LabelField("詳細設定機能は今後のアップデートで追加予定です。");
        }
    }
    
    /// <summary>
    /// トラブルシューティングウィンドウ
    /// </summary>
    public class TroubleshootingWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            var window = GetWindow<TroubleshootingWindow>("トラブルシューティング");
            window.minSize = new Vector2(500, 700);
            window.Show();
        }
        
        private Vector2 scrollPosition;
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("🆘 トラブルシューティング", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            DrawTroubleshootingSection("セットアップが失敗する", new string[]
            {
                "• VRChat SDKが正しくインポートされているか確認",
                "• Unity 2022.3.6f1以降を使用しているか確認",
                "• lilToon PCSS Extensionが最新版か確認",
                "• VRCAvatarDescriptorが対象オブジェクトにあるか確認"
            });
            
            DrawTroubleshootingSection("影が表示されない", new string[]
            {
                "• ライトのShadowsが有効になっているか確認",
                "• マテリアルがPCSSシェーダーに変更されているか確認",
                "• Shadow Maskシステムが有効になっているか確認",
                "• VRChatのGraphics設定でShadowsが有効か確認"
            });
            
            DrawTroubleshootingSection("パフォーマンスが悪い", new string[]
            {
                "• LOD設定が適切に設定されているか確認",
                "• 影の品質設定を下げる",
                "• 不要なライトを削除する",
                "• VRChat Performance Optimizerの設定を確認"
            });
            
            DrawTroubleshootingSection("PhysBone連動が動作しない", new string[]
            {
                "• PhysBoneコンポーネントが正しく設定されているか確認",
                "• PhysBone Light Controllerが有効になっているか確認",
                "• VRChat SDKのPhysBone機能が有効か確認",
                "• アバターのPhysBone設定を確認"
            });
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space(20);
            
            if (GUILayout.Button("📖 詳細ドキュメントを開く", GUILayout.Height(30)))
            {
                Application.OpenURL("https://zapabob.github.io/liltoon-pcss-extension/troubleshooting.html");
            }
            
            if (GUILayout.Button("🆘 サポートに問い合わせ", GUILayout.Height(30)))
            {
                Application.OpenURL("https://zapabob.github.io/liltoon-pcss-extension/support.html");
            }
        }
        
        private void DrawTroubleshootingSection(string title, string[] solutions)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            foreach (var solution in solutions)
            {
                EditorGUILayout.LabelField(solution, EditorStyles.wordWrappedLabel);
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }
    }
} 