using UnityEngine;

public class Door : MonoBehaviour
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

    public bool IsOpen()
    {
        return isOpen;
    }

    private float targetYRotation = 0f;
    private float defaultYRotation = 0f;
    private float timer = 0f;
    private bool isOpen = false;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogWarning("No GameObject tagged 'Player' found.");

        if (pivot == null)
            Debug.LogError("Pivot not assigned.");

        defaultYRotation = pivot.localEulerAngles.y;
        pivot.localRotation = Quaternion.Euler(0f, defaultYRotation, 0f);
        targetYRotation = 0f;
    }

    void Update()
    {
        if (pivot != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, defaultYRotation + targetYRotation, 0f);
            pivot.rotation = Quaternion.Lerp(pivot.rotation, targetRotation, smooth * Time.deltaTime);
        }

        if (autoClose && isOpen)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f && player != null)
                Close();
        }
    }

    public void Interact()
    {
        if (player != null)
            ToggleDoor(player.position);

        Debug.Log("Door interacted.");
    }

    public string GetDescription()
    {
        return isOpen ? "Close the door" : "Open the door";
    }

    public void ToggleDoor(Vector3 pos)
    {
        if (isOpen)
            Close();
        else
            Open(pos);

        isOpen = !isOpen;
    }

    public void Open(Vector3 pos)
    {
        Vector3 toPlayer = (pos - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, toPlayer);

        targetYRotation = (dot >= 0f) ? -90f : 90f;
        timer = autoCloseTime;
    }

    public void Close()
    {
        targetYRotation = 0f;
        isOpen = false;
        timer = 0f;
    }
}
