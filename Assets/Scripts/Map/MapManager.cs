using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] Vector3Int mapSize;
    [SerializeField] int unityMapSize;
    public int UnityMapSize {get {return unityMapSize; } }

    Dictionary<Vector3Int, Node> gridMap = new Dictionary<Vector3Int, Node>();
    public Dictionary<Vector3Int, Node> GridMap {get {return gridMap; } }

    void Awake()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    Vector3Int thisCords = new Vector3Int(x, y, z);
                    gridMap.Add(thisCords, new Node(thisCords, false));
                }
            }
        }
    }
    
    public Node GetNode(Vector3Int cords)
    {
        if (gridMap.ContainsKey(cords))
        {
            return gridMap[cords];
        }

        return null;
    }

    public void BlockNode(Vector3Int cords)
    {
        if (gridMap.ContainsKey(cords))
        {
            gridMap[cords].Blocked = true;
        }
    }
}
