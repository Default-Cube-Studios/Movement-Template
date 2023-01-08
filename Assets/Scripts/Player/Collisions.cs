using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Collisions : MonoBehaviour
{
    #region Variable Initials
    public Player Player { get; private set; }
    [Tooltip("A set of tags attatched to platforms and ground objects")][SerializeField] private string[] _groundTags;
    private HashSet<GameObject> _touchingGameObjects = new();
    #endregion

    #region Unity Events
    void Awake() => Player = GetComponentInParent<PlayerInstance>().ThisPlayer;
    private void OnTriggerStay(Collider other) => _touchingGameObjects.Add(other.gameObject);
    private void OnTriggerExit(Collider other)
    {
        if (!Player.isJumping && !GroundCheck())
            Player.canJump = false;
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
                    Player.jumpCounter = 0;
                    Player.isGrounded = true;
                    Player.canJump = true;
                    return true;
                }
            }
        }
        Player.isGrounded = false;
        return false;
    }
}