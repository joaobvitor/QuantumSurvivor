using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private bool unlocksGravitySwitch;
    [SerializeField] private bool unlocksStopTime;
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
        if (unlocksGravitySwitch)
            interactionZone.detectedColliders[0].gameObject.GetComponentInParent<PlayerController>().UnlockGravitySwitch();
        else if (unlocksStopTime)
            interactionZone.detectedColliders[0].gameObject.GetComponentInParent<PlayerController>().UnlockStopTime();
    }
}
