public sealed class AIStateChase : IAIState
{
    private readonly AIController controller;

    public AIStateChase(AIController ctrl) => controller = ctrl;

    public void EnterState() { }

    public void UpdateState()
    {
        if (controller.Target == null)
        {
            controller.SwitchState(AIController.AIStateType.Idle);
            return;
        }

        float distance = controller.Movement.DistanceTo(controller.Target.position);
        if (distance <= controller.Attack.AttackRange)
        {
            controller.SwitchState(AIController.AIStateType.Attack);
            return;
        }

        controller.Movement.MoveTo(controller.Target.position);
    }

    public void ExitState()
    {
        controller.Movement.Stop();
    }
}