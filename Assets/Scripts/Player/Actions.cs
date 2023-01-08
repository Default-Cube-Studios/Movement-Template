using UnityEngine;

[RequireComponent(typeof(PlayerInstance))]
[DisallowMultipleComponent]
public class Actions : MonoBehaviour
{
    public void Heal(float amount) => Player.ActivePlayer.Heal(amount);
    public void Damage(float amount) => Player.ActivePlayer.Damage(amount);
    public void DrainStamina(float amount) => Player.ActivePlayer.DrainStamina(amount);
    public void RegenStamina(float amount) => Player.ActivePlayer.RegenStamina(amount);
    public void SetState(bool state) => Player.ActivePlayer.isAlive = state;
    public void Kill() => Player.ActivePlayer.Kill();
    public void Respawn() => Player.ActivePlayer.Respawn();

    //public void AddWalkSpeed(float speed) => Player.ActivePlayer.MovementScript._walkSpeed += speed;
    //public void AddSprintSpeed(float speed) => Player.ActivePlayer.MovementScript._sprintSpeed += speed;

    //public void SetWalkStaminaDrain(float amount) => Player.ActivePlayer.MovementScript._moveStaminaDrainRate = amount;
    //public void SetSprintStaminaDrain(float amount) => Player.ActivePlayer.MovementScript._sprintStaminaDrainRate = amount;
    //public void SetStaminaRegen(float amount) => Player.ActivePlayer.MovementScript._staminaRegenRate = amount;

    //public void AddJumpForce(float force) => Player.ActivePlayer.MovementScript._jumpForce += force;
    //public void SetJumpStaminaLoss(float amount) => Player.ActivePlayer.MovementScript._jumpStaminaLoss = amount;
    //public void SetMaxJumps(int jumps) => Player.ActivePlayer.MovementScript._maxJumps = jumps;
    //public void SetInfiniteJump(bool state) => Player.ActivePlayer.MovementScript._infiniteJump = state;

    //public void SetMinFallForce(float force) => Player.ActivePlayer.MovementScript._minFallForce = force;
    //public void SetMaxFallForce(float force) => Player.ActivePlayer.MovementScript._maxFallForce = force;
}