// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effect/TexMul_Add_UV" {
	Properties 
	{
		_Color			("Main Color", Color)	= (1,1,1,1)		
		_MainTex 		("Texture", 2D) 		= "white" {}
		_SubTex 		("Texture", 2D) 		= "white" {}
		_gAlpha 			("Alpha", float) 		= 1.0

		
		_gFactorU("gFactorU", float) = 0.0
        _gFactorV("gFactorV", float) = 0.0
	
		_sFactorU("subTexU", float) = 0.0
        _sFactorV("subTexV", float) = 0.0		
        
        _pow("Pow", float) = 1.0
        


        
		
	}

	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off
		Cull Off
		Blend SrcAlpha One
		ZWrite Off

		Pass 
		{
CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
      		
      		struct vin
      		{
      		float4 vertex   	: POSITION;
			fixed4 color		: COLOR;
      		float4 texcoord1	: TEXCOORD0;
      		float4 texcoord2	: TEXCOORD1;

      		};
      		
      		struct vout
      		{
      		float4 Position		: SV_POSITION; 
			fixed4 color		: COLOR;
      		float4 UV1	 		: TEXCOORD0;
      		float4 UV2	 		: TEXCOORD1;

      		};

      		sampler2D	_MainTex;      		
       		sampler2D	_SubTex;
       		     		      		      		
			fixed		_gAlpha;


      		fixed4		_MainTex_ST;
      		half4		_Color;

      		fixed4		_SubTex_ST;
      		
	        fixed       _gFactorU;
	        fixed       _gFactorV;
	        
	        fixed       _sFactorU;
	        fixed       _sFactorV;	
  	        fixed       _pow;     

		
      		
      		vout vert(vin IN)
      		{
      		vout OutData;
      		

			
			
			OutData.UV1.x		=	(IN.texcoord1.x  * _MainTex_ST.x) + _gFactorU;
			OutData.UV1.y		=	(IN.texcoord1.y  * _MainTex_ST.y) + _gFactorV;  
			

			OutData.UV2.x		=	(IN.texcoord2.x  * _SubTex_ST.x) + _sFactorU;
			OutData.UV2.y		=	(IN.texcoord2.y  * _SubTex_ST.y) + _sFactorV;  			
	
		
					
			OutData.color		=	IN.color;
      		OutData.Position	= 	UnityObjectToClipPos(IN.vertex);
      			
      		return OutData;
      		}
      		
      		half4 frag(vout IN) : COLOR
      		{
      		half4 vColor	=	tex2D(_MainTex, IN.UV1.xy);
      		half4 sColor	=	tex2D(_SubTex, IN.UV2.xy);
      		

      		
// 			vColor			=	(vColor * sColor) * IN.color * _gAlpha * _Color;
			vColor			=	pow(((vColor * sColor) * IN.color), _pow )* _gAlpha * _Color;
			//vColor			=	pow((vColor  * IN.color), _pow)* _gAlpha * _Color;
			
			
//			vColor			=	(pow(vColor,_pow) * sColor) * IN.color * _gAlpha * _Color;
//			vColor			=	(vColor * pow(sColor,_pow)) * IN.color * _gAlpha * _Color;

  			return half4(vColor.rgb, _gAlpha);

      		}

 ENDCG
		}
	}
}
