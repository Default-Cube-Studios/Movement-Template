using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Player/Collisions")]
[DisallowMultipleComponent]
public class PlayerCollisions : PlayerScript
{
    #region Variable Initials
    [SerializeField][Tooltip("A set of tags attatched to platforms and ground objects")] private string[] _groundTags;
    [SerializeField][Tooltip("A set of layers platforms and ground objects are assigned")] private string[] _groundLayers;
    [SerializeField] PlayerMovement movement;
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
    #endregion

    bool GroundCheck()
    {
        foreach (GameObject gameObject in _touchingGameObjects)
        {
            foreach (string tag in _groundTags)
                if (gameObject.CompareTag(tag)) return OnFall();
            foreach (string layer in _groundLayers)
                if (gameObject.layer == LayerMask.NameToLayer(layer)) return OnFall();
        }

        ThisPlayer.isGrounded = false;
        return false;

        bool OnFall()
        {
            movement.jumpCounter = 0;
            ThisPlayer.isGrounded = true;
            ThisPlayer.canJump = true;
            return true;
        }
    }
}