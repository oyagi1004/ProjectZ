// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Mobile/PlaneShadow"
{
	Properties
	{
		//_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
		Blend SrcAlpha OneMinusSrcAlpha

		//스텐실버퍼를 이용해서 0일때만 그리게 
		Stencil{
			Ref 0
			Comp Equal
			Pass IncrWrap
		}
		LOD 100

		Pass
		{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		struct v2f {
			float4 pos : SV_POSITION;
			fixed4 color : COLOR;
		};

		v2f vert(appdata_base v)
		{
			v2f o;

			half _PlaneHeight = 0.001;
			fixed _ShadowIntensity = 0.45;

			float4 vPosWorld = mul(unity_ObjectToWorld, v.vertex);

			float4 lightDirection = -normalize(_WorldSpaceLightPos0);

			float opposite = vPosWorld.y - _PlaneHeight;

			//높이에 따른 그림자 강도
			float shadowOppositeInten = 1 - ( opposite / 7 );

			float cosTheta = -lightDirection.y;	// = lightDirection dot (0,-1,0)

			float hypotenuse = opposite / cosTheta;

			float3 vPos = vPosWorld.xyz + (lightDirection * hypotenuse);

			o.pos = mul(UNITY_MATRIX_VP, float4(vPos.x, _PlaneHeight, vPos.z ,1));
			o.color = fixed4(0, 0, 0, _ShadowIntensity*shadowOppositeInten);// lightDirection;

			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			return i.color;
		}
		ENDCG
		}
	}
}
