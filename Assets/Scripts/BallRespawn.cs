using UnityEngine;

public class BallRespawn : MonoBehaviour
{
    private Vector3 startPosition;
    private Rigidbody rb;
    
    [Header("Settings")]
    public float maxDistance = 15f; // How far away before it resets
    public Transform playerXR; // Drag your XR Origin here so it knows where you are

    void Start()
    {
        // Remember exactly where the ball was placed at the start of the game
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Fail-safe 1: Did it fall through the floor into the void?
        if (transform.position.y < -2f)
        {
            ResetBall();
        }

        // Fail-safe 2: Did it get thrown way out of bounds?
        if (playerXR != null && Vector3.Distance(transform.position, playerXR.position) > maxDistance)
        {
            ResetBall();
        }
    }

    public void ResetBall()
    {
        // Teleport it back to the start
        transform.position = startPosition;
        
        // Kill all momentum so it doesn't keep flying after teleporting!
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero; 
        }
    }
}