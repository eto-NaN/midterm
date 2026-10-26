using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private LayerMask groundMask;

    [Header("Stats")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 3f;

    public void Init(int direction)
    {
        rigidBody.linearVelocity = new Vector2(speed * direction, 0f);
        Vector3 s = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(s.x) * direction, s.y, s.z);
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeFireDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (((1 << other.gameObject.layer) & groundMask) != 0)
            Destroy(gameObject);
    }
}
