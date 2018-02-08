// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effect/VAlpha_Add_UV_Oneside" {
	Properties 
	{
		_MainTex ("Texture", 2D) = "white" {}  
        _gFactorU("gFactorU", Float) = 0.0
        _gFactorV("gFactorV", Float) = 0.0
   		_gAlpha ("Alpha", Float) = 0.5
		_Color	("Main Color", Color) = (1,1,1,1)    
      
	}

	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off
		Cull Back
		Blend SrcAlpha One
		ZWrite Off
		//Fog off

		Pass 
		{
CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
      		
      		struct vin
      		{
      			float4 vertex    : POSITION;
				fixed4 color	 : COLOR;
      			float4 texcoord	 : TEXCOORD0;
      		};
      		
      		struct vout
      		{
      			float4 Position  : SV_POSITION; 
				fixed4 color	 : COLOR;	
      			float4 UV		 : TEXCOORD0;
      		};
      		

      		sampler2D	_MainTex;
			float		_gAlpha;      
	        float       _gFactorU;
	        float       _gFactorV;
	        half4 		_Color;
        
	        
	        
      		
      		vout vert(vin IN)
      		{
      			vout OutData;
				
				IN.texcoord.x		=	IN.texcoord.x + _gFactorU;
				IN.texcoord.y		=	IN.texcoord.y + _gFactorV;
				

				OutData.color		=	IN.color;
				OutData.UV			=	IN.texcoord;
      			OutData.Position	= 	UnityObjectToClipPos(IN.vertex);

      			
      			return OutData;
      		}
      		
      		half4 frag(vout IN) : COLOR
      		{
      			half4 vColor	=	tex2D(_MainTex, IN.UV.xy); 
      			     			
      			vColor.rgb		=	vColor.rgb * _Color.rgb;    
      			_gAlpha			=	_gAlpha * IN.color.a;      			
      			return half4(vColor.rgb, _gAlpha);
      		}
 ENDCG
		}
	}
}
