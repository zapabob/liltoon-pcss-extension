//----------------------------------------------------------------------------------------------------------------------
// lilToon PCSS Extension - PCSS Shadows Include
// Copyright (c) 2025 lilToon PCSS Extension Team
// 
// This file provides PCSS (Percentage-Closer Soft Shadows) functionality for lilToon shaders
// Compatible with lilToon v1.10.3+ and VRChat SDK3
//----------------------------------------------------------------------------------------------------------------------

#ifndef LIL_PCSS_SHADOWS_INCLUDED
#define LIL_PCSS_SHADOWS_INCLUDED

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
// PCSS Properties
//----------------------------------------------------------------------------------------------------------------------
CBUFFER_START(lilPCSSProperties)
    float _PCSSEnabled;
    float _PCSSFilterRadius;
    float _PCSSLightSize;
    float _PCSSBias;
    float _PCSSIntensity;
    float _PCSSQuality;
    float4 _PCSSLightDirection;
    float4x4 _PCSSLightMatrix;
CBUFFER_END

//----------------------------------------------------------------------------------------------------------------------
// Sampling Patterns
//----------------------------------------------------------------------------------------------------------------------
static const float2 poissonDisk[16] = {
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
float random(float2 seed)
{
    return frac(sin(dot(seed, float2(12.9898, 78.233))) * 43758.5453);
}

float2 randomRotation(float2 uv)
{
    float angle = random(uv) * 6.28318530718; // 2 * PI
    float cosAngle = cos(angle);
    float sinAngle = sin(angle);
    return float2(cosAngle, sinAngle);
}

//----------------------------------------------------------------------------------------------------------------------
// PCSS Shadow Sampling
//----------------------------------------------------------------------------------------------------------------------
float PCSSBlockerSearch(float2 shadowCoord, float zReceiver, float searchRadius)
{
    float blockerSum = 0.0;
    float numBlockers = 0.0;
    
    float2 rotation = randomRotation(shadowCoord);
    
    for (int i = 0; i < PCSS_BLOCKER_SEARCH_SAMPLES; ++i)
    {
        float2 offset = poissonDisk[i] * searchRadius;
        
        // Apply rotation
        float2 rotatedOffset = float2(
            offset.x * rotation.x - offset.y * rotation.y,
            offset.x * rotation.y + offset.y * rotation.x
        );
        
        float2 sampleCoord = shadowCoord + rotatedOffset;
        float shadowDepth = SAMPLE_TEXTURE2D(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture, sampleCoord).r;
        
        if (shadowDepth < zReceiver)
        {
            blockerSum += shadowDepth;
            numBlockers += 1.0;
        }
    }
    
    return numBlockers > 0.0 ? blockerSum / numBlockers : -1.0;
}

float PCSSFilter(float2 shadowCoord, float zReceiver, float filterRadius)
{
    float shadow = 0.0;
    float2 rotation = randomRotation(shadowCoord);
    
    for (int i = 0; i < PCSS_SAMPLE_COUNT; ++i)
    {
        float2 offset = poissonDisk[i] * filterRadius;
        
        // Apply rotation
        float2 rotatedOffset = float2(
            offset.x * rotation.x - offset.y * rotation.y,
            offset.x * rotation.y + offset.y * rotation.x
        );
        
        float2 sampleCoord = shadowCoord + rotatedOffset;
        float shadowDepth = SAMPLE_TEXTURE2D(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture, sampleCoord).r;
        
        shadow += (shadowDepth >= zReceiver - _PCSSBias) ? 1.0 : 0.0;
    }
    
    return shadow / PCSS_SAMPLE_COUNT;
}

//----------------------------------------------------------------------------------------------------------------------
// Main PCSS Function
//----------------------------------------------------------------------------------------------------------------------
float SamplePCSSShadow(float4 shadowCoord, float3 worldPos, float3 normal)
{
    // Early exit if PCSS is disabled
    if (_PCSSEnabled < 0.5)
    {
        return SAMPLE_TEXTURE2D(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture, shadowCoord.xy).r;
    }
    
    float3 projCoords = shadowCoord.xyz / shadowCoord.w;
    
    // Check if fragment is in shadow map bounds
    if (projCoords.x < 0.0 || projCoords.x > 1.0 || 
        projCoords.y < 0.0 || projCoords.y > 1.0 || 
        projCoords.z > 1.0)
    {
        return 1.0;
    }
    
    float zReceiver = projCoords.z;
    float2 shadowMapCoord = projCoords.xy;
    
    // Step 1: Blocker search
    float searchRadius = _PCSSLightSize * (zReceiver - 0.1) / zReceiver;
    float avgBlockerDepth = PCSSBlockerSearch(shadowMapCoord, zReceiver, searchRadius);
    
    // No blockers found
    if (avgBlockerDepth < 0.0)
    {
        return 1.0;
    }
    
    // Step 2: Penumbra estimation
    float penumbraRatio = (zReceiver - avgBlockerDepth) / avgBlockerDepth;
    float filterRadius = penumbraRatio * _PCSSLightSize * _PCSSFilterRadius;
    
    // Step 3: Filtering
    float shadow = PCSSFilter(shadowMapCoord, zReceiver, filterRadius);
    
    // Apply intensity and bias
    shadow = lerp(0.0, 1.0, shadow);
    shadow = pow(shadow, _PCSSIntensity);
    
    return shadow;
}

//----------------------------------------------------------------------------------------------------------------------
// lilToon Integration Functions
//----------------------------------------------------------------------------------------------------------------------
float lilGetPCSSShadow(float4 shadowCoord, float3 worldPos, float3 normal)
{
    #if defined(LIL_FEATURE_SHADOW) && defined(_MAIN_LIGHT_SHADOWS)
        return SamplePCSSShadow(shadowCoord, worldPos, normal);
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
        return SAMPLE_TEXTURE2D(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture, shadowCoord.xy).r;
    }
    
    return SamplePCSSShadow(shadowCoord, worldPos, normal);
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