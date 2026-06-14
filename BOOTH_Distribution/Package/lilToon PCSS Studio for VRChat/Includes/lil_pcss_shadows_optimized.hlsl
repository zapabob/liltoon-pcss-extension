// lilToon PCSS Extension - Optimized Shadow Calculation
// Version 2.4.0
// Author: lilToon PCSS Extension Team

#ifndef LIL_PCSS_SHADOWS_OPTIMIZED_INCLUDED
#define LIL_PCSS_SHADOWS_OPTIMIZED_INCLUDED

#include "lil_pcss_common.hlsl"

//------------------------------------------------------------------------------------------------------------------------------
// PCSS - Blocker Search (Optimized)
//------------------------------------------------------------------------------------------------------------------------------
float SearchBlocker(sampler2D shadowMap, float2 uv, float lightSize, float initialDepth)
{
    float blockerDepthSum = 0.0;
    int blockerCount = 0;

    // Optimized sampling pattern (e.g., Rotated Poisson Disk)
    // This reduces the number of required samples while maintaining quality.
    for (int i = 0; i < _PCSS_BLOCKER_SAMPLES; i++)
    {
        float2 offset = POISSON_DISK_SAMPLES[i] * lightSize;
        float shadowMapDepth = tex2D(shadowMap, uv + offset).r;
        if (shadowMapDepth < initialDepth)
        {
            blockerDepthSum += shadowMapDepth;
            blockerCount++;
        }
    }

    if (blockerCount == 0) return 1.0; // No blockers found

    return blockerDepthSum / blockerCount;
}

//------------------------------------------------------------------------------------------------------------------------------
// PCSS - Penumbra Estimation
//------------------------------------------------------------------------------------------------------------------------------
float EstimatePenumbra(float avgBlockerDepth, float initialDepth, float lightSize)
{
    // Penumbra size is proportional to the distance between the receiver and the blocker.
    float penumbra = (initialDepth - avgBlockerDepth) * lightSize / avgBlockerDepth;
    return penumbra;
}

//------------------------------------------------------------------------------------------------------------------------------
// PCSS - Main Function
//------------------------------------------------------------------------------------------------------------------------------
float PCSS_Optimized(sampler2D shadowMap, float4 shadowCoord, float lightSize, float filterRadius)
{
    float2 uv = shadowCoord.xy / shadowCoord.w;
    float initialDepth = shadowCoord.z / shadowCoord.w;

    // Step 1: Blocker Search
    float avgBlockerDepth = SearchBlocker(shadowMap, uv, lightSize, initialDepth);

    // Step 2: Penumbra Estimation
    float penumbra = EstimatePenumbra(avgBlockerDepth, initialDepth, lightSize);

    // Step 3: PCF Filtering
    float finalShadow = 0.0;
    float filterSize = penumbra * filterRadius;

    // Optimized PCF filtering
    for (int i = 0; i < _PCSS_FILTER_SAMPLES; i++)
    {
        float2 offset = POISSON_DISK_SAMPLES_PCF[i] * filterSize;
        finalShadow += tex2D(shadowMap, uv + offset).r < initialDepth ? 0.0 : 1.0;
    }

    return finalShadow / _PCSS_FILTER_SAMPLES;
}

float PCSS_Optimized(float shadowValue, float depth, float filterRadius, float lightSize, float samples)
{
    float optimizedSamples = GetPlatformOptimizedSamples(samples);
    float softness = saturate(filterRadius * lightSize * max(optimizedSamples, 1.0));
    float shadow = smoothstep(0.0, max(softness, 0.0001), shadowValue);
    float depthBias = saturate(depth * 0.0005);
    return saturate(shadow + depthBias);
}

#endif // LIL_PCSS_SHADOWS_OPTIMIZED_INCLUDED
