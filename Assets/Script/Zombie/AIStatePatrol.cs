using UnityEngine;
using UnityEngine.AI;

public sealed class AIStatePatrol : IAIState
{
    private readonly AIController controller;

    private const float PatrolRadius = 15f;
    private const float DetectionRadius = 10f;
    private const float ArriveThreshold = 1.2f;
    private const float WaitAtPoint = 2f;

    private readonly LayerMask playerMask = LayerMask.GetMask("Player");

    private Vector3 patrolTarget;
    private float waitTimer;
    private bool isWaiting;
    private bool hasValidTarget;

    public AIStatePatrol(AIController ctrl)
    {
        controller = ctrl;
    }

    public void EnterState()
    {
        var agent = controller.Movement.Agent;
        agent.isStopped = false;

        // 🎯 Di chuyển kiểu zombie – chậm, không xoay nhanh
        agent.speed = 0.8f;
        agent.acceleration = 1.8f;
        agent.angularSpeed = 40f;
        agent.autoBraking = true;

        waitTimer = 0f;
        isWaiting = false;

        PickNewPatrolPoint();
    }

    public void UpdateState()
    {
        var agent = controller.Movement.Agent;

        // 1️⃣ Phát hiện người chơi
        Collider[] hits = Physics.OverlapSphere(controller.transform.position, DetectionRadius, playerMask);
        if (hits.Length > 0)
        {
            controller.Target = hits[0].transform;
            controller.SwitchState(AIController.AIStateType.Chase);
            return;
        }

        // 2️⃣ Cập nhật animation theo tốc độ thực
        float speed = agent.velocity.magnitude;
        controller.Movement.Animator.SetFloat("Speed", speed);

        // 🔹 Quay nhẹ theo hướng di chuyển (thay vì để agent tự xoay)
        if (agent.velocity.sqrMagnitude > 0.05f)
        {
            Quaternion lookRot = Quaternion.LookRotation(agent.velocity.normalized);
            controller.transform.rotation = Quaternion.Slerp(
                controller.transform.rotation,
                lookRot,
                Time.deltaTime * 2f // tốc độ quay chậm → cảm giác zombie
            );
        }

        // 3️⃣ Nếu tới nơi rồi → chờ rồi chọn điểm mới
        float dist = controller.Movement.DistanceTo(patrolTarget);
        if (dist <= ArriveThreshold || !agent.hasPath)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                agent.isStopped = true;
                controller.Movement.Animator.SetFloat("Speed", 0f);
                waitTimer = 0f;
            }

            waitTimer += Time.deltaTime;

            if (waitTimer >= WaitAtPoint)
            {
                isWaiting = false;
                agent.isStopped = false;
                PickNewPatrolPoint();
                waitTimer = 0f;
            }
        }

        // 4️⃣ Nếu path lỗi → chọn lại
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            PickNewPatrolPoint();
        }
    }

    public void ExitState()
    {
        controller.Movement.Stop();
    }

    private void PickNewPatrolPoint()
    {
        NavMeshHit hit;
        for (int i = 0; i < 15; i++)
        {
            Vector3 randomDir = Random.insideUnitSphere * PatrolRadius;
            randomDir += controller.transform.position;

            if (NavMesh.SamplePosition(randomDir, out hit, PatrolRadius, NavMesh.AllAreas))
            {
                if (Vector3.Distance(controller.transform.position, hit.position) > 2f)
                {
                    patrolTarget = hit.position;
                    controller.Movement.Agent.SetDestination(patrolTarget);
                    hasValidTarget = true;
                    return;
                }
            }
        }

        hasValidTarget = false;
    }
}
