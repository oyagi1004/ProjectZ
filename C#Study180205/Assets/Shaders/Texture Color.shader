Shader "Unlit/Texture Color" 
{
    Properties 
    {
        _Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
    }
    
    SubShader 
    {
        Pass 
        {      
			Tags 
			{ 
				"RenderType"="Opaque" 
				"IgnoreProjector" = "True"
			}
			
			LOD 100
			Lighting Off
			ZWrite On

			//Offset -1, -1
			Offset 0, -1
			
			SetTexture [_MainTex] 
			{ 
				constantColor [_Color]
				combine texture * constant
			} 
		}
    }
}
