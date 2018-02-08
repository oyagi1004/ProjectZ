// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ETC Transparent Colored (Packed) (HardClip)"
{
	Properties
	{
		_MainTex ("Base", 2D) = "black" {}
		_MainTex2 ("Effect", 2D) = "black" {}
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Offset -1, -1
			Fog { Mode Off }
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _MainTex2;
			half4 _MainTex_ST;

			struct appdata_t
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 worldPos : TEXCOORD1;
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				o.worldPos = TRANSFORM_TEX(v.vertex.xy, _MainTex);
				return o;
			}

			half4 frag (v2f IN) : COLOR
			{
				half4 mask = 0;
				mask.rgb = tex2D(_MainTex, IN.texcoord).rgb;
				mask.a = tex2D(_MainTex2, IN.texcoord).g;

				half4 mixed = saturate(ceil(IN.color - 0.5));
				half4 col = saturate((mixed * 0.51 - IN.color) / -0.49);
				float2 factor = abs(IN.worldPos);
				
				clip(1.0 - max(factor.x, factor.y));
				mask *= mixed;
				col.a *= mask.r + mask.g + mask.b + mask.a;
				return col;
			}
			ENDCG
		}
	}
	Fallback Off
}