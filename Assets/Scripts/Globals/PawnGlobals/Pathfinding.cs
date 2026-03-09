using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    MapManager mapManager;
    Dictionary<Vector3Int, Tile> gridMap;
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
        new Vector3Int(0,1,0),
        new Vector3Int(0,-1,0)
    };

    public void Initialize()
    {
        mapManager = GameManager.mapManager;
        gridMap = mapManager.GridMap;
    }

    public List<Tile> FindPath(Tile startTile, Tile endTile, int movementPoints)
    {
        foreach (var tile in gridMap.Values)
        {
            tile.G = 0; tile.H = 0; tile.previous = null;
        }

        List<Tile> openList = new List<Tile>(); //tiles to check
        HashSet<Tile> closedList = new HashSet<Tile>(); //cheked tiles

        int movementPool = movementPoints * 10;

        openList.Add(startTile);

        while (openList.Count > 0)
        {
            var currentTile = openList.OrderBy(n => n.F).First();

            if (currentTile == endTile)
                return BuildPath(endTile);

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            foreach (var neighbor in GetNeighbors(currentTile))
            {
                if (neighbor.Blocked || closedList.Contains(neighbor))
                    continue;

                if (!CanMoveDiagonal(currentTile, neighbor))
                    continue;

                int cost = GetMoveCost(currentTile, neighbor);
                int tentativeG = currentTile.G + cost;
                
                if (tentativeG > movementPool)
                    continue;

                if (!openList.Contains(neighbor) || tentativeG < neighbor.G)
                {
                    neighbor.G = tentativeG;
                    neighbor.H = Heuristic(neighbor.Cords, endTile.Cords);
                    neighbor.previous = currentTile;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
                
            }
        }

        return new List<Tile>();
    }

    List<Tile> GetNeighbors(Tile Tile)
    {
        List<Tile> result = new List<Tile>();
        foreach (var dir in directions)
        {
            Vector3Int check = Tile.Cords + dir;
            if (gridMap.ContainsKey(check))
                result.Add(gridMap[check]);
        }
        return result;
    }

    bool CanMoveDiagonal(Tile current, Tile neighbor)
    {
        int dx = neighbor.Cords.x - current.Cords.x;
        int dz = neighbor.Cords.z - current.Cords.z;

        if (dx == 0 || dz == 0) return true; 

        Vector3Int stepX = new Vector3Int(current.Cords.x + dx, current.Cords.y, current.Cords.z);
        Vector3Int stepZ = new Vector3Int(current.Cords.x, current.Cords.y, current.Cords.z + dz);

        return gridMap.ContainsKey(stepX) && !gridMap[stepX].Blocked &&
               gridMap.ContainsKey(stepZ) && !gridMap[stepZ].Blocked;
    }

    int GetMoveCost(Tile from, Tile to)
    {
        int baseCost;

        //diagonal check
        if (from.Cords.x != to.Cords.x && from.Cords.z != to.Cords.z)
            baseCost = (int)(Mathf.Sqrt(2) * from.costToMove);
        else
            baseCost = from.costToMove;

        int heightDiff = Mathf.Abs(from.Cords.y - to.Cords.y);
        baseCost += heightDiff * 5;

        return baseCost;
    }


    int Heuristic(Vector3Int a, Vector3Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dz = Mathf.Abs(a.z - b.z);
        return 10 * Mathf.Max(dx, dz);
    }

    List<Tile> BuildPath(Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        Tile curr = endTile;

        while (curr != null)
        {
            path.Add(curr);
            curr = curr.previous;
        }

        path.Reverse();
        return path;
    }

    public List<Tile> GetReachableTiles(Tile startTile, int movementPoints)
    {
        Dictionary<Tile, int> costSoFar = new Dictionary<Tile, int>();
        Queue<Tile> frontier = new Queue<Tile>();

        frontier.Enqueue(startTile);
        costSoFar[startTile] = 0;

        List<Tile> reachable = new List<Tile>();

        while (frontier.Count > 0)
        {
            Tile current = frontier.Dequeue();
            reachable.Add(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (neighbor.Blocked)
                    continue;

                if (!CanMoveDiagonal(current, neighbor))
                    continue;

                int newCost = costSoFar[current] + GetMoveCost(current, neighbor);

                if (newCost > movementPoints * current.costToMove)
                    continue;

                if (!costSoFar.ContainsKey(neighbor))
                {
                    costSoFar[neighbor] = newCost;
                    frontier.Enqueue(neighbor);
                }
            }
        }

        return reachable;
    }

}
