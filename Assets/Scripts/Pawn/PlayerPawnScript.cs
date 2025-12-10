using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawnScript : MonoBehaviour
{
    //logic data
    public List<Node> path;
    int moveSpeed;

    void Awake()
    {
        moveSpeed = GetComponent<Pawn>().moveSpeed;
        
        transform.name = GetComponent<Pawn>().name;
    }
    public IEnumerator MoveAlongPath()
    {
        while(path.Count != 0)
        {
            Vector3 targetPos = new Vector3(
            path[0].Cords.x,
            path[0].Cords.y,
            path[0].Cords.z
            );

            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                path.RemoveAt(0);
            }

            yield return null;
        }
    }
}
