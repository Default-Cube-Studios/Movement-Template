using UnityEngine;

[CreateAssetMenu(fileName = "New speed power up", menuName = "Power Up/Speed")]
public class PowerUpSpeed : ScriptableObject
{
    [Header("Identifiers")]
    public int level = 1;
    [TextArea(minLines: 2, maxLines: 6)] public string description;
    public string[] tags;
    [Header("Properties")]
    [Tooltip("Is the power up timed")] public bool isTimed;
    [Tooltip("This does not have an effect if the power up is not timed")] public float duration;
    [Header("Properties")]
    public float speed;
    public float lowStaminaSpeed;
    public float sprintSpeed;
    public float fov;
}