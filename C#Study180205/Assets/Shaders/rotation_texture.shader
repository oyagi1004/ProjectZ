// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.02 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.02;sub:START;pass:START;ps:flbk:,lico:0,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:1,fgrn:0,fgrf:343.05,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:8521,x:33514,y:32396,varname:node_8521,prsc:2|emission-455-OUT,alpha-8020-A;n:type:ShaderForge.SFN_Blend,id:455,x:33210,y:32423,varname:node_455,prsc:2,blmd:7,clmp:True|SRC-497-RGB,DST-8020-RGB;n:type:ShaderForge.SFN_Tex2d,id:497,x:32991,y:32511,ptovrint:False,ptlb:src_tex,ptin:_src_tex,varname:node_1993,prsc:2,tex:53117e88006f5d24f99d1e24313e3059,ntxv:2,isnm:False|UVIN-871-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:8020,x:32991,y:32284,ptovrint:False,ptlb:main_tex,ptin:_main_tex,varname:node_1993,prsc:2,tex:4e8bacebfa251e848841347bcc64a742,ntxv:2,isnm:False|UVIN-6524-UVOUT;n:type:ShaderForge.SFN_Rotator,id:871,x:32790,y:32498,varname:node_871,prsc:2|UVIN-3778-UVOUT,ANG-5301-OUT;n:type:ShaderForge.SFN_Multiply,id:5301,x:32641,y:32719,varname:node_5301,prsc:2|A-2093-OUT,B-5454-T;n:type:ShaderForge.SFN_ValueProperty,id:2093,x:32410,y:32675,ptovrint:False,ptlb:node_2810,ptin:_node_2810,varname:node_2810,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Time,id:5454,x:32410,y:32739,varname:node_5454,prsc:2;n:type:ShaderForge.SFN_TexCoord,id:6524,x:32768,y:32263,varname:node_6524,prsc:2,uv:0;n:type:ShaderForge.SFN_TexCoord,id:3778,x:32604,y:32498,varname:node_3778,prsc:2,uv:0;proporder:497-8020-2093;pass:END;sub:END;*/

Shader "Shader Forge/panning_texture" {
    Properties {
        _src_tex ("src_tex", 2D) = "black" {}
        _main_tex ("main_tex", 2D) = "black" {}
        _node_2810 ("node_2810", Float ) = 1
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
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers d3d11 xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _src_tex; uniform float4 _src_tex_ST;
            uniform sampler2D _main_tex; uniform float4 _main_tex_ST;
            uniform float _node_2810;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:
                float4 node_5454 = _Time + _TimeEditor;
                float node_871_ang = (_node_2810*node_5454.g);
                float node_871_spd = 1.0;
                float node_871_cos = cos(node_871_spd*node_871_ang);
                float node_871_sin = sin(node_871_spd*node_871_ang);
                float2 node_871_piv = float2(0.5,0.5);
                float2 node_871 = (mul(i.uv0-node_871_piv,float2x2( node_871_cos, -node_871_sin, node_871_sin, node_871_cos))+node_871_piv);
                float4 _src_tex_var = tex2D(_src_tex,TRANSFORM_TEX(node_871, _src_tex));
                float4 _main_tex_var = tex2D(_main_tex,TRANSFORM_TEX(i.uv0, _main_tex));
                float3 emissive = saturate((_main_tex_var.rgb/(1.0-_src_tex_var.rgb)));
                float3 finalColor = emissive;
                return fixed4(finalColor,_main_tex_var.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
