// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*SF_DATA;ver:0.27;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|emission-13-OUT,alpha-2-A;n:type:ShaderForge.SFN_Tex2d,id:2,x:33279,y:32661,ptlb:Diffuse,ptin:_Diffuse,tex:67fc479261525644c934bf1a1aa53932,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:7,x:33279,y:32881,ptlb:Color,ptin:_Color,glob:False,c1:0.4980392,c2:0.4980392,c3:0.4980392,c4:1;n:type:ShaderForge.SFN_Blend,id:13,x:33026,y:32675,blmd:10,clmp:True|SRC-2-RGB,DST-7-RGB;proporder:2-7;pass:END;sub:END;*/

Shader "Custom/Unlit_Light_ETC" {
    Properties {
    
        _Diffuse ("Diffuse", 2D) = "white" {}
        
        _MainTex2 ("MainTex2", 2D) = "white" {}
        
        _Color ("Color", Color) = (0.4980392,0.4980392,0.4980392,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    
    SubShader {

        
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        
        Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha          
        
        Pass {
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			
            uniform sampler2D _Diffuse; 
            uniform float4 _Diffuse_ST;
            
            uniform sampler2D _MainTex2; 
            uniform float4 _MainTex2_ST;            
            
            uniform float4 _Color;
            
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                fixed4 node_2 = 0;
                
                node_2.rgb = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse)).rgb;
                node_2.a = tex2D(_MainTex2, TRANSFORM_TEX(i.uv0, _MainTex2)).g;                
                
                fixed3 emissive = saturate(( _Color.rgb > 0.5 ? (1.0-(1.0-2.0*(_Color.rgb-0.5))*(1.0-node_2.rgb)) : (2.0*_Color.rgb*node_2.rgb) ));
                fixed3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,node_2.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
