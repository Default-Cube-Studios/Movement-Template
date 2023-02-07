using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerManager : MonoBehaviour
{
    #region Variable Initials
    [SerializeField] private Player ActivePlayer;
    [SerializeField] private List<Player> InitializedPlayers;

    public GameObject _playerPrefab;
    public static GameObject playerGameObject;
    #endregion

    private void Start()
    {
        ActivePlayer = Player.ActivePlayer;
        InitializedPlayers = Player.InitializedPlayers;
    }

    private void OnEnable() => playerGameObject = _playerPrefab;

    public static Player CreatePlayer(Vector3 position) =>
        Instantiate(playerGameObject, position, Quaternion.identity).GetComponent<PlayerInstance>().ThisPlayer;
}