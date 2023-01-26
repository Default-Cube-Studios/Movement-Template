using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Player/My Player Script")]
[Serializable]
public abstract class PlayerScript : MonoBehaviour
{
    #region Variable Initials
    public Player ThisPlayer = new();
    private PlayerInstance playerReference;
    [SerializeField] public int ScriptId { get => GetInstanceID(); }
    public static event Action<PlayerScript, bool> OnToggled;
    private bool loaded = false;
    #endregion

    #region Unity Events
    public virtual void Start() => SetupPlayer();
    public virtual void OnEnable()
    {
        if (!loaded) return;
        ThisPlayer.Scripts.Add(ScriptId, this);
        OnToggled?.Invoke(this, true);
    }
    public virtual void OnDisable()
    {
        if (!loaded) return;
        ThisPlayer.Scripts.Remove(ScriptId);
        OnToggled?.Invoke(this, false);
    } 
    #endregion

    /// <summary>
    /// Sets up a reference to the requested player on the same Game Object or its direct parent.
    /// </summary>
    /// <param name="player"></param>
    /// <exception cref="MissingComponentException()"></exception>
    public virtual void SetupPlayer()
    {
        loaded = true;
        if (TryGetComponent(out playerReference))
            ThisPlayer = playerReference.ThisPlayer;
        else if (transform.parent.gameObject.TryGetComponent(out playerReference))
            ThisPlayer = playerReference.ThisPlayer;
        else
            throw new MissingComponentException($"No players could be found on {gameObject}" +
                $" for {this} or its direct parent");

        if (isActiveAndEnabled) OnEnable();
        else OnDisable();
    }
}
public static class PlayerToolkit
{
    /// <summary>
    /// Returns a reference to a Player Script from a player.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="player"></param>
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
    /// <typeparam name="T"></typeparam>
    /// <param name="player"></param>
    /// <param name="filterId"></param>
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
    /// <typeparam name="T"></typeparam>
    /// <param name="player"></param>
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
    /// <typeparam name="T"></typeparam>
    /// <param name="player"></param>
    /// <param name="filterId"></param>
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


    /// <summary>
    /// Returns a reference to a Player Script from a player.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="player"></param>
    /// <returns>The the requested script as a MonoBehaviour.</returns>
    /// <exception cref="MissingComponentException"></exception>
    public static MonoBehaviour GetMonoBehaviour<T>(this Player player) where T : PlayerScript
    {
        foreach (KeyValuePair<int, PlayerScript> item in player.Scripts)
            if (item.Value.GetType() == typeof(T))
                return item.Value;

        throw new MissingComponentException($"The script {nameof(T)} could not be found in" +
            $" {player.gameObject}");
    }

    /// <summary>
    /// Returns a reference to a Player Script from a player, filtered by its instance ID (additive).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="player"></param>
    /// <param name="filterId"></param>
    /// <returns>The the requested script as a MonoBehaviour.</returns>
    /// <exception cref="MissingComponentException"></exception>
    public static MonoBehaviour GetMonoBehaviour<T>(this Player player, int filterId) where T : PlayerScript
    {
        foreach (KeyValuePair<int, PlayerScript> item in player.Scripts)
            if (item.Value.GetType() == typeof(T) && item.Key == filterId)
                return item.Value;

        throw new MissingComponentException($"The script {nameof(T)} with the ID {filterId} could" +
            $" not be found in {player.gameObject}");
    }

    /// <summary>
    /// Returns references to multiple Player Scripts from a player.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="player"></param>
    /// <returns>The the requested scripts as MonoBehaviour[].</returns>
    /// <exception cref="MissingComponentException"></exception>
    public static MonoBehaviour[] GetMonoBehaviours<T>(this Player player) where T : PlayerScript
    {
        List<MonoBehaviour> scripts = new();
        foreach (KeyValuePair<int, PlayerScript> item in player.Scripts)
            if (item.Value.GetType() == typeof(T))
                scripts.Add(item.Value);
        if (scripts.Count > 0)
            return scripts.ToArray();
        throw new MissingComponentException($"The script {nameof(T)} could not be found in" +
            $" {player.gameObject}");
    }

    /// <summary>
    /// Returns references to multiple Player Scripts from a player, filtered out by ID (subtractive).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="player"></param>
    /// <param name="filterId"></param>
    /// <returns>The the requested scripts as MonoBehaviour[].</returns>
    /// <exception cref="MissingComponentException"></exception>
    public static MonoBehaviour[] GetMonoBehaviours<T>(this Player player, int[] filterId) where T : PlayerScript
    {
        List<MonoBehaviour> scripts = new();
        foreach (var item in player.Scripts)
            if (item.Value.GetType() == typeof(T) && CheckFilters(player.Scripts, filterId))
                scripts.Add(item.Value);
        if (scripts.Count > 0)
            return scripts.ToArray();
        throw new MissingComponentException($"The script {nameof(T)} could not be" +
            $" found in {player.gameObject}");
    }

    internal static bool CheckFilters(Dictionary<int, PlayerScript> scripts, int[] filters)
    {
        foreach (var id in filters)
            if (scripts.ContainsKey(id))
                return false;
        return true;
    }
}