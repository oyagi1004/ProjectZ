Shader "Custom/Reflective/Unlit Hardlight" {
Properties {

//	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {} 
	_HardLight ("HardLight (RGB)", 2D) = "white" {}
	_Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
}

Category {
	Tags { "RenderType"="Opaque" }
	LOD 150

	SubShader {
	
		pass
		{
			Lighting Off Fog { Mode Off }
			Blend Off
			SetTexture[_MainTex] 
		}

		pass 
		{
			Lighting Off Fog { Mode Off }
			Blend DstColor SrcColor

			BindChannels 
			{
			    Bind "texcoord1", texcoord
			}
			SetTexture[_HardLight] 
		}		
		
		
		pass 
		{ 
			Lighting Off Fog { Mode Off }
//			Blend DstColor OneMinusSrcAlpha
			Blend Zero SrcColor
			
			SetTexture [_Cube]
			{
//				matrix [_ProjMatrix]
//				constantColor[_Color]
 				combine texture //DOUBLE
			}
		}		
	}
}

}
