using UnityEngine;

/// <summary>
/// Allows the player to collect a keycard and adds it to their inventory.
/// Shows a UI panel temporarily and logs the interaction for debugging.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 16/06/2025
 * Description: Handles interaction logic for collecting a keycard object in the scene.
 * Updates inventory, shows a temporary prompt confirmation UI, and removes the object from the world.
 */
public class KeycardPickupHandler : MonoBehaviour
{
    /// <summary>
    /// UI panel that is displayed when the keycard is collected by the player.
    /// </summary>
    [Tooltip("UI panel shown when keycard is collected")]
    public GameObject keycardCollectedPanel;

    /// <summary>
    /// How long (in seconds) the keycard collected panel stays on screen.
    /// </summary>
    [Tooltip("How long the collected panel stays visible")]
    public float displayDuration = 3f;

    /// <summary>
    /// Called when the player interacts with the keycard (via raycast + 'E' key).
    /// Gives the player a keycard, shows a visual confirmation, and removes the object from the scene.
    /// </summary>
    public void Interact()
    {
        // Attempt to find the PlayerInventory script on the GameObject tagged as "Player"
        PlayerInventory inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            // Add keycard to the player's inventory
            inventory.GiveKeycard();

            // Display confirmation UI if it's set
            if (keycardCollectedPanel != null)
            {
                keycardCollectedPanel.SetActive(true); // Show the collected message
                Invoke(nameof(HidePanel), displayDuration); // Schedule hiding the panel after delay
            }

            // Remove the keycard from the game world
            Destroy(gameObject);
        }
        else
        {
            // Warn in console if PlayerInventory is missing
            Debug.LogWarning("[KeycardPickup] Could not find PlayerInventory component.");
        }
    }

    /// <summary>
    /// Hides the collected panel after the display duration ends.
    /// </summary>
    private void HidePanel()
    {
        if (keycardCollectedPanel != null)
            keycardCollectedPanel.SetActive(false); // Hide the UI panel
    }
}
