using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    private List<Node> _visited = new List<Node>();
    private List<Node> _notVisited = new List<Node>();
    private List<Node> _path = new List<Node>();
    private NodesGenerator _nodesGenerator;
    private Node initialNode;
    private Node finalNode;

    private void Start()
    {
        _nodesGenerator = FindObjectOfType<NodesGenerator>();
    }
    
    public List<Node> GetPath(Node initial, Node finalNode)
    {

        _nodesGenerator.grid.ForEach(x => x.ClearNode());
        _visited.ForEach(x => x.ClearNode());
        _notVisited.ForEach(x => x.ClearNode());
        /*foreach (var item in _nodesGenerator.grid)
        {
            item.ClearNode();
        }

        foreach (var item in _visited)
        {
            item.ClearNode();
        }

        foreach (var item in _notVisited)
        {
            item.ClearNode();
        }*/

        _path.Clear();
        _visited.Clear();
        _notVisited.Clear();
        _notVisited.Add(initial);
        initial.g = 0f;

        while (_notVisited.Count > 0)
        {
            Node current = SearchNextNode();
            _visited.Add(current);
            foreach (var item in current.neighbors)
            {
                if (_visited.Contains(item))
                    continue;

                if (!_notVisited.Contains(item))
                {
                    _notVisited.Add(item);
                    item.h = Mathf.Abs((finalNode.transform.position -
                        item.transform.position).magnitude);
                }
                ComputeCost(current,item);
                if (_notVisited.Contains(finalNode))
                {
                    _path.Add(finalNode);
                    Node node = finalNode.previous;
                    while (node)
                    {
                        _path.Insert(0, node);
                        node = node.previous;
                    }
                }
            }
        }
        return _path;
    }

    private void ComputeCost(Node current, Node item)
    {
        if (current.previous && item.previous && LineOfSight(current.previous, item))
        {
            Debug.Log("ENTRA EN LINE OF SIGHT");
            float distanceToNeighbor = Vector3.Distance(current.previous.transform.position, item.transform.position);
            float newG = distanceToNeighbor + current.previous.g;
            if (newG < item.g)
            {
                item.previous.g = newG;
                item.previous = current;
                item.f = item.previous.g + item.previous.h;
            }
        }
        else
        {
            Debug.Log("ENTRA EN A*");
            float distanceToNeighbor = Vector3.Distance(current.transform.position, item.transform.position);
            float newG = distanceToNeighbor + current.g;
            if (newG < item.g)
            {
                item.g = newG;
                item.previous = current;
                item.f = item.g + item.h;
            }
        }
    }

    private bool LineOfSight(Node previous, Node item)
    {
        int previousX = previous.x;
        int previousY = previous.y;

        int itemY = item.y;
        int itemX = item.x;
        int deltaX = previousX - itemX;
        int deltaY = previousY - itemY;
        float f = 0;

        if (deltaY < 0)
        {
            deltaY = -deltaY;
            previous.y = -1;
        }
        else
        {
            previous.y = 1;
        }
        if (deltaX < 0)
        {
            deltaX = -deltaX;
            previous.x = -1;
        }
        else
        {
            previous.x = 1;
        }
        if (deltaX <= deltaY)
        {
            while (previousX != itemX)
            {
                f = f + deltaY;
                if (f >= deltaX)
                {
                    Debug.Log("COORD X: " + (previousX + ((previous.x - 1) / 2)));
                    
                    if (_nodesGenerator.gridMatrix[getCoordX(previousX, previous.x), getCoordY(previousY, previous.y)])
                        return false;

                    previousY = previousY + previous.y;
                    f = f - deltaX;
                }
                if (f != 0 && _nodesGenerator.gridMatrix[getCoordX(previousX, previous.x), getCoordY(previousY, previous.y)])
                    return false;

                if (deltaY == 0 && _nodesGenerator.gridMatrix[getCoordX(previousX, previous.x), previousY] && _nodesGenerator.gridMatrix[getCoordX(previousX, previous.x), previousY - 1])
                    return false;

                previousX = previousX + previous.x;
                previous.x = previousX;
            }
        }
        else
        {
            while (previousY != itemY)
            {
                f = f + deltaX;
                if (f >= deltaY)
                {
                    Debug.Log("COORD Y: " + (previousY + ((previous.y - 1) / 2)));
                    if (_nodesGenerator.gridMatrix[getCoordX(previousX, previous.x), getCoordY(previousY, previous.y)])
                        return false;

                    previousX = previousX + previous.x;
                    f = f - deltaX;
                }
                if (f != 0 && _nodesGenerator.gridMatrix[getCoordX(previousX, previous.x), getCoordY(previousY, previous.y)])
                    return false;

                if (deltaY == 0 && _nodesGenerator.gridMatrix[getCoordX(previousX, previous.x), previousY] && _nodesGenerator.gridMatrix[getCoordX(previousX, previous.x), previousY - 1])
                    return false;

                previousY = previousY + previous.y;
                previous.y = previousY;
            }
        }

        return true;
    }

    public int getCoordX(int x0,int s0)
    {
        return ((x0 + ((s0 - 1) / 2)) < 0) ? 0 : (x0 + ((s0 - 1) / 2));
    }

    public int getCoordY(int y0, int s0)
    {
        return ((y0 + ((s0 - 1) / 2)) < 0) ? 0 : (y0 + ((s0 - 1) / 2));
    }

    private Node SearchNextNode()
    {
        Node n = _notVisited[0];
        for (int i = 0; i < _notVisited.Count; i++)
        {
            if (_notVisited[i].f < n.f)
            {
                n = _notVisited[i];
            }
        }
        _notVisited.Remove(n);
        return n;
    }

}
