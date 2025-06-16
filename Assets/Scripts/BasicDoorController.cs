using UnityEngine;

/// <summary>
/// Controls a regular (non-locked) door that the player can open or close via interaction.
/// Includes optional auto-close, directional opening logic, and debug logs for tracing.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 6/16/2025
 * Description: Rotates the door open or closed when the player interacts.
 * Auto-closes after a delay if enabled. Opening direction depends on player position.
 */
public class BasicDoorController : MonoBehaviour
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

    /// <summary>
    /// Returns whether the door is currently open.
    /// </summary>
    public bool IsOpen()
    {
        return isOpen;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
            Debug.LogWarning("[Door] No GameObject tagged 'Player' found!");

        if (pivot == null)
        {
            Debug.LogError("[Door] Pivot not assigned! Please assign the pivot transform in the Inspector.");
            return;
        }

        // Initialize door rotation
        defaultYRotation = pivot.localEulerAngles.y;
        pivot.localRotation = Quaternion.Euler(0f, defaultYRotation, 0f);
        targetYRotation = 0f;
    }

    void Update()
    {
        // Smooth rotation
        if (pivot != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, defaultYRotation + targetYRotation, 0f);
            pivot.rotation = Quaternion.Lerp(pivot.rotation, targetRotation, smooth * Time.deltaTime);
        }

        // Handle auto-close countdown
        if (autoClose && isOpen)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f && player != null)
            {
                Debug.Log("[Door] Auto-closing door: " + gameObject.name);
                Close();
            }
        }
    }

    /// <summary>
    /// Triggered by player interaction (e.g., pressing E).
    /// </summary>
    public void Interact()
    {
        if (player != null)
        {
            ToggleDoor(player.position);
            Debug.Log("[Door] Interacted with door: " + gameObject.name);
        }
        else
        {
            Debug.LogWarning("[Door] Interact called but Player reference is missing.");
        }
    }

    /// <summary>
    /// Provides UI description of what this door will do.
    /// </summary>
    public string GetDescription()
    {
        return isOpen ? "Close the door" : "Open the door";
    }

    /// <summary>
    /// Toggles the door open/closed state.
    /// </summary>
    public void ToggleDoor(Vector3 playerPos)
    {
        if (isOpen)
        {
            Close();
            Debug.Log("[Door] Door closed.");
        }
        else
        {
            Open(playerPos);
            Debug.Log("[Door] Door opened.");
        }

        isOpen = !isOpen;
    }

    /// <summary>
    /// Opens the door based on player's position (left or right swing).
    /// </summary>
    public void Open(Vector3 playerPos)
    {
        if (pivot == null)
        {
            Debug.LogWarning("[Door] Cannot open: pivot is null.");
            return;
        }

        Vector3 toPlayer = (playerPos - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, toPlayer);

        targetYRotation = (dot >= 0f) ? -90f : 90f;
        timer = autoCloseTime;

        Debug.Log("[Door] Door opened.");
    }

    /// <summary>
    /// Closes the door back to default rotation.
    /// </summary>
    public void Close()
    {
        targetYRotation = 0f;
        isOpen = false;
        timer = 0f;

        Debug.Log("[Door] Door is now closed.");
    }
}
