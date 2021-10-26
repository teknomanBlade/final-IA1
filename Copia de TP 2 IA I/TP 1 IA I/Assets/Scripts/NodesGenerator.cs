using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodesGenerator : MonoBehaviour
{
    public static bool IsFirst = true;
    public static int Counter = 0;
    public List<Node> grid = new List<Node>();
    public Node[,] gridMatrix;

    // Start is called before the first frame update
    void Awake()
    {
        gridMatrix = new Node[12, 12];
        var numRows = gridMatrix.GetLength(0);
        var numCols = gridMatrix.GetLength(1);
        //Debug.Log(numRows);
        //Debug.Log(numCols);
        int index = grid.Count - 1;
        for (int i = 0; i < numRows; ++i)
        {
            for (int j = 0; j < numCols; ++j)
            {
                gridMatrix[i, j] = grid[index];
                gridMatrix[i, j].x = i;
                //Debug.Log("VALUE COORD X: " + i);
                gridMatrix[i, j].y = j;
                //Debug.Log("VALUE COORD Y: " + j);
                //Debug.Log("VALUE GRID: " + gridMatrix[i, j]);
                index--;
            }
        }
        EventsManager.SubscribeToEvent("OnReachEndPath", OnReachEndPath);
        StartCoroutine(DrawNeighbours());
    }

    private void OnReachEndPath(object[] parameterContainer)
    {
        foreach (var item in grid)
        {
            item.isPath = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DrawNeighbours()
    {
        yield return new WaitForSeconds(0.2f);
        Debug.Log("My Grid Length " + grid.Count);
        foreach (var e in grid)
        {
            e.x = 0;
            e.y = 0;
            e.CheckNeighbors();
        }
    }
}
