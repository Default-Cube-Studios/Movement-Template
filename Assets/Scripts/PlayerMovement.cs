using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variable Initials
    [HideInInspector] public GameObject playerGameObject;
    [HideInInspector] public Rigidbody rigidBody;

    [Header("Movement Speed")]
    [SerializeField] float walkSpeed;
    [Tooltip("The player speed while low on stamina")][SerializeField] float lowStaminaSpeed;
    [Tooltip("The player speed while sprinting")][SerializeField] float sprintSpeed;
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
    #endregion

    public void Awake()
    {
        rigidBody = Player.PlayerObject.rigidBody;
        playerGameObject = Player.PlayerObject.gameObject;
    }

    public void Update()
    {
        movementInput = Vector2.Lerp(movementInput, movementInputRaw, Time.deltaTime * inputSmoothing);
        Player.PlayerObject.playerSpeed = walkSpeed;

        if (Physics.Raycast(gameObject.transform.position, Vector3.down, raycastDistance))
            Player.PlayerObject.jumpCounter = 0;

        if (!(movementInput == Vector2.zero))
        {
            if (!(sprintInput == 0.0f) && Player.PlayerObject.stamina > staminaCorrectionTolerance)
            {
                Player.PlayerObject.Sprint(sprintStaminaDrainRate, sprintSpeed);
                currentFov = defaultFov + sprintFov;
            }
            else
            {
                currentFov = defaultFov;
            }

            if (Player.PlayerObject.stamina < staminaCorrectionTolerance)
                Player.PlayerObject.playerSpeed = lowStaminaSpeed;

            Player.PlayerObject.MovePlayer(moveStaminaDrainRate, movementInput);
        }
        
        if (movementInputRaw == Vector2.zero)
        {
            if (Player.PlayerObject.stamina < 1f)
                Player.PlayerObject.stamina += Time.deltaTime * staminaRegenRate;
            currentFov = defaultFov;
        }

        Player.PlayerObject.mainCamera.fieldOfView = Mathf.Lerp(Player.PlayerObject.mainCamera.fieldOfView, currentFov, fovSpeed);
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
        if (Player.PlayerObject.jumpCounter < maxJumps && value.performed)
        {
            if (Player.PlayerObject.stamina > jumpStaminaLoss + staminaCorrectionTolerance)
            {
                Player.PlayerObject.Jump(jumpForce, jumpStaminaLoss);
            }
            else
            {
                Player.PlayerObject.Jump(lowStaminaJumpForce, 0);
            }
        }
    }
}