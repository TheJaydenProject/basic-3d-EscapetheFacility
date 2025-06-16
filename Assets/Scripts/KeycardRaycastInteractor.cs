using UnityEngine;

/// <summary>
/// Handles raycast-based interaction for picking up a keycard.
/// Displays UI prompts and tracks if the keycard has been collected.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 6/16/2025
 * Description: This script allows the player to detect and pick up a keycard using raycasting.
 * It shows a prompt when looking at a keycard, calls the pickup logic on interaction,
 * and toggles a "keycard acquired" panel based on inventory state.
 */
public class KeycardRaycastInteractor : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Maximum distance to interact with keycards")]
    public float interactDistance = 2f;

    [Header("References")]
    [Tooltip("Point from which the raycast is fired (usually the camera)")]
    public Transform checkOrigin;

    [Tooltip("UI shown when player is looking at a keycard")]
    public GameObject keycardPromptPanel;

    [Tooltip("UI shown when player has collected the keycard")]
    public GameObject keycardAcquiredPanel;

    private KeycardPickupHandler currentKeycard;
    private PlayerInventory inventory;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>();

        if (inventory == null)
            Debug.LogWarning("[KeycardInteractor] PlayerInventory not found on Player object.");
    }

    void Update()
    {
        // Exit early if critical references are missing
        if (checkOrigin == null || keycardPromptPanel == null || inventory == null)
        {
            Debug.LogWarning("[KeycardInteractor] Missing reference(s): checkOrigin, promptPanel, or inventory.");
            return;
        }

        // Cast ray forward
        Ray ray = new Ray(checkOrigin.position, checkOrigin.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.CompareTag("Keycard"))
            {
                KeycardPickupHandler pickup = hit.collider.GetComponent<KeycardPickupHandler>();

                if (pickup != null && pickup != currentKeycard)
                {
                    HidePrompt();
                    currentKeycard = pickup;
                    keycardPromptPanel.SetActive(true);
                    Debug.Log("[KeycardInteractor] Looking at keycard: " + pickup.name);
                }

                // Player presses E to pick up
                if (Input.GetKeyDown(KeyCode.E) && currentKeycard != null)
                {
                    pickup.Interact();
                    currentKeycard = null;
                    keycardPromptPanel.SetActive(false);
                    Debug.Log($"[KeycardInteractor] Keycard collected and added to inventory: {pickup.name}");
                }

                return;
            }
        }

        // No keycard hit
        HidePrompt();
        currentKeycard = null;

        // Update keycard acquired panel
        if (keycardAcquiredPanel != null)
        {
            bool hasCard = inventory.HasKeycard();
            keycardAcquiredPanel.SetActive(hasCard);
        }
    }

    /// <summary>
    /// Hides the prompt panel if active.
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
