using UnityEngine;

public class WinZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameTimer timer;

    [Header("Runtime State")]
    private bool used;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used || !other.CompareTag("Player")) return;
        used = true;
        LevelManager.instance.LevelCompleted();
    }
}
