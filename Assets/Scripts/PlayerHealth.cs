using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI")]
    public TextMeshProUGUI healthText;

    [Header("Respawn")]
    public Transform spawnPoint;

    private int deathCount = 0; // Tracks how many times the player has died

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

        if (currentHealth <= 0)
        {
            Die();
        }
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

        foreach (GasHazard gas in Object.FindObjectsByType<GasHazard>(FindObjectsSortMode.None))
        {
            gas.CancelGasDamageFor(gameObject);
        }

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
        {
            healthText.text = "HP: " + currentHealth + " / " + maxHealth;
        }
    }
}
