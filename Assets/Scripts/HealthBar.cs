using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider sliderObject;

    private void Update()
    {
        sliderObject.value = Player.PlayerObject.health;
    }
}