using UnityEngine;

/// <summary>
/// Manages the onboarding overlay and enables gameplay after user clicks Start.
/// Disables player input on launch and restores it when the Start button is pressed.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 16/06/2025
 * Description: Displays the start overlay at launch, disables movement/look controls,
 * and re-enables them when the player presses the Start button. Also manages cursor visibility and locking.
 */
public class StartOverlayController : MonoBehaviour
{
    [Header("UI")]

    /// <summary>
    /// UI panel displayed at the beginning of the game as an onboarding screen.
    /// </summary>
    public GameObject overlayPanel;

    [Header("Disable Movement")]

    /// <summary>
    /// Script that controls player movement (e.g. FirstPersonController).
    /// Will be disabled on start and re-enabled when game begins.
    /// </summary>
    public MonoBehaviour movementScript;

    [Header("Disable Look")]

    /// <summary>
    /// Script that handles camera look input (e.g. CinemachineInputProvider).
    /// Disabled at the start until the player begins the game.
    /// </summary>
    public MonoBehaviour lookScript;

    /// <summary>
    /// Called on scene start. Disables movement and look input, shows UI, and unlocks cursor.
    /// </summary>
    private void Start()
    {
        // Show the onboarding overlay panel
        if (overlayPanel != null)
            overlayPanel.SetActive(true);

        // Disable movement script at launch
        if (movementScript != null)
            movementScript.enabled = false;

        // Disable look input script at launch
        if (lookScript != null)
            lookScript.enabled = false;

        // Unlock and show the cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Called when the player clicks the Start button.
    /// Hides the overlay and enables player control and camera look.
    /// </summary>
    public void StartGame()
    {
        // Hide the onboarding UI
        if (overlayPanel != null)
            overlayPanel.SetActive(false);

        // Enable player movement
        if (movementScript != null)
            movementScript.enabled = true;

        // Enable camera look control
        if (lookScript != null)
            lookScript.enabled = true;

        // Lock and hide the cursor for gameplay immersion
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Game started.");
    }
}
