using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
#endif

namespace lilToon.PCSS.Editor
{
    public static class VRChatLightMenuBuilder
    {
        private const string MenuPath = "Tools/lilToon-PCSS-Extension/VRChat/ライト制御メニューを作成";
        private const string ExprDir = "Assets/PCSS/Expressions";
        private const string ExprParamsPath = "Assets/PCSS/Expressions/PCSS_ExprParams.asset";
        private const string ExprMenuPath = "Assets/PCSS/Expressions/PCSS_ExprMenu.asset";

        private const string ParamOn = "PCSS_LightOn";
        private const string ParamIntensity = "PCSS_LightIntensity";

        [MenuItem(MenuPath, false, 90)]
        public static void BuildLightExpressions()
        {
#if VRC_SDK_VRCSDK3
            var avatar = Selection.activeGameObject;
            if (avatar == null || avatar.GetComponent<VRCAvatarDescriptor>() == null)
            {
                EditorUtility.DisplayDialog("PCSS", "VRChatアバターを選択してください。", "OK");
                return;
            }

            EnsureExprAssets(out var @params, out var menu);

            // Parameters
            EnsureParameter(@params, ParamOn, VRCExpressionParameters.ValueType.Bool, saved: true, defaultValue: 1f);
            EnsureParameter(@params, ParamIntensity, VRCExpressionParameters.ValueType.Float, saved: true, defaultValue: 1f);

            // Menu: Submenu with Toggle + Radial Puppet
            var rootMenu = menu;
            var subMenu = CreateOrGetSubMenu(rootMenu, "PCSS Light");
            AddOrReplaceToggle(subMenu, "Light On", ParamOn, value: 1f);
            AddOrReplaceRadial(subMenu, "Intensity", ParamIntensity);

            // Attach to avatar
            var desc = avatar.GetComponent<VRCAvatarDescriptor>();
            desc.expressionParameters = @params;
            desc.expressionsMenu = rootMenu;
            EditorUtility.SetDirty(desc);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("PCSS", "表情メニューにライト制御を追加しました。\nトグルでON/OFF、ラジアルで光量を連続調整できます。", "OK");
#else
            EditorUtility.DisplayDialog("PCSS", "VRChat SDK 3 が見つかりません。先に導入してください。", "OK");
#endif
        }

#if VRC_SDK_VRCSDK3
        private static void EnsureExprAssets(out VRCExpressionParameters @params, out VRCExpressionsMenu menu)
        {
            if (!AssetDatabase.IsValidFolder("Assets/PCSS")) AssetDatabase.CreateFolder("Assets", "PCSS");
            if (!AssetDatabase.IsValidFolder(ExprDir)) AssetDatabase.CreateFolder("Assets/PCSS", "Expressions");

            @params = AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(ExprParamsPath);
            if (@params == null)
            {
                @params = ScriptableObject.CreateInstance<VRCExpressionParameters>();
                @params.parameters = new VRCExpressionParameters.Parameter[0];
                AssetDatabase.CreateAsset(@params, ExprParamsPath);
            }

            menu = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(ExprMenuPath);
            if (menu == null)
            {
                menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                AssetDatabase.CreateAsset(menu, ExprMenuPath);
            }
        }

        private static void EnsureParameter(VRCExpressionParameters @params, string name, VRCExpressionParameters.ValueType type, bool saved, float defaultValue)
        {
            var list = new System.Collections.Generic.List<VRCExpressionParameters.Parameter>(@params.parameters ?? new VRCExpressionParameters.Parameter[0]);
            int idx = list.FindIndex(p => p != null && p.name == name);
            var param = new VRCExpressionParameters.Parameter
            {
                name = name,
                valueType = type,
                defaultValue = defaultValue,
                saved = saved
            };
            if (idx >= 0) list[idx] = param; else list.Add(param);
            @params.parameters = list.ToArray();
            EditorUtility.SetDirty(@params);
        }

        private static VRCExpressionsMenu CreateOrGetSubMenu(VRCExpressionsMenu root, string title)
        {
            foreach (var c in root.controls)
            {
                if (c != null && c.type == VRCExpressionsMenu.Control.ControlType.SubMenu && c.subMenu != null && c.name == title)
                    return c.subMenu;
            }
            var sub = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            AssetDatabase.AddObjectToAsset(sub, ExprMenuPath);
            var control = new VRCExpressionsMenu.Control
            {
                name = title,
                type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                subMenu = sub
            };
            root.controls.Add(control);
            EditorUtility.SetDirty(root);
            return sub;
        }

        private static void AddOrReplaceToggle(VRCExpressionsMenu menu, string title, string param, float value)
        {
            var ctrl = menu.controls.Find(c => c.name == title);
            var newCtrl = new VRCExpressionsMenu.Control
            {
                name = title,
                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                parameter = new VRCExpressionsMenu.Control.Parameter { name = param },
                value = value
            };
            if (ctrl != null) menu.controls[menu.controls.IndexOf(ctrl)] = newCtrl; else menu.controls.Add(newCtrl);
            EditorUtility.SetDirty(menu);
        }

        private static void AddOrReplaceRadial(VRCExpressionsMenu menu, string title, string param)
        {
            var ctrl = menu.controls.Find(c => c.name == title);
            var newCtrl = new VRCExpressionsMenu.Control
            {
                name = title,
                type = VRCExpressionsMenu.Control.ControlType.RadialPuppet,
                parameter = new VRCExpressionsMenu.Control.Parameter { name = param },
            };
            if (ctrl != null) menu.controls[menu.controls.IndexOf(ctrl)] = newCtrl; else menu.controls.Add(newCtrl);
            EditorUtility.SetDirty(menu);
        }
#endif
    }
}


