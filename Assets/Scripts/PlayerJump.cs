using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private AudioSource jumpSound;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2f;

    [Header("Runtime State")]
    private bool canJump = true;

    public void DisableJump()
    {
        canJump = false;
    }

    public void EnableJump()
    {
        canJump = true;
    }

    void Start()
    {
        jumpSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        bool grounded = IsGrounded();

        if (Input.GetButtonDown("Jump") && canJump && grounded)
        {
            Jump();
        }

        animator.SetBool(IsJumpingHash, !grounded);
    }

    private bool IsGrounded()
    {
        float halfHeight = spriteRenderer.bounds.extents.y;
        return Physics2D.Raycast(transform.position, Vector2.down, halfHeight + 0.1f, LayerMask.GetMask("Ground"));
    }

    private void Jump()
    {
        jumpSound.Play();
        float gravity = Mathf.Abs(Physics2D.gravity.y) * rigidBody.gravityScale;
        float jumpVelocity = Mathf.Sqrt(2f * gravity * jumpHeight);

        rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, jumpVelocity);
    }
}
