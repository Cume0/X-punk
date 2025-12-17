using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPawnScript : MonoBehaviour
{
    //logic data
    public List<Node> path;
    public Vector3Int cords;

    public MapManager mapManager;
    int moveSpeed;

    void Awake()
    {
        moveSpeed = GetComponent<Pawn>().moveSpeed;
        
        transform.name = GetComponent<Pawn>().name;

        mapManager = FindFirstObjectByType<MapManager>();
    }
    void Start()
    {
        cords = new Vector3Int(
        Mathf.RoundToInt(transform.position.x),
        Mathf.RoundToInt(transform.position.y),
        Mathf.RoundToInt(transform.position.z)
        );

        mapManager.BlockNode(cords);
    }

    public Vector3Int getPosition()
    {
        return cords;
    }

    public IEnumerator MoveAlongPath()
    {
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

        yield break;
    }
}
