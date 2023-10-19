using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] private Triggerable triggerable;
    private bool wasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if (!wasTriggered) {
            triggerable.OnTrigger();
            wasTriggered = true;
        }
    }
}
