// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effect/Twoside_Blended_Z" {
	Properties 
	{
		_MainTex ("Texture", 2D) = "white" {}
		_gAlpha  ("Alpha", Float) = 0.0
		_Color	 ("Main Color", Color) = (1,1,1,1)
	}

	SubShader 
	{
		Tags { "Queue"="Transparent"}
		Lighting Off
		ZWrite off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass 
		{
CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
      		
      		struct vin
      		{
      			float4 vertex    : POSITION;
      			float4 texcoord	 : TEXCOORD0;
      		};
      		
      		struct vout
      		{
      			float4 Position  : SV_POSITION; 
      			float4 UV		 : TEXCOORD0;
      		};
      		
			float		_gAlpha;
      		sampler2D	_MainTex;
			half4 _Color;
			
      		
      		vout vert(vin IN)
      		{
      			vout OutData;
      			
				OutData.UV			=	IN.texcoord;
				
      			OutData.Position	= 	UnityObjectToClipPos(IN.vertex);
      			
      			return OutData;
      		}
      		
      		half4 frag(vout IN) : COLOR
      		{
      			half4 vColor	=	tex2D(_MainTex, IN.UV.xy);

    			vColor =  vColor * _gAlpha * _Color; 
			
      			return half4(vColor.rgba);
      			
      		}

 ENDCG
		}
	}
}
