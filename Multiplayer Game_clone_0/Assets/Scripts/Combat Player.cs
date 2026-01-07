using UnityEngine;
using UnityEngine.InputSystem;

public class CombatPlayer : CombatEntity
{
    private InputAction interactAction;
    [SerializeField] private PlayerController playerController;
    private void Awake()
    {
        var actions = GetComponent<PlayerInput>().actions;
        interactAction = actions["Interact"];
    }
}
