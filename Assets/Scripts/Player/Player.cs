using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    #region Variable Initials
    [Tooltip("This player instance")] public static PlayerClass ThisPlayer;
    [SerializeField][Tooltip("Inspector-only values to debug the player")] private PlayerClass DebugPlayer;
    public bool selectedOnAwake;
    private bool hasBeenSetup = false;
    #endregion

    void Awake() => SetupPlayer();

    void SetupPlayer()
    {
        if (hasBeenSetup || !PlayerManager.Instance.hasLoaded)
            return;

        ThisPlayer = new(gameObject);
        PlayerManager.Instance.InitializePlayer(ThisPlayer);

        if (selectedOnAwake)
            PlayerManager.Instance.SelectPlayer(ThisPlayer.playerManagerIndex);

        DebugPlayer = ThisPlayer;
        hasBeenSetup = true;
    }
    public void Destroy() => Destroy(ThisPlayer.gameObject);
    public void Destroy(float delay) => Destroy(ThisPlayer.gameObject, delay);

    void OnEnable() => PlayerManager.HasLoaded += SetupPlayer;
    void OnDisable() => PlayerManager.HasLoaded -= SetupPlayer;

    #region Actions
    public void Heal(float amount) => ThisPlayer.Heal(amount);
    public void Damage(float amount) => ThisPlayer.Damage(amount);
    public void DrainStamina(float amount) => ThisPlayer.DrainStamina(amount);
    public void RegenStamina(float amount) => ThisPlayer.RegenStamina(amount);
    public void SetState(bool state) => ThisPlayer.isPlayerAlive = state;
    public void Kill() => ThisPlayer.Kill();
    public void Respawn() => ThisPlayer.Respawn();
    #endregion
}

[Serializable]
public class PlayerClass
{
    #region Variable Initials
    [Tooltip("The index of this player in the Player Manager")] public int playerManagerIndex;
    [Header("Stats")]
    public float stamina = 1.0f;
    public float health = 1.0f;
    [Header("Movement")]
    public float playerSpeed;
    public int jumpCounter;
    [Header("Functions")]
    public bool isPlayerAlive = true;
    public bool isPlayerOnGround = true;
    public bool isPlayerMoving = false;
    public bool isPlayerJumping = false;
    [Header("Components")]
    public GameObject gameObject;
    public Transform transform;
    public Rigidbody rigidBody;
    public Camera mainCamera;
    [Tooltip("The event called when the player's stamina changes")] public static event Action<float> OnStamina;
    [Tooltip("The event called when the player's health changes")] public static event Action<float> OnHealth;
    [Tooltip("The event called when the player dies or revives")] public static event Action<bool> OnStatus;
    #endregion

    #region Stamina
    public void DrainStamina(float staminaLoss)
    {
        if (stamina > staminaLoss)
            stamina -= staminaLoss;
        else
            stamina = 0.0f;

        OnStamina?.Invoke(stamina);
    }
    public void RegenStamina(float staminaGain)
    {
        if (stamina + staminaGain < 1.0f)
            stamina += staminaGain;
        else
            stamina = 1.0f;

        OnStamina?.Invoke(stamina);
    }
    #endregion

    #region Health
    public void Damage(float damageAmount)
    {
        if (health > damageAmount)
            health -= damageAmount;
        else
            Kill();

        OnHealth?.Invoke(health);
    }
    public void Heal(float gainAmount)
    {
        if (health + gainAmount < 1.0f)
            health += gainAmount;
        else
            health = 1.0f;

        OnHealth?.Invoke(health);
    }
    public void Kill()
    {
        rigidBody.constraints = RigidbodyConstraints.None;

        health = 0.0f;
        stamina = 0.0f;

        gameObject.GetComponent<PlayerMovement>().enabled = false;
        gameObject.GetComponent<PlayerDamage>().enabled = false;
        gameObject.GetComponent<PlayerInput>().enabled = false;
        transform.localRotation = Quaternion.Euler(1.0f, transform.localRotation.y, 0.0f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        OnStatus?.Invoke(false);
        OnHealth?.Invoke(health);
        OnStamina?.Invoke(stamina);
    }
    public void Respawn()
    {
        Players.ActivePlayer.rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Players.ActivePlayer.health = 1.0f;
        Players.ActivePlayer.stamina = 1.0f;

        Players.ActivePlayer.gameObject.GetComponent<PlayerMovement>().enabled = true;
        Players.ActivePlayer.gameObject.GetComponent<PlayerDamage>().enabled = true;
        Players.ActivePlayer.gameObject.GetComponent<PlayerInput>().enabled = true;
        Players.ActivePlayer.gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        OnStatus?.Invoke(true);
        OnHealth?.Invoke(health);
        OnStamina?.Invoke(stamina);
    }
    #endregion

    public PlayerClass(GameObject playerGameObject)
    {
        gameObject = playerGameObject;
        transform = playerGameObject.transform;
        rigidBody = playerGameObject.GetComponent<Rigidbody>();
        mainCamera = playerGameObject.GetComponentInChildren<Camera>();
    }
}