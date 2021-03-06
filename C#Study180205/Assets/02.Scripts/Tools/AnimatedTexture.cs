using UnityEngine;
using System.Collections;

public class AnimatedTexture : MonoBehaviour
{
    public float fps = 30.0f;
    public Texture2D[] frames;

    private int frameIndex = 0;
    private MeshRenderer rendererMy;

    void Start()
    {
        rendererMy = GetComponent<MeshRenderer>();
        //NextFrame();
        //InvokeRepeating("NextFrame", 1 / fps, 1 / fps);
    }

    void NextFrame()
    {
        rendererMy.sharedMaterial.SetTexture("_MainTex", frames[frameIndex]);
        frameIndex = (frameIndex + 1) % frames.Length;
    }

    public void PlayOneCircle()
    {
        rendererMy.sharedMaterial.SetTexture("_MainTex", frames[frameIndex]);
        //frameIndex = (frameIndex + 1) % frames.Length;
        if (frameIndex != frames.Length - 1)
        {
            frameIndex++;
            InvokeRepeating("PlayOneCircle", 1 / fps, 1 / fps);
        }
        else
        {
            frameIndex = 0;
            rendererMy.enabled = false;
            CancelInvoke("PlayOneCircle");
        }
    }
    
}