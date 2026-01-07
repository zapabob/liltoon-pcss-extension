Shader "lilToon/[Optional]lilToonFakeShadow"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0.2,0.2,0.2,0.8)
        _Intensity ("Intensity", Range(0.0, 1.0)) = 0.5
        _Softness ("Softness", Range(0.0, 1.0)) = 0.3
        _Offset ("Offset", Vector) = (0,0,0,0)
        _Scale ("Scale", Vector) = (1,1,1,1)
        
        // Stencil
        _StencilRef ("Stencil Reference", Range(0, 255)) = 1
        _StencilReadMask ("Stencil Read Mask", Range(0, 255)) = 255
        _StencilWriteMask ("Stencil Write Mask", Range(0, 255)) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Compare", Float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilPass ("Stencil Pass", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilFail ("Stencil Fail", Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilZFail ("Stencil ZFail", Float) = 0
        
        // Rendering
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 2
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 2
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
        [Enum(Off,0,On,1)] _ZWrite ("ZWrite", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "LightMode"="ForwardBase"
        }
        LOD 100
        
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
            Name "FAKE_SHADOW"
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
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            
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
                UNITY_FOG_COORDS(3)
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Intensity;
            float _Softness;
            float4 _Offset;
            float4 _Scale;
            
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                // オフセットとスケールを適用
                float3 offsetPos = v.vertex.xyz + _Offset.xyz;
                offsetPos *= _Scale.xyz;
                
                o.pos = UnityObjectToClipPos(float4(offsetPos, v.vertex.w));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, float4(offsetPos, v.vertex.w)).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // メインテクスチャ
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // ライト方向
                float3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                
                // 法線とライトの角度
                float normalDotLight = max(0.0, dot(i.worldNormal, worldLightDir));
                
                // 影の計算
                float shadow = 1.0 - normalDotLight;
                shadow = pow(shadow, 1.0 + _Softness * 2.0);
                
                // 強度を適用
                shadow *= _Intensity;
                
                // 最終的な色
                col *= _Color;
                col.a *= shadow;
                
                // フォグ
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                return col;
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
    CustomEditor "lilToon.PCSS.Editor.LilToonFakeShadowShaderGUI"
}
