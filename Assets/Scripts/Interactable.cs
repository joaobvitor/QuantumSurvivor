using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private bool unlocksGravitySwitch;
    [SerializeField] private bool unlocksStopTime;
    [SerializeField] private bool isDoor;
    [SerializeField] private bool setsEnemiesActive;
    [SerializeField] private bool setsDialogueBoxActive;
    [SerializeField] private bool hasKillCondition;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private string[] lines;
    [SerializeField] private Transform doorPosition;
    [SerializeField] private GameObject[] enemiesToActivate;
    [SerializeField] private GameObject[] enemiesToKill;
    [SerializeField] private string[] conditionDialogueLines;

    DetectionZone interactionZone;
    // Start is called before the first frame update
    void Awake()
    {
        interactionZone = GetComponent<DetectionZone>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoAction() {
        if (hasKillCondition && !killConditionMet()) {
            dialogueBox.GetComponent<DialogueBox>().lines = conditionDialogueLines;
            dialogueBox.SetActive(true);
        }
        else {
            if (unlocksGravitySwitch)
            interactionZone.detectedColliders[0].gameObject.GetComponentInParent<PlayerController>().UnlockGravitySwitch();
            if (unlocksStopTime)
                interactionZone.detectedColliders[0].gameObject.GetComponentInParent<PlayerController>().UnlockStopTime();
            if (isDoor)
                interactionZone.detectedColliders[0].gameObject.transform.position = doorPosition.position;
            if (setsEnemiesActive) {
                foreach (GameObject enemy in enemiesToActivate)
                    enemy.SetActive(true);
            }
            if (setsDialogueBoxActive) {
                dialogueBox.GetComponent<DialogueBox>().lines = lines;
                dialogueBox.SetActive(true);
            }
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
