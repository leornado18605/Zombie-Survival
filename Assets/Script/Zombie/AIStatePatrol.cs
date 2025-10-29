namespace AI.StateMachine
{
    using UnityEngine;
    using UnityEngine.AI;

    public sealed class AIStatePatrol : IAIState
    {
        private readonly AIController controller;

        private const float PatrolRadius = 15f;
        private const float DetectionRadius = 10f;
        private const float ArriveThreshold = 1.2f;

        private readonly LayerMask playerMask = LayerMask.GetMask("Player");
        private Vector3 patrolTarget;

        public AIStatePatrol(AIController ctrl)
        {
            controller = ctrl;
        }

        public void EnterState()
        {
            var agent = controller.Movement.Agent;
            agent.isStopped = false;
            agent.updateRotation = false;
            agent.speed = 1.4f;
            agent.acceleration = 6f;
            agent.angularSpeed = 240f;
            agent.autoBraking = true;

            PickNewPatrolPoint();
        }

        public void UpdateState()
        {
            var agent = controller.Movement.Agent;

            // 🔍 Phát hiện người chơi (chỉ kiểm tra có tồn tại, không va chạm)
            if (Physics.CheckSphere(controller.transform.position, DetectionRadius, playerMask))
            {
                // ✅ Khi phát hiện Player, tìm chính xác collider gần nhất (nếu cần)
                Collider nearest = null;
                float minDist = float.MaxValue;
                Collider[] nearby = Physics.OverlapSphere(controller.transform.position, DetectionRadius, playerMask);

                foreach (var hit in nearby)
                {
                    float d = Vector3.Distance(controller.transform.position, hit.transform.position);
                    if (d < minDist)
                    {
                        minDist = d;
                        nearest = hit;
                    }
                }

                if (nearest != null)
                {
                    controller.Target = nearest.transform;
                    controller.SwitchState(AIController.AIStateType.Chase);
                    return;
                }
            }

            // 🎞 Cập nhật anim
            float normalizedSpeed = agent.velocity.magnitude / agent.speed;
            controller.Movement.Animator.SetFloat("isSpeeding", normalizedSpeed, 0.1f, Time.deltaTime);

            // 🔁 Xoay mượt
            if (agent.desiredVelocity.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(agent.desiredVelocity.normalized);
                controller.transform.rotation = Quaternion.Slerp(
                    controller.transform.rotation,
                    targetRot,
                    Time.deltaTime * 6f
                );
            }

            // 🧭 Nếu tới nơi → chọn ngay điểm mới
            float dist = controller.Movement.DistanceTo(patrolTarget);
            if (dist <= ArriveThreshold)
            {
                PickNewPatrolPoint();
            }

            // 🚧 Nếu path lỗi → chọn lại
            if (!agent.hasPath || agent.pathStatus != NavMeshPathStatus.PathComplete)
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

            for (int i = 0; i < 10; i++)
            {
                Vector3 randomDir = Random.insideUnitSphere * PatrolRadius + controller.transform.position;
                if (NavMesh.SamplePosition(randomDir, out hit, PatrolRadius, NavMesh.AllAreas))
                {
                    if (Vector3.Distance(controller.transform.position, hit.position) > 3f)
                    {
                        patrolTarget = hit.position;
                        controller.Movement.MoveTo(patrolTarget);
                        return;
                    }
                }
            }

            // fallback
            patrolTarget = controller.transform.position + controller.transform.forward * 5f;
            controller.Movement.MoveTo(patrolTarget);
        }
    }
}
