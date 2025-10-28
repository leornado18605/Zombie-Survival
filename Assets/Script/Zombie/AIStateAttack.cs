public sealed class AIStateAttack : IAIState
{
    private readonly AIController controller;

    public AIStateAttack(AIController ctrl) => controller = ctrl;

    public void EnterState() { controller.Movement.Stop(); }

    public void UpdateState()
    {
        if (controller.Target == null)
        {
            controller.SwitchState(AIController.AIStateType.Idle);
            return;
        }

        controller.Movement.LookAt(controller.Target.position);

        float distance = controller.Movement.DistanceTo(controller.Target.position);
        if (distance > controller.Attack.AttackRange + 1f)
        {
            controller.SwitchState(AIController.AIStateType.Chase);
            return;
        }

        if (controller.Attack.CanAttack())
            controller.Attack.ExecuteAttack(controller.transform, controller.Movement.Agent);
    }

    public void ExitState()
    {
        controller.Attack.StopAiming();
    }
}