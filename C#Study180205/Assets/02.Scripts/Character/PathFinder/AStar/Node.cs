using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node : IComparable
{
    public float nodeTotalCost;
    public float estimatedCost;
    public bool bObstacle;
    public Node parent;
    public Vector3 position;
    public int MobNum = 0;

    public Node()
    {
        this.estimatedCost = 0f;
        this.nodeTotalCost = 1f;
        this.bObstacle = false;
        this.parent = null;
    }

    public Node(Vector3 pos)
    {
        this.estimatedCost = 0f;
        this.nodeTotalCost = 1f;
        this.bObstacle = false;
        this.parent = null;
        this.position = pos;
    }

    public void MarkAsObstacle()
    {
        this.bObstacle = true;
    }

    public int CompareTo(object obj)
    {
        Node node = (Node)obj;
        // 음수 값은 오브젝트가 정렬된 상태에서 현재보다 앞에 있음을 의미한다.
        if (this.estimatedCost < node.estimatedCost)
            return -1;
        // 양수 값은 오브젝트가 정렬된 상태에서 현재보다 뒤에 있음을 의미한다.
        if (this.estimatedCost > node.estimatedCost)
            return 1;
        return 0;
    }
}
