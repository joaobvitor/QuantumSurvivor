using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinsUIDisplay : MonoBehaviour
{
    public TMP_Text coinsText;
    PlayerController playerController;

    private void Awake() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        coinsText.text = "Coins: " + playerController.Money;
    }

    private void OnEnable() {
        playerController.moneyChanged.AddListener(OnPlayerMoneyChanged);
    }

    private void OnDisable() {
        playerController.moneyChanged.RemoveListener(OnPlayerMoneyChanged);
    }

    private void OnPlayerMoneyChanged(int money) {
        coinsText.text = "Coins: " + money;
    }
}
