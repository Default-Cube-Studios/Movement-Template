using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Player/Collisions")]
[DisallowMultipleComponent]
public class Collisions : MonoBehaviour
{
    #region Variable Initials
    private Player ThisPlayer;
    [Tooltip("A set of tags attatched to platforms and ground objects")][SerializeField] private string[] _groundTags;
    private HashSet<GameObject> _touchingGameObjects = new();
    #endregion

    #region Unity Events
    private void OnTriggerStay(Collider other) => _touchingGameObjects.Add(other.gameObject);
    private void OnTriggerExit(Collider other)
    {
        if (!ThisPlayer.isJumping && !GroundCheck())
            ThisPlayer.canJump = false;
    }
    private void LateUpdate()
    {
        GroundCheck();
        _touchingGameObjects.Clear();
    }
    private void OnEnable() => Player.PlayerSelected += SetPlayer;
    private void OnDisable() => Player.PlayerSelected -= SetPlayer;
    #endregion

    bool GroundCheck()
    {
        foreach (GameObject gameObject in _touchingGameObjects)
            foreach (string tag in _groundTags)
                if (gameObject.CompareTag(tag))
                {
                    ThisPlayer.jumpCounter = 0;
                    ThisPlayer.isGrounded = true;
                    ThisPlayer.canJump = true;
                    return true;
                }

        ThisPlayer.isGrounded = false;
        return false;
    }


    void SetPlayer(Player player)
    {
        if (player != GetComponentInParent<PlayerInstance>().ThisPlayer)
            return;

        ThisPlayer = player;
    }
}