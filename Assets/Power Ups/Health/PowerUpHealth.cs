using UnityEngine;

[CreateAssetMenu(fileName = "New health power up", menuName = "Power Up/Health")]
public class PowerUpHealth : ScriptableObject
{
    [Header("Identifiers")]
    public int level = 1;
    [TextArea(minLines: 2, maxLines: 6)] public string description;
    public string[] tags;
    [Header("Properties")]
    [Tooltip("Is the power up timed")] public bool isTimed;
    [Tooltip("This does not have an effect if the power up is not timed")] public float duration;
    [Range(0.0f, 1.0f)] public float health;
}