using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Collisions : MonoBehaviour
{
    #region Variable Initials
    [Tooltip("A set of tags attatched to platforms and ground objects")][SerializeField] private string[] _groundTags;
    private HashSet<GameObject> _touchingGameObjects = new();
    #endregion

    #region Unity Events
    private void OnTriggerStay(Collider other) => _touchingGameObjects.Add(other.gameObject);
    private void OnTriggerExit(Collider other)
    {
        if (!Player.ActivePlayer.isJumping && !GroundCheck())
            Player.ActivePlayer.canJump = false;
    }
    private void LateUpdate()
    {
        GroundCheck();
        _touchingGameObjects.Clear();
    }
    #endregion

    bool GroundCheck()
    {
        foreach (GameObject gameObject in _touchingGameObjects)
        {
            foreach (string tag in _groundTags)
            {
                if (gameObject.CompareTag(tag))
                {
                    Player.ActivePlayer.jumpCounter = 0;
                    Player.ActivePlayer.isGrounded = true;
                    Player.ActivePlayer.canJump = true;
                    return true;
                }
            }
        }
        Player.ActivePlayer.isGrounded = false;
        return false;
    }
}