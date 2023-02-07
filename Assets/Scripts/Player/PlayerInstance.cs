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
    [Tooltip("This player instance")] public Player ThisPlayer { get; private set; }
    public bool _selectedOnAwake;
    [HideInInspector] public bool loaded;
    #endregion

    public void Awake()
    {
        loaded = true;
        ThisPlayer = new(gameObject);
        Player.InitializePlayer(ThisPlayer, _selectedOnAwake);
    }

    public void Destroy() => Destroy(ThisPlayer.gameObject);
    public void Destroy(float delay) => Destroy(ThisPlayer.gameObject, delay);

    private void OnDestroy()
    {
        if (!ThisPlayer.destroyed) Player.DestroyPlayer(ThisPlayer);
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
    public Dictionary<int, PlayerScript> Scripts;
    [Tooltip("The event called when the player dies or revives")] public static event Action<bool> OnStateChanged;
    [HideInInspector] public bool destroyed;
    public List<PowerUpType> activePowerUps = new();
    #endregion

    #region States
    public void Kill()
    {
        rigidBody.constraints = RigidbodyConstraints.None;

        health = 0.0f;
        stamina = 0.0f;

        this.GetScript<PlayerMovement>().enabled = false;
        this.GetScript<PlayerCollisions>().enabled = false;
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

        this.GetScript<PlayerMovement>().enabled = true;
        this.GetScript<PlayerCollisions>().enabled = true;
        gameObject.GetComponent<PlayerInput>().enabled = true;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        OnStateChanged?.Invoke(true);
    }
    #endregion

    #region Player Manager
    private static Player activePlayer;
    private static List<Player> initializedPlayers;

    [Tooltip("The currently active player")]
    public static Player ActivePlayer
    {
        get
        {
            if (activePlayer == null)
                if (InitializedPlayers.Count <= 0)
                    Debug.LogWarning("No players have been initialized!");
                else activePlayer = InitializedPlayers.ElementAt(0);
            return activePlayer;
        }
        private set => activePlayer = value;
    }
    [Tooltip("A list of all selectable players")]
    public static List<Player> InitializedPlayers
    {
        get
        {
            if (initializedPlayers == null) initializedPlayers = new();
            return initializedPlayers;
        }
        private set => initializedPlayers = value;
    }

    public event Action PlayerInitialized;
    public event Action PlayerSelected;
    public event Action PlayerDestroyed;

    public static int InitializePlayer(Player player, bool select)
    {
        if (InitializedPlayers.Count > 0 && InitializedPlayers.Contains(player))
            return InitializedPlayers.IndexOf(player);

        InitializedPlayers.Add(player);
        player.playerManagerIndex = InitializedPlayers.Count;

        foreach (var script in player.gameObject.GetComponentsInChildren<PlayerScript>())
        {
            if (script.TryGetComponent(out PlayerInstance playerReference))
                script.ThisPlayer = playerReference.ThisPlayer;
            else if (script.transform.parent.gameObject.TryGetComponent(out playerReference))
                script.ThisPlayer = playerReference.ThisPlayer;
            else
                throw new MissingComponentException($"No players could be found on {script.gameObject}" +
                    $" for {script} or its direct parent");
            script.loaded = true;
            if (!player.Scripts.ContainsKey(script.ScriptId) && script.isActiveAndEnabled) script.OnEnable();
        }

        player.PlayerInitialized?.Invoke();
        if (select) SelectPlayer(player);
        return InitializedPlayers.Count;
    }
    public static void SelectPlayer(Player player)
    {
        if (!InitializedPlayers.Contains(player))
            player = InitializedPlayers.ElementAt(InitializePlayer(player, false));
        if (ActivePlayer == player) return;

        foreach (var script in ActivePlayer.Scripts.Values.ToArray())
            script.enabled = false;

        ActivePlayer.mainCamera.GetComponent<AudioListener>().enabled = false;
        ActivePlayer.rigidBody.isKinematic = true;

        foreach (var script in player.Scripts)
            if (script.Value.canEnable) script.Value.enabled = true;

        ActivePlayer = player;
        player.rigidBody.isKinematic = false;
        player.mainCamera.GetComponent<AudioListener>().enabled = true;
        player.PlayerSelected?.Invoke();
    }
    public static void SelectPlayer(int index)
    {
        if (InitializedPlayers.Count <= index)
            throw new IndexOutOfRangeException("A player selection was made at the index " +
                $"{index}, but only {InitializedPlayers.Count} players have been initialized!");
        Player player = InitializedPlayers.ElementAt(index);
        if (ActivePlayer == player) return;

        foreach (var script in ActivePlayer.Scripts)
            if (script.Value.canDisable) script.Value.enabled = false;

        ActivePlayer.mainCamera.GetComponent<AudioListener>().enabled = false;
        ActivePlayer.rigidBody.isKinematic = true;

        foreach (var script in player.Scripts)
            if (script.Value.canEnable) script.Value.enabled = true;

        ActivePlayer = player;
        player.rigidBody.isKinematic = false;
        player.mainCamera.GetComponent<AudioListener>().enabled = true;
        player.PlayerSelected?.Invoke();
    }
    public static void DestroyPlayer(Player player)
    {
        if (!InitializedPlayers.Contains(player)) return;

        InitializedPlayers.Remove(player);
        if (InitializedPlayers.Count > 0)
            SelectPlayer(InitializedPlayers.Count - 1);
        player.destroyed = true;
        player.PlayerDestroyed?.Invoke();
    }
    public static void DestroyPlayer(int index)
    {
        Player player = InitializedPlayers.ElementAt(index);
        if (InitializedPlayers.Count <= index) return;

        InitializedPlayers.Remove(player);
        if (InitializedPlayers.Count > 0)
            SelectPlayer(InitializedPlayers.Count - 1);
        player.destroyed = true;
        player.PlayerDestroyed?.Invoke();
    }
    #endregion

    public Player(GameObject playerGameObject)
    {
        gameObject = playerGameObject;
        transform = playerGameObject.GetComponent<Transform>();
        rigidBody = playerGameObject.GetComponent<Rigidbody>();
        mainCamera = playerGameObject.GetComponentInChildren<Camera>();
        Scripts = new();
    }
}