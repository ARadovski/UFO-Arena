using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed;
    private Rigidbody playerRb;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameManager gameManager;
    public GameObject bulletSpawn;

    IEnumerator startFiring;

    private bool hasPowerup;
    private bool gameActive;
    public float powerupDuration = 7;

    
    public float bulletSpeed = 1;
    private float firingRate = .125f;
    private bool isFiring;

    public float health;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerRb.centerOfMass = new Vector3(0, 0, 0);
        healthSlider.maxValue = health;
        healthSlider.value = health;

        StartGame();
    }

    void Update()
    {
        if (gameManager.gameIsActive && gameActive)
        {
            MovePlayer();
            if (!isFiring && Input.GetMouseButtonDown(0))
            {
                startFiring = ShootWeapon();
                StartCoroutine(startFiring);
            }

            if (Input.GetMouseButtonUp(0))
            {
                StopCoroutine(startFiring);
                isFiring = false;
            }
        }

        if (health <= 0)
        {
            GameOver();
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (gameManager.gameIsActive)
        {
            LookAtMouse();
        }      
    }

    // Control player movement with forces to rigidbody
    void MovePlayer()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        playerRb.AddForce(Vector3.forward * verticalInput * playerSpeed);
        playerRb.AddForce(Vector3.right * horizontalInput * playerSpeed);
    }

    // Turn to look at mouse pointer raycast
    void LookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
        {
            transform.LookAt(new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z));
            /*transform.forward = raycastHit.point;*/
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Powerup")
        {
            Destroy(other.gameObject);
            StartCoroutine(PowerupTimer());

            if (other.gameObject.GetComponent<Powerup>().powerupType == PowerupType.health)
            {
                health = healthSlider.maxValue;
                healthSlider.value = healthSlider.maxValue;
            }

            if (other.gameObject.GetComponent<Powerup>().powerupType == PowerupType.bulletPower)
            {
                bulletSpeed += 1;
            }
        }
    }

    IEnumerator ShootWeapon()
    {
        
        while (Input.GetMouseButton(0))
        {
            isFiring = true;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.velocity = bullet.transform.forward * bulletSpeed;
            yield return new WaitForSeconds(firingRate);
        }
    }

    public void UpdateHealth(float changeAmount)
    {
        health += changeAmount;
        healthSlider.value += changeAmount;
        
    }

    void GameOver()
    {
        gameActive = false;
        gameManager.GameOver();
    }

    void StartGame()
    {
        gameActive = true;
    }

    IEnumerator PowerupTimer()
    {
        hasPowerup = true;
        yield return new WaitForSeconds(powerupDuration);
        hasPowerup = false;
    }
}
