using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    MapManager mapManager;
    UnitController unitController;
    Dictionary<Vector3Int, Node> gridMap;
    private static readonly Vector3Int[] directions =
    {
        new Vector3Int(1,0,0),
        new Vector3Int(-1,0,0),
        new Vector3Int(0,0,1),
        new Vector3Int(0,0,-1),
        new Vector3Int(1,0,1),
        new Vector3Int(1,0,-1),
        new Vector3Int(-1,0,1),
        new Vector3Int(-1,0,-1),
    };

    void Start()
    {
        mapManager = FindAnyObjectByType<MapManager>();
        unitController = GetComponent<UnitController>();
        gridMap = mapManager.GridMap;
    }

    public List<Node> FindPath(Node startNode, Node endNode)
    {
        foreach (var tile in gridMap.Values)
        {
            tile.G = 0; tile.H = 0; tile.previous = null;
        }

        List<Node> openList = new List<Node>(); //tiles to check
        HashSet<Node> closedList = new HashSet<Node>(); //cheked tiles

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            var currentNode = openList.OrderBy(n => n.F).First();

            if (currentNode == endNode)
                return BuildPath(endNode);

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbor in GetNeighbors(currentNode))
            {
                if (neighbor.Blocked || closedList.Contains(neighbor))
                    continue;

                
                if (Mathf.Abs(currentNode.Cords.y - neighbor.Cords.y) > unitController.jumpHight)
                    continue;

                if (!CanMoveDiagonal(currentNode, neighbor))
                    continue;

                int cost = GetMoveCost(currentNode, neighbor);
                int tentativeG = currentNode.G + cost;

                if (!openList.Contains(neighbor) || tentativeG < neighbor.G)
                {
                    neighbor.G = tentativeG;
                    neighbor.H = Heuristic(neighbor.Cords, endNode.Cords);
                    neighbor.previous = currentNode;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        return new List<Node>(); // путь не найден
    }

    List<Node> GetNeighbors(Node node)
    {
        List<Node> result = new List<Node>();
        foreach (var dir in directions)
        {
            Vector3Int check = node.Cords + dir;
            if (gridMap.ContainsKey(check))
                result.Add(gridMap[check]);
        }
        return result;
    }

    bool CanMoveDiagonal(Node current, Node neighbor)
    {
        int dx = neighbor.Cords.x - current.Cords.x;
        int dz = neighbor.Cords.z - current.Cords.z;

        if (dx == 0 || dz == 0) return true; 

        Vector3Int stepX = new Vector3Int(current.Cords.x + dx, current.Cords.y, current.Cords.z);
        Vector3Int stepZ = new Vector3Int(current.Cords.x, current.Cords.y, current.Cords.z + dz);

        return gridMap.ContainsKey(stepX) && !gridMap[stepX].Blocked &&
               gridMap.ContainsKey(stepZ) && !gridMap[stepZ].Blocked;
    }

    int GetMoveCost(Node from, Node to)
{
    int baseCost;

    // Если шаг диагональный
    if (from.Cords.x != to.Cords.x && from.Cords.z != to.Cords.z)
        baseCost = 14; // √2 * 10 округлённо
    else
        baseCost = 10;

    // Добавляем стоимость изменения высоты
    int heightDiff = Mathf.Abs(from.Cords.y - to.Cords.y);
    // По вкусу: 3-6 нормально. 5 — золотая середина.
    baseCost += heightDiff * 5;

    return baseCost;
}


    int Heuristic(Vector3Int a, Vector3Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dz = Mathf.Abs(a.z - b.z);
        return 10 * Mathf.Max(dx, dz);
    }

    List<Node> BuildPath(Node endNode)
    {
        List<Node> path = new List<Node>();
        Node curr = endNode;

        while (curr != null)
        {
            path.Add(curr);
            curr = curr.previous;
        }

        path.Reverse();
        return path;
    }
}
