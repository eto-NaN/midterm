using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private Animator animator;
    [SerializeField] private PlayerPowerUp powerUp;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioSource shootSound;

    [Header("Fireball")]
    [SerializeField] private Fireball fireballPrefab;
    [SerializeField] private float spawnOffset = 0.7f;
    [SerializeField] private float cooldown = 0.4f;

    [Header("Input")]
    [SerializeField] private KeyCode fireKey = KeyCode.F;

    [Header("Runtime State")]
    private float nextFireTime;

    private void Update()
    {
        if (!powerUp.HasFire || Time.time < nextFireTime) return;
        if (!Input.GetKeyDown(fireKey)) return;

        animator.SetTrigger("Attack");
        nextFireTime = Time.time + cooldown;

        int dir = spriteRenderer.flipX ? -1 : 1;
        Vector3 pos = transform.position + new Vector3(dir * spawnOffset, 0f, 0f);
        shootSound.Play();

        Fireball ball = Instantiate(fireballPrefab, pos, Quaternion.identity);
        ball.Init(dir);
    }
}
