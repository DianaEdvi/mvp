using UnityEngine;
using UnityEngine.InputSystem;

// Followd this tutorial which made a way better character controller than I could ever do 
// https://www.youtube.com/watch?v=qExUqdazurI

public class Player : MonoBehaviour

{
    // Input actions
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 moveInput;

    [Header("References")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform playerCameraLocal;
    [SerializeField] private Transform roomCameraTransform;
    
    [Header("Movement and Rotation")]

    [Tooltip("Movement speed of the player")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Tooltip("How quickly the player rotates to face the movement direction")]
    [SerializeField] private float rotateDampening = 0.1f;
    private float turnSmoothVelocity; // Middle man for smooth rotation

    [Header("Jumping")]

    [Tooltip("Gravity is self explanatory")]
    [SerializeField] private float gravity = -9.8f;

    [Tooltip("The initial upward velocity applied when the player jumps")]
    [SerializeField] private float jumpForce = 5.0f;
    private float verticalVelocity = 0f; // Tracks how fast the player is moving up or down

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();    
        MoveAndRotate();
        // Jump();   
    }
    private void MoveAndRotate()
    {
        // Convert the 2D input into a 3D movement direction
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        // 2. Calculate vertical movement separately to apply gravity and jumping
        Vector3 verticalMove = new Vector3(0, verticalVelocity, 0) * Time.deltaTime; // TODO GET RID OF

        // Only rotate and move the player if there's horizontal input
        if (moveDirection.magnitude >= 0.1f)
        {
            // CHANGE OF BASIS
            // Get the camera's directional basis vectors
            Vector3 camForward = roomCameraTransform.forward;
            Vector3 camRight = roomCameraTransform.right;

            // Project them onto the horizontal plane (ignore camera tilt up/down)
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize(); // Fix the shrinking of the vectors
            camRight.Normalize();

            // Calculate final movement direction relative to the Room Camera
            Vector3 finalMoveDir = (camForward * moveDirection.z) + (camRight * moveDirection.x); // Linear combination 
            finalMoveDir.Normalize();

            // ROTATION
            // Face the direction of movement
            float targetAngle = Mathf.Atan2(finalMoveDir.x, finalMoveDir.z) * Mathf.Rad2Deg;
            float smoothTargetAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotateDampening);
            transform.rotation = Quaternion.Euler(0, smoothTargetAngle, 0);

            // MOVEMENT
            // Move the player by combining horizontal movement and vertical movement
            characterController.Move(finalMoveDir * moveSpeed * Time.deltaTime + verticalMove); // TODO get rid of vertical
        }
        else
        {
            characterController.Move(verticalMove);
        }
    }

    private void Jump()
    {
        // Check if the player is grounded before allowing them to jump
        if (characterController.isGrounded)
        {
            // Apply small downward force to keep the player grounded and prevent floating
            verticalVelocity = -1f;

            // If the jump button was pressed this frame, apply the jump force
            if (jumpAction.WasPressedThisFrame())
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            // If the player is in the air, apply gravity to pull them down
            verticalVelocity += gravity * Time.deltaTime;
        }

    }


}
