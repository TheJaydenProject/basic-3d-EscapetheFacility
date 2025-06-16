using UnityEngine;

/// <summary>
/// Handles player interaction with normal and locked doors using raycasting.
/// Displays appropriate UI prompts and messages, and logs key events for debugging.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 6/16/2025
 * Description: This script detects if the player is looking at a door using a raycast,
 * displays the correct UI prompt (Open, Close, Locked), and allows interaction using 'E'.
 * Also handles debug logs to confirm door state changes and access results.
 */
public class PlayerDoorInteractor : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Maximum distance to interact with doors")]
    public float interactDistance = 2f;

    [Header("References")]
    [Tooltip("Ray origin point (usually the camera or player head)")]
    public Transform checkOrigin;

    [Tooltip("UI panel shown when door is closed and can be opened")]
    public GameObject interactPromptOpen;

    [Tooltip("UI panel shown when door is open and can be closed")]
    public GameObject interactPromptClose;

    [Tooltip("UI panel shown when door is locked and cannot be opened")]
    public GameObject interactPromptLocked;

    [Tooltip("Temporary message panel that appears when door is locked and keycard is missing")]
    public GameObject lockedMessagePanel;

    // Timer to hide locked message after a delay
    private float lockedMessageTimer = 0f;

    void Update()
    {
        if (checkOrigin == null || interactPromptOpen == null || interactPromptClose == null || interactPromptLocked == null)
            return;

        // Handle auto-hide for locked message
        if (lockedMessagePanel != null && lockedMessagePanel.activeSelf)
        {
            lockedMessageTimer -= Time.deltaTime;
            if (lockedMessageTimer <= 0f)
            {
                lockedMessagePanel.SetActive(false);
            }
        }

        // Create a ray from the checkOrigin point forward
        Ray ray = new Ray(checkOrigin.position, checkOrigin.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            BasicDoorController door = hit.collider.GetComponentInParent<BasicDoorController>();
            LockedDoorController lockedDoor = hit.collider.GetComponentInParent<LockedDoorController>();

            // Regular door logic
            if (door != null)
            {
                bool isOpen = door.IsOpen();

                // Show correct UI prompt
                interactPromptOpen.SetActive(!isOpen);
                interactPromptClose.SetActive(isOpen);
                interactPromptLocked.SetActive(false);

                // Handle input
                if (Input.GetKeyDown(KeyCode.E))
                {
                    door.Interact();

                    if (isOpen)
                        Debug.Log("[DoorInteractor] Closed door: " + door.name);
                    else
                        Debug.Log("[DoorInteractor] Opened door: " + door.name);
                }

                return;
            }

            // Locked door logic
            else if (lockedDoor != null)
            {
                PlayerInventory inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>();
                bool hasKeycard = inventory != null && inventory.HasKeycard();
                bool isOpen = lockedDoor.IsOpen();

                // Update UI prompts
                interactPromptOpen.SetActive(hasKeycard && !isOpen);
                interactPromptClose.SetActive(hasKeycard && isOpen);
                interactPromptLocked.SetActive(!hasKeycard);

                // Handle input
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (hasKeycard)
                    {
                        lockedDoor.Interact();

                        if (isOpen)
                            Debug.Log("[DoorInteractor] Closed locked door (with keycard): " + lockedDoor.name);
                        else
                            Debug.Log("[DoorInteractor] Opened locked door (with keycard): " + lockedDoor.name);
                    }
                    else
                    {
                        Debug.Log("[DoorInteractor] Door is locked and player has no keycard: " + lockedDoor.name);

                        if (lockedMessagePanel != null)
                        {
                            lockedMessagePanel.SetActive(true);
                            lockedMessageTimer = 1.2f;
                        }
                    }
                }

                return;
            }
        }

        // Hide all prompts if nothing interactable
        interactPromptOpen.SetActive(false);
        interactPromptClose.SetActive(false);
        interactPromptLocked.SetActive(false);
    }

    /// <summary>
    /// Visualize ray direction in Scene view when selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (checkOrigin == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(checkOrigin.position, checkOrigin.position + checkOrigin.forward * interactDistance);
    }
}
