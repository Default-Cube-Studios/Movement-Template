using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Messages : MonoBehaviour
{
    #region Variable Initials
    private static GameObject messageBox;
    public static TextMeshProUGUI textBox;
    public static string text; 
    #endregion

    private void Awake()
    {
        messageBox = gameObject;
        textBox = messageBox.GetComponent<TextMeshProUGUI>();
        Clear();
    }

    #region Functions
    public static void SetText(string text)
    {
        Messages.text = text;
        textBox.text = Messages.text;
    }
    public static void AddText(string text) => SetText(Messages.text + text);
    public static void Clear() => SetText("");
    #endregion
}