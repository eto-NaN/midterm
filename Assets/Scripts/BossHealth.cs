using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int hp = 5;

    public void TakeFireDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0) Destroy(gameObject);
    }
}
