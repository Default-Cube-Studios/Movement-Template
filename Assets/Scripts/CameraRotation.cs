using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] GameObject playerGameObject;
    [SerializeField] Camera mainCamera;
    [Header("Mouse Sensitivity")]
    [Tooltip("The ratio between the X position of the mouse to the Y rotation of the player")]
    [SerializeField][Range(0f,100f)] 
    float horizontalSensitivity;
    [Tooltip("The ratio between the Y position of the mouse to the X rotation of the camera")]
    [SerializeField][Range(0f, 100f)] 
    float verticalSensitivity;

    private float rotationX, rotationY;
    private Vector2 cameraInput = Vector2.zero;
    private Vector2 cameraInputRaw = Vector2.zero;
    [SerializeField] float inputSmoothing;

    void Start()
    {
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        cameraInput = Vector2.Lerp(cameraInput, cameraInputRaw, Time.deltaTime * inputSmoothing);

        // Change rotationX by the position of the mouse, and clamp it to 90 degrees
        rotationX -= cameraInput.y * Time.deltaTime * verticalSensitivity;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        // Change rotationY by the position of the mouse
        rotationY += cameraInput.x * Time.deltaTime * horizontalSensitivity;
        // Rotate the main camera and the player, by the X and Y axis
        mainCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        playerGameObject.transform.localRotation = Quaternion.Euler(0, rotationY, 0);
    }
    public void OnLook(InputAction.CallbackContext value)
    {
        cameraInput = value.ReadValue<Vector2>();
    }
}
