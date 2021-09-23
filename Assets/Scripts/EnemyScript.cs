using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    private Rigidbody enemyRb;
    private GameObject player;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bulletSpawn;
    private GameManager gameManager;
    public Slider healthSlider;

    public float forwardSpeed = 1;
    public float lateralSpeed;
    public float forceMultiplier = 2;
    public float health;
    
    private int scoreValue;

    public float bounceForce = 1000;
    public float crashDamage = 5;
    public int firingRate = 2;
    public float bulletSpeed = 10;

    public bool isShooter;
    
    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
       
        healthSlider.maxValue = health;
        healthSlider.value = health;

        scoreValue = (int)health;

        if (isShooter)
        {
            StartCoroutine(FireWeapon());
        }
    }

    void Update()
    {
        Move();
        Rotate();
        
        if (health <= 0)
        {
            Destroy(gameObject);
            gameManager.UpdateScore(scoreValue);
        }
        
    }

    // Self-destruct on triggering invisible boundary
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "InvisibleBoundary")
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            UpdateHealth(-crashDamage);
            col.gameObject.GetComponent<PlayerController>().UpdateHealth(-crashDamage);
            /*enemyRb.AddForce((transform.position - col.transform.position).normalized * bounceForce, ForceMode.Impulse);*/
        }
    }

    private void Move()
    {
        // Moving toward the player
        //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, forwardSpeed * Time.deltaTime);
        enemyRb.AddForce((player.transform.position - transform.position).normalized * forwardSpeed * enemyRb.mass * forceMultiplier);


        // Moving laterally in relation to the player
        enemyRb.AddForce(transform.right * lateralSpeed * enemyRb.mass * forceMultiplier);
    }

    void Rotate()
    {
        // Looking at the player
        transform.LookAt(player.transform.position);
    }

    public void UpdateHealth(float healthChange)
    {
        health += healthChange;
        healthSlider.value = health;
    }

    private IEnumerator FireWeapon()
    {
        while (true)
        {
            yield return new WaitForSeconds(firingRate);
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.transform.position, transform.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
        }
        
    }

}
