using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    #region Variable Initials
    [Tooltip("The speed at which the health bar is smoothed (Larger numbers decrease smoothness)")][SerializeField] float smoothing;
    private Slider sliderObject;
    #endregion

    private void Awake() => sliderObject = gameObject.GetComponent<Slider>();
    private void Update()
    {
        if (!(Player.ActivePlayer.health == sliderObject.value))
            sliderObject.value = Mathf.Lerp(sliderObject.value, Player.ActivePlayer.health, Time.deltaTime * smoothing);
    }
}