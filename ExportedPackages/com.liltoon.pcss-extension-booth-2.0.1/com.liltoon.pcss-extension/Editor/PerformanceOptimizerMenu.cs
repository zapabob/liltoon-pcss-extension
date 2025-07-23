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
#if VRCHAT_SDK_AVAILABLE
        public VRCPhysBone targetPhysBone;
#endif
    }

    public class PerformanceOptimizerMenu : EditorWindow
    {
        private const string MenuPath = "Tools/lilToon PCSS Extension/Setup Performance Tuner";
        private const string PhysBoneMenuPath = "Tools/lilToon PCSS Extension/Setup PhysBone Light Controller";
        private const string GameObjectName = "PCSS Performance Tuner";

        [MenuItem(MenuPath)]
        private static void SetupPerformanceTuner()
        {
            // シーン内の既にオプティマイザーが存在するか確認
            VRChatPerformanceOptimizer existingOptimizer = Object.FindObjectOfType<VRChatPerformanceOptimizer>();
            if (existingOptimizer != null)
            {
                Debug.Log($"'{GameObjectName}' already exists in the scene. Selecting it for you.");
                Selection.activeObject = existingOptimizer.gameObject;
                return;
            }
            
            // 新しいGameObjectを作成し、コンポーネントをアタッチ
            GameObject optimizerObject = new GameObject(GameObjectName);
            optimizerObject.AddComponent<VRChatPerformanceOptimizer>();

            // 操作をUndo可能にする
            Undo.RegisterCreatedObjectUndo(optimizerObject, $"Create {GameObjectName}");

            // 作成したオブジェクトを選択
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
#if VRCHAT_SDK_AVAILABLE
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
#if VRCHAT_SDK_AVAILABLE
            var physBones = selectedObject.GetComponentsInChildren<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone>(true);
            var hairBone = System.Array.Find(physBones, pb => pb.name.ToLower().Contains("hair"));
            if (hairBone != null)
            {
                controller.targetPhysBone = hairBone;
                Debug.Log($"Found hair PhysBone: {hairBone.name}");
            }
#endif

            Selection.activeObject = lightObject;
            Debug.Log("PhysBone Light Controller setup complete. The light will now follow the avatar's head movement.");
        }
    }
} 
