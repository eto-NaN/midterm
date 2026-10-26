using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform healthBar;

    [Header("Health Values")]

    [SerializeField] private float health;

    [SerializeField] private float maxHealth;

    [Header("Size")]
    [SerializeField] private float width;

    [SerializeField] private float height;

    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
    }

    private void SetHealth(float health)
    {
        this.health = health;
        float newWidth = (this.health / maxHealth) * width;
        healthBar.sizeDelta = new Vector2(newWidth, height);
    }
}