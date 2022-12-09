using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variable Initials
    [SerializeField] Rigidbody globalRigidBody;
    [SerializeField] GameObject globalGameObject;
    [SerializeField] Camera mainCamera;

    [Header("Movement Speed")]
    [SerializeField] float walkSpeed;
    [Tooltip("The player speed while low on stamina")][SerializeField] float lowStaminaSpeed;
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
    [SerializeField][Range(0f, 1f)] private float staminaCorrectionTolerance;

    private Vector2 movementInput = Vector2.zero;
    private Vector2 movementInputRaw = Vector2.zero;
    private float sprintInput;

    public Player PlayerObject = new Player();
    #endregion

    public void Awake()
    {
        PlayerObject.rigidBody = globalRigidBody;
        PlayerObject.gameObject = globalGameObject;
    }

    public void Update()
    {
        movementInput = Vector2.Lerp(movementInput, movementInputRaw, Time.deltaTime * inputSmoothing);
        PlayerObject.playerSpeed = walkSpeed;

        if (Physics.Raycast(transform.position, Vector3.down, raycastDistance))
            PlayerObject.jumpCounter = 0;

        if (!(movementInput == Vector2.zero))
        {
            if (!(sprintInput == 0.0f) && PlayerObject.stamina > staminaCorrectionTolerance)
            {
                PlayerObject.Sprint(sprintStaminaDrainRate, walkSpeed, sprintSpeed);
                currentFov = defaultFov + sprintFov;
            }
            else
            {
                currentFov = defaultFov;
            }

            if (PlayerObject.stamina < staminaCorrectionTolerance)
                PlayerObject.playerSpeed = lowStaminaSpeed;

            PlayerObject.MovePlayer(moveStaminaDrainRate, movementInput);
        }
        
        if (movementInputRaw == Vector2.zero)
        {
            if (PlayerObject.stamina < 1f)
                PlayerObject.stamina += Time.deltaTime * staminaRegenRate;
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
        if (PlayerObject.jumpCounter < maxJumps)
        {
            if (PlayerObject.stamina > jumpStaminaLoss + staminaCorrectionTolerance)
            {
                PlayerObject.Jump(jumpForce, jumpStaminaLoss);
            }
            else
            {
                PlayerObject.Jump(lowStaminaJumpForce, 0);
            }
        }
    }

    public class Player
    {
        public float stamina = 1f;
        public float playerSpeed;
        public int jumpCounter;
        public Rigidbody rigidBody;
        public GameObject gameObject;

        public void MovePlayer(float moveStaminaDrainRate, Vector2 movementInput)
        {
            stamina -= Time.deltaTime * moveStaminaDrainRate;
            rigidBody.MovePosition(gameObject.transform.position + gameObject.transform.TransformDirection(new Vector3(movementInput.x * Time.deltaTime * playerSpeed, 0, movementInput.y * Time.deltaTime * playerSpeed)));
        }
        public void Sprint(float sprintStaminaDrainRate, float walkSpeed, float sprintSpeed)
        {
            stamina -= Time.deltaTime * sprintStaminaDrainRate;
            playerSpeed = walkSpeed + sprintSpeed;
        }
        public void Jump(float jumpForce, float jumpStaminaLoss)
        {
            rigidBody.AddForce(Vector3.up * jumpForce);
            stamina -= jumpStaminaLoss;
            jumpCounter++;
        }
    }
}