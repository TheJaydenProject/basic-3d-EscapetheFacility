using UnityEngine;

/// <summary>
/// Handles raycast-based interaction for picking up a gas mask.
/// Displays UI prompts and tracks if the gas mask has been collected.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 6/16/2025
 * Description: This script allows the player to detect and pick up a gas mask using raycasting.
 * It shows a prompt when looking at the gas mask, calls the pickup logic on interaction,
 * and toggles a "gas mask acquired" panel based on inventory state.
 */
public class GasMaskRaycastInteractor : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Maximum distance to interact with gas masks")]
    public float interactDistance = 2f;

    [Header("References")]
    [Tooltip("Point from which the raycast is fired (usually the camera)")]
    public Transform checkOrigin;

    [Tooltip("UI shown when player is looking at a gas mask")]
    public GameObject gasMaskPromptPanel;

    [Tooltip("UI shown when player has collected the gas mask")]
    public GameObject gasMaskAcquiredPanel;

    private GasMaskPickupHandler currentGasMask;
    private PlayerInventory inventory;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>();

        if (inventory == null)
            Debug.LogWarning("[GasMaskInteractor] PlayerInventory not found on Player object.");
    }

    void Update()
    {
        if (checkOrigin == null || gasMaskPromptPanel == null || inventory == null)
        {
            Debug.LogWarning("[GasMaskInteractor] Missing reference(s): checkOrigin, promptPanel, or inventory.");
            return;
        }

        Ray ray = new Ray(checkOrigin.position, checkOrigin.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.cyan);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.CompareTag("GasMask"))
            {
                GasMaskPickupHandler pickup = hit.collider.GetComponent<GasMaskPickupHandler>();

                if (pickup != null && pickup != currentGasMask)
                {
                    HidePrompt();
                    currentGasMask = pickup;
                    gasMaskPromptPanel.SetActive(true);
                    Debug.Log("[GasMaskInteractor] Looking at gas mask: " + pickup.name);
                }

                if (Input.GetKeyDown(KeyCode.E) && currentGasMask != null)
                {
                    pickup.Interact();
                    currentGasMask = null;
                    gasMaskPromptPanel.SetActive(false);
                    Debug.Log($"[GasMaskInteractor] Gas mask collected and added to inventory: {pickup.name}");
                }

                return;
            }
        }

        HidePrompt();
        currentGasMask = null;

        if (gasMaskAcquiredPanel != null)
        {
            bool hasMask = inventory.HasGasMask();
            gasMaskAcquiredPanel.SetActive(hasMask);
        }
    }

    void HidePrompt()
    {
        if (gasMaskPromptPanel != null && gasMaskPromptPanel.activeSelf)
        {
            gasMaskPromptPanel.SetActive(false);
            Debug.Log("[GasMaskInteractor] Prompt hidden.");
        }
    }
}
