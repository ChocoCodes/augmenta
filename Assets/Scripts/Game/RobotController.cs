using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class RobotController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
    public float gravity = 9.81f;

    private Animator animator;
    private CharacterController characterController;
    private PlayerControls playerControls;
    private Vector2 moveInput;
    private Vector3 verticalVelocity;
    private Camera mainCamera;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
        
        if (animator != null)
        {
            // Keep AlwaysAnimate to ensure animations play reliably in AR
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            Debug.Log($"<color=green>Robot Animator Ready.</color> Controller: {animator.runtimeAnimatorController?.name}");
        }
        
        if (characterController != null)
        {
            characterController.skinWidth = 0.001f; 
            characterController.minMoveDistance = 0f;
        }

        playerControls = new PlayerControls();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
        playerControls.Player.Move.performed += OnMovePerformed;
        playerControls.Player.Move.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        playerControls.Player.Move.performed -= OnMovePerformed;
        playerControls.Player.Move.canceled -= OnMoveCanceled;
        playerControls.Player.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext context) => moveInput = Vector2.zero;

    private void Update()
    {
        ApplyGravity();
        MoveRobot();
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f; 
        }
        else
        {
            verticalVelocity.y -= gravity * Time.deltaTime;
        }

        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    private void MoveRobot()
    {
        if (moveInput.sqrMagnitude < 0.01f) return;

        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        Vector3 forward = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(mainCamera.transform.right, Vector3.up).normalized;

        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
        
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    public void TriggerJab() => SafeTrigger("Jab");
    public void TriggerHook() => SafeTrigger("Hook");
    public void TriggerUppercut() => SafeTrigger("Uppercut");
    public void TriggerCross() => SafeTrigger("Cross");

    private void SafeTrigger(string triggerName)
    {
        if (animator == null) return;

        // Reset all other triggers to prevent overlapping animation requests
        animator.ResetTrigger("Jab");
        animator.ResetTrigger("Hook");
        animator.ResetTrigger("Uppercut");
        animator.ResetTrigger("Cross");

        animator.SetTrigger(triggerName);
        Debug.Log($"<color=cyan>Trigger Sent to Controller:</color> {triggerName}");
    }
}
