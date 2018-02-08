Shader "Custom/Hardlight" 
{
	Properties 
	{
		_HardLight ("HardLight (RGB)", 2D) = "white" {}
		_MainTex ("Base (RGB)", 2D) = "white" {} 
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }

		pass
		{
			Lighting Off Fog { Mode Off }
			Blend Off
			SetTexture[_MainTex] 
		}

		pass 
		{
			Lighting Off Fog { Mode Off }
			Blend DstColor SrcColor//2X Multiplicative

			BindChannels 
			{
			   Bind "texcoord1", texcoord1
			}

			SetTexture[_MainTex]
			SetTexture[_HardLight] 
		}
	//pass
	}
	//subshader
}
//shader
