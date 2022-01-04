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
    [SerializeField] int bulletPoolQuantity = 10;
    [SerializeField] private GameManager gameManager;
    public GameObject bulletSpawn;

    IEnumerator startFiring;

    private bool hasPowerup;
    private bool gameActive;
    public float powerupDuration = 7;

    
    public float bulletSpeed = 1;
    private float firingRate = .125f;
    private bool isFiring;

    public float maxHealth = 10;
    private float health;

    public static event System.Action OnPlayerKilled;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        GameManager.OnGameOver += EndGame;
    }
    void Start()
    {
        playerRb.centerOfMass = new Vector3(transform.position.x, 0, transform.position.z); 
        StartGame();

        PoolManager.instance.CreateNewPool(bulletPrefab, bulletPoolQuantity);
    }

    void Update()
    {
        if (gameActive)
        {
            LookAtMouse();
            MovePlayer();
            CheckWeaponFire();
        }

        CheckIfKilled();
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
// Add contitional?
            //StartCoroutine(PowerupTimer());

// Implement this in Powerup script?
            PowerupType type = other.gameObject.GetComponent<Powerup>().powerupType;
            switch(type)
            {
                case PowerupType.health:
                    UpdateHealth(maxHealth);
                    break;
                case PowerupType.bulletSpeed:
                    bulletSpeed += 1;
                    break;
                default:
                    break;
            }
        }
    }

    void CheckWeaponFire()
    {
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

    IEnumerator ShootWeapon()
    {        
        while (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
        {
            isFiring = true;
// IMPLEMENT OBJECT POOLING!
            //GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);

            GameObject bullet = PoolManager.instance.ReusePooledObject(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.velocity = bullet.transform.forward * bulletSpeed;
            yield return new WaitForSeconds(firingRate);
        }
    }

    void CheckIfKilled()
    {
        if (health <= 0)
        {
            if(OnPlayerKilled !=null)
            {
                OnPlayerKilled();
            }
            gameActive = false;
            Destroy(gameObject);
        }
    }

    public void UpdateHealth(float changeAmount)
    {
        health += changeAmount;
        healthSlider.value += changeAmount;
        if (health >= maxHealth)
        {
            health = maxHealth;
        }        
    }

    void StartGame()
    {
        gameActive = true;
    }

    void EndGame()
    {
        gameActive = false;
    }

// Never used - implement timed powerups!
    IEnumerator PowerupTimer()
    {
        hasPowerup = true;
        yield return new WaitForSeconds(powerupDuration);
        hasPowerup = false;
    }

    private void OnDestroy()
    {
        GameManager.OnGameOver -= EndGame;
    }
}
