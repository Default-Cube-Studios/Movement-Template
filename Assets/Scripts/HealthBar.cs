using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    #region Variable Initials
    public Slider sliderObject;
    [Tooltip("The speed at which the health bar is smoothed (Larger numbers decrease smoothness)")][SerializeField] float smoothing;
    private float healthRaw;
    private float health;
    #endregion

    private void Update()
    {
        healthRaw = Player.PlayerObject.health;
        health = Mathf.Lerp(sliderObject.value, healthRaw, Time.deltaTime * smoothing);
        sliderObject.value = health;
    }
}