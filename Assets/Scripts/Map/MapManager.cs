using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] Vector2Int mapSize;
    [SerializeField] int unityMapSize;
    public int UnityMapSize {get {return unityMapSize; } }

    Dictionary<Vector2Int, Node> gridMap = new Dictionary<Vector2Int, Node>();
    public Dictionary<Vector2Int, Node> GridMap {get {return gridMap; } } 

    void Awake()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector2Int thisCords = new Vector2Int(x, y);
                gridMap.Add(thisCords, new Node(thisCords));
            }
        }
    }
}
