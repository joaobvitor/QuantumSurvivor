using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Cooldown : MonoBehaviour
{
    [SerializeField]
    private Image imageCooldown;
    [SerializeField]
    private Image imageEdge;

    [SerializeField]
    private TMP_Text textCooldown;

    [SerializeField]
    private string ability;

    private bool onCooldown = false;
    private float cooldownTime = 10f;
    private float cooldownTimer = 0f;

    private PlayerController playerController;

    private void Awake() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        textCooldown.gameObject.SetActive(false);
        imageEdge.gameObject.SetActive(false);
        imageCooldown.fillAmount = 0f;
    }

    // Update is called once per frame
    void Update() {
        if (onCooldown)
            ApplyCooldown();
    }

    void ApplyCooldown() {
        if (cooldownTimer <= 0) {
            onCooldown = false;
            textCooldown.gameObject.SetActive(false);
            imageEdge.gameObject.SetActive(false);
            imageCooldown.fillAmount = 0f;
        }
        else {
            if (cooldownTimer >= 1)
                textCooldown.text = Mathf.RoundToInt(cooldownTimer).ToString();
            else
                textCooldown.text = Math.Round((decimal)cooldownTimer, 1).ToString();
            imageCooldown.fillAmount = cooldownTimer / cooldownTime;
            imageEdge.transform.localEulerAngles = new Vector3(0, 0, 360f * (cooldownTimer / cooldownTime));
        }

        cooldownTimer -= Time.deltaTime;
    }
    
    private void OnEnable() {
        playerController.abilityUsed.AddListener(OnAbilityUse);
    }

    private void OnDisable() {
        playerController.abilityUsed.RemoveListener(OnAbilityUse);
    }

    public void OnAbilityUse(string usedAbility, float cooldownToSet) {
        if (usedAbility == ability) {
            onCooldown = true;
            textCooldown.gameObject.SetActive(true);
            cooldownTimer = cooldownToSet;
            cooldownTime = cooldownToSet;
        }
    }
}
