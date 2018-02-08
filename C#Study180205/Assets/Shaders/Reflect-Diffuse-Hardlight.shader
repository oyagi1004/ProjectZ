Shader "Custom/Reflective/DiffuseLambert" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125		
		_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
		_MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {} 
		_HardLight ("HardLight (RGB)", 2D) = "white" {}
		_Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
	}
	
	SubShader 
	{
			LOD 200
			Tags { "RenderType"="Opaque" }
			
			Lighting Off Fog { Mode Off }
			Blend Off		
		
			CGPROGRAM
			#pragma surface surf Lambert

			struct Input 
			{
				float2 uv_MainTex : TEXCOORD0;
				float2 uv2_HardLight : TEXCOORD1;
				float3 worldRefl;
			};

			sampler2D _MainTex;
			sampler2D _HardLight;
			samplerCUBE _Cube;

			fixed4 _Color;
			fixed4 _ReflectColor;

			half _Shininess;

			fixed4 HardLight (fixed4 a, fixed4 b) {

			    fixed4 r = fixed4(1,1,1,1);
				
			    r = (b < 0.5) ? (2 * b * a):(1.0 - 2 * (1.0 - b) * (1.0 - a));			    
			    return r;
			}	

			void surf (Input IN, inout SurfaceOutput o) 
			{
				fixed4 tex = tex2D(_MainTex, IN.uv_MainTex.xy);
				fixed4 tex2 = tex2D(_HardLight, IN.uv2_HardLight.xy);
				
				fixed4 h = HardLight(tex , tex2);
				fixed4 c = h * _Color;
				o.Albedo = c.rgb;
				o.Gloss = h.a;
				o.Specular = _Shininess;
				
				fixed4 reflcol = texCUBE (_Cube, IN.worldRefl);
				reflcol *= h.a;
				o.Emission = reflcol.rgb * _ReflectColor.rgb;// c.rgb;// *
				//o.Alpha = _Color.a;
				o.Alpha = reflcol.a * _ReflectColor.a;
			}
		ENDCG

	}
	
	FallBack "Reflective/VertexLit"
} 