using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    #region Variable Initials
    private Slider sliderObject;
    [Tooltip("The speed at which the stamina bar is smoothed (Larger numbers decrease smoothness)")][SerializeField] float smoothing;
    private float staminaRaw;
    private float stamina;
    #endregion

    private void OnEnable() => PlayerClass.OnStamina += SetSlider;
    private void OnDisable() => PlayerClass.OnStamina -= SetSlider;

    private void SetSlider(float value)
    {
        staminaRaw = value;
        sliderObject.value = stamina;
    }

    private void Awake() => sliderObject = gameObject.GetComponent<Slider>();
    private void Update()
    {
        stamina = Mathf.Lerp(sliderObject.value, staminaRaw, Time.deltaTime * smoothing);
        if (!(Players.ActivePlayer.stamina == stamina))
            SetSlider(Players.ActivePlayer.stamina);
    }
}