using UnityEngine;

public class BallThrower : MonoBehaviour
{
    public GameObject ballPrefab;         // The ball we will throw
    public Transform spawnPoint;          // Where the ball spawns
    public float throwForce = 15f;        // How hard the ball is thrown
    public int totalBalls = 5;            // Total balls per round

    private int ballsLeft;
    private Vector3 dragStart;
    private bool isDragging = false;
    private GameObject currentBall;
    private bool roundOver = false;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        ballsLeft = totalBalls;
        SpawnBall();
    }

    void Update()
    {
        if (roundOver) return;

        // Player presses mouse button — start drag
        if (Input.GetMouseButtonDown(0))
        {
            dragStart = Input.mousePosition;
            isDragging = true;
        }

        // Player releases mouse button — throw ball
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            Vector3 dragEnd = Input.mousePosition;
            Vector3 dragDelta = dragEnd - dragStart;

            // Only throw if the player dragged far enough
            if (dragDelta.magnitude > 20f && currentBall != null)
            {
                ThrowBall(dragDelta);
            }
        }
    }

    void ThrowBall(Vector3 dragDelta)
    {
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();

        // Convert 2D drag into 3D throw direction
        // Dragging UP throws the ball forward and upward
        Vector3 throwDirection = new Vector3(
            dragDelta.x * 0.01f,     // sideways
            dragDelta.y * 0.02f,     // upward
            dragDelta.y * 0.05f      // forward
        );

        rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);

        ballsLeft--;
        gameManager.UpdateBallsLeft(ballsLeft);

        currentBall = null;

        if (ballsLeft <= 0)
        {
            roundOver = true;
            gameManager.ShowGameOver();
        }
        else
        {
            // Wait a moment then spawn the next ball
            Invoke("SpawnBall", 2f);
        }
    }

    void SpawnBall()
    {
        currentBall = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
    }

    public void Restart()
    {
        // Destroy all existing balls
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject b in balls) Destroy(b);

        ballsLeft = totalBalls;
        roundOver = false;
        gameManager.UpdateBallsLeft(ballsLeft);
        SpawnBall();
    }
}