using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[System.Serializable]
public sealed class AIAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private Animator animator;

    private float lastAttackTime;
    private bool isAttacking;
    private static readonly int AnimAttack = Animator.StringToHash("isAttacking");
    private static readonly int AnimSpeed = Animator.StringToHash("isSpeeding");

    public float AttackRange => attackRange;
    public bool CanAttack() => Time.time - lastAttackTime >= attackCooldown;

    public void ExecuteAttack(Transform origin, NavMeshAgent agent)
    {
        if (isAttacking) return;

        float speed = agent.velocity.magnitude;
        animator.SetFloat(AnimSpeed, speed);

        if (speed > 0.1f) return; // đang di chuyển thì không đánh

        isAttacking = true;
        lastAttackTime = Time.time;
        animator.SetBool(AnimAttack, true);

        // kiểm tra va chạm
        Collider[] hits = Physics.OverlapSphere(origin.position + origin.forward * 1.0f, attackRange, targetMask);
        foreach (var hit in hits)
        {
            Debug.Log("Hit: " + hit.name);
            if (hit.TryGetComponent(out Health health))
                health.TakeDamage(attackDamage);
        }

        StartCoroutine(ResetAttackFlag(0.8f)); // giữ anim 0.8s
    }

    private IEnumerator ResetAttackFlag(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool(AnimAttack, false);
        isAttacking = false;
    }

    public void StopAiming()
    {
        animator.SetBool(AnimAttack, false);
        isAttacking = false;
    }

    // 🔍 Vẽ vùng tấn công trong Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 1.0f, attackRange);
    }
}
