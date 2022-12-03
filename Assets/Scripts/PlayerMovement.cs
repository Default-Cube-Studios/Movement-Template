using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody playerRigidBody;
    [SerializeField] Camera mainCamera;
    [Header("Movement Speed")]
    [SerializeField] float walkSpeed;
    [SerializeField] float lowStaminaSpeed;
    [Tooltip("The speed added onto the walkSpeed attribute while sprinting")]
    [SerializeField] float sprintSpeed;
    [Header("Field of View")]
    [Tooltip("The speed of the transition between FOVs")]
    [SerializeField][Range(0f, 1f)] float fovSpeed;
    [SerializeField] float defaultFov;
    [Tooltip("The FOV added onto the defaultFov attribute while sprinting")]
    [SerializeField] float sprintFov;
    [SerializeField] float currentFov;
    [Header("Jump")]
    [Tooltip("The distance of the ray used to identify if the player is on the ground (keep at player's Y scale)")]
    [SerializeField] float raycastDistance;
    [SerializeField] float jumpForce;
    [SerializeField] float lowStaminaJumpForce;
    [Tooltip("The maximum number of jumps the player can do mid-air")]
    [SerializeField] int maxJumps;
    [Header("Stamina")]
    [SerializeField]
    [Range(0f, 1f)]
    private float sprintStaminaDrainRate;
    [SerializeField]
    [Range(0f, 1f)]
    private float moveStaminaDrainRate;
    [SerializeField]
    [Range(0f, 1f)]
    private float staminaRegenRate;
    [SerializeField]
    [Range(0f, 1f)]
    private float jumpStaminaLoss;
    public float stamina = 1f;
    [SerializeField]
    [Range(0f, 1f)]
    private float staminaCorrectionTolerance;

    private float playerSpeed;
    private bool isPlayerSprinting;
    private bool isPlayerMoving;
    private int jumpCounter = 0;

    public void Update()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        isPlayerSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        isPlayerMoving = !(horizontalAxis == 0.0f) || !(verticalAxis == 0.0f);

        playerSpeed = walkSpeed;

        // Reset the jumpCounter if player is on the ground
        if (Physics.Raycast(transform.position, Vector3.down, raycastDistance))
            jumpCounter = 0;
        
        Jump();

        if (isPlayerMoving)
        {
            if (isPlayerSprinting)
                Sprint();
            else
                currentFov = defaultFov;

            MovePlayer(horizontalAxis, verticalAxis);
        }
        else
        {
            if (stamina < 1f)
                stamina += Time.deltaTime * staminaRegenRate;
            currentFov = defaultFov;
        }

        TransitionFov(mainCamera.fieldOfView, currentFov, fovSpeed);
    }

    public void Sprint()
    {
        if (stamina < staminaCorrectionTolerance)
        {
            playerSpeed = lowStaminaSpeed;
            currentFov = defaultFov;
            return;
        }

        stamina -= Time.deltaTime * sprintStaminaDrainRate;
        playerSpeed = walkSpeed + sprintSpeed;
        currentFov = defaultFov + sprintFov;
    }

    public void TransitionFov(float currentFov, float newFov, float speed)
    {
        mainCamera.fieldOfView = Mathf.Lerp(currentFov, newFov, speed);
    }

    public void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCounter < maxJumps)
        {
            if (stamina > staminaCorrectionTolerance && stamina > jumpStaminaLoss)
            {
                playerRigidBody.AddForce(Vector3.up * jumpForce);
                stamina -= jumpStaminaLoss;
            }
            else
                playerRigidBody.AddForce(Vector3.up * lowStaminaJumpForce);

            jumpCounter++;
        }
    }
    public void MovePlayer(float horizontalAxis, float verticalAxis)
    {
        if (stamina > staminaCorrectionTolerance)
            stamina -= Time.deltaTime * moveStaminaDrainRate;
        else
            playerSpeed = lowStaminaSpeed;

        transform.Translate(new Vector3(horizontalAxis * Time.deltaTime * playerSpeed, 0, verticalAxis * Time.deltaTime * playerSpeed));

    }
}