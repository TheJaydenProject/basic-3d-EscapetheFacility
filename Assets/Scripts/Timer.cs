using UnityEngine;
using TMPro;

/// <summary>
/// Author: Jayden Wong  
/// Date: 16/06/2025  
/// Description: Displays the player's current elapsed time in MM:SS format during gameplay.  
/// The time value is pulled from the EndGameTrigger script, which tracks game duration.  
/// This script ensures real-time UI updates to enhance player awareness and immersion.
/// </summary>
public class TimerUI : MonoBehaviour
{
    /// <summary>
    /// Reference to the EndGameTrigger script which tracks the elapsed gameplay time.
    /// </summary>
    [Tooltip("Reference to the EndGameTrigger script")]
    public EndGameTrigger endGameTrigger;

    /// <summary>
    /// Text UI element used to display the current game time in MM:SS format.
    /// </summary>
    [Tooltip("Text UI element where time is displayed")]
    public TextMeshProUGUI timerText;

    /// <summary>
    /// Updates the timer display every frame with the current time from EndGameTrigger.
    /// </summary>
    void Update()
    {
        // Safety check: if either reference is missing, skip the update to prevent errors.
        if (endGameTrigger == null || timerText == null)
            return;

        // Get the total time elapsed since the player started the game.
        float time = endGameTrigger.GetElapsedTime();

        // Convert time from seconds into minutes and seconds format.
        int minutes = Mathf.FloorToInt(time / 60f);   // Whole minutes
        int seconds = Mathf.FloorToInt(time % 60f);   // Remaining seconds

        // Update the UI text to show formatted time as MM:SS.
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
