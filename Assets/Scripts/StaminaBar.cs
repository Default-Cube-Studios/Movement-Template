using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider sliderObject;

    private void Update()
    {
        sliderObject.value = Player.PlayerObject.stamina;
    }
}