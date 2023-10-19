using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class TankBot : MonoBehaviour
{
    public float walkAcceleration = 30f;
    public float maxSpeed = 3f;
    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;

    Animator animator;
    Rigidbody2D rb;

    TouchingDirections touchingDirections;
    Damageable damageable;

    AffectedByGravity gravity;
     AffectedByTime time;

    public enum WalkableDirection { Right, Left }

    public Vector2 walkDirectionVector;

    private WalkableDirection _walkDirection = WalkableDirection.Left;

    public WalkableDirection WalkDirection {
        get {
            return _walkDirection;
        } 
        private set {
            if (_walkDirection != value) {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if (value == WalkableDirection.Right)
                    walkDirectionVector = Vector2.right;
                else
                    walkDirectionVector = Vector2.left;
                
            }
            _walkDirection = value;
        } 
    }

    private bool _hasTarget = false;

    public bool HasTarget {
        get {
            return _hasTarget;
        } 
        private set {
            if (value == true) {
                bool isPlayerToTheRight = attackZone.detectedColliders[0].transform.position.x - transform.position.x > 0;
                if ((isPlayerToTheRight && WalkDirection == WalkableDirection.Left)
                || !isPlayerToTheRight && WalkDirection == WalkableDirection.Right)
                    FlipDirection();
            }
            animator.SetBool(AnimationStrings.hasTarget, value);
            _hasTarget = value;
        } 
    }

    public bool CanMove { 
        get {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public float AttackCooldown { 
        get {
            return animator.GetFloat(AnimationStrings.attackCooldown);
        }
        private set {
             animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
        gravity = GetComponent<AffectedByGravity>();
        walkDirectionVector = new Vector2(-gameObject.transform.localScale.x, 0);
        time = GetComponent<AffectedByTime>();
    }

    // Update is called once per frame
    void Update()
    {
        if (AttackCooldown > 0)
            AttackCooldown -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (!time.TimeIsStopped) {
            HasTarget = attackZone.detectedColliders.Count > 0;

            if (touchingDirections.IsOnWall && touchingDirections.IsGrounded && CanMove)
                FlipDirection();
            
            if (!damageable.IsHit && CanMove) {
                if (touchingDirections.IsOnWall && !touchingDirections.IsGrounded)
                    rb.velocity = new Vector2(0, rb.velocity.y);
                else
                    rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed), rb.velocity.y);
            }
        }
    }

    private void FlipDirection() {
        if (WalkDirection == WalkableDirection.Right)
            WalkDirection = WalkableDirection.Left;
        else
            WalkDirection = WalkableDirection.Right;
    }

    public void OnHit(int damage, Vector2 knockback) {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    public void OnCliffDetected() {
        if (touchingDirections.IsGrounded  && !gravity.OnCooldown) {
            FlipDirection();
        }
    }
}
