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

    [SerializeField]
    private bool _blackholeIsActive = false;

    public bool BlackholeIsActive { 
        get {
            return _blackholeIsActive;
        }
        private set {
            blackholeCurrentCooldown = 0;
            _blackholeIsActive = value;
        }
    }

    Rigidbody2D rb;
    Animator animator;
    
    public GameObject blackholePrefab;
    GameObject blackhole;

    public float blackholeMaxHeat = 5f;
    public float blackholeHeat = 0f;
    public float blackholeCooldown = 10f;
    public float blackholeCurrentCooldown = 0f;
    public float blackholeRechargeRate = 1f;
    public float blackholeRange = 5f;
    private bool blackholeCall;
    private bool blackholeEnter;
    
    [SerializeField]
    private float stopTimeDuration = 3f;
    [SerializeField]
    private float stopTimeCooldown = 15f;
    private float stopTimeCurrentCooldown = 0;

    [SerializeField] OverheatBar overheatBar;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
        gravity = GetComponent<AffectedByGravity>();
        overheatBar = GetComponentInChildren<OverheatBar>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (BlackholeIsActive && blackholeCurrentCooldown == 0) {
            blackholeHeat += Time.deltaTime;
            BlackholeBehaviour();
        }
        else {
            if (blackholeHeat > 0)
                blackholeHeat -= Time.deltaTime * blackholeRechargeRate;
        }

        if (blackholeCurrentCooldown > 0)
            blackholeCurrentCooldown -= Time.deltaTime;

        overheatBar.UpdateOverheatBar(blackholeHeat, blackholeMaxHeat);

        if (stopTimeCurrentCooldown > 0)
            stopTimeCurrentCooldown -= Time.deltaTime;
    }

    private void BlackholeBehaviour() {
        if (blackholeHeat >= blackholeMaxHeat) {
                BlackholeIsActive = false;
                Destroy(blackhole);
                blackholeCurrentCooldown = blackholeCooldown;
                blackholeHeat = 0;
                blackholeEnter = !blackholeEnter;
            }
        else {
            Vector3 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            mousePos.z = 0;
            if ((transform.position - mousePos).magnitude <= blackholeRange)
                blackhole.transform.position = mousePos;
            else
                blackhole.transform.position = transform.position + Vector3.Scale((mousePos - transform.position).normalized, new Vector3(blackholeRange, blackholeRange, 0));
        }
    }

    private void FixedUpdate()
    {
        //if (!damageable.IsHit)
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

    public void OnBlackhole(InputAction.CallbackContext context) {
        if (blackholeCurrentCooldown <= 0) {
            if (blackholeCall == blackholeEnter) {
                blackholeEnter = !blackholeEnter;
                BlackholeIsActive = !BlackholeIsActive;

                if (BlackholeIsActive) {
                    blackhole = Instantiate(blackholePrefab,  Input.mousePosition, blackholePrefab.transform.rotation);
                }
                else {
                    Destroy(blackhole);
                }
            }
        }

        blackholeCall = !blackholeCall;
    }

    public void OnStopTime(InputAction.CallbackContext context) {
        Debug.Log("dadada");
        if (context.started && stopTimeCurrentCooldown <= 0) {
            AffectedByTime[] everythingAffected = FindObjectsOfType<AffectedByTime>();
            foreach (var obj in everythingAffected) {
                Debug.Log("dadada");
                obj.OnTimeWasStopped(stopTimeDuration);
            }
            stopTimeCurrentCooldown = stopTimeCooldown;
        }
    }
}
