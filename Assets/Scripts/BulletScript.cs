using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float damageEffect = 10;

    private void OnEnable()
    {
        Invoke("Disable", 6);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Projectile") || other.CompareTag("Powerup")){
            return;
        }
        // Can I consolidate these 2 if player and enemy share an UpdateHealth method?
        else if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<PlayerController>().UpdateHealth(-damageEffect);
        }
        else if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyScript>().UpdateHealth(-damageEffect);
        }
        Disable();
    }

    protected void Disable()
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }
}
