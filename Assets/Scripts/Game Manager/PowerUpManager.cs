using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PowerUpManager : MonoBehaviour
{
    #region Variable Initials
    [Header("Power Ups")]
    public PowerUpHealth healthPowerUp;
    public PowerUpStamina staminaPowerUp;
    public PowerUpSpeed speedPowerUp;
    public PowerUpJump jumpPowerUp;
    #endregion

    public void OnTriggerEnter(Collider collision)
    {
        if (!collision.CompareTag(Player.PlayerObject.tag))
            return;

        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<SphereCollider>().enabled = false;

        if (healthPowerUp)
            HealthPowerUp();

        if (staminaPowerUp)
            StaminaPowerUp();

        if (speedPowerUp)
            SpeedPowerUp();

        if (jumpPowerUp)
            JumpPowerUp();

        Destroy(gameObject);
    }

    #region Power Ups
    public void HealthPowerUp()
    {
        float currentPlayerHealth = Player.PlayerObject.health;
        Player.PlayerObject.health += healthPowerUp.health;
        if (healthPowerUp.isTimed)
            StartCoroutine(HealthPowerUpTimer(currentPlayerHealth));
    }
    public void StaminaPowerUp()
    {
        float currentPlayerStamina = Player.PlayerObject.stamina;
        Player.PlayerObject.stamina += staminaPowerUp.stamina;
        if (staminaPowerUp.isTimed)
            StartCoroutine(StaminaPowerUpTimer(currentPlayerStamina));
    }
    public void SpeedPowerUp()
    {
        float[] movementSpeeds =
        {
            Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().walkSpeed,
            Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().sprintSpeed,
            Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().lowStaminaSpeed
        };

        Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().walkSpeed += speedPowerUp.speed;
        Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().sprintSpeed += speedPowerUp.sprintSpeed;
        Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().lowStaminaSpeed += speedPowerUp.lowStaminaSpeed;

        if (speedPowerUp.isTimed)
            StartCoroutine(SpeedPowerUpTimer(movementSpeeds));
    }
    public void JumpPowerUp()
    {
        float[] jumpForces =
        {
            Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().jumpForce,
            Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().lowStaminaJumpForce
        };
        bool infiniteJump = Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().infiniteJump;
        int maxJumps = Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().maxJumps;

        Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().jumpForce += jumpPowerUp.jumpForce;
        Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().lowStaminaJumpForce += jumpPowerUp.lowStaminaJumpForce;
        Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().infiniteJump = jumpPowerUp.infiniteJump;
        Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().maxJumps = jumpPowerUp.maxJumps;

        if (jumpPowerUp.isTimed)
            StartCoroutine(JumpPowerUpTimer(jumpForces, infiniteJump, maxJumps));
    }
    #endregion

    #region Coroutines
    IEnumerator HealthPowerUpTimer(float returnToHealth)
    {
        yield return new WaitForSeconds(healthPowerUp.duration);
        Player.PlayerObject.health = returnToHealth;
    }
    IEnumerator StaminaPowerUpTimer(float returnToStamina)
    {
        yield return new WaitForSeconds(staminaPowerUp.duration);
        Player.PlayerObject.health = returnToStamina;
    }
    IEnumerator SpeedPowerUpTimer(float[] movementSpeeds)
    {
        yield return new WaitForSeconds(speedPowerUp.duration);
        Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().walkSpeed = movementSpeeds[0];
        Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().sprintSpeed = movementSpeeds[1];
        Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().lowStaminaSpeed = movementSpeeds[2];
    }
    IEnumerator JumpPowerUpTimer(float[] jumpForces, bool infiniteJump, int maxJumps)
    {
        yield return new WaitForSeconds(speedPowerUp.duration);
        Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().jumpForce = jumpForces[0];
        Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().lowStaminaJumpForce = jumpForces[1];
        Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().infiniteJump = infiniteJump;
        Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().maxJumps = maxJumps;
    }
    #endregion
}