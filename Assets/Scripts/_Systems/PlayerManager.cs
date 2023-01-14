using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerManager : MonoBehaviour
{
    #region Variable Initials
    [SerializeField] private Player ActivePlayer;
    [SerializeField] private List<Player> InitializedPlayer = Player.InitializedPlayer;
    #endregion

    private void Awake() => ActivePlayer = Player.ActivePlayer;
}