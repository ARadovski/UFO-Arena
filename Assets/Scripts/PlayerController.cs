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
    Camera playCamera;

    IEnumerator startFiring;

    public bool hasPowerup;
    private bool controlsActive;

    
    public float bulletSpeed = 1;
    private float firingRate = .125f;
    private bool isFiring;

    public float maxHealth = 100;
    [SerializeField] float health;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        PoolManager.instance.CreateNewPool(bulletPrefab, bulletPoolQuantity);

        playCamera = Camera.main;
    }
    void Start()
    {
        playerRb.centerOfMass = new Vector3(transform.position.x, 0, transform.position.z); 

        GameManager.OnStartGame += EnableControls;
    }

// Move some/all to FixedUpdate? Get rid of CheckWeaponFire via events?
    void Update()
    {
        if (controlsActive)
        {
            LookAtMouse();
            MovePlayer();
            CheckWeaponFire();
        }
    }

// Move this over to FixedUpdate?
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
        Ray ray = playCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
        {
            transform.LookAt(new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z));
        }
    }

    // Check for powerups
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Powerup")
        {
            PowerupManager.instance.ActivatePowerup(other.gameObject);  
            other.gameObject.SetActive(false);   
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
                if (startFiring != null){
                    StopCoroutine(startFiring);
                }
                isFiring = false;
            }
    }

    IEnumerator ShootWeapon()
    {        
        while (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
        {
            isFiring = true;

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
            gameManager.GameOver();
            DisableControls();
            Destroy(gameObject);
        }
    }

    public void UpdateHealth(float changeAmount)
    {
        health += changeAmount;
        healthSlider.value = health;
        if (health >= maxHealth)
        {
            health = maxHealth;
        }    
        CheckIfKilled();    
    }

    void EnableControls()
    {
        controlsActive = true;
    }

    void DisableControls()
    {
        controlsActive = false;
    }

    private void OnDestroy()
    {
        GameManager.OnGameOver -= DisableControls;
        GameManager.OnStartGame -= EnableControls;
    }
}
