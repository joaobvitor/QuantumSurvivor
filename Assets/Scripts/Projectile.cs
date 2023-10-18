using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Vector2 moveSpeed = new Vector2(10f, 0);
    [SerializeField] private Vector2 knockback = new Vector2(0, 0);
    [SerializeField] private int damage = 20;
    private bool hit;
    
    Rigidbody2D rb;
    Animator animator;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = new Vector2(moveSpeed.x * -transform.localScale.x, moveSpeed.y);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null && !hit) {
            knockback = transform.localScale.x > 0 ? knockback : Vector2.Scale(knockback, new Vector2(-1, 1));
            hit = damageable.Hit(damage, knockback);
        }
        rb.velocity = Vector2.zero;
        animator.SetBool(AnimationStrings.hit, true);
    }
}
