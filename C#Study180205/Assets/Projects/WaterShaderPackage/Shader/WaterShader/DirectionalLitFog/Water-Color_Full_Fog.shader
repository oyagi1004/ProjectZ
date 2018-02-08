// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Water/Fog/ColorLitFull" 
{
	/* 
	 * Created by Martin Reintges
	 */

	Properties 
	{
		_AmbientMultiplier ("Ambient strength", Range(0,10)) = 1
		_Transparency ("Transparency", Range(0,1)) = 0.8
		_NormalStrength ("Normal strength", Range(0,1)) = 1
		_NormalTex ("Normalmap", 2D) = "blue" {}
		_EnvironmentTex ("Height map",2D) = "black" {}
		_FoamTex ("Foam",2D) = "black" {}

		_Shininess ("Shininess", float) = 200
		
		_Movement ("Movement", Vector) = (0.01,0.01,-0.01,0)
		_UVOffset ("Auto.gen", Vector) = (0,0,0,0)
		_ShallowColor ("Shallow color tint", Color) = (0,0,1,1)
		_DeepSeaColor ("Deep sea color tint", Color) = (0,0,1,1)
		
		_ReflectionTex ("Reflection", 2D) = "black" {}
		_Reflectivity ("Reflectivity", float) = 0
		_Refraction ("Refraction", float) = 2

		_FoamScale ("Foam scale", float) = 1
		_FoamRefraction ("Foam refraction", float) = 0.1
		_FoamDistance ("Foam distance", Range(0.001,1)) = 0.1
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
			#pragma multi_compile_fog
	
	
		////// Uniform user variable definition
			uniform float _AmbientMultiplier;		//
			uniform float _Transparency;			//
			uniform float _NormalStrength;			//
			uniform sampler2D _NormalTex;			// The main water texture (Normal+greyscale)
			uniform float4 _NormalTex_ST;			// Scale and offset of the main texture
			
			uniform sampler2D _EnvironmentTex;		// The texture containing environment information RGB as flowmap, A as heightmap
			uniform float4 _EnvironmentTex_ST;		// The env texture scale

			uniform float _Shininess;				// Control the specular spread
			
			uniform float4 _Movement; 				// The uv-offset movement of the waves xy direction, z speed
			uniform float4 _UVOffset; 				
			uniform float4 _ShallowColor;			// The color factor for shallow water
			uniform float4 _DeepSeaColor;			// The color factor for deep sea
			
			uniform float _Reflectivity;
			uniform float _Refraction;
			
			uniform sampler2D _FoamTex;			// 
			uniform float _FoamScale;
			uniform float _FoamRefraction;
			uniform float _FoamDistance;
			
			// Auto generated reflection texture
			uniform sampler2D _ReflectionTex;

	
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
				float4 uv_Environment : TEXCOORD4;
				UNITY_FOG_COORDS(5)
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
				float2 heightMapUv = vertIn.uv_NormalTex * _EnvironmentTex_ST.xy + _EnvironmentTex_ST.zw;
				float2 foamUv = vertIn.uv_NormalTex * _FoamScale + _UVOffset.xy;
				output.uv_Environment = float4(heightMapUv,foamUv);
				UNITY_TRANSFER_FOG(output,output.pos);
				
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
				
				// Height effects
				float groundLevel = tex2D(_EnvironmentTex, fragIn.uv_Environment.xy + normalDir.xz*_FoamRefraction ).x;
				float3 waterDepthTint = (saturate(groundLevel*_DeepSeaColor) + saturate((1-groundLevel)*_ShallowColor));
				
				// Reflection
				float reflectivity = GetReflectivity(viewDir, _Reflectivity);
				float4 reflection = GetReflection(fragIn.posScreen, _ReflectionTex, normalDir, _Refraction) * reflectivity;
				
				// Foam
				float foam = GetFoam(_FoamTex, fragIn.uv_Environment.xy, groundLevel, _FoamDistance);
				
				// Final composition
				float4 final = float4( (diffuseLight * waterDepthTint + specularLight)*(1-reflectivity) + foam,1) + reflection;
				UNITY_APPLY_FOG(fragIn.fogCoord, final);
				final.w = _Transparency;

				return(final);  
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
