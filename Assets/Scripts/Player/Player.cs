using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public static PlayerClass PlayerObject = new();
    [SerializeField][Tooltip("These values are read-only and can be used to debug")] private PlayerClass DebugPlayer = PlayerObject;

    public void Awake() => PlayerObject.tag = gameObject.tag;
}

[System.Serializable]
public class PlayerClass
{
    #region Variable Initials
    public string name;
    public string tag;
    public float stamina = 1f;
    public float playerSpeed;
    public int jumpCounter;
    public float health = 1f;
    public bool isPlayerAlive = true;
    public bool isPlayerOnGround = true;
    public bool isPlayerMoving = false;
    public bool isPlayerJumping = false;
    public GameObject gameObject;
    public Transform transform;
    public Rigidbody rigidBody;
    public Camera mainCamera;
    #endregion

    #region Movement
    public void Move(float staminaDrain, Vector2 movementInput)
    {
        if (stamina > 0.0f)
            DrainStamina(Time.deltaTime * staminaDrain);
        rigidBody.MovePosition(transform.position + transform.TransformDirection(new Vector3(movementInput.x * Time.deltaTime * playerSpeed, 0, movementInput.y * Time.deltaTime * playerSpeed)));
    }
    public void Jump(float jumpForce, float staminaLoss)
    {
        rigidBody.AddForce(Vector3.up * jumpForce);
        DrainStamina(staminaLoss);
    }
    #endregion

    #region Stamina
    public void DrainStamina(float staminaLoss)
    {
        if (stamina > staminaLoss)
            stamina -= staminaLoss;
        else
            stamina = 0.0f;
    }
    public void RegenStamina(float staminaGain)
    {
        if (stamina + staminaGain < 1.0f)
            stamina += staminaGain;
        else
            stamina = 1.0f;
    }
    #endregion

    #region Health
    public void Damage(float damageAmount)
    {
        if (health > damageAmount)
            health -= damageAmount;
        else
            Kill();
    }
    public void Repair(float gainAmount)
    {
        if (health + gainAmount < 1.0f)
            health += gainAmount;
        else
            health = 1.0f;
    }
    public void Kill()
    {
        rigidBody.constraints = RigidbodyConstraints.None;

        health = 0.0f;
        stamina = 0.0f;
        isPlayerAlive = false;

        gameObject.GetComponent<PlayerMovement>().enabled = false;
        gameObject.GetComponent<CameraRotation>().enabled = false;
        gameObject.GetComponent<PlayerDamage>().enabled = false;
        gameObject.GetComponent<PlayerInput>().enabled = false;
        rigidBody.MoveRotation(Quaternion.Euler(1, 1, 1));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion
}