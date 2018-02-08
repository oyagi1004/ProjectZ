using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockSystemManager : MonoBehaviour {

    private static FlockSystemManager instance = null;

    public static FlockSystemManager Instance
    {
        get { return instance; }
    }

    public GameObject[] ObstacleList;

    void Awake()
    {
        if(instance)
        {
            DestroyImmediate(this);
            return;
        }

        DontDestroyOnLoad(this);
    }
}
