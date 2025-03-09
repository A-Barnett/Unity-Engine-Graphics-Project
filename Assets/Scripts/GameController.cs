using UnityEngine;

/// <summary>
/// Created By: Alex Barnett
/// Controls the game state, toggling in-game menus and performance overlays.
/// Manages cursor state and handles enabling/disabling of UI overlays based on user input.
/// </summary>
public class GameController : MonoBehaviour
{
    [SerializeField] private CamMove camMove; // Camera movement script
    [SerializeField] private Canvas performanceOverlay; // Canvas for performance stats overlay
    [SerializeField] private Canvas changesOptions; // Canvas for changes/options overlay
    private bool inMenu; // Flag to check if the game menu is open

    [SerializeField] private bool performanceEnabled; // Flag to enable performance overlay
    [SerializeField] private bool changesEnabled; // Flag to enable changes/options overlay

    void Start()
    {
        // Initialize cursor and UI overlays
        Cursor.lockState = CursorLockMode.Locked;
        performanceOverlay.enabled = performanceEnabled;
        changesOptions.enabled = changesEnabled;
    }

    void Update()
    {
        // Toggle game menu on Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }

        // Toggle performance overlay on R key press
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleOverlay(ref performanceEnabled, performanceOverlay);
        }

        // Toggle changes/options overlay on F key press
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleOverlay(ref changesEnabled, changesOptions);
        }
    }

    /// <summary>
    /// Toggles the game menu and updates cursor lock state and camera movement.
    /// </summary>
    private void ToggleMenu()
    {
        inMenu = !inMenu;
        Cursor.lockState = inMenu ? CursorLockMode.Confined : CursorLockMode.Locked;
        camMove.inMenu = inMenu;
        changesOptions.enabled = inMenu || changesEnabled;
        performanceOverlay.enabled = inMenu || performanceEnabled;
    }

    /// <summary>
    /// Toggles a UI overlay's enabled state and updates the corresponding flag.
    /// </summary>
    private void ToggleOverlay(ref bool flag, Canvas canvas)
    {
        flag = !flag;
        canvas.enabled = flag;
    }
}
