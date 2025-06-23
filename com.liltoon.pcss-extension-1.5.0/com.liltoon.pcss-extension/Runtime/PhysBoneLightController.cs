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
    [AddComponentMenu("lilToon PCSS/PhysBone Light Controller")]
    public class PhysBoneLightController : MonoBehaviour
    {
        [Header("🎯 PhysBone連動ライト制御")]
        [SerializeField] private bool enablePhysBoneControl = true;
        [SerializeField] private Transform lightTransform;
        [SerializeField] private Vector3 directionOffset = Vector3.zero;
        [SerializeField, Range(0.1f, 20f)] private float smoothing = 5.0f;
        [Tooltip("外部のライトオブジェクトを制御対象にします。未設定の場合はこのゲームオブジェクトのライトを使用します。")]
        [SerializeField] public Light externalLight;

        [Header("🎮 先行製品互換設定")]
        [SerializeField] private bool competitorCompatibilityMode = true;
        [SerializeField] private ControlMode controlMode = ControlMode.HeadTracking;
        [SerializeField, Range(0f, 1f)] private float lightFollowStrength = 1.0f;
        [SerializeField] private bool enableAutoDetection = true;

        [Header("⚡ 高度な制御")]
        [SerializeField] private bool enableDistanceAttenuation = true;
        [SerializeField, Range(1f, 100f)] private float maxEffectDistance = 10.0f; // 先行製品のデフォルト値

        public enum ControlMode
        {
            HeadTracking,
            HandTracking,
            BodyTracking
        }

        private Light targetLight;
        private Vector3 lastDirection;

        void Start()
        {
            Initialize();
        }

        void LateUpdate()
        {
            if (!enablePhysBoneControl || !gameObject.activeInHierarchy || targetLight == null) return;
            UpdateLightDirection();
            UpdateDistanceAttenuation();
        }

        public void Initialize()
        {
            if (lightTransform == null)
            {
                lightTransform = transform;
            }

            if (externalLight != null)
            {
                targetLight = externalLight;
            }
            else
            {
                targetLight = lightTransform.GetComponent<Light>();
            }

            if (targetLight == null)
            {
                Debug.LogError("[PhysBoneLightController] 対象のLightコンポーネントが見つかりません。externalLightが設定されているか、このオブジェクトにLightコンポーネントがアタッチされているか確認してください。", this);
                enablePhysBoneControl = false;
                return;
            }
            lastDirection = lightTransform.forward;
        }

        private void UpdateLightDirection()
        {
            Vector3 targetDirection = CalculateTargetDirection();

            if (targetDirection != Vector3.zero)
            {
                Vector3 currentForward = lastDirection;
                if(smoothing > 0)
                {
                    currentForward = Vector3.Slerp(lastDirection, targetDirection, Time.deltaTime * smoothing);
                }
                else
                {
                    currentForward = targetDirection;
                }
                
                targetLight.transform.rotation = Quaternion.LookRotation(currentForward) * Quaternion.Euler(directionOffset);
                lastDirection = currentForward;
            }
        }

        private Vector3 CalculateTargetDirection()
        {
#if VRC_SDK_VRCSDK3
            var player = VRCPlayerApi.GetLocalPlayer();
            if (player == null) return transform.forward;

            VRCPlayerApi.TrackingDataType trackingType;
            switch (controlMode)
            {
                case ControlMode.HandTracking:
                    trackingType = VRCPlayerApi.TrackingDataType.RightHand;
                    break;
                case ControlMode.BodyTracking:
                    trackingType = VRCPlayerApi.TrackingDataType.Hip;
                    break;
                case ControlMode.HeadTracking:
                default:
                    trackingType = VRCPlayerApi.TrackingDataType.Head;
                    break;
            }

            var trackingData = player.GetTrackingData(trackingType);
            return trackingData.rotation * Vector3.forward;
#else
            var camera = Camera.main;
            if (camera != null && camera.enabled)
            {
                return camera.transform.forward;
            }
            return Vector3.forward;
#endif
        }

        private void UpdateDistanceAttenuation()
        {
            if (!enableDistanceAttenuation) return;
            
#if VRC_SDK_VRCSDK3
            var player = VRCPlayerApi.GetLocalPlayer();
            if (player == null) return;
            
            var headData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            float distance = Vector3.Distance(lightTransform.position, headData.position);
            float attenuation = 1.0f - Mathf.Clamp01(distance / maxEffectDistance);

            targetLight.intensity = Mathf.Lerp(0.0f, 2.0f, attenuation * lightFollowStrength);
#else
            var camera = Camera.main;
             if (camera != null && camera.enabled)
            {
                float distance = Vector3.Distance(lightTransform.position, camera.transform.position);
                float attenuation = 1.0f - Mathf.Clamp01(distance / maxEffectDistance);
                targetLight.intensity = Mathf.Lerp(0.0f, 2.0f, attenuation * lightFollowStrength);
            }
#endif
        }

        private void OnDrawGizmosSelected()
        {
            if (lightTransform != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(lightTransform.position, maxEffectDistance);
                
                Gizmos.color = Color.red;
                Gizmos.DrawRay(lightTransform.position, lightTransform.forward * 1.0f);
            }
        }
    }
} 