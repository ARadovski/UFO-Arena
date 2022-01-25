using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    GameObject[] enemies;
    GameObject target;
    [SerializeField] GameObject smokeSpawn;
    GameObject smokeParticles;
    
    public float rocketSpeed = 1;
    public int rocketDamage = 20;
    public float blastRadius = 2;
    public float blastForce = 10;
    public float blastLift = 10;
    public LayerMask blastedLayer;
    float minDistance = 0;

    bool seekingTarget;
    bool rocketActive;

    void OnEnable()
    {
        StartCoroutine(FindTarget());
        //Invoke("ReturnToPool", 30);
        rocketActive = true;
        ConnectSmokeTrail();
    }

// How can I get rid of Update here?
    void Update()
    {
        if (rocketActive)
        {
            if (target.activeInHierarchy)
            {
                PursueTarget();
            }
            else if (!seekingTarget)
            {
                StartCoroutine(FindTarget());
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Projectile"))
        {
            return;
        }
        rocketActive = false;
        Explode();
    }

    void PursueTarget()
    {
        transform.LookAt(target.transform);
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * rocketSpeed);
    }

    IEnumerator FindTarget()
    {
        seekingTarget = true;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0)
        {            
            minDistance = Vector3.Distance(enemies[0].transform.position, transform.position);
            target = enemies[0];
            for (int i = 0; i < enemies.Length; i++)
            {
                float distance = Vector3.Distance(enemies[i].transform.position, transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    target = enemies[i];
                }
            }
        }
        yield return new WaitForSeconds(.1f);
        seekingTarget = false;
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius, blastedLayer);
        foreach (Collider col in colliders)
        {
            Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
            rb.AddExplosionForce(blastForce, transform.position, blastRadius, blastLift, ForceMode.VelocityChange);
            col.gameObject.GetComponent<EnemyScript>().UpdateHealth(-rocketDamage);
        } 
        PoolManager.instance.ReusePooledObject(PoolManager.instance.particlePool["Particle_ExplosionRocket"], transform.position, transform.rotation);

        DisconnectSmokeTrail();
        ReturnToPool();
    }

    void ConnectSmokeTrail()
    {
// Is there a more streamlined way to reuse these particle systems?:
        if (!GetComponentInChildren<ParticleSystem>())
        {
            smokeParticles = PoolManager.instance.ReusePooledObject(PoolManager.instance.particlePool["Particle_SmokeTrail"], smokeSpawn.transform.position, transform.rotation);
            smokeParticles.transform.SetParent(transform);
        }
        var smokeTrail = smokeParticles.GetComponentInChildren<ParticleSystem>().main;
        smokeTrail.loop = true;
    }

    void DisconnectSmokeTrail()
    {
// Eliminate redundant repeating line already done in OnAwake
        var smokeTrail = smokeParticles.GetComponentInChildren<ParticleSystem>().main;
        smokeTrail.loop = false;
        smokeParticles.transform.SetParent(null);
    }

// Consistent naming/handling of disable&returnToPool ?
    void ReturnToPool()
    {
        gameObject.SetActive(false);
    }
}
