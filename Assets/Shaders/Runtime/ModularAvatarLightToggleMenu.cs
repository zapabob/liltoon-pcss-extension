#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
// ModularAvatarのコア名前空閁E
// ModularAvatarパッケージが見つからなぁE��合�E、パチE��ージマネージャーでインスト�Eルしてください
#if MODULAR_AVATAR
using nadena.dev.modularavatar.core;
#endif

/// <summary>
/// ModularAvatarでLightのON/OFFトグルをBaseレイヤーで制御するメニューを�E動生成するエチE��タ拡張
/// </summary>
public class ModularAvatarLightToggleMenu
{
    [MenuItem("Tools/ModularAvatar/Add Light Toggle")]
    public static void AddLightToggle()
    {
#if !MODULAR_AVATAR
        EditorUtility.DisplayDialog("エラー", "ModularAvatarパッケージがインスト�EルされてぁE��せん、EnVCCからModularAvatarをインスト�Eルしてください、E, "OK");
        return;
#else
        // 選択中のGameObject�E�アバターのルート）を取征E
        var avatar = Selection.activeGameObject;
        if (avatar == null)
        {
            EditorUtility.DisplayDialog("エラー", "アバターのルーチEameObjectを選択してください、E, "OK");
            return;
        }

        // 既存�ELightToggleがあれ�E削除
        var old = avatar.transform.Find("LightToggle");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        // LightToggle用オブジェクト作�E
        var toggleRoot = new GameObject("LightToggle");
        toggleRoot.transform.SetParent(avatar.transform, false);

        // ObjectToggle追加
        var objectToggle = toggleRoot.AddComponent<ModularAvatarObjectToggle>();

        // アバター配下�E全LightをObjectToggleに登録
        foreach (var light in avatar.GetComponentsInChildren<Light>(true))
        {
            objectToggle.entries.Add(new ModularAvatarObjectToggle.Entry
            {
                target = light.gameObject,
                enable = false // OFFで非表示
            });
        }

        // MenuItem追加
        var menuItem = toggleRoot.AddComponent<ModularAvatarMenuItem>();
        menuItem.menuItemName = "Light Toggle";
        menuItem.type = ModularAvatarMenuItem.MenuItemType.Toggle;

        // MenuInstaller追加
        toggleRoot.AddComponent<ModularAvatarMenuInstaller>();

        // 選択状態を新しいLightToggleに
        Selection.activeGameObject = toggleRoot;

        EditorUtility.DisplayDialog("完亁E, "Light Toggleが追加されました�E�\nVRChat冁E��メニューからON/OFFできます、E, "OK");
#endif
    }
}
#endif 
