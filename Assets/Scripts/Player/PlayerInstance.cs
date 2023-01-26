using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Player/Player")]
[DisallowMultipleComponent]
public class PlayerInstance : MonoBehaviour
{
    #region Variable Initials
    [Tooltip("This player instance")] public Player ThisPlayer;
    public bool _selectedOnAwake;

    public GameObject _playerPrefab;
    public static GameObject playerGameObject;
    #endregion

    void Awake()
    {
        playerGameObject = _playerPrefab;

        ThisPlayer = new(gameObject);
        Player.InitializePlayer(ThisPlayer);
        if (_selectedOnAwake)
            Player.SelectPlayer(ThisPlayer.playerManagerIndex);
    }

    public void Destroy() => Destroy(ThisPlayer.gameObject);
    public void Destroy(float delay) => Destroy(ThisPlayer.gameObject, delay);

    public static Player CreatePlayer(Vector3 position) => Instantiate(playerGameObject, position, Quaternion.identity).GetComponent<PlayerInstance>().ThisPlayer;

    private void OnDisable()
    {
        // WARNING! The code below causes a memory leak

        //if (!ThisPlayer.destroyed && setUp)
        //    Player.DestroyPlayer(ThisPlayer);
    }

    private void OnDestroy()
    {
        if (!ThisPlayer.destroyed)
            Player.DestroyPlayer(ThisPlayer);
        foreach (var script in ThisPlayer.Scripts)
            Destroy(script.Value);
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
    [Header("Components")]
    public GameObject gameObject;
    public Transform transform;
    public Rigidbody rigidBody;
    public Camera mainCamera;
    [Header("States")]
    public bool isAlive = true;
    public bool isGrounded = true;
    public bool isIdle = true;
    public bool isJumping = false;
    public bool canJump = true;
    [Space]
    public Dictionary<int, PlayerScript> Scripts = new();
    [Tooltip("The event called when the player dies or revives")] public static event Action<bool> OnStateChanged;
    [HideInInspector] public bool destroyed;
    [Header("Items")]
    public List<PowerUpType> activePowerUps = new();
    #endregion

    #region Status
    public void Kill()
    {
        rigidBody.constraints = RigidbodyConstraints.None;

        health = 0.0f;
        stamina = 0.0f;

        this.GetMonoBehaviour<PlayerMovement>().enabled = false;
        this.GetMonoBehaviour<PlayerCollisions>().enabled = false;
        gameObject.GetComponent<PlayerInput>().enabled = false;
        transform.Rotate(transform.eulerAngles + new Vector3(1.0f, 1.0f, 1.0f));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        OnStateChanged?.Invoke(false);
    }
    public void Respawn()
    {
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

        health = 1.0f;
        stamina = 1.0f;

        this.GetMonoBehaviour<PlayerMovement>().enabled = true;
        this.GetMonoBehaviour<PlayerCollisions>().enabled = true;
        gameObject.GetComponent<PlayerInput>().enabled = true;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        OnStateChanged?.Invoke(true);
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
        if (!InitializedPlayers.Contains(player)) return;

        foreach (Player thisPlayer in InitializedPlayers)
        {
            foreach (MonoBehaviour script in thisPlayer.gameObject.GetComponents<MonoBehaviour>())
                script.enabled = false;

            thisPlayer.rigidBody.isKinematic = true;
            thisPlayer.gameObject.GetComponent<PlayerInstance>().enabled = true;
        }

        if (!InitializedPlayers.Contains(player))
            InitializePlayer(player);

        if (ActivePlayer == player)
            return;

        ActivePlayer = player;

        foreach (MonoBehaviour script in player.gameObject.GetComponents<MonoBehaviour>())
            script.enabled = true;

        player.rigidBody.isKinematic = false;
        PlayerSelected?.Invoke(player);
    }
    public static void SelectPlayer(int index)
    {
        if (InitializedPlayers.Count <= index) return;

        foreach (Player thisPlayer in InitializedPlayers)
        {
            foreach (MonoBehaviour script in thisPlayer.gameObject.GetComponents<MonoBehaviour>())
                script.enabled = false;

            thisPlayer.rigidBody.isKinematic = true;

            thisPlayer.gameObject.GetComponent<PlayerInstance>().enabled = true;
        }

        if (ActivePlayer == InitializedPlayers.ElementAt(index))
            return;

        ActivePlayer = InitializedPlayers.ElementAt(index);

        foreach (MonoBehaviour script in InitializedPlayers.ElementAt(index).gameObject.GetComponents<MonoBehaviour>())
            script.enabled = true;

        InitializedPlayers.ElementAt(index).rigidBody.isKinematic = false;
        PlayerSelected?.Invoke(InitializedPlayers.ElementAt(index));
    }
    public static void DestroyPlayer(Player player)
    {
        if (!InitializedPlayers.Contains(player)) return;

        InitializedPlayers.Remove(player);
        if (InitializedPlayers.Count - 1 > 0) 
            SelectPlayer(InitializedPlayers.Count - 1);
        player.destroyed = true;
        PlayerDestroyed?.Invoke(player);
    }
    public static void DestroyPlayer(int index)
    {
        if (InitializedPlayers.Count <= index) return;

        InitializedPlayers.RemoveAt(index);
        SelectPlayer(InitializedPlayers.Count - 1);
        InitializedPlayers.ElementAt(index).destroyed = true;
        PlayerDestroyed?.Invoke(InitializedPlayers.ElementAt(index));
    }
    #endregion

    public Player() { }
    public Player(GameObject playerGameObject)
    {
        gameObject = playerGameObject;
        transform = playerGameObject.GetComponent<Transform>();
        rigidBody = playerGameObject.GetComponent<Rigidbody>();
        mainCamera = playerGameObject.GetComponentInChildren<Camera>();
    }
}