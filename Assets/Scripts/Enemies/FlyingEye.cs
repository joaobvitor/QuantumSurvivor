using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : MonoBehaviour
{
    public float flightSpeed = 2f;
    public float waypointReachedDistance = 0.1f;
    public DetectionZone biteDetectionZone;
    public List<Transform> waypoints;
    public Collider2D deathCollider;
    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;

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
    }

    void Start()
    {
        nextWaypoint = waypoints[0];
    }

    // Update is called once per frame
    void Update()
    {
        HasTarget = biteDetectionZone.detectedColliders.Count > 0;
    }

    private void FixedUpdate() {
        if (damageable.IsAlive) {
            if (CanMove) {
                Flight();
            }
            else {
                rb.velocity = Vector3.zero;
            }
        }
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
            if (rb.velocity.x < 0)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else {
            if (rb.velocity.x > 0)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    public void OnDeath() {
        rb.gravityScale = 2;
        rb.velocity = new Vector2(0, rb.velocity.y);
        deathCollider.enabled = true;
    }
}
