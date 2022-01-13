using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    public GameObject rocketPrefab;
    public float rocketFireRate;
    public int rocketPoolQuantity = 20;
    public Transform rocketSpawn;
   
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

        if (rocketFireRate <= 0){
            rocketFireRate = 1;
        }
    }

    private void Start()
    {
        PoolManager.instance.CreateNewPool(rocketPrefab, rocketPoolQuantity);
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
                case PowerupType.homingRocket:
                    StartCoroutine(LaunchRockets());
                    break;
                
                default:
                    break;
            }

        if (powerupScript.timedPowerup)
        {
// Fix bug where previous timer stops the next one - don't use StopAllCoroutines
            StartCoroutine(PowerupTimer(powerupScript.powerupDuration));
        }
    }

    IEnumerator PowerupTimer(float duration)
    {
        playerController.hasPowerup = true;
        yield return new WaitForSeconds(duration);
        playerController.hasPowerup = false;
        StopAllCoroutines();
    }

    IEnumerator LaunchRockets()
    {
        while(true)
        {
            PoolManager.instance.ReusePooledObject(rocketPrefab, rocketSpawn.position, player.transform.rotation);
            yield return new WaitForSeconds(1/rocketFireRate);
        }     
    }
}
