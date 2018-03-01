//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class GameManager : MonoBehaviour {

//    private static GameManager instance = null;

//    public static GameManager Instance
//    {
//        get
//        {
//            return instance;
//        }
//    }

//    bool isGameStart = false;
    
//    private void Awake()
//    {
//        if(instance != null)
//        {
//            DestroyImmediate(gameObject);
//            return;
//        }

//        instance = this;
//        DontDestroyOnLoad(gameObject);
//    }

//    List<GameObject> Enemies;

//    List<GameObject> ClosedEnemies;
//    GameObject ClosedEnemy;

//    GameObject[] SpawnPoints;
//    int SpawnMobsNum = 10; // 스테이지 데이터화 시킬것.

//	void Start () {
//        InitGame();
//    }
	
//    public void InitGame()
//    {
//        Enemies = new List<GameObject>();
//        ClosedEnemies = new List<GameObject>();

//        SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
         
//        CharacterData.Read();
//        EnemyData.Read();

//        PlayerManager.Instance.InitPlayer();
//        FollowCam.Instance.InitCam();

//        //SpawnEnemy();
//    }

//    void SpawnEnemy()
//    {
//        StartCoroutine(EnemySpawn());
//    }

//    IEnumerator EnemySpawn()
//    {
//        if(SpawnMobsNum > 0)
//        {
//            int seed = Random.Range(1,SpawnPoints.Length + 1);
//            GameObject obj;
//            yield return new WaitForSeconds(1f);
//            SpawnMobsNum--;
//            if(seed % 2 == 1)
//            {
//               obj = Instantiate(Resources.Load("Prefabs/ZombieM01"), SpawnPoints[seed-1].transform) as GameObject;
//            }
//            else
//            {
//                obj = Instantiate(Resources.Load("Prefabs/ZombieF01"), SpawnPoints[seed-1].transform) as GameObject;
//            }

//            AddEnemies(obj);
//        }


//        if (SpawnMobsNum != 0)
//            yield return StartCoroutine(EnemySpawn());
//        else
//            yield return null;
//    }

//    void Update()
//    {
//        CheckClosedEnemy();
//    }

//    public void AddEnemies(GameObject obj)
//    {
//        Enemies.Add(obj);
//    }

//    public void DeleteEnemies(GameObject obj)
//    {
//        Enemies.Remove(obj);
//    }

//    public void AddClosedEnemyList(GameObject obj)
//    {
//        ClosedEnemies.Add(obj);
//    }

//    public void DeleteClosedEnemyList(GameObject obj)
//    {
//        ClosedEnemies.Remove(obj);
//    }

//    void CheckClosedEnemy()
//    {
//        if(ClosedEnemies.Count > 0)
//        {
//            PlayerManager.Instance.attackState = PlayerManager.AttackState.FIRE;
//        }
//        else
//        {
//            PlayerManager.Instance.attackState = PlayerManager.AttackState.NONE;
//        }
//    }

//    public GameObject GetClosedEnemy()
//    {
//        float dist = 0f;
//        foreach(GameObject obj in ClosedEnemies)
//        {
//            if (dist == 0f)
//            {
//                dist = Vector3.Distance(obj.transform.position, PlayerManager.Instance.transform.position);
//                ClosedEnemy = obj;
//            }
//            else
//            {
//                float tempDist = Vector3.Distance(obj.transform.position, PlayerManager.Instance.transform.position);
//                if (dist > tempDist)
//                {
//                    dist = tempDist;
//                    ClosedEnemy = obj;
//                }
//            }
//        }

//        return ClosedEnemy;
//    }

//    public void DestroyManagers()
//    {
//        FollowCam.Instance.DestroyManager();
//        PlayerManager.Instance.DestroyManager();
//        DestroyImmediate(gameObject);
//    }
//}
