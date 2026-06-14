using System.Linq;
using UnityEngine;

namespace lilToon.PCSS.Runtime
{
    [AddComponentMenu("lilToon PCSS Extension/Modular Avatar PCSS Controller")]
    public class ModularAvatarPCSSController : MonoBehaviour
    {
        [Range(0.0f, 1.0f)]
        public float RealtimeQuality = 0.72f;

        public bool AutoLightManagement = false;
        public bool AutoVRCLightVolumes = false;

        [Header("VRChat Performance Gate")]
        public bool EnableVRChatPerformanceGate = true;

        [Range(1.0f, 10.0f)]
        public float MaxRealtimeDistance = 10.0f;

        [Range(0.5f, 10.0f)]
        public float DistanceFade = 3.0f;

        [Header("Gloss Shadow Coherence")]
        public bool EnableGlossShadowCoherence = true;

        [Range(0.0f, 1.0f)]
        public float GlossShadowCoherence = 0.55f;

        [Range(0.0f, 2.0f)]
        public float GlossShadowBoost = 0.35f;

        [Range(0.0f, 1.0f)]
        public float GlossShadowSuppression = 0.45f;

        [Range(0.0f, 2.0f)]
        public float GlossRimStrength = 0.35f;

        [Range(0.0f, 1.0f)]
        public float GlossSmoothness = 0.72f;

        [Header("Level of Detail Settings")]
        public bool EnableLOD = true;

        [System.Serializable]
        public struct LODLevel
        {
            public float Distance;
            public PCSSUtilities.PCSSQuality Quality;
        }

        public LODLevel[] lodLevels =
        {
            new LODLevel { Distance = 2.5f, Quality = PCSSUtilities.PCSSQuality.High },
            new LODLevel { Distance = 6.0f, Quality = PCSSUtilities.PCSSQuality.Medium },
            new LODLevel { Distance = 10.0f, Quality = PCSSUtilities.PCSSQuality.Low },
        };

        private Light pcssLight;
        private MaterialPropertyBlock propertyBlock;
        private Camera mainCamera;
        private Renderer[] cachedRenderers;

        private void Start()
        {
            pcssLight = GetComponentInChildren<Light>(true);
            cachedRenderers = GetComponentsInChildren<Renderer>(true);
            propertyBlock = new MaterialPropertyBlock();
            mainCamera = Camera.main;

            if (lodLevels != null)
            {
                lodLevels = lodLevels.OrderBy(level => level.Distance).ToArray();
            }

            ApplyRuntimeProperties(PCSSUtilities.GetQualityParameters(PCSSUtilities.PCSSQuality.Medium), RealtimeQuality, AutoVRCLightVolumes ? 1.0f : 0.0f);
        }

        private void Update()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null) return;
            }

            if (cachedRenderers == null || cachedRenderers.Length == 0 || !EnableLOD) return;

            float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
            bool qualitySet = TryGetQualityForDistance(distance, out PCSSUtilities.PCSSQuality targetQuality);
            Vector3 qualityParams = qualitySet ? PCSSUtilities.GetQualityParameters(targetQuality) : new Vector3(1.0f, 1.0f, 0.25f);

            float currentLightVolumesState = propertyBlock.GetFloat("_UseVRCLightVolumes");
            float intensity = qualitySet ? Mathf.Clamp01(RealtimeQuality) : 0.0f;
            ApplyRuntimeProperties(qualityParams, intensity, currentLightVolumesState);
        }

        private void OnBecameVisible()
        {
            if (AutoLightManagement && pcssLight != null)
            {
                pcssLight.enabled = true;
            }

            if (AutoVRCLightVolumes)
            {
                ApplyRuntimeProperties(GetCurrentQualityParameters(), Mathf.Clamp01(RealtimeQuality), 1.0f);
            }
        }

        private void OnBecameInvisible()
        {
            if (AutoLightManagement && pcssLight != null)
            {
                pcssLight.enabled = false;
            }

            if (AutoVRCLightVolumes)
            {
                ApplyRuntimeProperties(GetCurrentQualityParameters(), 0.0f, 0.0f);
            }
        }

        private bool TryGetQualityForDistance(float distance, out PCSSUtilities.PCSSQuality quality)
        {
            quality = PCSSUtilities.PCSSQuality.Low;
            if (lodLevels == null || lodLevels.Length == 0) return true;

            foreach (LODLevel lod in lodLevels)
            {
                if (distance < lod.Distance)
                {
                    quality = lod.Quality;
                    return true;
                }
            }

            return distance < MaxRealtimeDistance;
        }

        private Vector3 GetCurrentQualityParameters()
        {
            if (mainCamera == null) return PCSSUtilities.GetQualityParameters(PCSSUtilities.PCSSQuality.Medium);

            float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
            if (TryGetQualityForDistance(distance, out PCSSUtilities.PCSSQuality quality))
            {
                return PCSSUtilities.GetQualityParameters(quality);
            }

            return new Vector3(1.0f, 1.0f, 0.25f);
        }

        private void ApplyRuntimeProperties(Vector3 qualityParams, float intensity, float lightVolumeState)
        {
            if (propertyBlock == null)
            {
                propertyBlock = new MaterialPropertyBlock();
            }

            propertyBlock.SetFloat("_LocalPCSSSamples", Mathf.Max(1.0f, qualityParams.x));
            propertyBlock.SetFloat("_PCSSSamples", Mathf.Max(1.0f, qualityParams.x));
            propertyBlock.SetFloat("_BlockerSamples", Mathf.Max(1.0f, qualityParams.y));
            propertyBlock.SetFloat("_FilterScale", Mathf.Max(0.1f, qualityParams.z));
            propertyBlock.SetFloat("_PCSSIntensity", Mathf.Clamp01(intensity));

            propertyBlock.SetFloat("_UseVRChatPerformanceGate", EnableVRChatPerformanceGate ? 1.0f : 0.0f);
            propertyBlock.SetFloat("_PCSSMaxDistance", Mathf.Clamp(MaxRealtimeDistance, 1.0f, 10.0f));
            propertyBlock.SetFloat("_PCSSDistanceFade", Mathf.Max(0.5f, DistanceFade));

            propertyBlock.SetFloat("_UseGlossShadowCoherence", EnableGlossShadowCoherence ? 1.0f : 0.0f);
            propertyBlock.SetFloat("_GlossShadowCoherence", Mathf.Clamp01(GlossShadowCoherence));
            propertyBlock.SetFloat("_GlossShadowBoost", Mathf.Max(0.0f, GlossShadowBoost));
            propertyBlock.SetFloat("_GlossShadowSuppression", Mathf.Clamp01(GlossShadowSuppression));
            propertyBlock.SetFloat("_GlossRimStrength", Mathf.Max(0.0f, GlossRimStrength));
            propertyBlock.SetFloat("_GlossSmoothness", Mathf.Clamp01(GlossSmoothness));

            propertyBlock.SetFloat("_UseVRCLightVolumes", lightVolumeState);
            propertyBlock.SetFloat("_VRCLightVolumesEnabled", lightVolumeState);

            ApplyPropertyBlock();
        }

        private void ApplyPropertyBlock()
        {
            if (cachedRenderers == null) return;

            foreach (Renderer renderer in cachedRenderers)
            {
                if (renderer != null)
                {
                    renderer.SetPropertyBlock(propertyBlock);
                }
            }
        }
    }
}
