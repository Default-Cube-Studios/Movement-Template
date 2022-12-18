using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FramesPerSecond : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;

    public void LateUpdate()
    {
        fpsText.text = (Mathf.Round(1 / Time.unscaledDeltaTime)).ToString();
    }
}