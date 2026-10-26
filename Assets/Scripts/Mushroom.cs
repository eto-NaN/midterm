using UnityEngine;

public class Mushroom : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int value;

    [Header("Runtime State")]
    private bool hasTriggered;
    private CoinManager coinManager;

    private void Start()
    {
        coinManager = CoinManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            coinManager.ChangeCoins(value);
            ScoreManager.instance.AddScore(10);
            Destroy(gameObject);
        }
    }
}
