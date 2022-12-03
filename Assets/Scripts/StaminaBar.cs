using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public GameObject playerGameobject;
    private float stamina;
    public Slider sliderObject;


    private void Update()
    {
        stamina = playerGameobject.GetComponent<PlayerMovement>().stamina;
        sliderObject.value = stamina;
    }
}
