using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private LayerMask enemyMask;

    private Animator animator;
    private float lastAttackTime;
    private bool isAttacking;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void StartAttack()
    {
        if (isAttacking) return; 
        if (Time.time - lastAttackTime < attackCooldown) return; 

        lastAttackTime = Time.time;
        isAttacking = true;

        animator.SetTrigger("attack");
        PerformAttack();

        Invoke(nameof(EndAttack), 1.5f); 
    }

    private void PerformAttack()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, attackRange, enemyMask);

        foreach (var hit in hits)
        {
            // hit.GetComponent<Health>()?.TakeDamage(attackDamage);
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        animator.ResetTrigger("attack");
    }

    public void CancelAttack()
    {
        if (!isAttacking) return;

        isAttacking = false;
        CancelInvoke(nameof(EndAttack));
        animator.ResetTrigger("attack");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, attackRange);
    }
}