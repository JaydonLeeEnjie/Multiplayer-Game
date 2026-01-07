using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerController : NetworkIdentity
{
    private CharacterController charactercontroller;
    [SerializeField] private NetworkAnimator PlayerAnimator;
    public NetworkAnimator animator;
    [SerializeField] private float Speed = 3f;
    public CombatPlayer CombatPlayer;
    private InputAction moveAction;
    private InputAction lookAction;

    public ThirdPersonCamera thirdPersonCamera;
    private Transform pendingCombatSpot;

    public Vector2 LookInput => lookAction.ReadValue<Vector2>();
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;
    private bool wasGrounded;


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
        GroundCheck();
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        PlayerAnimator.SetBool("Walking", isGrounded && input.sqrMagnitude != 0);
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

    private void GroundCheck()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

    }

    public void MoveCombatPlayerToSpot(Transform spot)
    {
        if (CombatPlayer == null) return;

        CombatPlayer.transform.position = spot.position;
        CombatPlayer.transform.rotation = spot.rotation;
        CombatPlayer.gameObject.SetActive(true);
        thirdPersonCamera.combatCinemachineCamera.Prioritize();
        gameObject.SetActive(false);
    }

    public void OnEnterCombatAnimationEvent()
    {
        if (pendingCombatSpot == null)
        {
            Debug.LogWarning("Combat spot missing");
            return;
        }

        MoveCombatPlayerToSpot(pendingCombatSpot);
        pendingCombatSpot = null;
    }


    public void MovePlayerBack()
    {
        if (CombatPlayer == null) return;

        CombatPlayer.gameObject.SetActive(false);
        thirdPersonCamera.cinemachineCamera.Prioritize();
        gameObject.SetActive(true);
    }

    public void PrepareEnterCombat(Transform spot)
    {
        pendingCombatSpot = spot;

        // Trigger animation (network-safe)
        animator.Play("CameraTransition");
    }




}
