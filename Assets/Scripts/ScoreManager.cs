using UnityEngine;
using TMPro; // This lets us talk to your UI Text!

public class ScoreManager : MonoBehaviour
{
    public int currentScore = 0;
    public TextMeshProUGUI scoreTextDisplay;

    void Start()
    {
        // Set the text to 0 right when the game starts
        if (scoreTextDisplay != null)
        {
            scoreTextDisplay.text = "SCORE: 0";
        }
    }

    public void AddPoint()
    {
        currentScore += 1; // Add 1 to the math
        
        // Update the screen
        if (scoreTextDisplay != null)
        {
            scoreTextDisplay.text = "SCORE: " + currentScore;
        }
        
        Debug.Log("Point Scored! Total: " + currentScore);
    }
}