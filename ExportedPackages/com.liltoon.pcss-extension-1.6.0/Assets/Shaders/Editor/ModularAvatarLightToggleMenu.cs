 #if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#if MODULAR_AVATAR_AVAILABLE
using nadena.dev.modular_avatar.core;
#endif

/// <summary>
/// ModularAvatarでLightのON/OFFトグルをBaseレイヤーで制御するメニューを�E動生成するエチE��タ拡張
/// </summary>
public class ModularAvatarLightToggleMenu : MonoBehaviour
{
#if MODULAR_AVATAR_AVAILABLE
    [MenuItem("Tools/lilToon PCSS Extension/Utilities/PCSS Extension/ModularAvatar/ライトトグルメニューを追加")]
    public static void AddLightToggleMenu()
    {
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("エラー", "アバターのルートを選択してください、E, "OK");
            return;
        }

        // Expression Parameter
        var param = selected.GetComponent<ModularAvatarParameters>();
        if (param == null) param = selected.AddComponent<ModularAvatarParameters>();
        bool found = false;
        foreach (var p in param.parameters)
        {
            if (p.name == "LightToggle") { found = true; break; }
        }
        if (!found)
        {
            param.parameters.Add(new ParameterConfig()
            {
                name = "LightToggle",
                defaultValue = 1,
                syncType = ParameterSyncType.Bool,
                saved = true
            });
        }

        // Expression Menu
        var menu = selected.GetComponent<ModularAvatarMenuInstaller>();
        if (menu == null) menu = selected.AddComponent<ModularAvatarMenuInstaller>();
        var rootMenu = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionMenu();
        var control = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionMenu.Control
        {
            name = "ライチEN/OFF",
            type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionMenu.ControlType.Toggle,
            parameter = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter { name = "LightToggle" }
        };
        rootMenu.controls.Add(control);
        menu.menu = rootMenu;

        // Light Toggle用のModularAvatarGameObjectToggle
        var lightObj = FindLightObject(selected.transform);
        if (lightObj == null)
        {
            EditorUtility.DisplayDialog("エラー", "子オブジェクトにLightが見つかりません、E, "OK");
            return;
        }
        var toggle = lightObj.GetComponent<ModularAvatarGameObjectToggle>();
        if (toggle == null) toggle = lightObj.AddComponent<ModularAvatarGameObjectToggle>();
        toggle.parameter = "LightToggle";
        toggle.isOn = true;
        toggle.layer = ModularAvatarGameObjectToggle.LayerType.Base; // Baseレイヤーで動佁E

        EditorUtility.DisplayDialog("完亁E, "ModularAvatarライトトグルメニューを追加しました、E, "OK");
    }

    private static GameObject FindLightObject(Transform root)
    {
        foreach (var light in root.GetComponentsInChildren<Light>(true))
        {
            return light.gameObject;
        }
        return null;
    }
#endif
}
#endif 
