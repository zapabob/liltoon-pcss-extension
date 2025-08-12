// Competitive Features for lilToon PCSS Extension
// nHaruka PCSS for VRC シェア奪取のための高度なシェーダー機能

#ifndef LIL_COMPETITIVE_FEATURES_INCLUDED
#define LIL_COMPETITIVE_FEATURES_INCLUDED

// ========================================
// lilToon 2.1.4 最新機能対応
// ========================================

// ピクセル単位計算機能
#define LIL_FEATURE_PERPIXEL_CALCULATION
#ifdef LIL_FEATURE_PERPIXEL_CALCULATION
    float4 _PerPixelCalculationParams;
    #define PERPIXEL_CALCULATION_ENABLED 1
#else
    #define PERPIXEL_CALCULATION_ENABLED 0
#endif

// 方向性を考慮したライティング
#define LIL_FEATURE_DIRECTION_AWARE_LIGHTING
#ifdef LIL_FEATURE_DIRECTION_AWARE_LIGHTING
    float4 _DirectionAwareLightingParams;
    #define DIRECTION_AWARE_LIGHTING_ENABLED 1
#else
    #define DIRECTION_AWARE_LIGHTING_ENABLED 0
#endif

// 強化されたリムライト制御
#define LIL_FEATURE_ENHANCED_RIM_LIGHT
#ifdef LIL_FEATURE_ENHANCED_RIM_LIGHT
    float4 _EnhancedRimLightParams;
    #define ENHANCED_RIM_LIGHT_ENABLED 1
#else
    #define ENHANCED_RIM_LIGHT_ENABLED 0
#endif

// ========================================
// VRC Light Volumes 2.0.0 対応
// ========================================

#define LIL_FEATURE_VRC_LIGHT_VOLUMES_2_0_0
#ifdef LIL_FEATURE_VRC_LIGHT_VOLUMES_2_0_0
    // VRC Light Volumes 2.0.0 新プロパティ
    float _EnvRimBorder;
    float _EnvRimBlur;
    float4 _VRCLightVolumes2Params;
    #define VRC_LIGHT_VOLUMES_2_0_0_ENABLED 1
#else
    #define VRC_LIGHT_VOLUMES_2_0_0_ENABLED 0
#endif

// ========================================
// Quest版対応機能
// ========================================

#define LIL_FEATURE_MOBILE_OPTIMIZATION
#ifdef LIL_FEATURE_MOBILE_OPTIMIZATION
    float _PerformanceLevel;
    float4 _MobileOptimizationParams;
    #define MOBILE_OPTIMIZATION_ENABLED 1
#else
    #define MOBILE_OPTIMIZATION_ENABLED 0
#endif

// ========================================
// AMD GPU最適化機能
// ========================================

#define LIL_FEATURE_AMD_GPU_OPTIMIZATION
#ifdef LIL_FEATURE_AMD_GPU_OPTIMIZATION
    float _AMDGPUSpecific;
    float4 _AMDGPUOptimizationParams;
    #define AMD_GPU_OPTIMIZATION_ENABLED 1
#else
    #define AMD_GPU_OPTIMIZATION_ENABLED 0
#endif

// ========================================
// NVIDIA GPU最適化機能（新機能）
// ========================================

#define LIL_FEATURE_NVIDIA_GPU_OPTIMIZATION
#ifdef LIL_FEATURE_NVIDIA_GPU_OPTIMIZATION
    float _NVIDIAGPUSpecific;
    float4 _NVIDIAGPUOptimizationParams;
    #define NVIDIA_GPU_OPTIMIZATION_ENABLED 1
#else
    #define NVIDIA_GPU_OPTIMIZATION_ENABLED 0
#endif

// ========================================
// Quest最適化機能（新機能）
// ========================================

#define LIL_FEATURE_QUEST_OPTIMIZATION
#ifdef LIL_FEATURE_QUEST_OPTIMIZATION
    float _QuestSpecific;
    float4 _QuestOptimizationParams;
    #define QUEST_OPTIMIZATION_ENABLED 1
#else
    #define QUEST_OPTIMIZATION_ENABLED 0
#endif

// ========================================
// リアルタイム最適化機能（新機能）
// ========================================

#define LIL_FEATURE_REALTIME_OPTIMIZATION
#ifdef LIL_FEATURE_REALTIME_OPTIMIZATION
    float _DynamicQualityLevel;
    float4 _RealtimeOptimizationParams;
    #define REALTIME_OPTIMIZATION_ENABLED 1
#else
    #define REALTIME_OPTIMIZATION_ENABLED 0
#endif

// ========================================
// 高度なマスクシステム
// ========================================

#define LIL_FEATURE_GRADIENT_MASK
#ifdef LIL_FEATURE_GRADIENT_MASK
    sampler2D _GradientMask;
    float4 _GradientMaskParams;
    #define GRADIENT_MASK_ENABLED 1
#else
    #define GRADIENT_MASK_ENABLED 0
#endif

#define LIL_FEATURE_SPECIAL_PART_MASK
#ifdef LIL_FEATURE_SPECIAL_PART_MASK
    sampler2D _SpecialPartMask;
    float4 _SpecialPartMaskParams;
    #define SPECIAL_PART_MASK_ENABLED 1
#else
    #define SPECIAL_PART_MASK_ENABLED 0
#endif

#define LIL_FEATURE_AUTO_MASK_GENERATION
#ifdef LIL_FEATURE_AUTO_MASK_GENERATION
    float _AutoMaskGeneration;
    float4 _AutoMaskGenerationParams;
    #define AUTO_MASK_GENERATION_ENABLED 1
#else
    #define AUTO_MASK_GENERATION_ENABLED 0
#endif

// ========================================
// パフォーマンス最適化関数群
// ========================================

// 動的品質調整関数
float GetDynamicQualityLevel(float baseQuality, float fpsRatio)
{
    #if REALTIME_OPTIMIZATION_ENABLED
        return lerp(baseQuality * 0.5, baseQuality, fpsRatio);
    #else
        return baseQuality;
    #endif
}

// AMD GPU最適化関数
float4 ApplyAMDGPUOptimization(float4 color, float3 worldPos, float3 worldNormal)
{
    #if AMD_GPU_OPTIMIZATION_ENABLED
        // AMD GPU専用の最適化処理
        float amdOptimization = _AMDGPUSpecific;
        float3 optimizedNormal = normalize(worldNormal + amdOptimization * 0.1);
        return color * (1.0 + amdOptimization * 0.05);
    #else
        return color;
    #endif
}

// NVIDIA GPU最適化関数
float4 ApplyNVIDIAGPUOptimization(float4 color, float3 worldPos, float3 worldNormal)
{
    #if NVIDIA_GPU_OPTIMIZATION_ENABLED
        // NVIDIA GPU専用の最適化処理
        float nvidiaOptimization = _NVIDIAGPUSpecific;
        float3 optimizedNormal = normalize(worldNormal + nvidiaOptimization * 0.1);
        return color * (1.0 + nvidiaOptimization * 0.05);
    #else
        return color;
    #endif
}

// Quest最適化関数
float4 ApplyQuestOptimization(float4 color, float3 worldPos, float3 worldNormal)
{
    #if QUEST_OPTIMIZATION_ENABLED
        // Quest専用の最適化処理
        float questOptimization = _QuestSpecific;
        float3 optimizedNormal = normalize(worldNormal + questOptimization * 0.1);
        return color * (1.0 + questOptimization * 0.05);
    #else
        return color;
    #endif
}

// モバイル最適化関数
float4 ApplyMobileOptimization(float4 color, float3 worldPos, float3 worldNormal)
{
    #if MOBILE_OPTIMIZATION_ENABLED
        // モバイル専用の最適化処理
        float mobileOptimization = _PerformanceLevel;
        float3 optimizedNormal = normalize(worldNormal + mobileOptimization * 0.1);
        return color * (1.0 + mobileOptimization * 0.05);
    #else
        return color;
    #endif
}

// ========================================
// 高度なマスク処理関数群
// ========================================

// グラデーションマスク処理
float GetGradientMask(float2 uv)
{
    #if GRADIENT_MASK_ENABLED
        float4 gradientMask = tex2D(_GradientMask, uv);
        return gradientMask.r * _GradientMaskParams.x + 
               gradientMask.g * _GradientMaskParams.y + 
               gradientMask.b * _GradientMaskParams.z + 
               gradientMask.a * _GradientMaskParams.w;
    #else
        return 1.0;
    #endif
}

// 特殊部位マスク処理
float GetSpecialPartMask(float2 uv)
{
    #if SPECIAL_PART_MASK_ENABLED
        float4 specialMask = tex2D(_SpecialPartMask, uv);
        return specialMask.r * _SpecialPartMaskParams.x + 
               specialMask.g * _SpecialPartMaskParams.y + 
               specialMask.b * _SpecialPartMaskParams.z + 
               specialMask.a * _SpecialPartMaskParams.w;
    #else
        return 1.0;
    #endif
}

// 自動マスク生成処理
float GetAutoGeneratedMask(float2 uv, float3 worldPos, float3 worldNormal)
{
    #if AUTO_MASK_GENERATION_ENABLED
        float autoMask = _AutoMaskGeneration;
        float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);
        float fresnel = 1.0 - saturate(dot(viewDir, worldNormal));
        return lerp(1.0, fresnel, autoMask);
    #else
        return 1.0;
    #endif
}

// ========================================
// VRC Light Volumes 2.0.0 処理関数群
// ========================================

// VRC Light Volumes 2.0.0 リムライト処理
float4 ApplyVRCLightVolumesRimLight(float4 color, float3 worldPos, float3 worldNormal, float3 viewDir)
{
    #if VRC_LIGHT_VOLUMES_2_0_0_ENABLED
        float rimDot = 1.0 - saturate(dot(viewDir, worldNormal));
        float rimBorder = _EnvRimBorder;
        float rimBlur = _EnvRimBlur;
        
        float rimIntensity = smoothstep(rimBorder - rimBlur, rimBorder + rimBlur, rimDot);
        
        return color + rimIntensity * _VRCLightVolumes2Params.rgb * _VRCLightVolumes2Params.a;
    #else
        return color;
    #endif
}

// ========================================
// lilToon 2.1.4 最新機能処理関数群
// ========================================

// ピクセル単位計算処理
float4 ApplyPerPixelCalculation(float4 color, float3 worldPos, float3 worldNormal)
{
    #if PERPIXEL_CALCULATION_ENABLED
        float3 pixelOffset = _PerPixelCalculationParams.xyz;
        float pixelIntensity = _PerPixelCalculationParams.w;
        
        float3 adjustedPos = worldPos + pixelOffset;
        float3 adjustedNormal = normalize(worldNormal + pixelOffset * 0.1);
        
        return color * (1.0 + pixelIntensity * 0.1);
    #else
        return color;
    #endif
}

// 方向性を考慮したライティング処理
float4 ApplyDirectionAwareLighting(float4 color, float3 worldPos, float3 worldNormal, float3 lightDir)
{
    #if DIRECTION_AWARE_LIGHTING_ENABLED
        float3 directionParams = _DirectionAwareLightingParams.xyz;
        float directionIntensity = _DirectionAwareLightingParams.w;
        
        float3 adjustedLightDir = normalize(lightDir + directionParams);
        float lightingDot = saturate(dot(worldNormal, adjustedLightDir));
        
        return color * (1.0 + lightingDot * directionIntensity);
    #else
        return color;
    #endif
}

// 強化されたリムライト制御処理
float4 ApplyEnhancedRimLight(float4 color, float3 worldPos, float3 worldNormal, float3 viewDir)
{
    #if ENHANCED_RIM_LIGHT_ENABLED
        float3 rimParams = _EnhancedRimLightParams.xyz;
        float rimIntensity = _EnhancedRimLightParams.w;
        
        float rimDot = 1.0 - saturate(dot(viewDir, worldNormal));
        float enhancedRim = pow(rimDot, rimParams.x) * rimParams.y + rimParams.z;
        
        return color + enhancedRim * rimIntensity;
    #else
        return color;
    #endif
}

// ========================================
// 統合最適化関数
// ========================================

// 全最適化を統合適用する関数
float4 ApplyAllOptimizations(float4 color, float3 worldPos, float3 worldNormal, float3 viewDir, float3 lightDir, float2 uv)
{
    // 基本色に最適化を適用
    float4 optimizedColor = color;
    
    // ハードウェア最適化
    optimizedColor = ApplyAMDGPUOptimization(optimizedColor, worldPos, worldNormal);
    optimizedColor = ApplyNVIDIAGPUOptimization(optimizedColor, worldPos, worldNormal);
    optimizedColor = ApplyQuestOptimization(optimizedColor, worldPos, worldNormal);
    optimizedColor = ApplyMobileOptimization(optimizedColor, worldPos, worldNormal);
    
    // lilToon 2.1.4 最新機能
    optimizedColor = ApplyPerPixelCalculation(optimizedColor, worldPos, worldNormal);
    optimizedColor = ApplyDirectionAwareLighting(optimizedColor, worldPos, worldNormal, lightDir);
    optimizedColor = ApplyEnhancedRimLight(optimizedColor, worldPos, worldNormal, viewDir);
    
    // VRC Light Volumes 2.0.0
    optimizedColor = ApplyVRCLightVolumesRimLight(optimizedColor, worldPos, worldNormal, viewDir);
    
    // マスク処理
    float gradientMask = GetGradientMask(uv);
    float specialMask = GetSpecialPartMask(uv);
    float autoMask = GetAutoGeneratedMask(uv, worldPos, worldNormal);
    
    float finalMask = gradientMask * specialMask * autoMask;
    
    return optimizedColor * finalMask;
}

// ========================================
// パフォーマンス監視関数
// ========================================

// パフォーマンスメトリクス計算
float CalculatePerformanceMetrics(float3 worldPos, float3 worldNormal)
{
    float complexity = length(worldPos) + length(worldNormal);
    float performanceScore = 1.0 / (1.0 + complexity * 0.01);
    
    #if REALTIME_OPTIMIZATION_ENABLED
        performanceScore *= _DynamicQualityLevel;
    #endif
    
    return performanceScore;
}

#endif // LIL_COMPETITIVE_FEATURES_INCLUDED 