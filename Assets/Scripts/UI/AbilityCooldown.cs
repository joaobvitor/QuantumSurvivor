using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cooldown : MonoBehaviour
{
    [SerializeField]
    private Image imageCooldown;
    [SerializeField]
    private Image imageEdge;

    [SerializeField]
    private TMP_Text textCooldown;

    private bool onCooldown = true;
    private float cooldownTime = 10f;
    private float cooldownTimer = 10f;

    // Start is called before the first frame update
    void Start()
    {
        textCooldown.gameObject.SetActive(true);
        imageEdge.gameObject.SetActive(true);
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
            textCooldown.text = Mathf.RoundToInt(cooldownTimer).ToString();
            imageCooldown.fillAmount = cooldownTimer / cooldownTime;
            imageEdge.transform.localEulerAngles = new Vector3(0, 0, 360f * (cooldownTimer / cooldownTime));
        }

        cooldownTimer -= Time.deltaTime;
    }

    public void OnAbilityUse(float cooldownToSet) {
        onCooldown = true;
        textCooldown.gameObject.SetActive(true);
        cooldownTimer = cooldownToSet;
        cooldownTime = cooldownToSet;
    }
}
