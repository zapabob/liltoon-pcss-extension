using UnityEditor;
using UnityEngine;
using lilToon.PCSS.Runtime;
using System.Linq;

#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
#endif

#if MODULAR_AVATAR
using UnityEditor;
#endif

namespace lilToon.PCSS.Editor
{
    public class PhysBoneLightController : MonoBehaviour
    {
        public Light targetLight;
        public Transform lightOrigin;
        public VRCPhysBone targetPhysBone;
    }

    public class PerformanceOptimizerMenu : EditorWindow
    {
        private const string MenuPath = "Tools/lilToon PCSS Extension/Setup Performance Tuner";
        private const string PhysBoneMenuPath = "Tools/lilToon PCSS Extension/Setup PhysBone Light Controller";
        private const string GameObjectName = "PCSS Performance Tuner";

        [MenuItem(MenuPath)]
        private static void SetupPerformanceTuner()
        {
            // 繧ｷ繝ｼ繝ｳ蜀・↓譌｢縺ｫ繧ｪ繝励ユ繧｣繝槭う繧ｶ繝ｼ縺悟ｭ伜惠縺吶ｋ縺狗｢ｺ隱・
            VRChatPerformanceOptimizer existingOptimizer = Object.FindObjectOfType<VRChatPerformanceOptimizer>();
            if (existingOptimizer != null)
            {
                Debug.Log($"'{GameObjectName}' already exists in the scene. Selecting it for you.");
                Selection.activeObject = existingOptimizer.gameObject;
                return;
            }
            
            // 譁ｰ縺励＞GameObject繧剃ｽ懈・縺励√さ繝ｳ繝昴・繝阪Φ繝医ｒ繧｢繧ｿ繝・メ
            GameObject optimizerObject = new GameObject(GameObjectName);
            optimizerObject.AddComponent<VRChatPerformanceOptimizer>();

            // 謫堺ｽ懊ｒUndo蜿ｯ閭ｽ縺ｫ縺吶ｋ
            Undo.RegisterCreatedObjectUndo(optimizerObject, $"Create {GameObjectName}");

            // 菴懈・縺励◆繧ｪ繝悶ず繧ｧ繧ｯ繝医ｒ驕ｸ謚・
            Selection.activeObject = optimizerObject;

            Debug.Log($"Successfully set up '{GameObjectName}'. The intelligent performance tuner is now active in your scene.");
        }

        [MenuItem(PhysBoneMenuPath)]
        public static void SetupPhysBoneLightController()
        {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select an avatar in the Hierarchy view.", "OK");
                return;
            }

            Animator animator = selectedObject.GetComponent<Animator>();
#if VRC_SDK_VRCSDK3
            if (selectedObject.GetComponent<VRCAvatarDescriptor>() == null)
            {
                EditorUtility.DisplayDialog("Error", "The selected object does not appear to be a VRChat avatar. Please select the root of your avatar.", "OK");
                return;
            }
#else
            if (animator == null || !animator.isHuman)
            {
                EditorUtility.DisplayDialog("Error", "The selected object does not appear to be a humanoid avatar. Please select the root of your avatar.", "OK");
                return;
            }
#endif

            Transform headBone = animator.GetBoneTransform(HumanBodyBones.Head);
            if (headBone == null)
            {
                EditorUtility.DisplayDialog("Error", "Could not find the head bone on the selected avatar.", "OK");
                return;
            }

            // Create the Light GameObject
            GameObject lightObject = new GameObject("PhysBone Dynamic Light");
            Undo.RegisterCreatedObjectUndo(lightObject, "Create PhysBone Dynamic Light");
            lightObject.transform.SetParent(headBone, false); // Attach to head
            lightObject.transform.localPosition = new Vector3(0, 0.1f, 0.15f); // Position slightly in front of head

            // Configure the Light component
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Spot;
            light.spotAngle = 70f;
            light.range = 1.2f;
            light.intensity = 2.0f;
            light.shadows = LightShadows.Soft;
            light.shadowStrength = 0.9f;
            light.shadowNormalBias = 0.1f;
            light.cullingMask = 1; // Default layer only

            // Add and configure the controller
            PhysBoneLightController controller = lightObject.AddComponent<PhysBoneLightController>();
            controller.targetLight = light;
            controller.lightOrigin = headBone;

            // Try to find a hair PhysBone
            #if VRC_SDK_VRCSDK3 && UDON
            var physBones = selectedObject.GetComponentsInChildren<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone>(true);
            var hairBone = System.Array.Find(physBones, pb => pb.name.ToLower().Contains("hair"));
            if (hairBone != null)
            {
                controller.targetPhysBone = hairBone;
                Debug.Log($"Automatically assigned '{hairBone.name}' to PhysBone Light Controller.");
            }
            else
            {
                Debug.LogWarning("Could not automatically find a 'hair' PhysBone. Please assign the target PhysBone manually.", lightObject);
            }
            #endif

            Selection.activeGameObject = lightObject;

            bool maToggleCreated = false;
#if MODULAR_AVATAR
            try
            {
                GameObject toggleControlObject = new GameObject("Dynamic Shadow Toggle");
                Undo.RegisterCreatedObjectUndo(toggleControlObject, "Create Dynamic Shadow Toggle");
                toggleControlObject.transform.SetParent(selectedObject.transform, false);

                // We need to use reflection or SerializedObject because we don't have a direct reference to the MA types
                var menuInstallerType = System.Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarMenuInstaller, nadena.dev.modular-avatar.core");
                if (menuInstallerType != null) toggleControlObject.AddComponent(menuInstallerType);

                var menuItemType = System.Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarMenuItem, nadena.dev.modular-avatar.core");
                if (menuItemType != null)
                {
                    var menuItem = toggleControlObject.AddComponent(menuItemType);
                    var so = new UnityEditor.SerializedObject(menuItem);
                    var control = so.FindProperty("menuItem");
                    control.FindPropertyRelative("name").stringValue = "Dynamic Shadow";
                    control.FindPropertyRelative("icon").objectReferenceValue = null;
                    control.FindPropertyRelative("type").enumValueIndex = 1; // Toggle
                    control.FindPropertyRelative("defaultValue").floatValue = 1.0f; // Default On
                    so.ApplyModifiedProperties();
                }

                var objectToggleType = System.Type.GetType("nadena.dev.modular_avatar.core.ModularAvatarToggle, nadena.dev.modular-avatar.core");
                if (objectToggleType != null)
                {
                    var objectToggle = toggleControlObject.AddComponent(objectToggleType);
                    var so = new UnityEditor.SerializedObject(objectToggle);
                    var objectsToToggle = so.FindProperty("objects");
                    objectsToToggle.arraySize = 1;
                    var element = objectsToToggle.GetArrayElementAtIndex(0);
                    element.FindPropertyRelative("obj").objectReferenceValue = lightObject;
                    so.ApplyModifiedProperties();
                }
                maToggleCreated = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to create Modular Avatar toggle automatically. Please create it manually. Error: " + e.Message);
            }
#endif

            if (maToggleCreated)
            {
                EditorUtility.DisplayDialog("Success", "Successfully set up the PhysBone Light Controller on your avatar.\nA Modular Avatar toggle has also been created.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Success", "Successfully set up the PhysBone Light Controller on your avatar.\n(Modular Avatar not detected, so the toggle was not created automatically).", "OK");
            }
        }
    }
} 
