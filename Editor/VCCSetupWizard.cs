using UnityEngine;
using UnityEditor;
using System.IO;
#if VRCHAT_SDK_AVAILABLE
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
#endif
#if MODULAR_AVATAR_AVAILABLE
using nadena.dev.modular_avatar.core;
#endif

namespace lilToon.PCSS
{
    public class VCCSetupWizard : EditorWindow
    {
        private GameObject selectedAvatar;
        private QualityPreset selectedQuality = QualityPreset.Medium;
        private bool enableAutoOptimization = true;
        private bool enableVRCLightVolumes = true;
        private bool showAdvancedOptions = false;
        
        private Vector2 scrollPosition;
        private GUIStyle headerStyle;
        private GUIStyle buttonStyle;
        
        public enum QualityPreset
        {
            Low = 8,
            Medium = 16,
            High = 32,
            Ultra = 64
        }

        [MenuItem("Window/lilToon PCSS Extension/Setup Wizard")]
        public static void ShowWindow()
        {
            var window = GetWindow<VCCSetupWizard>("PCSS Setup Wizard");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }

        private void OnEnable()
        {
            // 自動的にアクティブなアバターを検出
            DetectActiveAvatar();
        }

        private void InitializeStyles()
        {
            if (headerStyle == null)
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 16,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 12,
                    fixedHeight = 30
                };
            }
        }

        private void OnGUI()
        {
            InitializeStyles();
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // ヘッダー
            DrawHeader();
            
            EditorGUILayout.Space(10);
            
            // ステップ1: アバター選択
            DrawAvatarSelection();
            
            EditorGUILayout.Space(10);
            
            // ステップ2: 品質設定
            DrawQualitySettings();
            
            EditorGUILayout.Space(10);
            
            // ステップ3: 追加オプション
            DrawAdditionalOptions();
            
            EditorGUILayout.Space(10);
            
            // ステップ4: セットアップボタン
            DrawSetupButtons();
            
            EditorGUILayout.Space(10);
            
            // ヘルプ・サポート
            DrawHelpSection();
            
            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("🎬 lilToon PCSS Extension", headerStyle);
            GUILayout.Label("Setup Wizard v1.2.0", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space(5);
            GUILayout.Label("映画品質のソフトシャドウをアバターに追加", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.EndVertical();
        }

        private void DrawAvatarSelection()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("📝 Step 1: アバター選択", EditorStyles.boldLabel);
            
            selectedAvatar = (GameObject)EditorGUILayout.ObjectField(
                "Target Avatar", 
                selectedAvatar, 
                typeof(GameObject), 
                true
            );
            
            if (selectedAvatar != null)
            {
#if VRCHAT_SDK_AVAILABLE
                var avatarDescriptor = selectedAvatar.GetComponent<VRCAvatarDescriptor>();
                if (avatarDescriptor == null)
                {
                    EditorGUILayout.HelpBox("⚠️ 選択されたオブジェクトにVRC Avatar Descriptorが見つかりません。", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox("✅ VRChatアバターが検出されました！", MessageType.Info);
                }
#endif
            }
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🔍 アクティブアバターを検出", buttonStyle))
            {
                DetectActiveAvatar();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }

        private void DrawQualitySettings()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("⚙️ Step 2: 品質設定", EditorStyles.boldLabel);
            
            selectedQuality = (QualityPreset)EditorGUILayout.EnumPopup("Quality Preset", selectedQuality);
            
            // 品質説明
            string qualityDescription = GetQualityDescription(selectedQuality);
            EditorGUILayout.HelpBox(qualityDescription, MessageType.Info);
            
            EditorGUILayout.EndVertical();
        }

        private void DrawAdditionalOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("🔧 Step 3: 追加オプション", EditorStyles.boldLabel);
            
            enableAutoOptimization = EditorGUILayout.Toggle("自動最適化を有効化", enableAutoOptimization);
            enableVRCLightVolumes = EditorGUILayout.Toggle("VRC Light Volumes統合", enableVRCLightVolumes);
            
            showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "高度な設定");
            
            if (showAdvancedOptions)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("高度な設定は経験豊富なユーザー向けです。", MessageType.Info);
                // 将来的に高度な設定を追加
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawSetupButtons()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("🚀 Step 4: セットアップ実行", EditorStyles.boldLabel);
            
            GUI.enabled = selectedAvatar != null;
            
            if (GUILayout.Button("✨ 自動セットアップ実行", buttonStyle))
            {
                PerformAutoSetup();
            }
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🎨 マテリアルのみ設定", buttonStyle))
            {
                SetupMaterialsOnly();
            }
            
            if (GUILayout.Button("🎮 エクスプレッションのみ設定", buttonStyle))
            {
                SetupExpressionsOnly();
            }
            EditorGUILayout.EndHorizontal();
            
            GUI.enabled = true;
            
            EditorGUILayout.EndVertical();
        }

        private void DrawHelpSection()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("📚 ヘルプ・サポート", EditorStyles.boldLabel);
            
            if (GUILayout.Button("📖 ドキュメントを開く"))
            {
                Application.OpenURL("https://github.com/zapabob/liltoon-pcss-extension/blob/main/README.md");
            }
            
            if (GUILayout.Button("🐛 問題を報告"))
            {
                Application.OpenURL("https://github.com/zapabob/liltoon-pcss-extension/issues");
            }
            
            if (GUILayout.Button("💬 Discordサポート"))
            {
                Application.OpenURL("https://discord.gg/vrchat-creators");
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DetectActiveAvatar()
        {
#if VRCHAT_SDK_AVAILABLE
            var avatarDescriptors = FindObjectsOfType<VRCAvatarDescriptor>();
            if (avatarDescriptors.Length > 0)
            {
                selectedAvatar = avatarDescriptors[0].gameObject;
                Debug.Log($"[PCSS Setup] アバターを検出しました: {selectedAvatar.name}");
            }
            else
            {
                Debug.LogWarning("[PCSS Setup] VRChatアバターが見つかりませんでした。");
            }
#endif
        }

        private string GetQualityDescription(QualityPreset quality)
        {
            switch (quality)
            {
                case QualityPreset.Low:
                    return "Low (8サンプル): Quest・軽量化向け。最小限のパフォーマンス負荷。";
                case QualityPreset.Medium:
                    return "Medium (16サンプル): バランス重視。PC標準環境に最適。";
                case QualityPreset.High:
                    return "High (32サンプル): 高品質。PC高性能環境向け。";
                case QualityPreset.Ultra:
                    return "Ultra (64サンプル): 最高品質。PC最高性能環境向け。";
                default:
                    return "";
            }
        }

        private void PerformAutoSetup()
        {
            if (selectedAvatar == null)
            {
                EditorUtility.DisplayDialog("エラー", "アバターが選択されていません。", "OK");
                return;
            }

            try
            {
                EditorUtility.DisplayProgressBar("PCSS セットアップ", "セットアップを開始しています...", 0.0f);
                
                // 1. マテリアル設定
                EditorUtility.DisplayProgressBar("PCSS セットアップ", "マテリアルを設定しています...", 0.25f);
                SetupMaterials();
                
                // 2. ModularAvatar設定
                EditorUtility.DisplayProgressBar("PCSS セットアップ", "ModularAvatarを設定しています...", 0.5f);
                SetupModularAvatar();
                
                // 3. エクスプレッション設定
                EditorUtility.DisplayProgressBar("PCSS セットアップ", "エクスプレッションを設定しています...", 0.75f);
                SetupExpressions();
                
                // 4. 最適化設定
                EditorUtility.DisplayProgressBar("PCSS セットアップ", "最適化設定を適用しています...", 0.9f);
                if (enableAutoOptimization)
                {
                    SetupOptimization();
                }
                
                EditorUtility.DisplayProgressBar("PCSS セットアップ", "完了しています...", 1.0f);
                
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("完了", "PCSS セットアップが正常に完了しました！", "OK");
                
                Debug.Log("[PCSS Setup] 自動セットアップが完了しました。");
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("エラー", $"セットアップ中にエラーが発生しました: {e.Message}", "OK");
                Debug.LogError($"[PCSS Setup] エラー: {e}");
            }
        }

        private void SetupMaterialsOnly()
        {
            if (selectedAvatar == null) return;
            
            try
            {
                SetupMaterials();
                EditorUtility.DisplayDialog("完了", "マテリアルの設定が完了しました！", "OK");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("エラー", $"マテリアル設定中にエラーが発生しました: {e.Message}", "OK");
            }
        }

        private void SetupExpressionsOnly()
        {
            if (selectedAvatar == null) return;
            
            try
            {
                SetupModularAvatar();
                SetupExpressions();
                EditorUtility.DisplayDialog("完了", "エクスプレッションの設定が完了しました！", "OK");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("エラー", $"エクスプレッション設定中にエラーが発生しました: {e.Message}", "OK");
            }
        }

        private void SetupMaterials()
        {
            var renderers = selectedAvatar.GetComponentsInChildren<Renderer>();
            int materialCount = 0;
            
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null && ShouldConvertMaterial(material))
                    {
                        ConvertMaterialToPCSS(material);
                        materialCount++;
                    }
                }
            }
            
            Debug.Log($"[PCSS Setup] {materialCount}個のマテリアルを変換しました。");
        }

        private bool ShouldConvertMaterial(Material material)
        {
            var shaderName = material.shader.name;
            return shaderName.Contains("lilToon") || shaderName.Contains("Poiyomi");
        }

        private void ConvertMaterialToPCSS(Material material)
        {
            var shaderName = material.shader.name;
            
            if (shaderName.Contains("lilToon"))
            {
                var pcssShader = Shader.Find("lilToon/PCSS Extension");
                if (pcssShader != null)
                {
                    material.shader = pcssShader;
                    SetMaterialQuality(material);
                }
            }
            else if (shaderName.Contains("Poiyomi"))
            {
                var pcssShader = Shader.Find("Poiyomi/PCSS Extension");
                if (pcssShader != null)
                {
                    material.shader = pcssShader;
                    SetMaterialQuality(material);
                }
            }
        }

        private void SetMaterialQuality(Material material)
        {
            if (material.HasProperty("_PCSSSampleCount"))
            {
                material.SetInt("_PCSSSampleCount", (int)selectedQuality);
            }
            
            if (material.HasProperty("_PCSSQuality"))
            {
                material.SetInt("_PCSSQuality", (int)selectedQuality);
            }
        }

        private void SetupModularAvatar()
        {
#if MODULAR_AVATAR_AVAILABLE
            // ModularAvatarコンポーネントの自動追加
            var maSetup = selectedAvatar.GetComponent<ModularAvatarMenuInstaller>();
            if (maSetup == null)
            {
                maSetup = selectedAvatar.AddComponent<ModularAvatarMenuInstaller>();
            }
            
            Debug.Log("[PCSS Setup] ModularAvatarコンポーネントを設定しました。");
#endif
        }

        private void SetupExpressions()
        {
            // エクスプレッションメニューとパラメータの自動生成
            var creator = selectedAvatar.GetComponent<VRChatExpressionMenuCreator>();
            if (creator == null)
            {
                creator = selectedAvatar.AddComponent<VRChatExpressionMenuCreator>();
            }
            
            // 品質設定を適用
            creator.defaultQuality = selectedQuality.ToString();
            
            Debug.Log("[PCSS Setup] エクスプレッション設定を完了しました。");
        }

        private void SetupOptimization()
        {
            var optimizer = selectedAvatar.GetComponent<VRChatPerformanceOptimizer>();
            if (optimizer == null)
            {
                optimizer = selectedAvatar.AddComponent<VRChatPerformanceOptimizer>();
            }
            
            optimizer.enableAutoOptimization = enableAutoOptimization;
            optimizer.enableVRCLightVolumes = enableVRCLightVolumes;
            
            Debug.Log("[PCSS Setup] 最適化設定を適用しました。");
        }
    }
} 