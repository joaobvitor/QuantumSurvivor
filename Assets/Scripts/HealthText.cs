using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthText : MonoBehaviour
{
    public Vector3 moveSpeed = new Vector3(0, 75, 0);
    public float timeToFade = 1f;
    private float timeElapsed = 0f;
    RectTransform textTranform;
    TextMeshProUGUI textMeshPro;
    private Color startColor;

    private void Awake() {
        textTranform = GetComponent<RectTransform>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
        startColor = textMeshPro.color;
    }

    // Update is called once per frame
    void Update()
    {
        textTranform.position += moveSpeed * Time.deltaTime;

        timeElapsed += Time.deltaTime;

        if (timeElapsed < timeToFade) {
            float fadeAlpha = startColor.a * (1 - timeElapsed / timeToFade);
            textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, fadeAlpha);
        }
        else
            Destroy(gameObject);
    }
}
