using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UNOShaderData : MonoBehaviour {
	
	public Color UNOShaderShadowColor;
	public float UNOShaderLightmapOpacity;
    public float UNOShaderShadowFalloff;
	// Use this for initialization
	void Start () 
	{
		Shader.SetGlobalColor("_UNOShaderShadowColor", UNOShaderShadowColor);	
		Shader.SetGlobalFloat("_UNOShaderLightmapOpacity", UNOShaderLightmapOpacity);
        Shader.SetGlobalFloat("_UNOShaderShadowFalloff", UNOShaderShadowFalloff);
    }
	
	// Update is called once per frame
	void Update () 
	{
	}
	
}
