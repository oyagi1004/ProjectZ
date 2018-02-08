Shader "Custom/CustomAlpha" {
	Properties 
	{
		_MainTex ("Base (RGBA)", 2D) = "white" {}
		_Alpha ("Contrast Amount", Range(0.0,1.0)) = 1.0
	}
	SubShader 
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform float _Alpha;
			
			float4 frag (v2f_img i) : COLOR
			{
				float4 renderTex = tex2D(_MainTex, i.uv);
				
				renderTex.a = _Alpha; 
				
				return renderTex;
			}
			
			ENDCG
		}
	}
}