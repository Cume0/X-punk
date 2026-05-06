using UnityEngine;
using UnityEngine.AI;

public class AnimatorController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    public void Initialize(NavMeshAgent agent)
    {
        animator = GetComponent<Animator>();
        navMeshAgent = agent;
    }

    private void FixedUpdate()
    {
        SetXZ();
    }
    public void SetXZ()
    {
        animator.SetFloat("x", navMeshAgent.velocity.x / navMeshAgent.speed);
        animator.SetFloat("z", navMeshAgent.velocity.z / navMeshAgent.speed);
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude / navMeshAgent.speed);
    }
}
