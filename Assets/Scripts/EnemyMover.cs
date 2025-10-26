using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMover : MonoBehaviour
{
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.state == GameManager.GameState.Playing)
        {
            MoveEnemy();
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void MoveEnemy()
    {
        float speed = GameManager.Instance.enemySpeed;

        Vector3 vel = rb.linearVelocity;
        vel.z = -speed;
        rb.linearVelocity = vel;

    }

}
