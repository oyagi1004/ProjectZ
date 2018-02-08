// Free MatCap Shaders 에셋스토어 참고

Shader "Phantom/TextureMatCap_modulate" {
	Properties{
		_MainTex("Base (RGB) Team Mask (A)", 2D) = "white" {}
		_MatCap("MatCap (RGB)", 2D) = "white" {}
		_Modulate("Modulate", Float) = 1.0
		_RimPower ("Rim Power", Float ) = 2.000
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
	sampler2D	_MatCap;
	half		_Modulate;
	fixed4		_TeamColor;
	fixed4		_DamageColor;
	half _RimPower;

	struct Input {
		half2 uv_MainTex;
		half3 matcapUV;
	};

	void vert(inout appdata_full v, out Input o)
	{
		UNITY_INITIALIZE_OUTPUT(Input, o);

		float3 worldNorm = normalize(mul((float3x3)UNITY_MATRIX_MV, v.normal));
		o.matcapUV.xy = worldNorm.xy * 0.5 + 0.5;
		
		half3 normal = normalize(v.normal);
		half3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
		half rim = 1.0 - saturate(dot(normal, viewDir));
		rim = pow(rim, _RimPower);
		o.matcapUV.z = rim;
	}

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		fixed3 texresult = tex.a * tex.rgb;	//Mask 없을때
		texresult += (1 - tex.a) * tex.rgb * _TeamColor;		//Mask 있을때
		fixed3 mc = tex2D(_MatCap, IN.matcapUV.xy);
		//modulate
		o.Albedo = _Modulate * texresult * mc.rgb + _DamageColor.rgb * IN.matcapUV.z;
		//overlay
		//o.Albedo = tex.rgb < 0.5 ? tex.rgb * mc.rgb * 2 : 1 - 2 * ( 1 - tex.rgb) * ( 1 - mc.rgb );
		//o.Albedo = tex.rgb * mc.rgb * 2;
		//o.Albedo = 1 - 2 * ( 1 - tex.rgb) * ( 1 - mc.rgb );
		//softlight
		//o.Albedo = 2 * tex.rgb * mc.rgb + tex.rgb * tex.rgb * ( 1 - 2 * mc.rgb);
		//screen
		//o.Albedo = tex.rgb < 0.5 ? 2.0 * tex.rgb * mc.rgb + tex.rgb * mc.rgb * (1.0 - 2.0 * mc.rgb) : sqrt(tex.rgb) * (2.0 * mc.rgb - 1.0) + 2.0 * tex.rgb * (1.0 - mc.rgb);
		//o.Albedo = 2.0 * tex.rgb * mc.rgb + tex.rgb * mc.rgb * (1.0 - 2.0 * mc.rgb);
		//o.Albedo = sqrt(tex.rgb) * (2.0 * mc.rgb - 1.0) + 2.0 * tex.rgb * (1.0 - mc.rgb);
		//o.Albedo = lerp( tex.rgb, mc.rgb, 0.5 );
		
		o.Alpha = tex.a;
	}

	ENDCG
	}

	Fallback "Mobile/VertexLit"
}
