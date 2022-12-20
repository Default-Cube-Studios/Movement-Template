using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider sliderObject;
    [Tooltip("The speed at which the stamina bar is smoothed (Larger numbers decrease smoothness)")][SerializeField] float smoothing;
    private float staminaRaw;
    private float stamina;

    private void Update()
    {
        staminaRaw = Player.PlayerObject.stamina;
        stamina = Mathf.Lerp(sliderObject.value, staminaRaw, Time.deltaTime * smoothing);
        sliderObject.value = stamina;
    }
}