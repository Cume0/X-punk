using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.VisualScripting;

public class Pawn : MonoBehaviour, IDamageable
{
    public Guid id;
    public PawnInfo pawnInfo;
    public WeaponInfo weaponInfo;
    
    [HideInInspector] public NavMeshPath path;
    [HideInInspector] public List<Vector3> pathCorners;
    [HideInInspector] public Vector3 finalPathPoint;
    public StatesEnum state;
    int HP;
    public float movePoints;
    [HideInInspector] public float movePointsSpent;
    [HideInInspector] public float movePointsRemaining;
    [HideInInspector] public float previewMovePointsSpent;
    [HideInInspector] public float previewMovePointsRemaining;
    [HideInInspector] public int maxActionPoints;
    [HideInInspector] public int actionPoints;
    [HideInInspector] public int initiative;

    GameEvents gameEvents;
    EnemyAI AI;
    SphereCollider DetectionRadius;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] NavMeshObstacle navMeshObstacle;
    [HideInInspector] AnimatorController animatorController;
    [HideInInspector] public GameObject selectedUnitSelectionSign;

    int IDamageable.HP { get => HP; set => HP = value; }

    public void Initialize()
    {    
        id = Guid.NewGuid();

        gameEvents = GameManager.gameEvents;

        selectedUnitSelectionSign = transform.GetChild(0).Find("SelectionSign").gameObject;
        selectedUnitSelectionSign.SetActive(false);

        gameEvents.onPawnsTurn += BeginTurn;

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        animatorController = GetComponentInChildren<AnimatorController>();

        animatorController.Initialize(navMeshAgent);
        path = new NavMeshPath();

        state = StatesEnum.InActive;

        HP = pawnInfo.maxHP;
        movePoints = pawnInfo.maxMovePoints;
        maxActionPoints = pawnInfo.maxActionPoints;
        actionPoints = maxActionPoints;
        initiative = getInitiative();

        navMeshAgent.enabled = false;
        navMeshObstacle.enabled = true;

        if (gameObject.TryGetComponent<EnemyAI>(out EnemyAI enemyAI))
        {
            DetectionRadius = GetComponentInChildren<SphereCollider>();
            AI = enemyAI;
            enemyAI.Initialize(pawnInfo, weaponInfo, this);
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
        if (this == null || gameObject == null) yield break;
        navMeshAgent.enabled = true;
    }

    public void BeginTurn(Guid givenID)
    {
        if (givenID != id)
        {
            return;
        }
        
        StartCoroutine(enableNavMeshAgent());

        state = StatesEnum.Active;
        movePoints = pawnInfo.maxMovePoints;
        movePointsRemaining = movePoints;
        movePointsSpent = 0f;
        previewMovePointsRemaining = movePoints;
        previewMovePointsSpent = 0f;
        actionPoints = maxActionPoints;

        if (AI != null)
        {
            print("Lets get some");
            StartCoroutine(BeginAITurn());
        }
    }

    private IEnumerator BeginAITurn()
    {
        yield return enableNavMeshAgent();
        AI.BeginTurn();
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
        if (path == null)
        {
            yield break;
        }

        float distanceTraveled = GetPathDistance(pathCorners);

        state = StatesEnum.Busy;

        navMeshAgent.SetPath(path);

        while ((transform.position - finalPathPoint).magnitude > 0.1f)
        {
            yield return null;
        }

        navMeshAgent.ResetPath();
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
        print("Ouch! I only have" + HP );
        HP -= damage;
        if (HP <= 0)
        {
            gameEvents.Die(id);
            Destroy(gameObject);
        }
    } 

    public Guid guid { get => id; }

    public IEnumerator EndTurn()
    {
        print("Im done");
        if (!navMeshAgent.enabled || !navMeshAgent.isOnNavMesh)
        {
            yield break;
        }
        navMeshAgent.ResetPath();
        navMeshAgent.enabled = false;
        yield return null;
        navMeshObstacle.enabled = true;
        state = StatesEnum.InActive;
    }
}