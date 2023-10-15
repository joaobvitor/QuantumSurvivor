using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private bool onTriggerEnter = true;
    [SerializeField] private bool onTriggerStay = false;
    public int attackDamage = 10;
    public Vector2 knockback = Vector2.zero;
    Collider2D attackCollider;
    AffectedByTime time;

    private void Awake() {
        attackCollider = GetComponent<Collider2D>();
        time = GetComponentInParent<AffectedByTime>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (onTriggerEnter) {
            if (time == null || !time.TimeIsStopped) {
                Damageable damageable = collision.GetComponent<Damageable>();

                if (damageable != null) {
                    if (transform.parent)
                        knockback = transform.parent.localScale.x > 0 ? knockback : Vector2.Scale(knockback, new Vector2(-1, 1));
                    bool gotHit = damageable.Hit(attackDamage, knockback);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (onTriggerStay) {
            if (time == null || !time.TimeIsStopped) {
                Damageable damageable = collision.GetComponent<Damageable>();

                if (damageable != null) {
                    bool gotHit = damageable.Hit(attackDamage, knockback);
                }
            }
        }
    }
}
