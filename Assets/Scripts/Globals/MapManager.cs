using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] int unityMapSize;
    public int UnityMapSize {get {return unityMapSize; } }

    Dictionary<Vector3Int, Node> gridMap = new Dictionary<Vector3Int, Node>();
    public Dictionary<Vector3Int, Node> GridMap {get {return gridMap; } }

    void Start()
    {
        for (int tileIndex = 0; tileIndex < transform.childCount; tileIndex++)
        {
            Vector3 gridPos = transform.GetChild(tileIndex).transform.position;

            Vector3Int gridPosInt = new Vector3Int((int)gridPos.x, (int)gridPos.y, (int)gridPos.z);

            gridMap[gridPosInt] = new Node(gridPosInt, transform.GetChild(tileIndex).gameObject.GetComponent<Tile>().blockNode);
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
        GetNode(cords).Blocked = true;
    }
}
