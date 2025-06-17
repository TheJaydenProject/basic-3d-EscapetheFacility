using UnityEngine;

/// <summary>
/// Handles raycast-based interaction for picking up a gas mask.
/// Displays UI prompts when looking at the gas mask, and shows the acquired status.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 16/06/2025
 * Description: This script allows the player to detect and pick up a gas mask using raycasting.
 * It shows a prompt when looking at the gas mask, calls the pickup logic on interaction,
 * and toggles a "gas mask acquired" panel based on inventory state.
 */
public class GasMaskRaycastInteractor : MonoBehaviour
{
    [Header("Settings")]

    /// <summary>
    /// Maximum distance allowed for interacting with the gas mask.
    /// </summary>
    [Tooltip("Maximum distance to interact with gas masks")]
    public float interactDistance = 2f;

    [Header("References")]

    /// <summary>
    /// Transform from where the raycast is fired (typically the player camera).
    /// </summary>
    [Tooltip("Point from which the raycast is fired (usually the camera)")]
    public Transform checkOrigin;

    /// <summary>
    /// UI panel displayed when the player is aiming at a gas mask.
    /// </summary>
    [Tooltip("UI shown when player is looking at a gas mask")]
    public GameObject gasMaskPromptPanel;

    /// <summary>
    /// UI panel that indicates the gas mask has been collected.
    /// </summary>
    [Tooltip("UI shown when player has collected the gas mask")]
    public GameObject gasMaskAcquiredPanel;

    /// <summary>
    /// The currently targeted gas mask pickup in view.
    /// </summary>
    private GasMaskPickupHandler currentGasMask;

    /// <summary>
    /// Reference to the player's inventory script.
    /// </summary>
    private PlayerInventory inventory;

    void Start()
    {
        // Attempt to get the PlayerInventory from the object tagged "Player"
        inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>();

        if (inventory == null)
            Debug.LogWarning("[GasMaskInteractor] PlayerInventory not found on Player object.");
    }

    void Update()
    {
        // Early return if essential references are missing
        if (checkOrigin == null || gasMaskPromptPanel == null || inventory == null)
        {
            Debug.LogWarning("[GasMaskInteractor] Missing reference(s): checkOrigin, promptPanel, or inventory.");
            return;
        }

        // Create a ray from the camera forward direction
        Ray ray = new Ray(checkOrigin.position, checkOrigin.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.cyan); // For editor debugging

        // Perform the raycast to detect objects in front
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            // Check if the object hit has the "GasMask" tag
            if (hit.collider.CompareTag("GasMask"))
            {
                // Try to get the GasMaskPickupHandler script on the object
                GasMaskPickupHandler pickup = hit.collider.GetComponent<GasMaskPickupHandler>();

                // If a new valid gas mask is detected
                if (pickup != null && pickup != currentGasMask)
                {
                    HidePrompt(); // Hide any existing prompt
                    currentGasMask = pickup;
                    gasMaskPromptPanel.SetActive(true); // Show new prompt
                    Debug.Log("[GasMaskInteractor] Looking at gas mask: " + pickup.name);
                }

                // If the player presses 'E', pick up the gas mask
                if (Input.GetKeyDown(KeyCode.E) && currentGasMask != null)
                {
                    pickup.Interact(); // Call the pickup logic
                    currentGasMask = null;
                    gasMaskPromptPanel.SetActive(false); // Hide prompt after pickup
                    Debug.Log($"[GasMaskInteractor] Gas mask collected and added to inventory: {pickup.name}");
                }

                return; // Exit early to prevent prompt being hidden below
            }
        }

        // If not looking at a gas mask, hide prompt and reset
        HidePrompt();
        currentGasMask = null;

        // Toggle the acquired panel based on whether the player owns a gas mask
        if (gasMaskAcquiredPanel != null)
        {
            bool hasMask = inventory.HasGasMask();
            gasMaskAcquiredPanel.SetActive(hasMask);
        }
    }

    /// <summary>
    /// Hides the gas mask interaction prompt if it is active.
    /// </summary>
    void HidePrompt()
    {
        if (gasMaskPromptPanel != null && gasMaskPromptPanel.activeSelf)
        {
            gasMaskPromptPanel.SetActive(false);
            Debug.Log("[GasMaskInteractor] Prompt hidden.");
        }
    }
}
