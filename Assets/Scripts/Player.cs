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
    public float stamina = 1f;
    public float playerSpeed;
    public int jumpCounter;
    public Rigidbody rigidBody;
    public GameObject gameObject;
    public Camera mainCamera;

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