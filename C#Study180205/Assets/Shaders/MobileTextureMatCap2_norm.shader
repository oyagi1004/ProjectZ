// Free MatCap Shaders 에셋스토어 참고


Shader "Phantom/TextureMatCap2_norm" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_MaskTex("Mask Team(R) MatCap2(G)", 2D) = "white" {}
		_MatCap("MatCap (RGB)", 2D) = "white" {}
		_MatCap2("MatCap2 (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
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

#pragma surface surf MobilePhong2 noforwardadd nofog vertex:vert
//#pragma target 3.0

	inline fixed4 LightingMobilePhong2(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
	{
		fixed4 c;
	
		c.rgb = s.Albedo * _LightColor0.rgb * atten;
		UNITY_OPAQUE_ALPHA(c.a);
		return c;
	}

	sampler2D	_MainTex;
	sampler2D	_MaskTex;
	sampler2D	_MatCap;
	sampler2D	_MatCap2;
	sampler2D 	_BumpMap;
	fixed4		_TeamColor;
	fixed4		_DamageColor;
	half _RimPower;

	struct Input {
		half2 uv_MainTex;
		float3 c0;
		float4 c1;
	};

	void vert(inout appdata_full v, out Input o)
	{
		UNITY_INITIALIZE_OUTPUT(Input, o);

		TANGENT_SPACE_ROTATION;
		o.c0 = normalize( mul(rotation, normalize(UNITY_MATRIX_IT_MV[0].xyz)) );
		o.c1.xyz = normalize( mul(rotation, normalize(UNITY_MATRIX_IT_MV[1].xyz)) );
		
		half3 normal = normalize(v.normal);
		half3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
		half rim = 1.0 - saturate(dot(normal, viewDir));
		rim = pow(rim, _RimPower);
		o.c1.w = rim;
	}

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		fixed3 mask = tex2D(_MaskTex, IN.uv_MainTex);
		fixed3 texresult = mask.r * tex.rgb;	//Mask 없을때
		texresult += (1-mask.r) * tex.rgb * _TeamColor;		//Mask 있을때
		fixed3 normal = UnpackNormal (tex2D(_BumpMap, IN.uv_MainTex));
		
		half2 capCoord = half2(dot(IN.c0, normal), dot(IN.c1.xyz, normal));
		half4 mc = tex2D(_MatCap, capCoord*0.5+0.5)*2;
		fixed4 mc4 = tex2D(_MatCap2, capCoord*0.5+0.5)*2;

		fixed4 mcResult = mc * mask.g + mc4 * (1- mask.g);
		//add min
		o.Albedo = texresult + mcResult - fixed3(1,1,1) + _DamageColor.rgb * IN.c1.w;

		o.Normal = normal;
		o.Gloss = mask.g;
		o.Alpha = tex.a;
	}

	ENDCG
	}

	Fallback "Phantom/TextureMatCap2"
}
