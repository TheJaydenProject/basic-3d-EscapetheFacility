using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Handles player's health, damage flash, respawn, and death count.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI")]
    public TextMeshProUGUI healthText;

    [Header("Respawn")]
    public Transform spawnPoint;

    [Header("Damage Feedback")]
    [Tooltip("Red overlay panel that flashes when taking damage")]
    public GameObject DamageOverlay;

    [Tooltip("Duration of red overlay flash in seconds")]
    public float damageFlashDuration = 0.4f;

    [Tooltip("Sound to play when taking damage")]
    public AudioSource damageSFX;

    private int deathCount = 0;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int amount, string sourceTag = "Unknown")
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        Debug.Log($"Damaged by: {sourceTag} | Amount: {amount}");
        Debug.Log($"Player HP: {currentHealth}");

        // Flash red overlay
        if (DamageOverlay != null)
            StartCoroutine(FlashDamageOverlay());

        // Play damage sound
        if (damageSFX != null && !damageSFX.isPlaying)
            damageSFX.Play();

        if (currentHealth <= 0)
            Die();
    }

    public void InstantKill(string sourceTag = "Unknown")
    {
        currentHealth = 0;
        UpdateHealthUI();
        Debug.Log($"Instant killed by: {sourceTag}");
        Die();
    }

    public void Die()
    {
        deathCount++;
        Debug.Log("Player died.");
        Debug.Log("Deaths this run: " + deathCount);

        foreach (WaterHazard water in Object.FindObjectsByType<WaterHazard>(FindObjectsSortMode.None))
            water.CancelWaterDamageFor(gameObject);

        foreach (GasHazard gas in Object.FindObjectsByType<GasHazard>(FindObjectsSortMode.None))
            gas.CancelGasDamageFor(gameObject);

        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }

        if (controller != null) controller.enabled = true;

        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = "" + currentHealth + " / " + maxHealth;
    }

    private IEnumerator FlashDamageOverlay()
    {
        if (DamageOverlay != null)
        {
            DamageOverlay.SetActive(true);
            yield return new WaitForSeconds(damageFlashDuration);
            DamageOverlay.SetActive(false);
        }
    }
}
