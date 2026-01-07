Shader "lilToon/PCSS Extension"
{
    Properties
    {
        // --- lilToon基本プロパティ ---
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        
        // --- 影・シャドウ ---
        _UseShadow ("Use Shadow", Float) = 1
        _ShadowColorTex ("Shadow Color Texture", 2D) = "white" {}
        _ShadowBorder ("Shadow Border", Range(0.0, 1.0)) = 0.5
        _ShadowBlur ("Shadow Blur", Range(0.0, 1.0)) = 0.1
        
        // --- PCSS設定 ---
        _UsePCSS ("Use PCSS", Float) = 1
        _PCSSPresetMode ("PCSS Preset Mode", Range(0, 2)) = 1
        _LocalPCSSFilterRadius ("Local PCSS Filter Radius", Range(0.001, 0.1)) = 0.01
        _LocalPCSSLightSize ("Local PCSS Light Size", Range(0.01, 1.0)) = 0.1
        _LocalPCSSBias ("Local PCSS Bias", Range(0.0001, 0.01)) = 0.001
        _PCSSIntensity ("PCSS Intensity", Range(0.0, 2.0)) = 1.0
        _PCSSQualityLevel ("PCSS Quality Level", Range(0, 2)) = 1
        _LocalPCSSSamples ("Local PCSS Samples", Range(4, 64)) = 16
        
        // --- その他の設定 ---
        _UseShadowClamp ("Use Shadow Clamp", Float) = 0
        _ShadowClamp ("Shadow Clamp", Range(0.0, 1.0)) = 0.5
        _Translucency ("Translucency", Range(0.0, 1.0)) = 0.5
        
        // --- VRC Light Volumes 2.0.0 ---
        _UseVRCLightVolumes ("Use VRC Light Volumes", Float) = 0
        _VRCLightVolumeIntensity ("VRC Light Volume Intensity", Range(0.0, 2.0)) = 1.0
        _VRCLightVolumeTint ("VRC Light Volume Tint", Color) = (1,1,1,1)
        _VRCLightVolumeDistanceFactor ("VRC Light Volume Distance Factor", Range(0.01, 1.0)) = 0.1
        _EnvRimBorder ("[VRCLV] Rim Border", Range(0, 1)) = 0.85
        _EnvRimBlur ("[VRCLV] Rim Blur", Range(0, 1)) = 0.35
    }
    
    CustomEditor "lilToonPCSS.Editor.LilToonPCSSShaderGUI"
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma shader_feature_local _ _USEPCSS_ON
            #pragma shader_feature_local _ _USESHADOW_ON
            #pragma shader_feature_local _ _USEVRCLIGHTVOLUMES_ON
            #pragma shader_feature_local _ _USEVRCLV_RIMLIGHT_ON
            
            // VRChat対応のインクルード
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            // lilToon 2.x.x対応
            #if defined(LIL_LILTOON_SHADER_INCLUDED)
                #include "Packages/jp.lilxyzw.liltoon/ShaderIncludes/lil_common.cginc"
                #include "Packages/jp.lilxyzw.liltoon/ShaderIncludes/lil_lighting.cginc"
                #include "Packages/jp.lilxyzw.liltoon/ShaderIncludes/lil_shadow.cginc"
            #endif
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                SHADOW_COORDS(3)
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Cutoff;
            float _UsePCSS;
            float _PCSSIntensity;
            float _LocalPCSSFilterRadius;
            float _LocalPCSSLightSize;
            float _LocalPCSSBias;
            float _PCSSQualityLevel;
            float _LocalPCSSSamples;
            
            // PCSS関数（VRChat対応版）
            float ApplyPCSS(float shadow, float3 worldPos, float3 worldNormal, float3 lightDir)
            {
                #ifdef _USEPCSS_ON
                    if (_UsePCSS > 0.5)
                    {
                        // VRChat向けに最適化されたPCSS
                        float samples = _LocalPCSSSamples;
                        float quality = _PCSSQualityLevel;
                        
                        // 品質に応じてサンプル数を調整
                        if (quality < 1.0) samples = max(4.0, samples * 0.5);
                        if (quality > 1.0) samples = min(32.0, samples * 1.5);
                        
                        // 簡易版PCSS（VRChatの制限内）
                        float filterRadius = _LocalPCSSFilterRadius;
                        float lightSize = _LocalPCSSLightSize;
                        float bias = _LocalPCSSBias;
                        
                        // 距離ベースのソフトシャドウ
                        float distance = length(worldPos - _WorldSpaceCameraPos);
                        float softness = lerp(0.1, 0.5, saturate(distance / 10.0));
                        
                        shadow = lerp(shadow, 1.0, softness * filterRadius);
                        shadow = lerp(1.0, shadow, _PCSSIntensity);
                    }
                #endif
                return shadow;
            }
            
            // VRC Light Volumes 2.0.0対応
            float3 ApplyVRCLightVolumes(float3 color, float3 worldPos, float3 worldNormal)
            {
                #ifdef _USEVRCLIGHTVOLUMES_ON
                    // VRC Light Volumes 2.0.0の簡易実装
                    float3 lightVolumeColor = float3(1.0, 1.0, 1.0);
                    
                    // 距離ベースのライトボリューム効果
                    float distance = length(worldPos - _WorldSpaceCameraPos);
                    float volumeIntensity = _VRCLightVolumeIntensity;
                    float3 volumeTint = _VRCLightVolumeTint.rgb;
                    float distanceFactor = _VRCLightVolumeDistanceFactor;
                    
                    // 距離による減衰
                    float distAttenuation = 1.0 - saturate(distance * distanceFactor);
                    lightVolumeColor = lerp(float3(1.0, 1.0, 1.0), volumeTint, volumeIntensity * distAttenuation);
                    
                    color *= lightVolumeColor;
                #endif
                return color;
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                TRANSFER_SHADOW(o);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // 基本ライティング
                float3 worldNormal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = dot(worldNormal, lightDir);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                
                // 影の計算
                float shadow = SHADOW_ATTENUATION(i);
                
                // PCSS適用
                shadow = ApplyPCSS(shadow, i.worldPos, worldNormal, lightDir);
                
                // VRC Light Volumes適用
                col.rgb = ApplyVRCLightVolumes(col.rgb, i.worldPos, worldNormal);
                
                // 基本ライティング適用
                col.rgb *= lerp(0.5, 1.0, shadow);
                
                // アルファカットオフ
                clip(col.a - _Cutoff);
                
                return col;
            }
            ENDHLSL
        }
        
        // シャドウキャストパス
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                V2F_SHADOW_CASTER;
                float2 uv : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Cutoff;
            
            v2f vert(appdata v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                clip(col.a - _Cutoff);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDHLSL
        }
    }
    
    FallBack "Diffuse"
} 