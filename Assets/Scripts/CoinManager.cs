using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coinsDisplay;

    [Header("Runtime State")]
    private int coins;
    private AudioSource collectedSound;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        collectedSound = GetComponent<AudioSource>();
    }

    private void OnGUI()
    {
        coinsDisplay.text = coins.ToString();
    }

    public void ChangeCoins(int amount)
    {
        collectedSound.Play();
        coins += amount;
    }
}
