using UnityEngine;

public abstract class EnemyState
{
    public abstract void StateStart(StateMachine stateMachine);
    public abstract void StateUpdate(StateMachine stateMachine);
    public abstract void StateEnd(StateMachine stateMachine);
}
