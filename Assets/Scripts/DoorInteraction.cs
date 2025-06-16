using UnityEngine;

public class DoorInteraction : MonoBehaviour
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

        Ray ray = new Ray(checkOrigin.position, checkOrigin.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            Door door = hit.collider.GetComponentInParent<Door>();
            LockedDoor lockedDoor = hit.collider.GetComponentInParent<LockedDoor>();

            if (door != null)
            {
                bool isOpen = door.IsOpen();

                interactPromptOpen.SetActive(!isOpen);
                interactPromptClose.SetActive(isOpen);
                interactPromptLocked.SetActive(false);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    door.Interact();
                }

                return;
            }
            else if (lockedDoor != null)
            {
                PlayerInventory inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInventory>();
                bool canOpen = inventory != null && inventory.HasKeycard();

                if (canOpen)
                {
                    bool isOpen = lockedDoor.IsOpen();
                    interactPromptOpen.SetActive(!isOpen);
                    interactPromptClose.SetActive(isOpen);
                    interactPromptLocked.SetActive(false);
                }
                else
                {
                    interactPromptOpen.SetActive(false);
                    interactPromptClose.SetActive(false);
                    interactPromptLocked.SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (canOpen)
                    {
                        lockedDoor.Interact();
                    }
                    else if (lockedMessagePanel != null)
                    {
                        lockedMessagePanel.SetActive(true);
                        lockedMessageTimer = 1.2f;
                    }
                }

                return;
            }
        }

        interactPromptOpen.SetActive(false);
        interactPromptClose.SetActive(false);
        interactPromptLocked.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        if (checkOrigin == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(checkOrigin.position, checkOrigin.position + checkOrigin.forward * interactDistance);
    }
}
