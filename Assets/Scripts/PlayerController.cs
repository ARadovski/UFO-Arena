using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;

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

    public float health = 10;

    public static event System.Action OnPlayerKilled;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerRb.centerOfMass = new Vector3(transform.position.x, 0, transform.position.z); 
        healthSlider.maxValue = health;
        healthSlider.value = health;

        StartGame();
    }

    void Update()
    {
// Should be done without accessing gameManager?
        if (gameManager.gameIsActive && gameActive)
        {
            MovePlayer();
// Abstract away into FireWeapon method?
            
            if (!isFiring && (Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.Space)))
            {
                startFiring = ShootWeapon();
                StartCoroutine(startFiring);
            }

            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
            {
                StopCoroutine(startFiring);
                isFiring = false;
            }
        }

        if (health <= 0)
        {
            if(OnPlayerKilled !=null)
            {
                OnPlayerKilled();
            }
            PlayerKilled();
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
// Do without accessing gameManager?
        if (gameManager.gameIsActive)
        {
            LookAtMouse();
        }      
    }

    // Control player movement with forces to rigidbody
    void MovePlayer()
    {
        Vector3 playerInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        playerRb.AddForce(playerInput * playerSpeed * Time.deltaTime);
        //Debug.Log(playerRb.velocity.magnitude);
    }

    // Turn to look at mouse pointer raycast
    void LookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
        {
            transform.LookAt(new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z));
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
// Replace with call to UpdateHealth() ?
                health = healthSlider.maxValue;
                healthSlider.value = healthSlider.maxValue;
            }
// This is increasing bullet speed not power
            if (other.gameObject.GetComponent<Powerup>().powerupType == PowerupType.bulletPower)
            {
                bulletSpeed += 1;
            }
        }
    }

    IEnumerator ShootWeapon()
    {        
        while (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
        {
            isFiring = true;
// IMPLEMENT OBJECT POOLING!
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

    void PlayerKilled()
    {
        gameActive = false;
    }

    void StartGame()
    {
        gameActive = true;
    }

// Never used
    IEnumerator PowerupTimer()
    {
        hasPowerup = true;
        yield return new WaitForSeconds(powerupDuration);
        hasPowerup = false;
    }
}
