using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInstance))]
[DisallowMultipleComponent]
public class Movement : MonoBehaviour
{
    #region Variable Initials
    public Player Player { get; private set; }
    [Header("Movement")]
    [Tooltip("The player speed while low on stamina")] public float _lowStaminaSpeed;
    public float _walkSpeed;
    [Tooltip("The player speed while sprinting")] public float _sprintSpeed;
    [Tooltip("The speed at which player movement is smoothed (Larger numbers decrease smoothness)")] public float _inputSmoothing;

    [Header("Field of View")]
    [SerializeField][Tooltip("The speed of the transition between FOVs")][Range(0f, 1f)] private float _fovSpeed;
    [SerializeField] private float _defaultFov;
    [SerializeField][Tooltip("The FOV added onto the defaultFov attribute while sprinting")] private float _sprintFov;
    [SerializeField] private float _currentFov;

    [Header("Jump")]
    public bool _infiniteJump;
    public float _jumpForce;
    public float _lowStaminaJumpForce;
    [Tooltip("The maximum number of jumps the player can do mid-air")] public int _maxJumps;

    [Header("Stamina")]
    [Range(0.0f, 1.0f)] public float _moveStaminaDrainRate;
    [Range(0.0f, 1.0f)] public float _sprintStaminaDrainRate;
    [Range(0.0f, 1.0f)] public float _jumpStaminaLoss;
    [Range(0.0f, 1.0f)] public float _staminaRegenRate;

    [Header("Camera")]
    [SerializeField][Tooltip("The speed at which mouse movement is smoothed (Larger numbers decrease smoothness)")] private float _mouseSmoothing;

    [Header("Damage")]
    [Range(0.0f, 1.0f)] public float _damageRegenRate;
    [Tooltip("The minimum fall force before the player takes damage")] public float _minFallForce;
    [Tooltip("The largest amount of fall damage before the player dies")] public float _maxFallForce;
    public float _forceExponent;

    private Vector2 movementInput, movementInputRaw = Vector2.zero;
    private Vector2 cameraInput, cameraInputRaw = Vector2.zero;
    private float rotationX, rotationY;
    private float sprintInput;
    #endregion

    #region Unity Events
    void Awake() => Player = GetComponent<PlayerInstance>().ThisPlayer;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        movementInput = Vector2.Lerp(movementInput, movementInputRaw, Time.deltaTime * _inputSmoothing);
        cameraInput = Vector2.Lerp(cameraInput, cameraInputRaw, Time.deltaTime * _mouseSmoothing);
        rotationX -= cameraInput.y * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, -90.0f, 90.0f);
        rotationY += cameraInput.x * Time.deltaTime;
        Player.mainCamera.transform.localRotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
        Player.rigidBody.MoveRotation(Quaternion.Euler(0.0f, rotationY, 0.0f));

        if (!(movementInput == Vector2.zero))
            Player.rigidBody.MovePosition(transform.position + transform.TransformDirection(new Vector3(movementInput.x * Time.deltaTime * Player.speed, 0, movementInput.y * Time.deltaTime * Player.speed)));

        if (movementInputRaw == Vector2.zero)
        {
            Player.isIdle = true;
            _currentFov = _defaultFov;

            if (!Player.isJumping && Player.isGrounded)
            {
                Player.RegenStamina(Time.deltaTime * _staminaRegenRate);
                Player.Heal(Time.deltaTime * _damageRegenRate);
            }
        }
        else
        {
            Player.isIdle = false;
            Player.speed = _walkSpeed;
            _currentFov = _defaultFov;

            if (!(sprintInput == 0.0f) && Player.stamina > 0.0f)
                Sprint();
            else if (Player.stamina <= 0.0f)
                Player.speed = _lowStaminaSpeed;
            else
                Player.DrainStamina(Time.deltaTime * _moveStaminaDrainRate);
        }

        Player.mainCamera.fieldOfView = Mathf.Lerp(Player.mainCamera.fieldOfView, _currentFov, _fovSpeed);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.y > _minFallForce)
            Player.Damage(Mathf.Pow(collision.relativeVelocity.y, _forceExponent) / Mathf.Pow(_maxFallForce, _forceExponent));
    }
    #endregion

    #region Input System
    public void OnMove(InputAction.CallbackContext value) => movementInputRaw = value.ReadValue<Vector2>();
    public void OnSprint(InputAction.CallbackContext value) => sprintInput = value.ReadValue<float>();
    public void OnLook(InputAction.CallbackContext value) => cameraInputRaw = value.ReadValue<Vector2>();
    public void OnJump(InputAction.CallbackContext value)
    {
        if (Player.jumpCounter >= _maxJumps)
            Player.canJump = false;

        Player.isJumping = value.started;

        if ((Player.canJump || _infiniteJump) && value.started)
        {
            if (Player.stamina > _jumpStaminaLoss)
                Jump(_jumpForce, _jumpStaminaLoss);
            else
                Jump(_lowStaminaJumpForce, 1.0f);
        }
    }
    #endregion

    void Sprint()
    {
        Player.DrainStamina(Time.deltaTime * _sprintStaminaDrainRate);
        Player.speed = _sprintSpeed;
        _currentFov = _defaultFov + _sprintFov;
    }
    void Jump(float jumpForce, float staminaLoss)
    {
        Player.rigidBody.AddForce(Vector3.up * jumpForce);
        Player.DrainStamina(staminaLoss);
        Player.jumpCounter++;
    }
}