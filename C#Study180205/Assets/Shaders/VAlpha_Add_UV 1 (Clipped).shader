// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Effect/VAlpha_Add_UV 1" {
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
		Cull Off
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
      			float2 worldPos  : TEXCOORD1;
      		};
      		

      		sampler2D	_MainTex;
			float		_gAlpha;      
	        float       _gFactorU;
	        float       _gFactorV;
	        half4 		_Color;
	        
        	float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
			float2 _ClipArgs0 = float2(1000.0, 1000.0);
	        
	        
      		
      		vout vert(vin IN)
      		{
      			vout OutData;
				
				IN.texcoord.x		=	IN.texcoord.x + _gFactorU;
				IN.texcoord.y		=	IN.texcoord.y + _gFactorV;
				

				OutData.color		=	IN.color;
				OutData.UV			=	IN.texcoord;
      			OutData.Position	= 	UnityObjectToClipPos(IN.vertex);
      			OutData.worldPos = IN.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;

      			
      			return OutData;
      		}
      		
      		half4 frag(vout IN) : COLOR
      		{
      		
      			half4 vColor	=	tex2D(_MainTex, IN.UV.xy); 
      			     			
      			vColor.rgb		=	vColor.rgb * _Color.rgb;    
      			_gAlpha			=	_gAlpha * IN.color.a;      			
      			vColor = half4(vColor.rgb, _gAlpha);

      			float2 factor = (float2(1.0, 1.0) - abs(IN.worldPos)) * _ClipArgs0;
				vColor.a *= clamp( min(factor.x, factor.y), 0.0, 1.0);
				return vColor;

      			      			      			
      		}
 ENDCG
		}
	}
}
