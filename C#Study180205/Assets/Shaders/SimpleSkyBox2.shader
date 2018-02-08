Shader "Unlit/SkyBox2" 
{
    Properties 
    {
		_MainTex ("Base (RGB)", 2D) = "white" {}
    }
    
    SubShader 
    {
		Tags { "Queue"="Background" "RenderType"="Background" }
		
		LOD 100
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		//Cull Off
		//ZWrite Off Fog { Mode Off }

        Pass 
        {      
			SetTexture [_MainTex] 
		}
    }
}
