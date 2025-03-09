using TMPro;
using UnityEngine;

/// <summary>
/// Created By: Alex Barnett
/// Displays performance statistics such as frames per second (FPS) and memory usage.
/// </summary>
public class PerformanceOverlay : MonoBehaviour
{
    public TextMeshProUGUI fpsText; // Text field for displaying FPS.
    public TextMeshProUGUI memoryText; // Text field for displaying memory usage.

    private float fpsUpdateInterval = 0.5f; // How often the FPS should be updated.
    private float timeLeft; // Timer for FPS update countdown.
    private int frames = 0; // Frame counter for FPS calculation.
    private float accumulatedFps = 0f; // Accumulated frames for averaging FPS.

    void Start()
    {
        // Initialize the FPS update countdown timer.
        timeLeft = fpsUpdateInterval;
    }

    void Update()
    {
        CalculateFPS();
        DisplayMemoryUsage();
    }

    /// <summary>
    /// Calculates and updates the FPS display at regular intervals.
    /// </summary>
    private void CalculateFPS()
    {
        timeLeft -= Time.deltaTime;
        accumulatedFps += Time.timeScale / Time.deltaTime;
        ++frames;

        // Update the FPS display when the interval is reached.
        if (timeLeft <= 0.0)
        {
            float fps = accumulatedFps / frames;
            if (fpsText)
            {
                fpsText.text = $"FPS: {Mathf.RoundToInt(fps)}"; // Update the FPS text display.
            }
            // Reset counters for the next interval.
            timeLeft = fpsUpdateInterval;
            accumulatedFps = 0f;
            frames = 0;
        }
    }

    /// <summary>
    /// Displays the current memory usage in MB.
    /// </summary>
    private void DisplayMemoryUsage()
    {
        if (memoryText)
        {
            memoryText.text = $"Memory: {Mathf.RoundToInt(System.GC.GetTotalMemory(false) / (1024 * 1024))} MB";
        }
    }
}