using UnityEngine;

public class PatrolState : EnemyState
{
    public override void StateStart(StateMachine stateMachine)
    {
        Debug.Log("Entering Patrol State");
        Patrol(stateMachine);
    }

    public override void StateUpdate(StateMachine stateMachine)
    {

    }

    public override void StateEnd(StateMachine stateMachine)
    {
        Debug.Log("Exiting Patrol State");
    }

    private void Patrol(StateMachine stateMachine)
    {
        Debug.Log("I might be the rats");
        stateMachine.StartCoroutine(stateMachine.EndTurn());
    }
}
