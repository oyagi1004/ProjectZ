Shader "Custom/Blends/Normal/Overlay" {
	Properties {

		_TexA ("Base", 2D) = "white" {}
		_TexB ("Effect", 2D) = "white" {}
	}
//	SubShader {
//		Tags { "RenderType"="Opaque" }
//		Pass {
//			Lighting Off Fog { Mode Off }
//			Blend Off
//			
//			CGPROGRAM
//			
//			#include "Assets/shaders/Blends/_Includes/Aub_BlendsNoneColor.cginc"
//			#pragma vertex vert_uv0
//			#pragma fragment frag
//
//			sampler2D _TexA, _TexB;
//
//			fixed4 frag( v2f_uv0 i ) : COLOR {
//				fixed4 a = tex2D(_TexA, i.uv);
//				fixed4 b = tex2D(_TexB, i.uv2);
//				return Overlay(a, b);
//			}
//			ENDCG
//		}
//	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }

		LOD 100

		pass
		{
		Lighting Off Fog { Mode Off }
			Blend Off
			SetTexture[_TexA] 
		}

		pass 
		{
			Lighting Off Fog { Mode Off }
			Blend DstColor SrcColor//2X Multiplicative

			BindChannels 
			{
			   Bind "texcoord1", texcoord1
			}

			SetTexture[_TexA]
			SetTexture[_TexB] 
		}
	//pass
	}	

}