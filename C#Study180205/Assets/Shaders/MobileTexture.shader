

Shader "Phantom/Texture" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_DamageColor("Damage Color", Vector) = (0,0,0,0)
		[PhantomTeam]_TeamColor("Team Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader{
	Tags{ "RenderType" = "Opaque" }
	LOD 200

	CGPROGRAM
#pragma surface surf MobileTexture noforwardadd

	inline fixed4 LightingMobileTexture(SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
	{
		fixed4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * atten;
		UNITY_OPAQUE_ALPHA(c.a);
		return c;
	}

	sampler2D	_MainTex;
	fixed4		_TeamColor;
	fixed4		_DamageColor;

	struct Input {
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		fixed3 texresult = tex.a * tex.rgb;	//Mask 없을때
		texresult += (1 - tex.a) * tex.rgb * _TeamColor;		//Mask 있을때
		o.Albedo = texresult.rgb + _DamageColor.rgb;
		o.Alpha = tex.a;
	}

	ENDCG
	}

	Fallback "Mobile/VertexLit"
}
