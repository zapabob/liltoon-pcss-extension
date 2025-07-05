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
            PCSSSetupCenterWindow.ShowWindow();
        }

        [MenuItem(PhysBoneMenuPath)]
        public static void SetupPhysBoneLightController()
        {
            PCSSSetupCenterWindow.ShowWindow();
        }
    }
} 