// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Water/Texture/Foam" 
{
	/* 
	 * Created by Martin Reintges
	 */

	Properties 
	{
		_MainTex ("Main Texture", 2D) = "black" {}
		_Transparency ("Transparency", Range(0,1)) = 0.8
		_EnvironmentTex ("Height map",2D) = "black" {}
		_FoamTex ("Foam",2D) = "black" {}
		
		_Movement ("Movement", Vector) = (0.01,0.01,-0.01,0)
		_UVOffset ("Auto.gen", Vector) = (0,0,0,0)

		_FoamScale ("Foam scale", float) = 1
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
	
	
		////// Uniform user variable definition
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float _Transparency;			//

			uniform sampler2D _EnvironmentTex;		// The texture containing environment information RGB as flowmap, A as heightmap
			uniform float4 _EnvironmentTex_ST;		// The env texture scale
			
			uniform float4 _Movement; 				// The uv-offset movement of the waves xy direction, z speed
			uniform float4 _UVOffset; 				
			
			uniform sampler2D _FoamTex;			// 
			uniform float4 _FoamTex_ST;			// 
			uniform float _FoamScale;
			uniform float _FoamDistance;

	
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
				float4 uv : TEXCOORD1;
				float4 uv_Environment : TEXCOORD2;
				float2 uv_Tex : TEXCOORD3;
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

				output.uv.xy = vertIn.uv_NormalTex * _MainTex_ST.xy + _MainTex_ST.zw + _UVOffset.xy;
				float2 heightMapUv = vertIn.uv_NormalTex * _EnvironmentTex_ST.xy + _EnvironmentTex_ST.zw;
				float2 foamUv = vertIn.uv_NormalTex * _FoamScale + _UVOffset.xy;
				output.uv_Environment = float4(heightMapUv,foamUv);
				output.uv_Tex = vertIn.uv_NormalTex * _MainTex_ST.xy + _MainTex_ST.zw + _UVOffset.xy;
				
				return(output);
			}
			
			// Fragent shader
			float4 frag(Vert2Frag fragIn) : SV_Target
			{
				// Height effects
				float groundLevel = tex2D(_EnvironmentTex, fragIn.uv_Environment.xy ).x;
				
				// Foam
				float foam = GetFoam(_FoamTex, fragIn.uv_Environment.xy, groundLevel, _FoamDistance);
				
				// Final composition
				float4 final = float4( tex2D(_MainTex, fragIn.uv_Tex.xy).xyz + foam, _Transparency);

				return(final);  
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
