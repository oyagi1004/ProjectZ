Shader "FX/Mirror Reflection Lightmapped" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
    _LightMap ("Lightmap (RGB)", 2D) = "black" {}
    _ReflectionTex ("Environment Reflection", 2D) = "" { TexGen ObjectLinear }
}  

// -----------------------------------------------------------
// Texture, Lightmap, Reflection; three texture cards

Subshader {
    // Ambient pass
    Pass {
        Color [_Color]
        BindChannels {
            Bind "Vertex", vertex
            Bind "texcoord", texcoord0 // main uses 1st uv
            Bind "texcoord1", texcoord1 // lightmap uses 2nd uv
        }
        SetTexture [_MainTex] {
            combine texture * primary
        }
        SetTexture [_LightMap] {
            combine texture * previous, previous
        }
        SetTexture [_ReflectionTex] {
            matrix [_ProjMatrix]
            combine texture lerp(previous) previous, texture
        }
    }
}

// -----------------------------------------------------------
// Fallback: just base texture

Subshader {
    Pass {
        SetTexture [_MainTex] { constantColor[_Color] combine texture * constant }
    }
}

}