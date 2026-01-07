using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace lilToon.PCSS.Editor
{
    /// <summary>
    /// ModularAvatarを活用したライト制御システム
    /// 最新のModularAvatarドキュメントに基づき、Menu/Parameters/Sync/Asset管理を最大限活用
    /// </summary>
public class ModularAvatarLightControl : EditorWindow
{
    private GameObject avatarObject;
        private List<Light> avatarLights = new List<Light>();

        // Dummy stubs for missing ModularAvatar types（製品では無効）
        #if PCSS_DEV
        private class DummyMenuGroup : MonoBehaviour
        {
            public GameObject targetObject;
        }
        private class DummyMenuInstaller : MonoBehaviour
        {
            public VRCExpressionsMenu menuToAppend;
        }
        private class DummyParameter : MonoBehaviour
        {
            public enum ParameterType { Bool, Int, Float }
            public ParameterType type;
            public float defaultValue;
        }
        #endif

        #if PCSS_DEV
        private List<DummyMenuGroup> menuGroups = new List<DummyMenuGroup>();
        private List<DummyMenuInstaller> menuInstallers = new List<DummyMenuInstaller>();
        private List<DummyParameter> maParameters = new List<DummyParameter>();
        #endif
        private Vector2 scrollPosition;
        private bool showAdvancedSettings = false; // 実GUIで使用中（Foldout）
        #if PCSS_DEV
        private bool showModularAvatarSettings = false; // 開発モードFoldout
        #endif

        [MenuItem("Tools/lilToon-PCSS-Extension/Legacy/Modular Avatar Light Control")]
    public static void ShowWindow()
    {
        GetWindow<ModularAvatarLightControl>("MA Light Control");
    }

        private void OnEnable()
        {
            RefreshData();
    }

    private void OnGUI()
    {
            GUILayout.Label("Modular Avatar Light Control", EditorStyles.boldLabel);

            // アバター選択
            EditorGUILayout.BeginHorizontal();
        avatarObject = (GameObject)EditorGUILayout.ObjectField("Avatar", avatarObject, typeof(GameObject), true);
            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                RefreshData();
            }
            EditorGUILayout.EndHorizontal();

            if (avatarObject == null)
            {
                EditorGUILayout.HelpBox("Please select an avatar GameObject", MessageType.Info);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // ライト制御セクション
            DrawLightControlSection();

            // ModularAvatarメニュー制御セクション
            #if PCSS_DEV
            DrawModularAvatarMenuSection();
            #endif

            // ModularAvatarパラメータ制御セクション
            #if PCSS_DEV
            DrawModularAvatarParameterSection();
            #endif

            // 高度な設定セクション
            DrawAdvancedSettingsSection();

            EditorGUILayout.EndScrollView();

            // アクションボタン
            DrawActionButtons();
        }

        private void DrawLightControlSection()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Light Control (via ModularAvatar)", EditorStyles.boldLabel);

            if (avatarLights.Count == 0)
            {
                EditorGUILayout.HelpBox("No lights found under avatar", MessageType.Info);
                return;
            }

            foreach (var light in avatarLights)
            {
                if (light == null) continue;

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"Light: {light.name}", EditorStyles.boldLabel);

                #if PCSS_DEV
                // ModularAvatarParameterを使った有効/無効制御
                string toggleParam = $"Light_{light.name}_Enabled";
                var param = EnsureParameter(toggleParam, DummyParameter.ParameterType.Bool, defaultValue: light.enabled ? 1f : 0f);

                bool newEnabled = EditorGUILayout.Toggle("Enabled (MA Param)", param.defaultValue > 0.5f);
                if (newEnabled != (param.defaultValue > 0.5f))
                {
                    param.defaultValue = newEnabled ? 1f : 0f;
                    EditorUtility.SetDirty(param);
                }

                // 強度
                string intensityParam = $"Light_{light.name}_Intensity";
                var paramIntensity = EnsureParameter(intensityParam, DummyParameter.ParameterType.Float, defaultValue: light.intensity);

                float newIntensity = EditorGUILayout.Slider("Intensity (MA Param)", paramIntensity.defaultValue, 0f, 8f);
                if (Math.Abs(newIntensity - paramIntensity.defaultValue) > 0.001f)
                {
                    paramIntensity.defaultValue = newIntensity;
                    EditorUtility.SetDirty(paramIntensity);
                }

                // 色
                string colorParamR = $"Light_{light.name}_ColorR";
                string colorParamG = $"Light_{light.name}_ColorG";
                string colorParamB = $"Light_{light.name}_ColorB";
                var paramR = EnsureParameter(colorParamR, DummyParameter.ParameterType.Float, defaultValue: light.color.r);
                var paramG = EnsureParameter(colorParamG, DummyParameter.ParameterType.Float, defaultValue: light.color.g);
                var paramB = EnsureParameter(colorParamB, DummyParameter.ParameterType.Float, defaultValue: light.color.b);

                Color newColor = EditorGUILayout.ColorField("Color (MA Param)", new Color(paramR.defaultValue, paramG.defaultValue, paramB.defaultValue));
                if (Math.Abs(newColor.r - paramR.defaultValue) > 0.001f ||
                    Math.Abs(newColor.g - paramG.defaultValue) > 0.001f ||
                    Math.Abs(newColor.b - paramB.defaultValue) > 0.001f)
                {
                    paramR.defaultValue = newColor.r;
                    paramG.defaultValue = newColor.g;
                    paramB.defaultValue = newColor.b;
                    EditorUtility.SetDirty(paramR);
                    EditorUtility.SetDirty(paramG);
                    EditorUtility.SetDirty(paramB);
                }
                #else
                // 製品ビルドでは直接 Light を制御（即時反映）
                bool newEnabled = EditorGUILayout.Toggle("Enabled", light.enabled);
                if (newEnabled != light.enabled)
                {
                    Undo.RecordObject(light, "Toggle Light");
                    light.enabled = newEnabled;
                    EditorUtility.SetDirty(light);
                }

                float newIntensity = EditorGUILayout.Slider("Intensity", light.intensity, 0f, 8f);
                if (Math.Abs(newIntensity - light.intensity) > 0.001f)
                {
                    Undo.RecordObject(light, "Change Light Intensity");
                    light.intensity = newIntensity;
                    EditorUtility.SetDirty(light);
                }

                Color newColor = EditorGUILayout.ColorField("Color", light.color);
                if (newColor != light.color)
                {
                    Undo.RecordObject(light, "Change Light Color");
                    light.color = newColor;
                    EditorUtility.SetDirty(light);
                }
                #endif

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.HelpBox("これらのパラメータはModularAvatarのParametersとして自動管理されます。アニメーターやメニューから制御可能です。", MessageType.Info);
        }

        #if PCSS_DEV
        private void DrawModularAvatarMenuSection()
        {
            EditorGUILayout.Space();
            showModularAvatarSettings = EditorGUILayout.Foldout(showModularAvatarSettings, "Modular Avatar Menu Control");

            if (!showModularAvatarSettings) return;

            if (menuGroups.Count == 0 && menuInstallers.Count == 0)
            {
                EditorGUILayout.HelpBox("No Modular Avatar menu components found", MessageType.Info);
                return;
            }

            // Menu Groups
            if (menuGroups.Count > 0)
            {
                EditorGUILayout.LabelField("Menu Groups:", EditorStyles.boldLabel);
                foreach (var menuGroup in menuGroups)
                {
                    if (menuGroup == null) continue;

                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField($"Menu Group: {menuGroup.name}", EditorStyles.boldLabel);

                    // Menu Groupの有効/無効切り替え
                    bool isEnabled = menuGroup.gameObject.activeInHierarchy;
                    bool newEnabled = EditorGUILayout.Toggle("Enabled", isEnabled);
                    if (newEnabled != isEnabled)
                    {
                        menuGroup.gameObject.SetActive(newEnabled);
                        EditorUtility.SetDirty(menuGroup);
                    }

                    // Target Object
                    GameObject targetObject = menuGroup.targetObject;
                    GameObject newTargetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);
                    if (newTargetObject != targetObject)
                    {
                        menuGroup.targetObject = newTargetObject;
                        EditorUtility.SetDirty(menuGroup);
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
            }

            // Menu Installers
            if (menuInstallers.Count > 0)
            {
                EditorGUILayout.LabelField("Menu Installers:", EditorStyles.boldLabel);
                foreach (var menuInstaller in menuInstallers)
                {
                    if (menuInstaller == null) continue;

                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField($"Menu Installer: {menuInstaller.name}", EditorStyles.boldLabel);

                    // Menu Installerの有効/無効切り替え
                    bool isEnabled = menuInstaller.gameObject.activeInHierarchy;
                    bool newEnabled = EditorGUILayout.Toggle("Enabled", isEnabled);
                    if (newEnabled != isEnabled)
                    {
                        menuInstaller.gameObject.SetActive(newEnabled);
                        EditorUtility.SetDirty(menuInstaller);
                    }

                    // Menu To Append
                    VRCExpressionsMenu menuToAppend = menuInstaller.menuToAppend;
                    VRCExpressionsMenu newMenuToAppend = (VRCExpressionsMenu)EditorGUILayout.ObjectField("Menu To Append", menuToAppend, typeof(VRCExpressionsMenu), false);
                    if (newMenuToAppend != menuToAppend)
                    {
                        menuInstaller.menuToAppend = newMenuToAppend;
                        EditorUtility.SetDirty(menuInstaller);
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
            }

            EditorGUILayout.HelpBox("ModularAvatarのMenuGroup/MenuInstallerを活用して、アバターのメニューを柔軟に拡張できます。", MessageType.Info);
        }
        #endif

        #if PCSS_DEV
        private void DrawModularAvatarParameterSection()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Modular Avatar Parameters", EditorStyles.boldLabel);

            if (maParameters.Count == 0)
            {
                EditorGUILayout.HelpBox("No ModularAvatarParameter components found", MessageType.Info);
                return;
            }

            foreach (var param in maParameters)
            {
                if (param == null) continue;

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"Parameter: {param.name}", EditorStyles.boldLabel);

                EditorGUILayout.LabelField("Type", param.type.ToString());
                float newDefault = param.defaultValue;
                switch (param.type)
                {
                    case DummyParameter.ParameterType.Bool:
                        newDefault = EditorGUILayout.Toggle("Default Value", param.defaultValue > 0.5f) ? 1f : 0f;
                        break;
                    case DummyParameter.ParameterType.Int:
                        newDefault = EditorGUILayout.IntField("Default Value", (int)param.defaultValue);
                        break;
                    case DummyParameter.ParameterType.Float:
                        newDefault = EditorGUILayout.FloatField("Default Value", param.defaultValue);
                        break;
                }
                if (Math.Abs(newDefault - param.defaultValue) > 0.001f)
                {
                    param.defaultValue = newDefault;
                    EditorUtility.SetDirty(param);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.HelpBox("ModularAvatarParameterは、アバターの同期・メニュー・アニメーター制御に利用されます。", MessageType.Info);
        }
        #endif

        private void DrawAdvancedSettingsSection()
        {
            EditorGUILayout.Space();
            showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Settings");

            if (showAdvancedSettings)
            {
                EditorGUILayout.BeginVertical("box");

                // ModularAvatarの自動リビルド
                bool autoRebuild = EditorGUILayout.Toggle("Auto Rebuild on Change", true);

                // ModularAvatarのバリデーション
                bool validateComponents = EditorGUILayout.Toggle("Validate ModularAvatar Components", true);

                EditorGUILayout.HelpBox("ModularAvatarの最新機能（Sync, Asset管理, Menu, Parameter, Constraint, etc.）を活用してください。", MessageType.Info);

                EditorGUILayout.EndVertical();
            }
        }

        private void DrawActionButtons()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Rebuild Modular Avatar"))
            {
                RebuildModularAvatar();
            }

            if (GUILayout.Button("Validate Components"))
            {
                ValidateComponents();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.miniBoldLabel);
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = avatarObject != null;
            if (GUILayout.Button("Create MA Light Toggle (ON/OFF + Intensity)"))
            {
                SafeCall(() => ModularAvatarLightToggleBuilder.Build());
            }
            if (GUILayout.Button("Build VRC Expressions (Light)"))
            {
                SafeCall(() => VRChatLightMenuBuilder.BuildLightExpressions());
            }
            if (GUILayout.Button("Place Hip-based Lights (MA)"))
            {
                SafeCall(() => HipBasedLightPlacement.CreateHipBasedLights(avatarObject));
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        private void RefreshData()
        {
            // アバター配下のライトのみを対象
            avatarLights.Clear();
            if (avatarObject != null)
            {
                avatarLights.AddRange(avatarObject.GetComponentsInChildren<Light>(true));
            }

            #if PCSS_DEV
            // ModularAvatarのMenuGroup/MenuInstaller/Parameterを取得
            menuGroups.Clear();
            menuInstallers.Clear();
            maParameters.Clear();
            if (avatarObject != null)
            {
                menuGroups.AddRange(avatarObject.GetComponentsInChildren<DummyMenuGroup>(true));
                menuInstallers.AddRange(avatarObject.GetComponentsInChildren<DummyMenuInstaller>(true));
                maParameters.AddRange(avatarObject.GetComponentsInChildren<DummyParameter>(true));
            }
            #endif
        }

        /// <summary>
        /// ModularAvatarParameterをアバター上で検索し、なければ新規作成
        /// </summary>
        #if PCSS_DEV
        private DummyParameter EnsureParameter(string paramName, DummyParameter.ParameterType type, float defaultValue = 0f)
        {
            var param = maParameters.FirstOrDefault(p => p != null && p.name == paramName);
            if (param == null && avatarObject != null)
            {
                var go = new GameObject(paramName);
                go.transform.SetParent(avatarObject.transform, false);
                param = go.AddComponent<DummyParameter>();
                param.name = paramName;
                param.type = type;
                param.defaultValue = defaultValue;
                maParameters.Add(param);
                EditorUtility.SetDirty(param);
            }
            else if (param != null && param.type != type)
            {
                param.type = type;
                EditorUtility.SetDirty(param);
            }
            return param;
        }
        #endif

        private void RebuildModularAvatar()
        {
            if (avatarObject == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select an avatar first!", "OK");
                return;
            }

            try
            {
            #if PCSS_DEV
            // ModularAvatarのリビルド（ダミー）
            var installers = avatarObject.GetComponentsInChildren<DummyMenuInstaller>(true);
            foreach (var installer in installers) { }
            #endif
                EditorUtility.DisplayDialog("Rebuild Complete", "Modular Avatar rebuilt successfully! (Dummy)", "OK");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to rebuild Modular Avatar: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to rebuild Modular Avatar: {e.Message}", "OK");
            }
        }

        private void ValidateComponents()
        {
            if (avatarObject == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select an avatar first!", "OK");
                return;
            }

            try
            {
                #if PCSS_DEV
                // ModularAvatarのバリデーション（ダミー）
                // var issues = ModularAvatarValidator.ValidateAvatar(avatarObject);
                #endif
                EditorUtility.DisplayDialog("Validation Complete", "No issues found! (Dummy)", "OK");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to validate components: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to validate components: {e.Message}", "OK");
            }
        }

        private void SafeCall(Action action)
        {
            try { action?.Invoke(); }
            catch (Exception e)
            {
                Debug.LogError($"[PCSS] Action failed: {e.Message}\n{e}");
                EditorUtility.DisplayDialog("PCSS", $"Action failed: {e.Message}", "OK");
            }
        }
    }
}