using UnityEngine;
#if VRC_SDK_VRCSDK3
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
#endif

namespace lilToon.PCSS.Runtime
{
    /// <summary>
    /// PhysBoneと連動してエミッシブ（発光）マテリアルを制御するコンポーネント。
    /// AutoFIX対策済み。エミッシブのみで光表現を実現し、Modular AvatarやPhysBone連携も対応。
    /// </summary>
#if UNITY_EDITOR
    [AddComponentMenu("lilToon PCSS/PhysBone Emissive Controller")]
#endif
    public class PhysBoneEmissiveController : MonoBehaviour
    {
        public enum EmissivePresetPosition
        {
            Free,      // 自由移動
            Ground,    // 地面から1.5m
            Head,      // 頭上（親の+1.7m）
            Hand,      // 手元（親の+1.0m, 横に0.3m）
        }

        [Header("PhysBone連動エミッシブ制御（アバター用途）")]
        [SerializeField] private Renderer _emissiveRenderer;
        [SerializeField] private int _emissiveMaterialIndex = 0;
        [SerializeField] private string _emissionProperty = "_EmissionColor";
        [SerializeField] private Color _baseEmission = Color.white;
        [SerializeField, Range(0, 10)] private float _maxEmission = 5f;
        [SerializeField, Range(0f, 1f)] private float _emissionStrength = 1f;
        [Header("Flicker（揺らぎ）効果")]
        [SerializeField] private bool _enableFlicker = false;
        [SerializeField, Range(0f, 0.2f)] private float _flickerStrength = 0.1f;
        [SerializeField, Range(0.01f, 0.5f)] private float _flickerSpeed = 0.1f;
        [Header("エミッシブ位置プリセット")]
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
            // 必要ならGizmos描画
        }
#endif
    }
} 