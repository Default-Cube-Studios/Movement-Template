using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
[DisallowMultipleComponent]
public class Players
{
    #region Variable Initials
    [Tooltip("The currently active player")] public static PlayerClass ActivePlayer;
    [SerializeField][Tooltip("The currently active player")] private PlayerClass DebugActivePlayer = ActivePlayer;
    [Tooltip("A list of all selectable players")] public List<PlayerClass> InitializedPlayers;

    public static event Action PlayerInitialized;
    public static event Action PlayerSelected;
    public static event Action PlayerDestroyed;

    public bool hasLoaded = false;
    #endregion

    public int InitializePlayer(PlayerClass player) 
    {
        if (InitializedPlayers.Contains(player))
            return InitializedPlayers.IndexOf(player);

        InitializedPlayers.Add(player);
        player.playerManagerIndex = InitializedPlayers.Count - 1;
        PlayerInitialized?.Invoke();
        return InitializedPlayers.Count - 1;
    }
    public void SelectPlayer(PlayerClass player)
    {
        if (!InitializedPlayers.Contains(player))
            InitializePlayer(player);

        if (ActivePlayer == player)
            return;

        ActivePlayer = player;
        DebugActivePlayer = ActivePlayer;
        PlayerSelected?.Invoke();
    }
    public void SelectPlayer(int index)
    {
        if (ActivePlayer == InitializedPlayers.ElementAt(index))
            return;

        ActivePlayer = InitializedPlayers.ElementAt(index);
        DebugActivePlayer = ActivePlayer;
        PlayerSelected?.Invoke();
    }
    public void DestroyPlayer(PlayerClass player)
    {
        player.gameObject.GetComponent<Player>().Destroy();
        InitializedPlayers.Remove(player);
        PlayerDestroyed?.Invoke();
    }
    public void DestroyPlayer(int index)
    {
        InitializedPlayers.ElementAt(index).gameObject.GetComponent<Player>().Destroy();
        InitializedPlayers.RemoveAt(index);
        PlayerDestroyed?.Invoke();
    }
}

public class PlayerManager : MonoBehaviour
{
    #region Variable Initials
    [Tooltip("This manager instance")] public static PlayerManager ManagerInstance;
    [Tooltip("The instance of the group of Players used by this Player Manager")] public static Players Instance = new();
    [SerializeField][Tooltip("Inspector-only values to debug the active player")] private Players DebugPlayers = Instance;
    public static event Action HasLoaded;
    #endregion

    void Awake()
    {
        if (ManagerInstance == null)
            ManagerInstance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance.hasLoaded = true;
        HasLoaded?.Invoke();
    }
}