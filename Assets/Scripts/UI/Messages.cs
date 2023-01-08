using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Messages : MonoBehaviour
{
    private static GameObject messageBox;
    public static TextMeshProUGUI textBox;
    public static string text;

    private void Awake()
    {
        messageBox = gameObject;
        textBox = messageBox.GetComponent<TextMeshProUGUI>();
        Clear();
    }

    public static void SetText(string text)
    {
        Messages.text = text;
        textBox.text = Messages.text;
    }
    public static void AddText(string text)
    {
        Messages.text += text;
        textBox.text = Messages.text;
    }
    public static void Clear()
    {
        text = string.Empty;
        textBox.text = text;
    }
}