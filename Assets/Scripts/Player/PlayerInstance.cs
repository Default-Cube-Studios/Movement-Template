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
    private bool setUp;

    public GameObject _playerPrefab;
    public static GameObject playerGameObject;
    #endregion

    void Awake()
    {
        playerGameObject = _playerPrefab;
        SetupPlayer();
    }

    void SetupPlayer()
    {
        if (setUp)
            return;

        ThisPlayer = new(gameObject);

        foreach (MonoBehaviour script in GetComponents<MonoBehaviour>())
            ThisPlayer.Scripts.Add(script);

        Player.InitializePlayer(ThisPlayer);
        if (_selectedOnAwake)
            Player.SelectPlayer(ThisPlayer.playerManagerIndex);
        setUp = true;
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

    //private void OnDestroy()
    //{
    //    if (!ThisPlayer.destroyed)
    //        Player.DestroyPlayer(ThisPlayer);
    //}
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
    public float speed;
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
    [Space]
    public List<MonoBehaviour> Scripts = new();
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

        gameObject.GetComponent<Movement>().enabled = false;
        gameObject.GetComponentInChildren<Collisions>().enabled = false;
        gameObject.GetComponent<PlayerInput>().enabled = false;
        transform.Rotate(transform.eulerAngles + new Vector3(1.0f, 1.0f, 1.0f));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        OnStatus?.Invoke(false);
    }
    public void Respawn()
    {
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

        health = 1.0f;
        stamina = 1.0f;

        gameObject.GetComponent<Movement>().enabled = true;
        gameObject.GetComponentInChildren<Collisions>().enabled = true;
        gameObject.GetComponent<PlayerInput>().enabled = true;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        OnStatus?.Invoke(true);
    }
    #endregion

    #region Player Manager
    [Tooltip("The currently active player")] public static Player ActivePlayer;
    [Tooltip("A list of all selectable players")] public static List<Player> InitializedPlayer = new();

    public static event Action<Player> PlayerInitialized;
    public static event Action<Player> PlayerSelected;
    public static event Action<Player> PlayerDestroyed;

    public static int InitializePlayer(Player player)
    {
        if (InitializedPlayer.Count > 0 && InitializedPlayer.Contains(player))
            return InitializedPlayer.IndexOf(player);

        InitializedPlayer.Add(player);
        player.playerManagerIndex = InitializedPlayer.Count - 1;
        PlayerInitialized?.Invoke(player);
        return InitializedPlayer.Count - 1;
    }

    public static void SelectPlayer(Player player)
    {
        if (!InitializedPlayer.Contains(player)) return;

        foreach (Player thisPlayer in InitializedPlayer)
        {
            foreach (MonoBehaviour script in thisPlayer.gameObject.GetComponents<MonoBehaviour>())
                script.enabled = false;

            thisPlayer.rigidBody.isKinematic = true;
            thisPlayer.gameObject.GetComponent<PlayerInstance>().enabled = true;
        }

        if (!InitializedPlayer.Contains(player))
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
        if (InitializedPlayer.Count <= index) return;

        foreach (Player thisPlayer in InitializedPlayer)
        {
            foreach (MonoBehaviour script in thisPlayer.gameObject.GetComponents<MonoBehaviour>())
                script.enabled = false;

            thisPlayer.rigidBody.isKinematic = true;

            thisPlayer.gameObject.GetComponent<PlayerInstance>().enabled = true;
        }

        if (ActivePlayer == InitializedPlayer.ElementAt(index))
            return;

        ActivePlayer = InitializedPlayer.ElementAt(index);

        foreach (MonoBehaviour script in InitializedPlayer.ElementAt(index).gameObject.GetComponents<MonoBehaviour>())
            script.enabled = true;

        InitializedPlayer.ElementAt(index).rigidBody.isKinematic = false;
        PlayerSelected?.Invoke(InitializedPlayer.ElementAt(index));
    }
    public static void DestroyPlayer(Player player)
    {
        if (!InitializedPlayer.Contains(player)) return;

        player.gameObject.GetComponent<PlayerInstance>().Destroy();
        InitializedPlayer.Remove(player);
        SelectPlayer(InitializedPlayer.Count - 1);
        player.destroyed = true;
        PlayerDestroyed?.Invoke(player);
    }
    public static void DestroyPlayer(int index)
    {
        if (InitializedPlayer.Count <= index) return;

        InitializedPlayer.ElementAt(index).gameObject.GetComponent<PlayerInstance>().Destroy();
        InitializedPlayer.RemoveAt(index);
        SelectPlayer(InitializedPlayer.Count - 1);
        InitializedPlayer.ElementAt(index).destroyed = true;
        PlayerDestroyed?.Invoke(InitializedPlayer.ElementAt(index));
    }
    #endregion

    public Player(GameObject playerGameObject)
    {
        gameObject = playerGameObject;
        transform = playerGameObject.GetComponent<Transform>();
        rigidBody = playerGameObject.GetComponent<Rigidbody>();
        mainCamera = playerGameObject.GetComponentInChildren<Camera>();
    }
}