using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    GameObject player;
    PlayerController playerController;
    static PowerupManager _instance;
    public static PowerupManager instance 
    {
        get {
            if (_instance == null){
                _instance = GameObject.FindObjectOfType<PowerupManager>();
            }
            return _instance;
        }
    }
    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    public void ActivatePowerup(PowerupType type)
    {
        switch(type)
            {
                case PowerupType.health:
                    playerController.UpdateHealth(playerController.maxHealth);
                    break;
                case PowerupType.bulletSpeed:
                    playerController.bulletSpeed += 1;
                    break;
                default:
                    break;
            }
    }

    
}
