// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/RuneMakeCardFrame_SSBackground" 
{
    Properties {
        _src_tex ("src_tex", 2D) = "black" {}
        
        _main_tex ("main_tex", 2D) = "black" {}
		_main_tex_alpha ("main_tex_alpha", 2D) = "white" {}        
        
        _node_2810 ("node_2810", Float ) = 1
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        
         _time ("time", Float ) = -0.5
    }
    
    SubShader 
    {
        Tags 
        {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        
        Pass 
        {

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            Fog {Mode Off}
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
            
//            uniform float4 _TimeEditor;
            
            uniform sampler2D _src_tex; 
            uniform float4 _src_tex_ST;
            
            uniform sampler2D _main_tex; 
            uniform float4 _main_tex_ST;
            
            uniform sampler2D _main_tex_alpha;    
            uniform float4 _main_tex_alpha_ST;     
            
            uniform float _node_2810;
            
            uniform float _time;
            
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
            
            fixed4 frag(VertexOutput i) : COLOR 
            {
//                fixed4 node_5454 = _Time + _TimeEditor;
                
//                fixed node_871_ang = (_node_2810*node_5454.g);
                fixed node_871_ang = (_node_2810*_time);
                
                fixed node_871_cos = cos(node_871_ang);
                fixed node_871_sin = sin(node_871_ang);
                
                fixed2 node_871_piv = fixed2(0.5,0.5);
                fixed2 node_871 = (mul(i.uv0-node_871_piv,fixed2x2( node_871_cos, -node_871_sin, node_871_sin, node_871_cos))+node_871_piv);
                fixed4 _src_tex_var = tex2D(_src_tex,TRANSFORM_TEX(node_871, _src_tex));
                
                fixed4 _main_tex_var = 0;
                
                _main_tex_var.rgb = tex2D(_main_tex,TRANSFORM_TEX(i.uv0, _main_tex)).rgb;
                _main_tex_var.a = tex2D(_main_tex_alpha,TRANSFORM_TEX(i.uv0, _main_tex_alpha)).g;
                
                fixed3 emissive = saturate((_main_tex_var.rgb/(1.0-_src_tex_var.rgb)));
                fixed3 finalColor = emissive;
                return fixed4(finalColor,_main_tex_var.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"    
}
