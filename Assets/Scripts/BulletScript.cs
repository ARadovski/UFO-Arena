using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float damageEffect = 10;
    GameObject hitSparks;

    private void OnEnable()
    {
        Invoke("Disable", 6);
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if(other.CompareTag("Projectile") || other.CompareTag("Powerup")){
    //         return;
    //     }
    //     // Can I consolidate these 2 if player and enemy share an UpdateHealth method?
    //     else if (other.CompareTag("Player"))
    //     {
    //         other.GetComponentInParent<PlayerController>().UpdateHealth(-damageEffect);
    //     }
    //     else if (other.CompareTag("Enemy"))
    //     {
    //         other.GetComponent<EnemyScript>().UpdateHealth(-damageEffect);
    //     }
        
    //     // Trying to use raycast from behind the bullet in its forward direction to find a collision point for hit particles
    //     if (Physics.Raycast(transform.position - transform.forward * 10, transform.forward, out RaycastHit hit)){
    //         PoolManager.instance.ReusePooledObject(PoolManager.instance.particlePool["Particle_BulletHit"], hit.point, transform.rotation);
    //     }

    //     Disable();
    // }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Projectile") || other.gameObject.CompareTag("Powerup")){
            return;
        }
        // Can I consolidate these 2 if player and enemy share an UpdateHealth method?
        else if (other.gameObject.CompareTag("MyPlayerTag"))
        {
            other.gameObject.GetComponentInParent<PlayerController>().UpdateHealth(-damageEffect);
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyScript>().UpdateHealth(-damageEffect);
        }
        
        hitSparks = PoolManager.instance.ReusePooledObject(PoolManager.instance.particlePool["Particle_BulletHit"], other.GetContact(0).point, transform.rotation);
        hitSparks.transform.SetParent(other.gameObject.transform);
// For a cool glitchy effect do not unparent hit particles from the above upon them stopping

        Disable();
    }

    protected void Disable()
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }
}
