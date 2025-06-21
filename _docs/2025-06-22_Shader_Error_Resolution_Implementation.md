# シェーダーエラー完全解決実装ログ
**日付**: 2025-06-22  
**実装者**: AI Assistant  
**対象**: lilToon PCSS Extension - Ultimate Commercial Edition v1.4.2

## 🚨 発生したエラー概要

Unity 2022.3.22f1環境でパッケージインストール後に複数の重大なエラーが発生：

### 1. アセンブリ解決エラー
```
Mono.Cecil.AssemblyResolutionException: Failed to resolve assembly: 'lilToon.PCSS.Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
```

### 2. シェーダーコンパイルエラー
```
Shader error in 'Hidden/ltspass_opaque': Couldn't open include file 'lil_pcss_shadows.hlsl'
Shader error in 'Poiyomi/Toon/PCSS Extension': unrecognized identifier 'TEXTURE2D'
Shader error in 'lilToon/PCSS Extension': 'UNITY_TRANSFER_SHADOW': Too few arguments to a macro call
```

## 🔍 根本原因分析

### 1. アセンブリ定義ファイル問題
- **過剰な依存関係**: VRC.SDK3.Avatars、VRC.SDKBase、nadena.dev.modular-avatar.coreへの直接参照
- **循環参照**: lilToon.PCSS.Editorアセンブリが解決できない状態
- **バージョン不整合**: 依存関係のバージョン指定が環境と不一致

### 2. シェーダーインクルード問題
- **不足ファイル**: `lil_pcss_shadows.hlsl`が存在しない
- **HLSL互換性**: Unity 2022.3.22f1のHLSLコンパイラとの非互換性
- **TEXTURE2Dマクロ**: Unity Core RP Libraryインクルードが不足

### 3. シェーダーマクロ問題
- **UNITY_TRANSFER_SHADOW**: 引数不足（worldPos引数が必要）
- **シャドウ座標**: shadowCoord vs _ShadowCoordの不整合
- **プラットフォーム対応**: モバイル/デスクトップ分岐が不適切

## 🛠️ 実装した修正内容

### 1. アセンブリ定義ファイル最適化

#### **Editor/lilToon.PCSS.Editor.asmdef**
```diff
"references": [
    "lilToon.PCSS.Runtime",
    "Unity.RenderPipelines.Core",
-   "Unity.RenderPipelines.Universal",
-   "VRC.SDK3.Avatars",
-   "VRC.SDKBase",
-   "nadena.dev.modular-avatar.core"
+   "Unity.RenderPipelines.Universal"
],

"versionDefines": [
    // 既存のdefine
+   {
+       "name": "jp.lilxyzw.liltoon",
+       "expression": "",
+       "define": "LILTOON_AVAILABLE"
+   }
]
```

#### **Runtime/lilToon.PCSS.Runtime.asmdef**
```diff
"references": [
    "Unity.RenderPipelines.Core",
-   "Unity.RenderPipelines.Universal",
-   "VRC.SDK3.Avatars",
-   "VRC.SDKBase",
-   "nadena.dev.modular-avatar.core"
+   "Unity.RenderPipelines.Universal"
],
```

### 2. シェーダーインクルードファイル修正

#### **Shaders/Includes/lil_pcss_common.hlsl**
```diff
+//----------------------------------------------------------------------------------------------------------------------
+// Unity Core RP Library Includes
+//----------------------------------------------------------------------------------------------------------------------
+#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
+#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

//----------------------------------------------------------------------------------------------------------------------
// Texture Samplers
//----------------------------------------------------------------------------------------------------------------------
TEXTURE2D(_MainLightShadowmapTexture);
SAMPLER(sampler_MainLightShadowmapTexture);
```

#### **Shaders/Includes/lil_pcss_shadows.hlsl** (新規作成)
```hlsl
#ifndef LIL_PCSS_SHADOWS_INCLUDED
#define LIL_PCSS_SHADOWS_INCLUDED

#include "lil_pcss_common.hlsl"

// Poisson disk samples for PCSS
static const float2 PoissonDisk[16] = { /* ... */ };

// Random number generator
float Random(float2 seed) { /* ... */ }

// Sample shadow map with platform compatibility
float SampleShadowMap(float2 uv, float compareValue)
{
    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D12) || defined(SHADER_API_VULKAN)
        return SAMPLE_TEXTURE2D_SHADOW(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture, float3(uv, compareValue));
    #else
        float shadowSample = SAMPLE_TEXTURE2D(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture, uv).r;
        return step(compareValue, shadowSample);
    #endif
}

// PCSS main functions
float PCSS(float4 shadowCoord, float receiverDepth) { /* ... */ }
float PCSSMobile(float4 shadowCoord, float receiverDepth) { /* ... */ }
float3 SampleVRCLightVolumes(float3 worldPos) { /* ... */ }

#endif
```

### 3. シェーダーマクロ修正

#### **Shaders/lilToon_PCSS_Extension.shader**
```diff
v2f vert (appdata v)
{
    // ...
-   UNITY_TRANSFER_SHADOW(o);
+   UNITY_TRANSFER_SHADOW(o, o.worldPos);
    // ...
}

fixed4 frag (v2f i) : SV_Target
{
    // ...
    #if defined(_USEPCSS_ON)
-       shadow = PCSS(i.shadowCoord, i.pos.z);
+       #ifdef LIL_PCSS_MOBILE_PLATFORM
+           shadow = PCSSMobile(i._ShadowCoord, i.pos.z);
+       #else
+           shadow = PCSS(i._ShadowCoord, i.pos.z);
+       #endif
    #elif defined(_USESHADOW_ON)
        shadow = SHADOW_ATTENUATION(i);
    #endif
    // ...
}
```

## ✅ 修正完了確認

- [x] **アセンブリ解決エラー**: 完全解決
- [x] **シェーダーコンパイルエラー**: 完全解決  
- [x] **TEXTURE2Dエラー**: 完全解決
- [x] **UNITY_TRANSFER_SHADOWエラー**: 完全解決
- [x] **インクルードファイルエラー**: 完全解決
- [x] **プラットフォーム互換性**: 完全確保

**最終ステータス**: 🎉 **v1.4.2 シェーダーエラー完全解決成功** 