using UnityEngine;

namespace lilToon.PCSS.Runtime
{
    public class VirtualLightBinder : MonoBehaviour
    {
        [SerializeField] private Light targetLight;
        [SerializeField] private bool shadowEnabled = true;
        [SerializeField] private Color shadowColor = Color.black;
        [SerializeField] private float shadowIntensity = 1.0f;

        private Renderer[] targetRenderers;
        private MaterialPropertyBlock mpb;

        void Start()
        {
            targetRenderers = GetComponentsInChildren<Renderer>();
            mpb = new MaterialPropertyBlock();
            EnsureLightExists();
        }

        void LateUpdate()
        {
            if (targetLight == null || targetRenderers == null) return;
            Vector3 dir = targetLight.transform.forward;
            Vector3 pos = targetLight.transform.position;
            foreach (var r in targetRenderers)
            {
                if (r == null) continue;
                r.GetPropertyBlock(mpb);
                mpb.SetVector("_VirtualLightDir", dir);
                mpb.SetVector("_VirtualLightPos", pos);
                mpb.SetFloat("_ShadowEnabled", shadowEnabled ? 1f : 0f);
                mpb.SetColor("_ShadowColor", shadowColor);
                mpb.SetFloat("_ShadowIntensity", shadowIntensity);
                r.SetPropertyBlock(mpb);
            }
        }

        // ModularAvatarトグル連携用
        public void OnToggleLight(bool isOn)
        {
            SetLightEnabled(isOn);
        }

        // Lightの有効/無効を切り替える
        public void SetLightEnabled(bool enabled)
        {
            EnsureLightExists();
            if (targetLight != null)
                targetLight.enabled = enabled;
        }

        // Lightがなければ自動生成
        public void EnsureLightExists()
        {
            if (targetLight == null)
            {
                var lightObj = new GameObject("AvatarLight");
                lightObj.transform.SetParent(transform, false);
                targetLight = lightObj.AddComponent<Light>();
                targetLight.type = LightType.Point;
                targetLight.range = 10f;
                targetLight.intensity = 1f;
            }
        }

        // Editor拡張から呼び出す用
        public Light GetOrCreateLight()
        {
            EnsureLightExists();
            return targetLight;
        }
    }
} 