using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Handles coin collection, coin UI display, and showing a congrats panel when all coins are collected.
/// </summary>
public class PlayerCoinCollector : MonoBehaviour
{
    [Header("Coin Settings")]
    public int totalCoins = 25;
    private int collectedCoins = 0;

    [Header("UI")]
    public TextMeshProUGUI coinText;
    public GameObject congratsPanel;
    public float congratsDuration = 3f;

    [Header("Audio")]
    [Tooltip("Sound to play when a coin is collected")]
    public AudioSource coinSFX;

    void Start()
    {
        UpdateCoinUI();
        if (congratsPanel != null)
            congratsPanel.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            if (coinSFX != null)
                coinSFX.Play();

            collectedCoins++;
            Destroy(other.gameObject);
            UpdateCoinUI();

            if (collectedCoins >= totalCoins && congratsPanel != null)
            {
                StartCoroutine(ShowCongrats());
            }
        }
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = collectedCoins + " / " + totalCoins;
    }

    IEnumerator ShowCongrats()
    {
        congratsPanel.SetActive(true);
        yield return new WaitForSeconds(congratsDuration);
        congratsPanel.SetActive(false);
    }
}
