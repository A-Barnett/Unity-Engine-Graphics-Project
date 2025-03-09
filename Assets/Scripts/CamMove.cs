using UnityEngine;

/// <summary>
/// Created by: Alex Barnett
/// Handles camera movement and rotation based on player input.
/// </summary>
public class CamMove : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float fastSpeedMultiplier = 3.0f;
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float rollSpeed = 30.0f;
    [SerializeField] private GameObject camera;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private float roll = 0.0f;
    public bool inMenu; // Used to disable movement when the in-game menu is active

    void Update()
    {
        HandleCameraLook();
        HandleRoll();
        HandleMovement();
    }

    /// <summary>
    /// Handles camera look direction based on mouse input.
    /// </summary>
    private void HandleCameraLook()
    {
        if (!inMenu)
        {
            yaw += mouseSensitivity * Input.GetAxis("Mouse X");
            pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
            pitch = Mathf.Clamp(pitch, -90f, 90f); // Clamp pitch to prevent flipping

            // Set camera rotation
            camera.transform.eulerAngles = new Vector3(pitch, yaw, roll);
        }
    }

    /// <summary>
    /// Handles camera roll based on Q and E key input.
    /// </summary>
    private void HandleRoll()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            roll = Mathf.Clamp(roll - rollSpeed * Time.deltaTime, -90f, 90f); // Clamp roll
        }
        else if (Input.GetKey(KeyCode.E))
        {
            roll = Mathf.Clamp(roll + rollSpeed * Time.deltaTime, -90f, 90f); // Clamp roll
        }
    }

    /// <summary>
    /// Handles player movement based on WASD and Space/Ctrl for vertical movement.
    /// </summary>
    private void HandleMovement()
    {
        float speed = movementSpeed * (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? fastSpeedMultiplier : 1f);

        // Calculate movement vector
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        movement = camera.transform.TransformDirection(movement); // Transform movement to camera's local space
        movement.y = 0; // Maintain current y position (no vertical movement)

        // Apply movement to position
        transform.position += movement * speed * Time.deltaTime;

        // Vertical movement
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.position -= Vector3.up * speed * Time.deltaTime;
        }
    }
}
