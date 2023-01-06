using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerManager : MonoBehaviour
{
    #region Variable Initials
    [SerializeField] private GameObject _playerPrefab;
    public static GameObject playerGameObject;
    [Header("Player")]
    [SerializeField] private Player ActivePlayer = Player.ActivePlayer;
    [SerializeField] private List<Player> InitializedPlayers = Player.InitializedPlayers;

    public static bool setUp = false;
    public static event Action SetUp;
    #endregion

    private void Awake()
    {
        playerGameObject = _playerPrefab;
        setUp = true;
        SetUp?.Invoke();
    }

    private void OnEnable() => Player.PlayerSelected += UpdatePlayer;
    private void OnDisable() => Player.PlayerSelected -= UpdatePlayer;

    public void UpdatePlayer(Player player) => ActivePlayer = player;

    public static Player CreatePlayer(Vector3 position) => Instantiate(playerGameObject, position, Quaternion.identity).GetComponent<PlayerInstance>().ThisPlayer;
}