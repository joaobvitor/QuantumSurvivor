using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggerable : MonoBehaviour
{
    [SerializeField] private bool isPincerDoor;
    [SerializeField] private bool isHammerDoor;
    [SerializeField] private bool inverted;
    
    private Animator animator;
    private Collider2D collisionBox;

    void Awake() {
        animator = GetComponent<Animator>();
        collisionBox = GetComponent<Collider2D>();
    }

    public void OnTrigger() {
        if (isPincerDoor) {
            collisionBox.enabled = !inverted;
            animator.SetBool(AnimationStrings.wasTriggered, inverted);
        }

        if (isHammerDoor) {
            collisionBox.enabled = inverted;
            animator.SetBool(AnimationStrings.wasTriggered, !inverted);
        }
    }

    public void OnUntrigger() {
        if (isHammerDoor) {
            collisionBox.enabled = !inverted;
            animator.SetBool(AnimationStrings.wasTriggered, inverted);
        }
    }
}
