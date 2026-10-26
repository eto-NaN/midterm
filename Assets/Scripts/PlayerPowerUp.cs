using UnityEngine;

public class PlayerPowerUp : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float scaleMultiplier = 1.2f;

    [Header("Audio")]
    [SerializeField] private AudioSource PowerUpSound;

    [Header("Runtime State")]
    private Vector3 baseScale;
    public bool HasFire { get; private set; }

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    public void CollectFire()
    {
        PowerUpSound.Play();
        if (HasFire) return;
        HasFire = true;
        transform.localScale = baseScale * scaleMultiplier;
        transform.position += Vector3.up * 0.1f;
    }

    public bool AbsorbHit()
    {
        if (!HasFire) return false;
        HasFire = false;
        transform.localScale = baseScale;
        return true;
    }
}
