Shader "Custom/SimpleSaturationShader" {
	Properties 
	{
	
		//238, 91,25 <= damage color
		_Color ("Main Color", Color) = (1,1,1,1)	
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SaturationAmount ("Saturation Amount", Range(0.0, 8.0)) = 1.2
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
			
			uniform fixed _SaturationAmount;
			
			uniform fixed3 _Color;
			
			
			fixed3 Saturation( fixed3 color, fixed3 applyColor, fixed sat)
			{
				//Luminace Coefficients for brightness of image
				fixed3 LuminaceCoeff = fixed3(0.2125,0.7154,0.0721);
				
				fixed3 brtColor = color ;
				fixed intensityf = dot(brtColor, LuminaceCoeff);
				fixed3 intensity = fixed3(intensityf, intensityf, intensityf);
				
				//Saturation calculation
				fixed3 satColor = lerp(intensity, brtColor, sat);
				
				return satColor * applyColor;
			}
			
			
			fixed4 frag (v2f_img i) : COLOR
			{
				fixed4 renderTex = tex2D(_MainTex, i.uv) ;
				
				renderTex.rgb = ContrastSaturationBrightness(renderTex.rgb, _Color, _SaturationAmount);
				
				return renderTex;
			
			}
			
			ENDCG
		}
		
	}
}