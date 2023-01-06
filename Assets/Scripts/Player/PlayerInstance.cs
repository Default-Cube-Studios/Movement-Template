using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class PlayerInstance : MonoBehaviour
{
    #region Variable Initials
    [Tooltip("This player instance")] public Player ThisPlayer;
    public bool _selectedOnAwake = true;

    private bool setUp;
    #endregion

    void Awake() => SetupPlayer();

    void SetupPlayer()
    {
        if (PlayerManager.setUp && setUp)
            return;

        ThisPlayer = new(gameObject);
        Player.InitializePlayer(ThisPlayer);
        if (_selectedOnAwake)
            Player.SelectPlayer(ThisPlayer.playerManagerIndex);
        setUp = true;
    }
    public void Destroy() => Destroy(ThisPlayer.gameObject);
    public void Destroy(float delay) => Destroy(ThisPlayer.gameObject, delay);

    private void OnEnable() => PlayerManager.SetUp += SetupPlayer;
    private void OnDisable()
    {
        PlayerManager.SetUp -= SetupPlayer;

        //if (!ThisPlayer.destroyed && setUp)
        //    Player.DestroyPlayer(ThisPlayer);
    }
}

[Serializable]
public class Player
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
    public bool isAlive = true;
    public bool isGrounded = true;
    public bool isIdle = true;
    public bool isJumping = false;
    public bool canJump = true;
    [Header("Components")]
    public GameObject gameObject;
    public Transform transform;
    public Rigidbody rigidBody;
    public Camera mainCamera;
    //[Header("Scripts")]
    //public Movement MovementScript;
    //public Collisions CollisionScript;
    [Tooltip("The event called when the player dies or revives")] public static event Action<bool> OnStatus;
    [HideInInspector] public bool destroyed;
    [Header("Items")]
    public List<PowerUpType> activePowerUps = new();
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
    public void Heal(float gainAmount)
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

        //gameObject.GetComponent<Movement>().enabled = false;
        //gameObject.GetComponentInChildren<Collisions>().enabled = false;
        gameObject.GetComponent<PlayerInput>().enabled = false;
        transform.localRotation = Quaternion.Euler(1.0f, transform.localRotation.y, 0.0f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        OnStatus?.Invoke(false);
    }
    public void Respawn()
    {
        Player.ActivePlayer.rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Player.ActivePlayer.health = 1.0f;
        Player.ActivePlayer.stamina = 1.0f;

        //Player.ActivePlayer.gameObject.GetComponent<Movement>().enabled = true;
        //Player.ActivePlayer.gameObject.GetComponentInChildren<Collisions>().enabled = true;
        Player.ActivePlayer.gameObject.GetComponent<PlayerInput>().enabled = true;
        Player.ActivePlayer.gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        OnStatus?.Invoke(true);
    }
    #endregion

    #region Player Manager
    [Tooltip("The currently active player")] public static Player ActivePlayer;
    [Tooltip("A list of all selectable players")] public static List<Player> InitializedPlayers = new();

    public static event Action<Player> PlayerInitialized;
    public static event Action<Player> PlayerSelected;
    public static event Action<Player> PlayerDestroyed;

    public static int InitializePlayer(Player player)
    {
        if (InitializedPlayers.Count > 0 && InitializedPlayers.Contains(player))
            return InitializedPlayers.IndexOf(player);

        InitializedPlayers.Add(player);
        player.playerManagerIndex = InitializedPlayers.Count - 1;
        PlayerInitialized?.Invoke(player);
        return InitializedPlayers.Count - 1;
    }

    public static void SelectPlayer(Player player)
    {
        if (InitializedPlayers.Count - 1 <= 0)
            PlayerManager.CreatePlayer(Vector3.zero);

        foreach (Player thisPlayer in InitializedPlayers)
        {
            foreach (MonoBehaviour script in thisPlayer.gameObject.GetComponents<MonoBehaviour>())
                script.enabled = false;

            thisPlayer.gameObject.GetComponent<PlayerInstance>().enabled = true;
        }

        if (!InitializedPlayers.Contains(player))
            InitializePlayer(player);

        if (ActivePlayer == player)
            return;

        ActivePlayer = player;

        foreach (MonoBehaviour script in player.gameObject.GetComponents<MonoBehaviour>())
            script.enabled = true;

        PlayerSelected?.Invoke(player);
    }
    public static void SelectPlayer(int index)
    {
        if (InitializedPlayers.Count - 1 <= 0)
            PlayerManager.CreatePlayer(Vector3.zero);

        foreach (Player thisPlayer in InitializedPlayers)
        {
            foreach (MonoBehaviour script in thisPlayer.gameObject.GetComponents<MonoBehaviour>())
                script.enabled = false;

            thisPlayer.gameObject.GetComponent<PlayerInstance>().enabled = true;
        }
        if (ActivePlayer == InitializedPlayers.ElementAt(index))
            return;

        ActivePlayer = InitializedPlayers.ElementAt(index);
        foreach (MonoBehaviour script in InitializedPlayers.ElementAt(index).gameObject.GetComponents<MonoBehaviour>())
            script.enabled = true;

        Debug.Log(ActivePlayer.health);
        PlayerSelected?.Invoke(InitializedPlayers.ElementAt(index));
    }
    public static void DestroyPlayer(Player player)
    {
        if (InitializedPlayers.Count - 1 <= 0)
            PlayerManager.CreatePlayer(Vector3.zero);

        player.gameObject.GetComponent<PlayerInstance>().Destroy();
        InitializedPlayers.Remove(player);
        SelectPlayer(InitializedPlayers.Count - 1);
        player.destroyed = true;
        PlayerDestroyed?.Invoke(player);
    }
    public static void DestroyPlayer(int index)
    {
        if (InitializedPlayers.Count - 1 <= 0)
            PlayerManager.CreatePlayer(Vector3.zero);

        InitializedPlayers.ElementAt(index).gameObject.GetComponent<PlayerInstance>().Destroy();
        InitializedPlayers.RemoveAt(index);
        SelectPlayer(InitializedPlayers.Count - 1);
        InitializedPlayers.ElementAt(index).destroyed = true;
        PlayerDestroyed?.Invoke(InitializedPlayers.ElementAt(index));
    }
    #endregion

    public Player(GameObject playerGameObject)
    {
        gameObject = playerGameObject;
        transform = playerGameObject.transform;
        rigidBody = playerGameObject.GetComponent<Rigidbody>();
        mainCamera = playerGameObject.GetComponentInChildren<Camera>();
        //MovementScript = playerGameObject.GetComponent<Movement>();
        //CollisionScript = playerGameObject.GetComponentInChildren<Collisions>();
    }
}