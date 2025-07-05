using UnityEngine;
using UnityEditor;
#if MODULAR_AVATAR
using nadena.dev.modular_avatar.core;
#endif
#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.ScriptableObjects;
using UnityEditor.Animations;
#endif

// プリセット定義
public struct LightPreset
{
    public string objName;
    public LightType type;
    public Color color;
    public float intensity;
    public float range;
    public float spotAngle;
    public string label;
    public Vector3 localPosition;
    public Vector3 localRotation;

    public LightPreset(string objName, LightType type, Color color, float intensity, float range, float spotAngle, string label, Vector3 localPosition, Vector3 localRotation)
    {
        this.objName = objName;
        this.type = type;
        this.color = color;
        this.intensity = intensity;
        this.range = range;
        this.spotAngle = spotAngle;
        this.label = label;
        this.localPosition = localPosition;
        this.localRotation = localRotation;
    }
}

public static class ModularAvatarLightPresetMenu
{
    // プリセット一覧
    private static readonly LightPreset[] presets = new LightPreset[]
    {
        new LightPreset(
            "BeautyLightToggle",
            LightType.Spot,
            new Color(1.0f, 0.95f, 0.9f),
            2.0f, 5.0f, 60f,
            "美肌ライト",
            new Vector3(0, 1.6f, 0.3f), // 顔前方
            new Vector3(20, 0, 0)
        ),
        new LightPreset(
            "RimLightToggle",
            LightType.Directional,
            new Color(0.8f, 0.9f, 1.0f),
            0.7f, 10.0f, 0f,
            "シルエットライト",
            new Vector3(0, 1.7f, -0.5f), // 背面
            new Vector3(340, 180, 0)
        ),
        new LightPreset(
            "MoodLightToggle",
            LightType.Point,
            new Color(1.0f, 0.8f, 0.7f),
            1.2f, 7.0f, 0f,
            "雰囲気ライト",
            new Vector3(0.2f, 1.2f, 0.2f), // 側面
            Vector3.zero
        ),
    };

    [MenuItem("Tools/lilToonPCSSExtension/Add Light Preset/美肌ライト")]
    public static void AddBeautyLight() => AddLightPreset(presets[0]);
    [MenuItem("Tools/lilToonPCSSExtension/Add Light Preset/シルエットライト")]
    public static void AddRimLight() => AddLightPreset(presets[1]);
    [MenuItem("Tools/lilToonPCSSExtension/Add Light Preset/雰囲気ライト")]
    public static void AddMoodLight() => AddLightPreset(presets[2]);

    private static void AddLightPreset(LightPreset preset)
    {
        var avatar = Selection.activeGameObject;
        if (avatar == null)
        {
            Debug.LogError("アバターのルートGameObjectを選択してください");
            return;
        }

        // 既存チェック
        var existing = avatar.transform.Find(preset.objName);
        if (existing != null)
        {
            Debug.LogWarning($"既に{preset.label}が存在します");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(avatar, $"Add {preset.label}");

        // LightToggleオブジェクト生成
        var toggleObj = new GameObject(preset.objName);
        toggleObj.transform.SetParent(avatar.transform, false);
        toggleObj.transform.localPosition = preset.localPosition;
        toggleObj.transform.localEulerAngles = preset.localRotation;

        // Lightコンポーネント追加
        var light = toggleObj.AddComponent<Light>();
        light.type = preset.type;
        light.color = preset.color;
        light.intensity = preset.intensity;
        light.range = preset.range;
        if (preset.type == LightType.Spot) light.spotAngle = preset.spotAngle;

#if MODULAR_AVATAR
        // ModularAvatarObjectToggle追加
        var toggle = toggleObj.AddComponent<ModularAvatarObjectToggle>();
        toggle.isToggle = true;
        toggle.defaultState = true;
#endif

#if VRC_SDK_VRCSDK3
        // Expression Parameters自動追加
        var parametersAssetPath = "Assets/ExpressionParameters.asset"; // TODO: 実際のパスに合わせて調整
        var parameters = AssetDatabase.LoadAssetAtPath<VRCExpressionsParameters>(parametersAssetPath);
        if (parameters == null)
        {
            parameters = ScriptableObject.CreateInstance<VRCExpressionsParameters>();
            AssetDatabase.CreateAsset(parameters, parametersAssetPath);
        }
        var existingNames = new HashSet<string>();
        foreach (var p in parameters.parameters)
        {
            if (!string.IsNullOrEmpty(p.name)) existingNames.Add(p.name);
        }
        if (!existingNames.Contains(preset.objName))
        {
            var paramList = new List<VRCExpressionParameters.Parameter>(parameters.parameters);
            paramList.Add(new VRCExpressionParameters.Parameter
            {
                name = preset.objName,
                valueType = VRCExpressionParameters.ValueType.Bool,
                saved = true,
                defaultValue = 1f
            });
            parameters.parameters = paramList.ToArray();
            EditorUtility.SetDirty(parameters);
            AssetDatabase.SaveAssets();
            Debug.Log($"Expression Parameter '{preset.objName}' を追加しました");
        }

        // Expression Menu自動追加
        var menuAssetPath = "Assets/ExpressionsMenu.asset"; // TODO: 実際のパスに合わせて調整
        var menu = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(menuAssetPath);
        if (menu == null)
        {
            menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            AssetDatabase.CreateAsset(menu, menuAssetPath);
        }
        bool exists = false;
        foreach (var control in menu.controls)
        {
            if (control != null && control.parameter != null && control.parameter.name == preset.objName)
            {
                exists = true;
                break;
            }
        }
        if (!exists)
        {
            var control = new VRCExpressionsMenu.Control
            {
                name = preset.label,
                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                parameter = new VRCExpressionsMenu.Control.Parameter { name = preset.objName },
                value = 1f
            };
            var controlsList = new List<VRCExpressionsMenu.Control>(menu.controls);
            controlsList.Add(control);
            menu.controls = controlsList.ToArray();
            EditorUtility.SetDirty(menu);
            AssetDatabase.SaveAssets();
            Debug.Log($"Expression Menuにトグル '{preset.label}' を追加しました");
        }

        // Animator Controller自動生成
        var animatorPath = $"Assets/{preset.objName}_FX.controller"; // TODO: 実際のパスに合わせて調整
        AnimatorController controller = null;
        if (System.IO.File.Exists(animatorPath))
        {
            controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(animatorPath);
        }
        if (controller == null)
        {
            controller = AnimatorController.CreateAnimatorControllerAtPath(animatorPath);
        }
        // パラメータ追加（重複回避）
        if (!controller.parameters.Any(p => p.name == preset.objName))
        {
            controller.AddParameter(preset.objName, AnimatorControllerParameterType.Bool);
        }
        // レイヤー追加（重複回避）
        var layerName = $"{preset.label}Toggle";
        var layer = controller.layers.FirstOrDefault(l => l.name == layerName);
        if (layer == null)
        {
            var stateMachine = new AnimatorStateMachine();
            var onState = stateMachine.AddState("On");
            var offState = stateMachine.AddState("Off");
            // ONアニメーション
            var onClip = new AnimationClip();
            var binding = new EditorCurveBinding
            {
                path = preset.objName,
                type = typeof(GameObject),
                propertyName = "m_IsActive"
            };
            AnimationUtility.SetEditorCurve(onClip, binding, AnimationCurve.Constant(0, 1, 1));
            AssetDatabase.CreateAsset(onClip, $"Assets/{preset.objName}_On.anim");
            onState.motion = onClip;
            // OFFアニメーション
            var offClip = new AnimationClip();
            AnimationUtility.SetEditorCurve(offClip, binding, AnimationCurve.Constant(0, 1, 0));
            AssetDatabase.CreateAsset(offClip, $"Assets/{preset.objName}_Off.anim");
            offState.motion = offClip;
            // 遷移
            var toOn = offState.AddTransition(onState);
            toOn.AddCondition(AnimatorConditionMode.If, 0, preset.objName);
            toOn.hasExitTime = false;
            toOn.duration = 0;
            var toOff = onState.AddTransition(offState);
            toOff.AddCondition(AnimatorConditionMode.IfNot, 0, preset.objName);
            toOff.hasExitTime = false;
            toOff.duration = 0;
            // デフォルト状態
            stateMachine.defaultState = offState;
            // レイヤー追加
            layer = new AnimatorControllerLayer
            {
                name = layerName,
                stateMachine = stateMachine,
                defaultWeight = 1f
            };
            controller.AddLayer(layer);
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            Debug.Log($"Animator Controllerにレイヤー '{layerName}' を追加しました");
        }
#endif

        Undo.RegisterCreatedObjectUndo(toggleObj, $"Create {preset.label}");
        EditorUtility.SetDirty(avatar);
        Debug.Log($"{preset.label}プリセットを追加しました");
    }
} 