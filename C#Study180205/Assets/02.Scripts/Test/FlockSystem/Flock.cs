using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class Flock : EnemyFSM
{
    internal FlockController controller;

    float Speed = 0f;

    Animator anim;

    public enum FlockState
    {
        IDLE,
        EATING,
        PATROL,
        CHASE,
        ATTACK,
        DEAD,
    }

    public int EID = 0;
    public float DistToPlayer = 0f;

    public FlockState curState;

    private bool bDead;

    public Enemy EnemyStat;

    public Bounds FlockBound;

    public float AvoidForce = 4f;

    Transform AvoidTr;

    public bool ImLeader = false;
    public bool bAvoid = false;

    private NavMeshAgent agent;
    private Rigidbody rigidBody;
    protected override void Initialize()
    {
        Enemy data = EnemyData.FindEnemyInfoByID(1);
        EnemyStat = new Enemy(data.Id, data.Name, data.Health, data.Stemina, data.Defence, data.Strength, data.Concentration, data.Speed, data.Path);

        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        Speed = Random.Range(3f, 3f);
        agent = GetComponent<NavMeshAgent>();
        agent.speed = Speed;

        curState = FlockState.IDLE;

        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");

        playerTransform = objPlayer.transform;

        if (!playerTransform)
            print("Player doesn't exist..");

    }

    public bool CheckBound()
    {
        FlockBound = GetComponentInChildren<SkinnedMeshRenderer>().bounds;

        for (int i = 0; i < controller.ObstacleList.Length; i++)
        {
            if (FlockBound.Intersects(controller.ObstacleList[i].GetComponent<MeshRenderer>().bounds))
            {
                AvoidTr = controller.ObstacleList[i].transform;

                return true;
            }
        }

        return false;
    }

    public Vector3 AvoidObstacle(Transform avoidTr)
    {
        RaycastHit hit;
        int layerMask = 1 << 10; //10:obstacle

        Vector3 dir = Vector3.zero;

        if (Physics.Raycast(transform.position, (avoidTr.position - transform.position),
            out hit, Vector3.Distance(transform.position, (avoidTr.position - transform.position)), layerMask))
        {
            Vector3 hitNormal = hit.normal;
            hitNormal.y = 0f;
            dir = transform.forward + hitNormal * AvoidForce;
        }
        return dir;
    }



    protected override void FSMUpdate()
    {
        if (curState != FlockState.DEAD)
        {
            SetAttackMass();
        }

        if (controller)
        {
            switch (curState)
            {
                case FlockState.IDLE:
                    UpdateIdleState();
                    break;
                case FlockState.EATING:
                    UpdateEatingState();
                    break;
                case FlockState.PATROL:
                    UpdatePatrolState();
                    break;
                case FlockState.CHASE:
                    UpdateChaseState();
                    break;
                case FlockState.ATTACK:
                    UpdateAttackState();
                    break;
                case FlockState.DEAD:
                    UpdateDeadState();
                    break;
            }
        }
    }

    void UpdateIdleState()
    {
        if (curState != FlockState.DEAD)
        {
            if (!anim.GetBool("Idle"))
                anim.SetBool("Idle", true);

            if (anim.GetBool("Chase"))
                anim.SetBool("Chase", false);

            if (anim.GetBool("Attack"))
                anim.SetBool("Attack", false);

            if (!agent.isStopped)
                agent.isStopped = true;
        }
    }

    void UpdateChaseState()
    {
        if(curState != FlockState.DEAD)
        {
            if (!anim.GetBool("Chase"))
                anim.SetBool("Chase", true);

            if (anim.GetBool("Idle"))
                anim.SetBool("Idle", false);
            if (anim.GetBool("Attack"))
                anim.SetBool("Attack", false);

            if (agent.isStopped)
                agent.isStopped = false;

            Vector3 randomWeight = Vector3.zero; // PC 주변의 올바른 보정값을 알아낸뒤 수정.

            //agent.destination = playerTransform.position + randomWeight;
        }

    }

    void UpdateEatingState()
    {
        
    }

    void UpdatePatrolState()
    {

    }

    void UpdateAttackState()
    {
        if(curState != FlockState.DEAD)
        {
            if(!anim.GetBool("Attack"))
                anim.SetBool("Attack", true);

            if (anim.GetBool("Idle"))
                anim.SetBool("Idle", false);
            if (anim.GetBool("Chase"))
                anim.SetBool("Chase", false);

            if (!agent.isStopped)
                agent.isStopped = true;

            Debug.Log("Attack!!");

            Quaternion rot = Quaternion.LookRotation((playerTransform.position - transform.position).normalized);

            transform.rotation = rot;
        }
    }

    public void SetAttackMass()
    {
        //공격 상태일때 다른 오브젝트들에 밀리지 않도록 일시적으로 중량을 높게 셋팅한다.
        if (curState == FlockState.ATTACK)
        {
            rigidBody.mass = 1000;
        }
        else
        {
            rigidBody.mass = 1;
        }
    }

    void UpdateDeadState()
    {
        if (agent.enabled)
            agent.enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        anim.SetBool("Idle", false);
        anim.SetBool("Attack", false);
        anim.SetBool("Chase", false);
        anim.SetBool("Death", true);
        EquipPlayerManager.Instance.DetectedEnemies.Remove(controller.FindFlockByID(EID));
        controller.flockList.Remove(controller.FindFlockByID(EID));
        Destroy(gameObject, 10f);
    }

    public void ShotFromPlayer(float _damage)
    {
        Quaternion rot = Quaternion.LookRotation(EquipPlayerManager.Instance.transform.position - transform.position);
        transform.rotation = rot;

        EnemyStat.Health -= (int)_damage;

        if (EnemyStat.Health > 0)
            anim.SetTrigger("Hit");
        else
            curState = FlockState.DEAD;
    }

    void OnDrawGizmos()
    {
        //Vector3 leftBottom = FlockBound.min;
        //Vector3 leftTop = new Vector3(FlockBound.min.x, 0f, FlockBound.max.z);
        //Vector3 rightBottom = new Vector3(FlockBound.max.x, 0f, FlockBound.min.z);
        //Vector3 rightTop = new Vector3(FlockBound.max.x, 0f, FlockBound.max.z);
        //Debug.DrawLine(leftBottom, rightBottom, Color.white);
        //Debug.DrawLine(rightBottom, rightTop, Color.white);
        //Debug.DrawLine(rightTop, leftTop, Color.white);
        //Debug.DrawLine(leftTop, leftBottom, Color.white);
    }
}
