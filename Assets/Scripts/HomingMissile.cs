using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    GameObject[] enemies;
    GameObject target;
    public float rocketSpeed = 1;
    public int rocketDamage = 20;
    public float blastRadius = 2;
    public float blastForce = 10;
    public float blastLift = 10;
    public LayerMask blastedLayer;
    float minDistance = 0;

    bool seekingTarget;
    void OnEnable()
    {
        StartCoroutine(FindTarget());
        Invoke("ReturnToPool", 30);
    }

    void Update()
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

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            Explode();
        }
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
            Debug.Log("Targeting: " + target);
        }
        yield return new WaitForSeconds(.2f);
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
        ReturnToPool();
    }

// Consistent naming/handling of disable&returnToPool ?
    void ReturnToPool()
    {
        gameObject.SetActive(false);
    }
}
