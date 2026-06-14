#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace lilToon.PCSS.Editor
{
    internal static class PCSSCameraDepthPreview
    {
        [MenuItem("Tools/lilToon-PCSS-Extension/Preview/Enable Camera Depth Texture")]
        private static void EnableDepthTexture()
        {
            int updated = ApplyDepthMode(true);
            EditorUtility.DisplayDialog(
                "PCSS Camera Depth Preview",
                $"Depth texture preview enabled on {updated} camera(s).",
                "OK");
        }

        [MenuItem("Tools/lilToon-PCSS-Extension/Preview/Disable Camera Depth Texture")]
        private static void DisableDepthTexture()
        {
            int updated = ApplyDepthMode(false);
            EditorUtility.DisplayDialog(
                "PCSS Camera Depth Preview",
                $"Depth texture preview disabled on {updated} camera(s).",
                "OK");
        }

        private static int ApplyDepthMode(bool enabled)
        {
            int updated = 0;

            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null && sceneView.camera != null)
            {
                SetDepthMode(sceneView.camera, enabled);
                updated++;
            }

            Camera[] cameras = Object.FindObjectsOfType<Camera>(true);
            foreach (Camera camera in cameras)
            {
                if (camera == null) continue;
                SetDepthMode(camera, enabled);
                EditorUtility.SetDirty(camera);
                updated++;
            }

            SceneView.RepaintAll();
            return updated;
        }

        private static void SetDepthMode(Camera camera, bool enabled)
        {
            if (camera == null) return;

            if (enabled)
            {
                camera.depthTextureMode |= DepthTextureMode.Depth;
            }
            else
            {
                camera.depthTextureMode &= ~DepthTextureMode.Depth;
            }
        }
    }
}
#endif
