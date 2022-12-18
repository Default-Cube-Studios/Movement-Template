using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{
    #region Variable Initials
    [Header("Mouse Sensitivity")]
    [Tooltip("The ratio between the X position of the mouse to the Y rotation of the player")][SerializeField][Range(0.0f,100.0f)] float horizontalSensitivity;
    [Tooltip("The ratio between the Y position of the mouse to the X rotation of the camera")][SerializeField][Range(0.0f, 100.0f)] float verticalSensitivity;

    private float rotationX, rotationY;
    private Vector2 cameraInput = Vector2.zero;
    private Vector2 cameraInputRaw = Vector2.zero;
    [Tooltip("The speed at which mouse movement is smoothed (Larger numbers decrease smoothness)")][SerializeField] float inputSmoothing;
    #endregion

    #region Unity Engine
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        cameraInput = Vector2.Lerp(cameraInput, cameraInputRaw, Time.deltaTime * inputSmoothing);

        rotationX -= cameraInput.y * Time.deltaTime * verticalSensitivity;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        rotationY += cameraInput.x * Time.deltaTime * horizontalSensitivity;

        Player.PlayerObject.mainCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        Player.PlayerObject.rigidBody.MoveRotation(Quaternion.Euler(0, rotationY, 0));
    }
    #endregion

    public void OnLook(InputAction.CallbackContext value)
    {
        cameraInputRaw = value.ReadValue<Vector2>();
    }
}