

Shader "Mobile/UnlitRim" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_RimColor("Rim Color", Color) = (1,1,1,1)
			_RimFresnel("Rim Fresnel", Range(0, 5)) = 1
		_RimIntensity("Rim Intensity", Range(1, 10)) = 1
	}
	SubShader{
	Tags{ "RenderType" = "Opaque" }
	LOD 200

	CGPROGRAM
#pragma surface surf MobileTexture noforwardadd noambient

	inline fixed4 LightingMobileTexture(SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
	{
		fixed4 c;
		c.rgb = s.Albedo /** _LightColor0.rgb*/ * atten;
		UNITY_OPAQUE_ALPHA(c.a);
		return c;
	}

	sampler2D	_MainTex;
	fixed4 _RimColor;
	fixed _RimFresnel;
	fixed _RimIntensity;

	struct Input {
		float2 uv_MainTex;
		float3 viewDir;
		float3 worldNormal;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

		half rim = 1.0 - saturate(dot(normalize(IN.viewDir), IN.worldNormal));
		fixed3 rimcolor = _RimColor.rgb * pow(rim, _RimFresnel) * _RimIntensity;

		o.Albedo = lerp(tex.rgb, rimcolor, _RimColor.a );// tex.rgb;
		o.Alpha = tex.a;
	}

	ENDCG
	}

	Fallback "Mobile/VertexLit"
}
