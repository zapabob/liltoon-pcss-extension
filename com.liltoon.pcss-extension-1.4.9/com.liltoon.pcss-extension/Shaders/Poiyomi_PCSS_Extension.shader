Shader "Poiyomi/Toon/PCSS Extension"
{
    Properties
    {
        [HideInInspector] shader_master_label ("✦ Poiyomi PCSS Extension v1.0 ✦", Float) = 0
        [HideInInspector] shader_is_using_thry_editor ("", Float) = 0
        
        // Main Properties
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        
        // PCSS Properties
        [Header(PCSS Soft Shadows)]
        [Toggle(PCSS_ENABLED)] _PCSSEnabled ("Enable PCSS", Float) = 1
        _PCSSBlockerSearchRadius ("Blocker Search Radius", Range(0.001, 0.1)) = 0.01
        _PCSSFilterRadius ("Filter Radius", Range(0.001, 0.1)) = 0.01
        _PCSSSampleCount ("Sample Count", Range(4, 64)) = 16
        _PCSSLightSize ("Light Size", Range(0.001, 1.0)) = 0.1
        _PCSSShadowBias ("Shadow Bias", Range(0.0001, 0.01)) = 0.001
        
        // Quality Presets
        [Header(Quality Presets)]
        [Enum(Low, 0, Medium, 1, High, 2, Ultra, 3)] _PCSSQuality ("Quality Preset", Float) = 1
        
        // VRChat Expression Parameters
        [Header(VRChat Expression Control)]
        [Toggle] _VRChatExpression ("Enable VRChat Expression Control", Float) = 0
        _PCSSToggleParam ("PCSS Toggle Parameter", Float) = 1
        _PCSSQualityParam ("PCSS Quality Parameter", Float) = 1
        
        // VRC Light Volumes Integration
        [Header(VRC Light Volumes Integration)]
        [Toggle] _UseVRCLightVolumes ("Use VRC Light Volumes", Float) = 1
        _VRCLightVolumeIntensity ("Light Volume Intensity", Range(0.0, 2.0)) = 1.0
        _VRCLightVolumeTint ("Light Volume Tint", Color) = (1,1,1,1)
        _VRCLightVolumeDistanceFactor ("Distance Factor", Range(0.0, 1.0)) = 1.0
        
        // Poiyomi Properties
        [Header(Poiyomi Base)]
        _MainTex_ST ("Main Texture Scale Transform", Vector) = (1,1,0,0)
        _AlphaMask ("Alpha Mask", 2D) = "white" {}
        _AlphaMask_ST ("Alpha Mask Scale Transform", Vector) = (1,1,0,0)
        
        // Shading
        [Header(Shading)]
        _ShadowStrength ("Shadow Strength", Range(0, 1)) = 1
        _ShadowOffset ("Shadow Offset", Range(-1, 1)) = 0
        
        // Rim Lighting  
        [Header(Rim Lighting)]
        [Toggle] _EnableRimLighting ("Enable Rim Lighting", Float) = 0
        _RimLightColor ("Rim Light Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0.1, 10)) = 4
        _RimStrength ("Rim Strength", Range(0, 5)) = 1
        
        // Outline
        [Header(Outline)]
        [Toggle] _EnableOutline ("Enable Outline", Float) = 0
        _OutlineWidth ("Outline Width", Range(0, 10)) = 1
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        
        // Rendering
        [Header(Rendering)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Source Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Destination Blend", Float) = 0
        [Enum(Off, 0, On, 1)] _ZWrite ("ZWrite", Float) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4
        
        // Stencil
        [Header(Stencil)]
        [IntRange] _StencilRef ("Stencil Reference", Range(0, 255)) = 0
        [IntRange] _StencilReadMask ("Stencil ReadMask", Range(0, 255)) = 255
        [IntRange] _StencilWriteMask ("Stencil WriteMask", Range(0, 255)) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Compare", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilOp ("Stencil Op", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilFail ("Stencil Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilZFail ("Stencil ZFail", Float) = 0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "Queue"="Geometry"
            "IgnoreProjector"="True"
            "VRCFallback"="Toon"
        }
        
        Stencil
        {
            Ref [_StencilRef]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
            Comp [_StencilComp]
            Pass [_StencilOp]
            Fail [_StencilFail]
            ZFail [_StencilZFail]
        }
        
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }
            
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Cull [_Cull]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile _ SHADOWS_SCREEN
            
            #pragma shader_feature PCSS_ENABLED
            #pragma shader_feature _VRCHAT_EXPRESSION
            #pragma shader_feature _USEVRCLIGHT_VOLUMES_ON
            
            // VRC Light Volumes統合キーワード
            #pragma multi_compile _ VRC_LIGHT_VOLUMES_ENABLED
            #pragma multi_compile _ VRC_LIGHT_VOLUMES_MOBILE
            
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            
            // PCSS関連の関数群は外部インクルードファイルに委譲
            #include "Includes/lil_pcss_common.hlsl"
            #include "Includes/lil_pcss_shadows.hlsl"
            
            // Textures
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _AlphaMask;
            float4 _AlphaMask_ST;
            sampler2D _ShadowMapTexture;
            
            // Main Properties
            fixed4 _Color;
            float _Cutoff;
            
            // PCSS Properties
            float _PCSSEnabled;
            float _PCSSBlockerSearchRadius;
            float _PCSSFilterRadius;
            float _PCSSSampleCount;
            float _PCSSLightSize;
            float _PCSSShadowBias;
            float _PCSSQuality;
            
            // VRChat Expression
            float _VRChatExpression;
            float _PCSSToggleParam;
            float _PCSSQualityParam;
            
            // VRC Light Volumes関連の変数
            float _UseVRCLightVolumes;
            float _VRCLightVolumeIntensity;
            float4 _VRCLightVolumeTint;
            float _VRCLightVolumeDistanceFactor;
            sampler3D _VRCLightVolumeTexture;
            float4 _VRCLightVolumeParams;
            float4x4 _VRCLightVolumeWorldToLocal;
            
            // Poiyomi Properties
            float _ShadowStrength;
            float _ShadowOffset;
            
            // Rim Lighting
            float _EnableRimLighting;
            fixed4 _RimLightColor;
            float _RimPower;
            float _RimStrength;
            
            // Outline
            float _EnableOutline;
            float _OutlineWidth;
            fixed4 _OutlineColor;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                SHADOW_COORDS(4)
                UNITY_FOG_COORDS(5)
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                
                UNITY_TRANSFER_SHADOW(o, v.uv);
                UNITY_TRANSFER_FOG(o, o.pos);
                
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                // Base color
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // Alpha mask
                float alphaMask = tex2D(_AlphaMask, TRANSFORM_TEX(i.uv, _AlphaMask)).r;
                col.a *= alphaMask;
                
                // Alpha test
                clip(col.a - _Cutoff);
                
                // Lighting
                float3 worldNormal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = dot(worldNormal, lightDir);
                
                // Shadow calculation
                float shadow = 1.0;
                
                #ifdef PCSS_ENABLED
                    #ifdef SHADOWS_SCREEN
                        #ifdef LIL_PCSS_MOBILE_PLATFORM
                            shadow = PCSSMobile(i._LightCoord, i.pos.z);
                        #else
                            shadow = PCSS(i._LightCoord, i.pos.z);
                        #endif
                    #else
                        shadow = SHADOW_ATTENUATION(i);
                    #endif
                #else
                    shadow = SHADOW_ATTENUATION(i);
                #endif
                
                // Apply shadow
                float lightIntensity = saturate(NdotL);
                lightIntensity = lerp(lightIntensity, lightIntensity * shadow, _ShadowStrength);
                lightIntensity = saturate(lightIntensity + _ShadowOffset);
                
                // Toon shading
                lightIntensity = smoothstep(0.0, 0.01, lightIntensity);
                
                // Apply lighting
                col.rgb *= lightIntensity * _LightColor0.rgb + ShadeSH9(float4(worldNormal, 1.0));
                
                // Rim lighting
                if (_EnableRimLighting > 0.5)
                {
                    float rim = 1.0 - saturate(dot(i.viewDir, worldNormal));
                    rim = pow(rim, _RimPower);
                    col.rgb += _RimLightColor.rgb * rim * _RimStrength;
                }
                
                // Fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                return col;
            }
            ENDCG
        }
        
        // Shadow Caster Pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On ZTest LEqual Cull [_Cull]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            
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
            };
            
            struct v2f
            {
                V2F_SHADOW_CASTER;
                float2 uv : TEXCOORD1;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                clip(col.a - _Cutoff);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
        
        // Outline Pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "Always" }
            
            Cull Front
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            float _EnableOutline;
            float _OutlineWidth;
            fixed4 _OutlineColor;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                
                if (_EnableOutline > 0.5)
                {
                    float3 norm = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
                    float2 offset = TransformViewToProjection(norm.xy);
                    
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.pos.xy += offset * o.pos.z * _OutlineWidth * 0.01;
                }
                else
                {
                    o.pos = float4(0, 0, 0, 0);
                }
                
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
    
    Fallback "Toon/Lit"
    CustomEditor "Thry.ShaderEditor"
} 