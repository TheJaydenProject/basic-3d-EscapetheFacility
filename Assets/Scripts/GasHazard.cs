using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Applies damage to the player over time when they enter a gas hazard zone.
/// Also displays a visual overlay and supports gas mask immunity.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 16/06/2025
 * Description: Damages the player over time when inside a gas hazard zone. Handles damage logic, 
 * overlay feedback, and immunity if the player owns a gas mask. Uses Unity Physics triggers.
 */
public class GasHazard : MonoBehaviour
{
    /// <summary>
    /// Amount of health deducted per interval when in the gas zone.
    /// </summary>
    public int damageAmount = 20;

    /// <summary>
    /// Time interval (in seconds) between damage ticks.
    /// </summary>
    public float damageInterval = 1f;

    /// <summary>
    /// UI panel shown as a green overlay when the player is inside the gas.
    /// </summary>
    [Tooltip("Green screen overlay shown when player is inside gas")]
    public GameObject gasOverlayPanel;

    /// <summary>
    /// Dictionary to track which players are currently inside the gas and have active damage coroutines.
    /// </summary>
    private Dictionary<GameObject, Coroutine> gasCoroutines = new Dictionary<GameObject, Coroutine>();

    /// <summary>
    /// Triggered when a collider enters the gas zone.
    /// Starts damaging the player if they don't have a gas mask and aren't already being damaged.
    /// </summary>
    /// <param name="other">Collider that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Start damaging only if it's the player and they're not already being damaged
        if (other.CompareTag("Player") && !gasCoroutines.ContainsKey(other.gameObject))
        {
            // Begin coroutine to apply damage over time
            Coroutine c = StartCoroutine(DamageOverTime(other.gameObject));
            gasCoroutines.Add(other.gameObject, c);

            // Enable the green overlay to visually indicate hazard
            if (gasOverlayPanel != null)
                gasOverlayPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Triggered when a collider exits the gas zone.
    /// Stops the damage coroutine and removes visual feedback.
    /// </summary>
    /// <param name="other">Collider that exited the trigger zone.</param>
    private void OnTriggerExit(Collider other)
    {
        // Stop coroutine only if it's the player and they are currently being damaged
        if (other.CompareTag("Player") && gasCoroutines.ContainsKey(other.gameObject))
        {
            // Stop damaging the player and remove reference
            StopCoroutine(gasCoroutines[other.gameObject]);
            gasCoroutines.Remove(other.gameObject);

            // Hide the gas overlay panel
            if (gasOverlayPanel != null)
                gasOverlayPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Coroutine that continuously damages the player while they are in the gas hazard zone.
    /// </summary>
    /// <param name="player">The player GameObject taking damage.</param>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator DamageOverTime(GameObject player)
    {
        // Get health and inventory references from the player
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        // While the player is alive, continue applying damage
        while (health != null && health.GetCurrentHealth() > 0)
        {
            // Only apply damage if the player doesn't have a gas mask equipped
            if (inventory != null && !inventory.HasGasMask())
            {
                health.TakeDamage(damageAmount, gameObject.tag); // Apply gas damage
            }

            // Wait for the specified damage interval before repeating
            yield return new WaitForSeconds(damageInterval);
        }

        // Hide overlay if the player dies or leaves the zone
        if (gasOverlayPanel != null)
            gasOverlayPanel.SetActive(false);

        // Remove the player from the coroutine tracker
        gasCoroutines.Remove(player);
    }

    /// <summary>
    /// Forcefully stops gas damage for a specific player (e.g., when teleporting or during cutscenes).
    /// </summary>
    /// <param name="player">The player GameObject to stop damaging.</param>
    public void CancelGasDamageFor(GameObject player)
    {
        // If the player is being damaged, stop it and clean up
        if (gasCoroutines.ContainsKey(player))
        {
            StopCoroutine(gasCoroutines[player]);
            gasCoroutines.Remove(player);

            // Hide the gas overlay if active
            if (gasOverlayPanel != null)
                gasOverlayPanel.SetActive(false);
        }
    }
}
