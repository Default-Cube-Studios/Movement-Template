using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    #region Variable Initials
    [Header("Damage")]
    [SerializeField][Range(0.0f, 1.0f)] float damageRegenRate;
    [Header("Fall Damage")]
    [SerializeField] float minimumFallForce;
    [Tooltip("The exponent that fall damage multiplies against")][SerializeField] float forceExponent;
    [Tooltip("The amount fall damage divides by (Larger numbers decrease fall damage)")][SerializeField] float forceDivider;

    private Vector3 playerVelocityLastFrame;
    private Vector3 playerVelocityThisFrame;

    private Vector3 velocityChange;
    private float forceOnPlayer;
    #endregion

    public void Update()
    {
        playerVelocityThisFrame = Player.PlayerObject.rigidBody.velocity;
        velocityChange = playerVelocityThisFrame - playerVelocityLastFrame;
        forceOnPlayer = velocityChange.x + velocityChange.y + velocityChange.z;

        if (forceOnPlayer > minimumFallForce)
        {
            Player.PlayerObject.Damage(Mathf.Pow(forceOnPlayer, forceExponent) / forceDivider);
            // Debug.Log(Mathf.Pow(forceOnPlayer, forceExponent) / forceDivider);
        }

        playerVelocityLastFrame = playerVelocityThisFrame;
    }

    public void LateUpdate()
    {
        if (!Player.PlayerObject.isPlayerMoving && !Player.PlayerObject.isPlayerJumping && Player.PlayerObject.isPlayerOnGround && Player.PlayerObject.health < 1.0f)
        {
            Player.PlayerObject.health += Time.deltaTime * damageRegenRate;
        }
    }
}