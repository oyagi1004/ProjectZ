Shader "Phantom/InGame/Stealth" {
	Properties{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		[PhantomTeam]_TeamColor("Team Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_TransParent("TransParent",Float) = 0
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert alpha:fade

		sampler2D _MainTex;
		fixed4 _Color;
		half _TransParent;
		fixed4		_TeamColor;

		struct Input 
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) 
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed3 texresult = tex.a * tex.rgb;	//Mask 없을때
			texresult += (1 - tex.a) * tex.rgb * _TeamColor;		//Mask 있을때
			o.Albedo = texresult.rgb;
			o.Alpha = _TransParent;
		}
		ENDCG
	}

	Fallback "Legacy Shaders/Transparent/VertexLit"
}
