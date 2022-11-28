using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]  Rigidbody playerRigidBody;
    [Header("Movement Speed")]
    [SerializeField]  float walkSpeed;
    [Tooltip("The speed added onto the walkSpeed attribute while sprinting")]
    [SerializeField]  float sprintSpeed;
    [Header("Field of View")]
    [Tooltip("The speed of the transition between FOVs")]
    [SerializeField][Range(0f, 1f)] float fovSpeed;
    [SerializeField] float defaultFov;
    [Tooltip("The FOV added onto the defaultFov attribute while sprinting")]
    [SerializeField] float sprintFov;
    [Header("Jump")]
    [Tooltip("The distance of the ray used to identify if the player is on the ground (keep at player's Y scale)")]
    [SerializeField] float raycastDistance;
    [SerializeField] float jumpForce;
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
    [HideInInspector]
    public float stamina = 1f;

    private float playerSpeed;
    private bool isPlayerSprinting;
    private bool isPlayerMoving;
    private int jumpCounter = 0;

    private void Update()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        isPlayerSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        isPlayerMoving = !(horizontalAxis == 0.0f) || !(verticalAxis == 0.0f);

        Jump();
        Sprint();

        // Translate the player on a new Vector3, along the Horizontal and Vertical input axis, along the local axis
        if (isPlayerMoving)
        {
            transform.Translate(new Vector3(horizontalAxis * Time.deltaTime * playerSpeed, 0, verticalAxis * Time.deltaTime * playerSpeed));

            // If the player isn't sprinting and we have stamina, reduce stamina by moveStaminaDrainRate
            if (!isPlayerSprinting && stamina > moveStaminaDrainRate)
                stamina -= Time.deltaTime * moveStaminaDrainRate;
        } 

    }

    public void Sprint()
    {
        // If the player is sprinting...
        if (isPlayerSprinting && isPlayerMoving && stamina > sprintStaminaDrainRate)
        {
            // Reduce stamina...
            stamina -= Time.deltaTime * sprintStaminaDrainRate;

            // ... and if we still have some...
            if (stamina > 0f)
            {
                // ... sprint...
                playerSpeed = walkSpeed + sprintSpeed;
                // ... and change the FOV
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, defaultFov + sprintFov, fovSpeed);
            }
        }
        else
        {
            // Regenerate stamina if the player isn't moving
            if (stamina < 1f && !isPlayerMoving)
                stamina += Time.deltaTime * staminaRegenRate;

            // ... reset playerSpeed...
            playerSpeed = walkSpeed;
            // ... and reset the FOV
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, defaultFov, fovSpeed);
        }
    }
    public void Jump()
    {
        // Reset the jumpCounter if player is on the ground
        if (Physics.Raycast(transform.position, Vector3.down, raycastDistance))
            jumpCounter = 0;

        // If the player is jumping and hasn't jumped more than maxJumps...
        if (Input.GetButtonDown("Jump") && jumpCounter < maxJumps)
        {
            //... jump...
            playerRigidBody.AddForce(Vector3.up * jumpForce);
            jumpCounter++;

            // ... and reduce stamina by jumpStaminaLoss
            if (stamina > jumpStaminaLoss)
                stamina -= jumpStaminaLoss;
        }
    }
}
