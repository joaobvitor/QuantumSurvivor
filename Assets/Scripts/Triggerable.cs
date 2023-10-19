using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggerable : MonoBehaviour
{
    [SerializeField] private bool isPincerDoor;
    
    private Animator animator;
    private Collider2D collisionBox;

    void Awake() {
        animator = GetComponent<Animator>();
        collisionBox = GetComponent<Collider2D>();
    }

    public void OnTrigger() {
        if (isPincerDoor) {
            collisionBox.enabled = true;
            animator.SetBool(AnimationStrings.wasTriggered, true);
        }
    }
}
