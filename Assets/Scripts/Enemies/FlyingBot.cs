using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBot : MonoBehaviour
{
    public float flightSpeed = 2f;
    public float waypointReachedDistance = 0.1f;
    public List<Transform> waypoints;
    public DetectionZone attackZone;

    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;
    AffectedByTime time;

    int waypointNum = 0;
    Transform nextWaypoint;

    private bool _hasTarget = false;

    public bool HasTarget {
        get {
            return _hasTarget;
        } 
        private set {
            animator.SetBool(AnimationStrings.hasTarget, value);
            _hasTarget = value;
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

    public bool CanMove { 
        get {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
        time = GetComponent<AffectedByTime>();
    }

    void Start()
    {
        nextWaypoint = waypoints[0];
    }

    // Update is called once per frame
    void Update() {
        HasTarget = attackZone.detectedColliders.Count > 0;
        if (AttackCooldown > 0)
            AttackCooldown -= Time.deltaTime;
    }

    private void FixedUpdate() {
        if (damageable.IsAlive && !time.TimeIsStopped) {
            if (CanMove) {
                Flight();
            }
            else {
                rb.velocity = Vector3.zero;
            }
        }
        else
            rb.velocity = Vector3.zero;
    }

    private void Flight() {
        Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

        float distance = Vector2.Distance(nextWaypoint.position, transform.position);

        rb.velocity = directionToWaypoint * flightSpeed;
        UpdateDirection();

        if (distance <= waypointReachedDistance) {
            waypointNum++;
            if (waypointNum >= waypoints.Count)
                waypointNum = 0;
            
            nextWaypoint = waypoints[waypointNum];    
        }
    }

    private void UpdateDirection() {
        if (transform.localScale.x > 0) {
            if (rb.velocity.x > 0)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else {
            if (rb.velocity.x < 0)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}
