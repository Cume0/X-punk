using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    MapManager mapManager;
    Dictionary<Vector3Int, Node> gridMap = new Dictionary<Vector3Int, Node>();

    Vector3Int[] searchOrder = {
        Vector3Int.right, Vector3Int.left, Vector3Int.forward, Vector3Int.back, Vector3Int.up, Vector3Int.down,
        Vector3Int.right + Vector3Int.forward, Vector3Int.left + Vector3Int.forward, Vector3Int.right + Vector3Int.back, Vector3Int.left + Vector3Int.back,
        Vector3Int.up + Vector3Int.forward, Vector3Int.down + Vector3Int.forward, Vector3Int.up + Vector3Int.back, Vector3Int.down + Vector3Int.back,
        Vector3Int.right + Vector3Int.up, Vector3Int.right + Vector3Int.down, Vector3Int.left + Vector3Int.up, Vector3Int.left + Vector3Int.down,};
    void Awake()
    {
        mapManager = FindAnyObjectByType<MapManager>();
        if (mapManager == null) return;

        gridMap = mapManager.GridMap;
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

            }

            var neigbourNodes = GetNeighbourNodes(thisNode);

            foreach (var neigbourNode in neigbourNodes)
            {
                if (neigbourNode.Blocked == false)
                {
                    
                }
            }
        }

        return null;
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
}   
