using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    public GameObject upgradeMenu;
    public bool IsOn;

    [SerializeField]
    public PauseMenu PauseMenu;

    [SerializeField]
    public upgrades upgrades;

    // Start is called before the first frame update    
    void Start()
    {
        upgradeMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void close()
    {
        IsOn = false;
        upgradeMenu.SetActive(false);
        PauseMenu.ResumeGame();
    }

    public void open()
    {
        IsOn = true;
        upgradeMenu.SetActive(true);
        PauseMenu.PauseWithoutMenu();
    }
}
