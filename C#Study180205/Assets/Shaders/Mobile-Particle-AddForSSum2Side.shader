// Simplified Additive Particle shader. Differences from regular Additive Particle one:
// - no Tint color
// - no Smooth particle support
// - no AlphaTest
// - no ColorMask

Shader "Mobile/Particles/AdditiveForSSummon2Side" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
	Tags {  "Queue"="Transparent+291" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	Lighting Off ZWrite Off Cull off Fog { Color (0,0,0,0) }
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}
}
}