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
    [Range(0f, 10f)]
    public float maxStamina;
    [SerializeField]
    [Range(0f, 10f)]
    private float sprintStaminaDrainRate;
    [SerializeField]
    [Range(0f, 10f)]
    private float moveStaminaDrainRate;
    [SerializeField]
    [Range(0f, 10f)]
    private float staminaRegenRate;
    [SerializeField]
    [Range(0f, 10f)]
    private float jumpStaminaLoss;
    [HideInInspector]
    public float stamina;

    private float playerSpeed;
    private bool isPlayerSprinting;
    private bool isPlayerMoving;
    private int jumpCounter = 0;

    private void Start()
    {
        stamina = maxStamina;
    }

    private void Update()
    {
        isPlayerSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        Jump();
        Sprint();

        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        isPlayerMoving = !(horizontalAxis == 0.0f) || !(verticalAxis == 0.0f);


        // Translate the player on a new Vector3, along the Horizontal and Vertical input axis, along the local axis
        if (isPlayerMoving)
        {
            transform.Translate(new Vector3(horizontalAxis * Time.deltaTime * playerSpeed, 0, verticalAxis * Time.deltaTime * playerSpeed));
            if (!isPlayerSprinting)
                stamina -= Time.deltaTime * moveStaminaDrainRate;
        } 

    }

    public void Sprint()
    {
        // If the player is trying to sprint...
        if (isPlayerSprinting && isPlayerMoving && stamina > 0f)
        {
            stamina -= Time.deltaTime * sprintStaminaDrainRate;

            if (stamina > 0f)
            {
                // ... sprint...
                playerSpeed = walkSpeed + sprintSpeed;
                // ... and change the FOV...
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, defaultFov + sprintFov, fovSpeed);
            }
        }
        else
        {

            if (stamina < maxStamina && !isPlayerMoving)
                stamina += Time.deltaTime * staminaRegenRate;

            // ... or don't, if the shift key isn't being held down...
            playerSpeed = walkSpeed;
            // ... and reset the FOV
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, defaultFov, fovSpeed);
        }
    }
    public void Jump()
    {
        if (Physics.Raycast(transform.position, Vector3.down, raycastDistance))
            jumpCounter = 0;

        // If the player is jumping and hasn't jumped more than maxJumps...
        if (Input.GetButtonDown("Jump") && jumpCounter < maxJumps)
        {
            //... jump
            playerRigidBody.AddForce(Vector3.up * jumpForce);
            jumpCounter++;
            stamina -= jumpStaminaLoss;
        }
    }
}
