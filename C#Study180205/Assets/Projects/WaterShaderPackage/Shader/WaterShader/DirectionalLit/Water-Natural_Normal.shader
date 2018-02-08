// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Water/NatuarlLit/Normal" 
{
	/* 
	 * Created by Martin Reintges
	 */

	Properties 
	{
		_AmbientMultiplier ("Ambient strength", Range(0,10)) = 1
		_NormalStrength ("Normal strength", Range(0,1)) = 1
		_NormalTex ("Normalmap", 2D) = "blue" {}

		_Shininess ("Shininess", float) = 200
		
		_Movement ("Movement", Vector) = (0.01,0.01,-0.01,0)
		_UVOffset ("Auto.gen", Vector) = (0,0,0,0)
		_EdgeFade ("Edge fade", float) = 0.5
		_ShallowColor ("Shallow color tint", Color) = (0,0,1,1)
		_DeepSeaColor ("Deep sea color tint", Color) = (0,0,1,1)
		
		_ShallowDepth ("ShallowDepth", float) = 0.5
		_ShallowDeepFade ("Shallow-Deep-Fade", float) = 0.5
		_ShoreEdgeIndicator("Water shore edge indicator", float) = 0.2
	}
	SubShader 
	{
		// Subshader Tags
		Tags { "Queue"="Transparent-1" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		
		// Render Pass
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "WaterShaderHelper.cginc"
			#pragma target 2.0
			
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#pragma vertex vert
			#pragma fragment frag
	
	
		////// Uniform user variable definition
			uniform float _AmbientMultiplier;		//
			uniform float _NormalStrength;			//
			uniform sampler2D _NormalTex;			// The main water texture (Normal+greyscale)
			uniform float4 _NormalTex_ST;			// Scale and offset of the main texture

			uniform float _Shininess;				// Control the specular spread
			
			uniform float4 _Movement; 				// The uv-offset movement of the waves xy direction, z speed
			uniform float4 _UVOffset; 				
			uniform float _EdgeFade;				// Fading from shore to shallow water
			uniform float4 _ShallowColor;			// The color factor for shallow water
			uniform float4 _DeepSeaColor;			// The color factor for deep sea
			
			uniform float _Reflectivity;
			uniform float _Refraction;
			uniform float _ShallowDepth;
			uniform float _ShallowDeepFade;
			uniform float _ShoreEdgeIndicator;
			
			// Unity variable definition
			uniform sampler2D _LastCameraDepthTexture;

	
		////// Input structs
			struct VertexInput 
			{
				float4 vertex : POSITION;
				float2 uv_NormalTex : TEXCOORD0;
			};
			struct Vert2Frag
			{
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float4 posScreen : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float4 uv : TEXCOORD3;
			};
			
			
		////// Helper functions
			// can be found in "WaterShaderHelper.cginc"
			
	
		////// Shader functions
			// Vertex shader
			Vert2Frag vert(VertexInput vertIn)
			{
				Vert2Frag output;
				
				output.pos = UnityObjectToClipPos(vertIn.vertex);
				
				output.posWorld = mul(unity_ObjectToWorld, vertIn.vertex);
				output.viewDir = WorldSpaceViewDir(vertIn.vertex);
				output.posScreen = ComputeScreenPos(output.pos);
				output.uv.xy = vertIn.uv_NormalTex * _NormalTex_ST.xy + _NormalTex_ST.zw + _UVOffset.xy;
				output.uv.zw = vertIn.uv_NormalTex * _NormalTex_ST.xy + _NormalTex_ST.zw + _UVOffset.zw;
				
				return(output);
			}
			
			// Fragent shader
			float4 frag(Vert2Frag fragIn) : SV_Target
			{				
				// Value definitions
				float3 viewDir = normalize(fragIn.viewDir);
				float3 lightDir = _WorldSpaceLightPos0;
				
				// Normal mapping
				float3 normalDir = GetNormal(_NormalTex, fragIn.uv, _NormalStrength);
				 
				// Light
				float3 diffuseLight = DiffuseLightSimple(normalDir, lightDir, _AmbientMultiplier);
				float3 specularLight = SpecularLightSimple(normalDir, lightDir, viewDir, _Shininess) * 0.5;
				
				// Depth fog
				float2 depthValues = GetDepthValues(fragIn.posScreen, _LastCameraDepthTexture, _ShallowDeepFade, _ShallowDepth, _EdgeFade);
				float depthLevel = depthValues.x;
				float waterLevel = depthValues.y;

				float3 waterDepthTint = (saturate(waterLevel*_DeepSeaColor) + saturate((1-waterLevel)*_ShallowColor));
				
				// Final composition
				float4 final = float4( (diffuseLight * waterDepthTint + specularLight),1);
				final.w = max(max(depthLevel, specularLight.x),_ShoreEdgeIndicator);

				return(final);  
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
