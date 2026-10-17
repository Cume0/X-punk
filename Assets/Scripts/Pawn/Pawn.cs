using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Cumeo.Utils;

public class Pawn : MonoBehaviour, IDamageable
{
    public Guid id;
    public NavMeshPath path;

    public StatesEnum state;

    [Header("BaseData")]
    public PawnInfo pawnInfo;
    public WeaponInfo weaponInfo;
    
    [Header("Navigation")]
    [ReadOnly] public float previewMovePointsSpent;
    [ReadOnly] public float previewMovePointsRemaining;
    [HideInInspector] public List<Vector3> pathCorners;
    [HideInInspector] public Vector3 finalPathPoint;

    [Header("Stats")]
    public int MaxHealth;
    public int CurrentHealth;

    public float MaxMovePoints;
    public float CurrentMovePoints;

    public int maxActionPoints;
    public int actionPoints;

    public int initiative;

    [Header("Components")]
    public EnemyAI AI;

    [Header("If you see this, something is wrong")]
    GameEvents gameEvents;
    SphereCollider DetectionRadius;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    NavMeshObstacle navMeshObstacle;
    EventCameraAssigner eventCameraAssigner;
    AnimatorController animatorController;
    [HideInInspector] public GameObject selectedUnitSelectionSign;
    [HideInInspector] public WorldSpaceUIBar HealthBar;

    int IDamageable.CurrentHealth { get => CurrentHealth; set => CurrentHealth = value; }
    int IDamageable.MaxHealth { get => MaxHealth; set => MaxHealth = value; }

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
        eventCameraAssigner = GetComponentInChildren<EventCameraAssigner>();
        HealthBar = GetComponentInChildren<WorldSpaceUIBar>();

        animatorController.Initialize(navMeshAgent);
        eventCameraAssigner.Initialize();
        HealthBar.Initialize(pawnInfo.baseMaxHealth, pawnInfo.baseCurrentHealth);

        path = new NavMeshPath();

        state = StatesEnum.Inactive;

        MaxHealth = pawnInfo.baseMaxHealth;
        CurrentHealth = pawnInfo.baseCurrentHealth;
        MaxMovePoints = pawnInfo.baseMovePoints;
        CurrentMovePoints = MaxMovePoints;
        maxActionPoints = pawnInfo.baseActionPoints;
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
        if (givenID != id) return;

        StartCoroutine(enableNavMeshAgent());

        state = StatesEnum.Active;
        CurrentMovePoints = MaxMovePoints;
        previewMovePointsRemaining = CurrentMovePoints;
        previewMovePointsSpent = 0f;
        actionPoints = maxActionPoints;

        if (AI != null)
        {
            print("AI turn begin");
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
            if (navMeshAgent.hasPath)
            {
                pathCorners = new List<Vector3>(navMeshAgent.path.corners);
            }

            yield return null;
        }

        navMeshAgent.ResetPath();
        CurrentMovePoints = Mathf.Max(0f, CurrentMovePoints - distanceTraveled);
        previewMovePointsRemaining = CurrentMovePoints;
        previewMovePointsSpent = 0f;

        state = StatesEnum.Active;
        gameEvents.HidePath(id);

        yield break;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        HealthBar.ChangeValue(-damage);
        
        if (CurrentHealth <= 0)
        {
            gameEvents.Die(id);
            Destroy(gameObject);
        }
    } 

    public Guid guid { get => id; }

    public IEnumerator EndTurn()
    {
        print("AI turn end");
        if (!navMeshAgent.enabled || !navMeshAgent.isOnNavMesh)
        {
            yield break;
        }
        navMeshAgent.ResetPath();

        navMeshAgent.enabled = false;
        yield return null;
        navMeshObstacle.enabled = true;
        state = StatesEnum.Inactive;
    }
}