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

    private void OnCollisionEnter(Collision other)
    {
// Can I consolidate these 2 if player and enemy share an UpdateHealth method?
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().UpdateHealth(-damageEffect);
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyScript>().UpdateHealth(-damageEffect);
        }

        Disable();
    }

    protected void Disable()
    {
        gameObject.SetActive(false);
    }
}
