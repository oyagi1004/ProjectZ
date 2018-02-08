// Free MatCap Shaders 에셋스토어 참고

Shader "Phantom/TextureMatCap_addmin" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_MaskTex("Mask Team(R) Gloss(G)", 2D) = "white" {}
		_MatCap("MatCap (RGB)", 2D) = "white" {}
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

	inline fixed4 LightingMobileTexture(SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
	{
		fixed4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * atten;
		UNITY_OPAQUE_ALPHA(c.a);
		return c;
	}

	sampler2D	_MainTex;
	sampler2D	_MaskTex;
	sampler2D	_MatCap;
	fixed4		_TeamColor;
	fixed4		_DamageColor;

	struct Input {
		half2 uv_MainTex;
		half2 matcapUV;
	};

	void vert(inout appdata_full v, out Input o)
	{
		UNITY_INITIALIZE_OUTPUT(Input, o);

		float3 worldNorm = mul((float3x3)UNITY_MATRIX_MV, v.normal);
		o.matcapUV = worldNorm.xy * 0.5 + 0.5;
	}

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		fixed3 mask = tex2D(_MaskTex, IN.uv_MainTex);
		fixed3 texresult = mask.r * tex.rgb;	//Mask 없을때
		texresult += (1 - mask.r) * tex.rgb * _TeamColor;		//Mask 있을때
		fixed3 mc = tex2D(_MatCap, IN.matcapUV);// * 2;
		//add min
		o.Albedo = texresult + mc.rgb - fixed3(1,1,1) + _DamageColor.rgb;
		
		o.Alpha = tex.a;
	}

	ENDCG
	}

	Fallback "Mobile/VertexLit"
}
