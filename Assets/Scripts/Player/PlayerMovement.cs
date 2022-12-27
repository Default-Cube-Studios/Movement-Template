using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerMovement : MonoBehaviour
{
    #region Variable Initials
    [Header("Movement")]
    public float walkSpeed;
    [Tooltip("The player speed while low on stamina")] public float lowStaminaSpeed;
    [Tooltip("The player speed while sprinting")] public float sprintSpeed;
    [Tooltip("The speed at which player movement is smoothed (Larger numbers decrease smoothness)")] public float inputSmoothing;

    [Header("Field of View")]
    [Tooltip("The speed of the transition between FOVs")][SerializeField][Range(0f, 1f)] float fovSpeed;
    [SerializeField] float defaultFov;
    [Tooltip("The FOV added onto the defaultFov attribute while sprinting")][SerializeField] float sprintFov;
    [SerializeField] float currentFov;

    [Header("Jump")]
    [Tooltip("A set of tags attatched to platforms and ground objects")][SerializeField] string[] groundTags;
    public float jumpForce;
    public float lowStaminaJumpForce;
    public bool infiniteJump;
    [Tooltip("The maximum number of jumps the player can do mid-air")] public int maxJumps;

    [Header("Stamina")]
    [SerializeField]
    [Range(0f, 1f)] private float sprintStaminaDrainRate;
    [SerializeField][Range(0.0f, 1.0f)] private float moveStaminaDrainRate;
    [SerializeField][Range(0.0f, 1.0f)] private float staminaRegenRate;
    [SerializeField][Range(0.0f, 1.0f)] private float jumpStaminaLoss;

    [Header("Camera")]
    [Tooltip("The speed at which mouse movement is smoothed (Larger numbers decrease smoothness)")][SerializeField] float mouseSmoothing;

    private Vector2 movementInput = Vector2.zero;
    private Vector2 movementInputRaw = Vector2.zero;
    private float rotationX, rotationY;
    private Vector2 cameraInput = Vector2.zero;
    private Vector2 cameraInputRaw = Vector2.zero;
    private float sprintInput;
    private HashSet<GameObject> touchingGameObjects = new();
    #endregion

    #region Unity Events
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        movementInput = Vector2.Lerp(movementInput, movementInputRaw, Time.deltaTime * inputSmoothing);
        Players.ActivePlayer.playerSpeed = walkSpeed;

        if (!(movementInput == Vector2.zero))
        {
            float staminaDrain = moveStaminaDrainRate;

            if (!(sprintInput == 0.0f) && Players.ActivePlayer.stamina > 0.0f)
            {
                staminaDrain += Time.deltaTime * sprintStaminaDrainRate;
                Players.ActivePlayer.playerSpeed = sprintSpeed;
                currentFov = defaultFov + sprintFov;
            }
            else
                currentFov = defaultFov;

            if (Players.ActivePlayer.stamina <= 0.0f)
                Players.ActivePlayer.playerSpeed = lowStaminaSpeed;
            else
                Players.ActivePlayer.DrainStamina(Time.deltaTime * staminaDrain);

            Players.ActivePlayer.rigidBody.MovePosition(transform.position + transform.TransformDirection(new Vector3(movementInput.x * Time.deltaTime * Players.ActivePlayer.playerSpeed, 0, movementInput.y * Time.deltaTime * Players.ActivePlayer.playerSpeed)));
        }

        if (movementInputRaw == Vector2.zero)
        {
            if (!Players.ActivePlayer.isPlayerMoving && !Players.ActivePlayer.isPlayerJumping && Players.ActivePlayer.isPlayerOnGround && Players.ActivePlayer.stamina < 1.0f)
                Players.ActivePlayer.RegenStamina(Time.deltaTime * staminaRegenRate);
            currentFov = defaultFov;
            Players.ActivePlayer.isPlayerMoving = false;
        }
        else
            Players.ActivePlayer.isPlayerMoving = true;


        Players.ActivePlayer.mainCamera.fieldOfView = Mathf.Lerp(Players.ActivePlayer.mainCamera.fieldOfView, currentFov, fovSpeed);


        cameraInput = Vector2.Lerp(cameraInput, cameraInputRaw, Time.deltaTime * mouseSmoothing);

        rotationX -= cameraInput.y * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, -90.0f, 90.0f);
        rotationY += cameraInput.x * Time.deltaTime;

        Players.ActivePlayer.mainCamera.transform.localRotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
        Players.ActivePlayer.rigidBody.MoveRotation(Quaternion.Euler(0.0f, rotationY, 0.0f));
    }
    #endregion

    #region Collision Detection
    public void OnCollisionEnter(Collision collision)
    {
        touchingGameObjects.Add(collision.gameObject);
        Players.ActivePlayer.isPlayerOnGround = GroundCheck();
    }
    public void OnCollisionExit(Collision collision)
    {
        touchingGameObjects.Remove(collision.gameObject);
        Players.ActivePlayer.isPlayerOnGround = GroundCheck();
    }
    #endregion

    #region Input System
    public void OnMove(InputAction.CallbackContext value) => movementInputRaw = value.ReadValue<Vector2>();
    public void OnSprint(InputAction.CallbackContext value) => sprintInput = value.ReadValue<float>();
    public void OnLook(InputAction.CallbackContext value) => cameraInputRaw = value.ReadValue<Vector2>();
    public void OnJump(InputAction.CallbackContext value)
    {
        if ((Players.ActivePlayer.jumpCounter < maxJumps || infiniteJump) && value.started)
        {
            Players.ActivePlayer.isPlayerJumping = true;

            if (Players.ActivePlayer.stamina > jumpStaminaLoss)
                Jump(jumpForce, jumpStaminaLoss);
            else
                Jump(lowStaminaJumpForce, 0.0f);
        }
        else
            Players.ActivePlayer.isPlayerJumping = false;
    }
    #endregion

    bool GroundCheck()
    {
        foreach (GameObject currentObject in touchingGameObjects)
        {
            foreach (string tag in groundTags)
            {
                if (currentObject.CompareTag(tag))
                {
                    Players.ActivePlayer.jumpCounter = 0;
                    return true;
                }
            }
        }
        return false;
    }

    void Jump(float jumpForce, float staminaLoss)
    {
        Players.ActivePlayer.rigidBody.AddForce(Vector3.up * jumpForce);
        Players.ActivePlayer.DrainStamina(staminaLoss);
        Players.ActivePlayer.jumpCounter++;
    }

    #region Actions
    public void AddWalkSpeed(float speed) => walkSpeed += speed;
    public void AddSprintSpeed(float speed) => sprintSpeed += speed;

    public void SetWalkStaminaDrain(float amount) => moveStaminaDrainRate = amount;
    public void SetSprintStaminaDrain(float amount) => sprintStaminaDrainRate = amount;
    public void SetStaminaRegen(float amount) => staminaRegenRate = amount;

    public void AddJumpForce(float force) => jumpForce += force;
    public void SetJumpStaminaLoss(float amount) => jumpStaminaLoss = amount;
    public void SetMaxJumps(int jumps) => maxJumps = jumps;
    public void SetInfiniteJump(bool state) => infiniteJump = state;
    #endregion
}