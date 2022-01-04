using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float damageEffect = 10;

    private void OnEnable()
    {
        Invoke("Disable", 5);
    }

    private void OnTriggerEnter(Collider other)
    {
// Can I consolidate these 2 if player and enemy share an UpdateHealth method?
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().UpdateHealth(-damageEffect);
        }

        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyScript>().UpdateHealth(-damageEffect);
        }

        Disable();
    }

    protected void Disable()
    {
        gameObject.SetActive(false);
    }
}
