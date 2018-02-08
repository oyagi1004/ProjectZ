// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


// Test  water shader by oh.jinwoo

// Vertext Alpha For water Edge.
// tint Color.
// no Light.
// Scroll Uv Animation.

Shader "Phantom/Unlit_VertexAlpha_UVscroll" {
Properties 
{
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}

    _ScrollX ("Scroll UV X speed", Float) = 1.0 //+
	_ScrollY ("Scroll UV Y speed", Float) = 0.0 //+
}
 
SubShader 
{
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100
   
    //ZWrite Off
    ZWrite On
    Alphatest Greater 0 
    Blend SrcAlpha OneMinusSrcAlpha


   
    Pass 
    {



    	 Lighting Off  //
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"
 
            struct appdata_t 
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };
 
            struct v2f 
            {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            float _ScrollX; //+
			float _ScrollY; //+








            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex) +  frac(float2(_ScrollX, _ScrollY) * _Time);
                o.color = v.color;

               

                return o;
            }



            fixed4 frag (v2f i) : COLOR
            {
                fixed4 col = tex2D(_MainTex, i.texcoord) * _Color * i.color;
                return col;
            }

        ENDCG

    }
}
 
}
 