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
// Texture Samplers - Pipeline Specific with Complete Conflict Prevention
//----------------------------------------------------------------------------------------------------------------------
#ifdef LIL_PCSS_URP_AVAILABLE
    // URP用のテクスチャ宣言 - 条件付きで重複を完全防止
    #if !defined(_MainLightShadowmapTexture) && !defined(LIL_PCSS_SHADOWMAP_DECLARED)
        #define LIL_PCSS_SHADOWMAP_DECLARED
        TEXTURE2D(_MainLightShadowmapTexture);
        SAMPLER(sampler_MainLightShadowmapTexture);
    #endif
    #if !defined(_CameraDepthTexture) && !defined(LIL_PCSS_DEPTH_DECLARED)
        #define LIL_PCSS_DEPTH_DECLARED
        TEXTURE2D(_CameraDepthTexture);
        SAMPLER(sampler_CameraDepthTexture);
    #endif
#else
    // Built-in Render Pipeline用のテクスチャ宣言 - 完全な重複防止
    #if !defined(_ShadowMapTexture) && !defined(LIL_PCSS_BUILTIN_SHADOWMAP_DECLARED)
        #define LIL_PCSS_BUILTIN_SHADOWMAP_DECLARED
        #define _ShadowMapTexture _LIL_PCSS_ShadowMapTexture
        sampler2D _LIL_PCSS_ShadowMapTexture;
    #endif
    #if !defined(_MainLightShadowmapTexture) && !defined(LIL_PCSS_MAIN_SHADOWMAP_DECLARED)
        #define LIL_PCSS_MAIN_SHADOWMAP_DECLARED
        #define _MainLightShadowmapTexture _LIL_PCSS_MainLightShadowmapTexture
        sampler2D _LIL_PCSS_MainLightShadowmapTexture;
    #endif
    #if !defined(_CameraDepthTexture) && !defined(LIL_PCSS_BUILTIN_DEPTH_DECLARED)
        #define LIL_PCSS_BUILTIN_DEPTH_DECLARED
        sampler2D_float _CameraDepthTexture;
    #endif
#endif

//----------------------------------------------------------------------------------------------------------------------
// PCSS Properties - 全変数を一箇所で定義（重複完全防止）
//----------------------------------------------------------------------------------------------------------------------
#ifndef LIL_PCSS_PROPERTIES_DEFINED
#define LIL_PCSS_PROPERTIES_DEFINED

// Shader Compatibility Detection
#ifndef LIL_PCSS_SHADER_COMPATIBILITY_DEFINED
#define LIL_PCSS_SHADER_COMPATIBILITY_DEFINED

// lilToon Detection
#if defined(LIL_LILTOON_SHADER_INCLUDED) || defined(SHADER_STAGE_VERTEX) || defined(_LILTOON_INCLUDED)
    #define LIL_PCSS_LILTOON_AVAILABLE
#endif

// Poiyomi Detection  
#if defined(POI_MAIN) || defined(POIYOMI_TOON) || defined(_POIYOMI_INCLUDED)
    #define LIL_PCSS_POIYOMI_AVAILABLE
#endif

// Graceful Degradation for Missing Shaders
#if !defined(LIL_PCSS_LILTOON_AVAILABLE) && !defined(LIL_PCSS_POIYOMI_AVAILABLE)
    #define LIL_PCSS_STANDALONE_MODE
#endif

#endif // LIL_PCSS_SHADER_COMPATIBILITY_DEFINED

CBUFFER_START(lilPCSSProperties)
    // 基本PCSS設定 - 全シェーダーで共通使用
    float _PCSSEnabled;
    float _PCSSFilterRadius;
    float _PCSSLightSize;
    float _PCSSBias;
    float _PCSSIntensity;
    float _PCSSQuality;
    float _PCSSBlockerSearchRadius;
    float _PCSSSampleCount;
    
    // ライト設定
    float4 _PCSSLightDirection;
    float4x4 _PCSSLightMatrix;
    
    // プリセット設定
    float _PCSSPresetMode; // 0=リアル, 1=アニメ, 2=映画風, 3=カスタム
    float _PCSSRealtimePreset;
    float _PCSSAnimePreset;
    float _PCSSCinematicPreset;
CBUFFER_END

#endif // LIL_PCSS_PROPERTIES_DEFINED

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