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

    public void ActivatePowerup(GameObject powerup)
    {
        Powerup powerupScript = powerup.GetComponent<Powerup>();
        PowerupType powerupType = powerupScript.powerupType;

        switch(powerupType)
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

        if (powerupScript.timedPowerup)
        {
            StartCoroutine(PowerupTimer(powerupScript.powerupDuration));
        }
    }

    IEnumerator PowerupTimer(float duration)
    {
        playerController.hasPowerup = true;
        yield return new WaitForSeconds(duration);
        playerController.hasPowerup = false;
    }
}
