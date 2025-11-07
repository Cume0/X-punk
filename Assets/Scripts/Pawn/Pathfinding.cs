using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    MapManager mapManager;
    UnitController unitController;
    Dictionary<Vector3Int, Node> gridMap = new Dictionary<Vector3Int, Node>();

    Vector3Int[] searchOrder = {Vector3Int.right, Vector3Int.left, Vector3Int.forward, Vector3Int.back, Vector3Int.up, Vector3Int.down};
    void Start()
    {
        mapManager = FindAnyObjectByType<MapManager>();
        if (mapManager == null) return;

        gridMap = mapManager.GridMap;

        unitController = GetComponent<UnitController>();
    }
    
    public List<Node> FindPath(Node startNode, Node endNode)
    {
        List<Node> toCheckList = new List<Node>();
        List<Node> checkedList = new List<Node>();

        toCheckList.Add(startNode);

        while (toCheckList.Count > 0)
        {
            Node thisNode = toCheckList.OrderBy(x => x.F).First();

            toCheckList.Remove(thisNode);
            checkedList.Add(thisNode);

            if (thisNode == endNode)
            {
                return BuildPath(startNode, endNode);
            }

            var neighbourNodes = GetNeighbourNodes(thisNode);

            foreach (var neighbourNode in neighbourNodes)
            {
                if (neighbourNode.Blocked == true || checkedList.Contains(neighbourNode) || MathF.Abs(thisNode.Cords.z - neighbourNode.Cords.z) > unitController.jumpHight)
                {
                    continue;
                }

                neighbourNode.G = GetManhattanDistance(startNode, neighbourNode);
                neighbourNode.H = GetManhattanDistance(endNode, neighbourNode);

                neighbourNode.previous = thisNode;

                if (!toCheckList.Contains(neighbourNode))
                {
                    toCheckList.Add(neighbourNode);
                }
            }
        }

        return new List<Node>();
    }

    public List<Node> GetNeighbourNodes(Node thisNode)
    {
        List<Node> neighbors = new List<Node>();

        foreach (Vector3Int direction in searchOrder)
        {
            Vector3Int neighborCords = thisNode.Cords + direction;

            if (gridMap.ContainsKey(neighborCords))
            {
                neighbors.Add(gridMap[neighborCords]);
            }
        }

        return neighbors;
    }

    private List<Node> BuildPath(Node startNode, Node endNode)
    {
        List<Node> Path = new List<Node>();

        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            Path.Add(currentNode);
            currentNode = currentNode.previous;
        }

        Path.Reverse();

        return Path;
    }
    
    public int GetManhattanDistance(Node Start, Node Neighbour)
    {
        return (int)(MathF.Abs(Start.Cords.x - Neighbour.Cords.x) + MathF.Abs(Start.Cords.y - Neighbour.Cords.y) + MathF.Abs(Start.Cords.z - Neighbour.Cords.z));
    }
}   