using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("UI")]
    [SerializeField] private TMP_Text levelCompleteText;

    [Header("References")]
    [SerializeField] private Transform nextLevelSpawn;
    [SerializeField] private Rigidbody2D playerRb;

    [Header("Runtime State")]
    private bool waitingForInput;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        levelCompleteText.gameObject.SetActive(false);
    }

    public void LevelCompleted()
    {
        if (waitingForInput) return;
        waitingForInput = true;

        levelCompleteText.SetText("Level Completed !\nPress SPACE to continue");
        levelCompleteText.gameObject.SetActive(true);

        PlayerMovement movement = playerRb.GetComponent<PlayerMovement>();
        if (movement != null) movement.DisableControls();

        PlayerJump jump = playerRb.GetComponent<PlayerJump>();
        if (jump != null) jump.DisableJump();
    }

    private void Update()
    {
        if (waitingForInput && Input.GetKeyDown(KeyCode.Space))
            GoToNextLevel();
    }

    private void GoToNextLevel()
    {
        KillPlayer killPlayer = playerRb.GetComponent<KillPlayer>();
        waitingForInput = false;
        if (killPlayer != null) killPlayer.SetCheckpoint(playerRb.position);
        levelCompleteText.gameObject.SetActive(false);


        playerRb.linearVelocity = Vector2.zero;
        playerRb.position = nextLevelSpawn.position;
        playerRb.transform.position = nextLevelSpawn.position;

        if (killPlayer != null) killPlayer.SetCheckpoint(nextLevelSpawn.position);

        PlayerMovement movement = playerRb.GetComponent<PlayerMovement>();
        if (movement != null) movement.EnableControls();

        PlayerJump jump = playerRb.GetComponent<PlayerJump>();
        if (jump != null) jump.EnableJump();

        LifeManager.instance.SetLives(3);
    }
}
