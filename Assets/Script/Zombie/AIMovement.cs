using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public sealed class AIMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private float rotationSpeed = 5f;

    private static readonly int  AnimSpeed = Animator.StringToHash("Speed");

    public Animator Animator=> animator;
    public NavMeshAgent Agent => agent;
    
    public void MoveTo(Vector3 position)
    {
        
        agent.SetDestination(position);
        animator.SetFloat( AnimSpeed, agent.velocity.magnitude);
    }

    public void Stop()
    {
        agent.ResetPath();
        animator.SetFloat( AnimSpeed, 0);
    }

    public void LookAt(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
    }

    public float DistanceTo(Vector3 target)
    {
        return Vector3.Distance(transform.position, target);
    }
}