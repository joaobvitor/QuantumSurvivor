using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] private bool hasTriggerable;
    [SerializeField] private Triggerable triggerable;

    [SerializeField] private bool setsEnemiesActive;
    [SerializeField] private GameObject[] enemiesToActivate;

    [SerializeField] private bool setsDialogueBoxActive;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private string[] lines;
    private bool wasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if (hasTriggerable && !wasTriggered) {
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

        wasTriggered = true;
    }
}
