#if VRC_SDK_VRCSDK3
using UnityEngine;
using VRC.SDKBase.Editor.BuildPipeline;

namespace lilToon.PCSS.Editor
{
    public sealed class PCSSNDMFAAOPreprocessHook : IVRCSDKPreprocessAvatarCallback
    {
        public int callbackOrder => -12000;

        public bool OnPreprocessAvatar(GameObject avatarGameObject)
        {
            if (avatarGameObject == null) return true;

            if (PCSSAvatarPerformanceGuard.HasAllowAvatarLightsMarker(avatarGameObject))
            {
                Debug.LogWarning($"[PCSS] AAO performance guard skipped on {avatarGameObject.name} because PCSS avatar Lights were explicitly allowed.");
            }
            else
            {
                PCSSAvatarPerformanceGuard.PerformanceSummary performanceSummary =
                    PCSSAvatarPerformanceGuard.Apply(avatarGameObject, useUndo: false, removeGeneratedLightControls: true, tuneMaterials: true);
                if (performanceSummary.TotalChanges > 0)
                {
                    Debug.Log(
                        $"[PCSS] AAO performance guard applied on {avatarGameObject.name}: " +
                        $"{performanceSummary.RemovedLightObjects} generated PCSS Light object(s) removed, " +
                        $"{performanceSummary.RemovedOtherLightComponents} other avatar Light component(s) removed, " +
                        $"{performanceSummary.TunedMaterials} material(s) tuned, " +
                        $"{performanceSummary.RemainingLights} avatar Light(s) remain.");
                }
            }

            PCSSNDMFAAOFixUtility.RepairSummary summary = PCSSNDMFAAOFixUtility.RepairAvatar(avatarGameObject);
            int repaired = summary.BoneProxyFixed + summary.MenuInstallerFixed + summary.EmptyInstallerRemoved;
            if (repaired > 0)
            {
                Debug.Log($"[PCSS] NDMF/AAO build workflow auto-repair applied to {repaired} generated PCSS item(s) on {avatarGameObject.name}.");
            }

            return true;
        }
    }
}
#endif
