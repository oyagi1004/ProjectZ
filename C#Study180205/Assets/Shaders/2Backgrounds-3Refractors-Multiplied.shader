// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Refract2D/2 Backgrounds/3 Refractors/Multiplied" {

	Properties {
		_Background ("Background", 2D) = "" {}			//Receive input from a Texture on Texture Unit 0
		_BackgroundScrollX ("X Offset", float) = 0		//Receive input from float
		_BackgroundScrollY ("Y Offset", float) = 0		//Receive input from float
		_BackgroundScaleX ("X Scale", float) = 1.0		//Receive input from float
		_BackgroundScaleY ("Y Scale", float) = 1.0		//Receive input from float
		_Refraction ("Refraction", float) = 1.0			//Receive input from float
		_Background2 ("Background", 2D) = "" {}			//Receive input from a Texture on Texture Unit 1
		_BackgroundScrollX2 ("X Offset", float) = 0		//Receive input from float
		_BackgroundScrollY2 ("Y Offset", float) = 0		//Receive input from float
		_BackgroundScaleX2 ("X Scale", float) = 1.0		//Receive input from float
		_BackgroundScaleY2 ("Y Scale", float) = 1.0		//Receive input from float
		_BackgroundFade2 ("Fade", Range(0,1)) = 1.0		//Receive input from float
		_Refraction2 ("Refraction", float) = 1.0			//Receive input from float
		_DistortionMap ("Distortion Map", 2D) = "" {}	//Receive input from a Texture on Texture Unit 2
		_DistortionScrollX ("X Offset", float) = 0		//Receive input from float
		_DistortionScrollY ("Y Offset", float) = 0		//Receive input from float
		_DistortionScaleX ("X Scale", float) = 1.0		//Receive input from float
		_DistortionScaleY ("Y Scale", float) = 1.0		//Receive input from float
		_DistortionPower ("Distortion Power", float) = 0.08	//Receive input from float
		_DistortionMap2 ("Distortion Map", 2D) = "" {}	//Receive input from a Texture on Texture Unit 3
		_DistortionScrollX2 ("X Offset", float) = 0		//Receive input from float
		_DistortionScrollY2 ("Y Offset", float) = 0		//Receive input from float
		_DistortionScaleX2 ("X Scale", float) = 1.0		//Receive input from float
		_DistortionScaleY2 ("Y Scale", float) = 1.0		//Receive input from float
		_DistortionPower2 ("Distortion Power", float) = 0.08	//Receive input from float
		_DistortionMap3 ("Distortion Map", 2D) = "" {}	//Receive input from a Texture on Texture Unit 3
		_DistortionScrollX3 ("X Offset", float) = 0		//Receive input from float
		_DistortionScrollY3 ("Y Offset", float) = 0		//Receive input from float
		_DistortionScaleX3 ("X Scale", float) = 1.0		//Receive input from float
		_DistortionScaleY3 ("Y Scale", float) = 1.0		//Receive input from float
		_DistortionPower3 ("Distortion Power", float) = 0.08	//Receive input from float
	}

	//Define a shader
	SubShader {

		//Define what queue/order to render this shader in
		Tags {"Queue" = "Geometry" "RenderType" = "Opaque"}		//Background | Geometry | AlphaTest | Transparent | Overlay - Render this shader as part of the geometry queue because it does not use blending

		//Define a pass
		Pass {

			//Set up blending and other operations
			Cull Off			// Back | Front | Off - Do not cull any triangle faces
			ZTest LEqual		//Less | Greater | LEqual | GEqual | Equal | NotEqual | Always - Pixels will only be allowed to continue through the rendering pipeline if the Z coordinate of their position is LEqual the existing Z coordinate in the Z/Depth buffer
			ZWrite On			//On | Off - Z coordinates from pixel positions will be written to the Z/Depth buffer
			AlphaTest Off 		//0.0	//Less | Greater | LEqual | GEqual | Equal | NotEqual | Always   (also 0.0 (float value) | [_AlphaTestThreshold]) - All pixels will continue through the graphics pipeline because alpha testing is Off
			Lighting Off		//On | Off - Lighting will not be calculated or applied
			ColorMask RGBA		//RGBA | RGB | A | 0 | any combination of R, G, B, A - Color channels allowed to be modified in the backbuffer are: RGBA
			//BlendOp	//Add	// Min | Max | Sub | RevSub - BlendOp is not being used and will default to an Add operation when combining the source and destination parts of the blend mode
			Blend Off			//SrcFactor DstFactor (also:, SrcFactorA DstFactorA) = One | Zero | SrcColor | SrcAlpha | DstColor | DstAlpha | OneMinusSrcColor | OneMinusSrcAlpha | OneMinusDstColor | OneMinusDstAlpha - Blending between shader output and the backbuffer will use blend mode 'Solid'
								//Blend SrcAlpha OneMinusSrcAlpha     = Alpha blending
								//Blend One One                       = Additive
								//Blend OneMinusDstColor One          = Soft Additive
								//Blend DstColor Zero                 = Multiplicative
								//Blend DstColor SrcColor             = 2x Multiplicative

			CGPROGRAM						//Start a program in the CG language
			#pragma target 2.0				//Run this shader on at least Shader Model 2.0 hardware (e.g. Direct3D 9)
			#pragma fragment frag			//The fragment shader is named 'frag'
			#pragma vertex vert				//The vertex shader is named 'vert'
			#include "UnityCG.cginc"		//Include Unity's predefined inputs and macros

			//Unity variables to be made accessible to Vertex and/or Fragment shader
			uniform sampler2D _Background;					//Define _Background from Texture Unit 0 to be sampled in 2D
			//uniform float4 _Background_ST;				//Use the Float _Background_ST to pass the Offset and Tiling for the texture(s)
			uniform sampler2D _Background2;					//Define _Background2 from Texture Unit 1 to be sampled in 2D
			//uniform float4 _Background2_ST;				//Use the Float _Background2_ST to pass the Offset and Tiling for the texture(s)
			uniform sampler2D _DistortionMap;				//Define _DistortionMap from Texture Unit 2 to be sampled in 2D
			//uniform float4 _DistortionMap_ST;				//Use the Float _DistortionMap_ST to pass the Offset and Tiling for the texture(s)
			uniform sampler2D _DistortionMap2;				//Define _DistortionMap2 from Texture Unit 3 to be sampled in 2D
			//uniform float4 _DistortionMap2_ST;			//Use the Float _DistortionMap2_ST to pass the Offset and Tiling for the texture(s)
			uniform sampler2D _DistortionMap3;				//Define _DistortionMap3 from Texture Unit 3 to be sampled in 2D
			//uniform float4 _DistortionMap3_ST;			//Use the Float _DistortionMap3_ST to pass the Offset and Tiling for the texture(s)
			uniform float _BackgroundScrollX;
			uniform float _BackgroundScrollY;
			uniform float _BackgroundScaleX;
			uniform float _BackgroundScaleY;
			uniform float _BackgroundScrollX2;
			uniform float _BackgroundScrollY2;
			uniform float _BackgroundScaleX2;
			uniform float _BackgroundScaleY2;
			uniform float _BackgroundFade2;
			uniform float _DistortionScrollX;
			uniform float _DistortionScrollY; 
			uniform float _DistortionPower;
			uniform float _DistortionScaleX;
			uniform float _DistortionScaleY;
			uniform float _DistortionScrollX2;
			uniform float _DistortionScrollY2; 
			uniform float _DistortionPower2;
			uniform float _DistortionScaleX2;
			uniform float _DistortionScaleY2;
			uniform float _DistortionScrollX3;
			uniform float _DistortionScrollY3; 
			uniform float _DistortionPower3;
			uniform float _DistortionScaleX3;
			uniform float _DistortionScaleY3;
			uniform float _Refraction;
			uniform float _Refraction2;
												
			//Data structure communication from Unity to the vertex shader
			//Defines what inputs the vertex shader accepts
			struct AppData {
				float4 vertex : POSITION;					//Receive vertex position
				half2 texcoord : TEXCOORD0;					//Receive texture coordinates
							//half2 texcoord1 : TEXCOORD1;				//Receive texture coordinates
							//fixed4 color : COLOR;						//Receive vertex colors
			};

			//Data structure for communication from vertex shader to fragment shader
			//Defines what inputs the fragment shader accepts
			struct VertexToFragment {
				float4 pos : POSITION;						//Send fragment position to fragment shader
				half2 uv : TEXCOORD0;						//Send interpolated texture coordinate to fragment shader
							//half2 uv2 : TEXCOORD1;					//Send interpolated texture coordinate to fragment shader
							//fixed4 color : COLOR;						//Send interpolated gouraud-shaded vertex color to fragment shader
			};

			//Vertex shader
			VertexToFragment vert(AppData v) {
				VertexToFragment o;							//Create a data structure to pass to fragment shader
				o.pos = UnityObjectToClipPos(v.vertex);		//Include influence of Modelview + Projection matrices
				o.uv = v.texcoord.xy;		//Send texture coords from unit 0 to fragment shader
							//o.uv2 = v.texcoord1.xy;					//Send texture coords from unit 1 to fragment shader
							//o.color = v.color;						//Send interpolated vertex color to fragment shader
							//o.color = _Color;							//Send solid color to fragment shader
				return o;									//Transmit data to the fragment shader
			}

			//Fragment shader
			fixed4 frag(VertexToFragment i) : COLOR {
				fixed2 distortion = tex2D(_DistortionMap, float2(_DistortionScaleX,_DistortionScaleY)*i.uv+float2(_DistortionScrollX,_DistortionScrollY).xy) * _DistortionPower - (_DistortionPower*0.5);	//Get distortion map, Red=X, Green=Y
 				fixed2 distortion2 = tex2D(_DistortionMap2, float2(_DistortionScaleX2,_DistortionScaleY2)*i.uv+float2(_DistortionScrollX2,_DistortionScrollY2).xy) * _DistortionPower2 - (_DistortionPower2*0.5);	//Get distortion map, Red=X, Green=Y
 				fixed2 distortion3 = tex2D(_DistortionMap3, float2(_DistortionScaleX3,_DistortionScaleY3)*i.uv+float2(_DistortionScrollX3,_DistortionScrollY3).xy) * _DistortionPower3 - (_DistortionPower3*0.5);	//Get distortion map, Red=X, Green=Y
  				fixed distort=distortion+distortion2+distortion3;	//For speed
  				fixed4 background = tex2D(_Background, float2(_BackgroundScaleX,_BackgroundScaleY)*i.uv+(_Refraction*distort)+float2(_BackgroundScrollX,_BackgroundScrollY));	//Get the background
   				fixed4 background2 = tex2D(_Background2, float2(_BackgroundScaleX2,_BackgroundScaleY2)*i.uv+(_Refraction2*distort)+float2(_BackgroundScrollX2,_BackgroundScrollY2));	//Get the background
   				return fixed4(background.r*background2.r*_BackgroundFade2,background.g*background2.g*_BackgroundFade2,background.b*background2.b*_BackgroundFade2,1.0);	//Return multiplied backgrounds
			}

			ENDCG							//End of CG program

		}
	}
}

//Copyright (c) 2013 Paul West/Venus12 LLC
