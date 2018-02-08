Shader "Custom/RuneSkillShader" 
{

    Properties 
    {
		_MainTex ("Base (RGB)", 2D) = "white" {}
    }
        //Cull Off
		//Cull Back
        //ZWrite On // To keep the correct Z order
    
    SubShader 
    {
        Pass 
        {      
	        Tags {
	            "IgnoreProjector"="True"
	            "Queue"="Transparent-50"
	            "RenderType"="Opaque"
    	    }
			
			LOD 100
			Lighting Off
			//Cull Off
			ZWrite On

			//Offset -1, -1
			
			SetTexture [_MainTex] 
		}
    
    
    }

}





