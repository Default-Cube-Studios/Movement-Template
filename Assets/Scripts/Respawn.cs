using UnityEngine;
using UnityEngine.UI;

public class Respawn : MonoBehaviour
{
    public void RespawnPlayer()
    {
        Player.PlayerObject.gameObject.transform.position= Vector3.zero;
        Player.PlayerObject.health = 1.0f;
        Player.PlayerObject.stamina = 1.0f;
        Player.PlayerObject.isPlayerAlive = true;

        Player.PlayerObject.gameObject.GetComponent<PlayerMovement>().enabled = false;
        Player.PlayerObject.gameObject.GetComponent<CameraRotation>().enabled = false;
        Player.PlayerObject.gameObject.GetComponent<PlayerDamage>().enabled = false;
        Player.PlayerObject.rigidBody.AddForce(new Vector3(15f, 15f, 15f));

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}