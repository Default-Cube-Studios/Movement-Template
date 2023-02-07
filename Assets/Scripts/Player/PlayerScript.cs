using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Player/My Player Script")]
[Serializable]
public abstract class PlayerScript : MonoBehaviour
{
    #region Variable Initials
    public Player ThisPlayer;
    [HideInInspector] public bool loaded = false;
    public int ScriptId { get => GetInstanceID(); }
    [Header("Toggles")]
    [Tooltip("Will this script be enabled when its player is selected")] public bool canEnable = true;
    [Tooltip("Will this script be disabled when its player is unselected")] public bool canDisable = true;
    #endregion

    #region Unity Events
    public virtual void OnEnable() 
    {
        if (loaded && !ThisPlayer.Scripts.ContainsKey(ScriptId)) 
            ThisPlayer.Scripts?.Add(ScriptId, this);
    }
    public virtual void OnDisable() => ThisPlayer.Scripts?.Remove(ScriptId);
    #endregion
}
public static class PlayerToolkit
{
    /// <summary>
    /// Returns a reference to a Player Script from a player.
    /// </summary>
    /// <typeparam name="T">The type of the script to get.</typeparam>
    /// <param name="player">The player object.</param>
    /// <returns>The the requested script as T.</returns>
    /// <exception cref="MissingComponentException"></exception>
    public static T GetScript<T>(this Player player) where T : PlayerScript
    {
        foreach (KeyValuePair<int, PlayerScript> item in player.Scripts)
            if (item.Value.GetType() == typeof(T))
                return item.Value as T;

        throw new MissingComponentException($"The script {nameof(T)} could not be found in" +
            $" {player.gameObject}");
    }

    /// <summary>
    /// Returns a reference to a Player Script from a player, filtered by its instance ID (additive).
    /// </summary>
    /// <typeparam name="T">The type of the script to get.</typeparam>
    /// <param name="player">The player object.</param>
    /// <param name="filterId">The instance ID of the script.</param>
    /// <returns>The the requested script as T.</returns>
    /// <exception cref="MissingComponentException"></exception>
    public static T GetScript<T>(this Player player, int filterId) where T : PlayerScript
    {
        foreach (KeyValuePair<int, PlayerScript> item in player.Scripts)
            if (item.Value.GetType() == typeof(T) && item.Key == filterId)
                return item.Value as T;

        throw new MissingComponentException($"The script {nameof(T)} with the ID {filterId} could" +
            $" not be found in {player.gameObject}");
    }

    /// <summary>
    /// Returns references to multiple Player Scripts from a player.
    /// </summary>
    /// <typeparam name="T">The type of the script to get.</typeparam>
    /// <param name="player">The player object.</param>
    /// <returns>The the requested scripts as T[].</returns>
    /// <exception cref="MissingComponentException"></exception>
    public static T[] GetScripts<T>(this Player player) where T : PlayerScript
    {
        List<T> scripts = new();
        foreach (KeyValuePair<int, PlayerScript> item in player.Scripts)
            if (item.Value.GetType() == typeof(T))
                scripts.Add(item.Value as T);
        if (scripts.Count > 0)
            return scripts.ToArray();
        throw new MissingComponentException($"The script {nameof(T)} could not be found in" +
            $" {player.gameObject}");
    }

    /// <summary>
    /// Returns references to multiple Player Scripts from a player, filtered out by ID (subtractive).
    /// </summary>
    /// <typeparam name="T">The type of the script to get.</typeparam>
    /// <param name="player">The player object.</param>
    /// <param name="filterId">The instance IDs to filter out.</param>
    /// <returns>The the requested scripts as T[].</returns>
    /// <exception cref="MissingComponentException"></exception>
    public static T[] GetScripts<T>(this Player player, int[] filterId) where T : PlayerScript
    {
        List<T> scripts = new();
        foreach (var item in player.Scripts)
            if (item.Value.GetType() == typeof(T) && CheckFilters(player.Scripts, filterId))
                foreach (var id in filterId)
                    if (item.Key != id)
                        scripts.Add(item.Value as T);
        if (scripts.Count > 0)
            return scripts.ToArray();
        throw new MissingComponentException($"The script {nameof(T)} could not be" +
            $" found in {player.gameObject}");
    }

    private static bool CheckFilters(Dictionary<int, PlayerScript> scripts, int[] filters)
    {
        foreach (var id in filters)
            if (scripts.ContainsKey(id))
                return false;
        return true;
    }
}