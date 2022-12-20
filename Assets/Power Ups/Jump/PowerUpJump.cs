using UnityEngine;

[CreateAssetMenu(fileName = "New jump power up", menuName = "Power Up/Jump")]
public class PowerUpJump : ScriptableObject
{
    [Header("Identifiers")]
    public int level = 1;
    [TextArea(minLines: 2, maxLines: 6)] public string description;
    public string[] tags;
    [Header("Properties")]
    [Tooltip("Is the power up timed")] public bool isTimed;
    [Tooltip("This does not have an effect if the power up is not timed")] public float duration;
    [Header("Properties")]
    public float jumpForce;
    public float lowStaminaJumpForce;
    public bool infiniteJump;
    [Tooltip("This does not have an effect if infinite jumps are enabled")] public int maxJumps;
}