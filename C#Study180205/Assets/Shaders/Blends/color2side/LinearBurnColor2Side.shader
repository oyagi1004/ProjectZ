Shader "Custom/Blends/2Side/LinearBurnColor2Side" {
	Properties {

		_Color1 ("Main Color", Color) = (1,1,1,1)		
	
		_TexA ("Base", 2D) = "white" {}
		_TexB ("Effect", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
			Lighting Off Fog { Mode Off }
			Blend Off
			Cull Off

			CGPROGRAM
			
			#include "Assets/shaders/Blends/_Includes/Aub_Blends.cginc"
			
			
			#pragma vertex vert_uv0
			#pragma fragment frag

			sampler2D _TexA, _TexB;

			fixed4 frag( v2f_uv0 i ) : COLOR {
				fixed4 a = tex2D(_TexA, i.uv);
				fixed4 b = tex2D(_TexB, i.uv2);
				return LinearBurn(a, b) * _Color1;
			}
			ENDCG
		}
	}
}