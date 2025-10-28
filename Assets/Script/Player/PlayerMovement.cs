using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float moveSpeed = 8f;       // tốc độ bay ngang
    [SerializeField] private float verticalSpeed = 5f;   // tốc độ bay lên/xuống
    [SerializeField] private float rotationSpeed = 10f;  // độ mượt khi xoay

    [Header("Flight Settings")]
    [SerializeField] private bool canFly = false; // chỉ true khi tiến hóa

    private CharacterController controller;
    private Animator animator;
    private Vector3 moveInput;
    private bool isRunning;
    private float gravity = -9.81f;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 🧪 Test: Nhấn F để bật/tắt chế độ bay
        if (Input.GetKeyDown(KeyCode.F))
        {
            canFly = !canFly;
            Debug.Log("Flight mode: " + canFly);
        }

        HandleMovement();
    }

    public void EnableFlyingMode(bool enable)
    {
        canFly = enable;
    }

    public void SetMoveInput(float horizontal, float vertical, bool running)
    {
        moveInput = new Vector3(horizontal, 0f, vertical).normalized;
        isRunning = running;
    }

    private void HandleMovement()
    {
        // Hướng di chuyển theo camera
        Vector3 moveDir = (Camera.main.transform.forward * moveInput.z +
                           Camera.main.transform.right * moveInput.x);
        moveDir.y = 0f;
        moveDir.Normalize();

        float speed = isRunning ? moveSpeed * 1.5f : moveSpeed;

        Vector3 finalVelocity;

        if (canFly)
        {
            // 🕊 Bay tự do
            float verticalInput = 0f;
            if (Input.GetKey(KeyCode.V)) verticalInput = 1f;
            else if (Input.GetKey(KeyCode.LeftControl)) verticalInput = -1f;

            Vector3 verticalMove = Vector3.up * verticalInput * verticalSpeed;
            finalVelocity = moveDir * speed + verticalMove;

            // Xoay theo hướng di chuyển ngang
            if (moveDir.sqrMagnitude > 0.1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }

            // ✨ Hiệu ứng ngẩng/cúi đầu
            float tiltAngle = 0f;
            if (verticalInput > 0) tiltAngle = -50f;
            else if (verticalInput < 0) tiltAngle = 30f;

            Quaternion targetTilt = Quaternion.Euler(tiltAngle, transform.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetTilt, 3f * Time.deltaTime);
        }
        else
        {
            // 🚶‍♂️ Đi bộ
            verticalVelocity += gravity * Time.deltaTime;
            if (controller.isGrounded && verticalVelocity < 0)
                verticalVelocity = -2f;

            Vector3 move = moveDir * walkSpeed;
            move.y = verticalVelocity;
            finalVelocity = move;

            // Xoay theo hướng di chuyển
            if (moveDir.sqrMagnitude > 0.1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }

        controller.Move(finalVelocity * Time.deltaTime);
    }
}
