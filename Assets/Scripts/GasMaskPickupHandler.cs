using UnityEngine;

/// <summary>
/// Allows the player to collect a gas mask and adds it to their inventory.
/// Shows a UI panel temporarily and logs the interaction for debugging.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 6/16/2025
 * Description: Handles interaction logic for collecting a gas mask object in the scene.
 * Updates inventory, shows a temporary confirmation UI, and removes the object from the world.
 */
public class GasMaskPickupHandler : MonoBehaviour
{
    [Tooltip("UI panel shown when gas mask is collected")]
    public GameObject gasMaskCollectedPanel;

    [Tooltip("How long the collected panel stays visible")]
    public float displayDuration = 3f;

    /// <summary>
    /// Called when the player interacts with the gas mask (e.g., via raycast + 'E' key).
    /// Adds the gas mask to inventory, shows a temporary UI, and removes this object.
    /// </summary>
    public void Interact()
    {
        PlayerInventory inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.GiveGasMask();

            if (gasMaskCollectedPanel != null)
            {
                gasMaskCollectedPanel.SetActive(true);
                Invoke(nameof(HidePanel), displayDuration);
            }

            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("[GasMaskPickup] Could not find PlayerInventory component.");
        }
    }

    /// <summary>
    /// Hides the collected panel after the display duration ends.
    /// </summary>
    private void HidePanel()
    {
        if (gasMaskCollectedPanel != null)
            gasMaskCollectedPanel.SetActive(false);
    }
}
