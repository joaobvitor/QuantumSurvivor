using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OverheatBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Awake() {

    }

    public void UpdateOverheatBar(float heat, float maxHeat) {
        if (heat > 0 && heat < maxHeat) {
            slider.value = heat / maxHeat;
            gameObject.SetActive(true);
        }
        else
            gameObject.SetActive(false);
    }
}
