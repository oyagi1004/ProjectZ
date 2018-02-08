// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// unlit, vertex colour, premultiplied alpha blend

Shader "tk2d/ETC PremulVertexColor" 
{
	Properties 
	{
		_MainTex ("Base", 2D) = "black" {}
		_MainTex2 ("Effect", 2D) = "black" {}
	}


	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend One OneMinusSrcAlpha
		LOD 110
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert_vct
			#pragma fragment frag_mult 
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _MainTex2;
			float4 _MainTex_ST;

			struct vin_vct 
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f_vct
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			v2f_vct vert_vct(vin_vct v)
			{
				v2f_vct o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag_mult(v2f_vct i) : COLOR
			{
				fixed4 col = 0;
				col.rgb = tex2D(_MainTex, i.texcoord).rgb * i.color.rgb;
				col.a = tex2D(_MainTex2, i.texcoord).g * i.color.a;				
				
				return col;
			}
		
			ENDCG
		} 
	}
	

	SubShader 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off 
		//Blend One OneMinusSrcAlpha 
		Blend SrcAlpha OneMinusSrcAlpha 
		Cull Off Fog { Mode Off } 
		LOD 100
		
		BindChannels 
		{
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
			Bind "Color", color
		}

		Pass 
		{
			Lighting Off
			SetTexture [_MainTex] { combine texture * primary } 

//	       SetTexture [_MainTex2] { 
//	         Combine texture * primary
//	       } 
//
//	       SetTexture [_MainTex] { 
//	         Combine texture lerp(texture) previous 
//	       
//			} 	
	       
//	       SetTexture [_MainTex] { 
//	         combine texture lerp(texture) previous
//	       } 		       
//
//		 SetTexture [_MainTex] {combine texture}
//         SetTexture [_Mask] {combine texture, previous}
//         SetTexture[_Mask] {Combine previous  }
    
	       
		}
	}
}
