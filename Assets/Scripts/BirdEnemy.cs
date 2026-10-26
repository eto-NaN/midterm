using UnityEngine;

public class BirdEnemy : MonoBehaviour, IDamageable
{
    private enum State { Patrol, Dive, Return }

    [Header("References")]

    [SerializeField] private KillPlayer killPlayer;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private LayerMask groundMask;

    [Header("Patrol")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float patrolRange = 3f;

    [Header("Detection and Attack")]
    [SerializeField] private float detectionRangeX = 5f;
    [SerializeField] private float detectionRangeY = 4f;
    [SerializeField] private bool detectBelowOnly = false;
    [SerializeField] private float diveSpeed = 8f;
    [SerializeField] private float returnSpeed = 4f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Options")]
    [SerializeField] private bool spriteFacesRight = true;

    [Header("Runtime State")]
    private State state = State.Patrol;
    private Vector2 patrolOrigin;
    private Vector2 diveTarget;
    private Transform player;
    private int patrolDir = 1;
    private float nextAttackTime;
    private bool isDead;

    private void Start()
    {
        patrolOrigin = rigidBody.position;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        switch (state)
        {
            case State.Patrol: DoPatrol(); break;
            case State.Dive:   DoDive();   break;
            case State.Return: DoReturn(); break;
        }
    }

    private void DoPatrol()
    {
        float minX = patrolOrigin.x - patrolRange;
        float maxX = patrolOrigin.x + patrolRange;

        Vector2 pos = rigidBody.position;
        pos.x += patrolDir * patrolSpeed * Time.fixedDeltaTime;

        if (pos.x >= maxX) { pos.x = maxX; patrolDir = -1; }
        else if (pos.x <= minX) { pos.x = minX; patrolDir = 1; }

        pos.y = patrolOrigin.y;
        rigidBody.MovePosition(pos);
        FaceDirection(patrolDir);

        if (player != null && Time.time >= nextAttackTime && PlayerInRange())
        {
            StartDive();
        }
    }

    private void StartDive()
    {
        diveTarget = player.position;
        state = State.Dive;
    }

    private void DoDive()
    {
        Vector2 next = Vector2.MoveTowards(rigidBody.position, diveTarget, diveSpeed * Time.fixedDeltaTime);
        FaceDirection(diveTarget.x - rigidBody.position.x);
        rigidBody.MovePosition(next);

        if (Vector2.Distance(next, diveTarget) < 0.05f)
            StartReturn();
    }

    private void DoReturn()
    {
        Vector2 next = Vector2.MoveTowards(rigidBody.position, patrolOrigin, returnSpeed * Time.fixedDeltaTime);
        FaceDirection(patrolOrigin.x - rigidBody.position.x);
        rigidBody.MovePosition(next);

        if (Vector2.Distance(next, patrolOrigin) < 0.05f)
        {
            rigidBody.position = patrolOrigin;
            state = State.Patrol;
        }
    }

    private void StartReturn()
    {
        state = State.Return;
        nextAttackTime = Time.time + attackCooldown;
    }

    private void FaceDirection(float dirX)
    {
        if (Mathf.Abs(dirX) < 0.01f) return;
        bool goingRight = dirX > 0;
        spriteRenderer.flipX = spriteFacesRight ? !goingRight : goingRight;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (state == State.Dive && ((1 << other.gameObject.layer) & groundMask) != 0)
        {
            StartReturn();
            return;
        }

        if (!other.CompareTag("Player")) return;
        if (state == State.Return) return;

        HitPlayer(other.gameObject);
        StartReturn();
    }

    private void HitPlayer(GameObject playerObj)
    {
        PlayerMovement movement = playerObj.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            float pushDir = Mathf.Sign(playerObj.transform.position.x - transform.position.x);
            movement.Knockback(new Vector2(pushDir * 3f, 3f), 0.15f);
        }

        PlayerPowerUp power = playerObj.GetComponent<PlayerPowerUp>();
        if (power != null && power.AbsorbHit())
            return;

        killPlayer.Die();
    }

    private bool PlayerInRange()
    {
        Vector2 diff = (Vector2)player.position - rigidBody.position;

        if (Mathf.Abs(diff.x) > detectionRangeX) return false;

        if (detectBelowOnly)
            return diff.y <= 0f && diff.y >= -detectionRangeY;

        return Mathf.Abs(diff.y) <= detectionRangeY;
    }

    public void TakeFireDamage(int amount)
    {
        if (isDead) return;
        isDead = true;
        Destroy(gameObject);
    }
}
