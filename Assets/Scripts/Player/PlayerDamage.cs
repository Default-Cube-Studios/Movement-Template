using UnityEngine;

[RequireComponent(typeof(Player))]
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

    public void LateUpdate()
    {
        if (!Player.PlayerObject.isPlayerMoving && !Player.PlayerObject.isPlayerJumping && Player.PlayerObject.isPlayerOnGround && Player.PlayerObject.health < 1.0f)
        {
            Player.PlayerObject.Repair(Time.deltaTime * damageRegenRate);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.y > minFallForce)
            Player.PlayerObject.Damage(Mathf.Pow(collision.relativeVelocity.y, forceExponent) / Mathf.Pow(maxFallForce, forceExponent));
    }
}