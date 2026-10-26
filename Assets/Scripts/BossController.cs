using System.Collections;
using UnityEngine;
using TMPro;

public class BossController : MonoBehaviour, IDamageable
{
    private enum State { Patrol, Windup, Charge, Rest }

    [Header("References")]

    [SerializeField] private KillPlayer killPlayer;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D bodyCollider;
    [SerializeField] private TMP_Text hpText;

    [SerializeField] private Animator animator;

    [Header("Health")]
    [SerializeField] private int maxHits = 10;
    [SerializeField] private float hitInvincibility = 0.6f;
    [SerializeField] private float bounceForce = 10f;

    [Header("Patrol")]
    [SerializeField] private float patrolRange = 5f;
    [SerializeField] private float patrolSpeed = 2f;

    [Header("Attack (Charge)")]
    [SerializeField] private float detectionRangeX = 6f;
    [SerializeField] private float detectionHeightAbove = 1.5f;
    [SerializeField] private float detectionHeightBelow = 1.5f;
    [SerializeField] private float windupTime = 0.6f;
    [SerializeField] private float chargeSpeed = 9f;
    [SerializeField] private float restTime = 1.2f;

    [Header("Player Damage")]
    [SerializeField] private float damageCooldown = 1f;

    [Header("Options")]
    [SerializeField] private bool spriteFacesRight = false;
    [SerializeField] private int startDirection = 1;

    [Header("Runtime State")]
    private State state = State.Patrol;
    private Transform player;
    private float minX, maxX;
    private int dir = 1;
    private int hits;
    private float stateTimer;
    private float invincibleUntil;
    private float nextDamageTime;
    private bool isDead;
    private float stompGraceUntil;

    private void Start()
    {
        dir = startDirection >= 0 ? 1 : -1;
        UpdateFacing();

        minX = transform.position.x - patrolRange;
        maxX = transform.position.x + patrolRange;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        UpdateHpText();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        float x = transform.position.x;

        switch (state)
        {
            case State.Patrol:
                SetVelocityX(dir * patrolSpeed);
                if (x >= maxX) dir = -1;
                else if (x <= minX) dir = 1;
                UpdateFacing();

                if (PlayerInRange())
                {
                    dir = player.position.x > x ? 1 : -1;
                    UpdateFacing();
                    EnterState(State.Windup, windupTime);
                }
                break;

            case State.Windup:
                SetVelocityX(0f);
                stateTimer -= Time.fixedDeltaTime;
                if (stateTimer <= 0f) EnterState(State.Charge, 2f);
                break;

            case State.Charge:
                SetVelocityX(dir * chargeSpeed);
                stateTimer -= Time.fixedDeltaTime;

                if ((dir == 1 && x >= maxX) || (dir == -1 && x <= minX) || stateTimer <= 0f)
                    EnterState(State.Rest, restTime);
                break;

            case State.Rest:
                SetVelocityX(0f);
                stateTimer -= Time.fixedDeltaTime;
                if (stateTimer <= 0f)
                {
                    dir = x > (minX + maxX) / 2f ? -1 : 1;
                    EnterState(State.Patrol, 0f);
                }
                break;
        }
    }

    private bool PlayerInRange()
    {
        if (player == null) return false;

        float dx = player.position.x - transform.position.x;
        float dy = player.position.y - transform.position.y;

        if (Mathf.Abs(dx) > detectionRangeX) return false;
        if (player.position.x < minX || player.position.x > maxX) return false;

        return dy <= detectionHeightAbove && dy >= -detectionHeightBelow;
    }

    private void EnterState(State newState, float duration)
    {
        state = newState;
        stateTimer = duration;

        if (newState == State.Charge && animator != null)
            animator.SetTrigger("Attack");
    }

    private void SetVelocityX(float vx)
    {
        rigidBody.linearVelocity = new Vector2(vx, rigidBody.linearVelocity.y);
    }

    private void UpdateFacing()
    {
        bool goingRight = dir == 1;
        spriteRenderer.flipX = spriteFacesRight ? !goingRight : goingRight;
    }

    public void TakeFireDamage(int amount) => TakeHit();

    private void TakeHit()
    {
        if (isDead || Time.time < invincibleUntil) return;

        if (animator != null) animator.SetTrigger("Damage");
        hits++;
        invincibleUntil = Time.time + hitInvincibility;
        UpdateHpText();

        if (hits >= maxHits) Die();
        else StartCoroutine(FlashRoutine());
    }

    // Learned yield return with AI
    private IEnumerator FlashRoutine()
    {
        Color original = spriteRenderer.color;
        while (Time.time < invincibleUntil && !isDead)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.08f);
            spriteRenderer.color = original;
            yield return new WaitForSeconds(0.08f);
        }
        spriteRenderer.color = original;
    }

    private void UpdateHpText()
    {
        if (hpText != null) hpText.text = $"Boss : {maxHits - hits}/{maxHits}";
    }

    private void Die()
    {
        isDead = true;
        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.simulated = false;
        bodyCollider.enabled = false;
        spriteRenderer.color = Color.gray;
        StartCoroutine(DeathRoutine());
    }

    // Learned yield return with AI
    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1f);
        VictoryManager.instance.TriggerVictory();
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D c) => HandleContact(c);
    private void OnCollisionStay2D(Collision2D c) => HandleContact(c);

    private void HandleContact(Collision2D collision)
    {
        if (isDead || !collision.gameObject.CompareTag("Player")) return;

        Rigidbody2D playerRb = collision.rigidbody;
        Collider2D playerCol = collision.collider;

        float bossTop = bodyCollider.bounds.max.y;
        float tolerance = bodyCollider.bounds.size.y * 0.3f;
        bool fromAbove = playerCol.bounds.min.y >= bossTop - tolerance;

        if (fromAbove && playerRb.linearVelocity.y <= 0.5f)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);
            TakeHit();
            stompGraceUntil = Time.time + 0.3f;
            return;
        }

        if (Time.time < stompGraceUntil) return;
        if (Time.time < nextDamageTime) return;
        nextDamageTime = Time.time + damageCooldown;

        float pushDir = Mathf.Sign(playerRb.position.x - transform.position.x);
        PlayerMovement movement = collision.gameObject.GetComponent<PlayerMovement>();
        if (movement != null) movement.Knockback(new Vector2(pushDir * 6f, 5f));

        PlayerPowerUp power = collision.gameObject.GetComponent<PlayerPowerUp>();
        if (power != null && power.AbsorbHit()) return;

        killPlayer.Die();
    }
}
