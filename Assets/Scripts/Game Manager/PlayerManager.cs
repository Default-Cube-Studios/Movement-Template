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
    [SerializeField] private Player ActivePlayer = Player.ap;
    [SerializeField] private List<Player> InitializedPlayer = Player.ip;

    public static bool setUp = false;
    public static event Action SetUp;
    #endregion

    private void Awake()
    {
        playerGameObject = _playerPrefab;
        setUp = true;
        SetUp?.Invoke();
    }

    //public static Player CreatePlayer(Vector3 position) => Instantiate(playerGameObject, position, Quaternion.identity).GetComponent<PlayerInstance>().ThisPlayer;
}