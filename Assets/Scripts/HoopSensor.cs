using UnityEngine;

public class HoopSensor : MonoBehaviour
{
    public ScoreManager scoreManager; // The brain we created earlier

    // This built-in function runs the exact millisecond something touches the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Did the object that touched us have the "Ball" tag?
        if (other.gameObject.CompareTag("Ball"))
        {
            if (scoreManager != null)
            {
                scoreManager.AddPoint(); // Tell the brain to add a point!
            }
        }
    }
}