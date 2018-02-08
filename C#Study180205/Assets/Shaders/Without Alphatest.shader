Shader "Transparent/Unlit/Without Alphatest" {

    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }
 

    Category {

        //Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        Tags {"Queue" = "Transparent+100" }
        
        Lighting Off
        //Cull Off
	//Cull Back
        //ZWrite On // To keep the correct Z order
        

        SubShader {
            Pass {          

				Blend SrcAlpha OneMinusSrcAlpha // Doing Alpha Blending

                SetTexture [_MainTex] {

                    constantColor [_Color]
		            // Mixing the color and the alpha
                    Combine texture * constant, texture * constant 

                } 
            }
        } 
    }
}