// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Version=1.7
Shader"UNOShader/_Library/UNLIT/UNOShaderUNLIT VertexCol SH "
{
	Properties
	{
		_BRDF("BRDF Shading", 2D) = "white" {}
		_LightmapTex ("Lightmap Texture", 2D) = "gray" {}
		_MasksTex ("Masks", 2D) = "white" {}
		//--------------------- Xray Shader Features  --------------------------------
		 [HideInInspector] _XRAYEDGE("Xray edge", Float) = 0.0
		//---------------------  Shader Features  --------------------------------
		[HideInInspector] _lmUV1("Lightmap UV", Float) = 0.0
		[HideInInspector] _maskTex("Texture Masks", Float) = 0.0
		[HideInInspector] _mathPixel("Math", Float) = 0.0	
		[HideInInspector] _BASE("Shade Base", Float) = 0.0
		[HideInInspector] _AMBIENT("Ambient Light", Float) = 0.0
		[HideInInspector] _LDIR("Directional Light", Float) = 0.0
		[HideInInspector] _REFCUSTOM("Custom Reflection", Float) = 0.0
		[HideInInspector] _CUSTOMLIGHTMAP("Custom Lightmap", Float) = 0.0
		[HideInInspector] _CUSTOMSHADOW("Custom Shadow", Float) = 0.0
		[HideInInspector] _EDGETRANSPARENCY("Edge transparency", Float) = 0.0
	}
	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"Queue" = "Geometry"
		}
		Pass
			{
			Name "ForwardBase"
			Tags
			{
				"RenderType" = "Opaque"
				"Queue" = "Geometry"
				"LightMode" = "ForwardBase"
			}
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "UnityPBSLighting.cginc"
			#include "UnityStandardBRDF.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

			//------------------------------------------------- pragmas ---------------------------------------------------------
			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature BASE_UNLIT BASE_BRDF
			#pragma shader_feature lmUV1_ON lmUV1_OFF
			#pragma shader_feature maskTex_ON maskTex_OFF
			#pragma shader_feature mathPixel_ON mathPixel_OFF

			#pragma shader_feature AMBIENT_OFF AMBIENT_ON
			#pragma shader_feature LDIR_OFF LDIR_ON

			#pragma shader_feature CUSTOMLIGHTMAP_OFF CUSTOMLIGHTMAP_ON

			#pragma shader_feature CUSTOMSHADOW_OFF CUSTOMSHADOW_ON
			#pragma exclude_renderers d3d11_9x
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fog
			#if maskTex_ON
			sampler2D _MasksTex;
			half4 _MasksTex_ST;
			#endif
			sampler2D _BRDF;
			half4 _BRDF_ST;

			#ifdef CUSTOMLIGHTMAP_ON
				sampler2D _LightmapTex;
				half4 _LightmapTex_ST;
			#endif

			fixed4 _UNOShaderShadowColor;
			fixed _UNOShaderShadowFalloff;
			fixed _UNOShaderLightmapOpacity;
			struct customData
			{
				half4 vertex : POSITION;
				half3 normal : NORMAL;
				half4 tangent : TANGENT;
				fixed2 texcoord : TEXCOORD0;
				fixed2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
				fixed4 color : COLOR;
			};
			struct v2f // = vertex to fragment ( pass vertex data to pixel pass )
			{
				half4 pos : SV_POSITION;
				fixed4 vc : COLOR;
				half4 Ndot : COLOR1;
				fixed4 uv : TEXCOORD0;
				fixed4 uv2 : TEXCOORD1;
				half4 posWorld : TEXCOORD2;//position of vertex in world;
				half4 normalDir : TEXCOORD3;//vertex Normal Direction in world space
				half4 viewRefDir : TEXCOORD4;
				UNITY_FOG_COORDS(5)
				LIGHTING_COORDS(6, 7)
			};
			v2f vert (customData v)
			{
				v2f o;
				o.normalDir = fixed4 (0,0,0,0);
				o.Ndot = fixed4(0,0,0,0);
				o.posWorld = fixed4 (0,0,0,0);
				o.normalDir.xyz = UnityObjectToWorldNormal(v.normal);
				o.posWorld.xyz = mul(unity_ObjectToWorld, v.vertex);
			//--- Vectors
				half3 normalDirection = normalize(half3( mul(half4(v.normal, 0.0), unity_WorldToObject).xyz ));
				half3 lightDirection = normalize(half3(_WorldSpaceLightPos0.xyz));
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - o.posWorld.xyz);// world space

				o.pos = UnityObjectToClipPos (v.vertex);//UNITY_MATRIX_MVP is a matrix that will convert a model's vertex position to the projection space
				o.vc = fixed4(1,1,1,1);;// Vertex Colors
				o.vc = v.color;// Vertex Colors
				o.uv = fixed4(0,0,0,0);
				o.uv.xy = v.texcoord;
				o.uv2 = fixed4(0,0,0,0);
				o.uv2.xy = v.texcoord1; //--- regular uv2
				o.uv2.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw; //Unity matrix lightmap uvs
				# ifdef DYNAMICLIGHTMAP_ON
					o.uv2.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif
				o.viewRefDir = fixed4(0,0,0,0);
				#ifdef BASE_UNLIT
					o.Ndot.x = max(0.0, dot(normalDirection, lightDirection));//NdotL  light falloff
				#endif
				#ifdef BASE_BRDF
					o.Ndot.x = dot(normalDirection, lightDirection)*.5 +.5;//NdotL  light falloff
				#endif
				o.Ndot.y = clamp((dot(viewDirection, o.normalDir.xyz)) * 1.2 -.2 ,0.01,.99);

			//============================= Lights ================================
				fixed3 ambientLights = fixed3 (0,0,0);

				//___________________________ LightProbes Shade SH9 Math  __________________________________________
				fixed3 ambience = fixed3(1,1,1);
					ambience = ShadeSH9 (half4(o.normalDir.xyz,1.0)).rgb;
					ambientLights += ambience;


				o.normalDir.w = ambientLights.r;
				o.viewRefDir.w = ambientLights.g;
				o.posWorld.w = ambientLights.b;
				TRANSFER_VERTEX_TO_FRAGMENT(o) // This sets up the vertex attributes required for lighting and passes them through to the fragment shader.
			//_________________________________________ FOG  __________________________________________
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag (v2f i) : COLOR  // i = in gets info from the out of the v2f vert
			{
				fixed4 resultRGB = fixed4(1,1,1,0);
				fixed4 resultRGBnl = fixed4(0,0,0,0);
				fixed3 ambientLights = fixed3(i.normalDir.w,i.viewRefDir.w,i.posWorld.w);
			//__________________________________ Vectors _____________________________________
				half3 normalDirection = normalize(i.normalDir.xyz);
				half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				//_WorldSpaceCameraPos.xyz built in gets camera Position
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);//float3 _WorldSpaceLightPos0 -> built in gets light Position
				float3 halfDirection = normalize(viewDirection + lightDirection);
				half fresnel = dot(viewDirection, normalDirection);
				fixed atten = fixed (1);
				atten = LIGHT_ATTENUATION(i); // This gets the shadow and attenuation values combined.
			//__________________________________ Masks _____________________________________
				#if maskTex_ON
				fixed4 T_Masks = tex2D(_MasksTex, i.uv.xy);
				#endif
			//__________________________________ Vertex Color _____________________________________
				resultRGB.rgb = i.vc.rgb;

			//------------------------- GI Data --------------------------
			fixed3 lightColor = _LightColor0.rgb;

			UnityLight light;

			# ifdef LIGHTMAP_OFF
			light.color = lightColor;
			light.dir = lightDirection;
			light.ndotl = LambertTerm(normalDirection, light.dir);
			#else
			light.color = fixed3(0.f, 0.f, 0.f);
			light.dir = fixed3(0.f, 0.f, 0.f);
			light.ndotl = 0.0f;
			#endif

			UnityGIInput dat;
			dat.light = light;// initializes input for d.light
			dat.worldPos = i.posWorld.xyz;
			dat.worldViewDir = viewDirection;

			dat.atten = 1;

			#if defined (LIGHTMAP_ON) || defined (DYNAMICLIGHTMAP_ON)
			dat.ambient = 0;
			dat.lightmapUV = i.uv2;
			#else
			dat.ambient = i.uv2;
			#endif

			UnityGI gi = UnityGlobalIllumination(dat, 1, 0, normalDirection);
			lightDirection = gi.light.dir;
			lightColor = gi.light.color;
			fixed3 giDiffuse = gi.indirect.diffuse;
			//__________________________________ Lightmap _____________________________________
			//--- lightmap unity ---
				fixed4 Lightmap = fixed4(0, 0, 0, 0);
				#ifdef CUSTOMLIGHTMAP_OFF
					#ifdef LIGHTMAP_ON
						Lightmap = fixed4(DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2)),1);
					#endif
					Lightmap.rgb = giDiffuse;
				#endif

			//--- custom lightmap ---
				#ifdef CUSTOMLIGHTMAP_ON
					#if lmUV1_ON
						Lightmap = tex2D(_LightmapTex, i.uv);
					#endif
					#if lmUV1_OFF
						Lightmap = tex2D(_LightmapTex, i.uv2);
					#endif
					Lightmap.rgb = DecodeLightmap(Lightmap);
				#endif
				Lightmap = lerp(1,Lightmap,_UNOShaderLightmapOpacity);

			//__________________________________ Lighting _____________________________________
				fixed NdotL = i.Ndot.x;
				fixed NdotV = i.Ndot.y;
				fixed4 T_BRDF = tex2D(_BRDF, fixed2(NdotV,NdotL));
				fixed3 dirLight = fixed3(0, 0, 0);
				fixed3 lighting = fixed3(0, 0, 0);

				//----- Shadow Stuff
				fixed shadowMask = NdotL;
				fixed3 shadowColor = ambientLights;
				float shadowMask1 = NdotL*atten;// lerp(1, atten, clamp(NdotL * 2, 0, 1));
				float shadowMask2 = atten;
				#ifdef BASE_BRDF
					NdotL = T_BRDF.r;
					NdotL = clamp((NdotL * atten) + T_BRDF.a, 0, 1);
					shadowMask1 = NdotL;
					shadowMask2 = clamp(atten + T_BRDF.a, 0, 1);
				#endif
				shadowMask = lerp(shadowMask2, shadowMask1, _UNOShaderShadowFalloff);

				#ifdef CUSTOMSHADOW_ON
					shadowColor = _UNOShaderShadowColor;
				#endif

				//------ LIGHTMAPS INACTIVE
				#ifdef LIGHTMAP_OFF	

				#ifdef CUSTOMLIGHTMAP_ON
					#ifdef LDIR_ON
						dirLight = NdotL * atten * _LightColor0;
						lighting = Lightmap + dirLight;
					#else
						lighting = Lightmap;
					#endif
					#ifdef CUSTOMSHADOW_ON
						resultRGB.rgb *= lighting;
						resultRGB.rgb = lerp(Luminance(resultRGB) * (shadowColor * (_UNOShaderShadowColor.a*5)), resultRGB, shadowMask);
					#else
						resultRGB.rgb *= lighting;
					#endif
				#endif

				#ifdef CUSTOMLIGHTMAP_OFF
					#ifdef AMBIENT_ON
						lighting += ambientLights;
						#ifdef LDIR_ON
							lighting += (_LightColor0*NdotL) ;
						#endif
						#ifdef LDIR_OFF
							shadowMask = shadowMask;
						#endif
					#endif
					#ifdef AMBIENT_OFF
						ambientLights = ambientLights;
						#ifdef LDIR_ON
							lighting += (_LightColor0*NdotL) +1;
						#endif
						#ifdef LDIR_OFF
							lighting = 1;
						#endif
					#endif
					resultRGB.rgb = lerp(resultRGB.rgb * shadowColor, resultRGB.rgb * lighting, shadowMask);
				#endif
				#endif

				//------ LIGHTMAPS ACTIVE
				#ifdef LIGHTMAP_ON
					#ifdef LDIR_ON
						dirLight = NdotL * atten * _LightColor0;
						lighting = Lightmap + dirLight;
					#else
						lighting = Lightmap;
					#endif
					#ifdef CUSTOMSHADOW_ON
						resultRGB.rgb *= lighting;
						resultRGB.rgb = lerp(Luminance(resultRGB) * (shadowColor * (_UNOShaderShadowColor.a*5)), resultRGB, shadowMask);
					#else
						resultRGB.rgb *= lighting;
					#endif
				#endif

			//__________________________________ Mask Occlussion _____________________________________
				#if maskTex_ON
				//--- Oclussion from alpha
				resultRGB.rgb = resultRGB.rgb * T_Masks.g;
				#endif

			//__________________________________ Fog  _____________________________________
				UNITY_APPLY_FOG(i.fogCoord, resultRGB);

			//__________________________________ Vertex Alpha _____________________________________
				resultRGB.a *= i.vc.a;

			//__________________________________ result Final  _____________________________________
				return resultRGB;
			}
			ENDCG
		}//-------------------------------Pass-------------------------------
		Pass
		{
			Name "Meta"
			Tags 
			{
				"LightMode"="Meta"
			}
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define UNITY_PASS_META 1
			#define _GLOSSYENV 1
			#include "UnityCG.cginc"
			#include "UnityPBSLighting.cginc"
			#include "UnityStandardBRDF.cginc"
			#include "UnityMetaPass.cginc"
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_fog
			#pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
			#pragma target 3.0
			uniform half4 _EmissionColor;
			uniform fixed _EmissionBakeIntensity;
			uniform sampler2D _EmissionMap; uniform half4 _EmissionMap_ST;
			struct VertexInput 
			{
				half4 vertex : POSITION;
				half2 texcoord0 : TEXCOORD0;
				half2 texcoord1 : TEXCOORD1;
				half2 texcoord2 : TEXCOORD2;
			};
			struct VertexOutput 
			{
				half4 pos : SV_POSITION;
				half2 uv0 : TEXCOORD0;
			};
			VertexOutput vert (VertexInput v) 
			{
				VertexOutput o = (VertexOutput)0;
				o.uv0 = v.texcoord0;
				o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
				return o;
			}
			half4 frag(VertexOutput i) : SV_Target 
			{
				/////// Vectors:
				UnityMetaInput o;
				UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
				half4 _EmissionMap_var = tex2D(_EmissionMap,TRANSFORM_TEX(i.uv0, _EmissionMap));

				o.Emission = ((_EmissionColor.rgb*_EmissionMap_var.rgb)*_EmissionBakeIntensity*10);

				half3 diffColor = half3(.5f,.5f,.5f);
				o.Albedo = diffColor;

				return UnityMetaFragment( o );
			}
			ENDCG
		}
		UsePass "UNOShader/_Library/Helpers/Shadows/SHADOWCAST"
	} //-------------------------------SubShader-------------------------------
	Fallback "UNOShader/_Library/Helpers/VertexUNLIT"
	CustomEditor "UNOShader_UNLIT"
}