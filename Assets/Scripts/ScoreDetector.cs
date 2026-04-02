using UnityEngine;

public class ScoreDetector : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // This runs automatically when something enters the trigger zone
    void OnTriggerEnter(Collider other)
    {
        // Check if it's a ball (by tag)
        if (other.CompareTag("Ball"))
        {
            gameManager.AddScore();
        }
    }
}