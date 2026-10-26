using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text timerText;

    [Header("Settings")]
    [SerializeField] private float startTime = 180f;

    [Header("Runtime State")]
    private float timeLeft;
    private int lastDisplayedSecond = -1;
    private bool running = true;

    public int SecondsLeft => Mathf.CeilToInt(timeLeft);

    private void Start()
    {
        timeLeft = startTime;
        UpdateDisplay();
    }

    private void Update()
    {
        if (!running) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            running = false;
            UpdateDisplay();
            OnTimeUp();
            return;
        }

        int currentSecond = Mathf.CeilToInt(timeLeft);
        if (currentSecond != lastDisplayedSecond)
        {
            lastDisplayedSecond = currentSecond;
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        int totalSeconds = Mathf.CeilToInt(timeLeft);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void OnTimeUp()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    public void StopTimer()
    {
        running = false;
    }
}
