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

            // 改良されたPoisson Disk Sampling for PCSS
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

            // PCSS Blocker Search - 改良版
            float findBlocker(float2 uv, float zReceiver, float searchRadius)
            {
                float blockerSum = 0.0;
                int numBlockers = 0;
                int sampleCount = min(_PCSSSampleCount, 64);
                
                for(int i = 0; i < sampleCount; i++)
                {
                    float2 sampleUV = uv + poissonDisk[i] * searchRadius;
                    
                    // UV座標のクランプ
                    sampleUV = clamp(sampleUV, 0.0, 1.0);
                    
                    float shadowMapDepth = SAMPLE_DEPTH_TEXTURE(_ShadowMapTexture, sampleUV);
                    shadowMapDepth = LinearEyeDepth(shadowMapDepth);
                    
                    if(shadowMapDepth < zReceiver - _PCSSBias)
                    {
                        blockerSum += shadowMapDepth;
                        numBlockers++;
                    }
                }
                
                return numBlockers > 0 ? blockerSum / numBlockers : -1.0;
            }

            // PCSS Penumbra Size Estimation - 改良版
            float penumbraSize(float zReceiver, float zBlocker, float lightSize)
            {
                return lightSize * (zReceiver - zBlocker) / max(zBlocker, 0.001);
            }

            // PCF Filtering - 改良版
            float PCF(float2 uv, float zReceiver, float filterRadius)
            {
                float sum = 0.0;
                int sampleCount = min(_PCSSSampleCount, 64);
                
                for(int i = 0; i < sampleCount; i++)
                {
                    float2 sampleUV = uv + poissonDisk[i] * filterRadius;
                    sampleUV = clamp(sampleUV, 0.0, 1.0);
                    
                    float shadowMapDepth = SAMPLE_DEPTH_TEXTURE(_ShadowMapTexture, sampleUV);
                    shadowMapDepth = LinearEyeDepth(shadowMapDepth);
                    
                    sum += (shadowMapDepth >= zReceiver - _PCSSBias) ? 1.0 : 0.0;
                }
                
                return sum / sampleCount;
            }

            // PCSS Main Function - 改良版
            float PCSS(float2 uv, float zReceiver)
            {
                // Step 1: Blocker Search
                float avgBlockerDepth = findBlocker(uv, zReceiver, _PCSSBlockerSearchRadius);
                
                if(avgBlockerDepth < 0.0) // No blockers
                    return 1.0;
                
                // Step 2: Penumbra Size
                float penumbraRadius = penumbraSize(zReceiver, avgBlockerDepth, _PCSSLightSize);
                penumbraRadius = clamp(penumbraRadius, 0.0, _PCSSFilterRadius * 10.0);
                
                // Step 3: PCF
                return PCF(uv, zReceiver, penumbraRadius * _PCSSFilterRadius);
            }
            
            // VRC Light Volumes統合関数
            float3 SampleVRCLightVolumes(float3 worldPos)
            {
                #if defined(VRC_LIGHT_VOLUMES_ENABLED) && defined(_USEVRCLIGHT_VOLUMES_ON)
                    // ワールド座標をLight Volume座標系に変換
                    float3 volumePos = mul(_VRCLightVolumeWorldToLocal, float4(worldPos, 1.0)).xyz;
                    
                    // UV座標に変換（0-1範囲）
                    float3 volumeUV = volumePos * 0.5 + 0.5;
                    
                    // 範囲外チェック
                    if (any(volumeUV < 0.0) || any(volumeUV > 1.0))
                        return float3(1, 1, 1); // デフォルト値
                    
                    // Light Volumeテクスチャからサンプリング
                    float3 lightVolume = tex3D(_VRCLightVolumeTexture, volumeUV).rgb;
                    
                    // 強度と色調を適用
                    lightVolume *= _VRCLightVolumeIntensity * _VRCLightVolumeTint.rgb;
                    
                    // 距離による減衰
                    lightVolume *= _VRCLightVolumeDistanceFactor;
                    
                    // モバイル最適化
                    #ifdef VRC_LIGHT_VOLUMES_MOBILE
                        lightVolume = saturate(lightVolume * 0.8); // モバイルでは少し抑える
                    #endif
                    
                    return lightVolume;
                #else
                    return float3(1, 1, 1); // VRC Light Volumes無効時はデフォルト値
                #endif
            }
            
            // 改良されたライティング計算（VRC Light Volumes統合）
            float3 CalculateLighting(float3 worldPos, float3 worldNormal, float shadow)
            {
                // 基本的な指向性ライト
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(0, dot(worldNormal, lightDir));
                float3 lighting = _LightColor0.rgb * NdotL * shadow;
                
                // VRC Light Volumesを統合
                #if defined(_USEVRCLIGHT_VOLUMES_ON)
                if (_UseVRCLightVolumes > 0.5)
                {
                    float3 volumeLighting = SampleVRCLightVolumes(worldPos);
                    
                    // ライティングをブレンド
                    lighting = lerp(lighting, lighting * volumeLighting, _VRCLightVolumeIntensity);
                    
                    // 環境光の補強
                    lighting += volumeLighting * 0.3; // 少し環境光として追加
                }
                #endif
                
                return lighting;
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
                
                // Light calculation
                float3 worldNormal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(0, dot(worldNormal, lightDir));
                
                // Shadow calculation
                float shadow = 1.0;
                
                #ifdef _USEPCSS_ON
                if(_UsePCSS > 0.5)
                {
                    float2 shadowUV = i._ShadowCoord.xy / i._ShadowCoord.w;
                    float receiverDepth = LinearEyeDepth(i._ShadowCoord.z / i._ShadowCoord.w);
                    shadow = PCSS(shadowUV, receiverDepth);
                }
                else
                #endif
                {
                    shadow = SHADOW_ATTENUATION(i);
                }
                
                // Apply shadow
                #ifdef _USESHADOW_ON
                if(_UseShadow > 0.5)
                {
                    float shadowMask = smoothstep(_ShadowBorder - _ShadowBlur, _ShadowBorder + _ShadowBlur, shadow);
                    fixed4 shadowColor = tex2D(_ShadowColorTex, i.uv);
                    col.rgb = lerp(shadowColor.rgb * col.rgb, col.rgb, shadowMask);
                }
                #endif
                
                // Apply lighting (VRC Light Volumes統合)
                float3 finalLighting = CalculateLighting(i.worldPos, worldNormal, shadow);
                col.rgb *= finalLighting;
                
                // Apply fog
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
    FallBack "lilToon"
} 