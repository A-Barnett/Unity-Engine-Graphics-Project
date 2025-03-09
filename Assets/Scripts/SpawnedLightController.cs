using UnityEngine;

/// <summary>
/// Created By: Alex Barnett
/// Controls the movement of a spawned light object, making it rise upwards while rotating around its spawner.
/// </summary>
public class SpawnedLightController : MonoBehaviour
{
    [SerializeField] private float upwardSpeed; // Speed at which the light moves upwards.
    [SerializeField] private float spiralSpeed; // Speed at which the light rotates around the spawner.
    [SerializeField] private float spiralRadius; // Distance from the spawner around which the light rotates.
    [SerializeField] private float destroyTime; // Time after which the light is destroyed.

    private Transform spawner; // Reference to the spawner object.
    private float angle; // Current angle around the spawner.
    private float totalUpwardMovement; // Total distance moved upwards since spawn.

    void Start()
    {
        spawner = transform.parent; // Assumes the spawner is this light's parent object.
        totalUpwardMovement = 0f;
        angle = Random.Range(0f, 360f); // Start at a random angle around the spawner for variety.
    }

    void Update()
    {
        destroyTime -= Time.deltaTime;
        if (destroyTime <= 0)
        {
            Destroy(gameObject); // Destroy the light object when the timer runs out.
        }

        UpdatePosition();
    }

    /// <summary>
    /// Calculates and applies the new position of the light object based on its movement behavior.
    /// </summary>
    private void UpdatePosition()
    {
        angle += spiralSpeed * Time.deltaTime; // Increment angle based on spiral speed.
        float radians = angle * Mathf.Deg2Rad; // Convert angle to radians for trigonometric functions.

        // Calculate offset from the spawner based on the current angle.
        Vector3 offset = new Vector3(Mathf.Cos(radians) * spiralRadius, 0f, Mathf.Sin(radians) * spiralRadius);
        
        totalUpwardMovement += upwardSpeed * Time.deltaTime; // Update the total upward movement.

        // Calculate the new position with the updated offset and upward movement.
        Vector3 newPosition = spawner.position + offset;
        newPosition.y += totalUpwardMovement; // Apply the upward movement along the y-axis.
        
        transform.position = newPosition; // Apply the new position to the light object.
    }
}
