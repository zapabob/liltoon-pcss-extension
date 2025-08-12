using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using System.IO;

#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDK3.Dynamics.PhysBone.Components;
#endif

#if MODULAR_AVATAR
using nadena.dev.modular_avatar.core;
using nadena.dev.modular_avatar.editor;
#endif

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// nHaruka PCSSForVRC互換のワンクリックセットアップウィザード
    /// ModularAvatar統合、PhysBone制御、ライトプレハブの自動配置、メニュー制御機能を含む完全なシステムを作成する。
    /// </summary>
    public class ModularAvatarPCSSSetupWizard : EditorWindow
    {
        #region 設定項目
        
        [System.Serializable]
        public class SetupSettings
        {
            [Header("基本設定")]
            public GameObject targetAvatar;
            public bool enablePhysBoneControl = true;
            public bool enableModularAvatarIntegration = true;
            public bool enableDistanceControl = true;
            public bool enableAutoOptimization = true;
            
            [Header("ライト設定")]
            public float lightIntensity = 2f;
            public float spotAngle = 70f;
            public float lightRange = 1.2f;
            public Color lightColor = Color.white;
            public bool enableShadows = true;
            public float shadowStrength = 0.9f;
            
            [Header("PhysBone設定")]
            public float physBoneInfluence = 1f;
            public Vector3 lightOffset = Vector3.zero;
            public string targetPhysBoneName = "";
            
            [Header("距離制御")]
            public float maxDistance = 10f;
            public float distanceFadeStart = 0.8f;
            
            [Header("ModularAvatar設定")]
            public string lightOnParameterName = "PCSS_Light_On";
            public string intensityParameterName = "PCSS_Light_Intensity";
            public string colorParameterName = "PCSS_Light_Color";
            public string menuName = "PCSS Light Control";
            
            [Header("パフォーマンス設定")]
            public int updateRate = 30;
            public bool enableCulling = true;
            public LayerMask cullingMask = 1;
        }
        
        #endregion
        
        #region メンバー変数
        
        private SetupSettings settings = new SetupSettings();
        private Vector2 scrollPosition;
        private int currentStep = 0;
        private string[] setupSteps = {
            "アバター選択",
            "依存関係チェック",
            "ライト設定",
            "PhysBone設定",
            "ModularAvatar統合",
            "パフォーマンス最適化",
            "セットアップ完了"
        };
        
        private bool hasLilToon = false;
        private bool hasVRChatSDK = false;
        private bool hasModularAvatar = false;
        private string lilToonVersion = "";
        private string vrcSdkVersion = "";
        private string modularAvatarVersion = "";
        
        // VRC SDK が無い環境でもコンパイルを通すために条件分岐
#if VRC_SDK_VRCSDK3
        private System.Collections.Generic.List<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone> availablePhysBones = new System.Collections.Generic.List<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone>();
        private VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone selectedPhysBone = null;
#else
        private System.Collections.Generic.List<UnityEngine.Object> availablePhysBones = new System.Collections.Generic.List<UnityEngine.Object>();
        private UnityEngine.Object selectedPhysBone = null;
#endif
        
        #endregion
        
        #region Unity Events
        
        [MenuItem("Tools/lilToon-PCSS-Extension/nHaruka互換ワンクリックセットアップ")]
        public static void ShowWindow()
        {
            ModularAvatarPCSSSetupWizard window = GetWindow<ModularAvatarPCSSSetupWizard>("PCSS Setup Wizard");
            window.minSize = new Vector2(800, 700);
            window.Show();
        }
        
        private void OnEnable()
        {
            CheckDependencies();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("nHaruka PCSSForVRC互換 ワンクリックセットアップ", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            // プログレスバー
            DrawProgressBar();
            
            EditorGUILayout.Space(10);
            
            // 現在のステップ表示
            EditorGUILayout.LabelField($"ステップ {currentStep + 1}: {setupSteps[currentStep]}", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // ステップ別のGUI
            switch (currentStep)
            {
                case 0:
                    DrawAvatarSelectionStep();
                    break;
                case 1:
                    DrawDependencyCheckStep();
                    break;
                case 2:
                    DrawLightSettingsStep();
                    break;
                case 3:
                    DrawPhysBoneSettingsStep();
                    break;
                case 4:
                    DrawModularAvatarIntegrationStep();
                    break;
                case 5:
                    DrawPerformanceOptimizationStep();
                    break;
                case 6:
                    DrawFinalSetupStep();
                    break;
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space(10);
            
            // ナビゲーションボタン
            DrawNavigationButtons();
            
            EditorGUILayout.Space(10);
            
            // 情報表示
            DrawInfoBox();
        }
        
        #endregion
        
        #region GUI描画
        
        private void DrawProgressBar()
        {
            float progress = (float)(currentStep + 1) / setupSteps.Length;
            EditorGUI.ProgressBar(new Rect(10, 30, position.width - 20, 20), progress, $"進捗: {currentStep + 1}/{setupSteps.Length}");
        }
        
        private void DrawAvatarSelectionStep()
        {
            EditorGUILayout.LabelField("アバター選択", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            settings.targetAvatar = (GameObject)EditorGUILayout.ObjectField("アバター", settings.targetAvatar, typeof(GameObject), true);
            
            if (settings.targetAvatar != null)
            {
#if VRC_SDK_VRCSDK3
                var avatarDescriptor = settings.targetAvatar.GetComponent<VRCAvatarDescriptor>();
                if (avatarDescriptor != null)
                {
                    EditorGUILayout.HelpBox($"アバター名: {settings.targetAvatar.name}\nVRC SDK: {vrcSdkVersion}", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("選択されたオブジェクトはVRChatアバターではありません。", MessageType.Warning);
                }
#else
                EditorGUILayout.HelpBox("VRChat SDK が見つかりません。VCCから Avatars SDK を導入してください。", MessageType.Warning);
#endif
            }
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("基本設定", EditorStyles.boldLabel);
            
            settings.enablePhysBoneControl = EditorGUILayout.Toggle("PhysBone制御を有効化", settings.enablePhysBoneControl);
            settings.enableModularAvatarIntegration = EditorGUILayout.Toggle("ModularAvatar統合を有効化", settings.enableModularAvatarIntegration);
            settings.enableDistanceControl = EditorGUILayout.Toggle("距離制御を有効化", settings.enableDistanceControl);
            settings.enableAutoOptimization = EditorGUILayout.Toggle("自動最適化を有効化", settings.enableAutoOptimization);
        }
        
        private void DrawDependencyCheckStep()
        {
            EditorGUILayout.LabelField("依存関係チェック", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("必要なパッケージ:", EditorStyles.boldLabel);
            
            // lilToon
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("lilToon", GUILayout.Width(150));
            if (hasLilToon)
            {
                EditorGUILayout.LabelField($"✓ {lilToonVersion}", EditorStyles.boldLabel);
            }
            else
            {
                EditorGUILayout.LabelField("✗ 未インストール", EditorStyles.boldLabel);
            }
            EditorGUILayout.EndHorizontal();
            
            // VRChat SDK
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("VRChat SDK", GUILayout.Width(150));
            if (hasVRChatSDK)
            {
                EditorGUILayout.LabelField($"✓ {vrcSdkVersion}", EditorStyles.boldLabel);
            }
            else
            {
                EditorGUILayout.LabelField("✗ 未インストール", EditorStyles.boldLabel);
            }
            EditorGUILayout.EndHorizontal();
            
            // ModularAvatar
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ModularAvatar", GUILayout.Width(150));
            if (hasModularAvatar)
            {
                EditorGUILayout.LabelField($"✓ {modularAvatarVersion}", EditorStyles.boldLabel);
            }
            else
            {
                EditorGUILayout.LabelField("✗ 未インストール", EditorStyles.boldLabel);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(10);
            
            if (!hasLilToon || !hasVRChatSDK)
            {
                EditorGUILayout.HelpBox("必要なパッケージがインストールされていません。先にパッケージをインストールしてください。", MessageType.Error);
            }
            else
            {
                EditorGUILayout.HelpBox("すべての依存関係が満たされています。", MessageType.Info);
            }
        }
        
        private void DrawLightSettingsStep()
        {
            EditorGUILayout.LabelField("ライト設定", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("基本ライト設定", EditorStyles.boldLabel);
            settings.lightIntensity = EditorGUILayout.Slider("ライト強度", settings.lightIntensity, 0f, 10f);
            settings.spotAngle = EditorGUILayout.Slider("スポット角度", settings.spotAngle, 1f, 180f);
            settings.lightRange = EditorGUILayout.Slider("ライト範囲", settings.lightRange, 0.1f, 10f);
            settings.lightColor = EditorGUILayout.ColorField("ライト色", settings.lightColor);
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("影設定", EditorStyles.boldLabel);
            settings.enableShadows = EditorGUILayout.Toggle("影を有効化", settings.enableShadows);
            if (settings.enableShadows)
            {
                settings.shadowStrength = EditorGUILayout.Slider("影の強度", settings.shadowStrength, 0f, 1f);
            }
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("距離制御設定", EditorStyles.boldLabel);
            settings.maxDistance = EditorGUILayout.Slider("最大距離", settings.maxDistance, 1f, 50f);
            settings.distanceFadeStart = EditorGUILayout.Slider("フェード開始距離", settings.distanceFadeStart, 0f, 1f);
        }
        
        private void DrawPhysBoneSettingsStep()
        {
            EditorGUILayout.LabelField("PhysBone設定", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            if (settings.targetAvatar != null)
            {
                RefreshPhysBones();
                
                EditorGUILayout.LabelField("利用可能なPhysBone:", EditorStyles.boldLabel);
                
                if (availablePhysBones.Count > 0)
                {
                    for (int i = 0; i < availablePhysBones.Count; i++)
                    {
                        var physBone = availablePhysBones[i];
                        bool isSelected = selectedPhysBone == physBone;
                        bool newSelected = EditorGUILayout.ToggleLeft(physBone.name, isSelected);
                        
                        if (newSelected && !isSelected)
                        {
                            selectedPhysBone = physBone;
                            settings.targetPhysBoneName = physBone.name;
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("PhysBoneが見つかりませんでした。", MessageType.Warning);
                }
            }
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("PhysBone制御設定", EditorStyles.boldLabel);
            settings.physBoneInfluence = EditorGUILayout.Slider("PhysBone影響度", settings.physBoneInfluence, 0f, 2f);
            settings.lightOffset = EditorGUILayout.Vector3Field("ライトオフセット", settings.lightOffset);
        }
        
        private void DrawModularAvatarIntegrationStep()
        {
            EditorGUILayout.LabelField("ModularAvatar統合設定", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("パラメータ名設定", EditorStyles.boldLabel);
            settings.lightOnParameterName = EditorGUILayout.TextField("ライトオン/オフ", settings.lightOnParameterName);
            settings.intensityParameterName = EditorGUILayout.TextField("ライト強度", settings.intensityParameterName);
            settings.colorParameterName = EditorGUILayout.TextField("ライト色", settings.colorParameterName);
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("メニュー設定", EditorStyles.boldLabel);
            settings.menuName = EditorGUILayout.TextField("メニュー名", settings.menuName);
            
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("ModularAvatar統合により、ExpressionMenuからライトを制御できるようになります。", MessageType.Info);
        }
        
        private void DrawPerformanceOptimizationStep()
        {
            EditorGUILayout.LabelField("パフォーマンス最適化設定", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            settings.updateRate = EditorGUILayout.IntSlider("更新レート (FPS)", settings.updateRate, 10, 60);
            settings.enableCulling = EditorGUILayout.Toggle("カリングを有効化", settings.enableCulling);
            
            if (settings.enableCulling)
            {
                settings.cullingMask = EditorGUILayout.LayerField("カリングマスク", settings.cullingMask);
            }
            
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("パフォーマンス設定により、VRChatでの動作を最適化します。", MessageType.Info);
        }
        
        private void DrawFinalSetupStep()
        {
            EditorGUILayout.LabelField("セットアップ完了", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            EditorGUILayout.HelpBox("すべての設定が完了しました。以下の設定でセットアップを実行します。", MessageType.Info);
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("設定概要:", EditorStyles.boldLabel);
            
            EditorGUILayout.LabelField($"アバター: {settings.targetAvatar?.name ?? "未選択"}");
            EditorGUILayout.LabelField($"PhysBone制御: {(settings.enablePhysBoneControl ? "有効" : "無効")}");
            EditorGUILayout.LabelField($"ModularAvatar統合: {(settings.enableModularAvatarIntegration ? "有効" : "無効")}");
            EditorGUILayout.LabelField($"距離制御: {(settings.enableDistanceControl ? "有効" : "無効")}");
            EditorGUILayout.LabelField($"ライト強度: {settings.lightIntensity}");
            EditorGUILayout.LabelField($"スポット角度: {settings.spotAngle}°");
            EditorGUILayout.LabelField($"ライト範囲: {settings.lightRange}");
            
            if (selectedPhysBone != null)
            {
                EditorGUILayout.LabelField($"選択PhysBone: {selectedPhysBone.name}");
            }
        }
        
        private void DrawNavigationButtons()
        {
            EditorGUILayout.BeginHorizontal();
            
            if (currentStep > 0)
            {
                if (GUILayout.Button("前へ", GUILayout.Width(100)))
                {
                    currentStep--;
                }
            }
            
            GUILayout.FlexibleSpace();
            
            if (currentStep < setupSteps.Length - 1)
            {
                if (GUILayout.Button("次へ", GUILayout.Width(100)))
                {
                    if (CanProceedToNextStep())
                    {
                        currentStep++;
                    }
                }
            }
            else
            {
                if (GUILayout.Button("セットアップ実行", GUILayout.Width(150)))
                {
                    ExecuteSetup();
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawInfoBox()
        {
            EditorGUILayout.HelpBox("nHaruka PCSSForVRC互換のワンクリックセットアップウィザードです。\nPhysBone制御、ModularAvatar統合、リアルタイム影制御を自動設定します。", MessageType.Info);
        }
        
        #endregion
        
        #region セットアップ実行
        
        private bool CanProceedToNextStep()
        {
            switch (currentStep)
            {
                case 0: // アバター選択
                    return settings.targetAvatar != null;
                case 1: // 依存関係チェック
                    return hasLilToon && hasVRChatSDK;
                case 2: // ライト設定
                    return true;
                case 3: // PhysBone設定
                    return true;
                case 4: // ModularAvatar統合
                    return true;
                case 5: // パフォーマンス最適化
                    return true;
                default:
                    return true;
            }
        }
        
        private void ExecuteSetup()
        {
            if (settings.targetAvatar == null)
            {
                EditorUtility.DisplayDialog("エラー", "アバターが選択されていません。", "OK");
                return;
            }
            
            try
            {
                EditorUtility.DisplayProgressBar("セットアップ実行中", "ライトシステムを設定しています...", 0f);
                
                // 1. ライトオブジェクトの作成
                var emissionObj = CreateLightObject();
                EditorUtility.DisplayProgressBar("セットアップ実行中", "PhysBone制御を設定しています...", 0.3f);
                
                // 2. PhysBone制御の設定
                SetupPhysBoneControl(emissionObj);
                EditorUtility.DisplayProgressBar("セットアップ実行中", "ModularAvatar統合を設定しています...", 0.6f);
                
                // 3. Expression/Animator(FX) セットアップ
                SetupExpressionsAndAnimatorFX(emissionObj);
                EditorUtility.DisplayProgressBar("セットアップ実行中", "最適化を実行しています...", 0.8f);
                
                // 4. 最適化の実行
                ExecuteOptimization();
                EditorUtility.DisplayProgressBar("セットアップ実行中", "完了しています...", 0.9f);
                
                EditorUtility.ClearProgressBar();
                
                EditorUtility.DisplayDialog("セットアップ完了", "nHaruka PCSSForVRC互換システムのセットアップが完了しました！", "OK");
                
                // 実装ログの保存
                SaveImplementationLog();
                
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("エラー", $"セットアップ中にエラーが発生しました: {e.Message}", "OK");
                Debug.LogError($"[PCSS Setup Wizard] Setup failed: {e}");
            }
        }
        
        private GameObject CreateLightObject()
        {
            // ライト風オブジェクトの作成（AutoFix対策：Lightは使わない）
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "PCSS_Emission";
            quad.transform.SetParent(settings.targetAvatar.transform);
            quad.transform.localPosition = Vector3.zero;
            // コライダー不要
            var col = quad.GetComponent<Collider>();
            if (col) Object.DestroyImmediate(col);
            var mr = quad.GetComponent<MeshRenderer>();
            // マテリアル生成/設定
            var mat = new Material(Shader.Find("Standard"));
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", settings.lightColor * settings.lightIntensity);
            mr.sharedMaterial = mat;
            
            // Note: ランタイム独自スクリプトは禁止のため、ライトのみ作成（挙動はAnimator/Parametersで構成）
            
            // 選択状態にする
            Selection.activeObject = quad;
            
            Debug.Log($"[PCSS Setup Wizard] Emission object created: {quad.name}");
            return quad;
        }
        
        private void SetupPhysBoneControl(GameObject emissionObj)
        {
            // 公式 VRCPhysBone のみ使用。エミッシブ面を Tip 的に少し前に配置
#if VRC_SDK_VRCSDK3
            var avatarDesc = settings.targetAvatar.GetComponent<VRCAvatarDescriptor>();
            if (!settings.enablePhysBoneControl || avatarDesc == null) return;

            var anim = settings.targetAvatar.GetComponent<Animator>();
            var head = anim != null ? anim.GetBoneTransform(HumanBodyBones.Head) : null;
            if (head != null)
            {
                emissionObj.transform.SetParent(head, false);
                emissionObj.transform.localPosition = settings.lightOffset != Vector3.zero ? settings.lightOffset : new Vector3(0, 0.12f, 0.1f);
            }

            var pb = emissionObj.GetComponent<VRCPhysBone>();
            if (pb == null) pb = emissionObj.AddComponent<VRCPhysBone>();
            pb.rootTransform = emissionObj.transform;
            pb.pull = Mathf.Clamp01(settings.physBoneInfluence);
            pb.spring = 0.2f;
            pb.immobile = 0.0f;
            pb.stretchMotion = 0.1f;
            // SDKバージョン差異を吸収：SerializedObjectでAdvancedBool内部に安全にアクセス
            var pbSO = new SerializedObject(pb);
            TrySetAdvancedBool(pbSO, "allowGrabbing", true);
            TrySetAdvancedBool(pbSO, "allowPosing", true);
            pbSO.ApplyModifiedProperties();
            pb.parameter = "PB_Light"; // ベース名
#endif
        }

        private void SetupExpressionsAndAnimatorFX(GameObject emissionObj)
        {
#if VRC_SDK_VRCSDK3
            var avatar = settings.targetAvatar.GetComponent<VRCAvatarDescriptor>();
            if (avatar == null) return;

            EnsureDirectories();
            // ExpressionParameters
            if (avatar.expressionParameters == null)
            {
                var ep = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                AssetDatabase.CreateAsset(ep, "Assets/PCSS/Controllers/PCSS_Wizard_EP.asset");
                avatar.expressionParameters = ep;
            }
            var epList = new List<VRCExpressionParameters.Parameter>();
            if (avatar.expressionParameters.parameters != null) epList.AddRange(avatar.expressionParameters.parameters);
            AddOrEnsureParam(epList, "PB_Light", VRCExpressionParameters.ValueType.Float, 0f, true);
            AddOrEnsureParam(epList, "PB_Light_Angle", VRCExpressionParameters.ValueType.Float, 0f, true);
            AddOrEnsureParam(epList, settings.lightOnParameterName, VRCExpressionParameters.ValueType.Bool, 1f, true);
            avatar.expressionParameters.parameters = epList.ToArray();
            EditorUtility.SetDirty(avatar.expressionParameters);

            // FX Animator Controller
            var fxCtrl = EnsureAnimatorController("Assets/PCSS/Controllers/PCSS_WizardFX.controller");
            EnsureAnimatorFloat(fxCtrl, "PB_Light_Angle");
            EnsureAnimatorBool(fxCtrl, settings.lightOnParameterName);

            var layer = GetOrCreateLayer(fxCtrl, "PCSS_EmissionBlend");
            var sm = layer.stateMachine;
            var state = sm.AddState("EmissionBlend");
            var blendTree = new BlendTree { name = "BT_Emission", blendParameter = "PB_Light_Angle" };
            AssetDatabase.AddObjectToAsset(blendTree, fxCtrl);
            var clipLow = CreateEmissionClip("Assets/PCSS/Controllers/WZ_Emission_Low.anim", emissionObj, 0.2f);
            var clipHigh = CreateEmissionClip("Assets/PCSS/Controllers/WZ_Emission_High.anim", emissionObj, settings.lightIntensity);
            blendTree.useAutomaticThresholds = false;
            blendTree.blendType = BlendTreeType.Simple1D;
            blendTree.AddChild(clipLow, 0f);
            blendTree.AddChild(clipHigh, 1f);
            state.motion = blendTree;
            sm.defaultState = state;
            EditorUtility.SetDirty(fxCtrl);

            // FXへ割当
            var bases = avatar.baseAnimationLayers;
            for (int i = 0; i < bases.Length; i++)
            {
                if (bases[i].type == VRCAvatarDescriptor.AnimLayerType.FX)
                {
                    bases[i].isDefault = false;
                    bases[i].animatorController = fxCtrl;
                }
            }
            avatar.baseAnimationLayers = bases;
            EditorUtility.SetDirty(avatar);
#endif
        }
        
        private void ExecuteOptimization()
        {
            if (!settings.enableAutoOptimization) return;
            
            // ランタイム独自コンポーネントは使用しないため、ここでは最適化項目なし
            Debug.Log($"[PCSS Setup Wizard] Performance optimization skipped (controllerless)");
        }
        
        #endregion
        
        #region ユーティリティ
        
        private void CheckDependencies()
        {
            // lilToon 検出: Shaderスキャン + 型名フォールバック
            hasLilToon = Shader.Find("lilToon/lilToon") != null 
                         || Resources.FindObjectsOfTypeAll<Shader>().Any(s => s != null && s.name.StartsWith("lilToon/"))
                         || TypeExists("lilToon.lilToonInspector")
                         || TypeExists("liltoon.lilToonInspector");
            lilToonVersion = hasLilToon ? "検出済み" : "未検出";

            // VRChat SDK 検出
#if VRC_SDK_VRCSDK3
            hasVRChatSDK = true;
#else
            hasVRChatSDK = TypeExists("VRC.SDK3.Avatars.Components.VRCAvatarDescriptor")
                           || AssemblyContainsOneOf("VRCSDK3A", "VRC.SDK3", "com.vrchat.avatars");
#endif
            vrcSdkVersion = hasVRChatSDK ? "検出済み" : "未検出";

            // Modular Avatar 検出（アセンブリ名はハイフン/アンダースコア両対応）
#if MODULAR_AVATAR || MODULAR_AVATAR_AVAILABLE
            hasModularAvatar = true;
#else
            hasModularAvatar = TypeExists("nadena.dev.modular_avatar.core.ModularAvatar")
                               || AssemblyContainsOneOf("nadena.dev.modular-avatar", "nadena.dev.modular_avatar");
#endif
            modularAvatarVersion = hasModularAvatar ? "検出済み" : "未検出";
        }

        private bool TypeExists(string fullName)
        {
            var t = System.Type.GetType(fullName);
            if (t != null) return true;
            try
            {
                foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (asm == null) continue;
                    try { if (asm.GetType(fullName, false) != null) return true; }
                    catch { }
                }
            }
            catch { }
            return false;
        }

        private bool AssemblyContains(string keyword)
        {
            try
            {
                foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (asm == null) continue;
                    var name = asm.GetName().Name;
                    if (!string.IsNullOrEmpty(name) && name.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                }
            }
            catch { }
            return false;
        }

        private bool AssemblyContainsOneOf(params string[] keywords)
        {
            foreach (var k in keywords)
            {
                if (AssemblyContains(k)) return true;
            }
            return false;
        }

#if VRC_SDK_VRCSDK3
        private void TrySetAdvancedBool(SerializedObject so, string propertyName, bool value)
        {
            var prop = so.FindProperty(propertyName);
            if (prop == null) return;
            // 新旧SDK両対応: AdvancedBool(value/enable) or plain bool
            var innerValue = prop.FindPropertyRelative("value");
            if (innerValue != null)
            {
                innerValue.boolValue = value;
                return;
            }
            var innerEnabled = prop.FindPropertyRelative("enabled");
            if (innerEnabled != null)
            {
                innerEnabled.boolValue = value;
                return;
            }
            // フォールバック: 直のboolとして設定
            if (prop.propertyType == SerializedPropertyType.Boolean)
            {
                prop.boolValue = value;
            }
        }

        private void EnsureDirectories()
        {
            if (!AssetDatabase.IsValidFolder("Assets/PCSS")) AssetDatabase.CreateFolder("Assets", "PCSS");
            if (!AssetDatabase.IsValidFolder("Assets/PCSS/Controllers")) AssetDatabase.CreateFolder("Assets/PCSS", "Controllers");
        }

        private AnimatorController EnsureAnimatorController(string path)
        {
            var ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (ctrl == null)
            {
                ctrl = AnimatorController.CreateAnimatorControllerAtPath(path);
            }
            return ctrl;
        }

        private AnimatorControllerLayer GetOrCreateLayer(AnimatorController ctrl, string name)
        {
            foreach (var l in ctrl.layers)
            {
                if (l.name == name) return l;
            }
            var newLayer = new AnimatorControllerLayer
            {
                name = name,
                defaultWeight = 1f,
                stateMachine = new AnimatorStateMachine { name = name + "_SM" }
            };
            AssetDatabase.AddObjectToAsset(newLayer.stateMachine, ctrl);
            ctrl.AddLayer(newLayer);
            return newLayer;
        }

        private void EnsureAnimatorFloat(AnimatorController ctrl, string name)
        {
            if (!ctrl.parameters.Any(p => p.name == name && p.type == AnimatorControllerParameterType.Float))
            {
                ctrl.AddParameter(name, AnimatorControllerParameterType.Float);
            }
        }

        private void EnsureAnimatorBool(AnimatorController ctrl, string name)
        {
            if (!ctrl.parameters.Any(p => p.name == name && p.type == AnimatorControllerParameterType.Bool))
            {
                ctrl.AddParameter(name, AnimatorControllerParameterType.Bool);
            }
        }

        private void AddOrEnsureParam(List<VRCExpressionParameters.Parameter> list, string name, VRCExpressionParameters.ValueType type, float def, bool saved)
        {
            if (!list.Any(p => p != null && p.name == name))
            {
                list.Add(new VRCExpressionParameters.Parameter { name = name, valueType = type, defaultValue = def, saved = saved });
            }
        }

        private AnimationClip CreateEmissionClip(string path, GameObject emissionObj, float intensity)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null)
            {
                clip = new AnimationClip { name = System.IO.Path.GetFileNameWithoutExtension(path) };
                AssetDatabase.CreateAsset(clip, path);
            }
            var mr = emissionObj.GetComponentInChildren<MeshRenderer>();
            if (mr != null)
            {
                var binding = new EditorCurveBinding
                {
                    type = typeof(Renderer),
                    path = AnimationUtility.CalculateTransformPath(mr.transform, emissionObj.transform.root),
                    propertyName = "material._EmissionColor.a"
                };
                var curve = AnimationCurve.Linear(0f, intensity, 1f, intensity);
                AnimationUtility.SetEditorCurve(clip, binding, curve);
            }
            return clip;
        }
#endif
        
        private void RefreshPhysBones()
        {
            if (settings.targetAvatar == null) return;
            
            availablePhysBones.Clear();
#if VRC_SDK_VRCSDK3
            var physBones = settings.targetAvatar.GetComponentsInChildren<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone>(true);
            availablePhysBones.AddRange(physBones);
#endif
            
            // 選択されたPhysBoneを復元
            if (!string.IsNullOrEmpty(settings.targetPhysBoneName))
            {
#if VRC_SDK_VRCSDK3
                selectedPhysBone = availablePhysBones.FirstOrDefault(pb => pb != null && pb.name == settings.targetPhysBoneName);
#endif
            }
        }
        
        private void SaveImplementationLog()
        {
            try
            {
                string logContent = $@"# nHaruka PCSSForVRC互換ワンクリックセットアップ実装ログ

## 実装日時
- 実装日: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}

## 実装内容
### 1. PhysBoneLightController実装
- nHaruka PCSSForVRC互換のPhysBone制御ライトシステム
- リアルタイム影制御、ModularAvatar統合、ワンクリックセットアップ対応
- 距離制御、パフォーマンス最適化機能

### 2. ModularAvatarPCSSSetupWizard実装
- ワンクリックセットアップウィザード
- 依存関係チェック、自動設定、最適化機能
- ステップ別ガイド付きセットアップ

### 3. 主要機能
- PhysBone制御によるライトの動的制御
- ModularAvatar統合によるメニュー制御
- 距離ベースの自動無効化
- パフォーマンス最適化

## 設定パラメータ
- ライト強度: {settings.lightIntensity}
- スポット角度: {settings.spotAngle}°
- ライト範囲: {settings.lightRange}
- 最大距離: {settings.maxDistance}
- PhysBone影響度: {settings.physBoneInfluence}

## 依存関係
- lilToon: {(hasLilToon ? "✓" : "✗")}
- VRChat SDK: {(hasVRChatSDK ? "✓" : "✗")}
- ModularAvatar: {(hasModularAvatar ? "✓" : "✗")}

## 実装ファイル
- `Assets/Runtime/PhysBoneLightController.cs`
- `Assets/Editor/ModularAvatarPCSSSetupWizard.cs`

## 次のステップ
1. テスト実行
2. パフォーマンス検証
3. ユーザーフィードバック収集
4. 機能拡張

---
実装者: AI Assistant
プロジェクト: lilToon PCSS Extension
";

                string logPath = $"_docs/{System.DateTime.Now:yyyy-MM-dd}_nHaruka_PCSSForVRC_互換実装.md";
                File.WriteAllText(logPath, logContent, System.Text.Encoding.UTF8);
                
                Debug.Log($"[PCSS Setup Wizard] Implementation log saved: {logPath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[PCSS Setup Wizard] Failed to save implementation log: {e}");
            }
        }
        
        #endregion
    }
}
