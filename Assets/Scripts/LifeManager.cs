using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    public static LifeManager instance;

    [SerializeField] private AudioSource hitSound;

    [Header("UI")]
    [SerializeField] private Image[] lifeIcons;

    [Header("Runtime State")]
    private int lives;

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
        lives = lifeIcons.Length;
        UpdateDisplay();
    }

    public bool LoseLife()
    {
        lives = Mathf.Max(0, lives - 1);
        hitSound.Play();
        UpdateDisplay();
        return lives > 0;
    }

    public void SetLives(int newLives)
    {
        lives = Mathf.Clamp(newLives, 0, lifeIcons.Length);
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].enabled = i < lives;
        }
    }
}