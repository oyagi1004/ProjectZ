Shader "Custom/WindowShader" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
 
	_Diffuse ("MainTex", 2D) = "white" {} 
	
 
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	
	_MainTex ("3DTexture", 2D) = "white" {} 
	[MaterialToggle(_COLOR_ON)] _TintColor ("HQ", Float) = 1 
	_RoomColor ("3DTexture_Color", Color) = (0.6,0.6,0.6,0.5)
	
	[MaterialToggle(_Dist_ON)] _TintColor2 ("Distortion_On", Float) = 0 
	_Distortion ("Distortion", Range (0.0, 5)) = 1.0
	
	_Lightning ("Lightning", Range (0.01, 1)) = 0.01
	
	_Offset ("Offset", Range (0.001, 8.0)) = 0.01
	_Depth ("Depth", Range (0.001, 8.0)) = 0.01
	
	
	_Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
    _ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
    _rim_power("Rim_power", Range(0.1,5) ) = 5
    
	 
}
SubShader {
	LOD 200
	Tags { "RenderType"="Opaque" }
	
CGPROGRAM
#pragma surface surf BlinnPhong 
#pragma target 3.0
#include "UnityCG.cginc"
#pragma multi_compile _COLOR_OFF _COLOR_ON
#pragma multi_compile _Dist_OFF _Dist_ON 

sampler2D _MainTex;
samplerCUBE _Cube;
sampler2D _Diffuse; 
sampler2D _BumpMap;
 
float  _Depth;
float _Offset;
fixed4 _Color;
fixed4 _ReflectColor;
fixed4 _RoomColor;
half _Shininess;
half _Lightning;
half _rim_power;
half _Distortion;

struct Input {
	float2 uv_MainTex;
	float2 uv_Diffuse;
	float3 worldRefl;
	float3 viewDir;
	INTERNAL_DATA
};


  
    
    

void surf (Input IN, inout SurfaceOutput o) {

    half h = tex2D (_MainTex, IN.uv_MainTex).w;
    
    o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_Diffuse));
    
    
    #if _Dist_ON
    float2 offset = ParallaxOffset  (h , -1, IN.viewDir ) + o.Normal * _Distortion  ;
    #else
    float2 offset = ParallaxOffset  (h , -1, IN.viewDir ) ;
    #endif
    
    #if _COLOR_ON
	float2 uv1 = IN.uv_MainTex += offset * _Offset*0.1 * _Depth ;
	float2 uv2 = IN.uv_MainTex += offset * 0.03 * _Offset;
	float2 uv3 = IN.uv_MainTex += offset * 0.035 * _Offset;
	float2 uv4 = IN.uv_MainTex += offset * 0.04 * _Offset;
	float2 uv5 = IN.uv_MainTex += offset * 0.045 * _Offset;
	float2 uv6 = IN.uv_MainTex += offset * 0.05 * _Offset;
	float2 uv7 = IN.uv_MainTex += offset * 0.055 * _Offset;
	float2 uv8 = IN.uv_MainTex += offset * 0.06 * _Offset;
	float2 uv9 = IN.uv_MainTex += offset * 0.065 * _Offset;
	
	
	fixed4 tex = tex2D(_MainTex, uv1)*0.6;
	fixed4 tex2 = tex2D(_MainTex, uv2)*0.22;
	fixed4 tex3 = tex2D(_MainTex, uv3)*0.22;
	fixed4 tex4 = tex2D(_MainTex, uv4)*0.21;
	fixed4 tex5 = tex2D(_MainTex, uv5)*0.2;
	fixed4 tex6 = tex2D(_MainTex, uv6)*0.15;
	fixed4 tex7 = tex2D(_MainTex, uv7)*0.12;
	fixed4 tex8 = tex2D(_MainTex, uv8)*0.11;
	fixed4 tex9 = tex2D(_MainTex, uv9)*0.09;
	
	fixed4 room = tex + tex2 + tex3 + tex4 + tex5  + tex6 + tex7 + tex8 + tex9 ;
	 #else
	 
	 float2 uv1 = IN.uv_MainTex += offset * _Offset*0.1 * _Depth ;
	float2 uv3 = IN.uv_MainTex += offset * 0.045 * _Offset;
	float2 uv7 = IN.uv_MainTex += offset * 0.055 * _Offset;
	float2 uv9 = IN.uv_MainTex += offset * 0.065 * _Offset;
	
	
	fixed4 tex = tex2D(_MainTex, uv1)*0.6;
	fixed4 tex3 = tex2D(_MainTex, uv3)*0.52;
	fixed4 tex7 = tex2D(_MainTex, uv7)*0.42;
	fixed4 tex9 = tex2D(_MainTex, uv9)*0.32;
	fixed4 room = tex + tex3 + tex7 + tex9 ;
	 
	 
	 
	 #endif
	 
	
	 fixed4 diff  = tex2D (_Diffuse, IN.uv_Diffuse)    ;
	 
	 
	  
	 
 
	o.Albedo =      (diff *  diff.a )* _Color   +  (room.rgb *_RoomColor*3 +_RoomColor.a) *  (1-diff.a) ;
	
	o.Gloss = 1-diff.a + diff*2  ;
	o.Specular = _Shininess ;
	
	fixed4 Fresnel_1 = float4(0,0,1,1);
	fixed4 Fresnel_2 =(1.0 - dot( normalize( float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz), normalize( Fresnel_1.xyz )   )).xxxx;
	fixed4 Pow = pow(Fresnel_2,_rim_power.xxxx  );
	
	float3 worldRefl = WorldReflectionVector (IN, o.Normal);
	fixed4 reflcol = texCUBE (_Cube, worldRefl )* Pow  ;
	reflcol *= (1- diff.a) * _ReflectColor;
	o.Emission = (room.rgb *_Lightning*2) *  (1-diff.a)  +   reflcol ;
	o.Alpha = reflcol.a * _ReflectColor.a;
	 
	
}
ENDCG
}
	
FallBack "Reflective/VertexLit"
} 
