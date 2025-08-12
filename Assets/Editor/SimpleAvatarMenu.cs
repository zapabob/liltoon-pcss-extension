using UnityEditor;
using UnityEngine;

#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
#endif

#if MODULAR_AVATAR
using nadena.dev.modular_avatar.core;
#endif

namespace lilToon.PCSS.Editor
{
    public static class SimpleAvatarMenu
    {
        private const string DefaultMenuRoot = "Tools/lilToon-PCSS-Extension/Quick Add";

        private enum LightPreset
        {
            Realistic,
            Anime,
            Cinematic
        }

        [MenuItem(DefaultMenuRoot + "/PCSS Light", priority = 0)]
        public static void QuickAddPCSSLight()
        {
            CreateLightWithPreset(LightPreset.Realistic);
        }

        [MenuItem(DefaultMenuRoot + "/VRC Light Volumes Integration", priority = 1)]
        public static void QuickAddVRCLightVolumes()
        {
            var target = Selection.activeGameObject;
            if (target == null)
            {
                EditorUtility.DisplayDialog("エラー", "GameObjectを選択してください。", "OK");
                return;
            }

            if (target.GetComponent<VRCLightVolumesIntegration>() == null)
            {
                target.AddComponent<VRCLightVolumesIntegration>();
            }

            EditorUtility.DisplayDialog("完了", "VRC Light Volumes Integration を追加しました。", "OK");
        }

        [MenuItem(DefaultMenuRoot + "/All-in-One", priority = 10)]
        public static void QuickAddAll()
        {
            QuickAddPCSSLight();
            var avatarRoot = GetSelectedAvatarRoot();
            if (avatarRoot != null)
            {
                Selection.activeGameObject = avatarRoot;
                QuickAddVRCLightVolumes();
            }
        }

        [MenuItem(DefaultMenuRoot + "/Presets/Realistic", priority = 20)]
        public static void QuickAddPresetRealistic()
        {
            CreateLightWithPreset(LightPreset.Realistic);
        }

        [MenuItem(DefaultMenuRoot + "/Presets/Anime", priority = 21)]
        public static void QuickAddPresetAnime()
        {
            CreateLightWithPreset(LightPreset.Anime);
        }

        [MenuItem(DefaultMenuRoot + "/Presets/Cinematic", priority = 22)]
        public static void QuickAddPresetCinematic()
        {
            CreateLightWithPreset(LightPreset.Cinematic);
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/Open Setup Wizard", priority = 50)]
        public static void OpenWizard()
        {
            ModularAvatarPCSSSetupWizard.ShowWindow();
        }

        private static GameObject GetSelectedAvatarRoot()
        {
            var go = Selection.activeGameObject;
            if (go == null) return null;

#if VRC_SDK_VRCSDK3
            var avatar = go.GetComponentInParent<VRCAvatarDescriptor>();
            return avatar != null ? avatar.gameObject : null;
#else
            return go;
#endif
        }

#if MODULAR_AVATAR
        private static void AddSimpleModularAvatarToggle(GameObject target, string menuItemName, string parameterName)
        {
            var toggleRoot = new GameObject("PCSS_Menu");
            toggleRoot.transform.SetParent(target.transform.parent, false);

            var objectToggle = toggleRoot.AddComponent<ModularAvatarObjectToggle>();
            objectToggle.parameter = parameterName;
            objectToggle.entries.Add(new ModularAvatarObjectToggle.Entry
            {
                target = target,
                enable = false
            });

            var menuItem = toggleRoot.AddComponent<ModularAvatarMenuItem>();
            menuItem.menuItemName = menuItemName;
            menuItem.type = ModularAvatarMenuItem.MenuItemType.Toggle;
            menuItem.parameter = parameterName;

            var parameters = toggleRoot.AddComponent<ModularAvatarParameters>();
            parameters.parameters.Add(new ModularAvatarParameters.Parameter()
            {
                name = parameterName,
                defaultValue = 0f
            });

            toggleRoot.AddComponent<ModularAvatarMenuInstaller>();
        }
#endif

        private static void CreateLightWithPreset(LightPreset preset)
        {
            var avatarRoot = GetSelectedAvatarRoot();
            if (avatarRoot == null)
            {
                EditorUtility.DisplayDialog("エラー", "VRChatアバターのルートを選択してください。", "OK");
                return;
            }

            var existing = avatarRoot.transform.Find("PCSS_Light");
            if (existing != null)
            {
                Selection.activeGameObject = existing.gameObject;
                EditorUtility.DisplayDialog("情報", "既にPCSS_Lightが存在します。選択しました。", "OK");
                return;
            }

            var lightObject = new GameObject("PCSS_Light");
            lightObject.transform.SetParent(avatarRoot.transform, false);
            lightObject.transform.localPosition = Vector3.zero;

            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Spot;
            light.shadows = LightShadows.Soft;

            float intensity = 2f;
            float spotAngle = 70f;
            float range = 1.2f;
            Color color = Color.white;
            float shadowStrength = 0.9f;

            switch (preset)
            {
                case LightPreset.Realistic:
                    intensity = 1.6f;
                    spotAngle = 60f;
                    range = 1.5f;
                    color = new Color(1f, 0.95f, 0.9f); // warm white
                    shadowStrength = 0.9f;
                    break;
                case LightPreset.Anime:
                    intensity = 2.5f;
                    spotAngle = 45f;
                    range = 1.0f;
                    color = Color.white;
                    shadowStrength = 1.0f;
                    break;
                case LightPreset.Cinematic:
                    intensity = 2.2f;
                    spotAngle = 35f;
                    range = 1.4f;
                    color = new Color(0.95f, 0.95f, 1.0f); // slightly cool
                    shadowStrength = 0.95f;
                    break;
            }

            light.intensity = intensity;
            light.spotAngle = spotAngle;
            light.range = range;
            light.color = color;
            light.shadowStrength = shadowStrength;

            // ランタイム独自コンポーネントの追加を廃止（AutoFix対策）
            // 以降の挙動は Animator/Parameters で制御する方針

#if VRC_SDK_VRCSDK3
            var physBone = avatarRoot.GetComponentsInChildren<VRCPhysBone>(true)
                .FirstOrDefaultSafe(pb => pb != null);
            // PhysBoneは検出のみ（直接のランタイム制御は行わない）
#endif

#if MODULAR_AVATAR
            AddSimpleModularAvatarToggle(lightObject, "PCSS Light", "PCSS_Light_On");
#endif

            Selection.activeGameObject = lightObject;
            EditorUtility.DisplayDialog("完了", $"PCSS Light ({preset}) を追加しました。", "OK");
        }
    }

    internal static class EnumerableSafe
    {
        public static T FirstOrDefaultSafe<T>(this System.Collections.Generic.IEnumerable<T> source, System.Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item)) return item;
            }
            return default;
        }
    }
}



