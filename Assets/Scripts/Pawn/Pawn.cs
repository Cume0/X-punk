using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    [Header ("Identification")]
    [SerializeField] public int id;
    [SerializeField] public TeamsEnum team;
    [SerializeField] private string pawnName;

    [Header ("Movement")]
    [SerializeField] public int moveSpeed;
    [SerializeField] public int maxMovePoints;

    [Header ("Combat")]
    [SerializeField] public int maxActionPoints;
    [SerializeField] public int maxHP;
    [SerializeField] public int SightDistance;
    [SerializeField] public int accuracy;
    [SerializeField] public int defence; //////


    
    public List<Tile> path;
    public Vector3Int cords;
    public List<Transform> pawnsInSight;
    public StatesEnum state;
    public int HP;
    public int movePoints;
    public int actionPoints;
    public int initiative;

    EnemyAI AI;
    MapManager mapManager;
    GameEvents gameEvents;

    public void Initialize()
    {    
        mapManager = GameManager.mapManager;
        gameEvents = GameManager.gameEvents;

        gameEvents.onPawnsTurn += BeginTurn;

        cords = new Vector3Int(
        Mathf.RoundToInt(transform.position.x),
        Mathf.RoundToInt(transform.position.y),
        Mathf.RoundToInt(transform.position.z)
        );

        mapManager.BlockNode(cords);

        state = StatesEnum.InActive;

        HP = maxHP;
        movePoints = maxMovePoints;
        actionPoints = maxActionPoints;
        initiative = getInitiative();

        if (team == TeamsEnum.Enemy)
        {
            AI = GetComponent<EnemyAI>();
        }
    }

    public Vector3Int getPosition()
    {
        return cords;
    }

    public int getInitiative()
    {
        return UnityEngine.Random.Range(1, 21);
    }

    public void BeginTurn(int givenID)
    {
        if (givenID != id)
        {
            return;
        }

        state = StatesEnum.Active;
        movePoints = maxMovePoints;
        actionPoints = maxActionPoints;

        if (AI == null)
        {
            return;
        }

        AI.skipTurn();
    }

    public void LookAtTarget(Vector3Int targetCords)
    {
        Transform visualsTransform = transform.GetChild(0).GetComponent<Transform>();

        Vector3 tileCenter = new Vector3(
                targetCords.x + 0.5f,
                visualsTransform.position.y,
                targetCords.z + 0.5f
                );

        visualsTransform.LookAt(tileCenter);
    }

    public IEnumerator MoveAlongPath()
    {
        if (path.Count == 0)
        {
            print("Too far");
            yield break;
        }

        state = StatesEnum.Busy;
        print("Unit is moving " + state);

        gameEvents.PawnWalkStart(id);

        mapManager.UnBlockNode(cords);
        mapManager.BlockNode(path[ path.Count - 1 ].Cords);

        movePoints -= path.Count;

        while(path.Count != 0)
        {
            Vector3Int targetPos = new Vector3Int(
            path[0].Cords.x,
            path[0].Cords.y,
            path[0].Cords.z
            );

            LookAtTarget(targetPos);

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

        state = StatesEnum.Active;

        gameEvents.PawnWalkFinish(id);

        yield break;
    }

    //
}
