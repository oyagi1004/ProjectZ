// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Rim/Rimlight_rampL" 
{
	Properties 
	{
        _Color ("Main Color", Color) = (1,1,1,1)
		_MainTex			("Base (RGB)", 2D)						= "white" {}

        gRimColor			("Rim_Color", Color)					= (1.0, 1.0, 1.0, 1.0)		
		gRimPower			("Rim_Power", float)					= 1.0 
   	    gRimThickness		("Rim_Thickness", float)				= 1.0 

		_RampTex			("Ramp Texture", 2D) 					= "white" {}
        _SColor ("Shadow Color", Color) = (1,1,1,1)
		_LColor ("Highlight Color", Color) = (1,1,1,1)
	}

	SubShader 
	{
		Tags {		"RenderType"="Opaque" "IgnoreProjector" = "True"}

			LOD 100
			Lighting Off
			ZWrite On
			Offset 0, -1
		
		Pass 
		{
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
      		
      		struct vin
      		{
      			float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
      		};
      		
      		struct vout
      		{
      			float4 Position	: SV_POSITION; 
      			float2 UV		: TEXCOORD0;
				float  vRim		: TEXCOORD1;
				float3 vNormal	: TEXCOORD2;
				float3 difLight	: TEXCOORD3;		
				
			};

      		sampler2D	_MainTex;
      		sampler2D	_RampTex;
      		float		gRimPower;
			float 		gRimThickness;	
      		float4		gRimColor;
			float4 _Color;			
			float4 _SColor;
			float4 _LColor;

      		vout vert(vin IN)
			{
				vout OutData;

				OutData.UV			=	IN.texcoord.xy;
				
				float4 Pw			=	UnityObjectToClipPos(IN.vertex);
				OutData.Position	= 	Pw;
				
				fixed3 Normal		=   normalize(IN.normal);
				fixed3 CamPos		=   mul(unity_WorldToObject, float4(_WorldSpaceCameraPos.xyz, 1.0)).xyz;
										
				float3 viewVec		=	normalize(CamPos);
				
				float3 LightDir = mul(unity_WorldToObject, _WorldSpaceLightPos0).xyz;
				
				float3 difLight = dot(Normal, LightDir);

				OutData.difLight		=	difLight;	
				
				float3 rimL = dot(Normal, viewVec);

				OutData.vRim		=	1.0 - rimL;

				return OutData;
			}
      		
			half4 frag(vout IN) : COLOR
			{
				float4 DiffuseColor	=   tex2D(_MainTex, IN.UV.xy);

				float3 rampT = 	tex2D(_RampTex, IN.difLight);

				float4 rampL = lerp(_SColor,_LColor,rampT);

				float3 ResultRim	=	DiffuseColor.xyz + ((gRimColor * pow(IN.vRim, gRimPower)) * gRimThickness);
								
				DiffuseColor.xyz	=	ResultRim.xyz * rampL * _Color;
				
				return half4(DiffuseColor.xyz, 1.0);
			}

 ENDCG
		}
	}
}