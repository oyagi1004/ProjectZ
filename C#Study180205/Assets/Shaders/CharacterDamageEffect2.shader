Shader "Custom/CharacterDamageEffect2" {
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)	
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BrightnessAmount ("Brightness Amount", Range(0.0, 4.0)) = 5.0
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		Pass
		{
			Lighting Off Fog { Mode Off }
		
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform fixed _BrightnessAmount;
			uniform fixed3 _Color;
			
			
//#if UNITY_ANDROID			
//			
//			fixed3 ContrastSaturationBrightness( fixed3 color, fixed3 applyColor, fixed brt)
//			{
//				return color * brt * applyColor;
//			}
//			
//
//			
//			fixed4 frag (v2f_img i) : COLOR
//			{
//				fixed4 renderTex = tex2D(_MainTex, i.uv) ;
//				renderTex.rgb = ContrastSaturationBrightness(renderTex.rgb, _Color, _BrightnessAmount);
//				return renderTex;
//			}
//			
//#else

			half3 ContrastSaturationBrightness( half3 color, fixed3 applyColor, fixed brt)
			{
				return color * brt * applyColor;
			}
			

			
			half4 frag (v2f_img i) : COLOR
			{
				half4 renderTex = tex2D(_MainTex, i.uv) ;
				renderTex.rgb = ContrastSaturationBrightness(renderTex.rgb, _Color, _BrightnessAmount);
				return renderTex;
			}

//#endif
						
			
			
			ENDCG
		}
		
	}
}