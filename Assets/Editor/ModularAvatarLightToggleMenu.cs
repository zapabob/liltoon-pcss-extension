#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
// ModularAvatarのコア名前空間（必要に応じてパッケージ名を調整）
using nadena.dev.modular_avatar.core;

/// <summary>
/// ModularAvatarでLightのON/OFFトグルをBaseレイヤーで制御するメニューを自動生成するエディタ拡張
/// </summary>
public class ModularAvatarLightToggleMenu
{
    [MenuItem("Tools/lilToonPCSSExtension/Add Light Toggle")]
    public static void AddLightToggle()
    {
        // 選択中のGameObject（アバターのルート）を取得
        var avatar = Selection.activeGameObject;
        if (avatar == null)
        {
            EditorUtility.DisplayDialog("エラー", "アバターのルートGameObjectを選択してください。", "OK");
            return;
        }

        // 既存のLightToggleがあれば削除
        var old = avatar.transform.Find("LightToggle");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        // LightToggle用オブジェクト作成
        var toggleRoot = new GameObject("LightToggle");
        toggleRoot.transform.SetParent(avatar.transform, false);

        // ObjectToggle追加
        var objectToggle = toggleRoot.AddComponent<ModularAvatarObjectToggle>();

        // アバター配下の全LightをObjectToggleに登録
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

        EditorUtility.DisplayDialog("完了", "Light Toggleが追加されました！\nVRChat内でメニューからON/OFFできます。", "OK");
    }
}
#endif 