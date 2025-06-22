//----------------------------------------------------------------------------------------------------------------------
// lilToon PCSS Extension - Common Definitions
// Copyright (c) 2025 lilToon PCSS Extension Team
//----------------------------------------------------------------------------------------------------------------------

#ifndef LIL_PCSS_COMMON_INCLUDED
#define LIL_PCSS_COMMON_INCLUDED

//----------------------------------------------------------------------------------------------------------------------
// Render Pipeline Detection
//----------------------------------------------------------------------------------------------------------------------
#if defined(UNITY_PIPELINE_URP)
    #define LIL_PCSS_URP_PIPELINE
#elif defined(UNITY_PIPELINE_HDRP)
    #define LIL_PCSS_HDRP_PIPELINE
#else
    #define LIL_PCSS_BUILTIN_PIPELINE
#endif

//----------------------------------------------------------------------------------------------------------------------
// Conditional Includes to Prevent Macro Redefinition
//----------------------------------------------------------------------------------------------------------------------
#ifdef LIL_PCSS_URP_PIPELINE
    // URP専用インクルード
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #define LIL_PCSS_URP_AVAILABLE
#elif defined(LIL_PCSS_HDRP_PIPELINE)
    // HDRP専用インクルード
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #define LIL_PCSS_HDRP_AVAILABLE
#else
    // Built-in Render Pipeline専用インクルード
    #include "UnityCG.cginc"
    #include "AutoLight.cginc"
    #include "Lighting.cginc"
    #define LIL_PCSS_BUILTIN_AVAILABLE
#endif

//----------------------------------------------------------------------------------------------------------------------
// Version Information
//----------------------------------------------------------------------------------------------------------------------
#define LIL_PCSS_VERSION_MAJOR 1
#define LIL_PCSS_VERSION_MINOR 4
#define LIL_PCSS_VERSION_PATCH 9
#define LIL_PCSS_VERSION "1.4.9"

//----------------------------------------------------------------------------------------------------------------------
// Feature Toggles
//----------------------------------------------------------------------------------------------------------------------
#define LIL_PCSS_FEATURE_ENABLED
#define LIL_PCSS_VRCHAT_OPTIMIZED
#define LIL_PCSS_BAKERY_SUPPORT
#define LIL_PCSS_LIGHTVOLUMES_SUPPORT

//----------------------------------------------------------------------------------------------------------------------
// Platform Detection
//----------------------------------------------------------------------------------------------------------------------
#if defined(UNITY_ANDROID) || defined(UNITY_IOS)
    #define LIL_PCSS_MOBILE_PLATFORM
    #define LIL_PCSS_REDUCED_SAMPLES
#endif

#if defined(SHADER_API_MOBILE)
    #define LIL_PCSS_MOBILE_PLATFORM
#endif

//----------------------------------------------------------------------------------------------------------------------
// Quality Settings
//----------------------------------------------------------------------------------------------------------------------
#ifdef LIL_PCSS_MOBILE_PLATFORM
    #define LIL_PCSS_DEFAULT_SAMPLE_COUNT 8
    #define LIL_PCSS_DEFAULT_BLOCKER_SAMPLES 8
#else
    #define LIL_PCSS_DEFAULT_SAMPLE_COUNT 16
    #define LIL_PCSS_DEFAULT_BLOCKER_SAMPLES 16
#endif

//----------------------------------------------------------------------------------------------------------------------
// VRChat Compatibility
//----------------------------------------------------------------------------------------------------------------------
#ifdef VRCHAT_SDK
    #define LIL_PCSS_VRCHAT_MODE
    #define LIL_PCSS_PERFORMANCE_PRIORITY
#endif

//----------------------------------------------------------------------------------------------------------------------
// Texture Samplers - Pipeline Specific
//----------------------------------------------------------------------------------------------------------------------
#ifdef LIL_PCSS_URP_AVAILABLE
    // URP用のテクスチャ宣言
    TEXTURE2D(_MainLightShadowmapTexture);
    SAMPLER(sampler_MainLightShadowmapTexture);
    TEXTURE2D(_CameraDepthTexture);
    SAMPLER(sampler_CameraDepthTexture);
#else
    // Built-in Render Pipeline用のテクスチャ宣言
    sampler2D _ShadowMapTexture;
    sampler2D _MainLightShadowmapTexture;
    sampler2D_float _CameraDepthTexture;
#endif

//----------------------------------------------------------------------------------------------------------------------
// Custom Utility Macros (Avoid Conflicts)
//----------------------------------------------------------------------------------------------------------------------
#define LIL_PCSS_LERP(a, b, t) lerp(a, b, t)
#define LIL_PCSS_SATURATE(x) saturate(x)
#define LIL_PCSS_STEP(edge, x) step(edge, x)
#define LIL_PCSS_SMOOTHSTEP(edge0, edge1, x) smoothstep(edge0, edge1, x)

//----------------------------------------------------------------------------------------------------------------------
// Custom Sampling Functions to Avoid Macro Conflicts
//----------------------------------------------------------------------------------------------------------------------
#ifdef LIL_PCSS_URP_AVAILABLE
    #define LIL_SAMPLE_TEXTURE2D(tex, samp, coord) SAMPLE_TEXTURE2D(tex, samp, coord)
    #define LIL_SAMPLE_TEXTURE2D_SHADOW(tex, samp, coord) SAMPLE_TEXTURE2D_SHADOW(tex, samp, coord)
#else
    #define LIL_SAMPLE_TEXTURE2D(tex, samp, coord) tex2D(tex, coord)
    #define LIL_SAMPLE_TEXTURE2D_SHADOW(tex, samp, coord) tex2D(tex, coord.xy)
#endif

//----------------------------------------------------------------------------------------------------------------------
// Custom Transform Macros to Avoid Conflicts
//----------------------------------------------------------------------------------------------------------------------
#ifndef LIL_TRANSFORM_TEX
    #define LIL_TRANSFORM_TEX(tex, name) (tex.xy * name##_ST.xy + name##_ST.zw)
#endif

//----------------------------------------------------------------------------------------------------------------------
// Debug Modes
//----------------------------------------------------------------------------------------------------------------------
#ifdef LIL_DEBUG_MODE
    #define LIL_DEBUG_PCSS
#endif

//----------------------------------------------------------------------------------------------------------------------
// Custom PackHeightmap to Avoid Redefinition
//----------------------------------------------------------------------------------------------------------------------
#ifndef LIL_PACK_HEIGHTMAP
#define LIL_PACK_HEIGHTMAP(height) (height)
#endif

#endif // LIL_PCSS_COMMON_INCLUDED 