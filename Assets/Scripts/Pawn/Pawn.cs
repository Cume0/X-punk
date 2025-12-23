using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    [SerializeField] public int id;
    [SerializeField] private TeamsEnum team;
    [SerializeField] private string pawnName;
    [SerializeField] public int moveSpeed;
    [SerializeField] public int movePoints;
    
    public List<Tile> path;
    public Vector3Int cords;
    public StatesEnum state;

    public MapManager mapManager;

    public void Initialize()
    {    
        mapManager = FindFirstObjectByType<MapManager>();

        cords = new Vector3Int(
        Mathf.RoundToInt(transform.position.x),
        Mathf.RoundToInt(transform.position.y),
        Mathf.RoundToInt(transform.position.z)
        );

        mapManager.BlockNode(cords);

        state = StatesEnum.Idle;
    }

    public Vector3Int getPosition()
    {
        return cords;
    }

    public IEnumerator MoveAlongPath()
    {
        state = StatesEnum.Moving;

        mapManager.UnBlockNode(cords);

        mapManager.BlockNode(path[ path.Count - 1 ].Cords);

        while(path.Count != 0)
        {
            Vector3Int targetPos = new Vector3Int(
            path[0].Cords.x,
            path[0].Cords.y,
            path[0].Cords.z
            );

            mapManager.BlockNode(targetPos);

            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                mapManager.UnBlockNode(targetPos);

                path.RemoveAt(0);
            }

            cords = targetPos;

            yield return null;
        }

        mapManager.BlockNode(cords);

        print(getPosition());

        state = StatesEnum.Idle;

        yield break;
    }
}
