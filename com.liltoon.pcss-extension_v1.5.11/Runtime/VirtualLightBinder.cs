using UnityEngine;

public class VirtualLightBinder : MonoBehaviour
{
    public Transform virtualLight;
    public bool shadowEnabled = true;
    public Color shadowColor = Color.black;
    public float shadowIntensity = 1.0f;

    private Renderer[] targetRenderers;
    private MaterialPropertyBlock mpb;

    void Start()
    {
        // SkinnedMeshRenderer/Rendererを自動検出
        targetRenderers = GetComponentsInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (virtualLight == null || targetRenderers == null) return;
        Vector3 dir = virtualLight.forward;
        Vector3 pos = virtualLight.position;
        foreach (var r in targetRenderers)
        {
            r.GetPropertyBlock(mpb);
            mpb.SetVector("_VirtualLightDir", dir);
            mpb.SetVector("_VirtualLightPos", pos);
            mpb.SetFloat("_ShadowEnabled", shadowEnabled ? 1f : 0f);
            mpb.SetColor("_ShadowColor", shadowColor);
            mpb.SetFloat("_ShadowIntensity", shadowIntensity);
            r.SetPropertyBlock(mpb);
        }
    }
} 