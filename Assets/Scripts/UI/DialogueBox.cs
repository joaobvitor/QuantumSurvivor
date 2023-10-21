using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueBox : MonoBehaviour
{   
    [SerializeField]
    public PauseMenu PauseMenu;
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    private int index;

    GameObject player;

    // Start is called before the first frame update
    void OnEnable()
    {
        textComponent.text = string.Empty;
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<Animator>().enabled = false;
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {   
        if(!PauseMenu.isPaused){
            if (Input.GetMouseButtonDown(0)) {
                if (textComponent.text == lines[index])
                    NextLine();
                else {
                    StopAllCoroutines();
                    textComponent.text = lines[index];
                }
            }
        }
    }

    void StartDialogue() {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine() {
        foreach (char c in lines[index].ToCharArray()) {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine() {
        if (index < lines.Length - 1) {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else {
            gameObject.SetActive(false);
            player.GetComponent<PlayerController>().enabled = true;
        player.GetComponent<Animator>().enabled = true;
        }
    }
}
