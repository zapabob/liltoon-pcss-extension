
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using ModularAvatar.Runtime;

public class ModularAvatarLightControl : EditorWindow
{
    private GameObject avatarObject;
    private Light targetLight;

    [MenuItem("Tools/Modular Avatar Light Control")]
    public static void ShowWindow()
    {
        GetWindow<ModularAvatarLightControl>("MA Light Control");
    }

    private void OnGUI()
    {
        GUILayout.Label("Modular Avatar Light Setup", EditorStyles.boldLabel);

        avatarObject = (GameObject)EditorGUILayout.ObjectField("Avatar", avatarObject, typeof(GameObject), true);
        targetLight = (Light)EditorGUILayout.ObjectField("Target Light", targetLight, typeof(Light), true);

        if (GUILayout.Button("Setup Light Toggle"))
        {
            if (avatarObject != null && targetLight != null)
            {
                SetupLightToggle();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please select an avatar and a light.", "OK");
            }
        }
    }

    private void SetupLightToggle()
    {
        // 1. Add Modular Avatar Menu Installer
        var menuInstaller = avatarObject.AddComponent<ModularAvatarMenuInstaller>();

        // 2. Create a new VRC Expressions Menu
        var menu = CreateInstance<VRCExpressionsMenu>();
        AssetDatabase.CreateAsset(menu, $"Assets/{avatarObject.name}_LightMenu.asset");

        var control = new VRCExpressionsMenu.Control
        {
            name = "Toggle Light",
            type = VRCExpressionsMenu.Control.ControlType.Toggle,
            parameter = new VRCExpressionsMenu.Control.Parameter { name = "MA_Light_Toggle" },
            value = 1
        };
        menu.controls.Add(control);
        EditorUtility.SetDirty(menu);

        menuInstaller.menuToInstall = menu;

        // 3. Add Modular Avatar Parameters
        var parameters = avatarObject.AddComponent<ModularAvatarParameters>();
        parameters.parameters.Add(new ParameterConfig
        {
            nameOrPrefix = "MA_Light_Toggle",
            syncType = ParameterSyncType.Bool,
            defaultValue = 1,
            saved = true
        });

        // 4. Add Modular Avatar Animator
        var animator = avatarObject.AddComponent<ModularAvatarAnimator>();
        var controller = new UnityEditor.Animations.AnimatorController();
        AssetDatabase.CreateAsset(controller, $"Assets/{avatarObject.name}_LightController.asset");
        animator.animator = controller;

        var layer = new UnityEditor.Animations.AnimatorControllerLayer
        {
            name = "LightControl",
            stateMachine = new UnityEditor.Animations.AnimatorStateMachine(),
            defaultWeight = 1
        };
        controller.AddLayer(layer);
        
        var stateMachine = layer.stateMachine;
        var offState = stateMachine.AddState("Off");
        var onState = stateMachine.AddState("On");

        offState.motion = CreateClip("LightOff", false);
        onState.motion = CreateClip("LightOn", true);

        var toOn = offState.AddTransition(onState);
        toOn.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 1, "MA_Light_Toggle");
        var toOff = onState.AddTransition(offState);
        toOff.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 1, "MA_Light_Toggle");

        // 5. Add animation to target light
        var animationComponent = targetLight.gameObject.AddComponent<Animation>();
        animationComponent.AddClip(offState.motion, "LightOff");
        animationComponent.AddClip(onState.motion, "LightOn");
        animationComponent.playAutomatically = false;


        EditorUtility.DisplayDialog("Success", "Light toggle has been set up successfully!", "OK");
    }

    private AnimationClip CreateClip(string name, bool lightState)
    {
        var clip = new AnimationClip();
        var curve = new AnimationCurve();
        curve.AddKey(0, lightState ? 1 : 0);
        clip.SetCurve(GetPath(targetLight.transform), typeof(Light), "m_Enabled", curve);
        AssetDatabase.CreateAsset(clip, $"Assets/{avatarObject.name}_{name}.anim");
        return clip;
    }

    private string GetPath(Transform current)
    {
        if (current.parent == null || current.parent == avatarObject.transform)
        {
            return current.name;
        }
        return GetPath(current.parent) + "/" + current.name;
    }
}
