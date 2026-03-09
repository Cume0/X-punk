using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] int unityMapSize;
    public int UnityMapSize {get {return unityMapSize; } }

    Dictionary<Vector3Int, Tile> gridMap = new Dictionary<Vector3Int, Tile>();
    public Dictionary<Vector3Int, Tile> GridMap {get {return gridMap; } }

    public void Initialize()
    {
        gridMap.Clear();
        
        for (int tileIndex = 0; tileIndex < transform.childCount; tileIndex++)
        {
            Vector3 gridPos = transform.GetChild(tileIndex).transform.position;

            Vector3Int gridPosInt = new Vector3Int((int)gridPos.x, (int)gridPos.y, (int)gridPos.z);

            gridMap.Add(gridPosInt, transform.GetChild(tileIndex).GetComponent<Tile>());

            gridMap[gridPosInt].Initialize(gridPosInt, transform.GetChild(tileIndex).gameObject.GetComponent<Tile>().blockNode);
        }
    }
    
    public Tile GetTile(Vector3Int cords)
    {
        if (gridMap.ContainsKey(cords))
        {
            return gridMap[cords];
        }

        return null;
    }

    public void BlockNode(Vector3Int cords)
    {
        GetTile(cords).Blocked = true;
    }

    public void UnBlockNode(Vector3Int cords)
    {
        GetTile(cords).Blocked = false;
    }
}
