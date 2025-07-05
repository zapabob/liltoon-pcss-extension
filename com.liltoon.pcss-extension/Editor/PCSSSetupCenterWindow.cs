using UnityEditor;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// lilToon PCSS Extension 統合セットアップセンター
    /// </summary>
    public class PCSSSetupCenterWindow : EditorWindow
    {
        private int selectedTab = 0;
        private readonly string[] tabs =
        {
            "一括セットアップ",
            "プリセット適用",
            "競合ウィザード",
            "最適化",
            "マテリアル修復",
            "LightVolumes",
            "ドキュメント"
        };

        [MenuItem("lilToon/PCSS Extension/セットアップセンター", priority = 0)]
        public static void ShowWindow()
        {
            var window = GetWindow<PCSSSetupCenterWindow>(false, "PCSS Setup Center", true);
            window.minSize = new Vector2(520, 600);
            window.Show();
        }

        private void OnGUI()
        {
            selectedTab = GUILayout.Toolbar(selectedTab, tabs, EditorStyles.toolbarButton);
            GUILayout.Space(10);
            switch (selectedTab)
            {
                case 0: DrawBatchSetupTab(); break;
                case 1: DrawPresetTab(); break;
                case 2: DrawCompetitorWizardTab(); break;
                case 3: DrawOptimizationTab(); break;
                case 4: DrawMaterialFixTab(); break;
                case 5: DrawLightVolumesTab(); break;
                case 6: DrawDocsTab(); break;
            }
        }

        // 各タブのUIは既存ウィンドウのOnGUI/Drawメソッドを移植・再利用する想定
        private void DrawBatchSetupTab()
        {
            GUILayout.Label("🟢 一括セットアップ", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("シーン内のアバター/ワールドにPCSS影・仮想ライト・PhysBone連動・最適化等を一括適用します。", MessageType.Info);
            if (GUILayout.Button("PCSSバッチセットアップウィンドウを開く", GUILayout.Height(30)))
            {
                PCSSBatchSetupWindow.ShowWindow();
            }
        }
        private void DrawPresetTab()
        {
            GUILayout.Label("🎨 プリセット適用", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("アバターごとにPCSS影プリセットを適用できます。", MessageType.Info);
            if (GUILayout.Button("アバターセレクタを開く", GUILayout.Height(30)))
            {
                AvatarSelectorMenu.ShowWindow();
            }
        }
        private void DrawCompetitorWizardTab()
        {
            GUILayout.Label("🏆 競合互換ウィザード", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("競合製品のセットアップを模倣し、ワンクリックで高品質な影設定を適用します。", MessageType.Info);
            if (GUILayout.Button("競合セットアップウィザードを開く", GUILayout.Height(30)))
            {
                CompetitorSetupWizard.ShowWindow();
            }
        }
        private void DrawOptimizationTab()
        {
            GUILayout.Label("⚡ 最適化", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("VRChat向けのパフォーマンス最適化・品質設定を管理します。", MessageType.Info);
            if (GUILayout.Button("最適化設定ウィンドウを開く", GUILayout.Height(30)))
            {
                VRChatOptimizationSettings.ShowWindow();
            }
        }
        private void DrawMaterialFixTab()
        {
            GUILayout.Label("🛠️ マテリアル修復・アップグレード", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("マテリアルの自動修復・アップグレード・バックアップ/リストアを行います。", MessageType.Info);
            if (GUILayout.Button("マテリアルバックアップウィンドウを開く", GUILayout.Height(30)))
            {
                MaterialBackup.ShowWindow();
            }
            if (GUILayout.Button("Missing Material AutoFixerを開く", GUILayout.Height(30)))
            {
                MissingMaterialAutoFixer.ShowWindow();
            }
        }
        private void DrawLightVolumesTab()
        {
            GUILayout.Label("💡 VRC Light Volumes統合", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("VRC Light VolumesとPCSS Extensionの統合設定を行います。", MessageType.Info);
            if (GUILayout.Button("Light Volumesエディタを開く", GUILayout.Height(30)))
            {
                // VRCLightVolumesEditorはCustomEditorなのでInspectorで選択を促す
                EditorUtility.DisplayDialog("ご案内", "Light Volumes統合はInspectorから設定してください。\n\nシーン内のVRCLightVolumesIntegrationを選択すると専用エディタが表示されます。", "OK");
            }
        }
        private void DrawDocsTab()
        {
            GUILayout.Label("📄 ドキュメント・サポート", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("README・実装ログ・サポート情報・バージョン情報をまとめて確認できます。", MessageType.Info);
            if (GUILayout.Button("READMEを開く", GUILayout.Height(30)))
            {
                LilToonPCSSExtensionInitializer.OpenDocumentation();
            }
            if (GUILayout.Button("実装ログディレクトリを開く", GUILayout.Height(30)))
            {
                EditorUtility.RevealInFinder("_docs");
            }
            GUILayout.Space(10);
            GUILayout.Label("lilToon PCSS Extension v1.5.x © 2025 lilxyzw", EditorStyles.centeredGreyMiniLabel);
        }
    }
} 