using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GasHazard : MonoBehaviour
{
    public int damageAmount = 20;
    public float damageInterval = 1f;

    [Tooltip("Green screen overlay shown when player is inside gas")]
    public GameObject gasOverlayPanel;

    private Dictionary<GameObject, Coroutine> gasCoroutines = new Dictionary<GameObject, Coroutine>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !gasCoroutines.ContainsKey(other.gameObject))
        {
            Coroutine c = StartCoroutine(DamageOverTime(other.gameObject));
            gasCoroutines.Add(other.gameObject, c);

            if (gasOverlayPanel != null)
                gasOverlayPanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && gasCoroutines.ContainsKey(other.gameObject))
        {
            StopCoroutine(gasCoroutines[other.gameObject]);
            gasCoroutines.Remove(other.gameObject);

            if (gasOverlayPanel != null)
                gasOverlayPanel.SetActive(false);
        }
    }

    IEnumerator DamageOverTime(GameObject player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        while (health != null && health.GetCurrentHealth() > 0)
        {
            // Skip damage if gas mask is equipped
            if (inventory != null && !inventory.HasGasMask())
            {
                health.TakeDamage(damageAmount, gameObject.tag);
            }

            yield return new WaitForSeconds(damageInterval);
        }

        if (gasOverlayPanel != null)
            gasOverlayPanel.SetActive(false);

        gasCoroutines.Remove(player);
    }

    public void CancelGasDamageFor(GameObject player)
    {
        if (gasCoroutines.ContainsKey(player))
        {
            StopCoroutine(gasCoroutines[player]);
            gasCoroutines.Remove(player);

            if (gasOverlayPanel != null)
                gasOverlayPanel.SetActive(false);
        }
    }
}
