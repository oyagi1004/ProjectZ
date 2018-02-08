Shader "Unlit/Damage Color" 
{

    Properties 
    {
        _Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
    }
        //Cull Off
		//Cull Back
        //ZWrite On // To keep the correct Z order
    
    SubShader 
    {
        Pass 
        {      
			Tags { "RenderType"="Opaque" }
			LOD 100
			Lighting Off

// Blend SrcAlpha OneMinusSrcAlpha //AlphaBlend
// Blend One One//Additive
// Blend One OneMinusDstColor//Soft Additive
// Blend DstColor Zero//Multiplicative
			Blend One One
			//Blend One OneMinusDstColor

			Offset -1, -1
			
			SetTexture [_MainTex] 
			{ 
				constantColor [_Color]
				combine texture * constant
			} 
		}
    
    
    }

}
