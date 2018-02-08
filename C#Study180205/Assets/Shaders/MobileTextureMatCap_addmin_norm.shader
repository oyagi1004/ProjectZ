﻿// Free MatCap Shaders 에셋스토어 참고

Shader "Phantom/TextureMatCap_addmin_norm" {
	Properties{
		_Shininess ("Shininess", Range (0.0, 1)) = 0.078125
		_MainTex("Base (RGB)", 2D) = "white" {}
		_MaskTex("Mask Team(R)", 2D) = "white" {}
		_MatCap("MatCap (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
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
#pragma target 3.0
#pragma surface surf MobilePhong noforwardadd halfasview vertex:vert

	inline fixed4 LightingMobilePhong(SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
	{
		fixed4 c;
		
		fixed nh = max (0, dot (s.Normal, halfDir));
		half spec = pow(nh, s.Specular*128) * s.Gloss;
		
		c.rgb = (s.Albedo * _LightColor0.rgb + _LightColor0.rgb * spec) * atten;
		UNITY_OPAQUE_ALPHA(c.a);
		return c;
	}

	sampler2D	_MainTex;
	sampler2D	_MaskTex;
	sampler2D	_MatCap;
	sampler2D 	_BumpMap;
	half 		_Shininess;
	fixed4		_TeamColor;
	fixed4		_DamageColor;

	struct Input {
		half2 uv_MainTex;
		float3 c0;
		float3 c1;
	};

	void vert(inout appdata_full v, out Input o)
	{
		UNITY_INITIALIZE_OUTPUT(Input, o);

		TANGENT_SPACE_ROTATION;
		o.c0 = mul(rotation, normalize(UNITY_MATRIX_IT_MV[0].xyz));
		o.c1 = mul(rotation, normalize(UNITY_MATRIX_IT_MV[1].xyz));
	}

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		fixed3 mask = tex2D(_MaskTex, IN.uv_MainTex);
		fixed3 texresult = mask.r * tex.rgb;	//Mask 없을때
		texresult += (1 - mask.r) * tex.rgb * _TeamColor;		//Mask 있을때
		fixed3 normal = UnpackNormal (tex2D(_BumpMap, IN.uv_MainTex));
		
		half2 capCoord = half2(dot(IN.c0, normal), dot(IN.c1, normal));
		float4 mc = tex2D(_MatCap, capCoord*0.5+0.5);//*2;
		//add min
		o.Albedo = texresult + mc.rgb - fixed3(1,1,1) + _DamageColor.rgb;

		o.Specular = _Shininess;
		o.Normal = normal;
		o.Gloss = mask.g;
		o.Alpha = tex.a;
	}

	ENDCG
	}

	Fallback "Phantom/TextureMatCap_addmin"
}
