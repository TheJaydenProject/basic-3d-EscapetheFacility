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
            // Start damage coroutine
            Coroutine c = StartCoroutine(DamageOverTime(other.gameObject));
            gasCoroutines.Add(other.gameObject, c);

            // Show overlay if assigned
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

            // Hide overlay
            if (gasOverlayPanel != null)
                gasOverlayPanel.SetActive(false);
        }
    }

    IEnumerator DamageOverTime(GameObject player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        while (health != null && health.GetCurrentHealth() > 0)
        {
            health.TakeDamage(damageAmount, gameObject.tag);
            yield return new WaitForSeconds(damageInterval);
        }

        // Safety: hide overlay after death
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

            // Hide overlay when cancelled
            if (gasOverlayPanel != null)
                gasOverlayPanel.SetActive(false);
        }
    }
}
