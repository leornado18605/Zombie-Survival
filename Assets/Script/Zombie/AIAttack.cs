using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public sealed class AIAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private Animator animator;


    public float AttackRange => attackRange;
    private float lastAttackTime;
    private static readonly int AnimAiming = Animator.StringToHash("Aiming");
    private static readonly int AnimSpeed = Animator.StringToHash("Speed");

    public bool CanAttack()
    {
        return Time.time - lastAttackTime >= attackCooldown;
    }

    public void ExecuteAttack(Transform origin, NavMeshAgent agent)
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat(AnimSpeed, speed);

        bool isMoving = speed > 0.1f;
        animator.SetBool(AnimAiming, !isMoving);

        if (isMoving)
        {
            animator.SetBool(AnimAiming, false);
            return;
        }

        lastAttackTime = Time.time;
        animator.SetBool(AnimAiming, true);

        Collider[] hits = Physics.OverlapSphere(origin.position + origin.forward, attackRange, targetMask);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Health health))
                health.TakeDamage(attackDamage);
        }
        
        animator.SetBool(AnimAiming, false);
    }


    public void StopAiming()
    {
        animator.SetBool(AnimAiming, false);
    }
}