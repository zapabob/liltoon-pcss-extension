using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDK3.Dynamics.PhysBone.Components;
#endif

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// nHaruka PCSSForVRC互換の統合メニューシステム
    /// ワンクリックセットアップ、PhysBone制御、ModularAvatar統合を統合管理
    /// </summary>
    public class UnifiedPCSSMenuSystem : EditorWindow
    {
        #region メニュー項目
        
        // Consolidated under Dashboard
        // [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/nHaruka互換システム/ワンクリックセットアップ", priority = 10)]
        public static void OpenSetupWizard()
        {
            ModularAvatarPCSSSetupWizard.ShowWindow();
        }
        // [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/nHaruka互換システム/ワンクリックセットアップ", true)]
        private static bool ValidateOpenSetupWizard()
        {
            return HasVRChatSDK() && HasLilToon();
        }
        
        // [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/nHaruka互換システム/PhysBone制御設定", priority = 20)]
        public static void OpenPhysBoneControl()
        {
            PhysBoneLightControlWindow.ShowWindow();
        }
        // [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/nHaruka互換システム/PhysBone制御設定", true)]
        private static bool ValidateOpenPhysBoneControl()
        {
            return HasVRChatSDK();
        }
        
        // [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/nHaruka互換システム/ModularAvatar統合設定", priority = 30)]
        public static void OpenModularAvatarIntegration()
        {
            ModularAvatarIntegrationWindow.ShowWindow();
        }
        // [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/nHaruka互換システム/ModularAvatar統合設定", true)]
        private static bool ValidateOpenModularAvatarIntegration()
        {
            return HasVRChatSDK();
        }
        
        // [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/nHaruka互換システム/パフォーマンス最適化", priority = 40)]
        public static void OpenPerformanceOptimization()
        {
            PerformanceOptimizationWindow.ShowWindow();
        }
        // [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/nHaruka互換システム/パフォーマンス最適化", true)]
        private static bool ValidateOpenPerformanceOptimization()
        {
            return true;
        }
        
        // [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/nHaruka互換システム/高度なプリセット管理", priority = 90)]
        public static void OpenAdvancedPresetManagement()
        {
            AdvancedPCSSMenuSystem.ShowWindow();
        }
        // [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/nHaruka互換システム/高度なプリセット管理", true)]
        private static bool ValidateOpenAdvancedPresetManagement()
        {
            // 実体が無い場合は非活性
            return System.Type.GetType("lilToon.PCSS.Editor.AdvancedPCSSMenuSystem") != null;
        }
        
        // [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/nHaruka互換システム/競合製品互換性設定", priority = 100)]
        public static void OpenCompetitorCompatibility()
        {
            CompetitorSetupWizard.ShowWindow();
        }
        // [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/nHaruka互換システム/競合製品互換性設定", true)]
        private static bool ValidateOpenCompetitorCompatibility()
        {
            return System.Type.GetType("lilToon.PCSS.Editor.CompetitorSetupWizard") != null;
        }
        
        // 依存関係チェッカー
        private static bool HasVRChatSDK()
        {
            return System.Type.GetType("VRC.SDK3.Avatars.Components.VRCAvatarDescriptor") != null;
        }
        private static bool HasLilToon()
        {
            return System.Type.GetType("lilToon.lilToonInspector") != null;
        }
        
        #endregion
        
        #region 統合ウィンドウ
        
        public class PhysBoneLightControlWindow : EditorWindow
        {
            // ショートカット: Ctrl/Cmd+Shift+D
            [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Dashboard %#D", priority = 10)]
            public static void OpenDashboard()
            {
                var w = GetWindow<PhysBoneLightControlWindow>("PCSS Dashboard");
                w.minSize = new Vector2(720, 560);
                w.Show();
            }
            [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Dashboard", true)]
            private static bool ValidateOpenDashboard()
            {
                return true;
            }

            private GameObject targetAvatar;
            private MeshRenderer selectedEmitter;
            private Vector2 scrollPosition;
            
            [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/nHaruka互換システム/PhysBone制御設定")]
            public static void ShowWindow()
            {
                PhysBoneLightControlWindow window = GetWindow<PhysBoneLightControlWindow>("PhysBone制御設定");
                window.minSize = new Vector2(600, 500);
                window.Show();
            }
            
            private void OnGUI()
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("PCSS Dashboard", EditorStyles.boldLabel);
                EditorGUILayout.Space(5);
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("プリセット導入", GUILayout.Width(140))) PCSSPresetInstaller.Install();
                    if (GUILayout.Button("ワンクリックセットアップ", GUILayout.Width(180))) ModularAvatarPCSSSetupWizard.ShowWindow();
                    #if VRC_SDK_VRCSDK3
                    if (GUILayout.Button("ModularAvatar統合", GUILayout.Width(180))) ModularAvatarIntegrationWindow.ShowWindow();
                    #else
                    EditorGUI.BeginDisabledGroup(true);
                    GUILayout.Button("ModularAvatar統合", GUILayout.Width(180));
                    EditorGUI.EndDisabledGroup();
                    #endif
                    if (GUILayout.Button("最適化", GUILayout.Width(120))) PerformanceOptimizationWindow.ShowWindow();
                    if (GUILayout.Button("Build FX", GUILayout.Width(100))) PCSSUtilitiesEditor.BuildFxControllerForSelected();
                }

                EditorGUILayout.Space(6);
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("欠損スクリプト削除", GUILayout.Width(160)))
                        typeof(PCSSUtilitiesEditor).GetMethod("RemoveMissingScripts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?.Invoke(null, null);

                    if (GUILayout.Button("名前一意化", GUILayout.Width(120)))
                        typeof(PCSSUtilitiesEditor).GetMethod("RenameToUnique", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?.Invoke(null, null);

                    if (GUILayout.Button("MeshRenderer preset適用", GUILayout.Width(200)))
                        typeof(PCSSUtilitiesEditor).GetMethod("ApplyMeshRendererPresetToSelection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?.Invoke(null, null);
                }
                
                // アバター選択
                EditorGUILayout.BeginHorizontal();
                targetAvatar = (GameObject)EditorGUILayout.ObjectField("アバター", targetAvatar, typeof(GameObject), true);
                if (GUILayout.Button("検索", GUILayout.Width(60)))
                {
                    FindControllers();
                }
                EditorGUILayout.EndHorizontal();
                
                if (targetAvatar == null)
                {
                    EditorGUILayout.HelpBox("アバターを選択してください。", MessageType.Info);
                    return;
                }
                
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                
                // コントローラー一覧
                DrawControllerList();
                
                // 選択されたエミッションの設定
                if (selectedEmitter != null)
                {
                    DrawControllerSettings();
                }
                
                EditorGUILayout.EndScrollView();
                
                // アクションボタン
                DrawActionButtons();
            }
            
            private void FindControllers()
            {
                if (targetAvatar == null) return;

                var emitters = targetAvatar.GetComponentsInChildren<MeshRenderer>(true)
                    .Where(mr => mr.name.Contains("PCSS_Emission")).ToArray();
                if (emitters.Length > 0)
                {
                    selectedEmitter = emitters[0];
                    Debug.Log($"[Emission Control] Found {emitters.Length} emitters");
                }
            }
            
            private void DrawControllerList()
            {
                if (targetAvatar == null) return;
                
                var emitters = targetAvatar.GetComponentsInChildren<MeshRenderer>(true)
                    .Where(mr => mr.name.Contains("PCSS_Emission")).ToArray();

                EditorGUILayout.LabelField("Emission 一覧:", EditorStyles.boldLabel);

                for (int i = 0; i < emitters.Length; i++)
                {
                    var em = emitters[i];
                    bool isSelected = selectedEmitter == em;
                    bool newSelected = EditorGUILayout.ToggleLeft(em.name, isSelected);

                    if (newSelected && !isSelected)
                    {
                        selectedEmitter = em;
                    }
                }

                if (emitters.Length == 0)
                {
                    EditorGUILayout.HelpBox("PCSS_Emission が見つかりませんでした。", MessageType.Warning);
                }
            }
            
            private void DrawControllerSettings()
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("設定項目:", EditorStyles.boldLabel);
                
                // Emission 設定
                var mat = selectedEmitter != null ? selectedEmitter.sharedMaterial : null;
                if (mat == null)
                {
                    EditorGUILayout.HelpBox("Emission用マテリアルが見つかりません。", MessageType.Warning);
                }
                else
                {
                    mat.EnableKeyword("_EMISSION");
                    Color current = mat.GetColor("_EmissionColor");
                    Color baseColor = EditorGUILayout.ColorField("Emission Color", current);
                    float intensity = EditorGUILayout.Slider("Intensity", current.maxColorComponent, 0f, 2f);
                    mat.SetColor("_EmissionColor", baseColor * intensity);
                }
                
                EditorGUILayout.Space(10);
                EditorGUILayout.HelpBox("スクリプト禁止のため、点灯/強度は Animator + Expression Parameters で制御してください。", MessageType.Info);
            }
            
            private void DrawActionButtons()
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("新しいEmissionを作成"))
                {
                    CreateNewController();
                }
                
                if (selectedEmitter != null)
                {
                    if (GUILayout.Button("Emissionを削除"))
                    {
                        DeleteController();
                    }
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            private void CreateNewController()
            {
                if (targetAvatar == null) return;
                
                var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.name = "PCSS_Emission";
                quad.transform.SetParent(targetAvatar.transform);
                var col = quad.GetComponent<Collider>();
                if (col) Object.DestroyImmediate(col);
                var mr = quad.GetComponent<MeshRenderer>();
                var mat = new Material(Shader.Find("Standard"));
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.white * 1.0f);
                mr.sharedMaterial = mat;

                selectedEmitter = mr;

                Selection.activeObject = quad;

                Debug.Log($"[Emission Control] New emitter created: {quad.name}");
            }
            
            private void DeleteController()
            {
                if (selectedEmitter == null) return;

                if (EditorUtility.DisplayDialog("確認", "このEmissionを削除しますか？", "削除", "キャンセル"))
                {
                    DestroyImmediate(selectedEmitter.gameObject);
                    selectedEmitter = null;
                }
            }
        }
        
        public class ModularAvatarIntegrationWindow : EditorWindow
        {
            private GameObject targetAvatar;
            private Vector2 scrollPosition;
            
            [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/nHaruka互換システム/ModularAvatar統合設定")]
            public static void ShowWindow()
            {
                ModularAvatarIntegrationWindow window = GetWindow<ModularAvatarIntegrationWindow>("ModularAvatar統合設定");
                window.minSize = new Vector2(600, 500);
                window.Show();
            }
            
            private void OnGUI()
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("ModularAvatar統合設定 - nHaruka PCSSForVRC互換", EditorStyles.boldLabel);
                EditorGUILayout.Space(5);
                
                targetAvatar = (GameObject)EditorGUILayout.ObjectField("アバター", targetAvatar, typeof(GameObject), true);
                
                if (targetAvatar == null)
                {
                    EditorGUILayout.HelpBox("アバターを選択してください。", MessageType.Info);
                    return;
                }
                
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                
                DrawModularAvatarSettings();
                
                EditorGUILayout.EndScrollView();
                
                DrawActionButtons();
            }
            
            private void DrawModularAvatarSettings()
            {
                #if VRC_SDK_VRCSDK3
                var avatar = targetAvatar.GetComponent<VRCAvatarDescriptor>();
                if (avatar == null)
                {
                    EditorGUILayout.HelpBox("選択されたオブジェクトはVRChatアバターではありません。", MessageType.Warning);
                    return;
                }
                #else
                EditorGUILayout.HelpBox("VRChat SDK が見つかりません。VRC SDK を導入してください。", MessageType.Warning);
                return;
                #endif
                
                EditorGUILayout.LabelField("ModularAvatar統合設定", EditorStyles.boldLabel);
                
                // パラメータ設定
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Expression Parameters", EditorStyles.boldLabel);
                
                #if VRC_SDK_VRCSDK3
                var parameters = avatar.expressionParameters;
                if (parameters != null)
                {
                    EditorGUILayout.LabelField($"パラメータ数: {parameters.parameters.Length}");
                    
                    foreach (var param in parameters.parameters)
                    {
                        if (param.name.Contains("PCSS"))
                        {
                            EditorGUILayout.LabelField($"- {param.name}: {param.valueType}");
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Expression Parametersが設定されていません。", MessageType.Warning);
                }
                #endif
                
                // メニュー設定
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Expression Menu", EditorStyles.boldLabel);
                
                #if VRC_SDK_VRCSDK3
                var menu = avatar.expressionsMenu;
                if (menu != null)
                {
                    EditorGUILayout.LabelField($"メニュー名: {menu.name}");
                    EditorGUILayout.LabelField($"コントロール数: {menu.controls.Count}");
                }
                else
                {
                    EditorGUILayout.HelpBox("Expression Menuが設定されていません。", MessageType.Warning);
                }
                #endif
            }
            
            private void DrawActionButtons()
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("PCSSパラメータを追加"))
                {
                    AddPCSSParameters();
                }
                
                if (GUILayout.Button("PCSSメニューを作成"))
                {
                    CreatePCSSMenu();
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            private void AddPCSSParameters()
            {
                #if VRC_SDK_VRCSDK3
                var avatar = targetAvatar.GetComponent<VRCAvatarDescriptor>();
                if (avatar == null) return;
                
                var parameters = avatar.expressionParameters;
                if (parameters == null)
                {
                    parameters = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                    avatar.expressionParameters = parameters;
                }
                
                var parameterList = new List<VRCExpressionParameters.Parameter>(parameters.parameters);
                
                // PCSSパラメータの追加
                string[] pcssParameters = {
                    "PCSS_Light_On",
                    "PCSS_Light_Intensity",
                    "PCSS_Light_Color",
                    "PCSS_Shadow_On",
                    "PCSS_Shadow_Strength"
                };
                
                foreach (var paramName in pcssParameters)
                {
                    if (!parameterList.Any(p => p.name == paramName))
                    {
                        parameterList.Add(new VRCExpressionParameters.Parameter
                        {
                            name = paramName,
                            valueType = paramName.Contains("Color") ? VRCExpressionParameters.ValueType.Float : VRCExpressionParameters.ValueType.Bool,
                            defaultValue = paramName.Contains("Intensity") ? 1f : 1f
                        });
                    }
                }
                
                parameters.parameters = parameterList.ToArray();
                
                Debug.Log("[ModularAvatar Integration] PCSS parameters added");
                #else
                EditorUtility.DisplayDialog("PCSS", "VRChat SDK が見つかりません。パラメータ追加はスキップしました。", "OK");
                #endif
            }
            
            private void CreatePCSSMenu()
            {
                #if VRC_SDK_VRCSDK3
                var avatar = targetAvatar.GetComponent<VRCAvatarDescriptor>();
                if (avatar == null) return;
                
                var menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                menu.name = "PCSS Light Control";
                
                // メニューコントロールの追加
                var controls = new List<VRCExpressionsMenu.Control>();
                
                // ライトオン/オフ
                controls.Add(new VRCExpressionsMenu.Control
                {
                    name = "Light On/Off",
                    parameter = new VRCExpressionsMenu.Control.Parameter { name = "PCSS_Light_On" },
                    type = VRCExpressionsMenu.Control.ControlType.Toggle
                });
                
                // ライト強度
                controls.Add(new VRCExpressionsMenu.Control
                {
                    name = "Light Intensity",
                    parameter = new VRCExpressionsMenu.Control.Parameter { name = "PCSS_Light_Intensity" },
                    type = VRCExpressionsMenu.Control.ControlType.RadialPuppet,
                    subParameters = new VRCExpressionsMenu.Control.Parameter[] {
                        new VRCExpressionsMenu.Control.Parameter { name = "PCSS_Light_Intensity" }
                    }
                });
                
                menu.controls = controls;  // VRCExpressionsMenu.controls は List<Control>
                
                // アセットとして保存
                string path = "Assets/PCSS_Menu.asset";
                AssetDatabase.CreateAsset(menu, path);
                AssetDatabase.SaveAssets();
                
                avatar.expressionsMenu = menu;
                
                Debug.Log($"[ModularAvatar Integration] PCSS menu created: {path}");
                #else
                EditorUtility.DisplayDialog("PCSS", "VRChat SDK が見つかりません。メニュー作成はスキップしました。", "OK");
                #endif
            }
        }
        
        public class PerformanceOptimizationWindow : EditorWindow
        {
            private GameObject targetAvatar;
            private Vector2 scrollPosition;
            
            [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/nHaruka互換システム/パフォーマンス最適化")]
            public static void ShowWindow()
            {
                PerformanceOptimizationWindow window = GetWindow<PerformanceOptimizationWindow>("パフォーマンス最適化");
                window.minSize = new Vector2(600, 500);
                window.Show();
            }
            
            private void OnGUI()
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("パフォーマンス最適化 - nHaruka PCSSForVRC互換", EditorStyles.boldLabel);
                EditorGUILayout.Space(5);
                
                targetAvatar = (GameObject)EditorGUILayout.ObjectField("アバター", targetAvatar, typeof(GameObject), true);
                
                if (targetAvatar == null)
                {
                    EditorGUILayout.HelpBox("アバターを選択してください。", MessageType.Info);
                    return;
                }
                
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                
                DrawPerformanceSettings();
                
                EditorGUILayout.EndScrollView();
                
                DrawActionButtons();
            }
            
            private void DrawPerformanceSettings()
            {
                var lights = targetAvatar.GetComponentsInChildren<Light>(true);
                
                EditorGUILayout.LabelField("パフォーマンス設定", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"検出されたライト: {lights.Length}");
                
                for (int i = 0; i < lights.Length; i++)
                {
                    var light = lights[i];
                    EditorGUILayout.Space(10);
                    EditorGUILayout.LabelField($"ライト {i + 1}: {light.name}", EditorStyles.boldLabel);
                    
                    var serializedLight = new SerializedObject(light);
                    var range = serializedLight.FindProperty("m_Range");
                    var intensity = serializedLight.FindProperty("m_Intensity");
                    var shadows = serializedLight.FindProperty("m_Shadows.m_Type");
                    
                    if (range != null) EditorGUILayout.PropertyField(range, new GUIContent("Range"));
                    if (intensity != null) EditorGUILayout.PropertyField(intensity, new GUIContent("Intensity"));
                    if (shadows != null) EditorGUILayout.PropertyField(shadows, new GUIContent("Shadows"));
                    
                    serializedLight.ApplyModifiedProperties();
                }
            }
            
            private void DrawActionButtons()
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("全コントローラーを最適化"))
                {
                    OptimizeAllControllers();
                }
                
                if (GUILayout.Button("パフォーマンステスト実行"))
                {
                    RunPerformanceTest();
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            private void OptimizeAllControllers()
            {
                var lights = targetAvatar.GetComponentsInChildren<Light>(true);
                
                foreach (var light in lights)
                {
                    light.shadows = LightShadows.None;
                    light.intensity = Mathf.Min(light.intensity, 1.0f);
                    light.range = Mathf.Min(light.range, 5.0f);
                }
                
                Debug.Log($"[Performance Optimization] Optimized {lights.Length} lights");
            }
            
            private void RunPerformanceTest()
            {
                var lights = targetAvatar.GetComponentsInChildren<Light>(true);
                
                float totalCost = lights.Length * 0.05f; // 簡易指標
                EditorUtility.DisplayDialog("パフォーマンステスト結果", 
                    $"ライト数: {lights.Length}\n" +
                    $"概算更新コスト: {totalCost:F2} ms/frame\n" +
                    $"推奨: 影オフ/範囲短縮/強度上限1.0", "OK");
            }
        }
        
        #endregion
    }
}
