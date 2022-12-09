using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public GameObject playerGameobject;
    public Slider sliderObject;

    private void Update()
    {
        sliderObject.value = playerGameobject.GetComponent<PlayerMovement>().PlayerObject.stamina;
    }
}