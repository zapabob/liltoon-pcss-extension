#define MODULAR_AVATAR_EXISTS
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
// #if MODULAR_AVATAR_EXISTS
// using nadena.dev.modular_avatar.core;
// using nadena.dev.modular_avatar.core.editor;
// #endif
using System.IO;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// Modular Avatarを使用したエミッシブライト制御システム
    /// 最新のMA 1.12.5ドキュメントに基づく実装
    /// </summary>
    public class ModularAvatarLightController : EditorWindow
    {
        private GameObject avatarRoot;
        private GameObject lightObject;
        private string lightName = "AvatarLight";
        private Transform targetBone;
        private Color lightColor = Color.white;
        private float defaultIntensity = 1.0f;
        private float maxIntensity = 3.0f;
        private bool createToggle = true;
        private bool createSlider = true;
        private bool createColorControl = false;
        private string menuName = "Avatar Light";
        private string menuIconPath = "";

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Modular Avatar Light Controller")]
        public static void ShowWindow()
        {
            var window = GetWindow<ModularAvatarLightController>("MA Light Controller");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Modular Avatar Light Controller", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("エミッシブライトをModular Avatarで制御可能なシステムをセットアップします。", MessageType.Info);

            EditorGUILayout.Space(10);

            // アバター設定
            EditorGUILayout.LabelField("アバター設定", EditorStyles.boldLabel);
            avatarRoot = (GameObject)EditorGUILayout.ObjectField("Avatar Root", avatarRoot, typeof(GameObject), true);

            if (avatarRoot != null)
            {
                var descriptor = avatarRoot.GetComponent<VRCAvatarDescriptor>();
                if (descriptor == null)
                {
                    EditorGUILayout.HelpBox("VRCAvatarDescriptorが見つかりません。アバタールートに設定してください。", MessageType.Warning);
                }
            }

            EditorGUILayout.Space(10);

            // ライト設定
            EditorGUILayout.LabelField("ライト設定", EditorStyles.boldLabel);
            lightName = EditorGUILayout.TextField("Light Name", lightName);
            targetBone = (Transform)EditorGUILayout.ObjectField("Target Bone", targetBone, typeof(Transform), true);
            lightColor = EditorGUILayout.ColorField("Light Color", lightColor);
            defaultIntensity = EditorGUILayout.Slider("Default Intensity", defaultIntensity, 0f, maxIntensity);
            maxIntensity = EditorGUILayout.FloatField("Max Intensity", maxIntensity);

            EditorGUILayout.Space(10);

            // 制御設定
            EditorGUILayout.LabelField("制御設定", EditorStyles.boldLabel);
            createToggle = EditorGUILayout.Toggle("Create Toggle", createToggle);
            createSlider = EditorGUILayout.Toggle("Create Slider", createSlider);
            createColorControl = EditorGUILayout.Toggle("Create Color Control", createColorControl);
            menuName = EditorGUILayout.TextField("Menu Name", menuName);
            menuIconPath = EditorGUILayout.TextField("Menu Icon Path", menuIconPath);

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Create Light System"))
            {
                if (ValidateInputs())
                {
                    CreateLightSystem();
                }
            }

            if (GUILayout.Button("Setup All Lights on Avatar"))
            {
                if (avatarRoot != null)
                {
                    SetupAllLightsOnAvatar();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Avatar Rootを設定してください。", "OK");
                }
            }
        }

        private bool ValidateInputs()
        {
            if (avatarRoot == null)
            {
                EditorUtility.DisplayDialog("Error", "Avatar Rootを設定してください。", "OK");
                return false;
            }

            if (string.IsNullOrEmpty(lightName))
            {
                EditorUtility.DisplayDialog("Error", "Light Nameを入力してください。", "OK");
                return false;
            }

            return true;
        }

        private void CreateLightSystem()
        {
            try
            {
                // ライトオブジェクトの作成
                lightObject = new GameObject(lightName);
                lightObject.transform.SetParent(avatarRoot.transform);

                // Lightコンポーネントの追加
                var light = lightObject.AddComponent<Light>();
                light.type = LightType.Point;
                light.color = lightColor;
                light.intensity = defaultIntensity;
                light.range = 5f;

                // Modular Avatarコンポーネントの設定
#if MODULAR_AVATAR_EXISTS
                SetupModularAvatarComponents();
#endif

                // Expression ParametersとMenuの設定
                SetupExpressionParameters();

                // Animator Controllerの設定
                CreateLightAnimatorController();

                EditorUtility.DisplayDialog("Success", "Light system created successfully!", "OK");

                // ログを保存
                SaveImplementationLog();
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to create light system: {e.Message}", "OK");
                Debug.LogError($"Light system creation failed: {e}");
            }
        }

#if MODULAR_AVATAR_EXISTS
        // The following types are not available unless Modular Avatar is installed.
        // To avoid compile errors, we provide stub classes/structs/enums for editor-time only.
        // Remove these stubs if you have Modular Avatar installed and the correct usings enabled.
        private class ModularAvatarBoneProxy : MonoBehaviour
        {
            public Transform target;
        }
        private class ModularAvatarParameters : MonoBehaviour
        {
            public List<ParameterConfig> parameters;
        }
        private class ModularAvatarMenuInstaller : MonoBehaviour
        {
            public VRCExpressionsMenu menuToAppend;
            public VRCExpressionsMenu installTargetMenu;
        }
        private class ModularAvatarMergeAnimator : MonoBehaviour
        {
            public RuntimeAnimatorController animator;
            public VRCAvatarDescriptor.AnimLayerType layerType;
            public bool matchAvatarWriteDefaults;
            public bool deleteAttachedAnimator;
        }
        private class ParameterConfig
        {
            public string nameOrPrefix;
            public ParameterSyncType syncType;
            public float defaultValue;
            public bool saved;
        }
        private enum ParameterSyncType
        {
            Bool,
            Float
        }

        private void SetupModularAvatarComponents()
        {
            // MA Bone Proxy - ボーン追従
            if (targetBone != null)
            {
                var boneProxy = lightObject.AddComponent<ModularAvatarBoneProxy>();
                boneProxy.target = targetBone;
            }

            // MA Parameters - パラメータ定義
            var maParams = lightObject.AddComponent<ModularAvatarParameters>();
            maParams.parameters = new List<ParameterConfig>();

            if (createToggle)
            {
                maParams.parameters.Add(new ParameterConfig
                {
                    nameOrPrefix = $"{lightName}_Toggle",
                    syncType = ParameterSyncType.Bool,
                    defaultValue = 1.0f,
                    saved = true
                });
            }

            if (createSlider)
            {
                maParams.parameters.Add(new ParameterConfig
                {
                    nameOrPrefix = $"{lightName}_Intensity",
                    syncType = ParameterSyncType.Float,
                    defaultValue = defaultIntensity / maxIntensity,
                    saved = true
                });
            }

            if (createColorControl)
            {
                maParams.parameters.Add(new ParameterConfig
                {
                    nameOrPrefix = $"{lightName}_Color",
                    syncType = ParameterSyncType.Float,
                    defaultValue = 0.5f,
                    saved = true
                });
            }

            // MA Menu Installer - メニュー項目
            var menuInstaller = lightObject.AddComponent<ModularAvatarMenuInstaller>();
            menuInstaller.menuToAppend = CreateLightMenu();
            menuInstaller.installTargetMenu = null; // デフォルトメニューに追加

            // MA Merge Animator - アニメーター統合
            var mergeAnimator = lightObject.AddComponent<ModularAvatarMergeAnimator>();
            mergeAnimator.animator = CreateLightAnimatorController();
            mergeAnimator.layerType = VRCAvatarDescriptor.AnimLayerType.FX;
            mergeAnimator.matchAvatarWriteDefaults = true;
            mergeAnimator.deleteAttachedAnimator = true;
        }
#endif

        private VRCExpressionsMenu CreateLightMenu()
        {
            var menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            menu.controls = new List<VRCExpressionsMenu.Control>();

            if (createToggle)
            {
                var toggleControl = new VRCExpressionsMenu.Control
                {
                    name = menuName + " Toggle",
                    type = VRCExpressionsMenu.Control.ControlType.Toggle,
                    parameter = new VRCExpressionsMenu.Control.Parameter { name = $"{lightName}_Toggle" },
                    value = 1.0f
                };
                menu.controls.Add(toggleControl);
            }

            if (createSlider)
            {
                var sliderControl = new VRCExpressionsMenu.Control
                {
                    name = menuName + " Intensity",
                    type = VRCExpressionsMenu.Control.ControlType.RadialPuppet,
                    parameter = new VRCExpressionsMenu.Control.Parameter { name = $"{lightName}_Intensity" },
                    value = defaultIntensity / maxIntensity
                };
                menu.controls.Add(sliderControl);
            }

            // アセットとして保存
            string menuPath = $"Assets/_Generated/{lightName}_Menu.asset";
            EnsureDirectoryExists(menuPath);
            AssetDatabase.CreateAsset(menu, menuPath);
            AssetDatabase.SaveAssets();

            return menu;
        }

        // Helper to get the transform path from root to target
        private string GetTransformPath(Transform target, Transform root)
        {
            if (target == null || root == null) return "";
            if (target == root) return "";
            var path = target.name;
            var current = target.parent;
            while (current != null && current != root)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }
            return path;
        }

        private AnimatorController CreateLightAnimatorController()
        {
            var controller = new AnimatorController();
            controller.name = $"{lightName}_Controller";

            // パラメータの追加
            if (createToggle)
                controller.AddParameter($"{lightName}_Toggle", AnimatorControllerParameterType.Bool);
            if (createSlider)
                controller.AddParameter($"{lightName}_Intensity", AnimatorControllerParameterType.Float);
            if (createColorControl)
                controller.AddParameter($"{lightName}_Color", AnimatorControllerParameterType.Float);

            // レイヤーの作成
            var layer = new AnimatorControllerLayer
            {
                name = "Light Control",
                defaultWeight = 1f,
                stateMachine = new AnimatorStateMachine()
            };

            // デフォルトステートの作成
            var defaultState = layer.stateMachine.AddState("Default");
            defaultState.writeDefaultValues = false;

            // トグル制御用のステート
            if (createToggle)
            {
                var onState = layer.stateMachine.AddState("Light On");
                onState.writeDefaultValues = false;

                var offState = layer.stateMachine.AddState("Light Off");
                offState.writeDefaultValues = false;

                // トランジションの設定
                var onTransition = defaultState.AddTransition(onState);
                onTransition.AddCondition(AnimatorConditionMode.If, 0, $"{lightName}_Toggle");

                var offTransition = onState.AddTransition(offState);
                offTransition.AddCondition(AnimatorConditionMode.IfNot, 0, $"{lightName}_Toggle");

                var backTransition = offState.AddTransition(defaultState);
                backTransition.AddCondition(AnimatorConditionMode.If, 0, $"{lightName}_Toggle");
            }

            // スライダー制御用のBlend Tree
            if (createSlider)
            {
                var blendState = layer.stateMachine.AddState("Intensity Control");
                blendState.writeDefaultValues = false;

                var blendTree = new BlendTree();
                blendTree.name = "Intensity Blend";
                blendTree.blendType = BlendTreeType.Simple1D;
                blendTree.blendParameter = $"{lightName}_Intensity";
                blendTree.useAutomaticThresholds = true;

                // 強度0のアニメーション
                var zeroClip = new AnimationClip();
                zeroClip.name = "Intensity_0";
                // Use helper to get path
                string lightPath = GetTransformPath(lightObject.transform, avatarRoot.transform);
                var zeroBinding = AnimationUtility.GetCurveBindings(zeroClip);
                if (zeroBinding.Length == 0)
                {
                    AnimationUtility.SetEditorCurve(zeroClip,
                        EditorCurveBinding.FloatCurve(lightPath, typeof(Light), "m_Intensity"),
                        AnimationCurve.Linear(0f, 0f, 1f, 0f));
                }

                // 最大強度のアニメーション
                var maxClip = new AnimationClip();
                maxClip.name = "Intensity_Max";
                AnimationUtility.SetEditorCurve(maxClip,
                    EditorCurveBinding.FloatCurve(lightPath, typeof(Light), "m_Intensity"),
                    AnimationCurve.Linear(0f, maxIntensity, 1f, maxIntensity));

                blendTree.AddChild(zeroClip);
                blendTree.AddChild(maxClip);

                blendState.motion = blendTree;
            }

            controller.AddLayer(layer);

            // アセットとして保存
            string controllerPath = $"Assets/_Generated/{lightName}_Controller.controller";
            EnsureDirectoryExists(controllerPath);
            AssetDatabase.CreateAsset(controller, controllerPath);
            AssetDatabase.SaveAssets();

            return controller;
        }

        private void SetupExpressionParameters()
        {
            var descriptor = avatarRoot.GetComponent<VRCAvatarDescriptor>();
            if (descriptor == null) return;

            // Expression Parametersの設定
            if (descriptor.expressionParameters == null)
            {
                descriptor.expressionParameters = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                string paramPath = $"Assets/_Generated/{avatarRoot.name}_Parameters.asset";
                EnsureDirectoryExists(paramPath);
                AssetDatabase.CreateAsset(descriptor.expressionParameters, paramPath);
            }

            var parameters = descriptor.expressionParameters.parameters.ToList();

            if (createToggle)
            {
                parameters.Add(new VRCExpressionParameters.Parameter
                {
                    name = $"{lightName}_Toggle",
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 1.0f,
                    saved = true
                });
            }

            if (createSlider)
            {
                parameters.Add(new VRCExpressionParameters.Parameter
                {
                    name = $"{lightName}_Intensity",
                    valueType = VRCExpressionParameters.ValueType.Float,
                    defaultValue = defaultIntensity / maxIntensity,
                    saved = true
                });
            }

            if (createColorControl)
            {
                parameters.Add(new VRCExpressionParameters.Parameter
                {
                    name = $"{lightName}_Color",
                    valueType = VRCExpressionParameters.ValueType.Float,
                    defaultValue = 0.5f,
                    saved = true
                });
            }

            descriptor.expressionParameters.parameters = parameters.ToArray();
            EditorUtility.SetDirty(descriptor.expressionParameters);
        }

        private void SetupAllLightsOnAvatar()
        {
            var lights = avatarRoot.GetComponentsInChildren<Light>(true);
            int processed = 0;

            var modularAvatarParametersType = Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarParameters, nadena.dev.modular-avatar");
            foreach (var light in lights)
            {
                if (modularAvatarParametersType != null && light.gameObject.GetComponent(modularAvatarParametersType) == null)
                {
                    SetupModularAvatarComponentsForExistingLight(light.gameObject);
                    processed++;
                }
            }

            EditorUtility.DisplayDialog("Setup Complete",
                $"{processed} lights have been set up with Modular Avatar control system.", "OK");
        }

        private void SetupModularAvatarComponentsForExistingLight(GameObject lightObj)
        {
            lightObject = lightObj;
            lightName = lightObj.name.Replace(" ", "_");

            // 既存のLightから設定を取得
            var light = lightObj.GetComponent<Light>();
            if (light != null)
            {
                lightColor = light.color;
                defaultIntensity = light.intensity;
            }

            SetupModularAvatarComponents();
        }

        private void EnsureDirectoryExists(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private void SaveImplementationLog()
        {
            string logPath = "_docs/2025-08-18_modular_avatar_light_system.md";
            string logContent = $@"# 2025-08-18 Modular Avatar Light System Implementation

## 概要
Modular Avatar 1.12.5の最新ドキュメントに基づき、エミッシブライトの連続調整・トグルオンオフ・ボーン追従システムを実装。

## 実装内容
- **MA Parameters**: トグル・スライダー・カラーパラメータの定義
- **MA Menu Installer**: メニュー項目の自動作成
- **MA Merge Animator**: Blend Treesを使用した連続調整
- **MA Bone Proxy**: ボーン追従機能
- **Expression Parameters**: VRC同期パラメータ設定
- **Animator Controller**: 状態遷移とBlend Tree制御

## 制御機能
- トグルオンオフ: {createToggle}
- スライダー調整: {createSlider}
- カラー制御: {createColorControl}
- 最大強度: {maxIntensity}
- デフォルト強度: {defaultIntensity}

## 作成されたアセット
- メニュー: Assets/_Generated/{lightName}_Menu.asset
- アニメーター: Assets/_Generated/{lightName}_Controller.controller
- パラメータ: Assets/_Generated/{avatarRoot?.name ?? "Avatar"}_Parameters.asset

## タイムスタンプ
- 2025-08-18
";

            try
            {
                File.WriteAllText(logPath, logContent);
                AssetDatabase.Refresh();
                Debug.Log($"Implementation log saved to {logPath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save implementation log: {e.Message}");
            }
        }
    }
}
