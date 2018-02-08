// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// UnoShader 참고

Shader "Phantom/TextureBRDF(Unit)" {
	Properties{
		_MainTex("Base (RGB) Team Mask (A)", 2D) = "white" {}
		_BRDF("BRDF Light (R) Ambient (G) Fresnel (B)", 2D) = "white" {}
		_DamageColor("Damage Color", Vector) = (0,0,0,0)
		[PhantomTeam]_TeamColor("Team Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader{
	Tags{ "RenderType" = "Opaque" }
	Stencil
	{
		Ref 255
		Comp NotEqual
		Pass Keep
	}
	LOD 200

	CGPROGRAM
#pragma surface surf MobileTexture noforwardadd vertex:vert

	sampler2D	_MainTex;
	sampler2D	_BRDF;
	half		_Modulate;
	fixed4		_TeamColor;
	fixed4		_DamageColor;
	half		_diff;

	inline fixed4 LightingMobileTexture(SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
	{
		fixed4 c;
		c.rgb = s.Albedo *_diff *_LightColor0.rgb * atten;
		UNITY_OPAQUE_ALPHA(c.a);
		return c;
	}

	struct Input {
		half2 uv_MainTex;
		fixed2 brdfUV;
	};

	void vert(inout appdata_full v, out Input o)
	{
		UNITY_INITIALIZE_OUTPUT(Input, o);

		float3 wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		float3 wNorm = UnityObjectToWorldNormal(v.normal);
		float3 wView = normalize(UnityWorldSpaceViewDir(wPos));
		float3 wLight = UnityWorldSpaceLightDir(wPos);
		fixed u = clamp((dot(wNorm, wView)) * 1.2 - .2, 0.01, .99);
		o.brdfUV = fixed2(u, dot(wNorm, wLight)*0.5f + 0.5f);
	}

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		fixed3 texresult = tex.a * tex.rgb;	//Mask 없을때
		texresult += (1 - tex.a) * tex.rgb * _TeamColor;		//Mask 있을때
		fixed4 TBRDF = tex2D(_BRDF, IN.brdfUV);
		_diff = TBRDF.r + TBRDF.g;

		o.Albedo = texresult + _DamageColor.rgb;

		o.Alpha = tex.a;
	}

	ENDCG
	}

	Fallback "Mobile/VertexLit"
}
