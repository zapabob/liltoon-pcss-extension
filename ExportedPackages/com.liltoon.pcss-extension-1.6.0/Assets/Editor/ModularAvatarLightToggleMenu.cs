#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
// ModularAvatar縺ｮ繧ｳ繧｢蜷榊燕遨ｺ髢難ｼ亥ｿ・ｦ√↓蠢懊§縺ｦ繝代ャ繧ｱ繝ｼ繧ｸ蜷阪ｒ隱ｿ謨ｴ・・
using nadena.dev.modular_avatar.core;

/// <summary>
/// ModularAvatar縺ｧLight縺ｮON/OFF繝医げ繝ｫ繧達ase繝ｬ繧､繝､繝ｼ縺ｧ蛻ｶ蠕｡縺吶ｋ繝｡繝九Η繝ｼ繧定・蜍慕函謌舌☆繧九お繝・ぅ繧ｿ諡｡蠑ｵ
/// </summary>
public class ModularAvatarLightToggleMenu
{
    [MenuItem("Tools/lilToonPCSSExtension/Add Light Toggle")]
    public static void AddLightToggle()
    {
        // 驕ｸ謚樔ｸｭ縺ｮGameObject・医い繝舌ち繝ｼ縺ｮ繝ｫ繝ｼ繝茨ｼ峨ｒ蜿門ｾ・
        var avatar = Selection.activeGameObject;
        if (avatar == null)
        {
            EditorUtility.DisplayDialog("繧ｨ繝ｩ繝ｼ", "繧｢繝舌ち繝ｼ縺ｮ繝ｫ繝ｼ繝・ameObject繧帝∈謚槭＠縺ｦ縺上□縺輔＞縲・, "OK");
            return;
        }

        // 譌｢蟄倥・LightToggle縺後≠繧後・蜑企勁
        var old = avatar.transform.Find("LightToggle");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        // LightToggle逕ｨ繧ｪ繝悶ず繧ｧ繧ｯ繝井ｽ懈・
        var toggleRoot = new GameObject("LightToggle");
        toggleRoot.transform.SetParent(avatar.transform, false);

        // ObjectToggle霑ｽ蜉
        var objectToggle = toggleRoot.AddComponent<ModularAvatarObjectToggle>();

        // 繧｢繝舌ち繝ｼ驟堺ｸ九・蜈ｨLight繧丹bjectToggle縺ｫ逋ｻ骭ｲ
        foreach (var light in avatar.GetComponentsInChildren<Light>(true))
        {
            objectToggle.entries.Add(new ModularAvatarObjectToggle.Entry
            {
                target = light.gameObject,
                enable = false // OFF縺ｧ髱櫁｡ｨ遉ｺ
            });
        }

        // MenuItem霑ｽ蜉
        var menuItem = toggleRoot.AddComponent<ModularAvatarMenuItem>();
        menuItem.menuItemName = "Light Toggle";
        menuItem.type = ModularAvatarMenuItem.MenuItemType.Toggle;

        // MenuInstaller霑ｽ蜉
        toggleRoot.AddComponent<ModularAvatarMenuInstaller>();

        // 驕ｸ謚樒憾諷九ｒ譁ｰ縺励＞LightToggle縺ｫ
        Selection.activeGameObject = toggleRoot;

        EditorUtility.DisplayDialog("螳御ｺ・, "Light Toggle縺瑚ｿｽ蜉縺輔ｌ縺ｾ縺励◆・―nVRChat蜀・〒繝｡繝九Η繝ｼ縺九ｉON/OFF縺ｧ縺阪∪縺吶・, "OK");
    }
}
#endif 
