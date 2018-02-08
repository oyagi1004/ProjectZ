using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockGrid : MonoBehaviour {
    public int numOfRows;
    public int numOfColumns;
    public float gridCellSize;
    public bool showGrid = true;
    public bool showObstacleBlocks = true;

    private Vector3 origin = Vector3.zero;
    public Vector3 Origin
    {
        get { return origin; }
    }

    public FlockNode[,] nodes { get; set; }

    public Vector3[] MobposInNode;

    private GameObject[] obstacleList;

    //장애물 검출을 그리드 기준으로 
    //1. 플록 콘트롤러 주변에 있는 방해물을 수시로 검출
    //2. 각 그리드에 해당 오브젝트들을 대입하여 비교 (AABB) 
    //3. 그리드에 방해물이 존재시 체크.
    //4. 노드에 몹 포지션 셋팅(최대 허용 기준치까지 미리 셋팅 - 노트 참고.)
    
    public int findStep = 1;

    public void InitGrid()
    {
        origin = transform.position;
        SetNode();
        StartCoroutine(SetGrid());
    }

    public void SetNode()
    {
        nodes = new FlockNode[numOfColumns, numOfRows];
    }

    //정해진 시간마다 그리드를 갱신하고 갱신한 그리드를 바탕으로 장애물을 찾아낸다.
    IEnumerator SetGrid()
    {
        int index = 0;
        for(int i = 0; i < numOfColumns; i++)
        {
            for(int j = 0; j < numOfRows; j++)
            {
                Vector3 cellPos = GetGridCellCenter(index);
                FlockNode node = new FlockNode(cellPos);
                node.SoloPos = node.position;

                node.DuoPos[0] = new Vector3(node.position.x - (gridCellSize * 0.25f), 0f, node.position.z);
                node.DuoPos[1] = new Vector3(node.position.x + (gridCellSize * 0.25f), 0f, node.position.z);

                node.TrioPos[0] = new Vector3(node.position.x - (gridCellSize * 0.25f), 0f,
                    node.position.z - (gridCellSize * 0.25f));
                node.TrioPos[1] = new Vector3(node.position.x + (gridCellSize * 0.25f), 0f,
                    node.position.z - (gridCellSize * 0.25f));
                node.TrioPos[2] = new Vector3(node.position.x, 0f, node.position.z + (gridCellSize * 0.25f));

                nodes[i, j] = node;

                node.index = (i * numOfColumns) + j;
                nodes[i, j] = node;
                index++;
            }
        }
        yield return null; 
        //yield return new WaitForSeconds(1f); //연속적으로 그리드를 갱신해야 할때 사용.
    }

    public FlockNode GetNodeByIndex(int index)
    {
        foreach(FlockNode node in nodes)
        {
            if (node.index == index)
                return node;
        }

        Debug.Log("Do not find node in this index: " + index.ToString());
        return null;
    }

    public Vector3 GetGridCellCenter(int index)
    {
        int row = GetRow(index);
        int col = GetColumn(index);
        Vector3 cellPosition = GetGridCellPosition(index);
        if(numOfRows % 2 == 1)
            cellPosition.x -= ((gridCellSize) * (numOfRows / 2));
        else
            cellPosition.x  -= ((gridCellSize) * (numOfRows / 2) - (gridCellSize) / 2);
        if (numOfColumns % 2 == 1 )
            cellPosition.z -= ((gridCellSize) * (numOfColumns / 2));
        else
            cellPosition.z -= ((gridCellSize) * (numOfColumns / 2) - (gridCellSize) / 2);
        return cellPosition;
    }

    public Vector3 GetGridCellPosition(int index)
    {
        int row = GetRow(index);
        int col = GetColumn(index);
        float xPosInGrid = col * gridCellSize;
        float zPosInGrid = row * gridCellSize;
        return Origin + new Vector3(xPosInGrid, 0f, zPosInGrid);
    }
    
    public int GetRow(int index)
    {
        int row = index / numOfColumns;
        return row;
    }

    public int GetColumn(int index)
    {
        int col = index % numOfColumns;
        return col;
    }

    public int GetGridIndex(Vector3 pos)
    {

        if (!IsInBounds(pos))
        {
            return -1;
        }
        Vector3 StartNodePos = new Vector3(Origin.x - (gridCellSize * (numOfColumns / 2f)), 0f,
            Origin.z - (gridCellSize * (numOfRows / 2f)) );

        pos -= StartNodePos;
        int col = (int)(pos.x / gridCellSize);
        int row = (int)(pos.z / gridCellSize);

        return (row * numOfColumns + col);
    }

    public bool IsInBounds(Vector3 pos)
    {
        float width = numOfColumns * gridCellSize;
        float height = numOfColumns * gridCellSize;
        return (pos.x >= Origin.x && pos.x <= Origin.x + width &&
            pos.z <= Origin.z + height && pos.z >= Origin.z);
    }

    public void GetNeighbors(Node node, ArrayList neighbors)
    {
        Vector3 neighborPos = node.position;
        int neighborIndex = GetGridIndex(neighborPos);

        int row = GetRow(neighborIndex);
        int column = GetColumn(neighborIndex);

        //아래 
        int leftNodeRow = row - findStep;
        int leftNodeColumn = column;

        if (leftNodeRow < 0)
            leftNodeRow = 0;
        AssignNeighbor(leftNodeRow, leftNodeColumn, neighbors);

        //위 
        leftNodeRow = row + findStep;
        leftNodeColumn = column;
        AssignNeighbor(leftNodeRow, leftNodeColumn, neighbors);

        //오른쪽 
        leftNodeRow = row;
        leftNodeColumn = column + findStep;
        AssignNeighbor(leftNodeRow, leftNodeColumn, neighbors);

        //왼쪽
        leftNodeRow = row;
        leftNodeColumn = column - findStep;

        if (leftNodeColumn < 0)
            leftNodeColumn = 0;
        AssignNeighbor(leftNodeRow, leftNodeColumn, neighbors);

        //북동쪽
        leftNodeRow = row + findStep;
        leftNodeColumn = column + findStep;
        AssignNeighbor(leftNodeRow, leftNodeColumn, neighbors);

        //북서쪽
        leftNodeRow = row + findStep;
        leftNodeColumn = column - findStep;

        if (leftNodeColumn < 0)
            leftNodeColumn = 0;
        AssignNeighbor(leftNodeRow, leftNodeColumn, neighbors);

        //남동쪽
        leftNodeRow = row - findStep;
        leftNodeColumn = column + findStep;

        if (leftNodeRow < 0)
            leftNodeRow = 0;
        AssignNeighbor(leftNodeRow, leftNodeColumn, neighbors);

        //남서쪽
        leftNodeRow = row - findStep;
        leftNodeColumn = column - findStep;

        if (leftNodeRow < 0)
            leftNodeRow = 0;
        if (leftNodeColumn < 0)
            leftNodeColumn = 0;
        AssignNeighbor(leftNodeRow, leftNodeColumn, neighbors);
    }

    void AssignNeighbor(int row, int column, ArrayList neighbors)
    {
        if(row != -1 && column != -1 &&
            row < numOfRows && column < numOfColumns)
        {
            Node nodeToAdd = nodes[row, column];
            if (!nodeToAdd.bObstacle)
                neighbors.Add(nodeToAdd);
        }
    }

    public void DebugDrawGrid(Vector3 origin, int numRows, int numCols, float cellSize, Color color)
    {
        float width = (numCols * cellSize);
        float height = (numRows * cellSize);

        float startX = origin.x - ((numRows * cellSize) / 2f);
        float startZ = origin.z - ((numCols * cellSize) / 2f);

        // 수평 격자 라인을 그린다.
        for (int i = 0; i < numRows + 1; i++)
        {
            Vector3 startPos = new Vector3(startX, 0, startZ) + i * cellSize * new Vector3(0f, 0f, 1f);
            Vector3 endPos = startPos + width * new Vector3(1f, 0f, 0f);
            Debug.DrawLine(startPos, endPos, color);
        }

        // 수직 격자 라인을 그린다.
        for (int i = 0; i < numCols + 1; i++)
        {
            Vector3 startPos = new Vector3(startX, 0, startZ) + i * cellSize * new Vector3(1f, 0f, 0f);
            Vector3 endPos = startPos + height * new Vector3(0f, 0f, 1f);
            Debug.DrawLine(startPos, endPos, color);
        }

        foreach (FlockNode node in nodes)
        {
            Gizmos.DrawCube(node.position, new Vector3(0.2f, 0.2f, 0.2f));
        }
    }

}
