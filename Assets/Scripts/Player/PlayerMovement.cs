using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInstance))]
[RequireComponent(typeof(PlayerStats))]
[AddComponentMenu("Player/Movement")]
[DisallowMultipleComponent]
public class PlayerMovement : PlayerScript
{
    #region Variable Initials
    [Header("Dependencies")]
    [SerializeField] PlayerStats stats;
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

    [Header("Jump")]
    public bool _infiniteJump;
    public float _jumpForce;
    public float _lowStaminaJumpForce;
    [Tooltip("The maximum number of jumps the player can do mid-air")] public int _maxJumps;

    [Header("Stamina")]
    [Range(0.0f, 1.0f)] public float _moveStaminaDrainRate;
    [Range(0.0f, 1.0f)] public float _sprintStaminaDrainRate;
    [Range(0.0f, 1.0f)] public float _jumpStaminaLoss;

    [Header("Camera")]
    [SerializeField][Tooltip("The speed at which mouse movement is smoothed (Larger numbers decrease smoothness)")] private float _mouseSmoothing;

    private Vector2 movementInput = Vector2.zero;
    private Vector2 cameraInput, cameraInputRaw = Vector2.zero;
    private float currentFov;
    private float rotationX, rotationY;
    private float sprintInput;
    private Vector3 stopVelocity;
    private Vector2 targetSpeed, targetAcceleration, speed, speedLastFrame = Vector2.zero;
    private float setSpeed;
    [HideInInspector] public int jumpCounter;
    #endregion

    #region Unity Events
    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        Look();
        ThisPlayer.isIdle = movementInput == Vector2.zero;
        currentFov = _defaultFov;

        if (ThisPlayer.isIdle) Stop();
        else
        {
            setSpeed = _walkSpeed;

            if (!(sprintInput == 0.0f) && ThisPlayer.stamina > 0.0f) Sprint();
            else if (ThisPlayer.stamina <= 0.0f) setSpeed = _lowStaminaSpeed;
            
            stats.Tire(Time.deltaTime * _moveStaminaDrainRate);
            Move();
        }

        ThisPlayer.mainCamera.fieldOfView = 
            Mathf.Lerp(ThisPlayer.mainCamera.fieldOfView, currentFov, _fovSpeed);
    }
    #endregion

    #region Input System
    public void OnMove(InputAction.CallbackContext value) => movementInput = value.ReadValue<Vector2>();
    public void OnSprint(InputAction.CallbackContext value) => sprintInput = value.ReadValue<float>();
    public void OnLook(InputAction.CallbackContext value) => cameraInputRaw = value.ReadValue<Vector2>();
    public void OnJump(InputAction.CallbackContext value)
    {
        if (jumpCounter >= _maxJumps)
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
        targetSpeed = new(setSpeed * movementInput.x, setSpeed * movementInput.y);
        targetAcceleration = new(_acceleration * movementInput.x, _acceleration * movementInput.y);
        speed = Vector2.zero;

        if (Mathf.Abs(localVelocity.z + (targetAcceleration.y * Time.deltaTime)) >= targetSpeed.y)
            speed.y = (targetSpeed.y - localVelocity.z) / Time.deltaTime;
        else
            speed.y = targetAcceleration.y;

        if (Mathf.Abs(localVelocity.x + (targetAcceleration.x * Time.deltaTime)) >= targetSpeed.x)
            speed.x = (targetSpeed.x - localVelocity.x) / Time.deltaTime;
        else
            speed.x = targetAcceleration.x;

        Debug.Log(targetAcceleration);
        speed = Vector2.Lerp(speedLastFrame, speed, _inputSmoothing);

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
        stats.Tire(Time.deltaTime * _sprintStaminaDrainRate);
        setSpeed = _sprintSpeed;
        currentFov = _defaultFov + _sprintFov;
    }
    void Jump(float jumpForce, float staminaLoss)
    {
        ThisPlayer.rigidBody.AddForce(Vector3.up * jumpForce);
        stats.Tire(staminaLoss);
        jumpCounter++;
    }
    #endregion
}