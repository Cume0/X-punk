using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    [Header ("Identification")]
    [SerializeField] public Guid id;
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
    [SerializeField] public int defence; 

    public List<Tile> path;
    public Vector3Int cords;
    public List<GameObject> pawnsInSight;
    public StatesEnum state;
    public int HP;
    public int movePoints;
    public int actionPoints;
    public int initiative;

    EnemyAI AI;
    GameEvents gameEvents;

    public void Initialize()
    {    
        id = Guid.NewGuid();

        gameEvents = GameManager.gameEvents;

        gameEvents.onPawnsTurn += BeginTurn;

        cords = new Vector3Int(
        Mathf.RoundToInt(transform.position.x),
        Mathf.RoundToInt(transform.position.y),
        Mathf.RoundToInt(transform.position.z)
        );

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

    public void BeginTurn(Guid givenID)
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
}
