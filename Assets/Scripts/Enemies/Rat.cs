using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class Rat : MonoBehaviour
{
    public float walkAcceleration = 30f;
    public float maxSpeed = 3f;
    public float walkStopRate = 0.05f;
    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;

    Animator animator;
    Rigidbody2D rb;

    TouchingDirections touchingDirections;
    Damageable damageable;

    AffectedByGravity gravity;

    public enum WalkableDirection { Right, Left }

    private Vector2 walkDirectionVector;

    private WalkableDirection _walkDirection;

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

    public bool CanMove { 
        get {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
        gravity = GetComponent<AffectedByGravity>();
        walkDirectionVector = new Vector2(gameObject.transform.localScale.x, 0);
    }

    private void FixedUpdate()
    {
        if (touchingDirections.IsOnWall && touchingDirections.IsGrounded && CanMove)
            FlipDirection();
        
        if (!damageable.IsHit) {
            if (touchingDirections.IsOnWall && !touchingDirections.IsGrounded)
                rb.velocity = new Vector2(0, rb.velocity.y);
            else if (!CanMove)
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
            else
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed), rb.velocity.y);
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
