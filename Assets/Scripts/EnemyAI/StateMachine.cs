using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class StateMachine : MonoBehaviour
{
    public PawnInfo pawnInfo { get; private set; }
    public WeaponInfo weaponInfo { get; private set; }
    public Pawn enemyPawn { get; private set; }
    public Transform eyesTransform { get; private set; }
    public SightTriggerProxy sightTriggerProxy { get; private set; }
    public GameEvents gameEvents { get; private set; }

    //States
    private EnemyState currentState;
    private PatrolState patrolState;
    private ShootState shootState;
    private CoverState coverState;

    public List<GameObject> pawnsInSight { get; set; }
    public List<Transform> coverPositionsInSight { get; set; }
    public Transform closestCover { get; set; }

    public void Initialize(PawnInfo givenPawnInfo, WeaponInfo givenWeaponInfo, Pawn givenEnemyPawn)
    {
        pawnInfo = givenPawnInfo;
        weaponInfo = givenWeaponInfo;
        enemyPawn = givenEnemyPawn;

        eyesTransform = transform.Find("FirstPersonCameraPosition").transform;
        sightTriggerProxy = transform.GetComponentInChildren<SightTriggerProxy>();
        gameEvents = GameManager.gameEvents;

        pawnsInSight = new List<GameObject>();
        coverPositionsInSight = new List<Transform>();

        patrolState = new PatrolState();
        shootState = new ShootState();
        coverState = new CoverState();

        currentState = patrolState;
    }

    public void BeginTurn()
    {
        print("StateMachine: Beginning turn in state: " + currentState.GetType().Name);
        currentState.StateStart(this);
    }

    public void RefreshSenses()
    {
        pawnsInSight = sightTriggerProxy.pawnsInSight.Where(pawn => pawn != null).ToList();
        coverPositionsInSight = sightTriggerProxy.coverPositionsInSight.Where(cover => cover != null).ToList();
    }

    public void SetState(EnemyState newState)
    {
        if (currentState != null)
        {
            currentState.StateEnd(this);
        }
        currentState = newState;
    }

    public void SwitchState(EnemyState newState)
    {
        if (currentState != null)
        {
            currentState.StateEnd(this);
        }
        currentState = newState;
        currentState.StateStart(this);
    }

    public EnemyState GetPatrolState() => patrolState;
    public EnemyState GetShootState() => shootState;
    public EnemyState GetCoverState() => coverState;

    public bool TryGetTargetInSight(out Transform targetTransform)
    {
        targetTransform = null;

        foreach (GameObject pawn in pawnsInSight)
        {
            if (pawn == null)
            {
                continue;
            }

            Transform pawnTransform = pawn.transform;
            if (pawnTransform == null)
            {
                continue;
            }

            Ray sightRay = new Ray(eyesTransform.position, (pawnTransform.position - eyesTransform.position).normalized);
            if (Physics.Raycast(sightRay, out RaycastHit hit))
            {
                if (hit.collider.gameObject == pawn)
                {
                    targetTransform = pawnTransform;
                    return true;
                }
            }
        }

        return false;
    }

    public void FindClosestCover()
    {
        closestCover = null;
        float minimalDistance = float.MaxValue;
        float temp;

        foreach (Transform position in coverPositionsInSight)
        {
            temp = Vector3.Distance(transform.position, position.position);
            if (temp < minimalDistance)
            {
                minimalDistance = temp;
                closestCover = position;
            }
        }
    }

    public IEnumerator EndTurn()
    {
        if (enemyPawn.path.corners.Length > 1)
        {
            yield return enemyPawn.MoveAlongPath();
        }

        gameEvents.EndTurn();
    }
}




