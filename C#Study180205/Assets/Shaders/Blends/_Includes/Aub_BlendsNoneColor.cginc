// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#ifndef AUB_BLENDSNONECOLOR_INCLUDED

#define AUB_BLENDSNONECOLOR_INCLUDED

 

/**********************INCLUDES**********************/

 
fixed4 _Color1;
 

/**********************STRUCTS**********************/

		
			
struct a2f_uv0 {
//    half4 vertex : POSITION;
//    half2 texcoord : TEXCOORD0;
//    half2 texcoord1 : TEXCOORD1;
    
    fixed4 vertex : POSITION;
    fixed2 texcoord : TEXCOORD0;
    fixed2 texcoord1 : TEXCOORD1;    
};

struct v2f_uv0 {
//    half4 pos : SV_POSITION;
//    half2 uv : TEXCOORD0;
//    half2 uv2 : TEXCOORD1;
    
    fixed4 pos : SV_POSITION;
    fixed2 uv : TEXCOORD0;
    fixed2 uv2 : TEXCOORD1;    
};			

/**********************VERTS**********************/

v2f_uv0 vert_uv0(a2f_uv0 v) {
    v2f_uv0 o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.texcoord;
    o.uv2 = v.texcoord1;
    return o;
}			 

/********************FUNCTIONS********************/

fixed4 Darken (fixed4 a, fixed4 b) { return fixed4(min(a.rgb, b.rgb), 1); }

fixed4 Multiply (fixed4 a, fixed4 b) { return (a * b); }

fixed4 ColorBurn (fixed4 a, fixed4 b) { return (1-(1-a)/b); }

fixed4 LinearBurn (fixed4 a, fixed4 b) { return (a+b-1); }

fixed4 Lighten (fixed4 a, fixed4 b) { return fixed4(max(a.rgb, b.rgb), 1); }

fixed4 Screen (fixed4 a, fixed4 b) { return (1-(1-a)*(1-b)); }

fixed4 ColorDodge (fixed4 a, fixed4 b) { return (a/(1-b)); }

fixed4 LinearDodge (fixed4 a, fixed4 b) { return (a+b); }


fixed4 Overlay (fixed4 a, fixed4 b) {

    fixed4 r = fixed4(0,0,0,1);

    if (a.r > 0.5) { r.r = 1-(1-2*(a.r-0.5))*(1-b.r); }

    else { r.r = (2*a.r)*b.r; }

    if (a.g > 0.5) { r.g = 1-(1-2*(a.g-0.5))*(1-b.g); }

    else { r.g = (2*a.g)*b.g; }

    if (a.b > 0.5) { r.b = 1-(1-2*(a.b-0.5))*(1-b.b); }

    else { r.b = (2*a.b)*b.b; }

    return r;

}



fixed4 SoftLight (fixed4 a, fixed4 b) 
{
	fixed4 r = fixed4(0,0,0,1);
	r = ((a < 0.5)?(2*((b*0.5)+0.25))*a:(1.0-(2*(1.0-((b*0.5)+0.25))*(1.0-a))));
    return r;
}
 

fixed4 SoftLight1 (fixed4 a, fixed4 b) {

    fixed4 r = fixed4(0,0,0,1);

    if (b.r > 0.5) { r.r = a.r*(1-(1-a.r)*(1-2*(b.r))); }

    else { r.r = 1-(1-a.r)*(1-(a.r*(2*b.r))); }

    if (b.g > 0.5) { r.g = a.g*(1-(1-a.g)*(1-2*(b.g))); }

    else { r.g = 1-(1-a.g)*(1-(a.g*(2*b.g))); }

    if (b.b > 0.5) { r.b = a.b*(1-(1-a.b)*(1-2*(b.b))); }

    else { r.b = 1-(1-a.b)*(1-(a.b*(2*b.b))); }

    return r;

}


fixed4 HardLight (fixed4 a, fixed4 b) {

    return (b < 0.5) ? (2 * b * a):(1.0 - 2 * (1.0 - b) * (1.0 - a));			    
    
}						



fixed4 HardLight1 (fixed4 a, fixed4 b) {

    fixed4 r = fixed4(0,0,0,1);

    if (b.r > 0.5) { r.r = 1-(1-a.r)*(1-2*(b.r)); }

    else { r.r = a.r*(2*b.r); }

    if (b.g > 0.5) { r.g = 1-(1-a.g)*(1-2*(b.g)); }

    else { r.g = a.g*(2*b.g); }

    if (b.b > 0.5) { r.b = 1-(1-a.b)*(1-2*(b.b)); }

    else { r.b = a.b*(2*b.b); }

    return r;

}

fixed4 VividLight (fixed4 a, fixed4 b) {

    fixed4 r = fixed4(0,0,0,1);

    if (b.r > 0.5) { r.r = 1-(1-a.r)/(2*(b.r-0.5)); }

    else { r.r = a.r/(1-2*b.r); }

    if (b.g > 0.5) { r.g = 1-(1-a.g)/(2*(b.g-0.5)); }

    else { r.g = a.g/(1-2*b.g); }

    if (b.b > 0.5) { r.b = 1-(1-a.b)/(2*(b.b-0.5)); }

    else { r.b = a.b/(1-2*b.b); }

    return r;

}

fixed4 LinearLight (fixed4 a, fixed4 b) {

    fixed4 r = fixed4(0,0,0,1);

    if (b.r > 0.5) { r.r = a.r+2*(b.r-0.5); }

    else { r.r = a.r+2*b.r-1; }

    if (b.g > 0.5) { r.g = a.g+2*(b.g-0.5); }

    else { r.g = a.g+2*b.g-1; }

    if (b.b > 0.5) { r.b = a.b+2*(b.b-0.5); }

    else { r.b = a.b+2*b.b-1; }

    return r;

}

fixed4 PinLight (fixed4 a, fixed4 b) {

    fixed4 r = fixed4(0,0,0,1);

    if (b.r > 0.5) { r.r = max(a.r, 2*(b.r-0.5)); }

    else { r.r = min(a.r, 2*b.r); }

    if (b.g > 0.5) { r.g = max(a.g, 2*(b.g-0.5)); }

    else { r.g = min(a.g, 2*b.g); }

    if (b.b > 0.5) { r.b = max(a.b, 2*(b.b-0.5)); }

    else { r.b = min(a.b, 2*b.b); }

    return r;

}

fixed4 Difference (fixed4 a, fixed4 b) { return (abs(a-b)); }

fixed4 Exclusion (fixed4 a, fixed4 b) { return (0.5-2*(a-0.5)*(b-0.5)); }

 

#endif