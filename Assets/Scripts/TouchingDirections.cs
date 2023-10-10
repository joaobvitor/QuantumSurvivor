using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    public ContactFilter2D castFilter;
    Animator animator;
    public float groundDistance = 0.05f;
    public float wallDistance = 0.2f;
    public float ceilingDistance = 0.05f;
    CapsuleCollider2D touchingCol;
    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];
    Rigidbody2D rb;

    [SerializeField]
    private bool _isGrounded = true;
    public bool IsGrounded { 
        get {
            return _isGrounded;
        }
        private set {
            _isGrounded = value;
            animator.SetBool(AnimationStrings.isGrounded, value);

        }
    }

    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    [SerializeField]
    private bool _isOnWall = true;
    public bool IsOnWall { 
        get {
            return _isOnWall;
        }
        private set {
            _isOnWall = value;
            animator.SetBool(AnimationStrings.isOnWall, value);

        }
    }

    [SerializeField]
    private bool _isOnCeiling = true;
    public bool IsOnCeiling { 
        get {
            return _isOnCeiling;
        }
        private set {
            _isOnCeiling = value;
            animator.SetBool(AnimationStrings.isOnCeiling, value);

        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingCol = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Vector2 up = Vector2.up;
        Vector2 down = Vector2.down;
        if (rb.gravityScale < 0) {
            down = Vector2.up;
            up = Vector2.down;
        }

        IsGrounded = touchingCol.Cast(down, castFilter, groundHits, groundDistance) > 0;
        IsOnCeiling = touchingCol.Cast(up, castFilter, ceilingHits, ceilingDistance) > 0;
        IsOnWall = touchingCol.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;

        //this prevents unwanted behaviour if the collider overlaps with another
        if (IsGrounded && IsOnWall && IsOnCeiling) {
            IsOnWall = false;
            IsOnCeiling = false;
        }
    }

}
