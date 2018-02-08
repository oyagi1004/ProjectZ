Shader "Custom/Reflective/Mirror Additive" {
	Properties {
//		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
		_ReflectionTex ("Reflection", 2D) = "white" { TexGen ObjectLinear }
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		Tags { "Queue"="Transparent+1" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100
		
//		Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
		Blend One One // Additive
		//Blend OneMinusDstColor One // Soft Additive
		//Blend DstColor Zero // Multiplicative
		//Blend DstColor SrcColor // 2x Multiplicative		
		
		
		Pass {
	        
	        SetTexture[_ReflectionTex] 
	        { 
	        	matrix [_ProjMatrix] 
	        	constantColor[_Color]
	        	combine texture * constant, texture * constant 
	        }
	    }
	}

	// fallback: just main texture
	Subshader {
		Tags { "RenderType"="Opaque" }
		LOD 100
		
	    Pass 
	    {
	        SetTexture [_MainTex] 
	        { 
	        	combine texture 
	        }
	    }
	}
}

