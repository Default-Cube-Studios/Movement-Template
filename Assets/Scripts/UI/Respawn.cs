using UnityEngine;
using UnityEngine.InputSystem;

public class Respawn : MonoBehaviour
{
    public void RespawnPlayer()
    {
        Player.PlayerObject.rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Player.PlayerObject.health = 1.0f;
        Player.PlayerObject.stamina = 1.0f;
        Player.PlayerObject.isPlayerAlive = true;

        Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().enabled = true;
        Player.PlayerObject.gameObject.GetComponent<CameraRotation>().enabled = true;
        Player.PlayerObject.gameObject.GetComponent<PlayerDamage>().enabled = true;
        Player.PlayerObject.gameObject.GetComponent<PlayerInput>().enabled = true;
        Player.PlayerObject.gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
