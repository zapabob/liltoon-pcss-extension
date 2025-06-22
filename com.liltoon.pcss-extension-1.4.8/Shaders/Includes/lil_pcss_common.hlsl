//----------------------------------------------------------------------------------------------------------------------
// lilToon PCSS Extension - Common Definitions
// Copyright (c) 2025 lilToon PCSS Extension Team
//----------------------------------------------------------------------------------------------------------------------

#ifndef LIL_PCSS_COMMON_INCLUDED
#define LIL_PCSS_COMMON_INCLUDED

// Unity標準のインクルード
#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "Lighting.cginc"

// Unity 2022.3 LTS URP互換性のためのインクルード
#if defined(UNITY_VERSION) && UNITY_VERSION >= 202220
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#endif

//----------------------------------------------------------------------------------------------------------------------
// Version Information
//----------------------------------------------------------------------------------------------------------------------
#define LIL_PCSS_VERSION_MAJOR 1
#define LIL_PCSS_VERSION_MINOR 2
#define LIL_PCSS_VERSION_PATCH 0
#define LIL_PCSS_VERSION "1.2.0"

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
// Unity Core RP Library Includes
//----------------------------------------------------------------------------------------------------------------------
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

//----------------------------------------------------------------------------------------------------------------------
// Texture Samplers
//----------------------------------------------------------------------------------------------------------------------
#if defined(UNITY_VERSION) && UNITY_VERSION >= 202220 && defined(URP_AVAILABLE)
    // URP用のテクスチャ宣言
    TEXTURE2D(_MainLightShadowmapTexture);
    SAMPLER(sampler_MainLightShadowmapTexture);
#else
    // Built-in Render Pipeline用のテクスチャ宣言
    sampler2D _ShadowMapTexture;
    sampler2D _MainLightShadowmapTexture;
    sampler2D_float _CameraDepthTexture;
#endif

//----------------------------------------------------------------------------------------------------------------------
// Utility Macros
//----------------------------------------------------------------------------------------------------------------------
#define LIL_PCSS_LERP(a, b, t) lerp(a, b, t)
#define LIL_PCSS_SATURATE(x) saturate(x)
#define LIL_PCSS_STEP(edge, x) step(edge, x)
#define LIL_PCSS_SMOOTHSTEP(edge0, edge1, x) smoothstep(edge0, edge1, x)

//----------------------------------------------------------------------------------------------------------------------
// Debug Modes
//----------------------------------------------------------------------------------------------------------------------
#ifdef LIL_DEBUG_MODE
    #define LIL_DEBUG_PCSS
#endif

#endif // LIL_PCSS_COMMON_INCLUDED 