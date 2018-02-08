// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge Beta 0.27 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.27;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|emission-45-OUT,alpha-2-A;n:type:ShaderForge.SFN_Tex2d,id:2,x:33197,y:32706,ptlb:diffuse_alpha,ptin:_diffuse_alpha,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Panner,id:7,x:33685,y:32877,spu:1,spv:1|UVIN-89-UVOUT,DIST-64-OUT;n:type:ShaderForge.SFN_Tex2d,id:44,x:33500,y:32877,ptlb:src,ptin:_src,ntxv:0,isnm:False|UVIN-7-UVOUT;n:type:ShaderForge.SFN_Blend,id:45,x:33000,y:32824,blmd:7,clmp:True|SRC-115-OUT,DST-2-RGB;n:type:ShaderForge.SFN_Time,id:54,x:33915,y:33057;n:type:ShaderForge.SFN_ValueProperty,id:63,x:33915,y:33227,ptlb:time,ptin:_time,glob:False,v1:-0.5;n:type:ShaderForge.SFN_Multiply,id:64,x:33722,y:33057|A-54-TSL,B-63-OUT;n:type:ShaderForge.SFN_TexCoord,id:89,x:33889,y:32877,uv:1;n:type:ShaderForge.SFN_Color,id:114,x:33362,y:33088,ptlb:blendcolor,ptin:_blendcolor,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:115,x:33197,y:32869|A-44-RGB,B-114-RGB;proporder:2-44-63-114;pass:END;sub:END;*/

Shader "Shader Forge/NewShader" {
    Properties {
        _diffuse_alpha ("diffuse_alpha", 2D) = "white" {}
        _src ("src", 2D) = "white" {}
        _time ("time", Float ) = -0.5
        _blendcolor ("blendcolor", Color) = (1,1,1,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers d3d11 xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _diffuse_alpha; uniform float4 _diffuse_alpha_ST;
            uniform sampler2D _src; uniform float4 _src_ST;
            uniform float _time;
            uniform float4 _blendcolor;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.uv1 = v.uv1;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_54 = _Time + _TimeEditor;
                float2 node_7 = (i.uv1.rg+(node_54.r*_time)*float2(1,1));
                float2 node_126 = i.uv0;
                float4 node_2 = tex2D(_diffuse_alpha,TRANSFORM_TEX(node_126.rg, _diffuse_alpha));
                float3 emissive = saturate((node_2.rgb/(1.0-(tex2D(_src,TRANSFORM_TEX(node_7, _src)).rgb*_blendcolor.rgb))));
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,node_2.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
