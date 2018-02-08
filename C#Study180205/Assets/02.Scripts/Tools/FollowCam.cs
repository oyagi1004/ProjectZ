using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour {

	public GameObject Target;
	public float Distance = 5f;
	public float Height = 8f;
	public float Speed = 2f;

	Vector3 Pos;

    bool isFollowing = false;

    private static FollowCam instance = null;

    public static FollowCam Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if(instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void InitCam()
    {
        isFollowing = true;
    }

	void Update () {
	
		Pos = new Vector3 (Target.transform.position.x, Height, Target.transform.position.z - Distance);

		//this.gameObject.transform.position
		//= Vector3.MoveTowards (this.gameObject.transform.position, Pos, Speed * Time.deltaTime);

		this.gameObject.transform.position
		= Vector3.Lerp (this.gameObject.transform.position, Pos, Speed * Time.deltaTime);
	}

    public void DestroyManager()
    {
        DestroyImmediate(gameObject);
    }
}
