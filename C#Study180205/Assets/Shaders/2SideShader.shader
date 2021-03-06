Shader "Custom/2SideFlag" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }

		Lighting Off

		//LOD 200
		
		Cull Back
		
		CGPROGRAM
		#pragma surface surf Lambert
		#pragma debug

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
		
		Cull Front
		
		CGPROGRAM
		#pragma surface surf Lambert
		
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Normal = fixed3(1, 1, -1);
		}
		ENDCG
		
	} 
	FallBack "Diffuse"
}
