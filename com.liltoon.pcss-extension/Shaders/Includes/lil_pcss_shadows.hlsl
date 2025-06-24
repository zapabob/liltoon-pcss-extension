#include "lil_pcss_common.hlsl"

// VRC Light Volumes用の追加シェーダー（存在しない場合はコメントアウト推奨）
// #include "Packages/com.vrchat.base/Runtime/Shader/VRChat_Shadow.cginc"

//----------------------------------------------------------------------------------------------------------------------
// PCSS Configuration
//----------------------------------------------------------------------------------------------------------------------

// シャドウマップテクスチャ参照の安全性確保
#ifndef _ShadowMapTexture
sampler2D _ShadowMapTexture;
#endif

// PCSS関数定義 - シャドウ値を直接受け取る
float PCSS(float shadowValue, float depth, float filterRadius, float lightSize, float samples)
{
    // 既に計算されたシャドウ値を基にPCSSエフェクトを適用
    float shadow = shadowValue;
    
    // ソフトシャドウ効果
    shadow = lerp(shadow, smoothstep(0.0, lightSize, shadow), filterRadius);
    
    // バイアス調整
    shadow = saturate(shadow + 0.001);
    
    return shadow;
}

// モバイル向け軽量版PCSS - シャドウ値を直接受け取る
float PCSSMobile(float shadowValue, float depth)
{
    // 簡易版PCSSフィルタリング
    float shadow = shadowValue;
    shadow = smoothstep(0.0, 0.05, shadow);
    return shadow;
} 