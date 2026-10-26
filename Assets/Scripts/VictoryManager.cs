using UnityEngine;
using TMPro;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager instance;

    [Header("UI")]
    [SerializeField] private TMP_Text winText;

    [Header("References")]
    [SerializeField] private GameTimer timer;

    [Header("Runtime State")]
    private bool hasWon;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void TriggerVictory()
    {
        if (hasWon) return;
        hasWon = true;

        if (timer != null)
        {
            ScoreManager.instance.AddScore(timer.SecondsLeft);
            timer.StopTimer();
        }

        winText.SetText("You won!\nScore : {0}", ScoreManager.instance.Score);
        winText.gameObject.SetActive(true);

        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        if (player != null) player.DisableControls();

        PlayerJump jump = FindAnyObjectByType<PlayerJump>();
        if (jump != null) jump.DisableJump();

        FreezeEnemies();
    }

    private void FreezeEnemies()
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        foreach (Rigidbody2D rb in FindObjectsByType<Rigidbody2D>())
        {
            if (rb.gameObject.layer != enemyLayer) continue;

            foreach (MonoBehaviour script in rb.GetComponents<MonoBehaviour>())
                script.enabled = false;

            Animator anim = rb.GetComponent<Animator>();
            if (anim != null) anim.enabled = false;

            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
    }
}
