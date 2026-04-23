using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class Pawn : MonoBehaviour, IDamageable
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
    [SerializeField] public int attackDamage;    
    [SerializeField] public int defence; 
    [SerializeField] public int evasion;


    public List<GameObject> pawnsInSight;
    public List<Vector3> path;
    public StatesEnum state;
    public int HP;
    public float movePoints;
    public float movePointsSpent;
    public float movePointsRemaining;
    public float previewMovePointsSpent;
    public float previewMovePointsRemaining;
    public int actionPoints;
    public int initiative;

    EnemyAI AI;
    GameEvents gameEvents;
    NavMeshAgent navMeshAgent;
    NavMeshObstacle navMeshObstacle;

    int IDamageable.HP { get => HP; set => HP = value; }

    public void Initialize()
    {    
        id = Guid.NewGuid();

        gameEvents = GameManager.gameEvents;

        gameEvents.onPawnsTurn += BeginTurn;

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        
        state = StatesEnum.InActive;

        HP = maxHP;
        movePoints = maxMovePoints;
        actionPoints = maxActionPoints;
        initiative = getInitiative();

        navMeshAgent.enabled = false;
        navMeshObstacle.enabled = true;

        if (team == TeamsEnum.Enemy)
        {
            AI = GetComponent<EnemyAI>();
        }
    }

    public int getInitiative()
    {
        return UnityEngine.Random.Range(1, 21);
    }

    private IEnumerator enableNavMeshAgent()
    {
        navMeshObstacle.enabled = false;
        yield return null;
        navMeshAgent.enabled = true;
        yield break;
    }

    private IEnumerator enableNavMeshObstacle()
    {
        navMeshAgent.enabled = false;
        yield return null;
        navMeshObstacle.enabled = true;
        yield break;
    }

    public void BeginTurn(Guid givenID)
    {
        if (givenID != id)
        {
            return;
        }
        
        StartCoroutine(enableNavMeshAgent());

        state = StatesEnum.Active;
        movePoints = maxMovePoints;
        movePointsRemaining = movePoints;
        movePointsSpent = 0f;
        previewMovePointsRemaining = movePoints;
        previewMovePointsSpent = 0f;
        actionPoints = maxActionPoints;

        if (AI == null)
        {
            return;
        }

        AI.skipTurn();
    }

    private float GetPathDistance(List<Vector3> pathToMeasure)
    {
        float distance = 0f;

        for (int i = 0; i < pathToMeasure.Count - 1; i++)
        {
            distance += Vector3.Distance(pathToMeasure[i], pathToMeasure[i + 1]);
        }

        return distance;
    }

    public IEnumerator MoveAlongPath()
    {
        if (path.Count == 0)
        {
            yield break;
        }

        float distanceTraveled = GetPathDistance(path);

        state = StatesEnum.Busy;

        while(path.Count != 0)
        {
            Vector3 targetPos = new Vector3(
            path[0].x,
            path[0].y,
            path[0].z
            );

            navMeshAgent.SetDestination(targetPos);

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                path.RemoveAt(0);
            }

            yield return null;
        }

        movePointsSpent += distanceTraveled;
        movePoints = Mathf.Max(0f, movePoints - distanceTraveled);
        movePointsRemaining = movePoints;
        previewMovePointsRemaining = movePoints;
        previewMovePointsSpent = 0f;

        state = StatesEnum.Active;
        gameEvents.HidePath(id);

        yield break;
    }

    public void TakeDamage(int damage)
    {
        print("Ouch!");
        HP -= damage;
        if (HP <= 0)
        {
            gameEvents.Die(id);
            Destroy(gameObject);
        }
    } 

    public void EndTurn()
    {
        StartCoroutine(enableNavMeshObstacle());

        state = StatesEnum.InActive;
    }
}