Shader "lilToon/PCSS Extension"
{
    Properties
    {
        // lilToon互換性のための基本プロパティ
        [lilHDR] [MainColor] _Color                 ("sColor", Color) = (1,1,1,1)
        [MainTexture]   _MainTex                    ("Texture", 2D) = "white" {}
        
        // PCSS専用プロパティ群
        [lilEnum]                                   _StencilRef ("Stencil Reference", Int) = 51
        [lilEnum]                                   _StencilReadMask ("Stencil ReadMask", Int) = 255
        [lilEnum]                                   _StencilWriteMask ("Stencil WriteMask", Int) = 255
        [lilEnum]                                   _StencilComp ("Stencil Compare", Int) = 3
        [lilEnum]                                   _StencilPass ("Stencil Pass", Int) = 0
        [lilEnum]                                   _StencilFail ("Stencil Fail", Int) = 0
        [lilEnum]                                   _StencilZFail ("Stencil ZFail", Int) = 0
        
        // PCSS機能制御
        [lilToggleLeft] _UsePCSS                    ("Use PCSS", Int) = 0
        [lilEnum]       _PCSSQuality                ("PCSS Quality|Low|Medium|High|Ultra", Int) = 1
        _PCSSBlockerSearchRadius                    ("PCSS Blocker Search Radius", Range(0.001, 0.1)) = 0.01
        _PCSSFilterRadius                           ("PCSS Filter Radius", Range(0.001, 0.1)) = 0.01
        _PCSSSampleCount                            ("PCSS Sample Count", Range(4, 64)) = 16
        _PCSSLightSize                              ("PCSS Light Size", Range(0.1, 10.0)) = 1.0
        _PCSSBias                                   ("PCSS Bias", Range(0.0, 0.1)) = 0.005
        
        // VRC Light Volumes統合
        [Header(VRC Light Volumes Integration)]
        [lilToggleLeft] _UseVRCLightVolumes         ("Use VRC Light Volumes", Int) = 1
        _VRCLightVolumeIntensity                    ("Light Volume Intensity", Range(0.0, 2.0)) = 1.0
        _VRCLightVolumeTint                         ("Light Volume Tint", Color) = (1,1,1,1)
        _VRCLightVolumeDistanceFactor               ("Distance Factor", Range(0.0, 1.0)) = 1.0
        
        // 通常のliltoonシャドウプロパティ
        [lilToggleLeft] _UseShadow                  ("sShadow", Int) = 0
        _ShadowBorder                               ("sBorder", Range(0, 1)) = 0.5
        _ShadowBlur                                 ("sBlur", Range(0, 1)) = 0.1
        [NoScaleOffset] _ShadowColorTex             ("Shadow Color", 2D) = "black" {}
        
        // その他の必要なプロパティ
        _Cutoff                                     ("sCutoff", Range(-0.001,1.001)) = 0.5
        [Enum(UnityEngine.Rendering.CullMode)]      _Cull ("Cull Mode", Float) = 2
        [Enum(UnityEngine.Rendering.BlendMode)]     _SrcBlend ("Src Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]     _DstBlend ("Dst Blend", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("Z Test", Float) = 4
        [Toggle] _ZWrite ("Z Write", Float) = 1
    }
    
    CustomEditor "lilToon.PCSS.LilToonPCSSShaderGUI"

    HLSLINCLUDE
        #define LIL_RENDER 0
        #include "UnityCG.cginc"
        #include "AutoLight.cginc"
    ENDHLSL

    SubShader
    {
        Tags {"RenderType"="Opaque" "Queue"="Geometry"}
        LOD 200

        // Stencil設定
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
            #pragma shader_feature _USEPCSS_ON
            #pragma shader_feature _USESHADOW_ON
            #pragma shader_feature _USEVRCLIGHT_VOLUMES_ON
            
            // VRC Light Volumes統合キーワード
            #pragma multi_compile _ VRC_LIGHT_VOLUMES_ENABLED
            #pragma multi_compile _ VRC_LIGHT_VOLUMES_MOBILE

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            // PCSSとVRC Light Volumesのインクルードファイル
            #include "Includes/lil_pcss_common.hlsl"
            #include "Includes/lil_pcss_shadows.hlsl"

            // プロパティの宣言
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            sampler2D _ShadowColorTex;
            
            // PCSS関連の変数
            float _UsePCSS;
            float _PCSSBlockerSearchRadius;
            float _PCSSFilterRadius;
            int _PCSSSampleCount;
            float _PCSSLightSize;
            float _PCSSBias;
            
            // VRC Light Volumes関連の変数
            float _UseVRCLightVolumes;
            float _VRCLightVolumeIntensity;
            float4 _VRCLightVolumeTint;
            float _VRCLightVolumeDistanceFactor;
            sampler3D _VRCLightVolumeTexture;
            float4 _VRCLightVolumeParams;
            float4x4 _VRCLightVolumeWorldToLocal;
            
            // シャドウ関連
            float _UseShadow;
            float _ShadowBorder;
            float _ShadowBlur;
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

            // PCSS関連の関数群を削除し、インクルードファイルに委譲

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                float shadow = 1.0;

                #if defined(_USEPCSS_ON)
                    #ifdef LIL_PCSS_MOBILE_PLATFORM
                        shadow = PCSSMobile(i._LightCoord, i.pos.z);
                    #else
                        shadow = PCSS(i._LightCoord, i.pos.z);
                    #endif
                #elif defined(_USESHADOW_ON)
                    shadow = SHADOW_ATTENUATION(i);
                #endif
                
                // VRC Light Volumesからのライティングを適用
                #if defined(VRC_LIGHT_VOLUMES_ENABLED) && defined(_USEVRCLIGHT_VOLUMES_ON)
                    float3 lightVolumeColor = SampleVRCLightVolumes(i.worldPos);
                    col.rgb *= lightVolumeColor;
                #endif

                // 影を適用
                col.rgb *= lerp(tex2D(_ShadowColorTex, i.uv).rgb, float3(1,1,1), shadow);
                
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDHLSL
        }
        
        // Shadow caster pass
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
            ENDHLSL
        }
    }
    
    // Fallback
    FallBack "Transparent/Cutout/VertexLit"
} 