using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool isPaused;
    public AudioSource pauseSound;
    public AudioSource resumeSound;

    [SerializeField] public UpgradeMenu UpgradeMenu;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(UpgradeMenu.IsOn){
                UpgradeMenu.Close();
                return;
            }
            if(isPaused){
                ResumeGame();
            }
            else{
                PauseGame();
            }
        }
    }

    public void PauseGame(){
        pauseMenu.SetActive(true);
        AudioSource.PlayClipAtPoint(pauseSound.clip, gameObject.transform.position, pauseSound.volume);
        PauseWithoutMenu();
    }

    public void PauseWithoutMenu(){
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame(){
        pauseMenu.SetActive(false);
        AudioSource.PlayClipAtPoint(resumeSound.clip, gameObject.transform.position, resumeSound.volume);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitGame(){
        Application.Quit();
    }

}
