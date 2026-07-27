using System.Collections.Generic;
using UnityEngine;

public class ShootState : EnemyState
{
    private Transform currentTarget;

    public override void StateStart(StateMachine stateMachine)
    {
        Debug.Log("Entering Shoot State");
        
        if (stateMachine.TryGetTargetInSight(out Transform target))
        {
            currentTarget = target;
            Shoot(stateMachine, currentTarget);
        }
        else
        {
            Debug.Log("Target lost, returning to patrol");
            stateMachine.SwitchState(stateMachine.GetPatrolState());
        }
    }

    public override void StateUpdate(StateMachine stateMachine)
    {

    }

    public override void StateEnd(StateMachine stateMachine)
    {
        Debug.Log("Exiting Shoot State");
    }

    private void Shoot(StateMachine stateMachine, Transform targetTransform)
    {
        Debug.Log("I wanna shoot");
        
        stateMachine.transform.LookAt(targetTransform);
        
        List<GameObject> tempList = new List<GameObject>();
        tempList.Add(targetTransform.gameObject);
        stateMachine.gameEvents.AttackTrigger(stateMachine.enemyPawn.id, tempList);

        stateMachine.StartCoroutine(stateMachine.EndTurn());
    }
}
