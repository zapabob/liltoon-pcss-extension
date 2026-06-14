Shader "lilToon/PCSS Extension"
{
    Properties
    {
        // lilToon 2.2.1 migration compatibility
        [HideInInspector] _lilToonVersion ("lilToonVersion", Float) = 2.2
        _MainTex ("Main Texture", 2D) = "white" {}
        _BaseMap ("Base Map", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        _AlphaClip ("Alpha Clip", Range(0.0, 1.0)) = 0.5
        
        [lilToggle] _UseShadow ("Use Shadow", Float) = 1
        [lilToggle] _ShadowReceive ("Shadow Receive", Float) = 1
        _ShadowColorTex ("Shadow Color", 2D) = "black" {}
        _ShadowColor ("Shadow Color", 2D) = "black" {}
        _ShadowBorder ("Shadow Border", Range(0, 1)) = 0.5
        _ShadowBorderRange ("Shadow Border Range", Range(0, 1)) = 0.5
        _ShadowBlur ("Shadow Blur", Range(0, 1)) = 0.1
        _ShadowBlurMask ("Shadow Blur Mask", Range(0, 1)) = 0.1
        
        [lilToggle] _UseShadow2 ("Use 2nd Shadow", Float) = 0
        [lilToggle] _Shadow2ndReceive ("Shadow 2nd Receive", Float) = 0
        _Shadow2ColorTex ("2nd Shadow Color", 2D) = "black" {}
        _Shadow2ndColor ("Shadow 2nd Color", 2D) = "black" {}
        _Shadow2Border ("2nd Shadow Border", Range(0, 1)) = 0.5
        _Shadow2ndBorderRange ("Shadow 2nd Border Range", Range(0, 1)) = 0.5
        _Shadow2Blur ("2nd Shadow Blur", Range(0, 1)) = 0.1
        _Shadow2ndBlurMask ("Shadow 2nd Blur Mask", Range(0, 1)) = 0.1
        
        [lilToggle] _UseShadow3 ("Use 3rd Shadow", Float) = 0
        [lilToggle] _Shadow3rdReceive ("Shadow 3rd Receive", Float) = 0
        _Shadow3ColorTex ("3rd Shadow Color", 2D) = "black" {}
        _Shadow3rdColor ("Shadow 3rd Color", 2D) = "black" {}
        _Shadow3Border ("3rd Shadow Border", Range(0, 1)) = 0.5
        _Shadow3rdBorderRange ("Shadow 3rd Border Range", Range(0, 1)) = 0.5
        _Shadow3Blur ("3rd Shadow Blur", Range(0, 1)) = 0.1
        _Shadow3rdBlurMask ("Shadow 3rd Blur Mask", Range(0, 1)) = 0.1
        
        [lilToggle] _UseSDFFaceShadow ("Use SDF Face Shadow", Float) = 0
        _SDFFaceShadowTex ("SDF Face Shadow Texture", 2D) = "white" {}
        _SDFFaceShadowIntensity ("SDF Face Shadow Intensity", Range(0.0, 1.0)) = 0.5
        _SDFFaceShadowSoftness ("SDF Face Shadow Softness", Range(0.0, 1.0)) = 0.1
        
        // --- LTCGI (Linearly Transformed Cosines Global Illumination) ---
        [lilToggle] _UseLTCGI ("Use LTCGI", Float) = 0
        _LTCGIIntensity ("LTCGI Intensity", Range(0.0, 2.0)) = 1.0
        _LTCGISamples ("LTCGI Samples", Range(1, 64)) = 16
        
        // --- Backlight & Light Direction Override ---
        [lilToggle] _UseBacklight ("Use Backlight", Float) = 0
        _BacklightColor ("Backlight Color", Color) = (1,1,1,1)
        _BacklightIntensity ("Backlight Intensity", Range(0.0, 2.0)) = 1.0
        [lilToggle] _UseLightDirectionOverride ("Use Light Direction Override", Float) = 0
        _LightDirectionOverride ("Light Direction Override", Vector) = (0,1,0,0)
        
        [lilToggle] _UsePCSS ("Use PCSS", Float) = 1
        [Enum(Realistic,0,Anime,1,Cinematic,2,Custom,3,DewySkin,4,SoftFlushSkin,5,StudioBoost,6,ExcitedTone,7)] _PCSSPresetMode ("PCSS Preset", Float) = 1
        _LocalPCSSFilterRadius ("PCSS Filter Radius", Range(0.001, 0.1)) = 0.01
        _LocalPCSSLightSize ("PCSS Light Size", Range(0.01, 0.5)) = 0.1
        _LocalPCSSBias ("PCSS Bias", Range(0.0001, 0.01)) = 0.001
        _PCSSIntensity ("PCSS Intensity", Range(0.0, 2.0)) = 1.0
        [Enum(Low,0,Medium,1,High,2,Ultra,3)] _PCSSQualityLevel ("PCSS Quality", Float) = 1
        _LocalPCSSSamples ("PCSS Samples", Range(1, 64)) = 16
        [lilToggle] _UseShadowMask ("Use Shadow Mask", Float) = 0
        _ShadowMaskTex ("Shadow Mask (R:Cast, G:Receive)", 2D) = "white" {}
        _ShadowMaskStrength ("Shadow Mask Strength", Range(0.0, 1.0)) = 1.0
        [lilToggle] _UsePCSSOptimization ("Use PCSS Optimization", Float) = 1
        _PCSSOptimizationLevel ("PCSS Optimization Level", Range(0, 3)) = 1
        [lilToggle] _UsePCSSMobileOptimization ("Use PCSS Mobile Optimization", Float) = 0
        
        [lilToggle] _UseShadowClamp ("Use Shadow Clamp (Anime Style)", Float) = 0
        _ShadowClamp ("Shadow Clamp", Range(0, 1)) = 0.5
        _Translucency ("Translucency", Range(0, 1)) = 0.5
        [lilToggle] _UseNoLightPCSSBoost ("Use No-Light PCSS Boost", Float) = 0
        _NoLightPCSSBoostStrength ("No-Light PCSS Boost Strength", Range(0.0, 1.0)) = 0.0
        _NoLightPCSSBoostSoftness ("No-Light PCSS Boost Softness", Range(0.0, 1.0)) = 0.65
        _NoLightPCSSBoostRim ("No-Light PCSS Boost Rim", Range(0.0, 2.0)) = 0.35
        _NoLightPCSSHighlightTint ("No-Light PCSS Highlight Tint", Color) = (0.55,0.52,0.50,1.0)
        [lilToggle] _UseSoftFlush ("Use Soft Flush", Float) = 0
        _SoftFlushColor ("Soft Flush Color", Color) = (1.0,0.40,0.36,1.0)
        _SoftFlushStrength ("Soft Flush Strength", Range(0.0, 1.0)) = 0.0
        _SoftFlushWidth ("Soft Flush Width", Range(0.0, 1.0)) = 0.56
        _SoftFlushVerticalBias ("Soft Flush Vertical Bias", Range(0.0, 1.0)) = 0.46
        [lilToggle] _UseExcitedTone ("Use Excited Tone", Float) = 0
        _ExcitedToneColor ("Excited Tone Color", Color) = (1.0,0.48,0.34,1.0)
        _ExcitedToneStrength ("Excited Tone Strength", Range(0.0, 1.0)) = 0.0
        _ExcitedToneBreath ("Excited Tone Breath", Range(0.0, 1.0)) = 0.0
        _ExcitedToneUpperBias ("Excited Tone Upper Bias", Range(0.0, 1.0)) = 0.56
        [lilToggle] _UseVRChatPerformanceGate ("Use VRChat Performance Gate", Float) = 1
        _PCSSMaxDistance ("PCSS Gaussian Cutoff (0 at meters)", Range(1.0, 10.0)) = 10.0
        _PCSSDistanceFade ("PCSS Gaussian Sigma", Range(0.5, 10.0)) = 3.0
        
        // VRC Light Volumes 2.0.1 enhancements for lilToon 2.2.1 support.
        [lilToggle] _UseVRCLightVolumes ("Use VRC Light Volumes", Float) = 0
        [lilToggle] _VRCLightVolumesEnabled ("VRC Light Volumes Enabled", Float) = 0
        _VRCLightVolumeIntensity ("VRC Light Volume Intensity", Range(0.0, 2.0)) = 1.0
        _VRCLVIntensity ("VRC LV Intensity", Range(0.0, 2.0)) = 1.0
        _VRCLightVolumeTint ("VRC Light Volume Tint", Color) = (1,1,1,1)
        _VRCLVTintColor ("VRC LV Tint Color", Color) = (1,1,1,1)
        _VRCLightVolumeDistanceFactor ("VRC Light Volume Distance Factor", Range(0.0, 1.0)) = 0.1
        _VRCLVDistanceAttenuation ("VRC LV Distance Attenuation", Range(0.0, 1.0)) = 0.1
        _EnvRimBorder ("[VRCLV] Rim Border", Range(0, 1)) = 0.85
        _EnvRimBlur ("[VRCLV] Rim Blur", Range(0, 1)) = 0.35
        [lilToggle] _UseVRCLVRimLight ("Use VRC LV Rim Light", Float) = 0
        _VRCLVRimLightIntensity ("VRC LV Rim Light Intensity", Range(0.0, 2.0)) = 1.0
        _VRCLVRimLightColor ("VRC LV Rim Light Color", Color) = (1,1,1,1)
        [lilToggle] _UseVRCLVOptimization ("Use VRC LV Optimization", Float) = 1
        [lilToggle] _VRCLVOptimizationEnabled ("VRC LV Optimization Enabled", Float) = 1
        _VRCLVOptimizationLevel ("VRC LV Optimization Level", Range(0, 3)) = 1
        _VRCLVOptimizationMode ("VRC LV Optimization Mode", Range(0, 3)) = 1
        
        [lilToggle] _UseFur ("Use Fur", Float) = 0
        [lilToggle] _FurEnabled ("Fur Enabled", Float) = 0
        _FurTex ("Fur Texture", 2D) = "white" {}
        _FurNoiseMask ("Fur Noise Mask", 2D) = "white" {}
        _FurLength ("Fur Length", Range(0.0, 1.0)) = 0.5
        _FurLengthMask ("Fur Length Mask", Range(0.0, 1.0)) = 0.5
        _FurDensity ("Fur Density", Range(1, 64)) = 16
        _FurRandomize ("Fur Randomize", Range(1, 64)) = 16
        _FurSubdivision ("Fur Subdivision", Range(1, 8)) = 4
        _FurLayerNum ("Fur Layer Num", Range(1, 8)) = 4
        _FurGravity ("Fur Gravity", Range(-1.0, 1.0)) = 0.0
        _FurGravityMask ("Fur Gravity Mask", Range(-1.0, 1.0)) = 0.0
        _FurWind ("Fur Wind", Range(0.0, 1.0)) = 0.0
        _FurWindMask ("Fur Wind Mask", Range(0.0, 1.0)) = 0.0
        _FurWindSpeed ("Fur Wind Speed", Range(0.0, 10.0)) = 1.0
        _FurWindFreq ("Fur Wind Freq", Range(0.0, 10.0)) = 1.0
        _FurAO ("Fur Ambient Occlusion", Range(0.0, 1.0)) = 0.5
        _FurAOMask ("Fur AO Mask", Range(0.0, 1.0)) = 0.5
        _FurShadow ("Fur Shadow", Range(0.0, 1.0)) = 0.5
        _FurMesh ("Fur Mesh", Range(0.0, 1.0)) = 0.5
        [lilToggle] _UseFurOptimization ("Use Fur Optimization", Float) = 1
        [lilToggle] _FurOptimizationEnabled ("Fur Optimization Enabled", Float) = 1
        _FurOptimizationLevel ("Fur Optimization Level", Range(0, 3)) = 1
        _FurOptimizationMode ("Fur Optimization Mode", Range(0, 3)) = 1
        
        // --- Flipbook ---
        [lilToggle] _UseFlipbook ("Use Flipbook", Float) = 0
        _FlipbookTex ("Flipbook Texture", 2D) = "white" {}
        _FlipbookDivisionsX ("Flipbook Divisions X", Float) = 4
        _FlipbookDivisionsY ("Flipbook Divisions Y", Float) = 4
        _FlipbookSpeed ("Flipbook Speed", Float) = 10

        [lilToggle] _UseRealisticShadow ("Use Realistic Shadow", Float) = 0
        _RealisticShadowColor ("Realistic Shadow Color", Color) = (0.2,0.2,0.2,0.8)
        _RealisticShadowIntensity ("Realistic Shadow Intensity", Range(0.0, 1.0)) = 0.5
        _RealisticShadowSoftness ("Realistic Shadow Softness", Range(0.0, 1.0)) = 0.3
        _RealisticShadowOffset ("Realistic Shadow Offset", Vector) = (0,0,0,0)
        _RealisticShadowScale ("Realistic Shadow Scale", Vector) = (1,1,1,1)
        
        [lilToggle] _UseRimShade ("Use RimShade", Float) = 0
        _RimShadeColor ("RimShade Color", Color) = (0.3,0.3,0.3,1.0)
        _RimShadeIntensity ("RimShade Intensity", Range(0.0, 1.0)) = 0.4
        _RimShadeWidth ("RimShade Width", Range(0.0, 1.0)) = 0.5

        // --- Gloss and shadow coherence ---
        [lilToggle] _UseGlossShadowCoherence ("Use Gloss Shadow Coherence", Float) = 1
        _GlossShadowCoherence ("Gloss Shadow Coherence", Range(0.0, 1.0)) = 0.55
        _GlossShadowBoost ("Gloss Boost In Light", Range(0.0, 2.0)) = 0.35
        _GlossShadowSuppression ("Gloss Shadow Suppression", Range(0.0, 1.0)) = 0.45
        _GlossRimStrength ("Gloss Rim Strength", Range(0.0, 2.0)) = 0.35
        _GlossSmoothness ("Gloss Smoothness", Range(0.0, 1.0)) = 0.72

        // --- Rendering ---
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 2
        [Enum(Off,0,On,1)] _ZWrite ("ZWrite", Float) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
        // --- Stencil ---
        _StencilRef ("Stencil Reference", Range(0, 255)) = 0
        _StencilReadMask ("Stencil Read Mask", Range(0, 255)) = 255
        _StencilWriteMask ("Stencil Write Mask", Range(0, 255)) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Compare", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilPass ("Stencil Pass", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilFail ("Stencil Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilZFail ("Stencil ZFail", Float) = 0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "Queue"="Geometry"
            "LightMode"="ForwardBase"
        }
        LOD 200

        Stencil
        {
            Ref [_StencilRef]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
            Comp [_StencilComp]
            Pass [_StencilPass]
            Fail [_StencilFail]
            ZFail [_StencilZFail]
        }

        Pass
        {
            Name "FORWARD"
            Tags {"LightMode" = "ForwardBase"}
            Cull [_Cull]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Blend [_SrcBlend] [_DstBlend]

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma shader_feature_local _ _ALPHATEST_ON
            #pragma shader_feature_local _ _USEPCSS_ON
            #pragma shader_feature_local _ _USESHADOW_ON
            #pragma shader_feature_local _ _USESHADOW2_ON
            #pragma shader_feature_local _ _USESHADOW3_ON
            #pragma shader_feature_local _ _USESDFFACESHADOW_ON
            #pragma shader_feature_local _ _USELTCGI_ON
            #pragma shader_feature_local _ _USEBACKLIGHT_ON
            #pragma shader_feature_local _ _USELIGHTDIRECTIONOVERRIDE_ON
            #pragma shader_feature_local _ _USEVRCLIGHT_VOLUMES_ON
            #pragma shader_feature_local _ _USEVRCLV_RIMLIGHT_ON
            #pragma shader_feature_local _ _USESHADOWCLAMP_ON
            #pragma shader_feature_local _ _USE_OPTIMIZED_PCSS_ON
            #pragma shader_feature_local _ _USESHADOWMASK_ON
            #pragma shader_feature_local _ _USEFLIPBOOK_ON
            #pragma shader_feature_local _ _USEFUROPTIMIZATION_ON
            #pragma shader_feature_local _ _USEVRCLVOPTIMIZATION_ON
            #pragma shader_feature_local _ _USEPCSSOPTIMIZATION_ON
            #pragma shader_feature_local _ _USEPCSSMOBILEOPTIMIZATION_ON
            #pragma shader_feature_local _ _USEVRCHATPERFORMANCEGATE_ON
            #pragma shader_feature_local _ _USEGLOSSSHADOWCOHERENCE_ON
            #pragma shader_feature_local _ _USENOLIGHTPCSSBOOST_ON
            #pragma shader_feature_local _ _USESOFTFLUSH_ON
            #pragma shader_feature_local _ _USEEXCITEDTONE_ON
            #pragma shader_feature_local _ _USEFUR_ON
            #pragma shader_feature_local _ _USEREALISTICSHADOW_ON
            #pragma shader_feature_local _ _USERIMSHADE_ON
            #pragma multi_compile _ VRC_LIGHT_VOLUMES_ENABLED
            #pragma multi_compile _ VRC_LIGHT_VOLUMES_MOBILE

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #define LIL_LILTOON_SHADER_INCLUDED
            
            #ifndef _MAINTEX
                #define _MAINTEX
            #endif
            
            #ifndef _SHADOWCOLORTEX
                #define _SHADOWCOLORTEX
            #endif
            
            // PCSS includes are ordered before local declarations to avoid duplicate shader variables.
            #include "Includes/lil_pcss_common.hlsl"
            #if defined(_USE_OPTIMIZED_PCSS_ON)
                #include "Includes/lil_pcss_shadows_optimized.hlsl"
            #else
                #include "Includes/lil_pcss_shadows.hlsl"
            #endif

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            sampler2D _ShadowColorTex;
            sampler2D _Shadow2ColorTex;
            sampler2D _Shadow3ColorTex;
            sampler2D _SDFFaceShadowTex;
            float _UsePCSS;
            float _UseVRCLightVolumes;
            float _VRCLightVolumeIntensity;
            float4 _VRCLightVolumeTint;
            float _VRCLightVolumeDistanceFactor;
            sampler3D _VRCLightVolumeTexture;
            float4 _VRCLightVolumeParams;
            float4x4 _VRCLightVolumeWorldToLocal;
            float _UseShadow;
            float _ShadowBorder;
            float _ShadowBlur;
            float _UseShadow2;
            float _Shadow2Border;
            float _Shadow2Blur;
            float _UseShadow3;
            float _Shadow3Border;
            float _Shadow3Blur;
            float _UseSDFFaceShadow;
            float _SDFFaceShadowIntensity;
            float _SDFFaceShadowSoftness;
            float _UseLTCGI;
            float _LTCGIIntensity;
            float _LTCGISamples;
            float _UseBacklight;
            float4 _BacklightColor;
            float _BacklightIntensity;
            float _UseLightDirectionOverride;
            float4 _LightDirectionOverride;
            float _Cutoff;
            float _UseShadowClamp;
            float _ShadowClamp;
            float _Translucency;
            float _UseSoftFlush;
            float4 _SoftFlushColor;
            float _SoftFlushStrength;
            float _SoftFlushWidth;
            float _SoftFlushVerticalBias;
            float _UseExcitedTone;
            float4 _ExcitedToneColor;
            float _ExcitedToneStrength;
            float _ExcitedToneBreath;
            float _ExcitedToneUpperBias;
            float _UseNoLightPCSSBoost;
            float _NoLightPCSSBoostStrength;
            float _NoLightPCSSBoostSoftness;
            float _NoLightPCSSBoostRim;
            float4 _NoLightPCSSHighlightTint;
            float _UseVRChatPerformanceGate;
            float _PCSSMaxDistance;
            float _PCSSDistanceFade;
            float _LocalPCSSFilterRadius;
            float _LocalPCSSLightSize;
            float _LocalPCSSSamples;
            float _LocalPCSSBias;
            float _PCSSQualityLevel;
            sampler2D _ShadowMaskTex;
            float _ShadowMaskStrength;
            float _UseFurOptimization;
            float _FurOptimizationLevel;
            float _UseVRCLVOptimization;
            float _VRCLVOptimizationLevel;
            float _UsePCSSOptimization;
            float _PCSSOptimizationLevel;
            float _UsePCSSMobileOptimization;
            // Feature-related variables.
            sampler2D _FurTex;
            float _FurLength;
            float _FurDensity;
            float _FurSubdivision;
            float _FurGravity;
            float _FurWind;
            float _FurWindSpeed;
            float _FurAO;
            float _FurShadow;

            // Flipbook
            sampler2D _FlipbookTex;
            float _FlipbookDivisionsX;
            float _FlipbookDivisionsY;
            float _FlipbookSpeed;

            float _UseRealisticShadow;
            float4 _RealisticShadowColor;
            float _RealisticShadowIntensity;
            float _RealisticShadowSoftness;
            float4 _RealisticShadowOffset;
            float4 _RealisticShadowScale;
            
            // RimShade variables.
            float _UseRimShade;
            float4 _RimShadeColor;
            float _RimShadeIntensity;
            float _RimShadeWidth;
            float _UseGlossShadowCoherence;
            float _GlossShadowCoherence;
            float _GlossShadowBoost;
            float _GlossShadowSuppression;
            float _GlossRimStrength;
            float _GlossSmoothness;

            float3 ApplySoftFlush(float3 baseColor, float2 uv, float3 worldNormal, float3 viewDir)
            {
                float width = lerp(0.055, 0.22, saturate(_SoftFlushWidth));
                float height = max(0.035, width * 0.62);
                float2 leftDelta = (uv - float2(0.34, _SoftFlushVerticalBias)) / float2(width, height);
                float2 rightDelta = (uv - float2(0.66, _SoftFlushVerticalBias)) / float2(width, height);
                float leftMask = exp(-dot(leftDelta, leftDelta) * 1.35);
                float rightMask = exp(-dot(rightDelta, rightDelta) * 1.35);
                float cheekMask = saturate(max(leftMask, rightMask));
                float frontFacing = saturate(dot(normalize(worldNormal), normalize(viewDir)));
                cheekMask *= lerp(0.72, 1.0, frontFacing);
                float strength = saturate(_SoftFlushStrength) * cheekMask;
                return lerp(baseColor, baseColor * _SoftFlushColor.rgb, strength);
            }

            float3 ApplyExcitedTone(float3 baseColor, float2 uv, float3 worldNormal, float3 viewDir)
            {
                float upperMask = smoothstep(0.18, _ExcitedToneUpperBias, uv.y);
                float frontFacing = saturate(dot(normalize(worldNormal), normalize(viewDir)));
                float rimWarmth = pow(1.0 - frontFacing, 2.0) * 0.35;
                float breath = 1.0 + sin(_Time.y * 1.8) * 0.08 * saturate(_ExcitedToneBreath);
                float strength = saturate(_ExcitedToneStrength) * saturate(upperMask * 0.82 + rimWarmth) * breath;
                float3 warmColor = lerp(_ExcitedToneColor.rgb, _ExcitedToneColor.rgb * 1.08, rimWarmth);
                return lerp(baseColor, baseColor * warmColor, saturate(strength));
            }

            #if defined(VRC_LIGHT_VOLUMES_ENABLED)
            float3 SampleVRCLightVolumes(float3 worldPos, float3 worldNormal)
            {
                #if defined(VRC_LIGHT_VOLUMES_MOBILE) || defined(_USEPCSSMOBILEOPTIMIZATION_ON)
                    float3 localPos = mul(_VRCLightVolumeWorldToLocal, float4(worldPos, 1.0)).xyz;
                    float3 volumeUV = localPos * 0.5 + 0.5;
                    float3 lightColor = tex3D(_VRCLightVolumeTexture, volumeUV).rgb;
                    return lightColor * _VRCLightVolumeTint.rgb * _VRCLightVolumeIntensity;
                #else
                    float3 localPos = mul(_VRCLightVolumeWorldToLocal, float4(worldPos, 1.0)).xyz;
                    float3 volumeUV = localPos * 0.5 + 0.5;
                    if (volumeUV.x < 0.0 || volumeUV.x > 1.0 || volumeUV.y < 0.0 || volumeUV.y > 1.0 || volumeUV.z < 0.0 || volumeUV.z > 1.0)
                        return float3(1.0, 1.0, 1.0);
                    float3 lightColor = tex3D(_VRCLightVolumeTexture, volumeUV).rgb;
                    float distFactor = 1.0 - saturate(length(localPos) * _VRCLightVolumeDistanceFactor);
                    // Direction-aware light volume blending.
                    float3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                    float normalDotLight = max(0.0, dot(worldNormal, worldLightDir));
                    
                    #if defined(_USEVRCLVOPTIMIZATION_ON)
                        float optimizationFactor = 1.0 - (_VRCLVOptimizationLevel * 0.25);
                        return lerp(float3(1.0, 1.0, 1.0), lightColor * _VRCLightVolumeTint.rgb, 
                                   _VRCLightVolumeIntensity * distFactor * normalDotLight * optimizationFactor);
                    #else
                        return lerp(float3(1.0, 1.0, 1.0), lightColor * _VRCLightVolumeTint.rgb, 
                                   _VRCLightVolumeIntensity * distFactor * normalDotLight);
                    #endif
                #endif
            }
            #else
            float3 SampleVRCLightVolumes(float3 worldPos, float3 worldNormal)
            {
                return float3(1.0, 1.0, 1.0);
            }
            #endif

            // SDF Face Shadow functions for lilToon 2.1.7 compatibility.
            float CalculateSDFFaceShadow(float2 uv, float3 worldNormal)
            {
                #if defined(_USESDFFACESHADOW_ON)
                    float sdfValue = tex2D(_SDFFaceShadowTex, uv).r;
                    float faceShadow = smoothstep(_SDFFaceShadowSoftness, 1.0 - _SDFFaceShadowSoftness, sdfValue);
                    return lerp(1.0, faceShadow, _SDFFaceShadowIntensity);
                #else
                    return 1.0;
                #endif
            }

            // LTCGI functions (Linearly Transformed Cosines Global Illumination).
            float3 CalculateLTCGI(float3 worldPos, float3 worldNormal)
            {
                #if defined(_USELTCGI_ON)
                    // Simple LTCGI accumulation.
                    float3 gi = 0;
                    for (int i = 0; i < _LTCGISamples; i++)
                    {
                        float3 sampleDir = normalize(float3(
                            sin(i * 2.39996323) * cos(i * 1.57079633),
                            cos(i * 2.39996323),
                            sin(i * 1.57079633)
                        ));
                        float weight = max(0.0, dot(worldNormal, sampleDir));
                        gi += weight * sampleDir;
                    }
                    gi /= _LTCGISamples;
                    return gi * _LTCGIIntensity;
                #else
                    return float3(1.0, 1.0, 1.0);
                #endif
            }

            // Backlight functions.
            float3 CalculateBacklight(float3 worldNormal, float3 worldLightDir)
            {
                #if defined(_USEBACKLIGHT_ON)
                    float backlightDot = max(0.0, dot(worldNormal, -worldLightDir));
                    return _BacklightColor.rgb * _BacklightIntensity * backlightDot;
                #else
                    return float3(0.0, 0.0, 0.0);
                #endif
            }
            
            float3 CalculateFur(float2 uv, float3 worldNormal, float density, float subdivision)
            {
                #if defined(_USEFUR_ON)
                    float4 furTex = tex2D(_FurTex, uv);
                    
                    float furLength = _FurLength * furTex.r;
                    float furDensity = density * furTex.g;
                    
                    float3 gravity = float3(0, -_FurGravity, 0);
                    float3 wind = float3(sin(_Time.y * _FurWindSpeed) * _FurWind, 0, cos(_Time.y * _FurWindSpeed) * _FurWind);
                    
                    float3 furDirection = normalize(worldNormal + gravity + wind);
                    
                    float3 furColor = float3(1, 1, 1);
                    for (int i = 0; i < subdivision; i++)
                    {
                        float t = (float)i / subdivision;
                        float3 furPos = furDirection * furLength * t;
                        float furIntensity = 1.0 - t;
                        
                        furIntensity *= lerp(1.0, _FurAO, t);
                        
                        furIntensity *= lerp(1.0, _FurShadow, t);
                        
                        furColor *= furIntensity;
                    }
                    
                    return furColor;
                #else
                    return float3(1, 1, 1);
                #endif
            }

            float CalculateRealisticShadow(float3 worldNormal, float3 worldLightDir)
            {
                #if defined(_USEREALISTICSHADOW_ON)
                    float normalDotLight = max(0.0, dot(worldNormal, worldLightDir));
                    
                    float shadow = 1.0 - normalDotLight;
                    shadow = pow(shadow, 1.0 + _RealisticShadowSoftness * 2.0);
                    
                    shadow *= _RealisticShadowIntensity;
                    
                    return shadow;
                #else
                    return 0.0;
                #endif
            }

            // RimShade functions.
            float3 CalculateRimShade(float3 worldNormal, float3 viewDir)
            {
                #if defined(_USERIMSHADE_ON)
                    float rimDot = 1.0 - max(0.0, dot(worldNormal, viewDir));
                    rimDot = pow(rimDot, _RimShadeWidth);
                    
                    float rimShade = rimDot * _RimShadeIntensity;
                    
                    return _RimShadeColor.rgb * rimShade;
                #else
                    return float3(0.0, 0.0, 0.0);
                #endif
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                SHADOW_COORDS(3)
                UNITY_FOG_COORDS(4)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            inline float ApplyShadowThreshold(float attenuation, float border, float blur)
            {
                attenuation = saturate(attenuation + border);
                float effectiveBlur = max(blur, 0.0001);
                return smoothstep(0.0, effectiveBlur, attenuation);
            }

            inline float AdjustPcssSampleCount(float sampleCount, int qualityLevel)
            {
                if (qualityLevel <= 0)
                {
                    return max(8.0, sampleCount * 0.5);
                }

                if (qualityLevel == 2)
                {
                    return min(24.0, sampleCount * 1.25);
                }

                if (qualityLevel >= 3)
                {
                    return min(32.0, sampleCount * 1.5);
                }

                return sampleCount;
            }

            inline float ApplyVRChatPerformanceGate(float shadow, float3 worldPos)
            {
                #if defined(_USEVRCHATPERFORMANCEGATE_ON)
                    float distanceToCamera = length(_WorldSpaceCameraPos - worldPos);
                    float cutoffDistance = min(max(_PCSSMaxDistance, 0.001), 10.0);
                    float sigma = max(_PCSSDistanceFade, 0.001);
                    float gaussian = exp(-0.5 * (distanceToCamera / sigma) * (distanceToCamera / sigma));
                    float cutoffGaussian = exp(-0.5 * (cutoffDistance / sigma) * (cutoffDistance / sigma));
                    float keepRealtime = saturate((gaussian - cutoffGaussian) / max(1.0 - cutoffGaussian, 0.0001));
                    keepRealtime *= 1.0 - step(cutoffDistance, distanceToCamera);
                    return lerp(1.0, shadow, keepRealtime);
                #else
                    return shadow;
                #endif
            }

            inline float3 ApplyGlossShadowCoherence(float3 color, float shadow, float3 worldNormal, float3 viewDir, float3 lightDir)
            {
                #if defined(_USEGLOSSSHADOWCOHERENCE_ON)
                    float3 normalDir = normalize(worldNormal);
                    float3 halfDir = normalize(viewDir + lightDir);
                    float specPower = lerp(32.0, 256.0, saturate(_GlossSmoothness));
                    float specular = pow(saturate(dot(normalDir, halfDir)), specPower);
                    float rim = pow(1.0 - saturate(dot(normalDir, viewDir)), 3.0);
                    float shadowDamping = lerp(1.0 - saturate(_GlossShadowSuppression), 1.0, saturate(shadow));
                    float highlight = (specular * _GlossShadowBoost + rim * _GlossRimStrength) * shadowDamping * _GlossShadowCoherence;
                    float3 highlightTint = _LightColor0.rgb;
                    #if defined(_USENOLIGHTPCSSBOOST_ON)
                        highlightTint = max(highlightTint, _NoLightPCSSHighlightTint.rgb);
                    #endif
                    return saturate(color + highlight * highlightTint);
                #else
                    return color;
                #endif
            }

            inline float ApplyNoLightPCSSBoost(float shadow, float3 worldNormal, float3 viewDir, float3 lightDir)
            {
                #if defined(_USENOLIGHTPCSSBOOST_ON)
                    float3 normalDir = normalize(worldNormal);
                    float3 lightDirection = normalize(lightDir);
                    float3 viewDirection = normalize(viewDir);
                    float facing = saturate(dot(normalDir, lightDirection));
                    float broadShade = pow(saturate(1.0 - facing), lerp(0.75, 2.60, saturate(_NoLightPCSSBoostSoftness)));
                    float rimShade = pow(1.0 - saturate(dot(normalDir, viewDirection)), 2.2) * _NoLightPCSSBoostRim;
                    float pseudoShadow = saturate(1.0 - (broadShade + rimShade) * _NoLightPCSSBoostStrength);
                    return min(shadow, pseudoShadow);
                #else
                    return shadow;
                #endif
            }

            inline float SamplePrimaryShadow(v2f i)
            {
                float attenuation = SHADOW_ATTENUATION(i);

                #if defined(_USEPCSS_ON)
                    #if defined(LIL_PCSS_MOBILE_PLATFORM) || defined(_USEPCSSMOBILEOPTIMIZATION_ON)
                        return PCSSMobile(attenuation, i.pos.z);
                    #else
                        float sampleCount = max(1.0, _LocalPCSSSamples);
                        int quality = (int)round(_PCSSQualityLevel);

                        #if defined(_USEPCSSOPTIMIZATION_ON)
                            float optimizationFactor = 1.0 - (_PCSSOptimizationLevel * 0.25);
                            sampleCount *= optimizationFactor;
                        #endif

                        sampleCount = AdjustPcssSampleCount(sampleCount, quality);

                        #if defined(_USE_OPTIMIZED_PCSS_ON)
                            attenuation = PCSS_Optimized(attenuation, i.pos.z, _LocalPCSSFilterRadius, _LocalPCSSLightSize, sampleCount);
                        #else
                            attenuation = PCSS(attenuation, i.pos.z, _LocalPCSSFilterRadius, _LocalPCSSLightSize, sampleCount);
                        #endif

                        return lerp(1.0f, attenuation, (float)_PCSSIntensity);
                    #endif
                #elif defined(_USESHADOW_ON)
                    return ApplyShadowThreshold(attenuation, _ShadowBorder, _ShadowBlur);
                #else
                    return attenuation;
                #endif
            }

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                #if defined(_USEREALISTICSHADOW_ON)
                    float3 offsetPos = v.vertex.xyz + _RealisticShadowOffset.xyz;
                    offsetPos *= _RealisticShadowScale.xyz;
                    o.pos = UnityObjectToClipPos(float4(offsetPos, v.vertex.w));
                    o.worldPos = mul(unity_ObjectToWorld, float4(offsetPos, v.vertex.w)).xyz;
                #else
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                #endif
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                UNITY_TRANSFER_SHADOW(o, v.uv);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Prevent pink renders when textures are missing.
                fixed4 col = fixed4(1, 1, 1, 1);

#if defined(_MAINTEX)
                col = tex2D(_MainTex, i.uv) * _Color;
#else
                col = _Color;
#endif

#if defined(_ALPHATEST_ON)
                clip(col.a - _Cutoff);
#endif

                float3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
#if defined(_USELIGHTDIRECTIONOVERRIDE_ON)
                worldLightDir = normalize(_LightDirectionOverride.xyz);
#endif

                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

#if defined(_USESOFTFLUSH_ON)
                col.rgb = ApplySoftFlush(col.rgb, i.uv, i.worldNormal, viewDir);
#endif

#if defined(_USEEXCITEDTONE_ON)
                col.rgb = ApplyExcitedTone(col.rgb, i.uv, i.worldNormal, viewDir);
#endif

                float shadow1 = SamplePrimaryShadow(i);
                float shadow2 = 1.0;
                float shadow3 = 1.0;

#if defined(_USESHADOW2_ON)
                shadow2 = ApplyShadowThreshold(SHADOW_ATTENUATION(i), _Shadow2Border, _Shadow2Blur);
#endif

#if defined(_USESHADOW3_ON)
                shadow3 = ApplyShadowThreshold(SHADOW_ATTENUATION(i), _Shadow3Border, _Shadow3Blur);
#endif

#if defined(_USESDFFACESHADOW_ON)
                float sdfFaceShadow = CalculateSDFFaceShadow(i.uv, i.worldNormal);
                shadow1 *= sdfFaceShadow;
#endif

#if defined(_USEREALISTICSHADOW_ON)
                float realisticShadow = CalculateRealisticShadow(i.worldNormal, worldLightDir);
                shadow1 *= (1.0 - realisticShadow);
#endif

                float finalShadow = shadow1 * shadow2 * shadow3;
                finalShadow = ApplyNoLightPCSSBoost(finalShadow, i.worldNormal, viewDir, worldLightDir);
                finalShadow = ApplyVRChatPerformanceGate(finalShadow, i.worldPos);

#if defined(_USESHADOWMASK_ON)
                fixed4 mask = tex2D(_ShadowMaskTex, i.uv);
                finalShadow = lerp(finalShadow, 1.0, mask.g * _ShadowMaskStrength);
#endif

#if defined(_USEFLIPBOOK_ON)
                float totalFrames = _FlipbookDivisionsX * _FlipbookDivisionsY;
                float currentFrame = floor(fmod(_Time.y * _FlipbookSpeed, totalFrames));
                float frameX = fmod(currentFrame, _FlipbookDivisionsX);
                float frameY = floor(currentFrame / _FlipbookDivisionsX);
                float2 frameUv = i.uv / float2(_FlipbookDivisionsX, _FlipbookDivisionsY);
                frameUv += float2(frameX / _FlipbookDivisionsX, -frameY / _FlipbookDivisionsY);
                col *= tex2D(_FlipbookTex, frameUv);
#endif

#if defined(_USEFUR_ON)
    #if defined(_USEFUROPTIMIZATION_ON)
                float furOptimizationFactor = 1.0 - (_FurOptimizationLevel * 0.25);
                float furDensity = _FurDensity * furOptimizationFactor;
                float furSubdivision = max(1.0, _FurSubdivision * furOptimizationFactor);
    #else
                float furDensity = _FurDensity;
                float furSubdivision = _FurSubdivision;
    #endif
                float3 furColor = CalculateFur(i.uv, i.worldNormal, furDensity, furSubdivision);
                col.rgb *= furColor;
#endif

#if defined(_USESHADOWCLAMP_ON)
                finalShadow = step(_ShadowClamp, finalShadow);
#endif

#if defined(VRC_LIGHT_VOLUMES_ENABLED) && defined(_USEVRCLIGHT_VOLUMES_ON)
                float3 lightVolumeColor = SampleVRCLightVolumes(i.worldPos, i.worldNormal);
                col.rgb *= lightVolumeColor;
#endif

#if defined(_USELTCGI_ON)
                float3 ltcgiColor = CalculateLTCGI(i.worldPos, i.worldNormal);
                col.rgb *= ltcgiColor;
#endif

                float3 backlightColor = CalculateBacklight(i.worldNormal, worldLightDir);
                col.rgb += backlightColor;

#if defined(_USERIMSHADE_ON)
                float3 rimShadeColor = CalculateRimShade(i.worldNormal, viewDir);
                col.rgb += rimShadeColor;
#endif

                finalShadow = lerp(1.0 - _Translucency, 1.0, finalShadow);

#if defined(_SHADOWCOLORTEX)
                col.rgb *= lerp(tex2D(_ShadowColorTex, i.uv).rgb, float3(1, 1, 1), finalShadow);
#else
                col.rgb *= lerp(float3(0.5, 0.5, 0.5), float3(1, 1, 1), finalShadow);
#endif

#if defined(_USEGLOSSSHADOWCOHERENCE_ON)
                col.rgb = ApplyGlossShadowCoherence(col.rgb, finalShadow, i.worldNormal, viewDir, worldLightDir);
#endif

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
        Pass
        {
            Name "ShadowCaster"
            Tags {"LightMode" = "ShadowCaster"}
            ZWrite On
            ZTest LEqual
            Cull [_Cull]
            
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing
            
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Cutoff;
            
            float4 _RealisticShadowOffset;
            float4 _RealisticShadowScale;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                V2F_SHADOW_CASTER;
                float2 uv : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                float3 offsetPos = v.vertex.xyz + _RealisticShadowOffset.xyz;
                offsetPos *= _RealisticShadowScale.xyz;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                clip(col.a - _Cutoff);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "lilToon"
    CustomEditor "lilToon.PCSS.Editor.LilToonPCSSShaderGUI"
} 

