using PurrNet;
using System.Globalization;
using Unity.Cinemachine;
using UnityEngine;

public class ThirdPersonCamera : NetworkIdentity
{
    public PlayerController player;
    [SerializeField] private Transform cameraTarget;
    public CinemachineCamera cinemachineCamera;
    public CinemachineCamera combatCinemachineCamera;


    [Header("Look Settings")]
    [SerializeField] private float sensitivity = 120f;
    [SerializeField] private float minPitch = -40f;
    [SerializeField] private float maxPitch = 70f;

    private float yaw;
    private float pitch;

    private void Awake()
    {
        cinemachineCamera.gameObject.SetActive(false);
        combatCinemachineCamera.gameObject.SetActive(false);
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();

        if (!isOwner) return;

        cinemachineCamera.gameObject.SetActive(true);
        combatCinemachineCamera.gameObject.SetActive(true);

        Vector3 euler = cameraTarget.localEulerAngles;
        yaw = euler.y;
        pitch = euler.x;
    }

    private void Update()
    {
        if (!isOwner || player == null) return;

        RotateCamera();
    }

    private void RotateCamera()
    {
        Vector2 look = player.LookInput;

        yaw += look.x * sensitivity * Time.deltaTime;
        pitch -= look.y * sensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cameraTarget.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

}
