using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private float lookSensitivity = 100f;
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 50f;
    [SerializeField] private float minHorizontalAngle = -70f;
    [SerializeField] private float maxHorizontalAngle = 80f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private float verticalRotation = 0f;   // Up/Down
    private float horizontalRotation = 0f; // Left/Right

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLookInput();
    }

    /// <summary>
    /// Handles mouse look input and applies rotation to player and camera.
    /// </summary>
    private void HandleLookInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;

        // Horizontal rotation (player body)
        horizontalRotation += mouseX;
        horizontalRotation = Mathf.Clamp(horizontalRotation, minHorizontalAngle, maxHorizontalAngle);
        transform.localRotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        // Vertical rotation (camera)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }
}
