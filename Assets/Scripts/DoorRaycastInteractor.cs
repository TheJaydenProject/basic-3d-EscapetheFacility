using UnityEngine;

/// <summary>
/// Handles player interaction with normal, locked, and end doors using raycasting.
/// Displays appropriate UI prompts and messages, and logs key events for debugging.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 16/06/2025
 * Description: Detects and interacts with regular, locked, and end doors using a raycast.
 * Shows specific UI prompts for each type and handles 'E' key interactions.
 */

public class PlayerDoorInteractor : MonoBehaviour
{
    [Header("Settings")]

    /// <summary>
    /// Maximum distance the player can interact with a door via raycast.
    /// </summary>
    [Tooltip("Maximum distance to interact with doors")]
    public float interactDistance = 2f;

    [Header("References")]

    /// <summary>
    /// Origin point from which the raycast will be cast (usually the player camera).
    /// </summary>
    public Transform checkOrigin;

    /// <summary>
    /// UI prompt shown when a door can be opened.
    /// </summary>
    public GameObject interactPromptOpen;

    /// <summary>
    /// UI prompt shown when a door can be closed.
    /// </summary>
    public GameObject interactPromptClose;

    /// <summary>
    /// UI prompt shown when a door is locked and cannot be opened.
    /// </summary>
    public GameObject interactPromptLocked;

    /// <summary>
    /// UI prompt shown when the player reaches the final escape door.
    /// </summary>
    public GameObject interactPromptEscape;

    /// <summary>
    /// Panel that briefly shows a locked door message when the player lacks a keycard.
    /// </summary>
    public GameObject lockedMessagePanel;

    /// <summary>
    /// Timer for how long the locked message panel stays visible.
    /// </summary>
    private float lockedMessageTimer = 0f;

    void Update()
    {
        // Exit if no origin for raycast is set (e.g., missing reference)
        if (checkOrigin == null) return;

        // Handle timer-based hiding of the locked door message panel
        if (lockedMessagePanel != null && lockedMessagePanel.activeSelf)
        {
            lockedMessageTimer -= Time.unscaledDeltaTime;

            // Hide the message when the timer runs out
            if (lockedMessageTimer <= 0f)
                lockedMessagePanel.SetActive(false);
        }

        // Create a ray from the origin forward to detect interactable objects
        Ray ray = new Ray(checkOrigin.position, checkOrigin.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red); // Debug line in editor
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // Try to get the relevant door or trigger component from the object hit
            BasicDoorController door = hit.collider.GetComponentInParent<BasicDoorController>();
            LockedDoorController lockedDoor = hit.collider.GetComponentInParent<LockedDoorController>();
            EndGameTrigger endTrigger = hit.collider.GetComponentInParent<EndGameTrigger>();

            // If a basic door was hit, handle open/close interaction
            if (door != null)
            {
                bool isOpen = door.IsOpen();

                // Show appropriate prompt depending on door state
                SetPromptStates(!isOpen, isOpen, false, false);

                // If 'E' is pressed, toggle the door state
                if (Input.GetKeyDown(KeyCode.E))
                {
                    door.Interact();
                    Debug.Log(isOpen ? "[DoorInteractor] Closed door" : "[DoorInteractor] Opened door");
                }

                return;
            }

            // If a locked door was hit, check inventory for keycard before allowing interaction
            if (lockedDoor != null)
            {
                var inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>();
                bool hasKeycard = inventory != null && inventory.HasKeycard();
                bool isOpen = lockedDoor.IsOpen();

                // Show prompts based on lock status and possession of keycard
                SetPromptStates(hasKeycard && !isOpen, hasKeycard && isOpen, !hasKeycard, false);

                // Handle interaction input
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (hasKeycard)
                    {
                        lockedDoor.Interact();
                        Debug.Log(isOpen ? "[DoorInteractor] Closed locked door" : "[DoorInteractor] Opened locked door");
                    }
                    else if (lockedMessagePanel != null)
                    {
                        // Show warning panel if keycard is missing
                        lockedMessagePanel.SetActive(true);
                        lockedMessageTimer = 1.2f;
                    }
                }

                return;
            }

            // If the hit object is tagged as an end trigger, show escape prompt
            if (hit.collider.CompareTag("End") && endTrigger != null)
            {
                SetPromptStates(false, false, false, true);

                // Trigger end game if 'E' is pressed
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactPromptEscape.SetActive(false); // Hide escape UI
                    endTrigger.TriggerEndGame();           // Start end game sequence
                    Debug.Log("[DoorInteractor] Player escaped via End door");
                }

                return;
            }
        }

        // No interactable object in front, hide all prompts
        SetPromptStates(false, false, false, false);
    }

    /// <summary>
    /// Controls the visibility of different interaction prompts.
    /// </summary>
    /// <param name="open">Show open door prompt</param>
    /// <param name="close">Show close door prompt</param>
    /// <param name="locked">Show locked door prompt</param>
    /// <param name="escape">Show escape door prompt</param>
    void SetPromptStates(bool open, bool close, bool locked, bool escape)
    {
        if (interactPromptOpen != null) interactPromptOpen.SetActive(open);
        if (interactPromptClose != null) interactPromptClose.SetActive(close);
        if (interactPromptLocked != null) interactPromptLocked.SetActive(locked);
        if (interactPromptEscape != null) interactPromptEscape.SetActive(escape);
    }

    /// <summary>
    /// Visualizes the raycast in the editor when the object is selected.
    /// Helps in debugging the raycast direction and range.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (checkOrigin == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(checkOrigin.position, checkOrigin.position + checkOrigin.forward * interactDistance);
    }
}
