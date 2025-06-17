using UnityEngine;

/// <summary>
/// Allows the player to collect a gas mask and adds it to their inventory.
/// Shows a UI panel temporarily and logs the interaction for debugging.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 16/06/2025
 * Description: Handles interaction logic for collecting a gas mask object in the scene.
 * Updates inventory, shows a temporary confirmation UI, and removes the object from the world.
 */
public class GasMaskPickupHandler : MonoBehaviour
{
    /// <summary>
    /// UI panel displayed when the gas mask is collected by the player.
    /// </summary>
    [Tooltip("UI panel shown when gas mask is collected")]
    public GameObject gasMaskCollectedPanel;

    /// <summary>
    /// Time in seconds the UI panel should stay visible on screen.
    /// </summary>
    [Tooltip("How long the collected panel stays visible")]
    public float displayDuration = 3f;

    /// <summary>
    /// Called when the player interacts with the gas mask (e.g., via raycast + 'E' key).
    /// Adds the gas mask to inventory, shows a temporary UI, and removes this object.
    /// </summary>
    public void Interact()
    {
        // Attempt to find the player's inventory component by tag
        PlayerInventory inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            // Add gas mask to player's inventory
            inventory.GiveGasMask();

            // Show confirmation panel if assigned in the inspector
            if (gasMaskCollectedPanel != null)
            {
                gasMaskCollectedPanel.SetActive(true);

                // Automatically hide panel after the set display duration
                Invoke(nameof(HidePanel), displayDuration);
            }

            // Destroy this pickup object from the scene after interaction
            Destroy(gameObject);
        }
        else
        {
            // Fallback log if player inventory couldn't be found
            Debug.LogWarning("[GasMaskPickup] Could not find PlayerInventory component.");
        }
    }

    /// <summary>
    /// Hides the gas mask collected UI panel after it has been shown.
    /// Called automatically via Invoke().
    /// </summary>
    private void HidePanel()
    {
        if (gasMaskCollectedPanel != null)
            gasMaskCollectedPanel.SetActive(false);
    }
}
