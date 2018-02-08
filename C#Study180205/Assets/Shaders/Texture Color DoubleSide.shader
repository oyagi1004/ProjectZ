Shader "Unlit/Texture Color DoubleSide" 
{

    Properties 
    {
        _Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
    }

		//Cull Back
        //ZWrite On // To keep the correct Z order
    
    SubShader 
    {
        Pass 
        {      
			Tags { "RenderType"="Opaque" }
			LOD 100
			Lighting Off
			Cull Off
			
			SetTexture [_MainTex] 
			{ 
				constantColor [_Color]
				//combine texture + primary, texture * primary
				combine texture * constant
			} 
		}
    
    
    }

}
