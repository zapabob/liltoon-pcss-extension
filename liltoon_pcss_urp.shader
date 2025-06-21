Shader "lilToon/PCSS Extension URP"
{
    Properties
    {
        // 基本的なliltoonプロパティ
        [lilHDR] [MainColor] _Color                 ("sColor", Color) = (1,1,1,1)
        [MainTexture]   _MainTex                    ("Texture", 2D) = "white" {}
        
        // PCSS関連プロパティ
        [lilToggleLeft] _UsePCSS                    ("Use PCSS", Int) = 0
        _PCSSBlockerSearchRadius                    ("PCSS Blocker Search Radius", Range(0.001, 0.1)) = 0.01
        _PCSSFilterRadius                           ("PCSS Filter Radius", Range(0.001, 0.1)) = 0.01
        _PCSSSampleCount                            ("PCSS Sample Count", Range(4, 64)) = 16
        _PCSSLightSize                              ("PCSS Light Size", Range(0.1, 10.0)) = 1.0
        _PCSSMaxDistance                            ("PCSS Max Distance", Range(1.0, 100.0)) = 50.0
        
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
        
        // Surface Options
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlendAlpha("Src Blend Alpha", Float) = 1.0
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlendAlpha("Dst Blend Alpha", Float) = 10.0
        [Toggle] _AlphaClip("Alpha Clip", Float) = 0.0
        [Toggle] _ReceiveShadows("Receive Shadows", Float) = 1.0
    }

    HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
    ENDHLSL

    SubShader
    {
        Tags {"RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry"}
        LOD 300

        // Forward Pass with PCSS
        Pass
        {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}
            
            Cull [_Cull]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _CLUSTERED_RENDERING

            // Unity keywords
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            
            // PCSS機能を有効にするシェーダーキーワード
            #pragma shader_feature _USEPCSS_ON
            #pragma shader_feature _USESHADOW_ON
            #pragma shader_feature _ALPHACLIP_ON
            #pragma shader_feature _RECEIVESHADOWS_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            // プロパティの宣言
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_ShadowColorTex);
            SAMPLER(sampler_ShadowColorTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                
                // PCSS関連の変数
                float _UsePCSS;
                float _PCSSBlockerSearchRadius;
                float _PCSSFilterRadius;
                int _PCSSSampleCount;
                float _PCSSLightSize;
                float _PCSSMaxDistance;
                
                // シャドウ関連
                float _UseShadow;
                float _ShadowBorder;
                float _ShadowBlur;
                float _Cutoff;
                float _AlphaClip;
                float _ReceiveShadows;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
                float fogCoord : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // Poisson Disk Sampling for PCSS (optimized for URP)
            static const float2 poissonDisk[32] = {
                float2(-0.613392, 0.617481), float2(0.170019, -0.040254), float2(-0.299417, 0.791925), float2(0.645680, 0.493210),
                float2(-0.651784, 0.717887), float2(0.421003, 0.027070), float2(-0.817194, -0.271096), float2(-0.705374, -0.668203),
                float2(0.977050, -0.108615), float2(0.063326, 0.142369), float2(0.203528, 0.214331), float2(-0.667531, 0.326090),
                float2(-0.098422, -0.295755), float2(-0.885922, 0.215369), float2(0.566637, 0.605213), float2(0.039766, -0.396100),
                float2(0.751946, 0.453352), float2(0.078707, -0.715323), float2(-0.075838, -0.529344), float2(0.724479, -0.580798),
                float2(0.222999, -0.215125), float2(-0.467574, -0.405438), float2(-0.248268, -0.814753), float2(0.354411, -0.887570),
                float2(0.175817, 0.382366), float2(0.487472, -0.063082), float2(-0.084078, 0.898312), float2(0.488876, -0.783441),
                float2(0.470016, 0.217933), float2(-0.696890, -0.549791), float2(-0.149693, 0.605762), float2(0.034211, 0.979980),
                float2(0.503098, -0.308878), float2(-0.016205, -0.872921), float2(0.385784, -0.393902), float2(-0.146886, -0.859249),
                float2(0.643361, 0.164098), float2(0.634388, -0.049471), float2(-0.688894, 0.007843), float2(0.464034, -0.188818)
            };

            // PCSS Blocker Search for URP
            float findBlockerURP(float4 shadowCoord, float searchRadius)
            {
                float blockerSum = 0.0;
                int numBlockers = 0;
                int sampleCount = min(_PCSSSampleCount, 32);
                
                for(int i = 0; i < sampleCount; i++)
                {
                    float2 sampleUV = shadowCoord.xy + poissonDisk[i] * searchRadius;
                    float shadowMapDepth = SAMPLE_TEXTURE2D_SHADOW(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture, float3(sampleUV, shadowCoord.z));
                    
                    if(shadowMapDepth < shadowCoord.z)
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
                return lightSize * (zReceiver - zBlocker) / max(zBlocker, 0.001);
            }

            // PCF Filtering for URP
            float PCF_URP(float4 shadowCoord, float filterRadius)
            {
                float sum = 0.0;
                int sampleCount = min(_PCSSSampleCount, 32);
                
                for(int i = 0; i < sampleCount; i++)
                {
                    float2 sampleUV = shadowCoord.xy + poissonDisk[i] * filterRadius;
                    sum += SAMPLE_TEXTURE2D_SHADOW(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture, float3(sampleUV, shadowCoord.z));
                }
                return sum / sampleCount;
            }

            // PCSS Main Function for URP
            float PCSS_URP(float4 shadowCoord)
            {
                // Step 1: Blocker Search
                float avgBlockerDepth = findBlockerURP(shadowCoord, _PCSSBlockerSearchRadius);
                
                if(avgBlockerDepth == -1.0) // No blockers
                    return 1.0;
                
                // Step 2: Penumbra Size
                float penumbraRadius = penumbraSize(shadowCoord.z, avgBlockerDepth, _PCSSLightSize);
                
                // Step 3: PCF
                return PCF_URP(shadowCoord, penumbraRadius * _PCSSFilterRadius);
            }

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                
                #if defined(_MAIN_LIGHT_SHADOWS) && defined(_RECEIVESHADOWS_ON)
                    output.shadowCoord = GetShadowCoord(vertexInput);
                #endif
                
                output.fogCoord = ComputeFogFactor(output.positionCS.z);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Base color
                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * _Color;
                
                // Alpha test
                #if defined(_ALPHACLIP_ON)
                    clip(albedo.a - _Cutoff);
                #endif
                
                // Lighting calculation
                Light mainLight = GetMainLight();
                half3 normalWS = normalize(input.normalWS);
                half NdotL = saturate(dot(normalWS, mainLight.direction));
                
                // Shadow calculation
                half shadow = 1.0;
                #if defined(_MAIN_LIGHT_SHADOWS) && defined(_USESHADOW_ON) && defined(_RECEIVESHADOWS_ON)
                    #if defined(_USEPCSS_ON)
                        if(_UsePCSS > 0.5)
                        {
                            // Use PCSS
                            shadow = PCSS_URP(input.shadowCoord);
                        }
                        else
                        {
                            // Use standard shadow
                            shadow = MainLightRealtimeShadow(input.shadowCoord);
                        }
                    #else
                        shadow = MainLightRealtimeShadow(input.shadowCoord);
                    #endif
                    
                    // Apply shadow border and blur
                    shadow = smoothstep(_ShadowBorder - _ShadowBlur * 0.5, _ShadowBorder + _ShadowBlur * 0.5, shadow);
                    
                    // Apply shadow color
                    half3 shadowColor = SAMPLE_TEXTURE2D(_ShadowColorTex, sampler_ShadowColorTex, half2(0.5, 0.5)).rgb;
                    albedo.rgb = lerp(albedo.rgb * shadowColor, albedo.rgb, shadow);
                #endif
                
                // Apply main light
                half3 color = albedo.rgb * (NdotL * mainLight.color * mainLight.shadowAttenuation + SampleSH(normalWS));
                
                // Apply fog
                color = MixFog(color, input.fogCoord);
                
                return half4(color, albedo.a);
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
            ColorMask 0
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #pragma multi_compile_instancing
            #pragma shader_feature _ALPHACLIP_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        // Depth Only Pass
        Pass
        {
            Name "DepthOnly"
            Tags {"LightMode" = "DepthOnly"}
            
            ZWrite On
            ColorMask 0
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            
            #pragma multi_compile_instancing
            #pragma shader_feature _ALPHACLIP_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
    }
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
} 