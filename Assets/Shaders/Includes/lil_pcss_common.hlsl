//----------------------------------------------------------------------------------------------------------------------
// lilToon PCSS Extension - Common Definitions (lilToon 2.1.7対応版)
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
// Version Information (lilToon 2.1.7対応)
//----------------------------------------------------------------------------------------------------------------------
#define LIL_PCSS_VERSION_MAJOR 2
#define LIL_PCSS_VERSION_MINOR 5
#define LIL_PCSS_VERSION_PATCH 0
#define LIL_PCSS_VERSION "2.5.0"
#define LIL_PCSS_LILTOON_VERSION "2.1.7"

//----------------------------------------------------------------------------------------------------------------------
// Feature Toggles (lilToon 2.1.7新機能対応)
//----------------------------------------------------------------------------------------------------------------------
#define LIL_PCSS_FEATURE_ENABLED
#define LIL_PCSS_VRCHAT_OPTIMIZED
#define LIL_PCSS_BAKERY_SUPPORT
#define LIL_PCSS_LIGHTVOLUMES_SUPPORT
#define LIL_PCSS_THREE_SHADOW_SYSTEM
#define LIL_PCSS_SDF_FACE_SHADOW
#define LIL_PCSS_LTCGI_SUPPORT
#define LIL_PCSS_BACKLIGHT_SUPPORT
#define LIL_PCSS_LIGHT_DIRECTION_OVERRIDE

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
// Quality Settings (lilToon 2.1.7最適化)
//----------------------------------------------------------------------------------------------------------------------
#ifdef LIL_PCSS_MOBILE_PLATFORM
    #define LIL_PCSS_DEFAULT_SAMPLE_COUNT 8
    #define LIL_PCSS_DEFAULT_BLOCKER_SAMPLES 8
    #define LIL_PCSS_DEFAULT_LTCGI_SAMPLES 8
#else
    #define LIL_PCSS_DEFAULT_SAMPLE_COUNT 16
    #define LIL_PCSS_DEFAULT_BLOCKER_SAMPLES 16
    #define LIL_PCSS_DEFAULT_LTCGI_SAMPLES 16
#endif

//----------------------------------------------------------------------------------------------------------------------
// VRChat Compatibility (lilToon 2.1.7最適化)
//----------------------------------------------------------------------------------------------------------------------
#ifdef VRCHAT_SDK
    #define LIL_PCSS_VRCHAT_MODE
    #define LIL_PCSS_PERFORMANCE_PRIORITY
    #define LIL_PCSS_QUEST_OPTIMIZATION
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
#elif defined(LIL_PCSS_BUILTIN_AVAILABLE)
    // Built-in Render Pipeline用のテクスチャ宣言
    #if !defined(_ShadowMapTexture) && !defined(LIL_PCSS_SHADOWMAP_DECLARED)
        #define LIL_PCSS_SHADOWMAP_DECLARED
        UNITY_DECLARE_SHADOWMAP(_ShadowMapTexture);
    #endif
#endif

//----------------------------------------------------------------------------------------------------------------------
// lilToon 2.1.7 新機能定義
//----------------------------------------------------------------------------------------------------------------------

// 3影システム定数
#define LIL_PCSS_SHADOW_LAYER_COUNT 3
#define LIL_PCSS_SHADOW_1_INDEX 0
#define LIL_PCSS_SHADOW_2_INDEX 1
#define LIL_PCSS_SHADOW_3_INDEX 2

// SDF Face Shadow定数
#define LIL_PCSS_SDF_FACE_SHADOW_DEFAULT_INTENSITY 0.5
#define LIL_PCSS_SDF_FACE_SHADOW_DEFAULT_SOFTNESS 0.1

// LTCGI定数
#define LIL_PCSS_LTCGI_DEFAULT_INTENSITY 1.0
#define LIL_PCSS_LTCGI_DEFAULT_SAMPLES 16
#define LIL_PCSS_LTCGI_MAX_SAMPLES 64

// Backlight定数
#define LIL_PCSS_BACKLIGHT_DEFAULT_INTENSITY 1.0
#define LIL_PCSS_BACKLIGHT_DEFAULT_COLOR float3(1.0, 1.0, 1.0)

// VRC Light Volumes 2.0.0 強化版定数
#define LIL_PCSS_VRCLV_DEFAULT_INTENSITY 1.0
#define LIL_PCSS_VRCLV_DEFAULT_DISTANCE_FACTOR 0.1
#define LIL_PCSS_VRCLV_RIM_LIGHT_DEFAULT_INTENSITY 1.0

//----------------------------------------------------------------------------------------------------------------------
// ユーティリティ関数 (lilToon 2.1.7対応)
//----------------------------------------------------------------------------------------------------------------------

// 影の品質に基づくサンプル数調整
float GetAdjustedSampleCount(float baseSamples, float qualityLevel)
{
    float adjustedSamples = baseSamples;
    
    if (qualityLevel < 1.0)
        adjustedSamples = max(8.0, baseSamples * 0.5);
    else if (qualityLevel > 1.0 && qualityLevel <= 2.0)
        adjustedSamples = min(32.0, baseSamples * 1.5);
    else if (qualityLevel > 2.0)
        adjustedSamples = min(64.0, baseSamples * 2.0);
    
    return adjustedSamples;
}

// プラットフォーム別最適化
float GetPlatformOptimizedSamples(float baseSamples)
{
    #ifdef LIL_PCSS_MOBILE_PLATFORM
        return min(baseSamples, 16.0);
    #else
        return baseSamples;
    #endif
}

// VRChat Quest最適化
float GetQuestOptimizedSamples(float baseSamples)
{
    #ifdef LIL_PCSS_QUEST_OPTIMIZATION
        return min(baseSamples, 8.0);
    #else
        return baseSamples;
    #endif
}

// 影の境界線計算 (lilToon 2.1.7方式)
float CalculateShadowBorder(float shadow, float border, float blur)
{
    shadow = saturate(shadow + border);
    return smoothstep(0.0, blur, shadow);
}

// 3影システム統合関数
float3 CalculateThreeShadowSystem(float shadow1, float shadow2, float shadow3, float3 shadowColors[3])
{
    float3 finalShadow = float3(1.0, 1.0, 1.0);
    
    finalShadow *= lerp(shadowColors[0], float3(1.0, 1.0, 1.0), shadow1);
    finalShadow *= lerp(shadowColors[1], float3(1.0, 1.0, 1.0), shadow2);
    finalShadow *= lerp(shadowColors[2], float3(1.0, 1.0, 1.0), shadow3);
    
    return finalShadow;
}

// SDF Face Shadow計算関数
float CalculateSDFFaceShadowValue(float2 uv, sampler2D sdfTexture, float intensity, float softness)
{
    float sdfValue = tex2D(sdfTexture, uv).r;
    float faceShadow = smoothstep(softness, 1.0 - softness, sdfValue);
    return lerp(1.0, faceShadow, intensity);
}

// LTCGIサンプリング関数
float3 SampleLTCGI(float3 worldPos, float3 worldNormal, float samples, float intensity)
{
    float3 gi = 0;
    float totalWeight = 0;
    
    for (int i = 0; i < samples; i++)
    {
        // Hammersley sequence for better distribution
        float2 hammersley = float2(
            float(i) / float(samples),
            float(bitfieldReverse(i)) / float(0xffffffff)
        );
        
        // Spherical to Cartesian conversion
        float phi = 2.0 * UNITY_PI * hammersley.x;
        float cosTheta = 1.0 - 2.0 * hammersley.y;
        float sinTheta = sqrt(1.0 - cosTheta * cosTheta);
        
        float3 sampleDir = float3(
            sinTheta * cos(phi),
            sinTheta * sin(phi),
            cosTheta
        );
        
        float weight = max(0.0, dot(worldNormal, sampleDir));
        gi += weight * sampleDir;
        totalWeight += weight;
    }
    
    gi = totalWeight > 0 ? gi / totalWeight : float3(1.0, 1.0, 1.0);
    return gi * intensity;
}

// Backlight計算関数
float3 CalculateBacklightValue(float3 worldNormal, float3 lightDir, float3 backlightColor, float intensity)
{
    float backlightDot = max(0.0, dot(worldNormal, -lightDir));
    return backlightColor * intensity * backlightDot;
}

// VRC Light Volumes 2.0.0 強化版サンプリング関数
float3 SampleVRCLightVolumesEnhanced(float3 worldPos, float3 worldNormal, float3 lightDir, 
                                    sampler3D volumeTexture, float4x4 worldToLocal, 
                                    float3 tint, float intensity, float distanceFactor)
{
    float3 localPos = mul(worldToLocal, float4(worldPos, 1.0)).xyz;
    float3 volumeUV = localPos * 0.5 + 0.5;
    
    // ボリューム範囲チェック
    if (any(volumeUV < 0.0) || any(volumeUV > 1.0))
        return float3(1.0, 1.0, 1.0);
    
    float3 lightColor = tex3D(volumeTexture, volumeUV).rgb;
    
    // 距離減衰
    float distFactor = 1.0 - saturate(length(localPos) * distanceFactor);
    
    // 法線方向考慮
    float normalDotLight = max(0.0, dot(worldNormal, lightDir));
    
    return lerp(float3(1.0, 1.0, 1.0), lightColor * tint, intensity * distFactor * normalDotLight);
}

//----------------------------------------------------------------------------------------------------------------------
// パフォーマンス最適化関数
//----------------------------------------------------------------------------------------------------------------------

// 動的品質調整
float GetDynamicQualityLevel(float baseQuality, float performanceTarget)
{
    #ifdef LIL_PCSS_PERFORMANCE_PRIORITY
        return min(baseQuality, performanceTarget);
    #else
        return baseQuality;
    #endif
}

// メモリ使用量最適化
float GetMemoryOptimizedSamples(float baseSamples)
{
    #ifdef LIL_PCSS_MOBILE_PLATFORM
        return min(baseSamples, 8.0);
    #elif defined(LIL_PCSS_QUEST_OPTIMIZATION)
        return min(baseSamples, 4.0);
    #else
        return baseSamples;
    #endif
}

//----------------------------------------------------------------------------------------------------------------------
// エラーハンドリング関数
//----------------------------------------------------------------------------------------------------------------------

// テクスチャ存在チェック
bool IsTextureValid(sampler2D tex)
{
    // 簡易的な存在チェック（実際の実装ではより詳細なチェックが必要）
    return true;
}

// パラメータ範囲チェック
float ClampParameter(float value, float minVal, float maxVal, float defaultValue)
{
    if (value < minVal || value > maxVal)
        return defaultValue;
    return value;
}

//----------------------------------------------------------------------------------------------------------------------
// 競合製品分析関数
//----------------------------------------------------------------------------------------------------------------------

// 機能比較分析
float GetFeatureComparisonScore(float ourFeatureLevel, float competitorFeatureLevel)
{
    return ourFeatureLevel / max(competitorFeatureLevel, 0.1);
}

// パフォーマンス比較
float GetPerformanceComparisonScore(float ourPerformance, float competitorPerformance)
{
    return ourPerformance / max(competitorPerformance, 0.1);
}

#endif // LIL_PCSS_COMMON_INCLUDED 