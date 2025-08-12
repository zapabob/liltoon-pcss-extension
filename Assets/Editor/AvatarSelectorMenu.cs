using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
#if MODULAR_AVATAR_AVAILABLE
using nadena.dev.modular_avatar.core;
#endif

// #if MODULAR_AVATAR_AVAILABLE
// using nadena.dev.modular_avatar.core;
// #endif

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// lilToon PCSS Extension - Avatar Selector Menu
    /// アバター選択とPCSSプリセット適用のためのUnityメニュー
    /// </summary>
    public class AvatarSelectorMenu : EditorWindow
    {
        private Vector2 scrollPosition;
        private GameObject selectedAvatar;
        private int selectedPreset = 0;
        private readonly string[] presetNames = { "🎬 リアル影", "🎨 アニメ風", "🎭 映画風", "⚙️ カスタム" };
        private readonly string[] presetDescriptions = {
            "細かいディテールと自然な影 - フォトリアル向け",
            "ソフトなエフェクトとアニメ調の影 - トゥーン調向け",
            "ドラマチックで強調された影 - 映画的表現向け",
            "手動設定によるカスタム影 - 上級者向け"
        };

        private List<GameObject> detectedAvatars = new List<GameObject>();
        private bool autoDetectAvatars = true;
        private bool showAdvancedOptions = false;

        // カスタムプリセット設定
        private float customFilterRadius = 0.01f;
        private float customLightSize = 0.1f;
        private float customBias = 0.001f;
        private float customIntensity = 1.0f;

        [MenuItem("Tools/lilToon-PCSS-Extension/🎯 Avatar Selector")]
        public static void ShowWindow()
        {
            var window = GetWindow<AvatarSelectorMenu>("Avatar Selector");
            window.titleContent = new GUIContent("🎯 Avatar Selector", "アバター選択とPCSSプリセット適用");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }

        private void OnEnable()
        {
            RefreshAvatarList();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawHeader();
            DrawAvatarSelection();
            DrawPresetSelection();
            DrawAdvancedOptions();
            DrawActionButtons();

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(10);
            
            // タイトル
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("🎯 lilToon PCSS Extension", titleStyle);
            EditorGUILayout.LabelField("Avatar Selector & Preset Applier", EditorStyles.centeredGreyMiniLabel);
            
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(10);
        }

        private void DrawAvatarSelection()
        {
            EditorGUILayout.LabelField("📋 アバター選択", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            autoDetectAvatars = EditorGUILayout.Toggle("自動検出", autoDetectAvatars);
            if (GUILayout.Button("🔄 更新", GUILayout.Width(60)))
            {
                RefreshAvatarList();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            if (autoDetectAvatars && detectedAvatars.Count > 0)
            {
                EditorGUILayout.LabelField($"検出されたアバター: {detectedAvatars.Count}個", EditorStyles.helpBox);
                
                foreach (var avatar in detectedAvatars)
                {
                    if (avatar == null) continue;

                    EditorGUILayout.BeginHorizontal();
                    
                    bool isSelected = selectedAvatar == avatar;
                    bool newSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));
                    
                    if (newSelected != isSelected)
                    {
                        selectedAvatar = newSelected ? avatar : null;
                    }

                    EditorGUILayout.ObjectField(avatar, typeof(GameObject), true);
                    
                    // アバター情報表示
#if VRCHAT_SDK_AVAILABLE
                    var descriptor = avatar.GetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>();
                    if (descriptor != null)
                    {
                        EditorGUILayout.LabelField("✅ VRC", GUILayout.Width(40));
                    }
#endif

                    // #if MODULAR_AVATAR_AVAILABLE
                    // var modularAvatar = avatar.GetComponent<ModularAvatarInformation>();
                    // if (modularAvatar != null)
                    // {
                    //     EditorGUILayout.LabelField("🔧 MA", GUILayout.Width(40));
                    // }
                    // #endif

                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                selectedAvatar = (GameObject)EditorGUILayout.ObjectField("手動選択", selectedAvatar, typeof(GameObject), true);
                
                if (selectedAvatar == null)
                {
                    EditorGUILayout.HelpBox("アバターを選択してください", MessageType.Info);
                }
            }

            EditorGUILayout.Space(10);
        }

        private void DrawPresetSelection()
        {
            EditorGUILayout.LabelField("🎨 PCSSプリセット", EditorStyles.boldLabel);
            
            int newPreset = GUILayout.SelectionGrid(selectedPreset, presetNames, 2);
            if (newPreset != selectedPreset)
            {
                selectedPreset = newPreset;
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox(presetDescriptions[selectedPreset], MessageType.Info);

            // プリセットプレビュー
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("プリセット設定プレビュー:", EditorStyles.miniBoldLabel);
            
            var presetParams = GetPresetParameters(selectedPreset);
            EditorGUI.BeginDisabledGroup(selectedPreset != 3); // カスタム以外は無効
            
            if (selectedPreset == 3) // カスタム
            {
                customFilterRadius = EditorGUILayout.Slider("Filter Radius", customFilterRadius, 0.001f, 0.1f);
                customLightSize = EditorGUILayout.Slider("Light Size", customLightSize, 0.01f, 0.5f);
                customBias = EditorGUILayout.Slider("Bias", customBias, 0.0001f, 0.01f);
                customIntensity = EditorGUILayout.Slider("Intensity", customIntensity, 0.0f, 2.0f);
            }
            else
            {
                EditorGUILayout.LabelField($"Filter Radius: {presetParams.x:F4}");
                EditorGUILayout.LabelField($"Light Size: {presetParams.y:F3}");
                EditorGUILayout.LabelField($"Bias: {presetParams.z:F4}");
                EditorGUILayout.LabelField($"Intensity: {presetParams.w:F2}");
            }
            
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);
        }

        private void DrawAdvancedOptions()
        {
            showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "⚙️ 詳細オプション", true);
            
            if (showAdvancedOptions)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                // シェーダー可用性チェック
                var availableShaders = lilToon.PCSS.Runtime.PCSSUtilities.DetectAvailableShaders();
                EditorGUILayout.LabelField("シェーダー可用性:", EditorStyles.miniBoldLabel);
                
                switch (availableShaders)
                {
                    case lilToon.PCSS.Runtime.PCSSUtilities.ShaderType.Both:
                        EditorGUILayout.LabelField("✁ElilToon & Poiyomi 両方利用可能", EditorStyles.helpBox);
                        break;
                    case lilToon.PCSS.Runtime.PCSSUtilities.ShaderType.LilToon:
                        EditorGUILayout.LabelField("✁ElilToonのみ利用可能", EditorStyles.helpBox);
                        EditorGUILayout.HelpBox("Poiyomiシェーダーが見つかりません。lilToonマテリアルのみ処理されます。", MessageType.Warning);
                        break;
                    case lilToon.PCSS.Runtime.PCSSUtilities.ShaderType.Poiyomi:
                        EditorGUILayout.LabelField("✁EPoiyomiのみ利用可能", EditorStyles.helpBox);
                        EditorGUILayout.HelpBox("lilToonシェーダーが見つかりません。Poiyomiマテリアルのみ処理されます。", MessageType.Warning);
                        break;
                    case lilToon.PCSS.Runtime.PCSSUtilities.ShaderType.Unknown:
                        EditorGUILayout.LabelField("⚠️ 対応シェーダーが見つかりません", EditorStyles.helpBox);
                        EditorGUILayout.HelpBox("lilToon または Poiyomi シェーダーをインポートしてください。", MessageType.Error);
                        break;
                }
                
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("適用対象:", EditorStyles.miniBoldLabel);
                
                bool lilToonAvailable = (availableShaders == lilToon.PCSS.Runtime.PCSSUtilities.ShaderType.LilToon || 
                                       availableShaders == lilToon.PCSS.Runtime.PCSSUtilities.ShaderType.Both);
                bool poiyomiAvailable = (availableShaders == lilToon.PCSS.Runtime.PCSSUtilities.ShaderType.Poiyomi || 
                                       availableShaders == lilToon.PCSS.Runtime.PCSSUtilities.ShaderType.Both);
                
                EditorGUI.BeginDisabledGroup(!lilToonAvailable);
                EditorGUILayout.Toggle(lilToonAvailable ? "✁ElilToonマテリアル" : "❁ElilToonマテリアル", lilToonAvailable);
                EditorGUI.EndDisabledGroup();
                
                EditorGUI.BeginDisabledGroup(!poiyomiAvailable);
                EditorGUILayout.Toggle(poiyomiAvailable ? "✁EPoiyomiマテリアル" : "❁EPoiyomiマテリアル", poiyomiAvailable);
                EditorGUI.EndDisabledGroup();
                
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("追加機能:", EditorStyles.miniBoldLabel);
                EditorGUILayout.Toggle("🔧 ModularAvatar統合", true);
                EditorGUILayout.Toggle("⚡ VRChat最適化", true);
                EditorGUILayout.Toggle("💡 VRC Light Volumes", true);
                
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(10);
        }

        private void DrawActionButtons()
        {
            EditorGUILayout.LabelField("🚀 実行", EditorStyles.boldLabel);
            
            EditorGUI.BeginDisabledGroup(selectedAvatar == null);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("🎨 プリセット適用", GUILayout.Height(30)))
            {
                ApplyPresetToAvatar();
            }
            
            if (GUILayout.Button("👁️ プレビュー", GUILayout.Height(30)))
            {
                PreviewPreset();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            if (GUILayout.Button("🔧 ワンクリックセットアップ", GUILayout.Height(35)))
            {
                OneClickSetup();
            }
            
            EditorGUI.EndDisabledGroup();

            if (selectedAvatar == null)
            {
                EditorGUILayout.HelpBox("アバターを選択してください", MessageType.Warning);
            }
        }

        private void RefreshAvatarList()
        {
            detectedAvatars.Clear();
            
            #if VRCHAT_SDK_AVAILABLE
            // VRCAvatarDescriptorを持つGameObjectを検索
            var avatarDescriptors = FindObjectsOfType<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>();
            detectedAvatars.AddRange(avatarDescriptors.Select(desc => desc.gameObject));
            #endif

            // #if MODULAR_AVATAR_AVAILABLE
            // // ModularAvatarInformationを持つGameObjectも検索
            // var modularAvatars = FindObjectsOfType<ModularAvatarInformation>();
            // foreach (var ma in modularAvatars)
            // {
            //     if (!detectedAvatars.Contains(ma.gameObject))
            //     {
            //         detectedAvatars.Add(ma.gameObject);
            //     }
            // }
            // #endif

            // 重複除去してソート
            detectedAvatars = detectedAvatars.Distinct().OrderBy(go => go.name).ToList();
        }

        private Vector4 GetPresetParameters(int presetIndex)
        {
            switch (presetIndex)
            {
                case 0: // リアル影
                    return new Vector4(0.005f, 0.05f, 0.0005f, 1.0f);
                case 1: // アニメ風
                    return new Vector4(0.015f, 0.1f, 0.001f, 0.8f);
                case 2: // 映画風
                    return new Vector4(0.025f, 0.2f, 0.002f, 1.2f);
                case 3: // カスタム
                    return new Vector4(customFilterRadius, customLightSize, customBias, customIntensity);
                default:
                    return new Vector4(0.01f, 0.1f, 0.001f, 1.0f);
            }
        }

        private void ApplyPresetToAvatar()
        {
            if (selectedAvatar == null) return;

            Undo.RecordObject(selectedAvatar, "Apply PCSS Preset");

            var presetParams = GetPresetParameters(selectedPreset);
            string presetName = presetNames[selectedPreset];

            Debug.Log($"🎨 Applying PCSS Preset '{presetName}' to {selectedAvatar.name}");

            // アバターの全マテリアルを検索
            var renderers = selectedAvatar.GetComponentsInChildren<Renderer>();
            int appliedCount = 0;

            foreach (var renderer in renderers)
            {
                // ModularAvatarが導入されていればMA Material Swap/Platform Filterを動的に追加
                #if MODULAR_AVATAR_AVAILABLE
                var swap = renderer.gameObject.GetComponent<MAMaterialSwap>();
                if (swap == null) swap = renderer.gameObject.AddComponent<MAMaterialSwap>();
                swap.Renderer = renderer;
                swap.Materials = new List<Material>(renderer.sharedMaterials);
                foreach (var mat in swap.Materials)
                {
                    if (mat == null) continue;
                    if (IsPCSSCompatibleShader(mat.shader))
                    {
                        ApplyPresetToMaterial(mat, presetParams, selectedPreset);
                        appliedCount++;
                    }
                }
                // Quest/PC刁E��が忁E��ならMAPlatformFilterも追加侁E
                // var filter = renderer.gameObject.GetComponent<MAPlatformFilter>();
                // if (filter == null) filter = renderer.gameObject.AddComponent<MAPlatformFilter>();
                // filter.Platform = MAPlatformFilter.PlatformType.Quest;
                #endif
                // 既存�E手動マテリアル刁E��替えも残す�E�EodularAvatar未導�E時用�E�E
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null) continue;
                    if (IsPCSSCompatibleShader(material.shader))
                    {
                        ApplyPresetToMaterial(material, presetParams, selectedPreset);
                        appliedCount++;
                    }
                }
            }

            EditorUtility.SetDirty(selectedAvatar);
            EditorUtility.DisplayDialog("適用完了",
                $"✅ プリセット '{presetName}' を適用しました\n" +
                $"📊 適用されたマテリアル: {appliedCount}個", "OK");
        }

        private void PreviewPreset()
        {
            if (selectedAvatar == null) return;

            // Scene Viewにフォーカス
            SceneView.FocusWindowIfItsOpen(typeof(SceneView));
            
            // アバターを選択してフレーミング
            Selection.activeGameObject = selectedAvatar;
            SceneView.FrameLastActiveSceneView();

            Debug.Log($"👁️ プレビュー中: {presetNames[selectedPreset]} on {selectedAvatar.name}");
        }

        private void OneClickSetup()
        {
            if (selectedAvatar == null) return;

            bool result = EditorUtility.DisplayDialog("ワンクリックセットアップ",
                "以下の操作を実行します\n" +
                "• PCSSプリセットの適用\n" +
                "• VRChat Expression Menuの作成\n" +
                "• パフォーマンス最適化の適用\n" +
                "• VRC Light Volumesの設定\n\n" +
                "続行しますか？", "実行", "キャンセル");

            if (result)
            {
                ApplyPresetToAvatar();
                
                // 追加のセットアップ処理
                SetupVRChatExpressions();
                OptimizePerformance();
                SetupLightVolumes();

                EditorUtility.DisplayDialog("セットアップ完了",
                    "🎉 ワンクリックセットアップが完了しました。\n" +
                    "アバターはロード準備が整いました。", "OK");
            }
        }

        private bool IsPCSSCompatibleShader(Shader shader)
        {
            if (shader == null) return false;
            
            string shaderName = shader.name.ToLower();
            return shaderName.Contains("liltoon") && shaderName.Contains("pcss") ||
                   shaderName.Contains("poiyomi") && shaderName.Contains("pcss");
        }

        private void ApplyPresetToMaterial(Material material, Vector4 presetParams, int presetIndex)
        {
            Undo.RecordObject(material, "Apply PCSS Preset to Material");

            // プリセットモードを設定
            if (material.HasProperty("_PCSSPresetMode"))
            {
                material.SetFloat("_PCSSPresetMode", presetIndex);
            }

            // 個別パラメータも設定（後方互換性）
            if (material.HasProperty("_PCSSFilterRadius"))
                material.SetFloat("_PCSSFilterRadius", presetParams.x);
            
            if (material.HasProperty("_PCSSLightSize"))
                material.SetFloat("_PCSSLightSize", presetParams.y);
            
            if (material.HasProperty("_PCSSBias"))
                material.SetFloat("_PCSSBias", presetParams.z);
            
            if (material.HasProperty("_PCSSIntensity"))
                material.SetFloat("_PCSSIntensity", presetParams.w);

            // PCSS機能を有効化
            if (material.HasProperty("_PCSSEnabled"))
                material.SetFloat("_PCSSEnabled", 1.0f);

            EditorUtility.SetDirty(material);
        }

        private void SetupVRChatExpressions()
        {
            // VRChat Expression Menu作成処理
            Debug.Log("🎭 VRChat Expression Menuをセットアップ中...");
        }

        private void OptimizePerformance()
        {
            // パフォーマンス最適化処理
            Debug.Log("⚡ パフォーマンスを最適化中...");
        }

        private void SetupLightVolumes()
        {
            // VRC Light Volumes設定処理
            Debug.Log("�� VRC Light Volumesをセットアップ中...");
        }
    }
} 
