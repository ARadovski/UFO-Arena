using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DeathParticleType { none, smallDeath, largeDeath, bossDeath}
// Break up into separate specialized scripts?
public class EnemyScript : MonoBehaviour
{
    public DeathParticleType deathParticle;
    protected Rigidbody enemyRb;
    protected GameObject player;
    GameObject bulletMuzzle;
    protected PlayerController playerController;
    protected SpawnManager spawnManager;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected GameObject bulletSpawn;
    protected GameManager gameManager;
    public Slider healthSlider;
    public float forwardSpeed = 1;
    public float lateralSpeed;
    public float forceMultiplier = 2;
    public float health = 10;
    [SerializeField] float currentHealth;
    protected int scoreValue;
    public float crashDamage = 5;
    public int firingRate = 2;
    public float bulletSpeed = 10;
    public bool isShooter;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        spawnManager = FindObjectOfType<SpawnManager>();

        enemyRb = GetComponent<Rigidbody>();

        playerController = FindObjectOfType<PlayerController>();
        player = playerController.gameObject;
    }

    protected virtual void OnEnable()
    {
        currentHealth = health;
        healthSlider.maxValue = health;
        healthSlider.value = health;
        scoreValue = (int)health;

        if (isShooter)
        {
            StartCoroutine(FireWeapon());
        }
    }

    protected virtual void FixedUpdate()
    {
        Move();
        Rotate();
    }

    // Self-destruct on triggering invisible boundary
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "InvisibleBoundary")
        {
            OnKilled();
        }
    }

    protected virtual void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            UpdateHealth(-crashDamage);
            playerController.UpdateHealth(-crashDamage);
        }
    }

    protected virtual void Move()
    {
        // Moving toward the player
        enemyRb.AddForce((player.transform.position - transform.position).normalized * forwardSpeed * enemyRb.mass * forceMultiplier);

        // Moving laterally in relation to the player
        if (lateralSpeed != 0)
        {
            enemyRb.AddForce(transform.right * lateralSpeed * enemyRb.mass * forceMultiplier);
        }
        
    }

    protected virtual void Rotate()
    {
        // Looking at the player
        transform.LookAt(player.transform.position);
    }

    public virtual void UpdateHealth(float healthChange)
    {
        currentHealth += healthChange;
        healthSlider.value = currentHealth;
        if (currentHealth <= 0)
        {
            OnKilled();
        }     
    }

    protected IEnumerator FireWeapon()
    {
        while (true)
        {
            yield return new WaitForSeconds(firingRate);

            GameObject bullet = PoolManager.instance.ReusePooledObject(bulletPrefab, bulletSpawn.transform.position, transform.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

// Copied from PlayerManager - code repetition. Should PlayerManager instead inherit from EnemyScript?
            bulletMuzzle = PoolManager.instance.ReusePooledObject(PoolManager.instance.particlePool["Particle_BulletMuzzle"], bulletSpawn.transform.position, transform.rotation);
            bulletMuzzle.transform.SetParent(transform);
        }
        
    }

    public void OnKilled()
    {
// Replace switch statement with a conversion of enum type to particle name string?
        switch (deathParticle){
            case DeathParticleType.smallDeath:
                PoolManager.instance.ReusePooledObject(PoolManager.instance.particlePool["Particle_SmallDeath"], transform.position, transform.rotation);
                break;
            case DeathParticleType.largeDeath:
                PoolManager.instance.ReusePooledObject(PoolManager.instance.particlePool["Particle_LargeDeath"], transform.position, transform.rotation);
                break;
            case DeathParticleType.bossDeath:
                PoolManager.instance.ReusePooledObject(PoolManager.instance.particlePool["Particle_BossDeath"], transform.position, transform.rotation);
                break;
            default:
                break;
        }
        gameObject.SetActive(false);
        gameManager.UpdateScore(scoreValue);
        spawnManager.CountEnemies();
        // Stopping shooting
        StopAllCoroutines();
    }

}
