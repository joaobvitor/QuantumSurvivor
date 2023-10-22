using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    public GameObject upgradeMenu;
    public bool IsOn;

    [SerializeField] public PauseMenu PauseMenu;

    public PlayerController playerController;

    public bool upgradedBlackholeRange = false;
    public bool upgradedTimeStopCooldown = false;
    public bool upgradedGravitySwitchCooldown = false;

    public AudioSource openSound;
    public AudioSource closeSound;
    public AudioSource buySound;
    public AudioSource cantBuySound;

    // Start is called before the first frame update    
    void Start()
    {
        upgradeMenu.SetActive(false);
        upgradedBlackholeRange = false;
        upgradedTimeStopCooldown = false;
        upgradedGravitySwitchCooldown = false;
    }

    void GetPlayerController() {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void Close()
    {
        IsOn = false;
        AudioSource.PlayClipAtPoint(closeSound.clip, gameObject.transform.position, closeSound.volume);
        upgradeMenu.SetActive(false);
        PauseMenu.ResumeGame();
    }

    public void Open()
    {
        IsOn = true;
        AudioSource.PlayClipAtPoint(openSound.clip, gameObject.transform.position, openSound.volume);
        upgradeMenu.SetActive(true);
        PauseMenu.PauseWithoutMenu();
    }

    public void UpgradeBlackholeRange() {
        GetPlayerController();

        if(!upgradedBlackholeRange && playerController.Money >= 5) {
            playerController.Money -= 5;
            playerController.blackholeRange += 3f;
            upgradedBlackholeRange = true;
            AudioSource.PlayClipAtPoint(buySound.clip, gameObject.transform.position, buySound.volume);
        }
        else 
            AudioSource.PlayClipAtPoint(cantBuySound.clip, playerController.gameObject.transform.position, cantBuySound.volume);

    }

    public void UpgradeTimeStopCooldown() {
        GetPlayerController();

        if(!upgradedTimeStopCooldown && playerController.Money >= 5) {
            playerController.Money -= 5;
            playerController.stopTimeCooldown -= 5f;
            upgradedTimeStopCooldown = true;
            AudioSource.PlayClipAtPoint(buySound.clip, gameObject.transform.position, buySound.volume);
        }
        else
            AudioSource.PlayClipAtPoint(cantBuySound.clip, playerController.gameObject.transform.position, cantBuySound.volume);
    }

    public void UpgradeGravitySwitchCooldown() {
        GetPlayerController();

        if(!upgradedGravitySwitchCooldown && playerController.Money >= 5) {
            playerController.Money -= 5;
            playerController.gravityCooldown -= 0.3f;
            upgradedGravitySwitchCooldown = true;
            AudioSource.PlayClipAtPoint(buySound.clip, gameObject.transform.position, buySound.volume);
        }
        else
            AudioSource.PlayClipAtPoint(cantBuySound.clip, playerController.gameObject.transform.position, cantBuySound.volume);
    }
}
