//----------------------------------------------------------------------------------------------------------------------
// lilToon PCSS Extension - Shadow Functions
// Copyright (c) 2025 lilToon PCSS Extension Team
//----------------------------------------------------------------------------------------------------------------------

#ifndef LIL_PCSS_SHADOWS_INCLUDED
#define LIL_PCSS_SHADOWS_INCLUDED

#include "lil_pcss_common.hlsl"

//----------------------------------------------------------------------------------------------------------------------
// PCSS Configuration
//----------------------------------------------------------------------------------------------------------------------
#define PCSS_ENABLED 1
#define PCSS_SAMPLE_COUNT 16
#define PCSS_BLOCKER_SEARCH_SAMPLES 16
#define PCSS_FILTER_RADIUS_SCALE 1.0
#define PCSS_LIGHT_SIZE_SCALE 0.1
#define PCSS_BIAS_SCALE 0.001

//----------------------------------------------------------------------------------------------------------------------
// PCSS Properties - 重複を完全に防ぐため、変数宣言をlil_pcss_common.hlslに集約
// このファイルでは変数宣言は行わず、関数のみを定義
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
// Sampling Patterns
//----------------------------------------------------------------------------------------------------------------------
static const float2 PoissonDisk[16] = {
    float2(-0.94201624, -0.39906216),
    float2(0.94558609, -0.76890725),
    float2(-0.094184101, -0.92938870),
    float2(0.34495938, 0.29387760),
    float2(-0.91588581, 0.45771432),
    float2(-0.81544232, -0.87912464),
    float2(-0.38277543, 0.27676845),
    float2(0.97484398, 0.75648379),
    float2(0.44323325, -0.97511554),
    float2(0.53742981, -0.47373420),
    float2(-0.26496911, -0.41893023),
    float2(0.79197514, 0.19090188),
    float2(-0.24188840, 0.99706507),
    float2(-0.81409955, 0.91437590),
    float2(0.19984126, 0.78641367),
    float2(0.14383161, -0.14100790)
};

//----------------------------------------------------------------------------------------------------------------------
// Utility Functions
//----------------------------------------------------------------------------------------------------------------------
float Random(float2 seed)
{
    return frac(sin(dot(seed, float2(12.9898, 78.233))) * 43758.5453);
}

float2 randomRotation(float2 uv)
{
    float angle = Random(uv) * 6.28318530718; // 2 * PI
    float cosAngle = cos(angle);
    float sinAngle = sin(angle);
    return float2(cosAngle, sinAngle);
}

//----------------------------------------------------------------------------------------------------------------------
// PCSS Shadow Sampling Functions
//----------------------------------------------------------------------------------------------------------------------

// Sample shadow map using custom macros
float SampleShadowMap(float2 uv, float compareValue)
{
    #ifdef LIL_PCSS_URP_AVAILABLE
        // URP用のサンプリング
        #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D12) || defined(SHADER_API_VULKAN)
            return LIL_SAMPLE_TEXTURE2D_SHADOW(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture, float3(uv, compareValue));
        #else
            float shadowSample = LIL_SAMPLE_TEXTURE2D(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture, uv).r;
            return step(compareValue, shadowSample);
        #endif
    #else
        // Built-in Render Pipeline用のサンプリング - 重複を防ぐため条件付き
        #ifdef _LIL_PCSS_ShadowMapTexture
            float shadowSample = LIL_SAMPLE_TEXTURE2D(_LIL_PCSS_ShadowMapTexture, sampler, uv).r;
        #elif defined(_MainLightShadowmapTexture)
            float shadowSample = LIL_SAMPLE_TEXTURE2D(_MainLightShadowmapTexture, sampler, uv).r;
        #else
            float shadowSample = LIL_SAMPLE_TEXTURE2D(_LIL_PCSS_MainLightShadowmapTexture, sampler, uv).r;
        #endif
        return step(compareValue, shadowSample);
    #endif
}

// Blocker search for PCSS
float FindBlockerDistance(float2 shadowCoord, float receiverDepth, float searchRadius, int numSamples)
{
    float blockerSum = 0.0;
    int numBlockers = 0;
    
    float2 randomOffset = float2(Random(shadowCoord), Random(shadowCoord.yx));
    
    for (int i = 0; i < numSamples; i++)
    {
        float2 offset = PoissonDisk[i % 16] * searchRadius;
        offset = lerp(offset, offset + randomOffset * 0.1, 0.5); // Add slight randomization
        
        float2 sampleCoord = shadowCoord + offset;
        #ifdef LIL_PCSS_URP_AVAILABLE
            float shadowDepth = LIL_SAMPLE_TEXTURE2D(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture, sampleCoord).r;
        #else
            // Built-in Render Pipeline用のサンプリング - 重複を防ぐため条件付き
            #ifdef _LIL_PCSS_ShadowMapTexture
                float shadowDepth = LIL_SAMPLE_TEXTURE2D(_LIL_PCSS_ShadowMapTexture, sampler, sampleCoord).r;
            #elif defined(_MainLightShadowmapTexture)
                float shadowDepth = LIL_SAMPLE_TEXTURE2D(_MainLightShadowmapTexture, sampler, sampleCoord).r;
            #else
                float shadowDepth = LIL_SAMPLE_TEXTURE2D(_LIL_PCSS_MainLightShadowmapTexture, sampler, sampleCoord).r;
            #endif
        #endif
        
        if (shadowDepth < receiverDepth)
        {
            blockerSum += shadowDepth;
            numBlockers++;
        }
    }
    
    return numBlockers > 0 ? blockerSum / numBlockers : -1.0;
}

// PCF filtering
float PCF(float2 shadowCoord, float receiverDepth, float filterRadius, int numSamples)
{
    float shadow = 0.0;
    float2 randomOffset = float2(Random(shadowCoord), Random(shadowCoord.yx));
    
    for (int i = 0; i < numSamples; i++)
    {
        float2 offset = PoissonDisk[i % 16] * filterRadius;
        offset = lerp(offset, offset + randomOffset * 0.1, 0.5); // Add slight randomization
        
        float2 sampleCoord = shadowCoord + offset;
        shadow += SampleShadowMap(sampleCoord, receiverDepth);
    }
    
    return shadow / numSamples;
}

//----------------------------------------------------------------------------------------------------------------------
// PCSS Preset Functions
//----------------------------------------------------------------------------------------------------------------------

// リアル影プリセット
float4 GetRealtimeShadowPreset()
{
    return float4(
        0.005,  // FilterRadius - 細かいディテール
        0.05,   // LightSize - 小さい光源
        0.0005, // Bias - 低いバイアス
        1.0     // Intensity - フル強度
    );
}

// アニメ風プリセット
float4 GetAnimeShadowPreset()
{
    return float4(
        0.015,  // FilterRadius - ソフトなエッジ
        0.1,    // LightSize - 中程度の光源
        0.001,  // Bias - 標準バイアス
        0.8     // Intensity - やや弱め
    );
}

// 映画風プリセット
float4 GetCinematicShadowPreset()
{
    return float4(
        0.025,  // FilterRadius - 非常にソフト
        0.2,    // LightSize - 大きい光源
        0.002,  // Bias - 高めのバイアス
        1.2     // Intensity - 強調された影
    );
}

// プリセットに基づいてパラメータを取得
float4 GetPCSSPresetParameters()
{
    if (_PCSSPresetMode < 0.5) // リアル
    {
        return GetRealtimeShadowPreset();
    }
    else if (_PCSSPresetMode < 1.5) // アニメ
    {
        return GetAnimeShadowPreset();
    }
    else if (_PCSSPresetMode < 2.5) // 映画風
    {
        return GetCinematicShadowPreset();
    }
    else // カスタム
    {
        return float4(_PCSSFilterRadius, _PCSSLightSize, _PCSSBias, _PCSSIntensity);
    }
}

// Main PCSS function with presets
float PCSS(float4 shadowCoord, float receiverDepth)
{
    // プリセットパラメータを取得
    float4 presetParams = GetPCSSPresetParameters();
    float filterRadius = presetParams.x;
    float lightSize = presetParams.y;
    float bias = presetParams.z;
    float intensity = presetParams.w;
    
    // Normalize shadow coordinates
    float3 projCoords = shadowCoord.xyz / shadowCoord.w;
    
    // Check if we're outside the shadow map
    if (projCoords.x < 0.0 || projCoords.x > 1.0 || 
        projCoords.y < 0.0 || projCoords.y > 1.0 ||
        projCoords.z < 0.0 || projCoords.z > 1.0)
    {
        return 1.0; // No shadow
    }
    
    float2 shadowUV = projCoords.xy;
    float depth = projCoords.z;
    
    // Apply bias to reduce shadow acne
    depth -= bias;
    
    // Step 1: Blocker search
    float searchRadius = lightSize * 0.1;
    float avgBlockerDepth = FindBlockerDistance(shadowUV, depth, searchRadius, LIL_PCSS_DEFAULT_BLOCKER_SAMPLES);
    
    if (avgBlockerDepth < 0.0)
    {
        return 1.0; // No blockers found
    }
    
    // Step 2: Penumbra estimation
    float penumbraWidth = (depth - avgBlockerDepth) / avgBlockerDepth;
    penumbraWidth *= lightSize;
    penumbraWidth = max(penumbraWidth, 0.001); // Minimum filter size
    
    // Step 3: PCF filtering
    float finalFilterRadius = penumbraWidth * filterRadius;
    int sampleCount = clamp(LIL_PCSS_DEFAULT_SAMPLE_COUNT, 4, 32);
    
    float shadowValue = PCF(shadowUV, depth, finalFilterRadius, sampleCount);
    
    // Apply intensity
    return lerp(1.0, shadowValue, intensity);
}

// Simplified PCSS for mobile platforms
float PCSSMobile(float4 shadowCoord, float receiverDepth)
{
    float4 presetParams = GetPCSSPresetParameters();
    float filterRadius = presetParams.x * 0.5; // Reduced for mobile
    float bias = presetParams.z;
    float intensity = presetParams.w;
    
    float3 projCoords = shadowCoord.xyz / shadowCoord.w;
    
    if (projCoords.x < 0.0 || projCoords.x > 1.0 || 
        projCoords.y < 0.0 || projCoords.y > 1.0 ||
        projCoords.z < 0.0 || projCoords.z > 1.0)
    {
        return 1.0;
    }
    
    float2 shadowUV = projCoords.xy;
    float depth = projCoords.z - bias;
    
    // Simplified single-step PCF for mobile
    float shadowValue = PCF(shadowUV, depth, filterRadius, 8);
    return lerp(1.0, shadowValue, intensity);
}

// VRC Light Volumes integration
float3 SampleVRCLightVolumes(float3 worldPos)
{
    #if defined(VRC_LIGHT_VOLUMES_ENABLED)
        // Transform world position to light volume local space
        float3 localPos = mul(_VRCLightVolumeWorldToLocal, float4(worldPos, 1.0)).xyz;
        
        // Sample the 3D light volume texture
        float3 lightColor = tex3D(_VRCLightVolumeTexture, localPos).rgb;
        
        // Apply intensity and tint
        lightColor *= _VRCLightVolumeIntensity;
        lightColor *= _VRCLightVolumeTint.rgb;
        
        // Apply distance-based falloff
        float distance = length(localPos - 0.5);
        float falloff = 1.0 - saturate(distance * _VRCLightVolumeDistanceFactor);
        lightColor *= falloff;
        
        return lightColor;
    #else
        return float3(1.0, 1.0, 1.0); // Default white light
    #endif
}

//----------------------------------------------------------------------------------------------------------------------
// lilToon Integration Functions
//----------------------------------------------------------------------------------------------------------------------
float lilGetPCSSShadow(float4 shadowCoord, float3 worldPos, float3 normal)
{
    #if defined(LIL_FEATURE_SHADOW) && defined(_MAIN_LIGHT_SHADOWS)
        return PCSS(shadowCoord, worldPos.z);
    #else
        return 1.0;
    #endif
}

float lilGetPCSSShadowWithFade(float4 shadowCoord, float3 worldPos, float3 normal, float shadowFade)
{
    float shadow = lilGetPCSSShadow(shadowCoord, worldPos, normal);
    return lerp(shadow, 1.0, shadowFade);
}

//----------------------------------------------------------------------------------------------------------------------
// VRChat Optimization Functions
//----------------------------------------------------------------------------------------------------------------------
float lilGetPCSSShadowOptimized(float4 shadowCoord, float3 worldPos, float3 normal, float quality)
{
    // Adjust sample count based on quality setting
    int sampleCount = (int)lerp(4, PCSS_SAMPLE_COUNT, quality);
    
    // Use simplified PCSS for lower quality
    if (quality < 0.5)
    {
        #if defined(UNITY_VERSION) && UNITY_VERSION >= 202220 && defined(URP_AVAILABLE)
            return SAMPLE_TEXTURE2D(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture, shadowCoord.xy).r;
        #else
            return tex2D(_MainLightShadowmapTexture, shadowCoord.xy).r;
        #endif
    }
    
    return PCSS(shadowCoord, worldPos.z);
}

//----------------------------------------------------------------------------------------------------------------------
// Compatibility Macros
//----------------------------------------------------------------------------------------------------------------------
#define LIL_SHADOW_COORDS(idx) SHADOW_COORDS(idx)
#define LIL_TRANSFER_SHADOW(o, pos) TRANSFER_SHADOW(o)
#define LIL_LIGHT_ATTENUATION(i) LIGHT_ATTENUATION(i)

// PCSS-enabled shadow sampling
#define LIL_SAMPLE_SHADOW(shadowCoord, worldPos, normal) lilGetPCSSShadow(shadowCoord, worldPos, normal)
#define LIL_SAMPLE_SHADOW_FADE(shadowCoord, worldPos, normal, fade) lilGetPCSSShadowWithFade(shadowCoord, worldPos, normal, fade)
#define LIL_SAMPLE_SHADOW_QUALITY(shadowCoord, worldPos, normal, quality) lilGetPCSSShadowOptimized(shadowCoord, worldPos, normal, quality)

//----------------------------------------------------------------------------------------------------------------------
// Debug Visualization
//----------------------------------------------------------------------------------------------------------------------
#ifdef LIL_DEBUG_PCSS
float3 lilDebugPCSS(float4 shadowCoord, float3 worldPos, float3 normal)
{
    float shadow = lilGetPCSSShadow(shadowCoord, worldPos, normal);
    
    // Visualize shadow intensity
    float3 debugColor = lerp(float3(1, 0, 0), float3(0, 1, 0), shadow);
    
    return debugColor;
}
#endif

#endif // LIL_PCSS_SHADOWS_INCLUDED 