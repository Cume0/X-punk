using System.Linq;
using UnityEngine;

public class CoverState : EnemyState
{
    public override void StateStart(StateMachine stateMachine)
    {
        Debug.Log("Entering Cover State");
        FindCover(stateMachine);
    }

    public override void StateUpdate(StateMachine stateMachine)
    {

    }

    public override void StateEnd(StateMachine stateMachine)
    {
        Debug.Log("Exiting Cover State");
    }

    private void FindCover(StateMachine stateMachine)
    {
        Debug.Log("I wanna run");
        
        stateMachine.FindClosestCover();

        if (stateMachine.closestCover != null)
        {
            float distanceToCover = Vector3.Distance(stateMachine.transform.position, stateMachine.closestCover.position);
            bool pathFound;

            if (distanceToCover < stateMachine.enemyPawn.movePoints)
            {
                pathFound = stateMachine.enemyPawn.navMeshAgent.CalculatePath(stateMachine.closestCover.position, stateMachine.enemyPawn.path);
            }
            else
            {
                pathFound = stateMachine.enemyPawn.navMeshAgent.CalculatePath(stateMachine.closestCover.position / stateMachine.enemyPawn.movePoints, stateMachine.enemyPawn.path);
            }

            if (pathFound && stateMachine.enemyPawn.path.corners.Length > 0)
            {
                stateMachine.enemyPawn.finalPathPoint = stateMachine.enemyPawn.path.corners.Last();
            }
            else
            {
                Debug.LogWarning("CoverState: no valid path to cover, ending turn.");
            }
        }

        stateMachine.StartCoroutine(stateMachine.EndTurn());
    }
}