using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;

    public int Score { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        UpdateDisplay();
    }

    public void AddScore(int amount)
    {
        Score += amount;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        scoreText.text = $"{Score}";
    }
}
