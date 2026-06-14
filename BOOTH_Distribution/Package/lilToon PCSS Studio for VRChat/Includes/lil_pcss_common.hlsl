//----------------------------------------------------------------------------------------------------------------------
// lilToon PCSS Extension - Common Definitions
// Version 2.8.0
//----------------------------------------------------------------------------------------------------------------------

#ifndef LIL_PCSS_COMMON_INCLUDED
#define LIL_PCSS_COMMON_INCLUDED

#if defined(UNITY_PIPELINE_URP)
    #define LIL_PCSS_URP_PIPELINE
#elif defined(UNITY_PIPELINE_HDRP)
    #define LIL_PCSS_HDRP_PIPELINE
#else
    #define LIL_PCSS_BUILTIN_PIPELINE
#endif

#if defined(UNITY_ANDROID) || defined(UNITY_IOS) || defined(SHADER_API_MOBILE)
    #define LIL_PCSS_MOBILE_PLATFORM
#endif

#define LIL_PCSS_VERSION_MAJOR 2
#define LIL_PCSS_VERSION_MINOR 8
#define LIL_PCSS_VERSION_PATCH 0
#define LIL_PCSS_VERSION "2.8.0"
#define LIL_PCSS_LILTOON_VERSION "2.3.2"
#define LIL_PCSS_VRCHAT_SDK_VERSION "3.10.3"
#define LIL_PCSS_VRCLV_VERSION "2.1.3"

#ifdef LIL_PCSS_MOBILE_PLATFORM
    #define LIL_PCSS_DEFAULT_SAMPLE_COUNT 8
    #define LIL_PCSS_DEFAULT_BLOCKER_SAMPLES 8
#else
    #define LIL_PCSS_DEFAULT_SAMPLE_COUNT 16
    #define LIL_PCSS_DEFAULT_BLOCKER_SAMPLES 16
#endif

#if !defined(LIL_PCSS_INTENSITY_DEFINED)
    #define LIL_PCSS_INTENSITY_DEFINED
    float _PCSSIntensity;
#endif

float LIL_PCSS_SafePow(float value, float power)
{
    return pow(max(value, 0.00001), max(power, 0.00001));
}

float3 LIL_PCSS_SafeNormalize(float3 value)
{
    return dot(value, value) > 0.000001 ? normalize(value) : float3(0.0, 1.0, 0.0);
}

uint LIL_PCSS_BitReverse32(uint value)
{
    value = ((value >> 1) & 0x55555555u) | ((value & 0x55555555u) << 1);
    value = ((value >> 2) & 0x33333333u) | ((value & 0x33333333u) << 2);
    value = ((value >> 4) & 0x0F0F0F0Fu) | ((value & 0x0F0F0F0Fu) << 4);
    value = ((value >> 8) & 0x00FF00FFu) | ((value & 0x00FF00FFu) << 8);
    return (value >> 16) | (value << 16);
}

float GetAdjustedSampleCount(float baseSamples, float qualityLevel)
{
    if (qualityLevel < 1.0)
    {
        return max(4.0, baseSamples * 0.5);
    }

    if (qualityLevel > 2.0)
    {
        return min(64.0, baseSamples * 2.0);
    }

    if (qualityLevel > 1.0)
    {
        return min(32.0, baseSamples * 1.5);
    }

    return baseSamples;
}

float GetPlatformOptimizedSamples(float baseSamples)
{
#ifdef LIL_PCSS_MOBILE_PLATFORM
    return min(baseSamples, 8.0);
#else
    return baseSamples;
#endif
}

float CalculateShadowBorder(float shadow, float border, float blur)
{
    shadow = saturate(shadow + border);
    return smoothstep(0.0, max(blur, 0.0001), shadow);
}

float3 CalculateThreeShadowSystem(float shadow1, float shadow2, float shadow3, float3 shadowColors[3])
{
    float3 finalShadow = float3(1.0, 1.0, 1.0);
    finalShadow *= lerp(shadowColors[0], float3(1.0, 1.0, 1.0), shadow1);
    finalShadow *= lerp(shadowColors[1], float3(1.0, 1.0, 1.0), shadow2);
    finalShadow *= lerp(shadowColors[2], float3(1.0, 1.0, 1.0), shadow3);
    return finalShadow;
}

float CalculateSDFFaceShadowValue(float2 uv, sampler2D sdfTexture, float intensity, float softness)
{
    float sdfValue = tex2D(sdfTexture, uv).r;
    float faceShadow = smoothstep(softness, 1.0 - softness, sdfValue);
    return lerp(1.0, faceShadow, intensity);
}

float3 SampleLTCGI(float3 worldPos, float3 worldNormal, float samples, float intensity)
{
    float3 gi = 0.0;
    float totalWeight = 0.0;
    int count = max(1, (int)samples);

    for (int index = 0; index < count; index++)
    {
        float2 hammersley = float2(
            (float)index / (float)count,
            (float)LIL_PCSS_BitReverse32((uint)index) / 4294967295.0
        );

        float phi = 6.2831853 * hammersley.x;
        float cosTheta = 1.0 - 2.0 * hammersley.y;
        float sinTheta = sqrt(saturate(1.0 - cosTheta * cosTheta));
        float3 sampleDir = float3(sinTheta * cos(phi), sinTheta * sin(phi), cosTheta);
        float weight = max(0.0, dot(worldNormal, sampleDir));
        gi += weight * sampleDir;
        totalWeight += weight;
    }

    gi = totalWeight > 0.0 ? gi / totalWeight : float3(1.0, 1.0, 1.0);
    return gi * intensity;
}

float3 CalculateBacklightValue(float3 worldNormal, float3 lightDir, float3 backlightColor, float intensity)
{
    float backlightDot = max(0.0, dot(worldNormal, -lightDir));
    return backlightColor * intensity * backlightDot;
}

float3 SampleVRCLightVolumesEnhanced(
    float3 worldPos,
    float3 worldNormal,
    float3 lightDir,
    sampler3D volumeTexture,
    float4x4 worldToLocal,
    float3 tint,
    float intensity,
    float distanceFactor)
{
    float3 localPos = mul(worldToLocal, float4(worldPos, 1.0)).xyz;
    float3 volumeUV = localPos * 0.5 + 0.5;

    if (any(volumeUV < 0.0) || any(volumeUV > 1.0))
    {
        return float3(1.0, 1.0, 1.0);
    }

    float3 lightColor = tex3D(volumeTexture, volumeUV).rgb;
    float distFactor = 1.0 - saturate(length(localPos) * distanceFactor);
    float normalDotLight = max(0.0, dot(worldNormal, lightDir));
    return lerp(float3(1.0, 1.0, 1.0), lightColor * tint, intensity * distFactor * normalDotLight);
}

float GetDynamicQualityLevel(float baseQuality, float performanceTarget)
{
    return min(baseQuality, performanceTarget);
}

float GetMemoryOptimizedSamples(float baseSamples)
{
#ifdef LIL_PCSS_MOBILE_PLATFORM
    return min(baseSamples, 8.0);
#else
    return baseSamples;
#endif
}

float GetPerformanceComparisonScore(float ourPerformance, float competitorPerformance)
{
    return ourPerformance / max(competitorPerformance, 0.1);
}

#endif
