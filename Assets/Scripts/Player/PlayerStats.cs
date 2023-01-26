using UnityEngine;

[RequireComponent(typeof(PlayerInstance))]
[AddComponentMenu("Player/Stats")]
[DisallowMultipleComponent]
public class PlayerStats : PlayerScript
{
    #region Variable Initials
    [Header("Health")]
    [Range(0.0f, 1.0f)] public float _healthRegenRate;
    [SerializeField][Tooltip("The minimum fall force before the player takes damage")] float _minFallForce;
    [SerializeField][Tooltip("The largest amount of fall damage before the player dies")] float _maxFallForce;
    [Header("Stamina")]
    [Range(0.0f, 1.0f)] public float _staminaRegenRate;
    #endregion

    #region Unity Events
    private void LateUpdate()
    {
        if (ThisPlayer.isIdle && ThisPlayer.isGrounded && !ThisPlayer.isJumping)
        {
            Heal(Time.deltaTime * _healthRegenRate);
            Energize(Time.deltaTime * _staminaRegenRate);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.y > _minFallForce)
            Damage((collision.relativeVelocity.y - _minFallForce) / _maxFallForce);
    } 
    #endregion

    #region Health
    public void Damage(float damageAmount)
    {
        if (ThisPlayer.health > damageAmount)
            ThisPlayer.health -= damageAmount;
        else
            ThisPlayer.Kill();
    }
    public void Heal(float healAmount)
    {
        if (ThisPlayer.health + healAmount < 1.0f)
            ThisPlayer.health += healAmount;
        else
            ThisPlayer.health = 1.0f;
    }
    #endregion

    #region Stamina
    public void Tire(float tireAmount)
    {
        if (ThisPlayer.stamina > tireAmount)
            ThisPlayer.stamina -= tireAmount;
        else
            ThisPlayer.stamina = 0.0f;
    }
    public void Energize(float staminaAmount)
    {
        if (ThisPlayer.stamina + staminaAmount < 1.0f)
            ThisPlayer.stamina += staminaAmount;
        else
            ThisPlayer.stamina = 1.0f;
    }
    #endregion
}