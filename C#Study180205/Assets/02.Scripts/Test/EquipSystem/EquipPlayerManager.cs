using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RootMotion.FinalIK;

public class EquipPlayerManager : MonoBehaviour {

    private static EquipPlayerManager instance = null;

    public static EquipPlayerManager Instance
    {
        get 
        {
            return instance;
        }
    }

    public GameObject playerObj;

    public UIJoystick JoyStick;

    public Animator anim;

    public float speed = 3f;

    float elapsedTime = 0f;

    public enum WeaponMode
    {
        NONE,
        PRIMARY,
        SUB,
    }

    public WeaponMode weaponMode;

    public Transform PrimaryPoint; //주무기 장착포인트
    public Transform PrimaryCarryPoint; //주무기 휴대포인트
    public Transform SubPoint;
    public Transform SubCarryPoint;

    Transform PWeaponTransform; //주무기 트랜스폼.
    Transform SWeaponTransform;

    GameObject SubGunMuzzle; //무기 머즐 이펙트
    GameObject PrimaryMuzzle;

    public Transform PrimaryShotPoint; //총구 트랜스폼
    public Transform SubShotPoint;

    //총알 레이캐스트 변수.
    int RifleDist = 15;
    int PistolDist = 10;
    RaycastHit RayCastData;
     

    float pistolDamage = 5f;
    float rifleDamage = 1f;

    AimIK aimIK;
    //aimTarget Transform.
    Transform AimTransform;
    Transform RifleAimTr;
    Transform PistolAimTr;

    //플래이어 시야각.
    public float FieldOfView = 45f;
    public float ViewDistance;

    BoxCollider DetectionCol; // 적감지 콜라이더.
    //Bounds DetectionBound;
    Vector3 rayDirection; //dir to Player-Enemy

    public List<Flock> DetectedEnemies;
    Flock TargetFlock;

    void Awake ()
    {
        if(instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(this);
    }

	void Start () {
        Init();
	}

    public void Init()
    {
        playerObj = transform.Find("Helena01").gameObject;

        anim = playerObj.GetComponent<Animator>();

        aimIK = playerObj.GetComponent<AimIK>();
        aimIK.enabled = false;

        weaponMode = WeaponMode.NONE;

        Transform[] allchilds = transform.GetComponentsInChildren<Transform>();

        for(int i = 0; i < allchilds.Length; i++)
        {
            if (allchilds[i].name == "Point001") PrimaryCarryPoint = allchilds[i];

            if (allchilds[i].name == "PrimaryPoint") PrimaryPoint = allchilds[i];

            if (allchilds[i].name == "MainWeaponSoket") PWeaponTransform = allchilds[i];

            if (allchilds[i].name == "Point002") SubCarryPoint = allchilds[i];

            if (allchilds[i].name == "SubPoint") SubPoint = allchilds[i];

            if (allchilds[i].name == "SubWeaponSoket") SWeaponTransform = allchilds[i];

            if (allchilds[i].name == "HandGunMuzzle") SubGunMuzzle = allchilds[i].gameObject;

            if (allchilds[i].name == "RifleMuzzle") PrimaryMuzzle = allchilds[i].gameObject;

            if (allchilds[i].name == "RifleShotPoint") PrimaryShotPoint = allchilds[i];

            if (allchilds[i].name == "RifleAimTransform") RifleAimTr = allchilds[i];

            if (allchilds[i].name == "PistolAimTransform") PistolAimTr = allchilds[i];

            if (allchilds[i].name == "AimTransform") AimTransform = allchilds[i];
        }
        DetectionCol = GetComponent<BoxCollider>();
        RayCastData = new RaycastHit();

        SetDetectionCol();

        DetectedEnemies = new List<Flock>();
    }
	
    void SetDetectionCol()
    {
        if (weaponMode == WeaponMode.NONE)
            DetectionCol.size = new Vector3(1, 1, 1);
        else if (weaponMode == WeaponMode.PRIMARY)
            DetectionCol.size = new Vector3(RifleDist * Mathf.Tan(FieldOfView * Mathf.Deg2Rad) * 2f, 1f, RifleDist);
        else if (weaponMode == WeaponMode.SUB)
            DetectionCol.size = new Vector3(PistolDist * Mathf.Tan(FieldOfView * Mathf.Deg2Rad) * 2f, 1f, PistolDist);

        DetectionCol.center = new Vector3(0, 0, DetectionCol.size.z / 2f);

        //if (weaponMode == WeaponMode.NONE)
        //    DetectionBound.size = new Vector3(1,1,1);
        //else if (weaponMode == WeaponMode.PRIMARY)
        //    DetectionBound.size = new Vector3(RifleDist * Mathf.Tan(FieldOfView * Mathf.Deg2Rad) * 2f, 1f, RifleDist);
        //else if (weaponMode == WeaponMode.SUB)
        //    DetectionBound.size = new Vector3(PistolDist * Mathf.Tan(FieldOfView * Mathf.Deg2Rad) * 2f, 1f, PistolDist);

        //DetectionBound.center = new Vector3(0, 0, DetectionBound.size.z / 2f);
    }

	void Update () {


        if (JoyStick.position != Vector2.zero)
        {
            InputState();
        }
        else
        {
            IdleState();
        }

        if (DetectedEnemies.Count != 0)
            TargetFlock = SearchTargetFlock();
	}

    void IdleState()
    {
        anim.SetBool("Walk", false);
        elapsedTime += Time.deltaTime;

        if(elapsedTime >= 10f)
        {
            RandomIdleAnim();
        }
    }

    void RandomIdleAnim()
    {
        if(weaponMode == WeaponMode.NONE)
        {
            int seed = Random.Range(1, 4);

            switch(seed)
            {
                case 1:
                    anim.SetTrigger("RandomIdle01");
                    break;
                case 2:
                    anim.SetTrigger("RandomIdle02");
                    break;
                case 3:
                    anim.SetTrigger("RandomIdle03");
                    break;
            }
        }
        elapsedTime = 0f;
    }

    void InputState()
    {
        Vector3 lookDir = JoyStick.position.x * Vector3.right + JoyStick.position.y * Vector3.forward;
        Quaternion rot = Quaternion.LookRotation(lookDir);
        transform.rotation = rot;
        if(playerObj.transform.localRotation != Quaternion.LookRotation(Vector3.zero))
            playerObj.transform.localRotation = Quaternion.LookRotation(Vector3.zero);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        anim.SetBool("Walk", true);
        elapsedTime = 0f;
    }

    public void ChangeWeaponMode()
    {
        SetDetectionCol();

        switch(weaponMode)
        {
            case WeaponMode.NONE:
                aimIK.enabled = false;

                anim.SetBool("PistolMode", false);
                anim.SetBool("RifleMode", false);
                PWeaponTransform.parent = PrimaryCarryPoint;
                PWeaponTransform.localPosition = Vector3.zero;
                PWeaponTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                SWeaponTransform.parent = SubCarryPoint;
                SWeaponTransform.localPosition = Vector3.zero;
                SWeaponTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                break;
            case WeaponMode.PRIMARY:
                AimTransform.parent = RifleAimTr;
                AimTransform.localPosition = Vector3.zero;
                AimTransform.localRotation = Quaternion.Euler(0, 0, 0);

                ViewDistance = RifleDist;

                anim.SetBool("PistolMode", false);
                anim.SetBool("RifleMode", true);
                anim.SetFloat("Aim Forward", 1f);
                PWeaponTransform.parent = PrimaryPoint;
                PWeaponTransform.localPosition = Vector3.zero;
                PWeaponTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                SWeaponTransform.parent = SubCarryPoint;
                SWeaponTransform.localPosition = Vector3.zero;
                SWeaponTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                break;
            case WeaponMode.SUB:
                AimTransform.parent = PistolAimTr;
                AimTransform.localPosition = Vector3.zero;
                AimTransform.localRotation = Quaternion.Euler(0, 0, 0);

                ViewDistance = PistolDist;

                anim.SetBool("PistolMode", true);
                anim.SetBool("RifleMode", false);
                anim.SetFloat("Aim Forward", 1f);
                PWeaponTransform.parent = PrimaryCarryPoint;
                PWeaponTransform.localPosition = Vector3.zero;
                PWeaponTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                SWeaponTransform.parent = SubPoint;
                SWeaponTransform.localPosition = Vector3.zero;
                SWeaponTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                break;
        }
    }

    public void ShootToGun()
    {
        float Damage = 0f;
        
        if(weaponMode == WeaponMode.PRIMARY)
        {
            PrimaryMuzzle.GetComponent<MeshRenderer>().enabled = true;
            PrimaryMuzzle.GetComponent<AnimatedTexture>().PlayOneCircle();
            Damage = rifleDamage;
            //if(Physics.Raycast(PrimaryShotPoint.position, PrimaryShotPoint.forward * RifleDist, out RayCastData))
            //{
            //    if(RayCastData.collider.gameObject.tag == "Obstacle")
            //    {
            //        Debug.Log("Hit Obstacle");
            //        return;
            //    }
            //    else if(RayCastData.collider.gameObject.tag == "Enemy")
            //    {
            //        Debug.Log("HitEnemy");

            //        //Enemy 피격 로직.
            //        //죽기 전에 내부 클래스에서 리스트 삭제해주기.
            //        //리더가 죽을 경우 새리더 임명하고 죽기.
            //        //Destroy(RayCastData.collider.gameObject);
            //    }
            //}
        }
        else if(weaponMode == WeaponMode.SUB)
        {
            SubGunMuzzle.GetComponent<MeshRenderer>().enabled = true;
            SubGunMuzzle.GetComponent<AnimatedTexture>().PlayOneCircle();
            Damage = pistolDamage;
        }

        if(TargetFlock) //타겟으로 지정된 적이 있을 때
        {
            Quaternion rot = Quaternion.LookRotation(TargetFlock.transform.position - transform.position);

            transform.rotation = rot;

            TargetFlock.ShotFromPlayer(Damage);
        }
        else //타겟에 지정된 적이 없을 때
        {
            Debug.Log("Non target Shoot");
        }
    }

    Flock SearchTargetFlock()
    {
        if (DetectedEnemies.Count == 0)
            return null;
        else
        {
            // 플레이어와 거리순으로 정렬한 후
            foreach (Flock f in DetectedEnemies)
            {
                f.DistToPlayer = Vector3.Distance(f.transform.position, transform.position); //거리 측정.
            }

            DetectedEnemies = DetectedEnemies.OrderBy(p => p.DistToPlayer).ToList(); // 정렬.
            
            // 가까운 적부터 타겟에 적합한지 판정.
            foreach(Flock f in DetectedEnemies)
            {
                RaycastHit hit;
                int layerMask = 1 << 31; //31: player
                layerMask = ~layerMask;
                //적과의 각도가 시야각(fieldOfView)안에 드는지
                rayDirection = f.transform.position - transform.position;
                
                if(Vector3.Angle(rayDirection, transform.forward) < FieldOfView)
                {
                    //레이케스팅후 벽이 가로막고 있으면 다음 순번.
                    if(Physics.Raycast(transform.position, rayDirection, out hit, ViewDistance, layerMask))
                    {
                        if (hit.transform.tag != "Enemy")
                            continue;
                        else
                        {
                            if (hit.transform.GetComponent<Flock>().EnemyStat.Health > 0)
                            {
                               // Debug.Log("f Pos: " + f.transform.localPosition + "  f ID: " + f.GetComponent<Flock>().EID);
                                return f;
                            }
                            else
                                continue;
                        }
                    }
                }
                

            }

            return null;
        }
    }

    void DetectAspect()
    {
        //RaycastHit hit;

        //rayDirection = transform.position - Flock
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Enemy")
        {
            DetectedEnemies.Add(col.gameObject.GetComponent<Flock>());
        }


    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Enemy")
            DetectedEnemies.Remove(col.gameObject.GetComponent<Flock>());
    }

    void OnDrawGizmos()
    {
        if(PrimaryShotPoint)
            Debug.DrawRay(PrimaryShotPoint.position, PrimaryShotPoint.forward * RifleDist, Color.green);


        //Vector3 leftBottom = new Vector3(DetectionBound.min.x, 0f, DetectionBound.min.z);
        //Vector3 leftTop = new Vector3(DetectionBound.min.x, 0f, DetectionBound.max.z);
        //Vector3 rightBottom = new Vector3(DetectionBound.max.x, 0f, DetectionBound.min.z);
        //Vector3 rightTop = new Vector3(DetectionBound.max.x, 0f, DetectionBound.max.z);
        
        //Debug.DrawLine(leftBottom, rightBottom, Color.white);
        //Debug.DrawLine(rightBottom, rightTop, Color.white);
        //Debug.DrawLine(rightTop, leftTop, Color.white);
        //Debug.DrawLine(leftTop, leftBottom, Color.white);
    }
}
