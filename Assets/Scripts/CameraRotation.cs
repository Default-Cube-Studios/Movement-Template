using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [Header("Mouse Sensitivity")]
    [Tooltip("The ratio between the X position of the mouse to the Y rotation of the player")]
    [SerializeField][Range(1.0f,100.0f)] 
    float horizontalSensitivity;
    [Tooltip("The ratio between the Y position of the mouse to the X rotation of the camera")]
    [SerializeField][Range(1.0f, 100.0f)] 
    float verticalSensitivity;

    private float rotationX = 0.0f, rotationY = 0.0f;

    void Start()
    {
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Change rotationX by the position of the mouse, and clamp it to 90 degrees
        rotationX -= Input.GetAxisRaw("Mouse Y") * Time.deltaTime * verticalSensitivity;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        // Change rotationY by the position of the mouse
        rotationY += Input.GetAxisRaw("Mouse X") * Time.deltaTime * horizontalSensitivity;
        // Rotate the main camera and the player, by the X and Y axis
        Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.localRotation = Quaternion.Euler(0, rotationY, 0);
    }
}
