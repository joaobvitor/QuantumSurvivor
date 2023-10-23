using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;



[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public UnityEvent<string, float> abilityUsed;
    public UnityEvent<string> abilityUnlocked;
    public UnityEvent<int> moneyChanged;

    [SerializeField]
    private int _money = 0;
    public int Money {
        get {
            return _money;
        }
        set {
            CharacterEvents.characterMoneyChanged.Invoke(gameObject, value - Money);
            moneyChanged?.Invoke(value);
            _money = value;
        }
    }

    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float speed;
    public float jumpImpulse = 10f;

    public float gravityCooldown = 0.5f;
    Vector2 moveInput;
    TouchingDirections touchingDirections;
    Damageable damageable;
    AffectedByGravity gravity;

    [SerializeField]
    public PauseMenu PauseMenu;

    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private string[] lines;

    public float CurrentMoveSpeed { get
        {
            if (CanMove) {
                if (IsMoving && !touchingDirections.IsOnWall) {
                    if (touchingDirections.IsGrounded) {
                        if (IsRunning)
                            speed = runSpeed;
                        else 
                            speed = walkSpeed;
                    }
                    else if (!IsRunning)
                        speed = walkSpeed;
                    
                    return speed;
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

    [SerializeField] private bool blackholeUnlocked = true;
    [SerializeField] private bool gravitySwitchUnlocked = false;
    [SerializeField] private bool stopTimeUnlocked = false;

    Rigidbody2D rb;
    Animator animator;
    DetectionZone interactableDetectionZone;
    
    public Sprite deathSprite;
    [SerializeField] private int numCheckpoints = 1;
    
    public GameObject blackholePrefab;
    GameObject blackhole;

    public float blackholeMaxHeat = 5f;
    public float blackholeHeat = 0f;
    public float blackholeCooldown = 10f;
    public float blackholeCurrentCooldown = 0f;
    public float blackholeRechargeRate = 1f;
    public float blackholeRange = 5f;
    public float blackholeScale = 0f;
    private bool blackholeCall;
    private bool blackholeEnter;
    public int extraDamage = 0;

    public bool gravityHealUpgrade = false;
    
    [SerializeField]
    private float stopTimeDuration = 3f;
    [SerializeField]
    public float stopTimeCooldown = 15f;
    private float stopTimeCurrentCooldown = 0.5f;

    [SerializeField] OverheatBar overheatBar;
    private RectTransform bar;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
        gravity = GetComponent<AffectedByGravity>();
        overheatBar = GetComponentInChildren<OverheatBar>();
        interactableDetectionZone = GetComponent<DetectionZone>();
        bar = overheatBar.GetComponent<RectTransform>();
        overheatBar.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (numCheckpoints > 1)
            UnlockGravitySwitchOnRespawn();
    }

    // Update is called once per frame
    void Update()
    {
        if(!PauseMenu.isPaused) {
            if (!IsAlive) {
                GetComponent<SpriteRenderer>().sprite = deathSprite;
                dialogueBox.GetComponent<DialogueBox>().lines = lines;
                dialogueBox.SetActive(true);
                Invoke("RespawnPlayer", 5f);
            }

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
    }

    private void RespawnPlayer() {
        SceneManager.LoadScene(numCheckpoints);
    }

    private void BlackholeBehaviour() {
        if (blackholeHeat >= blackholeMaxHeat) {
                BlackholeIsActive = false;
                Destroy(blackhole);
                blackholeCurrentCooldown = blackholeCooldown;
                blackholeHeat = 0;
                blackholeEnter = !blackholeEnter;
                abilityUsed?.Invoke("Blackhole", blackholeCooldown);
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
        if(!PauseMenu.isPaused) {
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
            animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);

            if (IsJumping && touchingDirections.IsGrounded && CanMove) {
                animator.SetTrigger(AnimationStrings.jumpTrigger);
                rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(!PauseMenu.isPaused) {
            moveInput = context.ReadValue<Vector2>();

            if (IsAlive) {
                IsMoving = (moveInput != Vector2.zero);
                SetFacingDirection(moveInput);
            }
        }
    }

    private void SetFacingDirection(Vector2 moveInput) {
        if (moveInput.x > 0 && !IsFacingRight) {
            IsFacingRight = true;
            bar.localScale = new Vector2 (-bar.localScale.x, bar.localScale.y);
        }
        else if (moveInput.x < 0 && IsFacingRight) {
            IsFacingRight = false;
            bar.localScale = new Vector2 (-bar.localScale.x, bar.localScale.y);
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if(!PauseMenu.isPaused) {
            if (context.started) {
                IsRunning = true;
            }
            else if (context.canceled) {
                IsRunning = false;
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context) {
        if(!PauseMenu.isPaused) {
            if (IsAlive) {
                if (context.started) {
                    IsJumping = true;
                }
                else if (context.canceled) {
                    IsJumping = false;
                }
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context) {
        if(!PauseMenu.isPaused) {
            if (context.started) {
                animator.SetTrigger(AnimationStrings.attackTrigger);
            }
        }
    }

    public void OnHit(int damage, Vector2 knockback) {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    public void OnGravitySwitch(InputAction.CallbackContext context) {
        if(!PauseMenu.isPaused) {
            if (context.started && IsAlive && !gravity.OnCooldown && gravitySwitchUnlocked) {
                AffectedByGravity[] everythingAffected = FindObjectsOfType<AffectedByGravity>();
                foreach (var obj in everythingAffected) {
                    obj.OnGravityWasSwitched(gravityCooldown);
                }
                jumpImpulse = -jumpImpulse;
                abilityUsed?.Invoke("GravitySwitch", gravityCooldown);
                if (gravityHealUpgrade)
                    damageable.Heal(1);
            }
        }
    }

    public void OnBlackhole(InputAction.CallbackContext context) {
        if(!PauseMenu.isPaused) {
            if (IsAlive && blackholeCurrentCooldown <= 0 && blackholeUnlocked) {
                if (blackholeCall == blackholeEnter) {
                    blackholeEnter = !blackholeEnter;
                    BlackholeIsActive = !BlackholeIsActive;

                    if (BlackholeIsActive) {
                        blackhole = Instantiate(blackholePrefab,  Input.mousePosition, blackholePrefab.transform.rotation);
                        blackhole.GetComponent<Attack>().attackDamage += extraDamage;
                        blackhole.transform.localScale = new Vector2(blackhole.transform.localScale.x + blackholeScale,
                                                                    blackhole.transform.localScale.y + blackholeScale);
                    }
                    else {
                        Destroy(blackhole);
                    }
                }
            }
        blackholeCall = !blackholeCall;
        }
    }

    public void OnStopTime(InputAction.CallbackContext context) {
        if(!PauseMenu.isPaused) {
            if (context.started && IsAlive && stopTimeCurrentCooldown <= 0 && stopTimeUnlocked) {
                AffectedByTime[] everythingAffected = FindObjectsOfType<AffectedByTime>();
                foreach (var obj in everythingAffected) {
                    obj.OnTimeWasStopped(stopTimeDuration);
                }
                stopTimeCurrentCooldown = stopTimeCooldown;
                abilityUsed?.Invoke("StopTime", stopTimeCooldown);
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context) {
        if(!PauseMenu.isPaused) {
            if (context.started && IsAlive) {
                foreach (Collider2D col in interactableDetectionZone.detectedColliders) {
                    if (col != null)
                        col.gameObject.GetComponentInParent<Interactable>()?.DoAction();
                }
            }
        }
    }

    public void UnlockGravitySwitch() {
        if (!gravitySwitchUnlocked) {
            gravitySwitchUnlocked = true;
            numCheckpoints++;
            abilityUnlocked?.Invoke("GravitySwitch");
        }
    }

    public void UnlockStopTime() {
        if (!stopTimeUnlocked) {
            stopTimeUnlocked = true;
            numCheckpoints++;
            abilityUnlocked?.Invoke("StopTime");
        }
    }

    public void UnlockGravitySwitchOnRespawn() {
        if (!gravitySwitchUnlocked) {
            gravitySwitchUnlocked = true;
            abilityUnlocked?.Invoke("GravitySwitch");
        }
    }
}