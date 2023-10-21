using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] private bool hasTriggerableOnce;
    [SerializeField] private bool hasTriggerable;
    [SerializeField] private Triggerable triggerable;
    [SerializeField] private bool hasTriggerableSet;
    [SerializeField] private Triggerable[] triggerableSet;

    [SerializeField] private bool setsEnemiesActive;
    [SerializeField] private GameObject[] enemiesToActivate;

    [SerializeField] private bool hasKillCondition;
    [SerializeField] private GameObject[] enemiesToKill;

    [SerializeField] private bool setsDialogueBoxActive;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private string[] lines;
    private bool wasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if (hasKillCondition && killConditionMet()) {
            if (hasTriggerableOnce && !wasTriggered) {
                triggerable.OnTrigger();
            }
            wasTriggered = true;
        }
        else if (!hasKillCondition) {
            if (hasTriggerableOnce && !wasTriggered) {
                triggerable.OnTrigger();
            }

            if (hasTriggerable) {
                triggerable.OnTrigger();
            }
            
            if (setsEnemiesActive && !wasTriggered) {
                foreach (GameObject enemy in enemiesToActivate)
                    enemy.SetActive(true);
            }

            if (setsDialogueBoxActive && !wasTriggered) {
                dialogueBox.GetComponent<DialogueBox>().lines = lines;
                dialogueBox.SetActive(true);
            }
            if (hasTriggerableSet) {
                foreach (Triggerable trigger in triggerableSet) 
                    trigger.OnTrigger();
            }

            wasTriggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (hasTriggerable) {
            triggerable.OnUntrigger();
        }

        if (hasTriggerableSet) {
            foreach (Triggerable trigger in triggerableSet) 
                trigger.OnUntrigger();
        }
    }

    bool killConditionMet() {
        foreach (GameObject enemy in enemiesToKill) {
            if (enemy != null)
                return false;
        }

        return true;
    }
}
