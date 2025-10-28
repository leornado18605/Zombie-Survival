using UnityEngine;

public sealed class AIController : MonoBehaviour
{
    [SerializeField] private AIMovement movement;
    [SerializeField] private AIAttack attack;

    private IAIState currentState;
    private IAIState idleState;
    private IAIState chaseState;
    private IAIState attackState;
    private IAIState deadState;
    private IAIState patrolState;  
    
    public Transform Target { get; set; }

    public AIMovement Movement => movement;
    public AIAttack Attack => attack;

    private void Awake()
    {
        idleState = new AIStateIdle(this);
        patrolState = new AIStatePatrol(this);
        chaseState = new AIStateChase(this);
        attackState = new AIStateAttack(this);
        deadState = new AIStateDead(this);

        currentState = idleState;
        currentState = patrolState;  
    }

    private void Update()
    {
        currentState?.UpdateState();
    }

    public void SwitchState(AIStateType type)
    {
        currentState?.ExitState();

        currentState = type switch
        {
            AIStateType.Idle => idleState,
            AIStateType.Patrol => patrolState,
            AIStateType.Chase => chaseState,
            AIStateType.Attack => attackState,
            AIStateType.Dead => deadState,
            _ => idleState
        };

        currentState.EnterState();
    }

    public enum AIStateType { Idle,Patrol, Chase, Attack, Dead }
}