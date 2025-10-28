using UnityEngine;

public sealed class AIStateIdle : IAIState
{
    private readonly AIController controller;
    private readonly float detectionRadius = 10f;
    private readonly LayerMask playerMask = LayerMask.GetMask("Player");

    private float idleTimer;
    private const float idleDuration = 2f;
    public AIStateIdle(AIController ctrl) => controller = ctrl;

    public void EnterState() { controller.Movement.Stop(); }

    public void UpdateState()
    {
        Collider[] hits = Physics.OverlapSphere(controller.transform.position, detectionRadius, playerMask);
        if (hits.Length > 0)
        {
            controller.Target = hits[0].transform;
            controller.SwitchState(AIController.AIStateType.Chase);
        }
        
        idleTimer += Time.deltaTime;
        if (idleTimer > -idleDuration)
        {
            controller.SwitchState(AIController.AIStateType.Patrol);
        }
    }

    public void ExitState() { }
}