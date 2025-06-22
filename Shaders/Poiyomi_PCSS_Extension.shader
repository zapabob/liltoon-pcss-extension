Shader "Poiyomi/Toon/PCSS Extension"
{
    Properties
    {
        // Base Properties
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _AlphaMask ("Alpha Mask", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        
        // Poiyomi互換性のためのプロパティ
        [ToggleUI] _EnableShadows ("Enable Shadows", Float) = 1
        _ShadowStrength ("Shadow Strength", Range(0.0, 1.0)) = 1.0
        _ShadowOffset ("Shadow Offset", Range(-1.0, 1.0)) = 0.0
        
        // PCSS Properties
        [ToggleUI] _PCSSEnabled ("Enable PCSS", Float) = 1
        [Enum(Realistic,0,Anime,1,Cinematic,2,Custom,3)] _PCSSPresetMode ("PCSS Preset", Float) = 1
        _PCSSFilterRadius ("PCSS Filter Radius", Range(0.001, 0.1)) = 0.01
        _PCSSLightSize ("PCSS Light Size", Range(0.01, 0.5)) = 0.1
        _PCSSBias ("PCSS Bias", Range(0.0001, 0.01)) = 0.001
        _PCSSIntensity ("PCSS Intensity", Range(0.0, 2.0)) = 1.0
        [Enum(Low,0,Medium,1,High,2,Ultra,3)] _PCSSQuality ("PCSS Quality", Float) = 1
        
        // VRC Light Volumes
        [ToggleUI] _UseVRCLightVolumes ("Use VRC Light Volumes", Float) = 0
        _VRCLightVolumeIntensity ("VRC Light Volume Intensity", Range(0.0, 2.0)) = 1.0
        _VRCLightVolumeTint ("VRC Light Volume Tint", Color) = (1,1,1,1)
        
        // Rendering Properties
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 2
        [Enum(Off,0,On,1)] _ZWrite ("ZWrite", Float) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
        
        // Stencil Properties
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

        // Stencil settings
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

        // Forward Pass with PCSS
        Pass
        {
            Name "FORWARD"
            Tags {"LightMode" = "ForwardBase"}
            
            Cull [_Cull]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Blend [_SrcBlend] [_DstBlend]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            // PCSS機能を有効にするシェーダーキーワード
            #pragma shader_feature PCSS_ENABLED
            #pragma shader_feature _USEVRCLIGHT_VOLUMES_ON
            #pragma shader_feature _VRCHAT_EXPRESSION
            
            // VRC Light Volumes統合キーワード
            #pragma multi_compile _ VRC_LIGHT_VOLUMES_ENABLED
            #pragma multi_compile _ VRC_LIGHT_VOLUMES_MOBILE

            // Poiyomi互換性のためのインクルード順序
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            // PCSS関連のインクルードファイル - 重複を防ぐためのマクロ定義
            #define LIL_POIYOMI_SHADER_INCLUDED  // Poiyomiシェーダーとの競合を防ぐ
            #include "Includes/lil_pcss_common.hlsl"
            #include "Includes/lil_pcss_shadows.hlsl"
            
            // Textures - 重複を避けるため条件付き宣言
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _AlphaMask;
            float4 _AlphaMask_ST;
            
            // Main Properties
            fixed4 _Color;
            float _Cutoff;
            
            // PCSS Properties - lil_pcss_common.hlslで定義済み
            // Poiyomi固有の変数のみここで定義
            
            // VRC Light Volumes関連の変数
            float _UseVRCLightVolumes;
            float _VRCLightVolumeIntensity;
            float4 _VRCLightVolumeTint;
            
            // Poiyomi Shadow Properties
            float _EnableShadows;
            float _ShadowStrength;
            float _ShadowOffset;

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
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                fixed4 alphaMask = tex2D(_AlphaMask, i.uv);
                
                // Alpha test
                clip(col.a * alphaMask.a - _Cutoff);
                
                float shadow = 1.0;

                #if defined(PCSS_ENABLED)
                    // PCSSを使用
                    #ifdef LIL_PCSS_MOBILE_PLATFORM
                        shadow = PCSSMobile(i._LightCoord, i.pos.z);
                    #else
                        shadow = PCSS(i._LightCoord, i.pos.z);
                    #endif
                #elif defined(_ENABLESHADOWS_ON)
                    // 標準のシャドウを使用
                    shadow = SHADOW_ATTENUATION(i);
                    
                    // Poiyomi風のシャドウ処理
                    shadow = lerp(1.0, shadow, _ShadowStrength);
                    shadow = saturate(shadow + _ShadowOffset);
                #endif
                
                // VRC Light Volumesからのライティングを適用
                #if defined(VRC_LIGHT_VOLUMES_ENABLED) && defined(_USEVRCLIGHT_VOLUMES_ON)
                    float3 lightVolumeColor = SampleVRCLightVolumes(i.worldPos);
                    col.rgb *= lightVolumeColor * _VRCLightVolumeIntensity;
                #endif

                // 影を適用 - Poiyomi風
                col.rgb *= shadow;
                
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDHLSL
        }
        
        // Shadow caster pass - Poiyomi互換
        Pass
        {
            Name "ShadowCaster"
            Tags {"LightMode" = "ShadowCaster"}
            
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing
            
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _AlphaMask;
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
                fixed4 alphaMask = tex2D(_AlphaMask, i.uv);
                clip(col.a * alphaMask.a - _Cutoff);
                
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDHLSL
        }
    }
    
    // Poiyomi互換のFallback
    FallBack "Standard"
    CustomEditor "lilToon.PCSS.Editor.PoiyomiPCSSShaderGUI"
} 