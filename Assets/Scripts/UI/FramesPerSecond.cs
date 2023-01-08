using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FramesPerSecond : MonoBehaviour
{
    public void LateUpdate() => gameObject.GetComponent<TextMeshProUGUI>().text = (Mathf.Round(1 / Time.unscaledDeltaTime)).ToString();
}