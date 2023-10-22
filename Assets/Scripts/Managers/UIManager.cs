using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;
    public GameObject moneyTextPrefab;
    public Canvas gameCanvas;

    private void Awake() {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases) {
            if (canvas.tag == "GameCanvas") {
                gameCanvas = canvas;
                return;
            }
        }
    }

    private void OnEnable() {
        CharacterEvents.characterDamaged += CharacterTookDamage;
        CharacterEvents.characterHealed += CharacterHealed;
        CharacterEvents.characterMoneyChanged += CharacterMoneyChanged;
    }

    private void OnDisable() {
        CharacterEvents.characterDamaged -= CharacterTookDamage;
        CharacterEvents.characterHealed -= CharacterHealed;
        CharacterEvents.characterMoneyChanged -= CharacterMoneyChanged;
    }

    public void CharacterTookDamage(GameObject character, int damageReceived) {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = damageReceived.ToString();
    }

    public void CharacterHealed(GameObject character, int healthRestored) {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = healthRestored.ToString();
    }

    public void CharacterMoneyChanged(GameObject character, int money) {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(moneyTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();
        if (money >= 0)
            tmpText.text = "+" + money.ToString() + " coins";
        else
            tmpText.text = money.ToString() + " coins";
    }

    public void OnExitGame(InputAction.CallbackContext context) {
        if (context.started) {
            #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
                Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif

            #if (UNITY_EDITOR)
                UnityEditor.EditorApplication.isPlaying = false;
            #elif (UNITY_STANDALONE)
                Application.Quit();
            #elif (UNITY_WEBGL)
                SceneManager.LoadScene("QuitScene");
            #endif
        }

    }
}
