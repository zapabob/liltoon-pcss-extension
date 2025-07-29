using UnityEngine;
#if VRC_SDK_VRCSDK3
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
#endif

namespace lilToon.PCSS
{
    /// <summary>
    /// PhysBone縺ｨ騾｣蜍輔＠縺ｦ繧ｨ繝溘ャ繧ｷ繝厄ｼ育匱蜈会ｼ峨・繝・Μ繧｢繝ｫ繧貞宛蠕｡縺吶ｋ繧ｳ繝ｳ繝昴・繝阪Φ繝医・
    /// AutoFIX蟇ｾ遲匁ｸ医∩縲ゅお繝溘ャ繧ｷ繝悶・縺ｿ縺ｧ蜈芽｡ｨ迴ｾ繧貞ｮ溽樟縺励｀odular Avatar繧ПhysBone騾｣謳ｺ繧ょｯｾ蠢懊・
    /// </summary>
#if UNITY_EDITOR
    [AddComponentMenu("lilToon PCSS/PhysBone Emissive Controller")]
#endif
    public class PhysBoneEmissiveController : MonoBehaviour
    {
        public enum EmissivePresetPosition
        {
            Free,      // 閾ｪ逕ｱ遘ｻ蜍・
            Ground,    // 蝨ｰ髱｢縺九ｉ1.5m
            Head,      // 鬆ｭ荳奇ｼ郁ｦｪ縺ｮ+1.7m・・
            Hand,      // 謇句・・郁ｦｪ縺ｮ+1.0m, 讓ｪ縺ｫ0.3m・・
        }

        [Header("PhysBone騾｣蜍輔お繝溘ャ繧ｷ繝門宛蠕｡・医い繝舌ち繝ｼ逕ｨ騾費ｼ・)]
        [SerializeField] private Renderer _emissiveRenderer;
        [SerializeField] private int _emissiveMaterialIndex = 0;
        [SerializeField] private string _emissionProperty = "_EmissionColor";
        [SerializeField] private Color _baseEmission = Color.white;
        [SerializeField, Range(0, 10)] private float _maxEmission = 5f;
        [SerializeField, Range(0f, 1f)] private float _emissionStrength = 1f;
        [Header("Flicker・域昭繧峨℃・牙柑譫・)]
        [SerializeField] private bool _enableFlicker = false;
        [SerializeField, Range(0f, 0.2f)] private float _flickerStrength = 0.1f;
        [SerializeField, Range(0.01f, 0.5f)] private float _flickerSpeed = 0.1f;
        [Header("繧ｨ繝溘ャ繧ｷ繝紋ｽ咲ｽｮ繝励Μ繧ｻ繝・ヨ")]
        [SerializeField] private EmissivePresetPosition _presetPosition = EmissivePresetPosition.Free;
        [SerializeField] private Transform _referenceRoot;
        [SerializeField] private bool _snapToPreset = false;

        private MaterialPropertyBlock _mpb;
        private float _flickerTimer = 0f;
        private float _flick = 1f;

        void Start()
        {
            if (_emissiveRenderer != null)
                _mpb = new MaterialPropertyBlock();
            if (_snapToPreset)
                SnapToPresetPosition();
        }

        void Update()
        {
            if (_emissiveRenderer == null) return;
            if (_enableFlicker)
            {
                _flickerTimer += Time.deltaTime;
                if (_flickerTimer > _flickerSpeed)
                {
                    _flick = 1f + Random.Range(-_flickerStrength, _flickerStrength);
                    _flickerTimer = 0f;
                }
            }
            else
            {
                _flick = 1f;
            }
            float finalStrength = _emissionStrength * _maxEmission * _flick;
            _emissiveRenderer.GetPropertyBlock(_mpb, _emissiveMaterialIndex);
            _mpb.SetColor(_emissionProperty, _baseEmission * finalStrength);
            _emissiveRenderer.SetPropertyBlock(_mpb, _emissiveMaterialIndex);

            if (_snapToPreset)
                SnapToPresetPosition();
        }

        private void SnapToPresetPosition()
        {
            if (_presetPosition == EmissivePresetPosition.Free) return;
            Vector3 pos = transform.position;
            if (_referenceRoot == null) _referenceRoot = transform.parent;
            switch (_presetPosition)
            {
                case EmissivePresetPosition.Ground:
                    pos = new Vector3(pos.x, 1.5f, pos.z);
                    break;
                case EmissivePresetPosition.Head:
                    if (_referenceRoot != null)
                        pos = _referenceRoot.position + Vector3.up * 1.7f;
                    else
                        pos = new Vector3(pos.x, 1.7f, pos.z);
                    break;
                case EmissivePresetPosition.Hand:
                    if (_referenceRoot != null)
                        pos = _referenceRoot.position + Vector3.up * 1.0f + Vector3.right * 0.3f;
                    else
                        pos = new Vector3(pos.x + 0.3f, 1.0f, pos.z);
                    break;
            }
            transform.position = pos;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // 蠢・ｦ√↑繧烏izmos謠冗判
        }
#endif
    }
} 
