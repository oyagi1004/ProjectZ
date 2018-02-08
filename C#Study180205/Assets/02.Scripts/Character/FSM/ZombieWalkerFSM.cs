using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieWalkerFSM : EnemyFSM {
    public enum FSMState
    {
        IDLE,
        EATING,
        PATROL,
        CHASE,
        ATTACK,
        DEAD,
    }

    public FSMState curState;

    private bool bDead;

    Enemy EnemyStat;

    int WanderID;

    public float AttackSpeedWeight;

    public float finalizeAttackDist;

    //public Node startNode { get; set; }
    //public Node goalNode { get; set; }

    //public ArrayList pathArray;

    private Vector3 startPos, endPos;

    private NavMeshAgent agent;

    protected override void Initialize()
    {
        wanderIndex = 0;
        curState = FSMState.PATROL;
        //testCode
        EnemyData.Read();
        EnemyStat = EnemyData.FindEnemyInfoByID(1);

        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");

        playerTransform = objPlayer.transform;

        if (!playerTransform)
            print("Player doesn't exist..");

        WanderID = 1;

        agent = GetComponent<NavMeshAgent>(); //FindObjectOfType(typeof(NavMeshAgent)) as NavMeshAgent;
        agent.speed = EnemyStat.Speed;

        //pathArray = new ArrayList();

        //최초 순회.
        FindNextPoint();


    }

    protected override void FSMUpdate()
    {
        switch(curState)
        {
            case FSMState.IDLE:
                break;
            case FSMState.CHASE:
                UpdateChaseState();
                break;
            case FSMState.PATROL:
                UpdatePatrolState();
                break;
            case FSMState.ATTACK:
                UpdateAttackState();
                break;
            case FSMState.DEAD:
                UpdateDeadState();
                break;
            case FSMState.EATING:
                UpdateEatingState();
                break;
        }

        DrawView();

        //시간 업데이트
        elapsedTime += Time.deltaTime;

        if (EnemyStat.Health <= 0)
            curState = FSMState.DEAD;
        FindPath();
    }

    public void DrawView()
    {
        Vector3 leftBoundary = DirFromAngle(-detectionAngle / 2);
        Vector3 rightBoundary = DirFromAngle(detectionAngle / 2);
        Debug.DrawLine(transform.position, transform.position + leftBoundary * ChaseDetectionDist, Color.blue);
        Debug.DrawLine(transform.position, transform.position + rightBoundary * ChaseDetectionDist, Color.blue);
        Debug.DrawLine(transform.position, transform.position + transform.forward * ChaseDetectionDist, Color.red);
    }

    public Vector3 DirFromAngle(float angleInDegrees)
    {
        //플레이어의 좌우 회전값 갱신
        angleInDegrees += transform.eulerAngles.y;
        //경계 벡터값 반환
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    protected void UpdatePatrolState()
    {
        agent.speed = EnemyStat.Speed;

        Vector3 dirToTarget = (playerTransform.position - transform.position).normalized;
        //Debug.Log("Chase Angle: " + Vector3.Angle(transform.forward, dirToTarget).ToString());
        Debug.DrawLine(transform.position, playerTransform.position, Color.green);
        // 플레이어와의 거리를 검사하고 충분히 근접하면 추격 상태로 전환한다.
        if (Vector3.Angle(transform.forward, dirToTarget) < detectionAngle / 2f)
        {
            //Debug.Log("Chase Dist: " + Vector3.Distance(transform.position, playerTransform.position).ToString());
            if(Vector3.Distance(transform.position, playerTransform.position) < ChaseDetectionDist)
            {
                curState = FSMState.CHASE;
                WanderID = 0;
                return;
            }
        }

        //Debug.Log("destPos dist: " + Vector3.Distance(transform.position, destPos).ToString());
        //현재 지점에 도달하면 임의의 다른 정찰 지점을 찾는다.
        if(Vector3.Distance(transform.position, destPos) <= NextPointOffset || WanderID == 0)
        {
            if (WanderID == pointList.Length)
                WanderID = 1;
            else
                WanderID++;
            FindNextPoint();
        }

        //목표지점으로 회전, 이동.
        //Quaternion targetRotation =
          //  Quaternion.LookRotation(destPos - transform.position);

        //transform.rotation = Quaternion.Slerp(transform.rotation,
        //    targetRotation, Time.deltaTime * rotSpeed);

        //transform.Translate(Vector3.forward * Time.deltaTime * EnemyStat.Speed);

        //Node node = (Node)pathArray[0];
        //transform.Translate(node.position * Time.deltaTime * EnemyStat.Speed, Space.World);
    }

    

    protected void FindNextPoint()
    {
        print("Finding next Point");

        foreach(GameObject obj in pointList)
        {
            if (WanderID == obj.GetComponent<WanderPoint>().ID)
                destPos = obj.transform.position;
        }

        
    }

    void FindPath()
    {
        startPos = transform.position;
        endPos = destPos;

        agent.destination = destPos;

        //startNode = new Node(GridManager.Instance.GetGridCellCenter(
        //    GridManager.Instance.GetGridIndex(startPos)));

        //goalNode = new Node(GridManager.Instance.GetGridCellCenter(
        //    GridManager.Instance.GetGridIndex(endPos)));

        //pathArray = AStar.FindPath(startNode, goalNode);
    }

    //void OnDrawGizmos()
    //{
    //    if(pathArray == null)
    //    {
    //        return;
    //    }

    //    if(pathArray.Count > 0)
    //    {
    //        int index = 1;
    //        foreach(Node node in pathArray)
    //        {
    //            if(index < pathArray.Count)
    //            {
    //                Node nextNode = (Node)pathArray[index];
    //                Debug.DrawLine(node.position, nextNode.position,
    //                    Color.green);
    //                index++;
    //            }
    //        }
    //    }
    //}

    protected void UpdateChaseState()
    {
        // 목표 지점을 플레이어의 위치로 설정한다.
        destPos = playerTransform.position;
        agent.speed = EnemyStat.Speed;
        //Quaternion rot = Quaternion.LookRotation(destPos - transform.position);

        //transform.rotation = rot;

        // 플레이어와의 거리를 검사하고, 일정 거리 이내에 진입하면 상태를 공격으로 바꾼다.
        float dist = Vector3.Distance(transform.position, destPos);

        if(dist <= AttackDetectionDist)
        {
            curState = FSMState.ATTACK;
        }

        if(dist >= ReturnChaseDist)
        {
            curState = FSMState.PATROL;
        }

        agent.destination = destPos;
        

        //transform.Translate(Vector3.forward * Time.deltaTime * EnemyStat.Speed);

    }

    protected void UpdateAttackState()
    {
        destPos = playerTransform.position;

        //Quaternion rot = Quaternion.LookRotation(destPos - transform.position);

        //transform.rotation = rot;

        //transform.Translate(Vector3.forward * Time.deltaTime * EnemyStat.Speed * AttackSpeedWeight);
        agent.speed = EnemyStat.Speed * AttackSpeedWeight;
        agent.destination = destPos;

        float dist = Vector3.Distance(transform.position, destPos);

        if (dist > ChaseDetectionDist)
        {
            curState = FSMState.CHASE;
        }

        if (dist >= ReturnChaseDist)
        {
            curState = FSMState.PATROL;
        }

        if (dist >= finalizeAttackDist)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        Debug.Log("Attack");
        //여기에서 애니메이션 이벤트 걸어서 해당 프레임에 컬라이더 깜빡깜빡!!
    }

    protected void UpdateDeadState()
    {
        //죽는 연출 (셰이더로 불꽃처럼 사그러드는 느낌도 괜츈할듯?)
    }

    protected void UpdateEatingState()
    {
        // 미끼나 섭취 가능한 오브젝트에 현혹되는 상태.(먹느라 플레이어에 주목하지 못함.)
    }
}
