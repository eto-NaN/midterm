using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");

    [Header("References")]
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioSource hitSound;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;

    [Header("Runtime State")]
    private float input;
    private float stunEndTime;
    private bool controlsDisabled;

    private bool IsStunned => Time.time < stunEndTime;

    public void Knockback(Vector2 velocity, float duration = 0.3f)
    {
        if (controlsDisabled) return;
        stunEndTime = Time.time + duration;
        rigidBody.linearVelocity = velocity;
        hitSound.Play();
    }

    public void DisableControls()
    {
        controlsDisabled = true;
        input = 0f;
        rigidBody.linearVelocity = new Vector2(0f, rigidBody.linearVelocity.y);
        animator.SetBool(IsRunningHash, false);
    }

    public void EnableControls()
    {
        controlsDisabled = false;
    }

    private void Update()
    {
        if (controlsDisabled) return;

        input = IsStunned ? 0f : Input.GetAxis("Horizontal");

        animator.SetBool(IsRunningHash, input != 0);

        if (input < 0)
            spriteRenderer.flipX = true;
        else if (input > 0)
            spriteRenderer.flipX = false;
    }

    private void FixedUpdate()
    {
        if (controlsDisabled)
        {
            rigidBody.linearVelocity = new Vector2(0f, rigidBody.linearVelocity.y);
            return;
        }

        if (IsStunned) return;

        rigidBody.linearVelocity = new Vector2(input * speed, rigidBody.linearVelocity.y);
    }
}