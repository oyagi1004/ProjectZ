// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/RuneMakeCardFrame_ETC" {
    Properties {
    
        _diffuse_alpha ("diffuse_alpha", 2D) = "white" {}
        
        _MainTex2 ("MainTex2", 2D) = "white" {}
        
        _src ("src", 2D) = "white" {}
        
        _time ("time", Float ) = -0.5
        
        _blendcolor ("blendcolor", Color) = (1,1,1,1)
    }
    
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent+290"
            "RenderType"="Transparent"
        }
        
        ZWrite On Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha   
        
        Pass {
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
            
            uniform sampler2D _diffuse_alpha;             
            uniform float4 _diffuse_alpha_ST;
            
            uniform sampler2D _MainTex2; 
            uniform float4 _MainTex2_ST;
            
            uniform sampler2D _src; 
            uniform float4 _src_ST;
            
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
            
            
            fixed4 frag(VertexOutput i) : COLOR 
            {
                fixed2 node_7 = (i.uv1 + (_time) * fixed2(1,1) );
                
                fixed4 node_2 = 0;
                
                node_2.rgb = tex2D(_diffuse_alpha, TRANSFORM_TEX( i.uv0, _diffuse_alpha)).rgb;                
                node_2.a = tex2D(_MainTex2, TRANSFORM_TEX( i.uv0, _MainTex2)).g;
                
                
                fixed3 emissive = saturate((node_2.rgb/(1.0-(tex2D(_src,TRANSFORM_TEX(node_7, _src)).rgb*_blendcolor.rgb))));
                
                fixed3 finalColor = emissive;
                
                return fixed4(finalColor,node_2.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"

}




