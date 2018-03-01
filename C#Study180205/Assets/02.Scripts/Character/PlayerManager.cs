//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using RootMotion.FinalIK;
//using RootMotion.Demos;
//using System;

//public class PlayerManager : MonoBehaviour {

//    private static PlayerManager instance = null;

//    public static PlayerManager Instance
//    {
//        get
//        {
//            return instance;
//        }
//    }


//    //public struct PlayerStat
//    //{
//    //    public float Health;
//    //    public float Stemina;
//    //    public float Defence;
//    //    public float Strength;
//    //    public float Concentration;
//    //    public float Speed;

//    //    public PlayerStat(float _Health, float _Stemina, float _Defence, float _Strength, float _Concentration, float _Speed)
//    //    {
//    //        Health = _Health;
//    //        Stemina = _Stemina;
//    //        Defence = _Defence;
//    //        Strength = _Strength;
//    //        Concentration = _Concentration;
//    //        Speed = _Speed;
//    //    }
//    //}

//    public Character playerStat;

//    public GameObject PlayerObj;

//    public UIJoystick JoyStick;

//    public Animator anim;

//    public enum PlayerState
//    {
//        IDLE,
//        WALK,
//        RUN,
//        DIE,
//    }

//    public PlayerState playerState = PlayerState.IDLE;

//    public enum AttackState
//    {
//        NONE,
//        FIRE,
//        MELEE,
//    }

//    public AttackState attackState = AttackState.NONE;

//    public float detectionAngle = 45f;

//    public Transform Pelvis;

//    bool bLookatTarget = false;

//    Vector3 leftBoundary;
//    Vector3 rightBoundary;


//    //IK Control 변수.
//    Transform PoleTarget;
//    Transform Target;
//    AimIK aimIK;
//    LookAtIK lookAtIK;
//    SimpleAimingSystem aimingSystem;
//    AimPoser aimPoser;

//    Vector3 DefaultTargetVec;

//    private void Awake()
//    {
//        if(instance != null)
//        {
//            DestroyImmediate(gameObject);
//            return;
//        }

//        instance = this;

//        DontDestroyOnLoad(this);
//    }
    
    
//    public void InitPlayer()
//    {

//        playerStat = CharacterData.FindCharacterInfoByID(1);
//        PlayerObj = Instantiate(Resources.Load("Prefabs/Helena01")) as GameObject;
//        PlayerObj.transform.parent = transform;
//        PlayerObj.transform.localPosition = Vector3.zero;
//        anim = PlayerObj.GetComponent<Animator>();
//        Pelvis = Pelvis.Find("Bip001 Pelvis");
//        Pelvis = Pelvis.Find("Bip001 Spine");

//        //IKSet
//        //PoleTarget = transform.Find("PoleTarget").transform;
//        //PlayerObj.transform.Find("PoleTarget").transform.position = PoleTarget.transform.position;
//        aimIK = PlayerObj.GetComponent<AimIK>();
//        lookAtIK = PlayerObj.GetComponent<LookAtIK>();
//        aimingSystem = PlayerObj.GetComponent<SimpleAimingSystem>();
//        aimPoser = PlayerObj.GetComponent<AimPoser>();
//        DefaultTargetVec = new Vector3(0, 1f, 4f);
//        PlayerObj.transform.Find("Target").transform.localPosition = DefaultTargetVec;
//        EnableIK(false);
//    }
    
//    void Update()
//    {
        

//        DrawView();

//        CheckPlayerState();

//        SetBehavior();

//        if (attackState == AttackState.FIRE)
//        {
//            Debug.Log("Fire!!");
//        }
//        if (attackState == AttackState.NONE)
//        {
//            Debug.Log("None!");
//        }
//    }
 
//    void CheckPlayerState()
//    {
//        if(JoyStick.position != Vector2.zero)
//        {
//            playerState = PlayerState.WALK;
//        }
//        else
//        {
//            playerState = PlayerState.IDLE;
//        }
//    }

//    void SetBehavior()
//    {
//        switch(playerState)
//        {
//            case PlayerState.IDLE:
//                HandleOnIdle();
//                break;
//            case PlayerState.WALK:
//                HandleOnWalk();
//                break;
//            case PlayerState.RUN:
//                HandleOnRun();
//                break;
//            case PlayerState.DIE:
//                HandleOnDie();
//                break;
//        }
        
//    }

//    public void SetAttack()
//    {
//        if(attackState != AttackState.MELEE)
//        {
//            EnableIK(true);
//            //anim.SetTrigger("FiringRifle");
//            StartCoroutine(WaitingFiringTrigger());
            
//        }
//    }

//    IEnumerator WaitingFiringTrigger()
//    {
//        if (aimIK.enabled && aimPoser.enabled && lookAtIK.enabled && aimingSystem.enabled)
//        {
//            anim.SetTrigger("FiringRifle");
//            yield return null;
//        }
//        else
//            yield return StartCoroutine(WaitingFiringTrigger());

//    }

//    void NormalMove()
//    {
//        EnableIK(false);
//        anim.SetBool("BattleIdle", false);
//        // 기본 총구방향 지정.
//        PlayerObj.transform.Find("Target").transform.localPosition = DefaultTargetVec;
//        Vector3 lookDir = JoyStick.position.x * Vector3.right + JoyStick.position.y * Vector3.forward;
//        Quaternion rot = Quaternion.LookRotation(lookDir);
//        transform.rotation = rot;
//        Quaternion charRot = Quaternion.LookRotation(Vector3.zero);
//        PlayerObj.transform.localRotation = charRot;
//        transform.Translate(Vector3.forward * playerStat.Speed * Time.deltaTime);
//        bLookatTarget = false;
//    }

//    void CombatMove()
//    {
//        EnableIK(true);
//        anim.SetBool("BattleIdle", true);
//        //적의 방향으로 조준하며 이동.
//        Vector3 TargetVec = GameManager.Instance.GetClosedEnemy().transform.Find("AimTarget").transform.position;
//        PlayerObj.transform.Find("Target").transform.position = TargetVec;
        
//        Vector3 lookDir = JoyStick.position.x * Vector3.right + JoyStick.position.y * Vector3.forward;
//        Quaternion rot = Quaternion.LookRotation(lookDir);
//        PlayerObj.transform.rotation = rot;

//        Vector3 MovePos = new Vector3(JoyStick.position.x, 0f, JoyStick.position.y).normalized;
//        transform.Translate(MovePos * playerStat.Speed * Time.deltaTime, Space.World);
//        bLookatTarget = true;
//    }

//    public Vector3 DirFromAngle(float angleInDegrees)
//    {
//        //플레이어의 좌우 회전값 갱신
//        angleInDegrees += transform.eulerAngles.y;
//        //경계 벡터값 반환
//        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
//    }

//    public void DrawView()
//    {
//        float ViewDistance = 7.89f;
//        leftBoundary = DirFromAngle(-detectionAngle / 2);
//        rightBoundary = DirFromAngle(detectionAngle / 2);
//        Debug.DrawLine(transform.position, transform.position + leftBoundary * ViewDistance, Color.blue);
//        Debug.DrawLine(transform.position, transform.position + rightBoundary * ViewDistance, Color.blue);
//        Debug.DrawLine(transform.position, transform.position + transform.forward * ViewDistance, Color.red);
//    }



//    bool CheckVisibleTarget()
//    {
//        Vector3 TargetVec;
//        try
//        {
//            TargetVec  = GameManager.Instance.GetClosedEnemy().transform.position;
//        }
//        catch(NullReferenceException e)
//        {
//            return false;
//        }
//        //플레이어 - 타겟 간 단위벡터
//        Vector3 dirToTarget = (TargetVec - transform.position).normalized;

//        //_transform.forward와 dirToTarget은 모두 단위벡터이므로 내적값은 두 벡터가 이루는 각의 Cos값과 같다.
//        //내적값이 시야각/2의 Cos값보다 크면 시야에 들어온 것이다.
//        //if(Vector3.Dot(transform.forward, dirToTarget) > Mathf.Cos(detectionAngle / 2) * Mathf.Deg2Rad)
//        //내적을 이용한 방법은 좀더 연구!!

//        if(Vector3.Angle(transform.forward, dirToTarget) < detectionAngle / 2f)
//        {
//            float distToTarget = Vector3.Distance(transform.position, TargetVec);
//            Debug.DrawLine(transform.position, TargetVec, Color.green);

//            return true;
//        }
//        return false;
//    }

//    //시야각과 조이스틱 인풋의 각도를 계산.
//    bool CheckInputDir()
//    {
//        Vector3 OffsetJoystickVec = new Vector3(JoyStick.position.x, 0f, JoyStick.position.y).normalized;
//        float LeftAngle = Vector3.Angle(leftBoundary.normalized, OffsetJoystickVec);
//        float RightAngle = Vector3.Angle(rightBoundary.normalized, OffsetJoystickVec);

//        if(LeftAngle >= RightAngle)
//        {
//            if (Vector3.Angle(rightBoundary.normalized, OffsetJoystickVec) <= detectionAngle)
//                return true;
//        }
//        else
//        {
//            if (Vector3.Angle(leftBoundary.normalized, OffsetJoystickVec) <= detectionAngle)
//                return true;
//        }

//        return false;
//    }

//    public void EnableIK(bool enabled)
//    {
//        aimIK.enabled = enabled;
//        lookAtIK.enabled = enabled;
//        aimingSystem.enabled = enabled;
//        aimPoser.enabled = enabled;
//    }

//    void HandleOnIdle()
//    {
//        anim.SetBool("Walk", false);
//        bLookatTarget = false;
//        //AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(1);
//        //if (anim.GetCurrentAnimatorStateInfo(1).IsName("RifleIdle"))
//        //    EnableIK(false);
//    }

//    void HandleOnWalk()
//    {
//        anim.SetBool("Walk", true);

//        if (attackState == AttackState.NONE)
//        {
//            NormalMove();
//        }
//        else if (attackState == AttackState.FIRE)
//        {
//            if (CheckVisibleTarget())
//            {
//                if (CheckInputDir())
//                {
//                    CombatMove();
//                }
//                else
//                {
//                    NormalMove();
//                }
//            }
//            else
//            {
//                NormalMove();
//            }
//        }

//    }

//    void HandleOnRun()
//    {

//    }

//    void HandleOnDie()
//    {

//    }

//    public void DestroyManager()
//    {
//        DestroyImmediate(PlayerObj);
//        DestroyImmediate(this);
//    }


//    void OnTriggerEnter(Collider other)
//    {
//        if(other.tag == "Enemy")
//            GameManager.Instance.AddClosedEnemyList(other.gameObject);
//    }

//    void OnTriggerExit(Collider other)
//    {
//        if (other.tag == "Enemy")
//            GameManager.Instance.DeleteClosedEnemyList(other.gameObject);
//    }

//}
