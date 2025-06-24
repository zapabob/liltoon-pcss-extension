#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#if MODULAR_AVATAR_AVAILABLE
using nadena.dev.modular_avatar.core;
#endif

/// <summary>
/// ModularAvatarでLightのON/OFFトグルをBaseレイヤーで制御するメニューを自動生成するエディタ拡張
/// </summary>
public class ModularAvatarLightToggleMenu : MonoBehaviour
{
#if MODULAR_AVATAR_AVAILABLE
    [MenuItem("lilToon/PCSS Extension/ModularAvatar/ライトトグルメニューを追加")]
    public static void AddLightToggleMenu()
    {
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("エラー", "アバターのルートを選択してください。", "OK");
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
            name = "ライトON/OFF",
            type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionMenu.ControlType.Toggle,
            parameter = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter { name = "LightToggle" }
        };
        rootMenu.controls.Add(control);
        menu.menu = rootMenu;

        // Light Toggle用のModularAvatarGameObjectToggle
        var lightObj = FindLightObject(selected.transform);
        if (lightObj == null)
        {
            EditorUtility.DisplayDialog("エラー", "子オブジェクトにLightが見つかりません。", "OK");
            return;
        }
        var toggle = lightObj.GetComponent<ModularAvatarGameObjectToggle>();
        if (toggle == null) toggle = lightObj.AddComponent<ModularAvatarGameObjectToggle>();
        toggle.parameter = "LightToggle";
        toggle.isOn = true;
        toggle.layer = ModularAvatarGameObjectToggle.LayerType.Base; // Baseレイヤーで動作

        EditorUtility.DisplayDialog("完了", "ModularAvatarライトトグルメニューを追加しました。", "OK");
    }

    [MenuItem("lilToon/PCSS Extension/ModularAvatar/表情赤くなるライトメニューを追加 (Base)")]
    public static void AddRedFaceToggleMenu()
    {
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("エラー", "アバターのルートを選択してください。", "OK");
            return;
        }

        // Expression Parameter
        var param = selected.GetComponent<ModularAvatarParameters>();
        if (param == null) param = selected.AddComponent<ModularAvatarParameters>();
        bool found = false;
        foreach (var p in param.parameters)
        {
            if (p.name == "RedFaceLight") { found = true; break; }
        }
        if (!found)
        {
            param.parameters.Add(new ParameterConfig()
            {
                name = "RedFaceLight",
                defaultValue = 0,
                syncType = ParameterSyncType.Bool,
                saved = true
            });
        }

        // Expression Menu
        var menu = selected.GetComponent<ModularAvatarMenuInstaller>();
        if (menu == null) menu = selected.AddComponent<ModularAvatarMenuInstaller>();
        
        // 既存のメニューがあれば使用、なければ新規作成
        var rootMenu = menu.menu;
        if (rootMenu == null)
        {
            rootMenu = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionMenu();
            menu.menu = rootMenu;
        }
        
        // 表情が赤くなるライト用のコントロールを追加
        var control = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionMenu.Control
        {
            name = "表情赤くなる",
            type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionMenu.ControlType.Toggle,
            parameter = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter { name = "RedFaceLight" }
        };
        rootMenu.controls.Add(control);

        // 赤いライトオブジェクトを探すか作成
        var redLightObj = FindRedLightObject(selected.transform);
        if (redLightObj == null)
        {
            // 赤いライトを作成
            redLightObj = CreateRedLight(selected.transform);
        }

        // ライトトグル用のコンポーネント追加
        var toggle = redLightObj.GetComponent<ModularAvatarGameObjectToggle>();
        if (toggle == null) toggle = redLightObj.AddComponent<ModularAvatarGameObjectToggle>();
        toggle.parameter = "RedFaceLight";
        toggle.isOn = false; // デフォルトはオフ
        toggle.layer = ModularAvatarGameObjectToggle.LayerType.Base; // Baseレイヤーで動作

        EditorUtility.DisplayDialog("完了", "表情が赤くなるライトメニューをBaseレイヤーに追加しました。", "OK");
    }

    private static GameObject FindLightObject(Transform root)
    {
        foreach (var light in root.GetComponentsInChildren<Light>(true))
        {
            return light.gameObject;
        }
        return null;
    }
    
    private static GameObject FindRedLightObject(Transform root)
    {
        foreach (var light in root.GetComponentsInChildren<Light>(true))
        {
            // 赤いライトを探す（名前またはライトの色で判断）
            if (light.gameObject.name.Contains("RedFace") || 
                light.gameObject.name.Contains("FaceLight") ||
                (light.color.r > 0.7f && light.color.g < 0.3f && light.color.b < 0.3f))
            {
                return light.gameObject;
            }
        }
        return null;
    }
    
    private static GameObject CreateRedLight(Transform root)
    {
        // 頭部を探す
        Transform headTransform = FindHeadBone(root);
        if (headTransform == null)
        {
            Debug.LogWarning("頭部ボーンが見つかりませんでした。アバタールートに作成します。");
            headTransform = root;
        }
        
        // 赤いライトを作成
        GameObject redLightObj = new GameObject("RedFaceLight");
        redLightObj.transform.SetParent(headTransform);
        redLightObj.transform.localPosition = new Vector3(0, 0, 0.1f); // 顔の前方に配置
        redLightObj.transform.localRotation = Quaternion.identity;
        
        Light redLight = redLightObj.AddComponent<Light>();
        redLight.type = LightType.Point;
        redLight.color = new Color(1f, 0.2f, 0.2f); // 赤色
        redLight.intensity = 0.8f;
        redLight.range = 0.5f;
        redLight.shadows = LightShadows.None; // 影なし（パフォーマンス対策）
        
        return redLightObj;
    }
    
    private static Transform FindHeadBone(Transform root)
    {
        // 一般的な頭部ボーン名で検索
        string[] headBoneNames = { "Head", "head", "Face", "face", "顔", "頭" };
        
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            foreach (string boneName in headBoneNames)
            {
                if (child.name.Contains(boneName))
                {
                    return child;
                }
            }
        }
        
        // Humanoidアバターの場合はAnimatorから頭部を取得
        Animator animator = root.GetComponent<Animator>();
        if (animator != null && animator.isHuman)
        {
            Transform headBone = animator.GetBoneTransform(HumanBodyBones.Head);
            if (headBone != null)
            {
                return headBone;
            }
        }
        
        return null;
    }
#endif
}
#endif 