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
    [SerializeField] private LayerMask layerMaskGround;
    [SerializeField] private GameObject bulletPrefab;
    GameObject laserFlash;
    [SerializeField] int bulletPoolQuantity = 10;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject laserMuzzleLight;
    [SerializeField] GameObject laserHitLight;
    [SerializeField] float lazerMaxDistance = 20;
    [SerializeField] float lazerPower = 1;
    [SerializeField] float lazerParticleTimer = .2f;
    [SerializeField] float particleCountdown = 0;
    [SerializeField] LayerMask layerMaskLazer;
    [SerializeField] private GameManager gameManager;
    public GameObject bulletSpawn;
    Camera playCamera;
    IEnumerator fireWeapon;
    public bool hasPowerup;
    private bool controlsActive;
    public float bulletSpeed = 1;
    public float bulletFireRate = 8;
    private bool isFiring;
    public bool lazerOn;
    public float maxHealth = 100;
    [SerializeField] float health;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        lineRenderer = GetComponentInChildren<LineRenderer>();

        laserMuzzleLight = GetComponentInChildren<PulsateLight>().gameObject;
        laserMuzzleLight.SetActive(false);
        laserHitLight = GetComponentInChildren<LaserHitLight>().gameObject;
        laserHitLight.SetActive(false);

        gameManager = FindObjectOfType<GameManager>();
        playCamera = Camera.main;

        if (lineRenderer != null){
            Debug.Log("Found line renderer: " + lineRenderer.gameObject.name);
        }

        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        PoolManager.instance.CreateNewPool(bulletPrefab, bulletPoolQuantity);

        if (bulletFireRate <= 0){
            bulletFireRate = 8;
        }
    }
    void Start()
    {
        playerRb.centerOfMass = new Vector3(transform.position.x, 0, transform.position.z); 

        GameManager.OnStartGame += EnableControls;
    }

// Move some/all to FixedUpdate? Get rid of CheckWeaponFire via events?
    private void Update()
    {
        if (controlsActive)
        {
            // This should stay in Update to prevent missed button clicks? Or do with events!
            CheckWeaponFire();
        }
    }
    void FixedUpdate()
    {
        if (controlsActive)
        {
// Redo with events instead of Update loop?
            LookAtMouse();
            MovePlayer();
        }
    }

// Move this over to FixedUpdate?
    // Control player movement with forces to rigidbody
    void MovePlayer()
    {
        Vector3 playerInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        playerRb.AddForce(playerInput * playerSpeed * Time.deltaTime);
    }

    // Turn to look at mouse pointer raycast
    void LookAtMouse()
    {
        Ray ray = playCamera.ScreenPointToRay(Input.mousePosition);
// Workaround with invisible LookAt ground object, better way?
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMaskGround))
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
                fireWeapon = FireWeapon();
                StartCoroutine(fireWeapon);
            }

            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
            {
                if (fireWeapon != null){
                    StopCoroutine(fireWeapon);
                }
                if (lazerOn)
                {
                    TurnLaserOff();
                }
                isFiring = false;
            }
    }

    IEnumerator FireWeapon()
    {        
        if (lazerOn)
        {
            laserFlash = PoolManager.instance.ReusePooledObject(PoolManager.instance.particlePool["Particle_LazerFlash"], bulletSpawn.transform.position, Quaternion.Euler(transform.forward));
            laserFlash.transform.SetParent(bulletSpawn.transform);
            laserMuzzleLight.SetActive(true);
        }
        while (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
        {
            isFiring = true;
            if (lazerOn)
            {
                ShootLazer();
                yield return null;
            }
            else 
            {
// Handle this via event without conditional in Update?                
                TurnLaserOff();
                ShootBullet();
                yield return new WaitForSeconds(1/bulletFireRate);
            }
        }
    }

    void ShootLazer()
    {
        lineRenderer.positionCount = 2;

        if (Physics.Raycast(bulletSpawn.transform.position, transform.forward, out RaycastHit hit, lazerMaxDistance))
        {
            lineRenderer.SetPosition(0, bulletSpawn.transform.position);
            lineRenderer.SetPosition(1, hit.point);

            laserHitLight.SetActive(true);
            laserHitLight.transform.position = hit.point;

            particleCountdown -= Time.deltaTime;
            if (particleCountdown <= 0)
            {
                PoolManager.instance.ReusePooledObject(PoolManager.instance.particlePool["Particle_LazerHit"], hit.point, Quaternion.Euler(-transform.forward));
                particleCountdown = lazerParticleTimer;
            }

            if (hit.transform.gameObject.TryGetComponent(out EnemyScript enemyScript))
            {
                enemyScript.UpdateHealth(-lazerPower);
            }
        }
        else 
        {
            lineRenderer.SetPosition(0, bulletSpawn.transform.position);
            lineRenderer.SetPosition(1, bulletSpawn.transform.position + transform.forward * lazerMaxDistance);

            laserHitLight.SetActive(false);
        }
    }

    public void TurnLaserOff()
    {
        lineRenderer.positionCount = 0;
        if(laserFlash != null)
        {
            laserFlash.SetActive(false);
        }
        laserMuzzleLight.SetActive(false);
        laserHitLight.SetActive(false);
    }

    void ShootBullet()
    {
        GameObject bullet = PoolManager.instance.ReusePooledObject(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.velocity = bullet.transform.forward * bulletSpeed;
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
