using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Text scoreText;          // UI text showing score
    public Text ballsLeftText;      // UI text showing balls remaining
    public Text gameOverText;       // UI text showing final score
    public Button restartButton;    // The restart button
    public BallThrower ballThrower; // Reference to the thrower script

    private int score = 0;

    void Start()
    {
        score = 0;
        UpdateScoreUI();
        gameOverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }

    public void AddScore()
    {
        score++;
        UpdateScoreUI();
    }

    public void UpdateBallsLeft(int ballsLeft)
    {
        ballsLeftText.text = "Balls Left: " + ballsLeft;
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }

    public void ShowGameOver()
    {
        gameOverText.text = "Game Over!\nFinal Score: " + score + "/5";
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    public void OnRestartButton()
    {
        score = 0;
        UpdateScoreUI();
        gameOverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        ballThrower.Restart();
    }
}