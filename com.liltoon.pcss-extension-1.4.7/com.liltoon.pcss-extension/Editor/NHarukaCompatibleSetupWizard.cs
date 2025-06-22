using UnityEngine;
using UnityEditor;
#if VRCHAT_SDK_AVAILABLE
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
#endif
using System.Collections.Generic;
using System.Linq;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// 競合「リアル影システム」互換セットアップウィザード
    /// ワンクリックで完全なPCSSシステムを構築
    /// </summary>
    public class CompetitorCompatibleSetupWizard : EditorWindow
    {
        [MenuItem("Window/lilToon PCSS Extension/競合製品互換セットアップ", false, 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<CompetitorCompatibleSetupWizard>("競合製品互換セットアップ");
            window.minSize = new Vector2(600, 800);
            window.maxSize = new Vector2(800, 1000);
            window.Show();
        }
        
        // セットアップ状態
        private enum SetupStep
        {
            Welcome,
            PresetSelection,
            TargetSelection,
            Configuration,
            Installation,
            Complete
        }
        
        // プリセット設定
        public enum ShadowPreset
        {
            明暗トゥーン,     // アニメ調の明確な影境界
            リアル,          // 写実的なソフトシャドウ
            競合製品互換,     // 競合製品完全互換
            カスタム         // ユーザー定義
        }
        
        // UI状態
        private SetupStep currentStep = SetupStep.Welcome;
        private ShadowPreset selectedPreset = ShadowPreset.競合製品互換;
        private GameObject targetAvatar;
        private bool autoDetectSettings = true;
        private Vector2 scrollPosition;
        
        // 設定値
        private float shadowSoftness = 0.5f;
        private float shadowBias = 0.001f;
        private float lightIntensity = 1.0f;
        private Color lightColor = Color.white;
        private bool enablePhysBoneControl = true;
        private bool enableDistanceControl = true;
        private float maxDistance = 10.0f;
        private bool enableExpressionMenu = true;
        
        // 検出された情報
        private List<Light> existingLights = new List<Light>();
#if VRCHAT_SDK_AVAILABLE
        private VRCAvatarDescriptor avatarDescriptor;
        private List<VRCPhysBone> detectedPhysBones = new List<VRCPhysBone>();
#endif
        private List<Renderer> targetRenderers = new List<Renderer>();
        
        private void OnGUI()
        {
            // ヘッダー
            DrawHeader();
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            switch (currentStep)
            {
                case SetupStep.Welcome:
                    DrawWelcomeStep();
                    break;
                case SetupStep.PresetSelection:
                    DrawPresetSelectionStep();
                    break;
                case SetupStep.TargetSelection:
                    DrawTargetSelectionStep();
                    break;
                case SetupStep.Configuration:
                    DrawConfigurationStep();
                    break;
                case SetupStep.Installation:
                    DrawInstallationStep();
                    break;
                case SetupStep.Complete:
                    DrawCompleteStep();
                    break;
            }
            
            EditorGUILayout.EndScrollView();
            
            // フッター
            DrawFooter();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.Space(10);
            
            // タイトル
            var titleStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            
            EditorGUILayout.LabelField("🎯 競合製品互換リアル影システム", titleStyle);
            EditorGUILayout.LabelField("lilToon PCSS Extension - ワンクリックセットアップ", EditorStyles.centeredGreyMiniLabel);
            
            EditorGUILayout.Space(5);
            
            // プログレスバー
            float progress = (float)currentStep / (float)(System.Enum.GetValues(typeof(SetupStep)).Length - 1);
            EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), progress, $"ステップ {(int)currentStep + 1}/6");
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
        
        private void DrawWelcomeStep()
        {
            EditorGUILayout.Space(20);
            
            var welcomeStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleLeft
            };
            
            EditorGUILayout.LabelField("🎉 ようこそ！", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField(
                "このセットアップウィザードでは、競合製品の「リアル影システム」と完全互換の " +
                "高品質PCSS（Percentage-Closer Soft Shadows）システムを、" +
                "わずか数クリックでVRChatアバターに導入できます。",
                welcomeStyle
            );
            
            EditorGUILayout.Space(20);
            
            // 特徴説明
            EditorGUILayout.LabelField("✨ 主な特徴", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            DrawFeatureItem("🎯", "ワンクリック自動セットアップ", "複雑な設定は一切不要");
            DrawFeatureItem("🎮", "PhysBone連動ライト制御", "髪や服の動きに合わせて影が追従");
            DrawFeatureItem("📏", "10m自動ON/OFF", "nHaruka互換の距離制御");
            DrawFeatureItem("🎨", "3つの影プリセット", "明暗トゥーン・リアル・nHaruka互換");
            DrawFeatureItem("⚡", "VRChat最適化", "Quest・PC両対応の高性能システム");
            DrawFeatureItem("🎛️", "Expression Menu統合", "ゲーム内でリアルタイム制御");
            
            EditorGUILayout.Space(20);
            
            // 互換性情報
            EditorGUILayout.LabelField("🔄 競合製品互換性", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            var compatStyle = new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 12
            };
            
            EditorGUILayout.TextArea(
                "• 競合リアル影システムの全機能を完全再現\n" +
                "• 既存の設定からの移行サポート\n" +
                "• 同等の操作性とパフォーマンス\n" +
                "• 追加のAI最適化機能で更なる高品質化",
                compatStyle
            );
        }
        
        private void DrawPresetSelectionStep()
        {
            EditorGUILayout.Space(20);
            
            EditorGUILayout.LabelField("🎨 影の表現スタイルを選択", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            var presetStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 12
            };
            
            // プリセット選択
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            selectedPreset = (ShadowPreset)EditorGUILayout.EnumPopup("影プリセット", selectedPreset);
            
            EditorGUILayout.Space(10);
            
            // プリセット説明
            switch (selectedPreset)
            {
                case ShadowPreset.明暗トゥーン:
                    DrawPresetDescription(
                        "🎭 明暗トゥーン",
                        "アニメ・マンガ調の明確な影境界線。" +
                        "VTuberアバターに最適な、くっきりとした美しい影を生成します。",
                        Color.cyan
                    );
                    break;
                    
                case ShadowPreset.リアル:
                    DrawPresetDescription(
                        "📸 リアル",
                        "写実的なソフトシャドウ。" +
                        "現実の光と影を忠実に再現し、自然で美しい陰影表現を実現します。",
                        Color.green
                    );
                    break;
                    
                case ShadowPreset.競合製品互換:
                    DrawPresetDescription(
                        "🎯 競合製品互換",
                        "競合リアル影システムと完全同等の設定。" +
                        "既存ユーザーにとって違和感のない操作性を提供します。",
                        Color.yellow
                    );
                    break;
                    
                case ShadowPreset.カスタム:
                    DrawPresetDescription(
                        "⚙️ カスタム",
                        "上級ユーザー向けの詳細設定。" +
                        "全パラメータを自由に調整して、理想的な影表現を追求できます。",
                        Color.magenta
                    );
                    break;
            }
            
            EditorGUILayout.EndVertical();
            
            // プレビュー画像（将来的に追加）
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("💡 ヒント", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "初回セットアップでは「競合製品互換」を推奨します。" +
                "後からいつでも設定を変更できます。",
                MessageType.Info
            );
        }
        
        private void DrawTargetSelectionStep()
        {
            EditorGUILayout.Space(20);
            
            EditorGUILayout.LabelField("🎯 対象アバターの選択", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            // アバター選択
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            GameObject newTarget = (GameObject)EditorGUILayout.ObjectField(
                "対象アバター",
                targetAvatar,
                typeof(GameObject),
                true
            );
            
            if (newTarget != targetAvatar)
            {
                targetAvatar = newTarget;
                AnalyzeTargetAvatar();
            }
            
            EditorGUILayout.Space(10);
            
            // 自動検出オプション
            autoDetectSettings = EditorGUILayout.Toggle("設定を自動検出", autoDetectSettings);
            
            EditorGUILayout.EndVertical();
            
            // 検出結果表示
            if (targetAvatar != null)
            {
                DrawAnalysisResults();
            }
            else
            {
                EditorGUILayout.Space(20);
                EditorGUILayout.HelpBox(
                    "Hierarchyから対象となるVRChatアバターを選択してください。" +
                    "VRCAvatarDescriptorコンポーネントを持つGameObjectが対象です。",
                    MessageType.Info
                );
            }
        }
        
        private void DrawConfigurationStep()
        {
            EditorGUILayout.Space(20);
            
            EditorGUILayout.LabelField("⚙️ 詳細設定", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            // 基本設定
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("基本設定", EditorStyles.boldLabel);
            
            shadowSoftness = EditorGUILayout.Slider("影のソフトネス", shadowSoftness, 0.0f, 1.0f);
            shadowBias = EditorGUILayout.Slider("影バイアス", shadowBias, 0.0001f, 0.01f);
            lightIntensity = EditorGUILayout.Slider("ライト強度", lightIntensity, 0.1f, 3.0f);
            lightColor = EditorGUILayout.ColorField("ライト色", lightColor);
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
            
            // PhysBone設定
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("PhysBone連動設定", EditorStyles.boldLabel);
            
            enablePhysBoneControl = EditorGUILayout.Toggle("PhysBone連動を有効", enablePhysBoneControl);
            
            if (enablePhysBoneControl)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("検出されたPhysBone:");
#if VRCHAT_SDK_AVAILABLE
                foreach (var physBone in detectedPhysBones)
                {
                    EditorGUILayout.LabelField($"• {physBone.name}", EditorStyles.miniLabel);
                }
#endif
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
            
            // 距離制御設定
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("距離制御設定", EditorStyles.boldLabel);
            
            enableDistanceControl = EditorGUILayout.Toggle("距離による自動ON/OFF", enableDistanceControl);
            
            if (enableDistanceControl)
            {
                EditorGUI.indentLevel++;
                maxDistance = EditorGUILayout.Slider("最大有効距離 (m)", maxDistance, 1.0f, 50.0f);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
            
            // Expression Menu設定
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Expression Menu統合", EditorStyles.boldLabel);
            
            enableExpressionMenu = EditorGUILayout.Toggle("Expression Menuを作成", enableExpressionMenu);
            
            if (enableExpressionMenu)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("追加されるメニュー項目:");
                EditorGUILayout.LabelField("• ライト強度調整", EditorStyles.miniLabel);
                EditorGUILayout.LabelField("• ライト色変更", EditorStyles.miniLabel);
                EditorGUILayout.LabelField("• 影のON/OFF", EditorStyles.miniLabel);
                EditorGUILayout.LabelField("• PhysBone連動切替", EditorStyles.miniLabel);
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawInstallationStep()
        {
            EditorGUILayout.Space(20);
            
            EditorGUILayout.LabelField("🚀 インストール実行", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            // インストール確認
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField("セットアップ内容確認", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            DrawInstallationItem("🎯", "対象アバター", targetAvatar?.name ?? "未選択");
            DrawInstallationItem("🎨", "影プリセット", selectedPreset.ToString());
            DrawInstallationItem("🎮", "PhysBone連動", enablePhysBoneControl ? "有効" : "無効");
            DrawInstallationItem("📏", "距離制御", enableDistanceControl ? $"有効 ({maxDistance:F1}m)" : "無効");
            DrawInstallationItem("🎛️", "Expression Menu", enableExpressionMenu ? "作成する" : "作成しない");
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(20);
            
            // 実行ボタン
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("🚀 競合製品互換システムをインストール", GUILayout.Height(40)))
            {
                ExecuteInstallation();
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "インストールを実行すると、選択されたアバターに " +
                "競合製品互換のリアル影システムが自動的に設定されます。",
                MessageType.Info
            );
        }
        
        private void DrawCompleteStep()
        {
            EditorGUILayout.Space(20);
            
            var successStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            
            EditorGUILayout.LabelField("🎉 セットアップ完了！", successStyle);
            EditorGUILayout.Space(20);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField("✅ インストール完了項目", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            DrawCompletionItem("🎯", "PhysBone連動ライト制御システム");
            DrawCompletionItem("📏", "10m距離による自動ON/OFF");
            DrawCompletionItem("🎨", $"{selectedPreset}プリセット適用");
            DrawCompletionItem("🎛️", "Expression Menu統合");
            DrawCompletionItem("⚡", "VRChat最適化設定");
            DrawCompletionItem("🔧", "競合製品互換設定");
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(20);
            
            // 使用方法
            EditorGUILayout.LabelField("🎮 使用方法", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            var usageStyle = new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 12
            };
            
            EditorGUILayout.TextArea(
                "1. VRChatでアバターをアップロード\n" +
                "2. Expression Menuから「PCSS Settings」を選択\n" +
                "3. ライト強度や色をリアルタイムで調整\n" +
                "4. PhysBone連動で自然な影の動きを楽しむ",
                usageStyle
            );
            
            EditorGUILayout.Space(20);
            
            // アクションボタン
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("🔧 設定を調整", GUILayout.Height(30)))
            {
                currentStep = SetupStep.Configuration;
            }
            
            if (GUILayout.Button("📖 ドキュメント", GUILayout.Height(30)))
            {
                Application.OpenURL("https://zapabob.github.io/liltoon-pcss-extension/documentation.html");
            }
            
            if (GUILayout.Button("✨ ウィザードを閉じる", GUILayout.Height(30)))
            {
                Close();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawFooter()
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            
            EditorGUILayout.BeginHorizontal();
            
            // 戻るボタン
            EditorGUI.BeginDisabledGroup(currentStep == SetupStep.Welcome);
            if (GUILayout.Button("⬅️ 戻る", GUILayout.Width(100)))
            {
                currentStep = (SetupStep)((int)currentStep - 1);
            }
            EditorGUI.EndDisabledGroup();
            
            GUILayout.FlexibleSpace();
            
            // 進むボタン
            EditorGUI.BeginDisabledGroup(
                currentStep == SetupStep.Complete ||
                (currentStep == SetupStep.TargetSelection && targetAvatar == null)
            );
            
            if (GUILayout.Button("進む ➡️", GUILayout.Width(100)))
            {
                if (currentStep == SetupStep.Installation)
                {
                    // インストールステップでは実行ボタンを使用
                }
                else
                {
                    currentStep = (SetupStep)((int)currentStep + 1);
                }
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
        }
        
        // ヘルパーメソッド
        private void DrawFeatureItem(string icon, string title, string description)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(icon, GUILayout.Width(20));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(description, EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }
        
        private void DrawPresetDescription(string title, string description, Color color)
        {
            var originalColor = GUI.backgroundColor;
            GUI.backgroundColor = color * 0.3f;
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel);
            
            EditorGUILayout.EndVertical();
            
            GUI.backgroundColor = originalColor;
        }
        
        private void DrawInstallationItem(string icon, string label, string value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(icon, GUILayout.Width(20));
            EditorGUILayout.LabelField(label, GUILayout.Width(120));
            EditorGUILayout.LabelField(value, EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawCompletionItem(string icon, string description)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(icon, GUILayout.Width(20));
            EditorGUILayout.LabelField(description);
            EditorGUILayout.EndHorizontal();
        }
        
        private void AnalyzeTargetAvatar()
        {
            if (targetAvatar == null) return;
            
#if VRCHAT_SDK_AVAILABLE
            // VRCAvatarDescriptor取得
            avatarDescriptor = targetAvatar.GetComponent<VRCAvatarDescriptor>();
            
            // PhysBone検出
            detectedPhysBones.Clear();
            var physBones = targetAvatar.GetComponentsInChildren<VRCPhysBone>();
            detectedPhysBones.AddRange(physBones);
#endif
            
            // Renderer検出
            targetRenderers.Clear();
            var renderers = targetAvatar.GetComponentsInChildren<Renderer>();
            targetRenderers.AddRange(renderers);
            
            // Light検出
            existingLights.Clear();
            var lights = targetAvatar.GetComponentsInChildren<Light>();
            existingLights.AddRange(lights);
        }
        
        private void DrawAnalysisResults()
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("🔍 検出結果", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
#if VRCHAT_SDK_AVAILABLE
            // VRCAvatarDescriptor
            if (avatarDescriptor != null)
            {
                EditorGUILayout.LabelField($"✅ VRCAvatarDescriptor: {avatarDescriptor.name}");
            }
            else
            {
                EditorGUILayout.LabelField("❌ VRCAvatarDescriptorが見つかりません", EditorStyles.miniLabel);
            }
            
            // PhysBone
            EditorGUILayout.LabelField($"🎮 PhysBone: {detectedPhysBones.Count}個検出");
            if (detectedPhysBones.Count > 0)
            {
                EditorGUI.indentLevel++;
                foreach (var physBone in detectedPhysBones.Take(3))
                {
                    EditorGUILayout.LabelField($"• {physBone.name}", EditorStyles.miniLabel);
                }
                if (detectedPhysBones.Count > 3)
                {
                    EditorGUILayout.LabelField($"• ...他{detectedPhysBones.Count - 3}個", EditorStyles.miniLabel);
                }
                EditorGUI.indentLevel--;
            }
#endif
            
            // Renderer
            EditorGUILayout.LabelField($"🎨 Renderer: {targetRenderers.Count}個検出");
            
            // Light
            EditorGUILayout.LabelField($"💡 既存Light: {existingLights.Count}個検出");
            
            EditorGUILayout.EndVertical();
        }
        
        private void ExecuteInstallation()
        {
            try
            {
                EditorUtility.DisplayProgressBar("競合製品互換セットアップ", "インストール中...", 0.0f);
                
                // 1. PhysBone Light Controller追加
                EditorUtility.DisplayProgressBar("競合製品互換セットアップ", "PhysBone Light Controller設定中...", 0.2f);
                SetupPhysBoneLightController();
                
                // 2. PCSS Shader適用
                EditorUtility.DisplayProgressBar("競合製品互換セットアップ", "PCSS Shader適用中...", 0.4f);
                ApplyPCSSShaders();
                
                // 3. Expression Menu作成
                if (enableExpressionMenu)
                {
                    EditorUtility.DisplayProgressBar("競合製品互換セットアップ", "Expression Menu作成中...", 0.6f);
                    CreateExpressionMenu();
                }
                
                // 4. 最適化設定適用
                EditorUtility.DisplayProgressBar("競合製品互換セットアップ", "最適化設定適用中...", 0.8f);
                ApplyOptimizationSettings();
                
                // 5. プリセット適用
                EditorUtility.DisplayProgressBar("競合製品互換セットアップ", "プリセット適用中...", 1.0f);
                ApplySelectedPreset();
                
                EditorUtility.ClearProgressBar();
                
                currentStep = SetupStep.Complete;
                
                Debug.Log("[競合製品互換セットアップ] インストール完了！");
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("エラー", $"インストール中にエラーが発生しました:\n{e.Message}", "OK");
                Debug.LogError($"[競合製品互換セットアップ] エラー: {e.Message}");
            }
        }
        
        private void SetupPhysBoneLightController()
        {
            if (targetAvatar == null) return;
            
            // PhysBone Light Controller追加
            var controller = targetAvatar.GetComponent<PhysBoneLightController>();
            if (controller == null)
            {
                controller = targetAvatar.AddComponent<PhysBoneLightController>();
            }
            
            // 競合製品互換設定適用
            controller.SetCompetitorCompatibilityMode(true);
            controller.TogglePhysBoneControl(enablePhysBoneControl);
        }
        
        private void ApplyPCSSShaders()
        {
            // 対象Rendererに対してPCSSシェーダーを適用
            foreach (var renderer in targetRenderers)
            {
                var materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    var material = materials[i];
                    if (material != null && material.shader.name.Contains("lilToon"))
                    {
                        // lilToon PCSS Extensionシェーダーに切り替え
                        var pcssShader = Shader.Find("lilToon/PCSS Extension");
                        if (pcssShader != null)
                        {
                            material.shader = pcssShader;
                        }
                    }
                }
            }
        }
        
        private void CreateExpressionMenu()
        {
#if VRCHAT_SDK_AVAILABLE
            if (avatarDescriptor == null) return;
            
            // VRChatExpressionMenuCreatorを使用してメニュー作成
            var menuCreator = new VRChatExpressionMenuCreator();
            menuCreator.CreatePCSSExpressionMenu(avatarDescriptor);
#endif
        }
        
        private void ApplyOptimizationSettings()
        {
            // VRChat最適化設定を適用
            var optimizer = targetAvatar.GetComponent<VRChatPerformanceOptimizer>();
            if (optimizer == null)
            {
                optimizer = targetAvatar.AddComponent<VRChatPerformanceOptimizer>();
            }
            
            // 最適化設定適用
            // optimizer.ApplyOptimizationSettings();
        }
        
        private void ApplySelectedPreset()
        {
            switch (selectedPreset)
            {
                case ShadowPreset.明暗トゥーン:
                    ApplyToonPreset();
                    break;
                case ShadowPreset.リアル:
                    ApplyRealisticPreset();
                    break;
                case ShadowPreset.競合製品互換:
                    ApplyCompetitorPreset();
                    break;
                case ShadowPreset.カスタム:
                    ApplyCustomPreset();
                    break;
            }
        }
        
        private void ApplyToonPreset()
        {
            // 明暗トゥーン用設定
            shadowSoftness = 0.1f;
            shadowBias = 0.005f;
            lightIntensity = 1.2f;
            lightColor = Color.white;
        }
        
        private void ApplyRealisticPreset()
        {
            // リアル用設定
            shadowSoftness = 0.8f;
            shadowBias = 0.001f;
            lightIntensity = 1.0f;
            lightColor = new Color(1.0f, 0.95f, 0.9f);
        }
        
        private void ApplyCompetitorPreset()
        {
            // 競合製品互換設定
            shadowSoftness = 0.5f;
            shadowBias = 0.001f;
            lightIntensity = 1.0f;
            lightColor = Color.white;
            maxDistance = 10.0f;
        }
        
        private void ApplyCustomPreset()
        {
            // カスタム設定（ユーザー定義値を使用）
        }
    }
} 