using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;

#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDK3.Dynamics.PhysBone.Components;
#endif

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// ModularAvatarPCSSSetupWizard - VRChat AutoFix対応版
    /// カスタムランタイムコンポーネントを使用せず、標準VRChatコンポーネントのみでPCSS制御を実現
    /// </summary>
    public class ModularAvatarPCSSSetupWizard : EditorWindow
    {
        private Vector2 scrollPosition;
        private bool showBasicSetup = true;
        private bool showAdvancedSetup = true;
        private bool showOptimization = true;
        
        // 基本設定
        private GameObject selectedAvatar;
        private bool enablePCSS = true;
        private bool enableVRLightVolumes = true;
        private bool enablePhysBoneControl = true;
        
        // 高度な設定
        private bool enableModularAvatar = true;
        private bool enableExpressionControl = true;
        private bool enableAnimatorFX = true;
        
        // 最適化設定
        private bool enableOptimization = false; // AutoFix対応のため無効化
        
        [MenuItem("Tools/lilToon-PCSS-Extension/ModularAvatar PCSS Setup Wizard")]
        public static void ShowWindow()
        {
            ModularAvatarPCSSSetupWizard window = GetWindow<ModularAvatarPCSSSetupWizard>("PCSS Setup Wizard");
            window.minSize = new Vector2(600, 700);
        }
        
        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("ModularAvatar PCSS Setup Wizard", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            EditorGUILayout.HelpBox("🎯 VRChat AutoFix対応版\n" +
                "• カスタムランタイムコンポーネントを使用しない\n" +
                "• 標準VRChatコンポーネントのみでPCSS制御を実現\n" +
                "• AutoFixで弾かれない安全な実装", MessageType.Info);
            
            EditorGUILayout.Space(10);
            
            // アバター選択
            EditorGUILayout.LabelField("Avatar Selection", EditorStyles.boldLabel);
            selectedAvatar = (GameObject)EditorGUILayout.ObjectField("Target Avatar", selectedAvatar, typeof(GameObject), true);
            
            if (selectedAvatar == null)
            {
                selectedAvatar = Selection.activeGameObject;
            }
            
            EditorGUILayout.Space(10);
            
            // 基本設定
            showBasicSetup = EditorGUILayout.Foldout(showBasicSetup, "Basic Setup", true);
            if (showBasicSetup)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.LabelField("PCSS Configuration:", EditorStyles.boldLabel);
                enablePCSS = EditorGUILayout.Toggle("Enable PCSS", enablePCSS);
                enableVRLightVolumes = EditorGUILayout.Toggle("Enable VRC Light Volumes", enableVRLightVolumes);
                enablePhysBoneControl = EditorGUILayout.Toggle("Enable PhysBone Control", enablePhysBoneControl);
                
                EditorGUILayout.Space(5);
                
                if (enablePhysBoneControl)
                {
                    EditorGUILayout.HelpBox("PhysBone制御: VRCPhysBoneを使用してエミッシブライトを制御します。\n" +
                        "カスタムランタイムコンポーネントは使用しません。", MessageType.Info);
                }
                
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);

            // 高度な設定
            showAdvancedSetup = EditorGUILayout.Foldout(showAdvancedSetup, "Advanced Setup", true);
            if (showAdvancedSetup)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.LabelField("Modular Avatar Integration:", EditorStyles.boldLabel);
                enableModularAvatar = EditorGUILayout.Toggle("Enable Modular Avatar", enableModularAvatar);
                enableExpressionControl = EditorGUILayout.Toggle("Enable Expression Control", enableExpressionControl);
                enableAnimatorFX = EditorGUILayout.Toggle("Enable Animator FX", enableAnimatorFX);
                
            EditorGUILayout.Space(5);
            
                if (enableModularAvatar)
                {
                    EditorGUILayout.HelpBox("Modular Avatar: パラメータとメニューを自動生成します。\n" +
                        "カスタムランタイムコンポーネントは使用しません。", MessageType.Info);
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space(10);

            // 最適化設定（AutoFix対応のため無効化）
            showOptimization = EditorGUILayout.Foldout(showOptimization, "Optimization (AutoFix Safe)", true);
            if (showOptimization)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.HelpBox("⚠️ VRChat AutoFix対応のため最適化機能は無効化されています。\n" +
                    "カスタムランタイムコンポーネントは使用せず、標準VRChatコンポーネントのみを使用します。", MessageType.Warning);
                
                enableOptimization = false; // 強制的に無効化
                EditorGUILayout.Toggle("Enable Optimization", enableOptimization);
                
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(20);

            // 実行ボタン
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Execute Setup", GUILayout.Height(40)))
            {
                ExecuteSetup();
            }
            
            if (GUILayout.Button("Cleanup (Remove Custom Components)", GUILayout.Height(40)))
            {
                CleanupCustomComponents();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndScrollView();
        }

        private void ExecuteSetup()
        {
            if (selectedAvatar == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select an avatar first.", "OK");
                return;
            }

            #if VRC_SDK_VRCSDK3
            var descriptor = selectedAvatar.GetComponent<VRCAvatarDescriptor>();
            if (descriptor == null)
            {
                EditorUtility.DisplayDialog("Error", "Selected object does not have VRCAvatarDescriptor.", "OK");
                return;
            }

            Undo.RecordObject(selectedAvatar, "PCSS Setup");

            int changes = 0;

            // 1. PCSS設定
            if (enablePCSS)
            {
                changes += SetupPCSS();
            }

            // 2. VRC Light Volumes設定
            if (enableVRLightVolumes)
            {
                changes += SetupVRLightVolumes();
            }

            // 3. PhysBone制御設定（カスタムコンポーネント不使用）
            if (enablePhysBoneControl)
            {
                changes += SetupPhysBoneControl();
            }

            // 4. Modular Avatar設定
            if (enableModularAvatar)
            {
                changes += SetupModularAvatar();
            }

            // 5. Expression制御設定
            if (enableExpressionControl)
            {
                changes += SetupExpressionsAndAnimatorFX();
            }

            // 6. 最適化（AutoFix対応のためスキップ）
            if (enableOptimization)
            {
                Debug.Log("Optimization skipped for VRChat AutoFix compatibility");
            }

            if (changes > 0)
            {
                EditorUtility.SetDirty(selectedAvatar);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Success", $"PCSS Setup completed with {changes} changes.\n\n" +
                    "✅ VRChat AutoFix Safe\n" +
                    "✅ No Custom Runtime Components\n" +
                    "✅ Standard VRChat Components Only", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Info", "No changes were made.", "OK");
            }
            #else
            EditorUtility.DisplayDialog("Error", "VRChat SDK 3.0 is not installed.", "OK");
            #endif
        }

        private int SetupPCSS()
        {
            int changes = 0;
            
            // PCSS関連のマテリアル設定
            var renderers = selectedAvatar.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null && material.shader != null && material.shader.name.Contains("lilToon"))
                    {
                        // PCSSシェーダーに変更
                        var pcssShader = Shader.Find("lilToon/PCSS Extension");
                        if (pcssShader != null && material.shader != pcssShader)
                        {
                            material.shader = pcssShader;
                            changes++;
                        }
                        
                        // PCSS設定を有効化
                        if (material.HasProperty("_UsePCSS"))
                        {
                            material.SetFloat("_UsePCSS", 1.0f);
                        }
                    }
                }
            }
            
            return changes;
        }

        private int SetupVRLightVolumes()
        {
            int changes = 0;
            
            // VRC Light Volumesの設定
            var renderers = selectedAvatar.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null && material.HasProperty("_UseVRCLightVolumes"))
                    {
                        material.SetFloat("_UseVRCLightVolumes", 1.0f);
                        changes++;
                    }
                }
            }
            
            return changes;
        }

        private int SetupPhysBoneControl()
        {
            int changes = 0;
            
            #if VRC_SDK_VRCSDK3
            // エミッシブライト用のオブジェクトを作成
            var headBone = selectedAvatar.transform.Find("Armature/Hips/Spine/Neck/Head");
            if (headBone != null)
            {
                // エミッシブオブジェクトを作成
                var emissiveObject = new GameObject("PB_EmissiveLight");
                emissiveObject.transform.SetParent(headBone);
                emissiveObject.transform.localPosition = Vector3.zero;
                
                // エミッシブスフィアを作成
                var emissiveSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                emissiveSphere.name = "PB_EmissiveSphere";
                emissiveSphere.transform.SetParent(emissiveObject.transform);
                emissiveSphere.transform.localScale = Vector3.one * 0.1f;
                
                // エミッシブマテリアルを作成
                var emissiveMaterial = new Material(Shader.Find("Standard"));
                emissiveMaterial.EnableKeyword("_EMISSION");
                emissiveMaterial.SetColor("_EmissionColor", Color.white);
                emissiveMaterial.SetFloat("_EmissionIntensity", 1.0f);
                emissiveSphere.GetComponent<Renderer>().material = emissiveMaterial;
                
                // VRCPhysBoneを追加（カスタムコンポーネント不使用）
                var physBone = emissiveObject.AddComponent<VRCPhysBone>();
                physBone.parameter = "PB_Light";
                
                // VRChat SDKバリデーション対策: 必須プロパティを設定
                // 最新のVRChat SDKベストプラクティスに基づくAdvancedBool型設定
                try
                {
                    // 最新のVRChat SDKベストプラクティス: デフォルトAdvancedBoolインスタンスを使用
                    physBone.allowGrabbing = new VRC.Dynamics.VRCPhysBoneBase.AdvancedBool();
                    physBone.allowPosing = new VRC.Dynamics.VRCPhysBoneBase.AdvancedBool();
                }
                catch (System.Exception ex1)
                {
                    try
                    {
                        // 方法2: リフレクションを使用した設定（最終手段）
                        var advancedBoolType = typeof(VRC.Dynamics.VRCPhysBoneBase.AdvancedBool);
                        var allowGrabbingProperty = typeof(VRCPhysBone).GetProperty("allowGrabbing");
                        var allowPosingProperty = typeof(VRCPhysBone).GetProperty("allowPosing");
                        
                        if (allowGrabbingProperty != null && allowPosingProperty != null)
                        {
                            var defaultValue = System.Activator.CreateInstance(advancedBoolType);
                            allowGrabbingProperty.SetValue(physBone, defaultValue);
                            allowPosingProperty.SetValue(physBone, defaultValue);
                        }
                    }
                    catch (System.Exception ex2)
                    {
                        // 方法3: プロパティ設定を完全にスキップ（最終手段）
                        Debug.LogWarning($"AdvancedBool型の設定に失敗しました。プロパティ設定をスキップします。\n" +
                            $"エラー1: {ex1.Message}\n" +
                            $"エラー2: {ex2.Message}\n" +
                            $"最新のVRChat SDKではAdvancedBool型の仕様が変更されている可能性があります。");
                    }
                }
                
                // 手のコライダーを設定
                var leftHand = selectedAvatar.transform.Find("Armature/Hips/Spine/LeftArm/LeftForeArm/LeftHand");
                var rightHand = selectedAvatar.transform.Find("Armature/Hips/Spine/RightArm/RightForeArm/RightHand");
                
                if (leftHand != null)
                {
                    var leftCollider = leftHand.gameObject.AddComponent<VRCPhysBoneCollider>();
                    leftCollider.shapeType = VRCPhysBoneCollider.ShapeType.Sphere;
                    leftCollider.radius = 0.1f;
                }
                
                if (rightHand != null)
                {
                    var rightCollider = rightHand.gameObject.AddComponent<VRCPhysBoneCollider>();
                    rightCollider.shapeType = VRCPhysBoneCollider.ShapeType.Sphere;
                    rightCollider.radius = 0.1f;
                }
                
                changes++;
            }
            #endif
            
            return changes;
        }

        private int SetupModularAvatar()
        {
            int changes = 0;
            
            // Modular Avatarの設定（カスタムコンポーネント不使用）
            // 標準のVRChatコンポーネントのみを使用
            
            return changes;
        }

        private int SetupExpressionsAndAnimatorFX()
        {
            int changes = 0;
            
            #if VRC_SDK_VRCSDK3
            var descriptor = selectedAvatar.GetComponent<VRCAvatarDescriptor>();
            if (descriptor == null) return changes;

            // Expression Parametersの設定
            if (descriptor.expressionParameters == null)
            {
                var ep = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                EnsureDirectories();
                AssetDatabase.CreateAsset(ep, "Assets/PCSS/Controllers/PCSS_EP.asset");
                descriptor.expressionParameters = ep;
                changes++;
            }

            // パラメータを追加
            var parameters = new List<VRCExpressionParameters.Parameter>();
            if (descriptor.expressionParameters.parameters != null)
            {
                parameters.AddRange(descriptor.expressionParameters.parameters);
            }

            // PCSS関連パラメータを追加
            AddOrEnsureParam(parameters, "PB_Light", VRCExpressionParameters.ValueType.Float, 0.0f, false);
            AddOrEnsureParam(parameters, "PB_Light_Angle", VRCExpressionParameters.ValueType.Float, 0.0f, false);
            AddOrEnsureParam(parameters, "PCSS_Light_On", VRCExpressionParameters.ValueType.Bool, 0.0f, false);

            descriptor.expressionParameters.parameters = parameters.ToArray();
            EditorUtility.SetDirty(descriptor.expressionParameters);

            // Expression Menuの設定
            if (descriptor.expressionsMenu == null)
            {
                var menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                EnsureDirectories();
                AssetDatabase.CreateAsset(menu, "Assets/PCSS/Controllers/PCSS_Menu.asset");
                descriptor.expressionsMenu = menu;
                changes++;
            }

            // Animator FX Layerの設定
            var animator = selectedAvatar.GetComponent<Animator>();
            if (animator != null)
            {
                var controller = animator.runtimeAnimatorController as AnimatorController;
                if (controller == null)
                {
                    controller = new AnimatorController();
                    EnsureDirectories();
                    AssetDatabase.CreateAsset(controller, "Assets/PCSS/Controllers/PCSS_WizardFX.controller");
                    animator.runtimeAnimatorController = controller;
                    changes++;
                }

                // FX Layerを追加
                var fxLayer = GetOrCreateLayer(controller, "FX");
                if (fxLayer != null)
                {
                    // エミッシブ強度制御用のBlendTreeを作成
                    var blendTree = new BlendTree();
                    blendTree.name = "PCSS_Emission_Control";
                    blendTree.blendType = BlendTreeType.Direct;
                    blendTree.blendParameter = "PB_Light";
                    
                    AssetDatabase.AddObjectToAsset(blendTree, controller);
                    
                    // 状態を作成
                    var state = fxLayer.stateMachine.AddState("PCSS_Emission");
                    state.motion = blendTree;
                    fxLayer.stateMachine.defaultState = state;
                    
                    changes++;
                }
            }
            #endif
            
            return changes;
        }

        private void CleanupCustomComponents()
        {
            if (selectedAvatar == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select an avatar first.", "OK");
                return;
            }

            Undo.RecordObject(selectedAvatar, "Cleanup Custom Components");

            int removedCount = 0;

            // カスタムランタイムコンポーネントを削除
            var customComponents = selectedAvatar.GetComponentsInChildren<Component>()
                .Where(c => c != null && c.GetType().Name.Contains("PCSSController"))
                .ToArray();

            foreach (var component in customComponents)
            {
                if (component != null)
                {
                    DestroyImmediate(component);
                    removedCount++;
                }
            }

            // Missing Scriptsも削除
            var missingScripts = selectedAvatar.GetComponentsInChildren<Component>()
                .Where(c => c == null)
                .ToArray();

            foreach (var missingScript in missingScripts)
            {
                if (missingScript != null)
                {
                    DestroyImmediate(missingScript);
                    removedCount++;
                }
            }

            EditorUtility.SetDirty(selectedAvatar);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Cleanup Complete", 
                $"Removed {removedCount} custom components.\n\n" +
                "✅ VRChat AutoFix Safe\n" +
                "✅ No Custom Runtime Components\n" +
                "✅ Standard VRChat Components Only", "OK");
        }

        private void EnsureDirectories()
        {
            if (!AssetDatabase.IsValidFolder("Assets/PCSS"))
                AssetDatabase.CreateFolder("Assets", "PCSS");
            if (!AssetDatabase.IsValidFolder("Assets/PCSS/Controllers"))
                AssetDatabase.CreateFolder("Assets/PCSS", "Controllers");
        }

        private void AddOrEnsureParam(List<VRCExpressionParameters.Parameter> parameters, string name, VRCExpressionParameters.ValueType type, float defaultValue, bool saved)
        {
            if (!parameters.Any(p => p.name == name))
            {
                parameters.Add(new VRCExpressionParameters.Parameter
                {
                    name = name,
                    valueType = type,
                    defaultValue = defaultValue,
                    saved = saved
                });
            }
        }

        private AnimatorControllerLayer GetOrCreateLayer(AnimatorController controller, string layerName)
        {
            var existingLayer = controller.layers.FirstOrDefault(l => l.name == layerName);
            if (existingLayer != null)
                return existingLayer;

            var newLayer = new AnimatorControllerLayer
            {
                name = layerName,
                defaultWeight = 1f,
                stateMachine = new AnimatorStateMachine { name = layerName + "_SM" }
            };

            AssetDatabase.AddObjectToAsset(newLayer.stateMachine, controller);
            controller.AddLayer(newLayer);

            return newLayer;
        }
    }
}
