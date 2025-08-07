using UnityEngine;
using System.Linq;

namespace lilToon.PCSS.Runtime
{
    [AddComponentMenu("lilToon PCSS Extension/Modular Avatar PCSS Controller")]
    public class ModularAvatarPCSSController : MonoBehaviour
    {
        [Range(0.0f, 1.0f)]
        public float RealtimeQuality = 1.0f;

        public bool AutoLightManagement = true;
        public bool AutoVRCLightVolumes = true;

        [Header("Level of Detail Settings")]
        public bool EnableLOD = true;

        [System.Serializable]
        public struct LODLevel
        {
            public float Distance;
            public PCSSUtilities.PCSSQuality Quality;
        }

        public LODLevel[] lodLevels = new LODLevel[]
        {
            new LODLevel { Distance = 10.0f, Quality = PCSSUtilities.PCSSQuality.Ultra },
            new LODLevel { Distance = 20.0f, Quality = PCSSUtilities.PCSSQuality.High },
            new LODLevel { Distance = 40.0f, Quality = PCSSUtilities.PCSSQuality.Medium },
            new LODLevel { Distance = 60.0f, Quality = PCSSUtilities.PCSSQuality.Low },
        };

        private Light pcssLight;
        private MaterialPropertyBlock propertyBlock;
        private Camera mainCamera;
        private Renderer[] cachedRenderers;

        void Start()
        {
            pcssLight = GetComponentInChildren<Light>();
            cachedRenderers = GetComponentsInChildren<Renderer>();
            propertyBlock = new MaterialPropertyBlock();
            mainCamera = Camera.main;

            // Sort LOD levels by distance to ensure correct evaluation
            if (lodLevels != null)
            {
                lodLevels = lodLevels.OrderBy(l => l.Distance).ToArray();
            }

            // Initialize VRC Light Volumes state
            if (AutoVRCLightVolumes)
            {
                propertyBlock.SetFloat("_UseVRCLightVolumes", 1.0f); // Default to ON
                foreach (var renderer in cachedRenderers)
                {
                    renderer.SetPropertyBlock(propertyBlock);
                }
            }
        }

        void Update()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null) return; // Still no camera, exit
            }

            if (cachedRenderers == null || !EnableLOD) return;

            float distance = Vector3.Distance(transform.position, mainCamera.transform.position);

            PCSSUtilities.PCSSQuality targetQuality = PCSSUtilities.PCSSQuality.Low;
            bool qualitySet = false;

            foreach (var lod in lodLevels)
            {
                if (distance < lod.Distance)
                {
                    targetQuality = lod.Quality;
                    qualitySet = true;
                    break;
                }
            }

            Vector3 qualityParams;
            if (!qualitySet)
            {
                // Furthest distance, turn off PCSS by setting samples to 0
                qualityParams = new Vector3(0, 0, 0);
            }
            else
            {
                qualityParams = PCSSUtilities.GetQualityParameters(targetQuality);
            }

            // Preserve current _UseVRCLightVolumes state before setting other properties
            float currentVRCLightVolumesState = propertyBlock.GetFloat("_UseVRCLightVolumes");

            propertyBlock.SetFloat("_LocalPCSSSamples", qualityParams.x);
            // You can also set other quality parameters here if needed
            // propertyBlock.SetFloat("_BlockerSamples", qualityParams.y);
            // propertyBlock.SetFloat("_FilterScale", qualityParams.z);

            // Restore _UseVRCLightVolumes state
            propertyBlock.SetFloat("_UseVRCLightVolumes", currentVRCLightVolumesState);

            foreach (var renderer in cachedRenderers)
            {
                renderer.SetPropertyBlock(propertyBlock);
            }
        }

        void OnBecameVisible()
        {
            if (AutoLightManagement && pcssLight != null)
            {
                pcssLight.enabled = true;
            }
            if (AutoVRCLightVolumes)
            {
                propertyBlock.SetFloat("_UseVRCLightVolumes", 1.0f);
                foreach (var renderer in cachedRenderers)
                {
                    renderer.SetPropertyBlock(propertyBlock);
                }
            }
        }

        void OnBecameInvisible()
        {
            if (AutoLightManagement && pcssLight != null)
            {
                pcssLight.enabled = false;
            }
            if (AutoVRCLightVolumes)
            {
                propertyBlock.SetFloat("_UseVRCLightVolumes", 0.0f);
                foreach (var renderer in cachedRenderers)
                {
                    renderer.SetPropertyBlock(propertyBlock);
                }
            }
        }
    }
}
