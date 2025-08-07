Shader "lilToon/PCSS Extension"
{
    Properties
    {
        // --- lilToon 2.1.7標準プロパティ ---
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        
        // --- 3影システム (lilToon 2.1.7新機能) ---
        [lilToggle] _UseShadow ("Use Shadow", Float) = 1
        _ShadowColorTex ("Shadow Color", 2D) = "black" {}
        _ShadowBorder ("Shadow Border", Range(0, 1)) = 0.5
        _ShadowBlur ("Shadow Blur", Range(0, 1)) = 0.1
        
        // --- 第2影 (lilToon 2.1.7) ---
        [lilToggle] _UseShadow2 ("Use 2nd Shadow", Float) = 0
        _Shadow2ColorTex ("2nd Shadow Color", 2D) = "black" {}
        _Shadow2Border ("2nd Shadow Border", Range(0, 1)) = 0.5
        _Shadow2Blur ("2nd Shadow Blur", Range(0, 1)) = 0.1
        
        // --- 第3影 (lilToon 2.1.7新機能) ---
        [lilToggle] _UseShadow3 ("Use 3rd Shadow", Float) = 0
        _Shadow3ColorTex ("3rd Shadow Color", 2D) = "black" {}
        _Shadow3Border ("3rd Shadow Border", Range(0, 1)) = 0.5
        _Shadow3Blur ("3rd Shadow Blur", Range(0, 1)) = 0.1
        
        // --- SDF Face Shadow (lilToon 2.1.7新機能) ---
        [lilToggle] _UseSDFFaceShadow ("Use SDF Face Shadow", Float) = 0
        _SDFFaceShadowTex ("SDF Face Shadow Texture", 2D) = "white" {}
        _SDFFaceShadowIntensity ("SDF Face Shadow Intensity", Range(0.0, 1.0)) = 0.5
        _SDFFaceShadowSoftness ("SDF Face Shadow Softness", Range(0.0, 1.0)) = 0.1
        
        // --- LTCGI (Linearly Transformed Cosines Global Illumination) ---
        [lilToggle] _UseLTCGI ("Use LTCGI", Float) = 0
        _LTCGIIntensity ("LTCGI Intensity", Range(0.0, 2.0)) = 1.0
        _LTCGISamples ("LTCGI Samples", Range(1, 64)) = 16
        
        // --- Backlight & Light Direction Override ---
        [lilToggle] _UseBacklight ("Use Backlight", Float) = 0
        _BacklightColor ("Backlight Color", Color) = (1,1,1,1)
        _BacklightIntensity ("Backlight Intensity", Range(0.0, 2.0)) = 1.0
        [lilToggle] _UseLightDirectionOverride ("Use Light Direction Override", Float) = 0
        _LightDirectionOverride ("Light Direction Override", Vector) = (0,1,0,0)
        
        // --- PCSS拡張プロパティ ---
        [lilToggle] _UsePCSS ("Use PCSS", Float) = 1
        [Enum(Realistic,0,Anime,1,Cinematic,2,Custom,3)] _PCSSPresetMode ("PCSS Preset", Float) = 1
        _LocalPCSSFilterRadius ("PCSS Filter Radius", Range(0.001, 0.1)) = 0.01
        _LocalPCSSLightSize ("PCSS Light Size", Range(0.01, 0.5)) = 0.1
        _LocalPCSSBias ("PCSS Bias", Range(0.0001, 0.01)) = 0.001
        _PCSSIntensity ("PCSS Intensity", Range(0.0, 2.0)) = 1.0
        [Enum(Low,0,Medium,1,High,2,Ultra,3)] _PCSSQualityLevel ("PCSS Quality", Float) = 1
        _LocalPCSSSamples ("PCSS Samples", Range(1, 64)) = 16
        [lilToggle] _UseShadowMask ("Use Shadow Mask", Float) = 0
        _ShadowMaskTex ("Shadow Mask (R:Cast, G:Receive)", 2D) = "white" {}
        _ShadowMaskStrength ("Shadow Mask Strength", Range(0.0, 1.0)) = 1.0
        
        // --- その他（省略可） ---
        [lilToggle] _UseShadowClamp ("Use Shadow Clamp (Anime Style)", Float) = 0
        _ShadowClamp ("Shadow Clamp", Range(0, 1)) = 0.5
        _Translucency ("Translucency", Range(0, 1)) = 0.5
        
        // --- VRC Light Volumes 2.0.0 強化版 ---
        [lilToggle] _UseVRCLightVolumes ("Use VRC Light Volumes", Float) = 0
        _VRCLightVolumeIntensity ("VRC Light Volume Intensity", Range(0.0, 2.0)) = 1.0
        _VRCLightVolumeTint ("VRC Light Volume Tint", Color) = (1,1,1,1)
        _VRCLightVolumeDistanceFactor ("VRC Light Volume Distance Factor", Range(0.0, 1.0)) = 0.1
        _EnvRimBorder ("[VRCLV] Rim Border", Range(0, 1)) = 0.85
        _EnvRimBlur ("[VRCLV] Rim Blur", Range(0, 1)) = 0.35
        [lilToggle] _UseVRCLVRimLight ("Use VRC LV Rim Light", Float) = 0
        _VRCLVRimLightIntensity ("VRC LV Rim Light Intensity", Range(0.0, 2.0)) = 1.0
        _VRCLVRimLightColor ("VRC LV Rim Light Color", Color) = (1,1,1,1)
        
        // --- Flipbook ---
        [lilToggle] _UseFlipbook ("Use Flipbook", Float) = 0
        _FlipbookTex ("Flipbook Texture", 2D) = "white" {}
        _FlipbookDivisionsX ("Flipbook Divisions X", Float) = 4
        _FlipbookDivisionsY ("Flipbook Divisions Y", Float) = 4
        _FlipbookSpeed ("Flipbook Speed", Float) = 10

        // --- Rendering ---
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 2
        [Enum(Off,0,On,1)] _ZWrite ("ZWrite", Float) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
        // --- Stencil ---
        _StencilRef ("Stencil Reference", Range(0, 255)) = 0
        _StencilReadMask ("Stencil Read Mask", Range(0, 255)) = 255
        _StencilWriteMask ("Stencil Write Mask", Range(0, 255)) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Compare", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilPass ("Stencil Pass", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilFail ("Stencil Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilZFail ("Stencil ZFail", Float) = 0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "Queue"="Geometry"
            "LightMode"="ForwardBase"
        }
        LOD 200

        Stencil
        {
            Ref [_StencilRef]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
            Comp [_StencilComp]
            Pass [_StencilPass]
            Fail [_StencilFail]
            ZFail [_StencilZFail]
        }

        Pass
        {
            Name "FORWARD"
            Tags {"LightMode" = "ForwardBase"}
            Cull [_Cull]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Blend [_SrcBlend] [_DstBlend]

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma shader_feature_local _ _USEPCSS_ON
            #pragma shader_feature_local _ _USESHADOW_ON
            #pragma shader_feature_local _ _USESHADOW2_ON
            #pragma shader_feature_local _ _USESHADOW3_ON
            #pragma shader_feature_local _ _USESDFFACESHADOW_ON
            #pragma shader_feature_local _ _USELTCGI_ON
            #pragma shader_feature_local _ _USEBACKLIGHT_ON
            #pragma shader_feature_local _ _USELIGHTDIRECTIONOVERRIDE_ON
            #pragma shader_feature_local _ _USEVRCLIGHT_VOLUMES_ON
            #pragma shader_feature_local _ _USEVRCLV_RIMLIGHT_ON
            #pragma shader_feature_local _ _USESHADOWCLAMP_ON
            #pragma shader_feature_local _ _USE_OPTIMIZED_PCSS_ON
            #pragma shader_feature_local _ _USESHADOWMASK_ON
            #pragma shader_feature_local _ _USEFLIPBOOK_ON
            #pragma multi_compile _ VRC_LIGHT_VOLUMES_ENABLED
            #pragma multi_compile _ VRC_LIGHT_VOLUMES_MOBILE

            // --- インクルード順序を厳守 ---
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #define LIL_LILTOON_SHADER_INCLUDED
            
            // テクスチャ存在確認用の定義
            #ifndef _MAINTEX
                #define _MAINTEX
            #endif
            
            #ifndef _SHADOWCOLORTEX
                #define _SHADOWCOLORTEX
            #endif
            
            #include "Includes/lil_pcss_common.hlsl"
            #if defined(_USE_OPTIMIZED_PCSS_ON)
                #include "Includes/lil_pcss_shadows_optimized.hlsl"
            #else
                #include "Includes/lil_pcss_shadows.hlsl"
            #endif

            // --- プロパティ宣言 ---
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            sampler2D _ShadowColorTex;
            sampler2D _Shadow2ColorTex;
            sampler2D _Shadow3ColorTex;
            sampler2D _SDFFaceShadowTex;
            float _UsePCSS;
            float _UseVRCLightVolumes;
            float _VRCLightVolumeIntensity;
            float4 _VRCLightVolumeTint;
            float _VRCLightVolumeDistanceFactor;
            sampler3D _VRCLightVolumeTexture;
            float4 _VRCLightVolumeParams;
            float4x4 _VRCLightVolumeWorldToLocal;
            float _UseShadow;
            float _ShadowBorder;
            float _ShadowBlur;
            float _UseShadow2;
            float _Shadow2Border;
            float _Shadow2Blur;
            float _UseShadow3;
            float _Shadow3Border;
            float _Shadow3Blur;
            float _UseSDFFaceShadow;
            float _SDFFaceShadowIntensity;
            float _SDFFaceShadowSoftness;
            float _UseLTCGI;
            float _LTCGIIntensity;
            float _LTCGISamples;
            float _UseBacklight;
            float4 _BacklightColor;
            float _BacklightIntensity;
            float _UseLightDirectionOverride;
            float4 _LightDirectionOverride;
            float _Cutoff;
            float _UseShadowClamp;
            float _ShadowClamp;
            float _Translucency;
            // PCSS変数の重複を避けるためにローカル変数として定義
            float _LocalPCSSFilterRadius;
            float _LocalPCSSLightSize;
            float _LocalPCSSSamples;
            float _LocalPCSSBias;
            // _PCSSIntensityはすでにIncludesで定義されているため、ここでは再定義しない
            float _PCSSQualityLevel; // _PCSSQualityから_PCSSQualityLevelに変更
            sampler2D _ShadowMaskTex;
            float _ShadowMaskStrength;

            // Flipbook
            sampler2D _FlipbookTex;
            float _FlipbookDivisionsX;
            float _FlipbookDivisionsY;
            float _FlipbookSpeed;

            // VRC Light Volumes 2.0.0 強化版関数
            #if defined(VRC_LIGHT_VOLUMES_ENABLED)
            float3 SampleVRCLightVolumes(float3 worldPos, float3 worldNormal)
            {
                #if defined(VRC_LIGHT_VOLUMES_MOBILE)
                    // モバイル向け簡易版
                    float3 localPos = mul(_VRCLightVolumeWorldToLocal, float4(worldPos, 1.0)).xyz;
                    float3 volumeUV = localPos * 0.5 + 0.5;
                    float3 lightColor = tex3D(_VRCLightVolumeTexture, volumeUV).rgb;
                    return lightColor * _VRCLightVolumeTint.rgb * _VRCLightVolumeIntensity;
                #else
                    // フル機能版 - ピクセル単位計算と方向性考慮
                    float3 localPos = mul(_VRCLightVolumeWorldToLocal, float4(worldPos, 1.0)).xyz;
                    float3 volumeUV = localPos * 0.5 + 0.5;
                    // ボリュームの範囲外なら影響なし
                    if (volumeUV.x < 0.0 || volumeUV.x > 1.0 || volumeUV.y < 0.0 || volumeUV.y > 1.0 || volumeUV.z < 0.0 || volumeUV.z > 1.0)
                        return float3(1.0, 1.0, 1.0);
                    float3 lightColor = tex3D(_VRCLightVolumeTexture, volumeUV).rgb;
                    // 距離による減衰
                    float distFactor = 1.0 - saturate(length(localPos) * _VRCLightVolumeDistanceFactor);
                    // 法線方向を考慮した照明計算
                    float3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                    float normalDotLight = max(0.0, dot(worldNormal, worldLightDir));
                    return lerp(float3(1.0, 1.0, 1.0), lightColor * _VRCLightVolumeTint.rgb, _VRCLightVolumeIntensity * distFactor * normalDotLight);
                #endif
            }
            #else
            float3 SampleVRCLightVolumes(float3 worldPos, float3 worldNormal)
            {
                return float3(1.0, 1.0, 1.0);
            }
            #endif

            // SDF Face Shadow関数 (lilToon 2.1.7新機能)
            float CalculateSDFFaceShadow(float2 uv, float3 worldNormal)
            {
                #if defined(_USESDFFACESHADOW_ON)
                    float sdfValue = tex2D(_SDFFaceShadowTex, uv).r;
                    float faceShadow = smoothstep(_SDFFaceShadowSoftness, 1.0 - _SDFFaceShadowSoftness, sdfValue);
                    return lerp(1.0, faceShadow, _SDFFaceShadowIntensity);
                #else
                    return 1.0;
                #endif
            }

            // LTCGI関数 (Linearly Transformed Cosines Global Illumination)
            float3 CalculateLTCGI(float3 worldPos, float3 worldNormal)
            {
                #if defined(_USELTCGI_ON)
                    // 簡易版LTCGI実装
                    float3 gi = 0;
                    for (int i = 0; i < _LTCGISamples; i++)
                    {
                        float3 sampleDir = normalize(float3(
                            sin(i * 2.39996323) * cos(i * 1.57079633),
                            cos(i * 2.39996323),
                            sin(i * 1.57079633)
                        ));
                        float weight = max(0.0, dot(worldNormal, sampleDir));
                        gi += weight * sampleDir;
                    }
                    gi /= _LTCGISamples;
                    return gi * _LTCGIIntensity;
                #else
                    return float3(1.0, 1.0, 1.0);
                #endif
            }

            // Backlight関数
            float3 CalculateBacklight(float3 worldNormal, float3 worldLightDir)
            {
                #if defined(_USEBACKLIGHT_ON)
                    float backlightDot = max(0.0, dot(worldNormal, -worldLightDir));
                    return _BacklightColor.rgb * _BacklightIntensity * backlightDot;
                #else
                    return float3(0.0, 0.0, 0.0);
                #endif
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                SHADOW_COORDS(3)
                UNITY_FOG_COORDS(4)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                UNITY_TRANSFER_SHADOW(o, v.uv);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // --- ピンク化防止: nullチェックとデフォルト値 ---
                fixed4 col = fixed4(1,1,1,1);
                
                // テクスチャのnullチェックを改善
                #if defined(_MAINTEX)
                    col = tex2D(_MainTex, i.uv) * _Color;
                #else
                    col = _Color;
                #endif
                
                // 照明方向の取得
                float3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                #if defined(_USELIGHTDIRECTIONOVERRIDE_ON)
                    worldLightDir = normalize(_LightDirectionOverride.xyz);
                #endif
                
                // 3影システム (lilToon 2.1.7)
                float shadow1 = 1.0;
                float shadow2 = 1.0;
                float shadow3 = 1.0;
                
                #if defined(_USEPCSS_ON)
                    // PCSSを使用 - 標準のシャドウ座標を使用
                    #ifdef LIL_PCSS_MOBILE_PLATFORM
                        // モバイル向け簡易版
                        shadow1 = SHADOW_ATTENUATION(i);
                        shadow1 = PCSSMobile(shadow1, i.pos.z);
                    #else
                        // フル機能版
                        shadow1 = SHADOW_ATTENUATION(i);
                        float samples = _LocalPCSSSamples;
                        float quality = _PCSSQualityLevel;
                        if (quality < 1.0f) samples = max(8.0, samples * 0.5);
                        if (quality > 1.0f) samples = min(32.0, samples * 1.5);
                        if (quality > 2.0f) samples = min(64.0, samples * 2.0);
                        #if defined(_USE_OPTIMIZED_PCSS_ON)
                            shadow1 = PCSS_Optimized(shadow1, i.pos.z, _LocalPCSSFilterRadius, _LocalPCSSLightSize, samples);
                        #else
                            shadow1 = PCSS(shadow1, i.pos.z, _LocalPCSSFilterRadius, _LocalPCSSLightSize, samples);
                        #endif
                        shadow1 = lerp(1.0, shadow1, _PCSSIntensity);
                    #endif
                #elif defined(_USESHADOW_ON)
                    shadow1 = SHADOW_ATTENUATION(i);
                    shadow1 = saturate(shadow1 + _ShadowBorder);
                    shadow1 = smoothstep(0.0, _ShadowBlur, shadow1);
                #endif
                
                // 第2影と第3影の計算
                #if defined(_USESHADOW2_ON)
                    shadow2 = SHADOW_ATTENUATION(i);
                    shadow2 = saturate(shadow2 + _Shadow2Border);
                    shadow2 = smoothstep(0.0, _Shadow2Blur, shadow2);
                #endif
                
                #if defined(_USESHADOW3_ON)
                    shadow3 = SHADOW_ATTENUATION(i);
                    shadow3 = saturate(shadow3 + _Shadow3Border);
                    shadow3 = smoothstep(0.0, _Shadow3Blur, shadow3);
                #endif
                
                // SDF Face Shadowの適用
                float sdfFaceShadow = CalculateSDFFaceShadow(i.uv, i.worldNormal);
                shadow1 *= sdfFaceShadow;
                
                // 最終的な影の合成
                float finalShadow = shadow1 * shadow2 * shadow3;
                
                #if defined(_USESHADOWMASK_ON)
                    fixed4 mask = tex2D(_ShadowMaskTex, i.uv);
                    finalShadow = lerp(finalShadow, 1.0, mask.g * _ShadowMaskStrength);
                #endif
                
                #if defined(_USEFLIPBOOK_ON)
                    float flipbookTotalFrames = _FlipbookDivisionsX * _FlipbookDivisionsY;
                    float currentFrame = floor(fmod(_Time.y * _FlipbookSpeed, flipbookTotalFrames));
                    float frameX = fmod(currentFrame, _FlipbookDivisionsX);
                    float frameY = floor(currentFrame / _FlipbookDivisionsX);
                    float2 flipbookUV = i.uv / float2(_FlipbookDivisionsX, _FlipbookDivisionsY) + float2(frameX / _FlipbookDivisionsX, -frameY / _FlipbookDivisionsY);
                    col *= tex2D(_FlipbookTex, flipbookUV);
                #endif

                #if defined(_USESHADOWCLAMP_ON)
                    finalShadow = step(_ShadowClamp, finalShadow);
                #endif
                
                // VRC Light Volumes 2.0.0 強化版
                #if defined(VRC_LIGHT_VOLUMES_ENABLED) && defined(_USEVRCLIGHT_VOLUMES_ON)
                    float3 lightVolumeColor = SampleVRCLightVolumes(i.worldPos, i.worldNormal);
                    col.rgb *= lightVolumeColor;
                #endif
                
                // LTCGIの適用
                float3 ltcgiColor = CalculateLTCGI(i.worldPos, i.worldNormal);
                col.rgb *= ltcgiColor;
                
                // Backlightの適用
                float3 backlightColor = CalculateBacklight(i.worldNormal, worldLightDir);
                col.rgb += backlightColor;
                
                finalShadow = lerp(1.0 - _Translucency, 1.0, finalShadow);
                
                #if defined(_SHADOWCOLORTEX)
                    col.rgb *= lerp(tex2D(_ShadowColorTex, i.uv).rgb, float3(1,1,1), finalShadow);
                #else
                    col.rgb *= lerp(float3(0.5, 0.5, 0.5), float3(1,1,1), finalShadow);
                #endif
                
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
        Pass
        {
            Name "ShadowCaster"
            Tags {"LightMode" = "ShadowCaster"}
            ZWrite On
            ZTest LEqual
            Cull [_Cull]
            
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing
            
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Cutoff;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                V2F_SHADOW_CASTER;
                float2 uv : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                clip(col.a - _Cutoff);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "lilToon"
    CustomEditor "lilToon.PCSS.Editor.LilToonPCSSShaderGUI"
} 