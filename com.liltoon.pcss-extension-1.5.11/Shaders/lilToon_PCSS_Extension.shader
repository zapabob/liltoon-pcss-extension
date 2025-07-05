Shader "lilToon/PCSS Extension"
{
    Properties
    {
        // --- lilToon標準プロパティ ---
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        // --- 影・シャドウ ---
        [lilToggle] _UseShadow ("Use Shadow", Float) = 1
        _ShadowColorTex ("Shadow Color", 2D) = "black" {}
        _ShadowBorder ("Shadow Border", Range(0, 1)) = 0.5
        _ShadowBlur ("Shadow Blur", Range(0, 1)) = 0.1
        // --- PCSS拡張プロパティ ---
        [lilToggle] _UsePCSS ("Use PCSS", Float) = 1
        [Enum(Realistic,0,Anime,1,Cinematic,2,Custom,3)] _PCSSPresetMode ("PCSS Preset", Float) = 1
        _LocalPCSSFilterRadius ("PCSS Filter Radius", Range(0.001, 0.1)) = 0.01
        _LocalPCSSLightSize ("PCSS Light Size", Range(0.01, 0.5)) = 0.1
        _LocalPCSSBias ("PCSS Bias", Range(0.0001, 0.01)) = 0.001
        _PCSSIntensity ("PCSS Intensity", Range(0.0, 2.0)) = 1.0
        [Enum(Low,0,Medium,1,High,2,Ultra,3)] _PCSSQualityLevel ("PCSS Quality", Float) = 1
        _LocalPCSSSamples ("PCSS Samples", Range(1, 64)) = 16
        // --- その他（省略可） ---
        [lilToggle] _UseShadowClamp ("Use Shadow Clamp (Anime Style)", Float) = 0
        _ShadowClamp ("Shadow Clamp", Range(0, 1)) = 0.5
        _Translucency ("Translucency", Range(0, 1)) = 0.5
        // --- VRC Light Volumes ---
        [lilToggle] _UseVRCLightVolumes ("Use VRC Light Volumes", Float) = 0
        _VRCLightVolumeIntensity ("VRC Light Volume Intensity", Range(0.0, 2.0)) = 1.0
        _VRCLightVolumeTint ("VRC Light Volume Tint", Color) = (1,1,1,1)
        _VRCLightVolumeDistanceFactor ("VRC Light Volume Distance Factor", Range(0.0, 1.0)) = 0.1
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
        // --- VirtualLight拡張 ---
        _VirtualLightPos ("Virtual Light Position", Vector) = (0,0,0)
        _VirtualLightDir ("Virtual Light Direction", Vector) = (0,0,0)
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
            #pragma shader_feature_local _ _USEVRCLIGHT_VOLUMES_ON
            #pragma shader_feature_local _ _USESHADOWCLAMP_ON
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
            #include "Includes/lil_pcss_shadows.hlsl"

            // --- プロパティ宣言 ---
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            sampler2D _ShadowColorTex;
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
            float _Cutoff;
            float _UseShadowClamp;
            float _ShadowClamp;
            float _Translucency;
            // PCSS変数の重複を避けるためにローカル変数として定義
            float _LocalPCSSFilterRadius;
            float _LocalPCSSLightSize;
            float _LocalPCSSSamples;
            float _LocalPCSSBias;
            float _PCSSQualityLevel; // _PCSSQualityから_PCSSQualityLevelに変更
            // --- VirtualLight拡張 ---
            float3 _VirtualLightPos;
            float3 _VirtualLightDir;

            // VRCライトボリューム用関数
            #if defined(VRC_LIGHT_VOLUMES_ENABLED)
            float3 SampleVRCLightVolumes(float3 worldPos)
            {
                #if defined(VRC_LIGHT_VOLUMES_MOBILE)
                    // モバイル向け簡易版
                    float3 localPos = mul(_VRCLightVolumeWorldToLocal, float4(worldPos, 1.0)).xyz;
                    float3 volumeUV = localPos * 0.5 + 0.5;
                    float3 lightColor = tex3D(_VRCLightVolumeTexture, volumeUV).rgb;
                    return lightColor * _VRCLightVolumeTint.rgb * _VRCLightVolumeIntensity;
                #else
                    // フル機能版
                    float3 localPos = mul(_VRCLightVolumeWorldToLocal, float4(worldPos, 1.0)).xyz;
                    float3 volumeUV = localPos * 0.5 + 0.5;
                    // ボリュームの範囲外なら影響なし
                    if (volumeUV.x < 0.0 || volumeUV.x > 1.0 || volumeUV.y < 0.0 || volumeUV.y > 1.0 || volumeUV.z < 0.0 || volumeUV.z > 1.0)
                        return float3(1.0, 1.0, 1.0);
                    float3 lightColor = tex3D(_VRCLightVolumeTexture, volumeUV).rgb;
                    // 距離による減衰
                    float distFactor = 1.0 - saturate(length(localPos) * _VRCLightVolumeDistanceFactor);
                    return lerp(float3(1.0, 1.0, 1.0), lightColor * _VRCLightVolumeTint.rgb, _VRCLightVolumeIntensity * distFactor);
                #endif
            }
            #else
            float3 SampleVRCLightVolumes(float3 worldPos)
            {
                return float3(1.0, 1.0, 1.0);
            }
            #endif

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
                
                float shadow = 1.0;
                #if defined(_USEPCSS_ON)
                    // --- 仮想光源による減衰・角度・距離計算 ---
                    float3 toLight = normalize(_VirtualLightPos - i.worldPos);
                    float angle = dot(toLight, normalize(_VirtualLightDir));
                    float attenuation = saturate(angle); // スポットライト的な減衰
                    float dist = length(_VirtualLightPos - i.worldPos);
                    float distAtten = 1.0 / (1.0 + dist * dist * 0.1); // 距離減衰（調整可）
                    float pcssFilter = _LocalPCSSFilterRadius * attenuation * distAtten;
                    // PCSSを使用 - 標準のシャドウ座標を使用
                    #ifdef LIL_PCSS_MOBILE_PLATFORM
                        shadow = SHADOW_ATTENUATION(i);
                        shadow = PCSSMobile(shadow, i.pos.z);
                    #else
                        shadow = SHADOW_ATTENUATION(i);
                        float samples = _LocalPCSSSamples;
                        float quality = _PCSSQualityLevel;
                        if (quality < 1.0f) samples = max(8.0, samples * 0.5);
                        if (quality > 1.0f) samples = min(32.0, samples * 1.5);
                        if (quality > 2.0f) samples = min(64.0, samples * 2.0);
                        shadow = PCSS(shadow, i.pos.z, pcssFilter, _LocalPCSSLightSize, samples);
                        shadow = lerp(1.0, shadow, _PCSSIntensity);
                    #endif
                #elif defined(_USESHADOW_ON)
                    shadow = SHADOW_ATTENUATION(i);
                    shadow = saturate(shadow + _ShadowBorder);
                    shadow = smoothstep(0.0, _ShadowBlur, shadow);
                #endif
                
                #if defined(_USESHADOWCLAMP_ON)
                    shadow = step(_ShadowClamp, shadow);
                #endif
                
                #if defined(VRC_LIGHT_VOLUMES_ENABLED) && defined(_USEVRCLIGHT_VOLUMES_ON)
                    float3 lightVolumeColor = SampleVRCLightVolumes(i.worldPos);
                    col.rgb *= lightVolumeColor;
                #endif
                
                shadow = lerp(1.0 - _Translucency, 1.0, shadow);
                
                #if defined(_SHADOWCOLORTEX)
                    col.rgb *= lerp(tex2D(_ShadowColorTex, i.uv).rgb, float3(1,1,1), shadow);
                #else
                    col.rgb *= lerp(float3(0.5, 0.5, 0.5), float3(1,1,1), shadow);
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