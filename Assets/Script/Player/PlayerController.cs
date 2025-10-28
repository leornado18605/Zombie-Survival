using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerAttack))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerAttack attack;
    private PlayerEvolution evolution;
    
    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
        evolution = GetComponent<PlayerEvolution>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        
        movement.SetMoveInput(horizontal, vertical, isRunning);

        if (Input.GetMouseButtonDown(0)) 
        {
            attack.StartAttack();
        }
        
        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
            attack.CancelAttack();
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            evolution.TryEvolve(); // <-- chỉ gửi lệnh, không xử lý logic ở đây
        }
    }
}