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

    private float playerSpeed;
    private bool isPlayerMoving;
    private int jumpCounter = 0;

    private void Update()
    {
        Jump();
        Sprint();

        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        isPlayerMoving = !(horizontalAxis == 0.0f) || !(verticalAxis == 0.0f);


        // Translate the player on a new Vector3, along the Horizontal and Vertical input axis, along the local axis
        if (isPlayerMoving)
            transform.Translate(new Vector3(horizontalAxis * Time.deltaTime * playerSpeed, 0, verticalAxis * Time.deltaTime * playerSpeed));

    }

    public void Sprint()
    {
        // ... and is holding down the shift key...
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && isPlayerMoving)
        {
            // ... sprint...
            playerSpeed = walkSpeed + sprintSpeed;
            // ... and change the FOV...
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, defaultFov + sprintFov, fovSpeed);
        }
        else
        {
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
            playerRigidBody.AddForce(Vector3.up * jumpForce);
            jumpCounter++;
        }
    }
}
