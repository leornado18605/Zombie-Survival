using UnityEngine;

public enum EvolutionStage { Ground, Fly }

public class PlayerEvolution : MonoBehaviour
{
    [Header("Evolution Settings")]
    public EvolutionStage currentStage = EvolutionStage.Ground;
    public int dnaCollected = 0;
    public int dnaRequiredToEvolve = 10;

    [Header("References")]
    public PlayerMovement movement;
    public Animator animator;
    public GameObject groundModel;
    public GameObject flyModel;

    private void Start()
    {
        SetStage(EvolutionStage.Ground);
    }

    public void AddDNA(int amount)
    {
        dnaCollected += amount;
        Debug.Log($"DNA Collected: {dnaCollected}/{dnaRequiredToEvolve}");
    }

    public void TryEvolve()
    {
        if (currentStage == EvolutionStage.Ground && dnaCollected >= dnaRequiredToEvolve)
        {
            EvolveToFly();
        }
        else
        {
            Debug.Log("❌ Not enough DNA to evolve yet!");
        }
    }

    private void EvolveToFly()
    {
        SetStage(EvolutionStage.Fly);
        animator.SetTrigger("Evolve");
        Debug.Log("🧬 Virus has evolved into flying form!");
    }

    private void SetStage(EvolutionStage stage)
    {
        currentStage = stage;
        bool isFlying = (stage == EvolutionStage.Fly);

        groundModel.SetActive(!isFlying);
        flyModel.SetActive(isFlying);

        movement.EnableFlyingMode(isFlying);
    }
}