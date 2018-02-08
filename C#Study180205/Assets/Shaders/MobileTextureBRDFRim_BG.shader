// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// UnoShader 참고

Shader "Phantom/TextureBRDFRim(BG)" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_BRDF("BRDF Light (R) Ambient (G) Fresnel (B)", 2D) = "white" {}
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimIntensity("Rim Intensity", Range(1, 10)) = 1
	}
	SubShader{
	Tags{ "RenderType" = "Opaque" }

	LOD 200

	CGPROGRAM
#pragma surface surf MobileTexture noforwardadd vertex:vert

	sampler2D	_MainTex;
	sampler2D	_BRDF;
	half		_Modulate;
	half		_diff;
	fixed4 _RimColor;
	fixed _RimFresnel;
	fixed _RimIntensity;

	inline fixed4 LightingMobileTexture(SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
	{
		fixed4 c;
		c.rgb = s.Albedo *_diff *_LightColor0.rgb * atten;
		UNITY_OPAQUE_ALPHA(c.a);
		return c;
	}

	struct Input {
		half2 uv_MainTex;
		fixed3 brdfUVRim;
	};

	void vert(inout appdata_full v, out Input o)
	{
		UNITY_INITIALIZE_OUTPUT(Input, o);

		float3 wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		float3 wNorm = UnityObjectToWorldNormal(v.normal);
		float3 wView = normalize(UnityWorldSpaceViewDir(wPos));
		float3 wLight = UnityWorldSpaceLightDir(wPos);

		half rim = 1.0 - saturate(dot(wView, wNorm));

		fixed u = clamp((dot(wNorm, wView)) * 1.2 - .2, 0.01, .99);
		o.brdfUVRim.xy = fixed2(u, dot(wNorm, wLight)*0.5f + 0.5f);
		o.brdfUVRim.z = rim;
	}

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		fixed3 texresult = tex.rgb;
		fixed4 TBRDF = tex2D(_BRDF, IN.brdfUVRim.xy);
		_diff = TBRDF.r + TBRDF.g;

		fixed3 rimcolor = _RimColor.rgb;// *TBRDF.b;

		texresult = lerp(texresult, rimcolor * _RimIntensity, _RimColor.a * TBRDF.b);

		o.Albedo = texresult;

		o.Alpha = tex.a;
	}

	ENDCG
	}

	Fallback "Mobile/VertexLit"
}
