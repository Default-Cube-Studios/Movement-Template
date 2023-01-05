using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[DisallowMultipleComponent]
public class PowerUp : MonoBehaviour
{
    #region Variable Initials
    public PowerUpType PowerUpType;
    [Header("Timer")]
    [Tooltip("Is the power up timed")] public bool isTimed;
    [Tooltip("How long the power up lasts for")] public float duration;
    #endregion

    #region Unity Events
    public void Awake() => SetColor(PowerUpType._color, PowerUpType._colorIntensity);
    public void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(Player.ActivePlayer.gameObject.tag))
            return;

        PowerUpType.OnCollect.Invoke();

        if (isTimed)
        {
            StartCoroutine(RevertPowerUp());
            Player.ActivePlayer.activePowerUps.Add(PowerUpType);
        }
        else
            Destroy(gameObject);
    }
    #endregion

    public void SetColor(Color color, float intensity)
    {
        float emissionIntensity = intensity - 0.4169f;
        Color emissionColor = color * Mathf.Pow(2.0f, emissionIntensity);
        Material renderer = gameObject.GetComponent<MeshRenderer>().material;

        renderer.SetColor("_EmissionColor", emissionColor);
    }
    IEnumerator RevertPowerUp()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<SphereCollider>().enabled = false;
        yield return new WaitForSeconds(duration);
        Player.ActivePlayer.activePowerUps.Remove(PowerUpType);
        PowerUpType.OnRevert.Invoke();
        Destroy(gameObject);
    }
    public PowerUp(PowerUpType powerUpType) => PowerUpType = powerUpType;

    public void SetPowerUp(PowerUpType powerUp)
    {
        PowerUpType = powerUp;
        SetColor(PowerUpType._color, PowerUpType._colorIntensity);
    }
}

// TEMP CODE BELOW
public class Player
{
    public static PlayerClass ActivePlayer = new();
    public static List<PlayerClass> InitializedPlayer = new();
    public static Player ap = new();
    public static List<Player> ip = new();
}

public class PlayerClass
{
    public List<PowerUpType> activePowerUps = new();
    public GameObject gameObject;
}