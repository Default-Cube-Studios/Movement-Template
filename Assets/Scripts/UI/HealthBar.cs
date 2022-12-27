using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    #region Variable Initials
    private Slider sliderObject;
    [Tooltip("The speed at which the health bar is smoothed (Larger numbers decrease smoothness)")][SerializeField] float smoothing;
    private float healthRaw;
    private float health;
    #endregion

    private void OnEnable() => PlayerClass.OnHealth += SetSlider;
    private void OnDisable() => PlayerClass.OnHealth -= SetSlider;

    private void SetSlider(float value)
    {
        healthRaw = value;
        sliderObject.value = health;
    }

    private void Awake() => sliderObject = gameObject.GetComponent<Slider>();
    private void Update()
    {
        health = Mathf.Lerp(sliderObject.value, healthRaw, Time.deltaTime * smoothing);
        if (!(Players.ActivePlayer.health == health))
            SetSlider(Players.ActivePlayer.health);
    }
}