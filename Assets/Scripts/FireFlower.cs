using UnityEngine;

public class FireFlower : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerPowerUp power = other.GetComponent<PlayerPowerUp>();
        if (power == null) return;

        power.CollectFire();

        Destroy(gameObject);
    }
}
