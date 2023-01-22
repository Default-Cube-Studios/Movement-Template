using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class StaminaBar : MonoBehaviour
{
    #region Variable Initials
    [Tooltip("The speed at which the stamina bar is smoothed (Larger numbers decrease smoothness)")][SerializeField] float smoothing;
    private Slider sliderObject;
    #endregion

    private void Awake() => sliderObject = gameObject.GetComponent<Slider>();
    private void Update()
    {
        if (!(Player.ActivePlayer.stamina == sliderObject.value))
            sliderObject.value = Mathf.Lerp(sliderObject.value, Player.ActivePlayer.stamina, Time.deltaTime * smoothing);
    }
}