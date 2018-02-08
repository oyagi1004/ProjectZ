Shader "Custom/Blends/Vertex/HardLightVertexColor" {
	Properties {

		_Color1 ("Base Color", Color) = (1,1,1,1)		
		_Color2 ("Effect Color", Color) = (1,1,1,1)
	
		_TexA ("Base", 2D) = "white" {}
		_TexB ("Effect", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
			Lighting Off Fog { Mode Off }
			
			CGPROGRAM
			
			#include "Assets/shaders/Blends/_Includes/Aub_BlendsVertexColor.cginc"
			#pragma vertex vert_uv0
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			sampler2D _TexA, _TexB;

			fixed4 frag( v2f_uv0 i ) : COLOR {
				fixed4 a = tex2D(_TexA, i.uv) * i.color; //* _Color1
				fixed4 b = tex2D(_TexB, i.uv2) * i.color; //* _Color2 
				return HardLight(a, b);
			}
			ENDCG
		}
	}
}