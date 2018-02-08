Shader "Custom/Hardlight01" 
{
Properties 
{
	_Color ("Hard Color", Color) = (1,1,1,1)
	_Color2 ("Main Color", Color) = (1,1,1,1)
	
//	_Lightmap ("Lightmap (A=Blend)", 2D) = ""
	
	_HardLight ("HardLight (RGB)", 2D) = "white" {}
	_MainTex ("Base (RGB)", 2D) = "white" {} 
}
SubShader {
Tags { "RenderType"="Opaque" }

pass{

	Blend Off

	SetTexture[_MainTex] 
	{
		constantColor [_Color]
		combine texture * constant
	}
//	SetTexture [_MainTex]
//	{
//		Combine Texture * Primary
//	}
//	
//	SetTexture [_MainTex]
//	{
//		ConstantColor [_Color]
//		Combine Previous * Constant
//	}

// Blend SrcAlpha OneMinusSrcAlpha //AlphaBlend
// Blend One One//Additive
// Blend One OneMinusDstColor//Soft Additive
// Blend DstColor Zero//Multiplicative
}


pass {

	Blend DstColor SrcColor//2X Multiplicative

//	Material
//	{
//		Diffuse[_Color]
//	}

	BindChannels 
	{
       //Bind "Vertex", vertex
//       Bind "texcoord", texcoord0
       Bind "texcoord1", texcoord1
	}
	
	SetTexture[_MainTex]

	SetTexture[_HardLight] 
	{
		constantColor [_Color2]
		combine texture * constant
		//Combine previous * texture, texture
	}

}


//pass
}
//subshader
}
//shader
