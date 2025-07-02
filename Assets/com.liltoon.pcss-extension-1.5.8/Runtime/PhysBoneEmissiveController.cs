// MIT License
// Copyright (c) 2025 lilToon PCSS Extension Team
// See LICENSE file in the project root for full license information.

using UnityEngine;
#if UNITY_EDITOR || VRC_SDK_VRCSDK3
// using System.Collections.Generic;
// using System.Linq;
#endif

#if VRC_SDK_VRCSDK3
using VRC.SDKBase;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;
#endif

namespace lilToon.PCSS
{
    /// <summary>
    /// PhysBoneと連動してライトの方向を制御するシステム。
    /// 先行製品の機能を参考にしつつ、より高度な統合機能を提供します。
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
        [SerializeField] private Renderer emissiveRenderer;
        [SerializeField] private int emissiveMaterialIndex = 0;
        [SerializeField] private string emissionProperty = "_EmissionColor";
        [SerializeField] private Color baseEmission = Color.white;
        [SerializeField, Range(0, 10)] private float maxEmission = 5f;
        [SerializeField, Range(0f, 1f)] private float emissionStrength = 1f;
        [Header("Flicker（揺らぎ）効果")]
        [SerializeField] private bool enableFlicker = false;
        [SerializeField, Range(0f, 1f)] private float flickerStrength = 0.2f;
        [SerializeField, Range(0.01f, 0.5f)] private float flickerSpeed = 0.1f;
        [Header("エミッシブ位置プリセット")]
        [SerializeField] private EmissivePresetPosition presetPosition = EmissivePresetPosition.Free;
        [SerializeField] private Transform referenceRoot; // 頭や手の基準
        [SerializeField] private bool snapToPreset = false;

        private MaterialPropertyBlock mpb;
        private float flickerTimer = 0f;
        private float flick = 1f;

        void Start()
        {
            if (emissiveRenderer != null)
                mpb = new MaterialPropertyBlock();
            if (snapToPreset)
                SnapToPresetPosition();
        }

        void Update()
        {
            if (emissiveRenderer == null) return;
            if (enableFlicker)
            {
                flickerTimer += Time.deltaTime;
                if (flickerTimer > flickerSpeed)
                {
                    flick = 1f + Random.Range(-flickerStrength, flickerStrength);
                    flickerTimer = 0f;
                }
            }
            else
            {
                flick = 1f;
            }
            float finalStrength = emissionStrength * maxEmission * flick;
            emissiveRenderer.GetPropertyBlock(mpb, emissiveMaterialIndex);
            mpb.SetColor(emissionProperty, baseEmission * finalStrength);
            emissiveRenderer.SetPropertyBlock(mpb, emissiveMaterialIndex);

            if (snapToPreset)
                SnapToPresetPosition();
        }

        private void SnapToPresetPosition()
        {
            if (presetPosition == EmissivePresetPosition.Free) return;
            Vector3 pos = transform.position;
            if (referenceRoot == null) referenceRoot = transform.parent;
            switch (presetPosition)
            {
                case EmissivePresetPosition.Ground:
                    pos = new Vector3(pos.x, 1.5f, pos.z);
                    break;
                case EmissivePresetPosition.Head:
                    if (referenceRoot != null)
                        pos = referenceRoot.position + Vector3.up * 1.7f;
                    else
                        pos = new Vector3(pos.x, 1.7f, pos.z);
                    break;
                case EmissivePresetPosition.Hand:
                    if (referenceRoot != null)
                        pos = referenceRoot.position + Vector3.up * 1.0f + Vector3.right * 0.3f;
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