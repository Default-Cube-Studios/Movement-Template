using UnityEngine;

[CreateAssetMenu(fileName = "New Power Up", menuName = "Power Up/Stats")]
public class PowerUpStats : ScriptableObject
{
    [Header("Identifiers")]
    public GameObject powerUpObject;
    public new string name;
    public string[] tags;
    [TextArea(minLines: 2, maxLines: 6)] public string description;
    [Header("Properties")]
    [Tooltip("Is the power up timed")] public bool isTimed;
    [Tooltip("This does not have an effect if the power up is not timed")] public float duration;
    [Header("Stats")]
    [Range(0.0f, 1.0f)] public float health;
    [Range(0.0f, 1.0f)] public float stamina;
}

[CreateAssetMenu(fileName = "New Power Up", menuName = "Power Up/Movement")]
public class PowerUpMovement : ScriptableObject
{
    [Header("Identifiers")]
    public GameObject powerUpObject;
    public new string name;
    public string[] tags;
    [TextArea(minLines: 2, maxLines: 6)] public string description;
    [Header("Properties")]
    [Tooltip("Is the power up timed")] public bool isTimed;
    [Tooltip("This does not have an effect if the power up is not timed")] public float duration;
    [Header("Movement")]
    public float movementSpeed;
    public float sprintSpeed;
    public float fov;
    public float jumpForce;
}