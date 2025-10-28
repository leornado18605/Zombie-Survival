using UnityEngine;
using Cinemachine;


[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CinemachineCameraOrbit : MonoBehaviour
{
    [SerializeField] private Transform target;          // Player hoặc Virus
    [SerializeField] private float sensitivityX = 150f; // Tốc độ xoay ngang
    [SerializeField] private float sensitivityY = 100f; // Tốc độ xoay dọc
    [SerializeField] private float minPitch = -30f;     // Giới hạn nhìn xuống
    [SerializeField] private float maxPitch = 60f;      // Giới hạn nhìn lên
    [SerializeField] private Vector3 followOffset = new Vector3(0, 3, -6);

    private CinemachineVirtualCamera vcam;
    private CinemachineTransposer transposer;
    private float yaw;
    private float pitch;

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
    }

    private void LateUpdate()
    {
        if (!target) return;

        // Nhận input chuột
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Xoay camera quanh target
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transposer.m_FollowOffset = rotation * followOffset;

        // Giữ camera luôn nhìn vào target
        vcam.LookAt = target;
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        yaw = 0;
        pitch = 0;
        if (vcam != null)
        {
            vcam.Follow = newTarget;
            vcam.LookAt = newTarget;
        }
    }

}