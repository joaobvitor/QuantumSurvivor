using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class upgrades : MonoBehaviour
{
    //mudar a variavel gold para: public Script Script onde Script.gold 
    //e a maneira de aceder 
    [SerializeField]
    public int gold;

    public PlayerController PlayerController;

    public bool UpgradedBlackholeRange = false;
    public bool UpgradedTimeStopCooldown = false;
    public bool UpgradedGravitySwitchCooldown = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        gold = 5000;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpgradeBlackholeRange(){
        if(!UpgradedBlackholeRange && gold > 500){
            gold -= 500;
            PlayerController.blackholeRange += 3f;
            UpgradedBlackholeRange = true;
        }

    }

    public void UpgradeTimeStopCooldown(){
        if(!UpgradedTimeStopCooldown && gold > 500){
            gold -= 500;
            PlayerController.stopTimeCooldown -= 5f;
            UpgradedTimeStopCooldown = true;
        }
    }

    public void UpgradeGravitySwitchCooldown(){
        if(!UpgradedGravitySwitchCooldown && gold > 500){
            gold -= 500;
            PlayerController.gravityCooldown -= 0.3f;
            UpgradedGravitySwitchCooldown = true;
        }
    }

}
