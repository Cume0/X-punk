using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{  
    StateMachine stateMachine;

    public void Initialize(PawnInfo GivenPawnInfo, WeaponInfo GivenWeaponInfo, Pawn GivenEnemyPawn)
    {
        stateMachine = GetComponent<StateMachine>();
        if (stateMachine == null)
        {
            stateMachine = gameObject.AddComponent<StateMachine>();
        }

        stateMachine.Initialize(GivenPawnInfo, GivenWeaponInfo, GivenEnemyPawn);
    }

    public void BeginTurn()
    {
        // Refresh sensory data before choosing a state
        stateMachine.RefreshSenses();

        if (stateMachine.TryGetTargetInSight(out Transform target))
        {
            // Enemy in sight - randomly choose between shooting and cover
            if (Random.Range(0, 2) == 0)
            {
                stateMachine.SwitchState(stateMachine.GetShootState());
            }
            else
            {
                stateMachine.SwitchState(stateMachine.GetCoverState());
            }
        }
        else
        {
            // No enemy in sight - patrol
            stateMachine.SwitchState(stateMachine.GetPatrolState());
        }
    }
}

