using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockNode : Node {

    public FlockNode()
    {
        this.estimatedCost = 0f;
        this.nodeTotalCost = 1f;
        this.bObstacle = false;
        this.parent = null;
    }

    public FlockNode(Vector3 pos)
    {
        this.estimatedCost = 0f;
        this.nodeTotalCost = 1f;
        this.bObstacle = false;
        this.parent = null;
        this.position = pos;
    }

    public Vector3 SoloPos;
    public Vector3[] DuoPos = new Vector3[2];
    public Vector3[] TrioPos = new Vector3[3];

    public int unitInNode = 0;

    Flock[] units = new Flock[3];

    public int index = 0;

    public void SetUnitInNode(Flock obj)
    {
        if(unitInNode == 0)
        {
            obj.transform.position = (SoloPos);
            units[0] = obj;
            unitInNode++;
        }
        else if(unitInNode == 1)
        {
            units[0].transform.position = (DuoPos[0]);
            obj.transform.position = (DuoPos[1]);
            units[1] = obj;
            unitInNode++;
        }
        else if(unitInNode == 2)
        {
            units[0].transform.position = (TrioPos[0]);
            units[1].transform.position = (TrioPos[1]);
            obj.transform.position = (TrioPos[2]);
            units[2] = obj;
            unitInNode++;
        }
        else
        {
            Debug.Log("Node Full!! Find Empty Node.");
        }
    }
    

}
