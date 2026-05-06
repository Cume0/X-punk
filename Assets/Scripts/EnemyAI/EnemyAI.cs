using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{  
    PawnInfo pawnInfo;
    WeaponInfo weaponInfo;
    Pawn enemyPawn;
    Transform eyesTransform;
    SightTriggerProxy sightTriggerProxy;

    GameEvents gameEvents;

    Transform closestCover;

    Ray sightRay;
    RaycastHit targetInSightRaycastHit;

    List<GameObject> pawnsInSight;
    List<Transform> coverPositionsInSight;
    public void Initialize(PawnInfo GivenPawnInfo, WeaponInfo GivenWeaponInfo, Pawn GivenEnemyPawn)
    {
        gameEvents = GameManager.gameEvents;
        pawnInfo = GivenPawnInfo;
        weaponInfo = GivenWeaponInfo;
        enemyPawn = GivenEnemyPawn;

        eyesTransform = transform.Find("FirstPersonCameraPosition").transform;
        sightTriggerProxy = transform.GetComponentInChildren<SightTriggerProxy>();

        pawnsInSight = new List<GameObject>();
        coverPositionsInSight = new List<Transform>();
    }

    public void BeginTurn()
    {
        print("I am gonna do smth");

        pawnsInSight = sightTriggerProxy.pawnsInSight;
        coverPositionsInSight = sightTriggerProxy.coverPositionsInSight;

        foreach (GameObject pawn in pawnsInSight)
        {
            sightRay = new Ray(eyesTransform.position, (pawn.transform.position - eyesTransform.position).normalized);
            if (Physics.Raycast(sightRay, out targetInSightRaycastHit))
            {
                if (targetInSightRaycastHit.collider.gameObject == pawn)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        print("I wanna shoot");
                        Shoot(targetInSightRaycastHit.transform);
                        return;
                    }
                    else
                    {
                        print("I wanna run");
                        FindCover();
                        return;
                    }
                }
            }
        }

        print("Might be the rats");
        Patrol();
    } 

    public void FindCover()
    {
        float minimalDistance = 0;
        float temp;

        foreach (Transform position in coverPositionsInSight)
        {
            temp = Vector3.Distance(transform.position, position.position);
            if (minimalDistance < temp)
            {
                minimalDistance = temp;
                closestCover = position;
            }
        }

        if (minimalDistance < enemyPawn.movePoints)
        {
            print(enemyPawn);
            print(enemyPawn.navMeshAgent);
            print(closestCover.position);
            print(enemyPawn.path);

            enemyPawn.navMeshAgent.CalculatePath(closestCover.position, enemyPawn.path);
            enemyPawn.finalPathPoint = enemyPawn.path.corners.Last();
        }
        else
        {
            enemyPawn.navMeshAgent.CalculatePath(closestCover.position / enemyPawn.movePoints, enemyPawn.path);
            enemyPawn.finalPathPoint = enemyPawn.path.corners.Last();
        }

        StartCoroutine(EndTurn());
    }
    
    public void Patrol()
    {
        StartCoroutine(EndTurn());
    }

    public void Shoot(Transform targetTransform)
    {
        transform.LookAt(targetTransform);
        
        List<GameObject> tempList = new List<GameObject>();
        tempList.Add(targetTransform.gameObject);
        gameEvents.AttackTrigger(enemyPawn.id, tempList);

        StartCoroutine(EndTurn());
    }

    public IEnumerator EndTurn()
    {
        if (enemyPawn.path != null)
        {
            yield return enemyPawn.MoveAlongPath();
        }
        yield return enemyPawn.EndTurn();

        gameEvents.EndTurn();
    }
}
