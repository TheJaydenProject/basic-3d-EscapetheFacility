using UnityEngine;

/// <summary>
/// Controls a locked door that opens only if the player has a keycard.
/// Automatically closes if enabled, with smooth rotation and directional logic.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 6/16/2025
 * Description: This script rotates a door open or closed based on the player's interaction.
 * It checks if the player has a keycard before allowing the door to open.
 * Supports auto-close, and includes debug logs for testing and fail-safes.
 */
public class LockedDoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("Pivot point used to rotate the door")]
    public Transform pivot;

    [Tooltip("Smoothness of door rotation")]
    public float smooth = 10f;

    [Tooltip("Whether the door should auto-close after a delay")]
    public bool autoClose = false;

    [Tooltip("Time before the door auto-closes")]
    public float autoCloseTime = 5f;

    private float targetYRotation = 0f;
    private float defaultYRotation = 0f;
    private float timer = 0f;
    private bool isOpen = false;

    private Transform player;
    private PlayerInventory inventory;

    void Start()
    {
        // Get player and inventory references
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogWarning("[LockedDoor] Player not found via tag!");

        inventory = player?.GetComponent<PlayerInventory>();
        if (inventory == null)
            Debug.LogWarning("[LockedDoor] PlayerInventory not found on player.");

        if (pivot == null)
        {
            Debug.LogError("[LockedDoor] Door pivot is not assigned! Please assign it in the Inspector.");
            return;
        }

        // Set default pivot rotation
        defaultYRotation = pivot.localEulerAngles.y;
        pivot.localRotation = Quaternion.Euler(0f, defaultYRotation, 0f);
        targetYRotation = 0f;
    }

    void Update()
    {
        // Smoothly rotate door toward target rotation
        if (pivot != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, defaultYRotation + targetYRotation, 0f);
            pivot.rotation = Quaternion.Lerp(pivot.rotation, targetRotation, smooth * Time.deltaTime);
        }

        // Auto-close logic
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
    /// Called by player to interact with the door.
    /// Will only open if the player has the keycard.
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
    /// Returns a description string based on current state and player inventory.
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
    /// Toggles the door open/closed and flips state.
    /// </summary>
    public void ToggleDoor(Vector3 playerPosition)
    {
        if (isOpen)
        {
            Close();
            Debug.Log("[LockedDoor] Door closed.");
        }
        else
        {
            Open(playerPosition);
            Debug.Log("[LockedDoor] Door opened.");
        }

        isOpen = !isOpen;
    }

    /// <summary>
    /// Rotates door open in direction based on player position.
    /// </summary>
    public void Open(Vector3 playerPosition)
    {
        if (pivot == null)
        {
            Debug.LogWarning("[LockedDoor] Cannot open: pivot is null.");
            return;
        }

        Vector3 toPlayer = (playerPosition - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, toPlayer);

        targetYRotation = (dot >= 0f) ? -90f : 90f;
        timer = autoCloseTime;

        Debug.Log("[Door] Door opened.");
    }

    /// <summary>
    /// Rotates door back to closed position.
    /// </summary>
    public void Close()
    {
        targetYRotation = 0f;
        isOpen = false;
        timer = 0f;

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
