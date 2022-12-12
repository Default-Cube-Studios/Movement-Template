using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variable Initials
    [Header("Movement")]
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
    [Tooltip("The distance of the ray used to identify if the player is on the ground (keep at player's Y scale)")] public float raycastDistance;
    [SerializeField] float jumpForce;
    [SerializeField] float lowStaminaJumpForce;
    [Tooltip("The maximum number of jumps the player can do mid-air")][SerializeField] int maxJumps;

    [Header("Stamina")]
    [SerializeField]
    [Range(0f, 1f)] private float sprintStaminaDrainRate;
    [SerializeField][Range(0.0f, 1.0f)] private float moveStaminaDrainRate;
    [SerializeField][Range(0.0f, 1.0f)] private float staminaRegenRate;
    [SerializeField][Range(0.0f, 1.0f)] private float jumpStaminaLoss;

    private Vector2 movementInput = Vector2.zero;
    private Vector2 movementInputRaw = Vector2.zero;
    private float sprintInput;
    #endregion

    public void Update()
    {
        movementInput = Vector2.Lerp(movementInput, movementInputRaw, Time.deltaTime * inputSmoothing);
        Player.PlayerObject.playerSpeed = walkSpeed;

        if (!(movementInput == Vector2.zero))
        {
            float staminaDrain = moveStaminaDrainRate;

            if (!(sprintInput == 0.0f) && Player.PlayerObject.stamina > 0.0f)
            {
                staminaDrain += Time.deltaTime * sprintStaminaDrainRate;
                Player.PlayerObject.playerSpeed = sprintSpeed;
                currentFov = defaultFov + sprintFov;
            }
            else
            {
                currentFov = defaultFov;
            }

            if (Player.PlayerObject.stamina < 0.0f)
                Player.PlayerObject.playerSpeed = lowStaminaSpeed;

            Player.PlayerObject.Move(staminaDrain, movementInput);
        }
        

        if (movementInputRaw == Vector2.zero)
        {
            if (Player.PlayerObject.stamina < 1.0f)
                Player.PlayerObject.stamina += Time.deltaTime * staminaRegenRate;
            currentFov = defaultFov;
            Player.PlayerObject.isPlayerMoving = false;
        }
        else
            Player.PlayerObject.isPlayerMoving = true;


        Player.PlayerObject.mainCamera.fieldOfView = Mathf.Lerp(Player.PlayerObject.mainCamera.fieldOfView, currentFov, fovSpeed);
    }
    public void FixedUpdate()
    {
        if (Physics.Raycast(gameObject.transform.position, Vector3.down, raycastDistance))
        {
            Player.PlayerObject.isPlayerOnGround = true;
            Player.PlayerObject.jumpCounter = 0;
        }
        else
            Player.PlayerObject.isPlayerOnGround = false;
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
            Player.PlayerObject.isPlayerJumping = true;
            if (Player.PlayerObject.stamina > jumpStaminaLoss)
            {
                Player.PlayerObject.Jump(jumpForce, jumpStaminaLoss);
            }
            else
            {
                Player.PlayerObject.Jump(lowStaminaJumpForce, 0.0f);
            }
        }
        else
            Player.PlayerObject.isPlayerJumping = false;
    }
}