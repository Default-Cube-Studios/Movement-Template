using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variable Initials
    [Header("Global Attributes")]
    [Tooltip("The Game Object scripts use to reference the player")] public GameObject globalGameObject;
    [Tooltip("The RigidBody component scripts use to reference the player")] public Rigidbody globalRigidBody;
    [Tooltip("The camera scripts use in relation with the player")] public Camera globalCamera;
    #endregion

    public static PlayerClass PlayerObject = new();
    [Header("Object")]
    [SerializeField][Tooltip("These values are read-only and can be used to debug")] private PlayerClass DebugPlayer = PlayerObject;

    public void Awake()
    {
        PlayerObject.rigidBody = globalRigidBody;
        PlayerObject.gameObject = globalGameObject;
        PlayerObject.mainCamera = globalCamera;
    }
}

[System.Serializable]
public class PlayerClass
{
    #region Variable Initials
    public float stamina = 1f;
    public float playerSpeed;
    public int jumpCounter;
    public float health = 1f;
    public bool isPlayerAlive = true;
    public bool isPlayerOnGround = true;
    public bool isPlayerMoving = false;
    public bool isPlayerJumping = false;
    public Rigidbody rigidBody;
    public GameObject gameObject;
    public Camera mainCamera;
    #endregion

    public void Move(float staminaDrain, Vector2 movementInput)
    {
        if (stamina > 0.0f)
            stamina -= Time.deltaTime * staminaDrain;
        rigidBody.MovePosition(gameObject.transform.position + gameObject.transform.TransformDirection(new Vector3(movementInput.x * Time.deltaTime * playerSpeed, 0, movementInput.y * Time.deltaTime * playerSpeed)));
    }
    public void Jump(float jumpForce, float staminaLoss)
    {
        rigidBody.AddForce(Vector3.up * jumpForce);
        stamina -= staminaLoss;
        jumpCounter++;
    }
    public void Damage(float damageAmount)
    {
        if (health > damageAmount)
            health -= damageAmount;
        else
            Kill();
    }
    public void Kill()
    {
        health = 0.0f;
        stamina = 0.0f;
        isPlayerAlive = false;

        gameObject.GetComponent<PlayerMovement>().enabled = false;
        gameObject.GetComponent<CameraRotation>().enabled = false;
        gameObject.GetComponent<PlayerDamage>().enabled = false;
        rigidBody.AddForce(new Vector3(15f, 15f, 15f));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}