using UnityEngine;
#if VRCHAT_SDK_AVAILABLE
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
#endif
using System.Collections.Generic;
using System.Linq;

namespace lilToon.PCSS
{
    /// <summary>
    /// PhysBone連動ライト方向制御システム
    /// 競合リアル影システム互換の高度なPhysBone統合機能
    /// </summary>
    [System.Serializable]
    public class PhysBoneLightController : MonoBehaviour
    {
        [Header("🎯 PhysBone Light Direction Control")]
        [SerializeField] private bool enablePhysBoneControl = true;
        [SerializeField] private Transform lightTransform;
        [SerializeField] private Vector3 directionOffset = Vector3.zero;
        [SerializeField] private float responsiveness = 1.0f;
        [SerializeField] private float smoothing = 5.0f;
        
        [Header("🎮 競合製品互換設定")]
        [SerializeField] private bool competitorCompatibilityMode = true;
        [SerializeField] private PhysBoneControlMode controlMode = PhysBoneControlMode.HeadTracking;
        [SerializeField] private float lightFollowStrength = 1.0f;
        [SerializeField] private bool enableAutoDetection = true;
        
        [Header("⚡ 高度な制御オプション")]
        [SerializeField] private bool enableMultiPhysBone = false;
        [SerializeField] private List<PhysBoneTarget> physBoneTargets = new List<PhysBoneTarget>();
        [SerializeField] private bool enableDistanceAttenuation = true;
        [SerializeField] private float maxEffectDistance = 10.0f; // 競合製品デフォルト
        
        [Header("🎨 ライト品質設定")]
        [SerializeField] private LightQualityPreset qualityPreset = LightQualityPreset.HighQuality;
        [SerializeField] private bool enableShadowOptimization = true;
        [SerializeField] private float shadowBias = 0.001f;
        [SerializeField] private float normalBias = 0.1f;
        
        public enum PhysBoneControlMode
        {
            HeadTracking,      // 頭部追従（競合製品標準）
            HandTracking,      // 手部追従
            BodyTracking,      // 体全体追従
            CustomBone,        // カスタムボーン
            MultiPoint         // 複数ポイント
        }
        
        public enum LightQualityPreset
        {
            Mobile,           // Quest互換
            Standard,         // PC標準
            HighQuality,      // PC高品質
            UltraQuality,     // RTX最適化
            CompetitorCompatible // 競合製品互換
        }
        
        [System.Serializable]
        public class PhysBoneTarget
        {
#if VRCHAT_SDK_AVAILABLE
            public VRCPhysBone physBone;
#endif
            public Transform targetTransform;
            public float influence = 1.0f;
            public Vector3 offset = Vector3.zero;
            public bool enableSmoothing = true;
        }
        
        // Private variables
        private Camera playerCamera;
        private Light targetLight;
        private Vector3 lastDirection;
        private float lastUpdateTime;
        private bool isInitialized = false;
        
#if VRCHAT_SDK_AVAILABLE
        private VRCPhysBone primaryPhysBone;
        private List<VRCPhysBone> detectedPhysBones = new List<VRCPhysBone>();
#endif
        
        private void Start()
        {
            InitializePhysBoneController();
        }
        
        private void Update()
        {
            if (!enablePhysBoneControl || !isInitialized) return;
            
            UpdateLightDirection();
            UpdateDistanceAttenuation();
            
            if (enableShadowOptimization)
            {
                OptimizeShadowSettings();
            }
        }
        
        private void InitializePhysBoneController()
        {
            // ライトコンポーネント取得
            if (lightTransform == null)
            {
                lightTransform = transform;
            }
            
            targetLight = lightTransform.GetComponent<Light>();
            if (targetLight == null)
            {
                targetLight = lightTransform.gameObject.AddComponent<Light>();
                SetupDefaultLightSettings();
            }
            
            // カメラ取得
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                playerCamera = FindObjectOfType<Camera>();
            }
            
            // PhysBone自動検出
            if (enableAutoDetection)
            {
                AutoDetectPhysBones();
            }
            
            // 競合製品互換モード設定
            if (competitorCompatibilityMode)
            {
                ApplyCompetitorCompatibleSettings();
            }
            
            isInitialized = true;
            
            Debug.Log("[PhysBone Light Controller] 初期化完了 - 競合製品互換モード: " + competitorCompatibilityMode);
        }
        
        private void UpdateLightDirection()
        {
            Vector3 targetDirection = CalculateTargetDirection();
            
            if (targetDirection != Vector3.zero)
            {
                // スムージング適用
                if (smoothing > 0)
                {
                    targetDirection = Vector3.Slerp(lastDirection, targetDirection, Time.deltaTime * smoothing);
                }
                
                // オフセット適用
                targetDirection += directionOffset;
                
                // ライト方向更新
                lightTransform.rotation = Quaternion.LookRotation(targetDirection);
                lastDirection = targetDirection;
            }
        }
        
        private Vector3 CalculateTargetDirection()
        {
            Vector3 direction = Vector3.forward;
            
            switch (controlMode)
            {
                case PhysBoneControlMode.HeadTracking:
                    direction = CalculateHeadTrackingDirection();
                    break;
                case PhysBoneControlMode.HandTracking:
                    direction = CalculateHandTrackingDirection();
                    break;
                case PhysBoneControlMode.BodyTracking:
                    direction = CalculateBodyTrackingDirection();
                    break;
                case PhysBoneControlMode.CustomBone:
                    direction = CalculateCustomBoneDirection();
                    break;
                case PhysBoneControlMode.MultiPoint:
                    direction = CalculateMultiPointDirection();
                    break;
            }
            
            return direction.normalized;
        }
        
        private Vector3 CalculateHeadTrackingDirection()
        {
            // 競合製品互換のヘッドトラッキング実装
            if (playerCamera != null)
            {
                Vector3 cameraForward = playerCamera.transform.forward;
                
#if VRCHAT_SDK_AVAILABLE
                // VRChatのヘッドトラッキング情報を使用
                var trackingData = VRCPlayerApi.GetTrackingData(VRCPlayerApi.GetLocalPlayer(), VRCPlayerApi.TrackingDataType.Head);
                if (trackingData.position != Vector3.zero)
                {
                    return trackingData.rotation * Vector3.forward;
                }
#endif
                return cameraForward;
            }
            
            return Vector3.forward;
        }
        
        private Vector3 CalculateHandTrackingDirection()
        {
#if VRCHAT_SDK_AVAILABLE
            // 右手のトラッキングデータを使用
            var rightHandData = VRCPlayerApi.GetTrackingData(VRCPlayerApi.GetLocalPlayer(), VRCPlayerApi.TrackingDataType.RightHand);
            var leftHandData = VRCPlayerApi.GetTrackingData(VRCPlayerApi.GetLocalPlayer(), VRCPlayerApi.TrackingDataType.LeftHand);
            
            if (rightHandData.position != Vector3.zero)
            {
                return rightHandData.rotation * Vector3.forward;
            }
            else if (leftHandData.position != Vector3.zero)
            {
                return leftHandData.rotation * Vector3.forward;
            }
#endif
            
            return CalculateHeadTrackingDirection(); // フォールバック
        }
        
        private Vector3 CalculateBodyTrackingDirection()
        {
#if VRCHAT_SDK_AVAILABLE
            // 体全体の向きを計算
            var headData = VRCPlayerApi.GetTrackingData(VRCPlayerApi.GetLocalPlayer(), VRCPlayerApi.TrackingDataType.Head);
            var hipData = VRCPlayerApi.GetTrackingData(VRCPlayerApi.GetLocalPlayer(), VRCPlayerApi.TrackingDataType.Hip);
            
            if (headData.position != Vector3.zero && hipData.position != Vector3.zero)
            {
                Vector3 bodyDirection = (headData.position - hipData.position).normalized;
                return Vector3.ProjectOnPlane(bodyDirection, Vector3.up).normalized;
            }
#endif
            
            return CalculateHeadTrackingDirection(); // フォールバック
        }
        
        private Vector3 CalculateCustomBoneDirection()
        {
#if VRCHAT_SDK_AVAILABLE
            if (primaryPhysBone != null && primaryPhysBone.rootTransform != null)
            {
                return primaryPhysBone.rootTransform.forward;
            }
#endif
            
            return Vector3.forward;
        }
        
        private Vector3 CalculateMultiPointDirection()
        {
            Vector3 combinedDirection = Vector3.zero;
            float totalInfluence = 0f;
            
            foreach (var target in physBoneTargets)
            {
                if (target.targetTransform != null)
                {
                    Vector3 direction = target.targetTransform.forward + target.offset;
                    combinedDirection += direction * target.influence;
                    totalInfluence += target.influence;
                }
            }
            
            if (totalInfluence > 0)
            {
                return combinedDirection / totalInfluence;
            }
            
            return Vector3.forward;
        }
        
        private void UpdateDistanceAttenuation()
        {
            if (!enableDistanceAttenuation || playerCamera == null) return;
            
            float distance = Vector3.Distance(lightTransform.position, playerCamera.transform.position);
            float attenuation = Mathf.Clamp01(1.0f - (distance / maxEffectDistance));
            
            // ライト強度を距離に応じて調整
            if (targetLight != null)
            {
                targetLight.intensity = Mathf.Lerp(0.1f, 2.0f, attenuation * lightFollowStrength);
            }
        }
        
        private void OptimizeShadowSettings()
        {
            if (targetLight == null) return;
            
            // フレームレートに応じてシャドウ品質を動的調整
            float currentFPS = 1.0f / Time.deltaTime;
            
            if (currentFPS < 30f)
            {
                // 低FPS時は品質を下げる
                targetLight.shadowBias = shadowBias * 2.0f;
                targetLight.shadowNormalBias = normalBias * 2.0f;
            }
            else if (currentFPS > 60f)
            {
                // 高FPS時は品質を上げる
                targetLight.shadowBias = shadowBias * 0.5f;
                targetLight.shadowNormalBias = normalBias * 0.5f;
            }
        }
        
        private void AutoDetectPhysBones()
        {
#if VRCHAT_SDK_AVAILABLE
            // アバター内のPhysBoneを自動検出
            var avatar = GetComponentInParent<VRCAvatarDescriptor>();
            if (avatar != null)
            {
                var physBones = avatar.GetComponentsInChildren<VRCPhysBone>();
                detectedPhysBones.AddRange(physBones);
                
                // 頭部のPhysBoneを優先的に選択
                primaryPhysBone = physBones.FirstOrDefault(pb => 
                    pb.name.ToLower().Contains("hair") || 
                    pb.name.ToLower().Contains("head") ||
                    pb.name.ToLower().Contains("face"));
                
                if (primaryPhysBone == null && physBones.Length > 0)
                {
                    primaryPhysBone = physBones[0];
                }
                
                Debug.Log($"[PhysBone Light Controller] 検出されたPhysBone数: {physBones.Length}");
            }
#endif
        }
        
        private void ApplyCompetitorCompatibleSettings()
        {
            // 競合製品互換の設定を適用
            maxEffectDistance = 10.0f; // 競合製品デフォルト
            responsiveness = 1.0f;
            smoothing = 5.0f;
            lightFollowStrength = 1.0f;
            
            // ライト設定を競合製品互換に
            if (targetLight != null)
            {
                targetLight.type = LightType.Directional;
                targetLight.shadows = LightShadows.Soft;
                targetLight.shadowStrength = 0.8f;
                targetLight.shadowBias = 0.001f;
                targetLight.shadowNormalBias = 0.1f;
            }
            
            Debug.Log("[PhysBone Light Controller] 競合製品互換設定を適用しました");
        }
        
        private void SetupDefaultLightSettings()
        {
            if (targetLight == null) return;
            
            switch (qualityPreset)
            {
                case LightQualityPreset.Mobile:
                    targetLight.shadows = LightShadows.Hard;
                    targetLight.shadowResolution = LightShadowResolution.Low;
                    break;
                case LightQualityPreset.Standard:
                    targetLight.shadows = LightShadows.Soft;
                    targetLight.shadowResolution = LightShadowResolution.Medium;
                    break;
                case LightQualityPreset.HighQuality:
                    targetLight.shadows = LightShadows.Soft;
                    targetLight.shadowResolution = LightShadowResolution.High;
                    break;
                case LightQualityPreset.UltraQuality:
                    targetLight.shadows = LightShadows.Soft;
                    targetLight.shadowResolution = LightShadowResolution.VeryHigh;
                    break;
                case LightQualityPreset.CompetitorCompatible:
                    ApplyCompetitorCompatibleSettings();
                    break;
            }
        }
        
        // Public API
        public void SetPhysBoneTarget(Transform target)
        {
#if VRCHAT_SDK_AVAILABLE
            var physBone = target.GetComponent<VRCPhysBone>();
            if (physBone != null)
            {
                primaryPhysBone = physBone;
            }
#endif
        }
        
        public void SetLightFollowStrength(float strength)
        {
            lightFollowStrength = Mathf.Clamp01(strength);
        }
        
        public void TogglePhysBoneControl(bool enabled)
        {
            enablePhysBoneControl = enabled;
        }
        
        public void SetCompetitorCompatibilityMode(bool enabled)
        {
            competitorCompatibilityMode = enabled;
            if (enabled)
            {
                ApplyCompetitorCompatibleSettings();
            }
        }
        
        // Inspector用のヘルパーメソッド
        private void OnValidate()
        {
            responsiveness = Mathf.Clamp(responsiveness, 0.1f, 10.0f);
            smoothing = Mathf.Clamp(smoothing, 0.1f, 20.0f);
            lightFollowStrength = Mathf.Clamp01(lightFollowStrength);
            maxEffectDistance = Mathf.Clamp(maxEffectDistance, 1.0f, 100.0f);
        }
        
        private void OnDrawGizmosSelected()
        {
            // エディタでの可視化
            if (lightTransform != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(lightTransform.position, maxEffectDistance);
                
                Gizmos.color = Color.red;
                Gizmos.DrawRay(lightTransform.position, lightTransform.forward * 5.0f);
            }
        }
    }
} 