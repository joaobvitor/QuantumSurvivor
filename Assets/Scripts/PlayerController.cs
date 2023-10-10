using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float airSpeed = 4f;
    public float jumpImpulse = 10f;
    public float gravityCooldown = 0.5f;
    Vector2 moveInput;
    TouchingDirections touchingDirections;
    Damageable damageable;
    AffectedByGravity gravity;

    public float CurrentMoveSpeed { get
        {
            if (CanMove) {
                if (IsMoving && !touchingDirections.IsOnWall) {
                    if (touchingDirections.IsGrounded) {
                        if (IsRunning)
                            return runSpeed;
                        else
                            return walkSpeed;
                    }
                    else {
                        return airSpeed;
                    }
                }
            }
            return 0;
        }
    }

    [SerializeField]
    private bool _isMoving = false;

    public bool IsMoving {
        get {
            return _isMoving;
        } 
        private set {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        } 
    }

    [SerializeField]
    private bool _isRunning = false;

    public bool IsRunning { 
        get {
            return _isRunning;
        }
        private set {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }

    [SerializeField]
    private bool _isJumping = false;

    public bool IsJumping { 
        get {
            return _isJumping;
        }
        private set {
            _isJumping = value;
        }
    }

    [SerializeField]
    private bool _isFacingRight = true;

    public bool IsFacingRight { 
        get {
            return _isFacingRight;
        }
        private set {
            if (_isFacingRight != value)
                transform.localScale *= new Vector2(-1, 1);

            _isFacingRight = value;
        }
    }

    public bool CanMove { 
        get {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public bool IsAlive { 
        get {
            return animator.GetBool(AnimationStrings.isAlive);
        }
    }

    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
        gravity = GetComponent<AffectedByGravity>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!damageable.IsHit)
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);

        if (IsJumping && touchingDirections.IsGrounded && CanMove) {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse * rb.gravityScale);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive) {
            IsMoving = (moveInput != Vector2.zero);
            SetFacingDirection(moveInput);
        }
    }

    private void SetFacingDirection(Vector2 moveInput) {
        if (moveInput.x > 0 && !IsFacingRight)
            IsFacingRight = true;
        else if (moveInput.x < 0 && IsFacingRight)
            IsFacingRight = false;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started) {
            IsRunning = true;
        }
        else if (context.canceled) {
            IsRunning = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (context.started) {
            IsJumping = true;
        }
        else if (context.canceled) {
            IsJumping = false;
        }
    }

    public void OnAttack(InputAction.CallbackContext context) {
        if (context.started) {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    public void OnHit(int damage, Vector2 knockback) {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    public void OnGravitySwitch(InputAction.CallbackContext context) {
        if (context.started && !gravity.OnCooldown) {
            AffectedByGravity[] everythingAffected = FindObjectsOfType<AffectedByGravity>();
            foreach (var obj in everythingAffected) {
                obj.OnGravityWasSwitched(gravityCooldown);
            }
        }
    }
}
