using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using StarterAssets;

/// <summary>
/// Triggers the end-of-game sequence when the player interacts with the escape object.
/// Displays the final score, time, coin count, and deaths. Disables player control.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 16/06/2025
 * Description: Ends the game and displays final results when the player presses E near this object.
 */

public class EndGameTrigger : MonoBehaviour
{
    [Header("UI References")]

    /// <summary>
    /// The end screen UI panel that will be shown when the game ends.
    /// </summary>
    public GameObject endPanel;

    /// <summary>
    /// UI text field to display the final score.
    /// </summary>
    public TextMeshProUGUI scoreText;

    /// <summary>
    /// UI text field to display the time taken.
    /// </summary>
    public TextMeshProUGUI timeText;

    /// <summary>
    /// UI text field to show the number of player deaths.
    /// </summary>
    public TextMeshProUGUI deathText;

    /// <summary>
    /// UI text field to show coins collected versus total.
    /// </summary>
    public TextMeshProUGUI coinText;

    [Header("Game Stats")]

    /// <summary>
    /// Reference to the player’s health script for tracking deaths.
    /// </summary>
    public PlayerHealth playerHealth;

    /// <summary>
    /// Reference to the player's coin collection script.
    /// </summary>
    public PlayerCoinCollector coinCollector;

    /// <summary>
    /// Time tracker for how long the player has been in the game.
    /// </summary>
    private float timeElapsed = 0f;

    [Header("Disable Movement")]

    /// <summary>
    /// Reference to the FirstPersonController to disable player movement at end.
    /// </summary>
    public FirstPersonController controllerScript;

    [Header("Disable Look")]

    /// <summary>
    /// Script used to control camera look input (e.g., CinemachineInputProvider).
    /// </summary>
    public MonoBehaviour lookInputScript;

    /// <summary>
    /// Boolean flag that becomes true when the player is within trigger zone.
    /// </summary>
    private bool inRange = false;

    void Update()
    {
        // Track time since game started
        timeElapsed += Time.deltaTime;

        // If player is in range and presses 'E', end the game
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            TriggerEndGame();
        }
    }

    /// <summary>
    /// Ends the game: disables controls, shows UI stats, and calculates score.
    /// </summary>
    public void TriggerEndGame()
    {
        // Pause game time
        Time.timeScale = 0f;

        // Get number of deaths and coins collected
        int deaths = playerHealth != null ? playerHealth.GetDeathCount() : 0;
        int coins = coinCollector != null ? coinCollector.GetCollectedCoins() : 0;
        int totalCoins = coinCollector != null ? coinCollector.totalCoins : 25;

        // Calculate score based on coins collected, deaths, and time taken
        float coinScore = (coins / (float)totalCoins) * 500f;
        float deathPenalty = Mathf.Min(200f, deaths * 20f);

        float timePenalty;
        if (timeElapsed <= 300f)
        {
            // Less than 5 mins — light penalty
            timePenalty = Mathf.Min(100f, Mathf.Max(0f, timeElapsed - 150f) * 0.5f);
        }
        else
        {
            // More than 5 mins — heavier penalty
            timePenalty = Mathf.Min(400f, 75f + Mathf.Max(0f, timeElapsed - 300f) * 1.5f);
        }

        int finalScore = Mathf.RoundToInt(250f + coinScore - deathPenalty - timePenalty);
        finalScore = Mathf.Clamp(finalScore, 0, 999); // Clamp to 0–999

        // Show end panel with game summary
        if (endPanel != null)
        {
            endPanel.SetActive(true);
            scoreText.text = $"Score: {finalScore}";
            timeText.text = $"Time: {Mathf.FloorToInt(timeElapsed / 60f)}min {Mathf.FloorToInt(timeElapsed % 60f)}s";
            deathText.text = $"Deaths: {deaths}";
            coinText.text = $"Coins: {coins}/{totalCoins}";
        }

        // Disable player movement and camera control
        if (controllerScript != null)
            controllerScript.enabled = false;

        if (lookInputScript != null)
            lookInputScript.enabled = false;

        // Unlock cursor and show it for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Game ended.");
    }

    /// <summary>
    /// Reloads the current scene to restart the game.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload scene
    }

    /// <summary>
    /// Detect when the player enters the end-game trigger zone.
    /// </summary>
    /// <param name="other">Collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            inRange = true; // Enable interaction
    }

    /// <summary>
    /// Detect when the player leaves the end-game trigger zone.
    /// </summary>
    /// <param name="other">Collider that exited the trigger.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            inRange = false; // Disable interaction
    }
}
