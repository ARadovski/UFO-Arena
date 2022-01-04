using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerupType { none, health, bulletPower, bulletSpeed, playerSpeed, maxHealth, lazer, homingRocket, cannon}

public class Powerup : MonoBehaviour
{
    public PowerupType powerupType;
    public bool timedPowerup;
    public float powerupDuration = 5;

}
