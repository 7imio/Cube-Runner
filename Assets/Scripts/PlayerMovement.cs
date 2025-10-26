using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public bool enableMovement = false;
    public float forwardSpeed = 10f;
    public float lateralSpeed = 6f;
    public float clampX = 4.5f;

    public bool useTriggerCollisionAlso = true;

    private Rigidbody rb;
    private bool gameOverSent;
    private bool _prevEnableMovement;

    public Vector3 initialPosision;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        initialPosision = transform.position;
    }

    private void Update()
    {
        // Detect enableMovement toggling from false -> true (safety)
        if (!_prevEnableMovement && enableMovement)
        {
            // starting a run: clear the flag just in case
            gameOverSent = false;
        }
        _prevEnableMovement = enableMovement;
    }

    private void FixedUpdate()
    {
        if (!enableMovement)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        // forward movement
        Vector3 vel = rb.linearVelocity;
        vel.z = forwardSpeed;

        // CoreInput movement (lateral move)
        float inputX = CoreInput.Instance ? CoreInput.Instance.moveX : 0f;
        vel.x = inputX * lateralSpeed;

        rb.linearVelocity = vel;

        // x axis clamp
        Vector3 pos = rb.position;
        pos.x = Mathf.Clamp(pos.x, -clampX, clampX);
        rb.position = pos;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (gameOverSent) return;
        CheckCollisionWithObstacle(other.collider);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!useTriggerCollisionAlso) return;
        if (gameOverSent) return;
        CheckCollisionWithObstacle(other);
    }

    private void CheckCollisionWithObstacle(Collider other)
    {
        if (other != null && other.CompareTag("Obstacle")) SendGameOver();
    }

    private void SendGameOver()
    {
        gameOverSent = true;
        rb.linearVelocity = Vector3.zero;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    // ----- public reset for new run -----
    public void ResetForNewRun()
    {
        gameOverSent = false;
        rb.linearVelocity = Vector3.zero;

        transform.position = initialPosision;
    }
}
