using UnityEngine;

public sealed class AIStateDead : IAIState
{
    private readonly AIController controller;
    private static readonly int AnimDead = Animator.StringToHash("Dead");

    public AIStateDead(AIController ctrl)
    {
        controller = ctrl;
    }

    public void EnterState()
    {
        controller.Movement.Stop();
        controller.GetComponent<Animator>().SetTrigger(AnimDead);
        controller.GetComponent<Collider>().enabled = false;
    }

    public void UpdateState()
    {
        // Dead — không làm gì
    }

    public void ExitState() { }
}