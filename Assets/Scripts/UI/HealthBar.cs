using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text healthBarText;
    Damageable playerDamageable;
    [SerializeField] private Image healthFill;
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private Image backgroundColor;
    [SerializeField] private Gradient backgroundGradient;

    private void Awake() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerDamageable = player.GetComponent<Damageable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        healthSlider.value = (float)playerDamageable.Health / (float)playerDamageable.MaxHealth;
        healthBarText.text = "HP " + playerDamageable.Health + " / " + playerDamageable.MaxHealth;
        CheckHealthBarColor();
    }

    private void OnEnable() {
        playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
    }

    private void OnDisable() {
        playerDamageable.healthChanged.RemoveListener(OnPlayerHealthChanged);
    }

    private void OnPlayerHealthChanged(int newHealth, int maxHealth) {
        healthSlider.value = (float)newHealth / (float)maxHealth;
        healthBarText.text = "HP " + newHealth + " / " + maxHealth;
        CheckHealthBarColor();
    }

    private void CheckHealthBarColor() {
        healthFill.color = healthGradient.Evaluate(healthSlider.value);
        backgroundColor.color = backgroundGradient.Evaluate(healthSlider.value);
    }
}
