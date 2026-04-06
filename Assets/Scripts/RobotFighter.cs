using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RobotFighter : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 10f;
    // The Inspector slot is gone. We handle it in code now!

    [Header("Animation Settings")]
    [Tooltip("Assign the child GameObject holding the robot's mesh here.")]
    [SerializeField] private Transform visualTransform; 

    [SerializeField] private float commandDistance = 0.12f;
    [SerializeField] private float commandLeanAngle = 20f;
    [SerializeField] private float commandSpeed = 12f;
    [SerializeField] private float commandHopHeight = 0.06f;
    [SerializeField] private float commandTwistAngle = 25f;

    private bool isReacting = false;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalScale;

    // --- SCRIPT-BASED INPUT ---
    private PlayerControls controls;

    private void Awake()
    {
        // Initialize the auto-generated input script
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        // Enable the "Player" action map
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    void Start()
    {
        if (visualTransform == null)
        {
            Debug.LogError("RobotFighter: Please assign a Visual Transform in the Inspector!");
            visualTransform = this.transform; 
        }

        originalLocalPosition = visualTransform.localPosition;
        originalLocalRotation = visualTransform.localRotation;
        originalScale = visualTransform.localScale;
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Read directly from the script reference
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > 0.01f)
        {
            Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            transform.Translate(moveDirection * (moveSpeed * Time.deltaTime), Space.World);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    public void PerformJab() { PerformOne(); }
    public void PerformOne() { TriggerPlaceholderReaction("one", PlaceholderAnimation.One); }
    public void PerformTwo() { TriggerPlaceholderReaction("two", PlaceholderAnimation.Two); }
    public void PerformThree() { TriggerPlaceholderReaction("three", PlaceholderAnimation.Three); }

    private enum PlaceholderAnimation { One, Two, Three }

    private void TriggerPlaceholderReaction(string commandName, PlaceholderAnimation animationType)
    {
        if (isReacting) return;
        StartCoroutine(PlaceholderReactionRoutine(animationType));
    }

    private IEnumerator PlaceholderReactionRoutine(PlaceholderAnimation animationType)
    {
        isReacting = true;

        originalLocalPosition = visualTransform.localPosition;
        originalLocalRotation = visualTransform.localRotation;
        originalScale = visualTransform.localScale;

        if (animationType == PlaceholderAnimation.One) yield return PlayOneAnimation();
        else if (animationType == PlaceholderAnimation.Two) yield return PlayTwoAnimation();
        else yield return PlayThreeAnimation();

        visualTransform.localPosition = originalLocalPosition;
        visualTransform.localRotation = originalLocalRotation;
        visualTransform.localScale = originalScale;
        isReacting = false;
    }

    private IEnumerator PlayOneAnimation()
    {
        Vector3 targetPos = originalLocalPosition + originalLocalRotation * Vector3.forward * commandDistance;
        Quaternion targetRot = originalLocalRotation * Quaternion.Euler(commandLeanAngle, 0f, 0f);
        Vector3 targetScale = originalScale * 1.05f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * commandSpeed;
            visualTransform.localPosition = Vector3.Lerp(originalLocalPosition, targetPos, t);
            visualTransform.localRotation = Quaternion.Lerp(originalLocalRotation, targetRot, t);
            visualTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * commandSpeed;
            visualTransform.localPosition = Vector3.Lerp(targetPos, originalLocalPosition, t);
            visualTransform.localRotation = Quaternion.Lerp(targetRot, originalLocalRotation, t);
            visualTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
    }

    private IEnumerator PlayTwoAnimation()
    {
        Vector3 leftPos = originalLocalPosition + originalLocalRotation * Vector3.left * (commandDistance * 0.7f);
        Vector3 rightPos = originalLocalPosition + originalLocalRotation * Vector3.right * commandDistance;
        Quaternion leftRot = originalLocalRotation * Quaternion.Euler(0f, 0f, commandLeanAngle);
        Quaternion rightRot = originalLocalRotation * Quaternion.Euler(0f, 0f, -commandLeanAngle);
        Vector3 squashedScale = new Vector3(originalScale.x * 0.95f, originalScale.y * 1.08f, originalScale.z * 0.95f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * commandSpeed;
            visualTransform.localPosition = Vector3.Lerp(originalLocalPosition, leftPos, t);
            visualTransform.localRotation = Quaternion.Lerp(originalLocalRotation, leftRot, t);
            visualTransform.localScale = Vector3.Lerp(originalScale, squashedScale, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * commandSpeed;
            visualTransform.localPosition = Vector3.Lerp(leftPos, rightPos, t);
            visualTransform.localRotation = Quaternion.Lerp(leftRot, rightRot, t);
            visualTransform.localScale = Vector3.Lerp(squashedScale, originalScale, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * commandSpeed;
            visualTransform.localPosition = Vector3.Lerp(rightPos, originalLocalPosition, t);
            visualTransform.localRotation = Quaternion.Lerp(rightRot, originalLocalRotation, t);
            visualTransform.localScale = Vector3.Lerp(originalScale, originalScale, t);
            yield return null;
        }
    }

    private IEnumerator PlayThreeAnimation()
    {
        Vector3 hopPos = originalLocalPosition + (originalLocalRotation * Vector3.up) * commandHopHeight;
        Quaternion twistRot = originalLocalRotation * Quaternion.Euler(0f, commandTwistAngle, 0f);
        Vector3 stretchedScale = new Vector3(originalScale.x * 1.08f, originalScale.y * 0.92f, originalScale.z * 1.08f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * commandSpeed;
            visualTransform.localPosition = Vector3.Lerp(originalLocalPosition, hopPos, t);
            visualTransform.localRotation = Quaternion.Lerp(originalLocalRotation, twistRot, t);
            visualTransform.localScale = Vector3.Lerp(originalScale, stretchedScale, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * commandSpeed;
            visualTransform.localPosition = Vector3.Lerp(hopPos, originalLocalPosition, t);
            visualTransform.localRotation = Quaternion.Lerp(twistRot, originalLocalRotation, t);
            visualTransform.localScale = Vector3.Lerp(stretchedScale, originalScale, t);
            yield return null;
        }
    }
}