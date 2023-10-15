using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AffectedByTime : MonoBehaviour
{

    private bool _timeIsStopped = false;

    public bool TimeIsStopped { 
        get {
            return _timeIsStopped;
        }
        private set {
            _timeIsStopped = value;
        }
    }

    [SerializeField]
    private float stopTimeDuration = 0f;

    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate() {
        if (stopTimeDuration > 0) {
            stopTimeDuration -= Time.deltaTime;
            rb.velocity = Vector2.zero;
            if (stopTimeDuration <= 0) {
                TimeIsStopped = false;
                if (animator != null) 
                    animator.enabled = true;
            }
        }
    }

    public void OnTimeWasStopped(float timeStopDurationToSet) {
        stopTimeDuration = timeStopDurationToSet;
        TimeIsStopped = true;
        if (animator != null) 
            animator.enabled = false;
    }
}
