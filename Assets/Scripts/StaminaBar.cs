using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public GameObject playerGameobject;
    private float stamina;
    private float maxStamina;
    public Slider sliderObject;


    private void Update()
    {
        stamina = playerGameobject.GetComponent<PlayerMovement>().stamina;
        maxStamina = playerGameobject.GetComponent<PlayerMovement>().maxStamina;
        stamina = stamina / maxStamina;
        sliderObject.value = stamina;
    }
}
