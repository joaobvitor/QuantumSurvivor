using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectedProjectile : MonoBehaviour
{
    [SerializeField] private Vector2 moveSpeed = new Vector2(10f, 10f);
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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 direction = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, 0);
        
        rb.velocity = new Vector2(moveSpeed.x * direction.normalized.x,
                                  moveSpeed.y * direction.normalized.y);

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * direction;
        transform.rotation = Quaternion.LookRotation(forward: Vector3.back, upwards: rotatedVectorToTarget);

        /* if (transform.rotation.x < 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y); */
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