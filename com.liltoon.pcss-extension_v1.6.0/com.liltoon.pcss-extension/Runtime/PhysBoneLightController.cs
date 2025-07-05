using UnityEngine;
#if VRC_SDK_VRCSDK3
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
#endif
using System.Collections.Generic;
using System.Linq;

namespace lilToon.PCSS
{
    /// <summary>
    /// PhysBoneと連動してライトの方向を制御するシステム。
    /// 先行製品の機能を参考にしつつ、より高度な統合機能を提供します。
    /// </summary>
#if UNITY_EDITOR
    [AddComponentMenu("lilToon PCSS/PhysBone Light Controller")]
#endif
    public class PhysBoneLightController : MonoBehaviour
    {
        [Header("🎯 PhysBone連動エミッシブ制御")]
        [SerializeField] private Renderer emissiveRenderer;
        [SerializeField] private Color baseEmission = Color.white;
        [SerializeField, Range(0, 10)] private float emissionStrength = 1.0f;
        [SerializeField, Range(0.1f, 20f)] private float smoothing = 5.0f;
        [SerializeField] private bool enableDistanceAttenuation = true;
        [SerializeField, Range(1f, 100f)] private float maxEffectDistance = 10.0f;

        private Material emissiveMaterial;
        private Color lastEmission;

        void Start()
        {
            if (emissiveRenderer != null)
            {
                emissiveMaterial = emissiveRenderer.material;
                emissiveMaterial.EnableKeyword("_EMISSION");
                lastEmission = baseEmission * emissionStrength;
                emissiveMaterial.SetColor("_EmissionColor", lastEmission);
            }
        }

        void LateUpdate()
        {
            if (emissiveMaterial == null) return;
            float attenuation = 1.0f;
            if (enableDistanceAttenuation)
            {
#if VRC_SDK_VRCSDK3
                var player = VRC.SDKBase.Networking.LocalPlayer;
                if (player != null)
                {
                    var headData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
                    float distance = Vector3.Distance(transform.position, headData.position);
                    attenuation = 1.0f - Mathf.Clamp01(distance / maxEffectDistance);
                }
#else
                var camera = Camera.main;
                if (camera != null)
                {
                    float distance = Vector3.Distance(transform.position, camera.transform.position);
                    attenuation = 1.0f - Mathf.Clamp01(distance / maxEffectDistance);
                }
#endif
            }
            Color targetEmission = baseEmission * emissionStrength * attenuation;
            lastEmission = Color.Lerp(lastEmission, targetEmission, Time.deltaTime * smoothing);
            emissiveMaterial.SetColor("_EmissionColor", lastEmission);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (transform != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, maxEffectDistance);
                
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, transform.forward * 1.0f);
            }
        }
#endif
    }
} 