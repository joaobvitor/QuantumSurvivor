using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class JetBot : MonoBehaviour
{
    public float walkAcceleration = 30f;
    public float maxSpeed = 3f;
    [SerializeField] private float chargeImpulse = 20f;
    [SerializeField] private int moneyOnDeath = 5;

    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;

    Animator animator;
    Rigidbody2D rb;

    TouchingDirections touchingDirections;
    Damageable damageable;

    AffectedByGravity gravity;
     AffectedByTime time;
     GameObject player;

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

    public bool CanMove { 
        get {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    private bool _hasTarget = false;

    public bool HasTarget {
        get {
            return _hasTarget;
        } 
        private set {
            if (_hasTarget != value && value == true) {
                bool isPlayerToTheRight = attackZone.detectedColliders[0].transform.position.x - transform.position.x > 0;
                if ((isPlayerToTheRight && WalkDirection == WalkableDirection.Right)
                || !isPlayerToTheRight && WalkDirection == WalkableDirection.Left) {
                    animator.SetBool(AnimationStrings.hasTarget, value);
                    _hasTarget = value;
                }
            }
            else if (_hasTarget != value && value == false) {
                animator.SetBool(AnimationStrings.hasTarget, value);
                _hasTarget = value;
            }
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

    public bool IsCharging { 
        get {
            return animator.GetBool(AnimationStrings.isCharging);
        }
        private set {
            animator.SetBool(AnimationStrings.isCharging, value);
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
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Start() {
        if (player.GetComponent<Rigidbody2D>().gravityScale < 0) {
            gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x, -gameObject.transform.localScale.y);
            rb.gravityScale = -rb.gravityScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;
        if (AttackCooldown > 0)
            AttackCooldown -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (!time.TimeIsStopped) {
            if (touchingDirections.IsOnWall && (CanMove || IsCharging))
                FlipDirection();
            
            if (!damageable.IsHit && CanMove) {
                if (touchingDirections.IsOnWall && !touchingDirections.IsGrounded)
                    rb.velocity = new Vector2(0, rb.velocity.y);
                else
                    rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed), rb.velocity.y);
            }
            else if (IsCharging)
                rb.velocity = new Vector2(chargeImpulse * -transform.localScale.x, rb.velocity.y);
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
        if (touchingDirections.IsGrounded  && !gravity.OnCooldown && !IsCharging) {
            FlipDirection();
        }
    }

    public void Charge() {
        IsCharging = true;
    }

    public void OnDeath() {
        player.GetComponent<PlayerController>().Money += moneyOnDeath;
    }
}
