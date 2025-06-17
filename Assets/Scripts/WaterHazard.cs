using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Continuously damages the player when they are inside a water hazard.
/// Also displays a visual overlay and stops damage when the player exits or dies.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 16/06/2025
 * Description: This script detects when the player enters water using Unity Physics triggers.
 * It deals periodic damage while the player remains inside and shows a blue screen overlay.
 * Damage is stopped when the player exits or dies, and overlays are cleaned up accordingly.
 */
public class WaterHazard : MonoBehaviour
{
    /// <summary>
    /// Amount of damage the player takes each interval while inside the water.
    /// </summary>
    public int damageAmount = 50;

    /// <summary>
    /// Time delay (in seconds) between damage ticks.
    /// </summary>
    public float damageInterval = 0.5f;

    /// <summary>
    /// UI overlay shown when the player is inside the water hazard.
    /// </summary>
    [Tooltip("Blue screen overlay shown when player is in water")]
    public GameObject waterOverlayPanel;

    /// <summary>
    /// Keeps track of ongoing damage coroutines for each player in the hazard zone.
    /// </summary>
    private Dictionary<GameObject, Coroutine> waterCoroutines = new Dictionary<GameObject, Coroutine>();

    /// <summary>
    /// Unity Physics trigger that detects when the player enters the water.
    /// Starts damage coroutine and shows overlay.
    /// </summary>
    /// <param name="other">Collider that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Only start damage if it's the player and no coroutine is already running for them
        if (other.CompareTag("Player") && !waterCoroutines.ContainsKey(other.gameObject))
        {
            // Start damage over time
            Coroutine c = StartCoroutine(DamageOverTime(other.gameObject));
            waterCoroutines.Add(other.gameObject, c);

            // Show water effect overlay
            if (waterOverlayPanel != null)
                waterOverlayPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Unity Physics trigger that detects when the player exits the water.
    /// Stops damage coroutine and hides overlay.
    /// </summary>
    /// <param name="other">Collider that exited the trigger zone.</param>
    private void OnTriggerExit(Collider other)
    {
        // Stop damage if player leaves the water zone
        if (other.CompareTag("Player") && waterCoroutines.ContainsKey(other.gameObject))
        {
            StopCoroutine(waterCoroutines[other.gameObject]);
            waterCoroutines.Remove(other.gameObject);

            // Hide overlay when player exits
            if (waterOverlayPanel != null)
                waterOverlayPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Continuously applies damage to the player while they're in the water zone.
    /// Ends if the player dies.
    /// </summary>
    /// <param name="player">The player GameObject being damaged.</param>
    IEnumerator DamageOverTime(GameObject player)
    {
        // Get the player's health component
        PlayerHealth health = player.GetComponent<PlayerHealth>();

        // Continue damaging while player is alive
        while (health != null && health.GetCurrentHealth() > 0)
        {
            // Apply water hazard damage
            health.TakeDamage(damageAmount, "WaterHazard");
            yield return new WaitForSeconds(damageInterval);
        }

        // Hide overlay if the player died inside the water
        if (waterOverlayPanel != null)
            waterOverlayPanel.SetActive(false);

        // Cleanup coroutine tracking
        waterCoroutines.Remove(player);
    }

    /// <summary>
    /// Cancels ongoing water damage and overlay for a specific player.
    /// Useful when respawning or resetting player state.
    /// </summary>
    /// <param name="player">Player GameObject whose damage should stop.</param>
    public void CancelWaterDamageFor(GameObject player)
    {
        if (waterCoroutines.ContainsKey(player))
        {
            StopCoroutine(waterCoroutines[player]);
            waterCoroutines.Remove(player);

            // Hide overlay if it's still active
            if (waterOverlayPanel != null)
                waterOverlayPanel.SetActive(false);
        }
    }
}
