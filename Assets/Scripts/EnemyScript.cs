using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    protected Rigidbody enemyRb;
    protected GameObject player;
    protected PlayerController playerController;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] int bulletPoolQuantity = 60;
    [SerializeField] protected GameObject bulletSpawn;
    protected GameManager gameManager;
    public Slider healthSlider;
    public float forwardSpeed = 1;
    public float lateralSpeed;
    public float forceMultiplier = 2;
    public float health;
    protected int scoreValue;
    public float crashDamage = 5;
    public int firingRate = 2;
    public float bulletSpeed = 10;
    public bool isShooter;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        enemyRb = GetComponent<Rigidbody>();

        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();

        PoolManager.instance.CreateNewPool(bulletPrefab, bulletPoolQuantity);
    }
    protected virtual void Start()
    {       
        healthSlider.maxValue = health;
        healthSlider.value = health;

        scoreValue = (int)health;

        if (isShooter)
        {
            StartCoroutine(FireWeapon());
        }
    }

    protected virtual void Update()
    {
        if (health <= 0)
        {
            Disable();
            gameManager.UpdateScore(scoreValue);
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
            Disable();
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
        enemyRb.AddForce(transform.right * lateralSpeed * enemyRb.mass * forceMultiplier);
    }

    protected virtual void Rotate()
    {
        // Looking at the player
        transform.LookAt(player.transform.position);
    }

    public virtual void UpdateHealth(float healthChange)
    {
        health += healthChange;
        healthSlider.value = health;
    }

    protected IEnumerator FireWeapon()
    {
        while (true)
        {
            yield return new WaitForSeconds(firingRate);

            GameObject bullet = PoolManager.instance.ReusePooledObject(bulletPrefab, bulletSpawn.transform.position, transform.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
        }
        
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

}
