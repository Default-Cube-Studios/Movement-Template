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
    [SerializeField][Range(1.0f, 50.0f)][Tooltip("The speed at which the player accelerates")] private float _acceleration;
    [SerializeField][Range(0.0f, 5.0f)][Tooltip("The time it takes for the player to stop")] private float _deceleration;
    [Range(0.0f, 1.0f)][Tooltip("The speed at which player movement is smoothed (Larger numbers decrease smoothness)")] public float _inputSmoothing;

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

    private Vector2 movementInput = Vector2.zero;
    private Vector2 cameraInput, cameraInputRaw = Vector2.zero;
    private float rotationX, rotationY;
    private float sprintInput;
    private Vector3 stopVelocity;
    private Vector2 targetSpeed, targetAcceleration, speed, speedLastFrame = Vector2.zero;
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

        if (movementInput == Vector2.zero)
        {
            ThisPlayer.isIdle = true;
            _currentFov = _defaultFov;

            if (!ThisPlayer.isJumping && ThisPlayer.isGrounded)
            {
                ThisPlayer.RegenStamina(Time.deltaTime * _staminaRegenRate);
                ThisPlayer.Heal(Time.deltaTime * _damageRegenRate);
            }

            Stop();
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

            Move();
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
    public void OnMove(InputAction.CallbackContext value) => movementInput = value.ReadValue<Vector2>();
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
        speedLastFrame = speed;
        Vector3 localVelocity = transform.InverseTransformDirection(ThisPlayer.rigidBody.velocity);
        targetSpeed = new(ThisPlayer.speed * movementInput.x, ThisPlayer.speed * movementInput.y);
        targetAcceleration = new(_acceleration * movementInput.x, _acceleration * movementInput.y);
        speed = Vector2.zero;

        //if (localVelocity.x > ThisPlayer.speed)
        //    speed.x -= localVelocity.x - ThisPlayer.speed;
        //else if (localVelocity.x < -ThisPlayer.speed)
        //    speed.x += localVelocity.x + ThisPlayer.speed;
        //else
        //    speed.x = _acceleration;

        //if (localVelocity.z > ThisPlayer.speed)
        //    speed.y -= localVelocity.z - ThisPlayer.speed;
        //else if (localVelocity.z < -ThisPlayer.speed)
        //    speed.y += localVelocity.z + ThisPlayer.speed;
        //else
        //    speed.y = _acceleration;

        //targetAcceleration.x = movementInput.x < 0.0f ? -targetAcceleration.x : targetAcceleration.x;
        //targetAcceleration.y = movementInput.y < 0.0f ? -targetAcceleration.y : targetAcceleration.y;

        if (Mathf.Abs(localVelocity.z + (targetAcceleration.y * Time.deltaTime)) >= targetSpeed.y)
            speed.y = (targetSpeed.y - localVelocity.z) / Time.deltaTime;
        else
            speed.y = targetAcceleration.y;

        if (Mathf.Abs(localVelocity.x + (targetAcceleration.x * Time.deltaTime)) >= targetSpeed.x)
            speed.x = (targetSpeed.x - localVelocity.x) / Time.deltaTime;
        else
            speed.x = targetAcceleration.x;

        speed = Vector2.Lerp(speedLastFrame, speed, _inputSmoothing);

        Debug.Log(localVelocity);

        ThisPlayer.rigidBody.velocity += transform.TransformDirection(new Vector3(
            speed.x * Time.deltaTime,
            0.0f,
            speed.y * Time.deltaTime));
    }

    void Stop() => ThisPlayer.rigidBody.velocity = Vector3.SmoothDamp(
                        ThisPlayer.rigidBody.velocity,
                        new Vector3(0.0f, ThisPlayer.rigidBody.velocity.y, 0.0f),
                        ref stopVelocity, _deceleration);
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