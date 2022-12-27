using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerDamage : MonoBehaviour
{
    #region Variable Initials
    [Header("Damage")]
    [SerializeField][Range(0.0f, 1.0f)] float damageRegenRate;
    [Header("Fall Damage")]
    [Tooltip("The minimum fall force before the player takes damage")][SerializeField] float minFallForce;
    [Tooltip("The largest amount of fall damage before the player dies")][SerializeField] float maxFallForce;
    [SerializeField] float forceExponent;
    #endregion

    void LateUpdate()
    {
        if (!Players.ActivePlayer.isPlayerMoving && !Players.ActivePlayer.isPlayerJumping && Players.ActivePlayer.isPlayerOnGround && Players.ActivePlayer.health < 1.0f)
            Players.ActivePlayer.Heal(Time.deltaTime * damageRegenRate);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.y > minFallForce)
            Players.ActivePlayer.Damage(Mathf.Pow(collision.relativeVelocity.y, forceExponent) / Mathf.Pow(maxFallForce, forceExponent));
    }

    #region Actions
    public void SetMinFallForce(float force) => minFallForce = force;
    public void SetMaxFallForce(float force) => maxFallForce = force;
    #endregion
}