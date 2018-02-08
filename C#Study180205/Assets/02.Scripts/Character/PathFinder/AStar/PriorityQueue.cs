using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue {
    private ArrayList nodes = new ArrayList(); 

    public int Length
    {
        get { return this.nodes.Count; }
    }

    public bool Contains(object node)
    {
        return this.nodes.Contains(node);
    }

    public Node First()
    {
        if(this.nodes.Count > 0)
        {
            return (Node)this.nodes[0];
        }
        return null;
    }

    public void Push(Node node)
    {
        this.nodes.Add(node);
        this.nodes.Sort();


    }

    public void Remove(Node node)
    {
        this.nodes.Remove(node);
        // 리스트를 정렬한다.
        this.nodes.Sort();
    }

    //삽입, 삭제시 ArrayList의 Sort를 호출한다. Sort()는 Node에 정의된 ComparTo메소드를 호출해 estiamteCost값에 따라 노드를 정렬한다.
}
