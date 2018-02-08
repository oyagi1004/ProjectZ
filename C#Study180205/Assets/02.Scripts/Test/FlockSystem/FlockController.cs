using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlockController : MonoBehaviour
{
    public int FID = 0;

    public int flockSize = 20;

    //그룹내 군집의 중앙 위치.
    internal Vector3 flockCenter;
    public Transform target;
    public List<Flock> flockList = new List<Flock>();
    public Flock leaderFlock;

    public Flock prefab;

    public float ChaseDistance;
    public float AttackDistance;

    public Vector3 MovingDir;

    private Bounds flockBound;

    public Bounds FlockBound
    {
        get { return flockBound; }
    }

    private bool b_FlockInPlayer = false;

    public bool B_FLockInPlayer
    {
        get { return b_FlockInPlayer; }
    }

    Color DebugColor = Color.green;

    public FlockGrid grid;

    public SphereCollider ObstacleDetecter;

    public GameObject[] ObstacleList;

    //플레어에 가장 근접한 플록.
    //public Flock ClosestFlock;

   // Use this for initialization
    void Start()
    {
        //TempCode
        EnemyData.Read();

        grid = GetComponent<FlockGrid>();
        grid.InitGrid();

        for (int i = 0; i < flockSize; i++)
        {
            Quaternion randRot = Quaternion.Euler(0f, Random.Range(0f, 90f), 0f);
            Flock flock = Instantiate(prefab, transform.position, randRot) as Flock;
            flock.transform.parent = transform;
            flock.controller = this;
            flock.EID = i;
            flockList.Add(flock);

            FindFlockPosition(flock);
        }

        target = GameObject.Find("PlayerBase").transform;

        StartCoroutine(CheckBoundIncludingChilds());


        if (flockSize >= grid.nodes.Length * 3)
            flockSize = grid.nodes.Length * 3;

        ObstacleDetecter.radius = (grid.numOfColumns > grid.numOfRows) ? grid.numOfColumns : grid.numOfRows;

        ObstacleList = GameObject.FindGameObjectsWithTag("Obstacle");
    }

    IEnumerator CheckBoundIncludingChilds()
    {
        flockBound = GetMaxBounds(flockList);
        b_FlockInPlayer = CheckFlockInPlayer();

        if (b_FlockInPlayer)
            DebugColor = Color.red;
        else
            DebugColor = Color.green;

        yield return new WaitForSeconds(0.1f);
        if (flockList.Count != 0)
            StartCoroutine(CheckBoundIncludingChilds());
        else
            yield return null;
    }

    Bounds GetMaxBounds(List<Flock> list)
    {
        Bounds b = new Bounds();

        foreach (Flock f in list)
        {
            Renderer r = f.GetComponentInChildren<Renderer>();
            if (b.center == Vector3.zero)
            {
                b = new Bounds(f.transform.position, Vector3.zero);
                //f.ImLeader = true;
                //leaderFlock = f;
            }
            else
            {
                b.Encapsulate(r.bounds);
                //f.ImLeader = false;
            }
        }

        return b;
    }


    bool CheckFlockInPlayer()
    {
        return FlockBound.Contains(target.position);
    }

    void FindFlockPosition(Flock flock)
    {
        FlockNode curNode = grid.GetNodeByIndex(grid.GetGridIndex(flock.transform.position));
        grid.findStep = 1;
        
        if (curNode.unitInNode < 3)
        {
            curNode.SetUnitInNode(flock);
            return;
        }
        else
            FindClosedEmptyNode(curNode, flock);
    }
    
    void FindClosedEmptyNode(FlockNode node, Flock flock)
    {
        
        ArrayList neighbors = new ArrayList();
        grid.GetNeighbors(node, neighbors);

        for(int i = 0; i < neighbors.Count; i++)
        {
            FlockNode neighborNode = (FlockNode)neighbors[i];
            if(neighborNode.unitInNode < 3)
            {
                neighborNode.SetUnitInNode(flock);
                return;
            }
        }
        if(grid.findStep < (grid.numOfColumns > grid.numOfRows ? grid.numOfColumns : grid.numOfRows ))
            grid.findStep++;
        else
        {
            for(int i = 0; i < grid.numOfColumns; i++)
            {
                for(int j = 0; j < grid.numOfRows; j++)
                {
                    if(grid.nodes[i,j].unitInNode < 3)
                    {
                        grid.nodes[i, j].SetUnitInNode(flock);
                        return;
                    }
                }
            }
            Debug.Log("FindEmptyNodeFalied.");
        }
        FindClosedEmptyNode(node, flock);
    }

    // Update is called once per frame
    void Update()
    {
        CheckFlockState();
    }

    void CheckFlockState()
    {
        float dist = Vector3.Distance(flockBound.ClosestPoint(target.transform.position), target.transform.position);

        //향후 시야 추가.
        foreach (Flock flock in flockList)
        {
            if(flock.curState != Flock.FlockState.DEAD)
            {
                if (dist >= ChaseDistance)
                {
                    flock.curState = Flock.FlockState.IDLE;
                }
                if (dist < ChaseDistance)
                {
                    float fDist = Vector3.Distance(flock.transform.position, target.transform.position);
                    if (fDist >= AttackDistance)
                        flock.curState = Flock.FlockState.CHASE;
                    else
                        flock.curState = Flock.FlockState.ATTACK;
                }
                //if (dist <= AttackDistance)
                //{
                //    flock.curState = Flock.FlockState.ATTACK;
                //}
            }

            //flock.DistToPlayer = Vector3.Distance(flock.transform.position, target.transform.position);

            //if(ClosestFlock == null)
            //{
            //    ClosestFlock = flock;
            //}
            //else
            //{
            //    ClosestFlock = (flock.DistToPlayer > ClosestFlock.DistToPlayer ? flock : ClosestFlock);
            //}
        }
    }

    public Flock FindFlockByID(int id)
    {
        foreach(Flock f in flockList)
        {
            if (f.EID == id)
                return f;
        }
        return null;
    }

    void OnDrawGizmos()
    {
        Vector3 leftBottom = flockBound.min;
        Vector3 leftTop = new Vector3(flockBound.min.x, 0f, flockBound.max.z);
        Vector3 rightBottom = new Vector3(flockBound.max.x, 0f, flockBound.min.z);
        Vector3 rightTop = new Vector3(flockBound.max.x, 0f, flockBound.max.z);
        Debug.DrawLine(leftBottom, rightBottom, DebugColor);
        Debug.DrawLine(rightBottom, rightTop, DebugColor);
        Debug.DrawLine(rightTop, leftTop, DebugColor);
        Debug.DrawLine(leftTop, leftBottom, DebugColor);

        if(grid && grid.showGrid)
        {
            grid.DebugDrawGrid(transform.position, grid.numOfRows, grid.numOfColumns, grid.gridCellSize, Color.white);
            
        }
    }
}
