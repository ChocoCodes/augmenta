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
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        
        // Fix for small scales: Reduce the collision buffer and minimum move distance
        if (characterController != null)
        {
            characterController.skinWidth = 0.001f; 
            characterController.minMoveDistance = 0f;
        }

        playerControls = new PlayerControls();
        mainCamera = Camera.main;
        if (mainCamera == null) mainCamera = FindFirstObjectByType<Camera>();
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

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

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
        if (moveInput.sqrMagnitude < 0.01f)
        {
            if (animator != null) animator.SetBool("isMoving", false);
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) mainCamera = FindFirstObjectByType<Camera>();
            if (mainCamera == null) return;
        }

        // --- Improved Camera Perspective Logic ---
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        // If looking nearly straight down (common in AR), use the camera's "up" projected on the plane as forward
        if (Mathf.Abs(forward.y) > 0.9f)
        {
            forward = Vector3.ProjectOnPlane(mainCamera.transform.up, Vector3.up).normalized;
        }
        else
        {
            forward.y = 0;
            forward.Normalize();
        }

        right.y = 0;
        right.Normalize();

        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
        // ------------------------------------------
        
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        
        if (animator != null) animator.SetBool("isMoving", true);
    }

    public void TriggerJab()
    {
        if (animator != null) animator.SetTrigger("Jab");
    }

    public void TriggerHook()
    {
        if (animator != null) animator.SetTrigger("Hook");
    }

    public void TriggerUppercut()
    {
        if (animator != null) animator.SetTrigger("Uppercut");
    }

    public void TriggerCross()
    {
        if (animator != null) animator.SetTrigger("Cross");
    }

    public void TriggerBlock()
    {
        if (animator != null) animator.SetTrigger("Block");
    }
}
