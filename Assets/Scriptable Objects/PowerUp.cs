using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Power Up", menuName = "Power Up")]
public class PowerUp : ScriptableObject
{
    #region Variable Initials
    [Tooltip("The event triggered when the power up is activated")] public UnityEvent OnCollect;
    [Tooltip("The event triggered when the power up timer finishes (Enable Is Timed first)")] public UnityEvent OnRevert;
    #endregion
}