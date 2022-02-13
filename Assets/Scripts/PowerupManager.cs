using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerupManager : MonoBehaviour
{
    public GameObject rocketPrefab;
    public float rocketFireRate;
    public int rocketPoolQuantity = 20;
    public Transform rocketSpawn;

    Coroutine launchRockets;
    Coroutine shootLaser;
    Coroutine changeBulletRate;
    static event Action OnLaserExpired;
   
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
        playerController = FindObjectOfType<PlayerController>();
        player = playerController.gameObject;

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
                    playerController.bulletSpeed += 3;
                    break;
                case PowerupType.bulletRate:
                    changeBulletRate = StartCoroutine(ChangeBulletRate(powerupScript.powerupDuration));
                    break;
                case PowerupType.homingRocket:
                    launchRockets = StartCoroutine(LaunchRockets(powerupScript.powerupDuration));
                    break;
// Bug: after having 2 lazer powerups concurrently lazer muzzle flash does not disappear 
                case PowerupType.lazer:
                    if(shootLaser != null)
                    {
                        StopCoroutine(shootLaser);
                    }
                    shootLaser = StartCoroutine(Lazer(powerupScript.powerupDuration));
                    break;
                default:
                    break;
            }

// Not using this temporatily
        if (powerupScript.timedPowerup)
        {
            //StartCoroutine(PowerupTimer(powerupScript.powerupDuration));
        }
    }

// Not using this temporatily
    IEnumerator PowerupTimer(float duration)
    {
        playerController.hasPowerup = true;
        yield return new WaitForSeconds(duration);
        playerController.hasPowerup = false;
// Stop individual named coroutines, otherwise multiple powerups get all cancelled
        //StopAllCoroutines();
    }

    IEnumerator ChangeBulletRate(float duration)
    {
        playerController.bulletFireRate *= 2;
        yield return new WaitForSeconds(duration);
        playerController.bulletFireRate /= 2;
    }

    IEnumerator LaunchRockets(float duration)
    {
        float t = 0;
        while(t <= duration)
        {
            PoolManager.instance.ReusePooledObject(rocketPrefab, rocketSpawn.position, player.transform.rotation);
            yield return new WaitForSeconds(1/rocketFireRate);
            t += 1/rocketFireRate;
        }     
    }

    public IEnumerator Lazer(float timer)
    {
// LAZER METHOD GOES HERE
        playerController.lazerOn = true;
        if (playerController.isFiring){
            playerController.TurnLaserOn();
        }
        Debug.Log("Lazer ON");
// Handle wait through PowerupTimer?
        yield return new WaitForSeconds(timer);
// Handle with an Action instead?
        // if (OnLaserExpired != null)
        // {
        //     OnLaserExpired();
        // }
        playerController.lazerOn = false;
    }
}
