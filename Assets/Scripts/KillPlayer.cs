using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float deathY = -10f;

    [Header("Runtime State")]
    private Vector3 respawnPoint;
    private Rigidbody2D rigidBody;

    private void Start()
    {
        respawnPoint = transform.position;
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (transform.position.y < deathY)
        {
            Die();
        }
    }

    public void SetCheckpoint(Vector3 newPoint)
    {
        respawnPoint = newPoint;
    }

    public void Die()
    {
        bool hasLivesLeft = LifeManager.instance.LoseLife();

        if (hasLivesLeft)
        {
            transform.position = respawnPoint;
            if (rigidBody != null)
                rigidBody.linearVelocity = Vector2.zero;
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}