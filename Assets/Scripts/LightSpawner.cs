using UnityEngine;

/// <summary>
/// Created By: Alex Barnett
/// Spawns light objects at regular intervals.
/// </summary>
public class LightSpawner : MonoBehaviour
{
    [SerializeField] private GameObject lightPrefab; // Prefab to spawn as light object.
    [SerializeField] private float spawnInterval = 2f; // Interval in seconds between each spawn.

    private float timeSinceLastSpawn; // Timer to track time since the last spawn.

    void Start()
    {
        // Initialize the timer.
        timeSinceLastSpawn = 0f;
    }

    void Update()
    {
        // Increment the timer by the time elapsed since the last frame.
        timeSinceLastSpawn += Time.deltaTime;

        // Check if the spawn interval has been reached or exceeded.
        if (timeSinceLastSpawn >= spawnInterval)
        {
            // Spawn the light and reset the timer.
            SpawnLight();
            timeSinceLastSpawn = 0f;
        }
    }

    /// <summary>
    /// Instantiate the light object as a child of the spawner to maintain organization in the hierarchy.
    /// </summary>
    void SpawnLight()
    {
        Instantiate(lightPrefab, transform.position, Quaternion.identity, transform);
    }
}

