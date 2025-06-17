using UnityEngine;

/// <summary>
/// Handles raycast-based interaction for picking up a keycard.
/// Displays UI prompts and tracks if the keycard has been collected.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 16/06/2025
 * Description: This script allows the player to detect and pick up a keycard using raycasting.
 * It shows a prompt when looking at a keycard, calls the pickup logic on interaction,
 * and toggles a "keycard acquired" panel based on inventory state.
 */
public class KeycardRaycastInteractor : MonoBehaviour
{
    [Header("Settings")]

    /// <summary>
    /// Maximum distance from which the player can interact with the keycard.
    /// </summary>
    [Tooltip("Maximum distance to interact with keycards")]
    public float interactDistance = 2f;

    [Header("References")]

    /// <summary>
    /// The origin point from which the raycast is fired (usually the player camera).
    /// </summary>
    [Tooltip("Point from which the raycast is fired (usually the camera)")]
    public Transform checkOrigin;

    /// <summary>
    /// UI panel displayed when the player is looking at a keycard.
    /// </summary>
    [Tooltip("UI shown when player is looking at a keycard")]
    public GameObject keycardPromptPanel;

    /// <summary>
    /// UI panel shown once the player has collected the keycard.
    /// </summary>
    [Tooltip("UI shown when player has collected the keycard")]
    public GameObject keycardAcquiredPanel;

    /// <summary>
    /// Tracks the keycard object the player is currently targeting.
    /// </summary>
    private KeycardPickupHandler currentKeycard;

    /// <summary>
    /// Reference to the player's inventory system to check for keycard possession.
    /// </summary>
    private PlayerInventory inventory;

    void Start()
    {
        // Find the player's inventory by tag and store the reference
        inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>();

        if (inventory == null)
            Debug.LogWarning("[KeycardInteractor] PlayerInventory not found on Player object.");
    }

    void Update()
    {
        // Early exit if critical references are missing to prevent errors
        if (checkOrigin == null || keycardPromptPanel == null || inventory == null)
        {
            Debug.LogWarning("[KeycardInteractor] Missing reference(s): checkOrigin, promptPanel, or inventory.");
            return;
        }

        // Create a ray pointing forward from the origin point
        Ray ray = new Ray(checkOrigin.position, checkOrigin.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.green); // Editor debug ray

        // Perform a raycast to check if the player is aiming at a keycard
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.CompareTag("Keycard"))
            {
                // Get the KeycardPickupHandler from the hit object
                KeycardPickupHandler pickup = hit.collider.GetComponent<KeycardPickupHandler>();

                // If the target has changed, update reference and show prompt
                if (pickup != null && pickup != currentKeycard)
                {
                    HidePrompt(); // Hide any previously active prompt
                    currentKeycard = pickup;
                    keycardPromptPanel.SetActive(true); // Show new prompt
                    Debug.Log("[KeycardInteractor] Looking at keycard: " + pickup.name);
                }

                // If the player presses E, collect the keycard
                if (Input.GetKeyDown(KeyCode.E) && currentKeycard != null)
                {
                    pickup.Interact(); // Trigger pickup logic
                    currentKeycard = null; // Reset reference
                    keycardPromptPanel.SetActive(false); // Hide prompt
                    Debug.Log($"[KeycardInteractor] Keycard collected and added to inventory: {pickup.name}");
                }

                return; // Early exit if keycard was hit
            }
        }

        // If raycast didn't hit a keycard, hide prompt and reset reference
        HidePrompt();
        currentKeycard = null;

        // Update the acquired panel based on whether the player owns the keycard
        if (keycardAcquiredPanel != null)
        {
            bool hasCard = inventory.HasKeycard();
            keycardAcquiredPanel.SetActive(hasCard);
        }
    }

    /// <summary>
    /// Hides the keycard prompt UI if it is currently active.
    /// </summary>
    void HidePrompt()
    {
        if (keycardPromptPanel != null && keycardPromptPanel.activeSelf)
        {
            keycardPromptPanel.SetActive(false);
            Debug.Log("[KeycardInteractor] Prompt hidden.");
        }
    }
}
