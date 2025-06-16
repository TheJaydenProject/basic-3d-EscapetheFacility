using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterHazard : MonoBehaviour
{
    public int damageAmount = 50;
    public float damageInterval = 0.5f;

    [Tooltip("Blue screen overlay shown when player is in water")]
    public GameObject waterOverlayPanel;

    private Dictionary<GameObject, Coroutine> waterCoroutines = new Dictionary<GameObject, Coroutine>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !waterCoroutines.ContainsKey(other.gameObject))
        {
            Coroutine c = StartCoroutine(DamageOverTime(other.gameObject));
            waterCoroutines.Add(other.gameObject, c);

            if (waterOverlayPanel != null)
                waterOverlayPanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && waterCoroutines.ContainsKey(other.gameObject))
        {
            StopCoroutine(waterCoroutines[other.gameObject]);
            waterCoroutines.Remove(other.gameObject);

            if (waterOverlayPanel != null)
                waterOverlayPanel.SetActive(false);
        }
    }

    IEnumerator DamageOverTime(GameObject player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        while (health != null && health.GetCurrentHealth() > 0)
        {
            health.TakeDamage(damageAmount, "WaterHazard");
            yield return new WaitForSeconds(damageInterval);
        }

        // Safety: hide overlay if player died in water
        if (waterOverlayPanel != null)
            waterOverlayPanel.SetActive(false);

        waterCoroutines.Remove(player);
    }

    public void CancelWaterDamageFor(GameObject player)
    {
        if (waterCoroutines.ContainsKey(player))
        {
            StopCoroutine(waterCoroutines[player]);
            waterCoroutines.Remove(player);

            if (waterOverlayPanel != null)
                waterOverlayPanel.SetActive(false);
        }
    }
}
