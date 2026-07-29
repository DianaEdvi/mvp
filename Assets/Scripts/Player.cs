using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    [Header("Input Setup")]
    public InputActionReference moveAction;
    public InputActionReference inventoryAction;
    public InputActionReference interactAction;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = -9.81f;
    private float verticalVelocity;

    private Transform mainCameraTransform;
    private CharacterController characterController;

    public Action OnInventoryActionPerformed;
    public Action OnInteractActionPerformed;

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;
        characterController = GetComponent<CharacterController>();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        if (Camera.main != null) mainCameraTransform = Camera.main.transform;
        else Debug.LogError("No main camera found! Does main camera have MainCamera tag?");

    }

    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
        if (inventoryAction != null)
        {
            inventoryAction.action.Enable();
            inventoryAction.action.performed += ctx => OnInventoryOpened();
        }

        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += ctx => OnInteract();
        }

    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
        if (inventoryAction != null) inventoryAction.action.Disable();
        if (interactAction != null) interactAction.action.Disable();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (mainCameraTransform == null || moveAction == null) return;

        // Gravity. Make sure player is grounded
        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        // Acceleration
        verticalVelocity += gravity * Time.deltaTime;


        // Movement
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        Vector3 camForward = mainCameraTransform.forward;
        Vector3 camRight = mainCameraTransform.right;

        // Change of basis
        camForward.y = 0; // Project onto forward-right plane 
        camRight.y = 0;
        camForward.Normalize(); // Fix squashed vectors 
        camRight.Normalize();

        Vector3 horizontalMoveDirection = (camForward * moveInput.y) + (camRight * moveInput.x); // Linear combination 


        // Combine horizontal and vertical
        Vector3 finalVelocity = (horizontalMoveDirection * moveSpeed) + (Vector3.up * verticalVelocity);

        // Move the Character Controller
        characterController.Move(finalVelocity * Time.deltaTime);


        // Rotation
        if (horizontalMoveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalMoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void TeleportPlayer(Vector3 newPosition)
    {
        if (characterController != null) characterController.enabled = false;

        transform.position = newPosition;

        if (characterController != null) characterController.enabled = true;

        verticalVelocity = -2f;
    }

    public void OnInventoryOpened()
    {
        OnInventoryActionPerformed?.Invoke();
    }

    public void OnInteract()
    {
        OnInteractActionPerformed?.Invoke();
    }
}