using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace lilToon.PCSS
{
    /// <summary>
    /// 高度なシャドウマスクシステム
    /// 競合製品互換のCastMask/ReceiveMask機能とアニメ調影境界制御
    /// </summary>
    [System.Serializable]
    public class ShadowMaskSystem : MonoBehaviour
    {
        [Header("🎭 シャドウマスク制御")]
        [SerializeField] private bool enableShadowMask = true;
        [SerializeField] private ShadowMaskMode maskMode = ShadowMaskMode.CompetitorCompatible;
        [SerializeField] private LayerMask castMask = -1;
        [SerializeField] private LayerMask receiveMask = -1;
        
        [Header("🎨 アニメ調影境界制御")]
        [SerializeField] private bool enableShadowClamp = true;
        [SerializeField] private float shadowClampThreshold = 0.5f;
        [SerializeField] private float shadowSharpness = 2.0f;
        [SerializeField] private AnimationCurve shadowFalloff = AnimationCurve.Linear(0, 0, 1, 1);
        
        [Header("🌟 半透明オブジェクト対応")]
        [SerializeField] private bool enableTransparentShadows = true;
        [SerializeField] private float transparentShadowStrength = 0.7f;
        [SerializeField] private List<Renderer> transparentRenderers = new List<Renderer>();
        
        [Header("⚡ パフォーマンス最適化")]
        [SerializeField] private bool enableLODShadows = true;
        [SerializeField] private float[] lodDistances = { 10f, 25f, 50f };
        [SerializeField] private ShadowQuality[] lodQualities = { 
            ShadowQuality.Ultra, 
            ShadowQuality.High, 
            ShadowQuality.Medium 
        };
        
        [Header("🎯 競合製品互換設定")]
        [SerializeField] private bool competitorCompatibilityMode = true;
        [SerializeField] private float worldLightInfluence = 1.0f;
        [SerializeField] private bool enableWorldColorReflection = true;
        [SerializeField] private Color ambientShadowColor = new Color(0.2f, 0.2f, 0.3f, 1.0f);
        
        public enum ShadowMaskMode
        {
            Standard,           // 標準シャドウマスク
            Advanced,           // 高度なマスク制御
            CompetitorCompatible,  // 競合製品完全互換
            Custom              // カスタム設定
        }
        
        public enum ShadowQuality
        {
            Mobile,    // Quest互換
            Low,       // 低品質
            Medium,    // 中品質
            High,      // 高品質
            Ultra      // 最高品質
        }
        
        [System.Serializable]
        public class ShadowMaskTarget
        {
            public Renderer targetRenderer;
            public bool castShadows = true;
            public bool receiveShadows = true;
            public float shadowStrength = 1.0f;
            public Color shadowTint = Color.white;
            public Material customShadowMaterial;
        }
        
        // Private variables
        private Light primaryLight;
        private Camera playerCamera;
        private List<ShadowMaskTarget> maskTargets = new List<ShadowMaskTarget>();
        private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
        private Dictionary<Renderer, Material> shadowMaterials = new Dictionary<Renderer, Material>();
        
        // Shader property IDs
        private static readonly int _ShadowClampThreshold = Shader.PropertyToID("_ShadowClampThreshold");
        private static readonly int _ShadowSharpness = Shader.PropertyToID("_ShadowSharpness");
        private static readonly int _ShadowStrength = Shader.PropertyToID("_ShadowStrength");
        private static readonly int _ShadowTint = Shader.PropertyToID("_ShadowTint");
        private static readonly int _WorldLightInfluence = Shader.PropertyToID("_WorldLightInfluence");
        private static readonly int _AmbientShadowColor = Shader.PropertyToID("_AmbientShadowColor");
        
        private void Start()
        {
            InitializeShadowMaskSystem();
        }
        
        private void Update()
        {
            if (!enableShadowMask) return;
            
            UpdateShadowMasks();
            UpdateLODShadows();
            
            if (enableWorldColorReflection)
            {
                UpdateWorldLightReflection();
            }
        }
        
        private void InitializeShadowMaskSystem()
        {
            // プライマリライト取得
            primaryLight = GetComponent<Light>();
            if (primaryLight == null)
            {
                primaryLight = FindObjectOfType<Light>();
            }
            
            // カメラ取得
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                playerCamera = FindObjectOfType<Camera>();
            }
            
            // マスクターゲット自動検出
            AutoDetectMaskTargets();
            
            // 競合製品互換モード設定
            if (competitorCompatibilityMode)
            {
                ApplyCompetitorCompatibleSettings();
            }
            
            // シャドウマテリアル作成
            CreateShadowMaterials();
            
            Debug.Log("[Shadow Mask System] 初期化完了 - 競合製品互換モード: " + competitorCompatibilityMode);
        }
        
        private void UpdateShadowMasks()
        {
            foreach (var target in maskTargets)
            {
                if (target.targetRenderer == null) continue;
                
                // シャドウキャスト設定
                var shadowCastingMode = target.castShadows ? 
                    ShadowCastingMode.On : ShadowCastingMode.Off;
                target.targetRenderer.shadowCastingMode = shadowCastingMode;
                
                // シャドウレシーブ設定
                target.targetRenderer.receiveShadows = target.receiveShadows;
                
                // カスタムシャドウマテリアル適用
                if (target.customShadowMaterial != null)
                {
                    ApplyCustomShadowMaterial(target);
                }
                
                // シャドウパラメータ更新
                UpdateShadowParameters(target);
            }
        }
        
        private void UpdateLODShadows()
        {
            if (!enableLODShadows || playerCamera == null) return;
            
            Vector3 cameraPosition = playerCamera.transform.position;
            
            foreach (var target in maskTargets)
            {
                if (target.targetRenderer == null) continue;
                
                float distance = Vector3.Distance(
                    cameraPosition, 
                    target.targetRenderer.bounds.center
                );
                
                ShadowQuality quality = GetLODQuality(distance);
                ApplyShadowQuality(target.targetRenderer, quality);
            }
        }
        
        private void UpdateWorldLightReflection()
        {
            if (primaryLight == null) return;
            
            // ワールドライトの色と強度を反映
            Color worldLightColor = RenderSettings.ambientLight;
            float worldLightIntensity = RenderSettings.ambientIntensity;
            
            // アンビエントシャドウ色を動的調整
            Color adjustedAmbientColor = Color.Lerp(
                ambientShadowColor,
                worldLightColor,
                worldLightInfluence
            );
            
            // 全マテリアルに適用
            foreach (var shadowMaterial in shadowMaterials.Values)
            {
                if (shadowMaterial != null)
                {
                    shadowMaterial.SetColor(_AmbientShadowColor, adjustedAmbientColor);
                    shadowMaterial.SetFloat(_WorldLightInfluence, worldLightInfluence);
                }
            }
        }
        
        private void AutoDetectMaskTargets()
        {
            maskTargets.Clear();
            
            // アバター内の全Rendererを検出
            var renderers = GetComponentsInChildren<Renderer>();
            
            foreach (var renderer in renderers)
            {
                // 透明オブジェクトかチェック
                bool isTransparent = IsTransparentRenderer(renderer);
                
                var maskTarget = new ShadowMaskTarget
                {
                    targetRenderer = renderer,
                    castShadows = !isTransparent,
                    receiveShadows = true,
                    shadowStrength = isTransparent ? transparentShadowStrength : 1.0f,
                    shadowTint = Color.white
                };
                
                maskTargets.Add(maskTarget);
                
                if (isTransparent)
                {
                    transparentRenderers.Add(renderer);
                }
            }
            
            Debug.Log($"[Shadow Mask System] 検出されたRenderer数: {maskTargets.Count}");
        }
        
        private bool IsTransparentRenderer(Renderer renderer)
        {
            if (renderer.sharedMaterial == null) return false;
            
            // マテリアルのレンダリングモードをチェック
            var material = renderer.sharedMaterial;
            
            // Transparentモードかチェック
            if (material.HasProperty("_Mode"))
            {
                int mode = material.GetInt("_Mode");
                return mode == 2 || mode == 3; // Transparent or Fade
            }
            
            // アルファ値をチェック
            if (material.HasProperty("_Color"))
            {
                Color color = material.GetColor("_Color");
                return color.a < 1.0f;
            }
            
            return false;
        }
        
        private void CreateShadowMaterials()
        {
            shadowMaterials.Clear();
            originalMaterials.Clear();
            
            foreach (var target in maskTargets)
            {
                if (target.targetRenderer == null) continue;
                
                var originalMaterial = target.targetRenderer.sharedMaterial;
                if (originalMaterial == null) continue;
                
                // オリジナルマテリアルを保存
                originalMaterials[target.targetRenderer] = originalMaterial;
                
                // PCSS対応シャドウマテリアル作成
                var shadowMaterial = CreatePCSSShadowMaterial(originalMaterial);
                shadowMaterials[target.targetRenderer] = shadowMaterial;
                
                // マテリアル適用
                target.targetRenderer.material = shadowMaterial;
            }
        }
        
        private Material CreatePCSSShadowMaterial(Material originalMaterial)
        {
            // PCSS対応シェーダーを使用
            Shader pcssShader = null;
            
            if (originalMaterial.shader.name.Contains("lilToon"))
            {
                pcssShader = Shader.Find("lilToon/PCSS Extension");
            }
            else if (originalMaterial.shader.name.Contains("Poiyomi"))
            {
                pcssShader = Shader.Find("Poiyomi/PCSS Extension");
            }
            else
            {
                pcssShader = Shader.Find("lilToon/PCSS Extension");
            }
            
            if (pcssShader == null)
            {
                Debug.LogWarning("[Shadow Mask System] PCSS Shaderが見つかりません");
                return new Material(originalMaterial);
            }
            
            // 新しいマテリアル作成
            var shadowMaterial = new Material(originalMaterial)
            {
                shader = pcssShader
            };
            
            // PCSS専用パラメータ設定
            SetupPCSSParameters(shadowMaterial);
            
            return shadowMaterial;
        }
        
        private void SetupPCSSParameters(Material material)
        {
            // シャドウクランプ設定
            if (enableShadowClamp)
            {
                material.SetFloat(_ShadowClampThreshold, shadowClampThreshold);
                material.SetFloat(_ShadowSharpness, shadowSharpness);
            }
            
            // アンビエントシャドウ色
            material.SetColor(_AmbientShadowColor, ambientShadowColor);
            
            // ワールドライト影響度
            material.SetFloat(_WorldLightInfluence, worldLightInfluence);
            
            // 競合製品互換設定
            if (competitorCompatibilityMode)
            {
                ApplyCompetitorShaderSettings(material);
            }
        }
        
        private void ApplyCustomShadowMaterial(ShadowMaskTarget target)
        {
            if (target.customShadowMaterial == null) return;
            
            // カスタムマテリアルのパラメータを適用
            var material = target.targetRenderer.material;
            
            // シャドウ強度
            material.SetFloat(_ShadowStrength, target.shadowStrength);
            
            // シャドウ色合い
            material.SetColor(_ShadowTint, target.shadowTint);
        }
        
        private void UpdateShadowParameters(ShadowMaskTarget target)
        {
            var material = target.targetRenderer.material;
            if (material == null) return;
            
            // 動的パラメータ更新
            if (material.HasProperty(_ShadowStrength))
            {
                material.SetFloat(_ShadowStrength, target.shadowStrength);
            }
            
            if (material.HasProperty(_ShadowTint))
            {
                material.SetColor(_ShadowTint, target.shadowTint);
            }
            
            // アニメーションカーブ適用
            if (enableShadowClamp && material.HasProperty(_ShadowClampThreshold))
            {
                float animatedThreshold = shadowFalloff.Evaluate(Time.time % 1.0f) * shadowClampThreshold;
                material.SetFloat(_ShadowClampThreshold, animatedThreshold);
            }
        }
        
        private ShadowQuality GetLODQuality(float distance)
        {
            for (int i = 0; i < lodDistances.Length; i++)
            {
                if (distance <= lodDistances[i])
                {
                    return lodQualities[Mathf.Min(i, lodQualities.Length - 1)];
                }
            }
            
            return ShadowQuality.Mobile; // 最も遠い場合
        }
        
        private void ApplyShadowQuality(Renderer renderer, ShadowQuality quality)
        {
            var material = renderer.material;
            if (material == null) return;
            
            switch (quality)
            {
                case ShadowQuality.Mobile:
                    ApplyMobileQuality(material);
                    break;
                case ShadowQuality.Low:
                    ApplyLowQuality(material);
                    break;
                case ShadowQuality.Medium:
                    ApplyMediumQuality(material);
                    break;
                case ShadowQuality.High:
                    ApplyHighQuality(material);
                    break;
                case ShadowQuality.Ultra:
                    ApplyUltraQuality(material);
                    break;
            }
        }
        
        private void ApplyMobileQuality(Material material)
        {
            // Quest互換設定
            if (material.HasProperty("_PCSSFilterSize"))
                material.SetFloat("_PCSSFilterSize", 1.0f);
            if (material.HasProperty("_PCSSSearchSamples"))
                material.SetInt("_PCSSSearchSamples", 4);
            if (material.HasProperty("_PCSSFilterSamples"))
                material.SetInt("_PCSSFilterSamples", 8);
        }
        
        private void ApplyLowQuality(Material material)
        {
            // 低品質設定
            if (material.HasProperty("_PCSSFilterSize"))
                material.SetFloat("_PCSSFilterSize", 2.0f);
            if (material.HasProperty("_PCSSSearchSamples"))
                material.SetInt("_PCSSSearchSamples", 8);
            if (material.HasProperty("_PCSSFilterSamples"))
                material.SetInt("_PCSSFilterSamples", 16);
        }
        
        private void ApplyMediumQuality(Material material)
        {
            // 中品質設定
            if (material.HasProperty("_PCSSFilterSize"))
                material.SetFloat("_PCSSFilterSize", 3.0f);
            if (material.HasProperty("_PCSSSearchSamples"))
                material.SetInt("_PCSSSearchSamples", 16);
            if (material.HasProperty("_PCSSFilterSamples"))
                material.SetInt("_PCSSFilterSamples", 32);
        }
        
        private void ApplyHighQuality(Material material)
        {
            // 高品質設定
            if (material.HasProperty("_PCSSFilterSize"))
                material.SetFloat("_PCSSFilterSize", 4.0f);
            if (material.HasProperty("_PCSSSearchSamples"))
                material.SetInt("_PCSSSearchSamples", 24);
            if (material.HasProperty("_PCSSFilterSamples"))
                material.SetInt("_PCSSFilterSamples", 48);
        }
        
        private void ApplyUltraQuality(Material material)
        {
            // 最高品質設定
            if (material.HasProperty("_PCSSFilterSize"))
                material.SetFloat("_PCSSFilterSize", 5.0f);
            if (material.HasProperty("_PCSSSearchSamples"))
                material.SetInt("_PCSSSearchSamples", 32);
            if (material.HasProperty("_PCSSFilterSamples"))
                material.SetInt("_PCSSFilterSamples", 64);
        }
        
        private void ApplyCompetitorCompatibleSettings()
        {
            // 競合製品互換の基本設定
            enableShadowClamp = true;
            shadowClampThreshold = 0.5f;
            shadowSharpness = 2.0f;
            enableTransparentShadows = true;
            transparentShadowStrength = 0.7f;
            worldLightInfluence = 1.0f;
            enableWorldColorReflection = true;
            
            // LOD設定を競合製品互換に
            lodDistances = new float[] { 10f, 25f, 50f };
            lodQualities = new ShadowQuality[] { 
                ShadowQuality.High, 
                ShadowQuality.Medium, 
                ShadowQuality.Low 
            };
            
            Debug.Log("[Shadow Mask System] 競合製品互換設定を適用しました");
        }
        
        private void ApplyCompetitorShaderSettings(Material material)
        {
            // 競合製品互換のシェーダーパラメータ
            if (material.HasProperty("_ShadowReceive"))
                material.SetFloat("_ShadowReceive", 1.0f);
            if (material.HasProperty("_ShadowBorderRange"))
                material.SetFloat("_ShadowBorderRange", 0.1f);
            if (material.HasProperty("_ShadowBlur"))
                material.SetFloat("_ShadowBlur", 0.5f);
            if (material.HasProperty("_Shadow2ndColor"))
                material.SetColor("_Shadow2ndColor", new Color(0.7f, 0.7f, 0.8f, 1.0f));
        }
        
        // Public API
        public void SetShadowMaskMode(ShadowMaskMode mode)
        {
            maskMode = mode;
            
            if (mode == ShadowMaskMode.CompetitorCompatible)
            {
                ApplyCompetitorCompatibleSettings();
            }
        }
        
        public void SetShadowClampThreshold(float threshold)
        {
            shadowClampThreshold = Mathf.Clamp01(threshold);
            
            // 全マテリアルに適用
            foreach (var material in shadowMaterials.Values)
            {
                if (material != null && material.HasProperty(_ShadowClampThreshold))
                {
                    material.SetFloat(_ShadowClampThreshold, shadowClampThreshold);
                }
            }
        }
        
        public void SetShadowSharpness(float sharpness)
        {
            shadowSharpness = Mathf.Clamp(sharpness, 0.1f, 10.0f);
            
            // 全マテリアルに適用
            foreach (var material in shadowMaterials.Values)
            {
                if (material != null && material.HasProperty(_ShadowSharpness))
                {
                    material.SetFloat(_ShadowSharpness, shadowSharpness);
                }
            }
        }
        
        public void ToggleShadowMask(bool enabled)
        {
            enableShadowMask = enabled;
            
            // 全Rendererのシャドウ設定を切り替え
            foreach (var target in maskTargets)
            {
                if (target.targetRenderer != null)
                {
                    target.targetRenderer.shadowCastingMode = enabled && target.castShadows ? 
                        ShadowCastingMode.On : ShadowCastingMode.Off;
                    target.targetRenderer.receiveShadows = enabled && target.receiveShadows;
                }
            }
        }
        
        public void AddCustomMaskTarget(Renderer renderer, bool castShadows, bool receiveShadows)
        {
            var existingTarget = maskTargets.FirstOrDefault(t => t.targetRenderer == renderer);
            if (existingTarget != null)
            {
                existingTarget.castShadows = castShadows;
                existingTarget.receiveShadows = receiveShadows;
            }
            else
            {
                var newTarget = new ShadowMaskTarget
                {
                    targetRenderer = renderer,
                    castShadows = castShadows,
                    receiveShadows = receiveShadows,
                    shadowStrength = 1.0f,
                    shadowTint = Color.white
                };
                
                maskTargets.Add(newTarget);
            }
        }
        
        private void OnValidate()
        {
            shadowClampThreshold = Mathf.Clamp01(shadowClampThreshold);
            shadowSharpness = Mathf.Clamp(shadowSharpness, 0.1f, 10.0f);
            transparentShadowStrength = Mathf.Clamp01(transparentShadowStrength);
            worldLightInfluence = Mathf.Clamp01(worldLightInfluence);
        }
        
        private void OnDestroy()
        {
            // オリジナルマテリアルを復元
            foreach (var kvp in originalMaterials)
            {
                if (kvp.Key != null && kvp.Value != null)
                {
                    kvp.Key.material = kvp.Value;
                }
            }
        }
    }
} 