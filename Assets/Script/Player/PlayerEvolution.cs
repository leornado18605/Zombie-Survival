using UnityEngine;
using Cinemachine;
using System.Collections;
public enum EvolutionStage { Ground, Evolved, Fly }

[DisallowMultipleComponent]
public class PlayerEvolution : MonoBehaviour
{
    [Header("Evolution Settings")]
    [SerializeField] private EvolutionStage currentStage = EvolutionStage.Ground;
    [SerializeField] private int dnaCollected = 0;
    [SerializeField] private int dnaToEvolveStage2 = 10;
    [SerializeField] private int dnaToEvolveStage3 = 25;

    [Header("Prefabs")]
    [SerializeField] private GameObject evolvedPrefab;
    [SerializeField] private GameObject flyPrefab;

    [Header("References")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Animator animator;
    [SerializeField] private CinemachineVirtualCamera vcam; // gán trong Inspector

    [Header("Effects")]
    [SerializeField] private ParticleSystem evolveEffect;
    [SerializeField] private AudioClip evolveSound;

    [SerializeField] private CinemachineCameraOrbit cameraOrbit; // gán thủ công trong Inspector
    
    [SerializeField] private Transform groundAnchor; // gán vị trí bàn chân của player

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        ApplyStage(currentStage);
    }

    // ---------------------------
    // Public API
    // ---------------------------
    public void AddDNA(int amount)
    {
        dnaCollected += amount;
        Debug.Log($"🧬 DNA Collected: {dnaCollected}");
        TryEvolve();
    }

    public void AssignCamera(CinemachineVirtualCamera cam)
    {
        vcam = cam;
    }

    public void SetInitialState(EvolutionStage stage, int dna)
    {
        currentStage = stage;
        dnaCollected = dna;
        ApplyStage(stage);
    }

    // ---------------------------
    // Evolution logic
    // ---------------------------
    public void TryEvolve()
    {
        Debug.Log($"🔎 TryEvolve() Stage={currentStage}, DNA={dnaCollected}");
        if (currentStage == EvolutionStage.Ground && dnaCollected >= dnaToEvolveStage2)
        {
            Debug.Log("✅ Evolve to Evolved");
            EvolveTo(EvolutionStage.Evolved);
        }
        else if (currentStage == EvolutionStage.Evolved && dnaCollected >= dnaToEvolveStage3)
        {
            Debug.Log("✅ Evolve to Fly");
            EvolveTo(EvolutionStage.Fly);
        }
    }

    private void EvolveTo(EvolutionStage newStage)
    {
        Debug.Log($"🧬 Evolving {currentStage} → {newStage}");

        // 🔹 Hiệu ứng tiến hóa
        if (animator) animator.SetTrigger("Evolve");
        if (evolveEffect) Instantiate(evolveEffect, transform.position, Quaternion.identity);
        if (evolveSound) audioSource.PlayOneShot(evolveSound);

        // ✅ Giữ lại vị trí & rotation của player cũ
        Vector3 pos = groundAnchor ? groundAnchor.position : transform.position;
        Quaternion rot = transform.rotation;


        // ✅ Xác định prefab kế tiếp
        GameObject nextPrefab = newStage switch
        {
            EvolutionStage.Evolved => evolvedPrefab,
            EvolutionStage.Fly => flyPrefab,
            _ => null
        };

        if (!nextPrefab)
        {
            Debug.LogWarning($"⚠️ Missing prefab for {newStage}");
            return;
        }

        // ✅ Spawn player mới đúng vị trí
        GameObject newPlayer = Instantiate(nextPrefab, pos, rot);

        var newCtrl = newPlayer.GetComponent<CharacterController>();
        
        if (Physics.Raycast(pos + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 5f, LayerMask.GetMask("Default")))
        {
            pos.y = hit.point.y; // đặt y chính xác tại mặt đất
            newPlayer.transform.position = pos;
        }
        else
        {
            Debug.LogWarning("⚠️ Không tìm thấy mặt đất dưới chân player!");
        }
        
        if (newCtrl)
        {
            newCtrl.enabled = false;
            newPlayer.transform.position = pos;
            newCtrl.enabled = true;
        }
        // ✅ Lấy script PlayerEvolution từ reference prefab có sẵn (không GetComponent)
        // Vì script nằm sẵn trong prefab → Unity tự liên kết khi bạn kéo vào Inspector.
        PlayerEvolution nextEvolution = newPlayer.GetComponent<PlayerEvolution>();

        if (nextEvolution == null)
        {
            Debug.LogError($"❌ Missing PlayerEvolution script in prefab {newPlayer.name}");
            Destroy(newPlayer);
            return;
        }

        // ✅ Gán dữ liệu và camera
        nextEvolution.SetInitialState(newStage, dnaCollected);
        nextEvolution.AssignCamera(vcam);

        // ✅ Cập nhật cho script CameraOrbit (gán sẵn reference trong Inspector)
        if (cameraOrbit != null)
            cameraOrbit.SetTarget(newPlayer.transform);

        // ✅ Gán Follow / LookAt
        if (vcam != null)
        {
            vcam.Follow = newPlayer.transform;
            vcam.LookAt = newPlayer.transform;
        }

        // ✅ Ẩn player cũ
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        // ✅ Xóa player cũ sau 0.3s
        Destroy(gameObject, 0.3f);
    }

    private void ApplyStage(EvolutionStage stage)
    {
        currentStage = stage;

        switch (stage)
        {
            case EvolutionStage.Ground:
                movement.EnableFlyingMode(false);
                movement.SetSpeedMultiplier(1f);
                break;

            case EvolutionStage.Evolved:
                movement.EnableFlyingMode(false);
                movement.SetSpeedMultiplier(1.5f);
                break;

            case EvolutionStage.Fly:
                movement.EnableFlyingMode(true);
                movement.SetSpeedMultiplier(1.8f);
                break;
        }
    }
}
