using UnityEngine;

/// <summary>
/// Allows the player to collect a keycard and adds it to their inventory.
/// Shows a UI panel temporarily and logs the interaction for debugging.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 6/16/2025
 * Description: Handles interaction logic for collecting a keycard object in the scene.
 * Updates inventory, shows a temporary confirmation UI, and removes the object from the world.
 */
public class KeycardPickupHandler : MonoBehaviour
{
    [Tooltip("UI panel shown when keycard is collected")]
    public GameObject keycardCollectedPanel;

    [Tooltip("How long the collected panel stays visible")]
    public float displayDuration = 3f;

    /// <summary>
    /// Called when the player interacts with the keycard (e.g., via raycast + 'E' key).
    /// Adds the keycard to inventory, shows a temporary UI, and removes this object.
    /// </summary>
    public void Interact()
    {
        // Attempt to find PlayerInventory on player
        PlayerInventory inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            // Give the keycard to inventory
            inventory.GiveKeycard();

            // Show the 'collected' panel if assigned
            if (keycardCollectedPanel != null)
            {
                keycardCollectedPanel.SetActive(true);
                Invoke(nameof(HidePanel), displayDuration);
            }

            // Remove the keycard object from the scene
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("[KeycardPickup] Could not find PlayerInventory component.");
        }
    }

    /// <summary>
    /// Hides the collected panel after the display duration ends.
    /// </summary>
    private void HidePanel()
    {
        if (keycardCollectedPanel != null)
            keycardCollectedPanel.SetActive(false);
    }
}
