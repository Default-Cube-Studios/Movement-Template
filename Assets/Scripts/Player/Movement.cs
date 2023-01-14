using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInstance))]
[AddComponentMenu("Player/Movement")]
[DisallowMultipleComponent]
public class Movement : MonoBehaviour
{
    #region Variable Initials
    private Player ThisPlayer;
    [Header("Movement")]
    [Tooltip("The player speed while low on stamina")] public float _lowStaminaSpeed;
    public float _walkSpeed;
    [Tooltip("The player speed while sprinting")] public float _sprintSpeed;
    [SerializeField] private float _accelerationRate;
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
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        Look();
        movementInput = Vector2.Lerp(movementInput, movementInputRaw, Time.deltaTime * _inputSmoothing);

        if (movementInput != Vector2.zero)
            Move();

        if (movementInputRaw == Vector2.zero)
        {
            ThisPlayer.isIdle = true;
            _currentFov = _defaultFov;

            if (!ThisPlayer.isJumping && ThisPlayer.isGrounded)
            {
                ThisPlayer.RegenStamina(Time.deltaTime * _staminaRegenRate);
                ThisPlayer.Heal(Time.deltaTime * _damageRegenRate);
            }
        }
        else
        {
            ThisPlayer.isIdle = false;
            ThisPlayer.speed = _walkSpeed;
            _currentFov = _defaultFov;

            if (!(sprintInput == 0.0f) && ThisPlayer.stamina > 0.0f)
                Sprint();
            else if (ThisPlayer.stamina <= 0.0f)
                ThisPlayer.speed = _lowStaminaSpeed;
            else
                ThisPlayer.DrainStamina(Time.deltaTime * _moveStaminaDrainRate);
        }

        ThisPlayer.mainCamera.fieldOfView = Mathf.Lerp(ThisPlayer.mainCamera.fieldOfView, _currentFov, _fovSpeed);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.y > _minFallForce)
            ThisPlayer.Damage(Mathf.Pow(collision.relativeVelocity.y, _forceExponent) / Mathf.Pow(_maxFallForce, _forceExponent));
    }
    private void OnEnable() => Player.PlayerSelected += SetPlayer;
    private void OnDisable() => Player.PlayerSelected -= SetPlayer;
    #endregion

    #region Input System
    public void OnMove(InputAction.CallbackContext value) => movementInputRaw = value.ReadValue<Vector2>();
    public void OnSprint(InputAction.CallbackContext value) => sprintInput = value.ReadValue<float>();
    public void OnLook(InputAction.CallbackContext value) => cameraInputRaw = value.ReadValue<Vector2>();
    public void OnJump(InputAction.CallbackContext value)
    {
        if (ThisPlayer.jumpCounter >= _maxJumps)
            ThisPlayer.canJump = false;

        ThisPlayer.isJumping = value.started;

        if ((ThisPlayer.canJump || _infiniteJump) && value.started)
        {
            if (ThisPlayer.stamina > _jumpStaminaLoss)
                Jump(_jumpForce, _jumpStaminaLoss);
            else
                Jump(_lowStaminaJumpForce, 1.0f);
        }
    }
    #endregion

    #region Movement
    void Look()
    {
        cameraInput = Vector2.Lerp(cameraInput, cameraInputRaw, Time.deltaTime * _mouseSmoothing);
        rotationX -= cameraInput.y * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, -90.0f, 90.0f);
        rotationY += cameraInput.x * Time.deltaTime;
        ThisPlayer.mainCamera.transform.localRotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
        ThisPlayer.rigidBody.MoveRotation(Quaternion.Euler(0.0f, rotationY, 0.0f));
    }
    void Move()
    {
        ThisPlayer.rigidBody.MovePosition(transform.position + transform.TransformDirection(new Vector3(movementInput.x * Time.deltaTime * ThisPlayer.speed, 0, movementInput.y * Time.deltaTime * ThisPlayer.speed)));
    }
    void Sprint()
    {
        ThisPlayer.DrainStamina(Time.deltaTime * _sprintStaminaDrainRate);
        ThisPlayer.speed = _sprintSpeed;
        _currentFov = _defaultFov + _sprintFov;
    }
    void Jump(float jumpForce, float staminaLoss)
    {
        ThisPlayer.rigidBody.AddForce(Vector3.up * jumpForce);
        ThisPlayer.DrainStamina(staminaLoss);
        ThisPlayer.jumpCounter++;
    }
    #endregion

    void SetPlayer(Player player)
    {
        if (player != GetComponent<PlayerInstance>().ThisPlayer)
            return;

        ThisPlayer = player;
    }
}