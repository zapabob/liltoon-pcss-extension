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

    HLSLINCLUDE
        #define LIL_RENDER 0
        #include "UnityCG.cginc"
        #include "AutoLight.cginc"
    ENDHLSL

    SubShader
    {
        Tags {"RenderType"="Opaque" "Queue"="Geometry"}
        LOD 200

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

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

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

            // Poisson Disk Sampling for PCSS
            static const float2 poissonDisk[64] = {
                float2(-0.613392, 0.617481), float2(0.170019, -0.040254), float2(-0.299417, 0.791925), float2(0.645680, 0.493210),
                float2(-0.651784, 0.717887), float2(0.421003, 0.027070), float2(-0.817194, -0.271096), float2(-0.705374, -0.668203),
                float2(0.977050, -0.108615), float2(0.063326, 0.142369), float2(0.203528, 0.214331), float2(-0.667531, 0.326090),
                float2(-0.098422, -0.295755), float2(-0.885922, 0.215369), float2(0.566637, 0.605213), float2(0.039766, -0.396100),
                float2(0.751946, 0.453352), float2(0.078707, -0.715323), float2(-0.075838, -0.529344), float2(0.724479, -0.580798),
                float2(0.222999, -0.215125), float2(-0.467574, -0.405438), float2(-0.248268, -0.814753), float2(0.354411, -0.887570),
                float2(0.175817, 0.382366), float2(0.487472, -0.063082), float2(-0.084078, 0.898312), float2(0.488876, -0.783441),
                float2(0.470016, 0.217933), float2(-0.696890, -0.549791), float2(-0.149693, 0.605762), float2(0.034211, 0.979980),
                float2(0.503098, -0.308878), float2(-0.016205, -0.872921), float2(0.385784, -0.393902), float2(-0.146886, -0.859249),
                float2(0.643361, 0.164098), float2(0.634388, -0.049471), float2(-0.688894, 0.007843), float2(0.464034, -0.188818),
                float2(-0.440840, 0.137486), float2(0.364483, 0.511704), float2(0.034028, 0.325968), float2(0.099094, -0.308023),
                float2(0.693960, -0.366253), float2(0.678884, -0.204688), float2(0.001801, 0.780328), float2(0.145177, -0.898984),
                float2(0.062655, -0.611866), float2(0.315226, -0.604297), float2(-0.780145, 0.486251), float2(-0.371868, 0.882138),
                float2(0.200476, 0.494430), float2(-0.494552, -0.711051), float2(0.612476, 0.705252), float2(-0.578845, -0.768792),
                float2(-0.772454, -0.090976), float2(0.504440, 0.372295), float2(0.155736, 0.065157), float2(0.391522, 0.849605),
                float2(-0.620106, -0.328104), float2(0.789239, -0.419965), float2(-0.545396, 0.538133), float2(-0.178564, -0.596057)
            };

            // PCSS Blocker Search
            float findBlocker(float2 uv, float zReceiver, float searchRadius)
            {
                float blockerSum = 0.0;
                int numBlockers = 0;
                
                for(int i = 0; i < _PCSSSampleCount; i++)
                {
                    float2 sampleUV = uv + poissonDisk[i] * searchRadius;
                    float shadowMapDepth = tex2D(_ShadowMapTexture, sampleUV).r;
                    
                    if(shadowMapDepth < zReceiver)
                    {
                        blockerSum += shadowMapDepth;
                        numBlockers++;
                    }
                }
                
                return numBlockers > 0 ? blockerSum / numBlockers : -1.0;
            }

            // PCSS Penumbra Size Estimation
            float penumbraSize(float zReceiver, float zBlocker, float lightSize)
            {
                return lightSize * (zReceiver - zBlocker) / zBlocker;
            }

            // PCF Filtering
            float PCF(float2 uv, float zReceiver, float filterRadius)
            {
                float sum = 0.0;
                for(int i = 0; i < _PCSSSampleCount; i++)
                {
                    float2 sampleUV = uv + poissonDisk[i] * filterRadius;
                    float shadowMapDepth = tex2D(_ShadowMapTexture, sampleUV).r;
                    sum += (shadowMapDepth >= zReceiver) ? 1.0 : 0.0;
                }
                return sum / _PCSSSampleCount;
            }

            // PCSS Main Function
            float PCSS(float2 uv, float zReceiver)
            {
                // Step 1: Blocker Search
                float avgBlockerDepth = findBlocker(uv, zReceiver, _PCSSBlockerSearchRadius);
                
                if(avgBlockerDepth == -1.0) // No blockers
                    return 1.0;
                
                // Step 2: Penumbra Size
                float penumbraRadius = penumbraSize(zReceiver, avgBlockerDepth, _PCSSLightSize);
                
                // Step 3: PCF
                return PCF(uv, zReceiver, penumbraRadius * _PCSSFilterRadius);
            }

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                
                TRANSFER_SHADOW(o);
                UNITY_TRANSFER_FOG(o, o.pos);
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Base color
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // Alpha test
                clip(col.a - _Cutoff);
                
                // Lighting calculation
                float3 worldNormal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(0, dot(worldNormal, lightDir));
                
                // Shadow calculation
                float shadow = 1.0;
                if(_UseShadow > 0.5)
                {
                    #if defined(_USEPCSS_ON)
                        if(_UsePCSS > 0.5)
                        {
                            // Use PCSS
                            float4 shadowCoord = i._ShadowCoord;
                            float2 shadowUV = shadowCoord.xy / shadowCoord.w;
                            float zReceiver = shadowCoord.z / shadowCoord.w;
                            shadow = PCSS(shadowUV, zReceiver);
                        }
                        else
                        {
                            // Use standard shadow
                            shadow = SHADOW_ATTENUATION(i);
                        }
                    #else
                        shadow = SHADOW_ATTENUATION(i);
                    #endif
                    
                    // Apply shadow border and blur
                    shadow = smoothstep(_ShadowBorder - _ShadowBlur * 0.5, _ShadowBorder + _ShadowBlur * 0.5, shadow);
                    
                    // Apply shadow color
                    float3 shadowColor = tex2D(_ShadowColorTex, float2(0.5, 0.5)).rgb;
                    col.rgb = lerp(col.rgb * shadowColor, col.rgb, shadow);
                }
                
                // Apply lighting
                col.rgb *= NdotL * _LightColor0.rgb + unity_AmbientSky.rgb;
                
                // Apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                return col;
            }
            ENDHLSL
        }

        // Shadow Caster Pass
        Pass
        {
            Name "ShadowCaster"
            Tags {"LightMode" = "ShadowCaster"}
            
            ZWrite On
            ZTest LEqual
            Cull [_Cull]

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
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);
                
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                clip(col.a - _Cutoff);
                
                SHADOW_CASTER_FRAGMENT(i);
            }
            ENDHLSL
        }
    }
    
    CustomEditor "lilToon.lilToonInspector"
    Fallback "VertexLit"
} 