using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variable Initials
    [SerializeField] Rigidbody playerRigidBody;
    [SerializeField] GameObject playerGameObject;
    [SerializeField] Camera mainCamera;
    [Header("Movement Speed")]
    [SerializeField] float walkSpeed;
    [SerializeField] float lowStaminaSpeed;
    [Tooltip("The speed added onto the walkSpeed attribute while sprinting")][SerializeField] float sprintSpeed;
    [Tooltip("The speed at which player movement is smoothed (Larger numbers decrease smoothness)")][SerializeField] float inputSmoothing;
    [Header("Field of View")]
    [Tooltip("The speed of the transition between FOVs")][SerializeField][Range(0f, 1f)] float fovSpeed;
    [SerializeField] float defaultFov;
    [Tooltip("The FOV added onto the defaultFov attribute while sprinting")][SerializeField] float sprintFov;
    [SerializeField] float currentFov;
    [Header("Jump")]
    [Tooltip("The distance of the ray used to identify if the player is on the ground (keep at player's Y scale)")][SerializeField] float raycastDistance;
    [SerializeField] float jumpForce;
    [SerializeField] float lowStaminaJumpForce;
    [Tooltip("The maximum number of jumps the player can do mid-air")][SerializeField] int maxJumps;
    [Header("Stamina")]
    [SerializeField]
    [Range(0f, 1f)] private float sprintStaminaDrainRate;
    [SerializeField][Range(0f, 1f)] private float moveStaminaDrainRate;
    [SerializeField][Range(0f, 1f)] private float staminaRegenRate;
    [SerializeField][Range(0f, 1f)] private float jumpStaminaLoss;
    public float stamina = 1f;
    [SerializeField][Range(0f, 1f)] private float staminaCorrectionTolerance;

    private float playerSpeed;
    private bool isPlayerMoving;
    private int jumpCounter;
    private Vector2 movementInput = Vector2.zero;
    private Vector2 movementInputRaw = Vector2.zero;
    private float sprintInput;

    public Player PlayerObject = new Player();
    #endregion

    public void FixedUpdate()
    {
        isPlayerMoving = (movementInputRaw == Vector2.zero);

        movementInput = Vector2.Lerp(movementInput, movementInputRaw, Time.deltaTime * inputSmoothing);
        playerSpeed = walkSpeed;

        if (Physics.Raycast(transform.position, Vector3.down, raycastDistance))
            jumpCounter = 0;
        
        if (!(movementInput == Vector2.zero))
        {
            float[] returns;

            if (!(sprintInput == 0.0f) && stamina > staminaCorrectionTolerance)
            {
                returns = PlayerObject.Sprint(stamina, sprintStaminaDrainRate, playerSpeed, walkSpeed, sprintSpeed, currentFov, defaultFov, sprintFov);
                stamina = returns[0];
                playerSpeed = returns[1];
                currentFov = returns[2];
            }

            currentFov = PlayerObject.MovePlayer(stamina, staminaCorrectionTolerance,moveStaminaDrainRate, sprintInput, currentFov, defaultFov, playerSpeed, lowStaminaSpeed, playerRigidBody, playerGameObject, movementInput);
        }
        
        if (isPlayerMoving)
        {
            if (stamina < 1f) 
                stamina += Time.deltaTime * staminaRegenRate;
            currentFov = defaultFov;
        }

        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, currentFov, fovSpeed);
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        movementInputRaw = value.ReadValue<Vector2>();
    }
    public void OnSprint(InputAction.CallbackContext value)
    {
        sprintInput = value.ReadValue<float>();
    }
    public void OnJump(InputAction.CallbackContext value)
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

    public class Player
    {
        public float MovePlayer(float stamina, float staminaCorrectionTolerance, float moveStaminaDrainRate, float sprintInput, float currentFov, float defaultFov, float playerSpeed, float lowStaminaSpeed, Rigidbody rigidBody, GameObject gameObject, Vector2 movementInput)
        {
            if (stamina > staminaCorrectionTolerance)
                stamina -= Time.deltaTime * moveStaminaDrainRate;

            if (!(sprintInput == 1f))
                currentFov = defaultFov;

            if (stamina < staminaCorrectionTolerance)
            {
                playerSpeed = lowStaminaSpeed;
                currentFov = defaultFov;
            }

            rigidBody.MovePosition(gameObject.transform.position + gameObject.transform.TransformDirection(new Vector3(movementInput.x * Time.deltaTime * playerSpeed, 0, movementInput.y * Time.deltaTime * playerSpeed)));
            return currentFov;
        }
        public float[] Sprint(float stamina, float sprintStaminaDrainRate, float playerSpeed, float walkSpeed, float sprintSpeed, float currentFov, float defaultFov, float sprintFov)
        {
            stamina -= Time.deltaTime * sprintStaminaDrainRate;
            playerSpeed = walkSpeed + sprintSpeed;
            currentFov = defaultFov + sprintFov;

            float[] returns = { stamina, playerSpeed, currentFov };

            return returns;
        }
    }
}