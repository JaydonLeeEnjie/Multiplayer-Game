using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerController : NetworkIdentity
{
    private CharacterController charactercontroller;
    [SerializeField] private float Speed = 3f;
    private InputAction moveAction;
    private InputAction lookAction;
    public Vector2 LookInput => lookAction.ReadValue<Vector2>();
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float rotationSpeed = 10f;


    private void Awake()
    {
        charactercontroller = GetComponent<CharacterController>();
        var actions = GetComponent<PlayerInput>().actions;
        moveAction = actions["Move"];
        lookAction = actions["Look"];
    }
    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;
    }

    private void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        if (input.sqrMagnitude < 0.01f)
            return;

        // Camera forward & right (flattened)
        Vector3 camForward = cameraTarget.forward;
        Vector3 camRight = cameraTarget.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        // Movement direction relative to camera
        Vector3 moveDir = camForward * input.y + camRight * input.x;

        // Move
        charactercontroller.Move(moveDir * Speed * Time.deltaTime);

        // Rotate player towards movement direction
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

}
