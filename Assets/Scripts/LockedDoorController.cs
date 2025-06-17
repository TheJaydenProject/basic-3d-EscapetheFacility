using UnityEngine;

/// <summary>
/// Controls a locked door that opens only if the player has a keycard.
/// Automatically closes if enabled, with smooth rotation and directional logic.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 16/06/2025
 * Description: This script rotates a door open or closed based on the player's interaction.
 * It checks if the player has a keycard before allowing the door to open.
 * Supports auto-close, and includes debug logs for testing and fail-safes.
 */
public class LockedDoorController : MonoBehaviour
{
    [Header("Door Settings")]

    /// <summary>
    /// The pivot transform that the door rotates around.
    /// </summary>
    [Tooltip("Pivot point used to rotate the door")]
    public Transform pivot;

    /// <summary>
    /// How smooth the door rotates between open and closed states.
    /// </summary>
    [Tooltip("Smoothness of door rotation")]
    public float smooth = 10f;

    /// <summary>
    /// Whether the door should automatically close after being opened.
    /// </summary>
    [Tooltip("Whether the door should auto-close after a delay")]
    public bool autoClose = false;

    /// <summary>
    /// Time in seconds before the door auto-closes (if enabled).
    /// </summary>
    [Tooltip("Time before the door auto-closes")]
    public float autoCloseTime = 5f;

    /// <summary>
    /// Target Y-axis rotation for the door when opening or closing.
    /// </summary>
    private float targetYRotation = 0f;

    /// <summary>
    /// Default Y rotation of the pivot when the door is closed.
    /// </summary>
    private float defaultYRotation = 0f;

    /// <summary>
    /// Timer countdown for auto-close behavior.
    /// </summary>
    private float timer = 0f;

    /// <summary>
    /// Flag indicating whether the door is currently open.
    /// </summary>
    private bool isOpen = false;

    /// <summary>
    /// Reference to the player's transform (used for rotation logic).
    /// </summary>
    private Transform player;

    /// <summary>
    /// Reference to the player's inventory to check for keycard access.
    /// </summary>
    private PlayerInventory inventory;

    void Start()
    {
        // Attempt to locate the player via tag and store transform
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogWarning("[LockedDoor] Player not found via tag!");

        // Try to get PlayerInventory from the player object
        inventory = player?.GetComponent<PlayerInventory>();
        if (inventory == null)
            Debug.LogWarning("[LockedDoor] PlayerInventory not found on player.");

        // Validate pivot assignment
        if (pivot == null)
        {
            Debug.LogError("[LockedDoor] Door pivot is not assigned! Please assign it in the Inspector.");
            return;
        }

        // Store default rotation and initialize
        defaultYRotation = pivot.localEulerAngles.y;
        pivot.localRotation = Quaternion.Euler(0f, defaultYRotation, 0f);
        targetYRotation = 0f;
    }

    void Update()
    {
        // Smoothly rotate the pivot toward the desired rotation
        if (pivot != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, defaultYRotation + targetYRotation, 0f);
            pivot.rotation = Quaternion.Lerp(pivot.rotation, targetRotation, smooth * Time.deltaTime);
        }

        // Handle auto-close if enabled
        if (autoClose && isOpen)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                Debug.Log("[LockedDoor] Auto-closing door: " + gameObject.name);
                Close();
            }
        }
    }

    /// <summary>
    /// Called by the player to interact with the door.
    /// Only opens if the player has the keycard in their inventory.
    /// </summary>
    public void Interact()
    {
        if (inventory != null && inventory.HasKeycard())
        {
            Debug.Log("[LockedDoor] Interacted with: Keycard present. Toggling door.");
            ToggleDoor(player.position);
        }
        else
        {
            Debug.Log("[LockedDoor] Interacted with: Door is locked. Keycard required.");
        }
    }

    /// <summary>
    /// Returns a string describing the current door state for UI prompts.
    /// </summary>
    public string GetDescription()
    {
        if (inventory != null && inventory.HasKeycard())
        {
            return isOpen ? "Close the door" : "Open the door";
        }
        else
        {
            return "Locked";
        }
    }

    /// <summary>
    /// Toggles the door open or closed based on current state.
    /// </summary>
    /// <param name="playerPosition">Position of the player to determine door swing direction.</param>
    public void ToggleDoor(Vector3 playerPosition)
    {
        if (isOpen)
        {
            Close(); // Close the door
            Debug.Log("[LockedDoor] Door closed.");
        }
        else
        {
            Open(playerPosition); // Open the door
            Debug.Log("[LockedDoor] Door opened.");
        }

        isOpen = !isOpen; // Flip state flag
    }

    /// <summary>
    /// Opens the door, swinging it based on which side the player is on.
    /// </summary>
    /// <param name="playerPosition">Player position used to determine door swing direction.</param>
    public void Open(Vector3 playerPosition)
    {
        if (pivot == null)
        {
            Debug.LogWarning("[LockedDoor] Cannot open: pivot is null.");
            return;
        }

        // Determine if player is in front or behind the door
        Vector3 toPlayer = (playerPosition - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, toPlayer);

        // Choose rotation direction based on relative position
        targetYRotation = (dot >= 0f) ? -90f : 90f;

        // Reset timer for auto-close countdown
        timer = autoCloseTime;

        Debug.Log("[Door] Door opened.");
    }

    /// <summary>
    /// Closes the door by resetting rotation.
    /// </summary>
    public void Close()
    {
        targetYRotation = 0f; // Reset to closed angle
        isOpen = false;       // Update state
        timer = 0f;           // Stop countdown

        Debug.Log("[LockedDoor] Door is now closed.");
    }

    /// <summary>
    /// Returns whether the door is currently open.
    /// </summary>
    public bool IsOpen()
    {
        return isOpen;
    }
}
