using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private static GameManager instance = null;

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    bool isGameStart = false;

    FlockController[] FlockCtrl;
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

    void Start()
    {
        InitGame();
    }

    void InitGame()
    {
        InitData();
        FindObject();
        InitObject();
    }

    void InitData()
    {
        CharacterData.Read();
        ItemData.Read();
        EnemyData.Read();
    }

    void FindObject()
    {
        //GameManager가 관리하는 오브젝트들을 찾아서 할당.

        FlockCtrl = GameObject.FindObjectsOfType<FlockController>();
    }

    void InitObject()
    {
        //GameManager에 할당된 오브젝트들을 찾아서 초기화(각 오브젝트 클래스의 초기화 함수를 호출할 것.)
        foreach(FlockController f in FlockCtrl)
        {
            f.InitFlockController();
        }
    }

    void Update()
    {

    }
}
