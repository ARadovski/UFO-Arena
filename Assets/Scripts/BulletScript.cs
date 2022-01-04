using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float damageEffect = 10;

    private void OnEnable()
    {
        Invoke("Destroy", 5);
    }
    void Start()
    {
// Replace this with pooling
        //Invoke("Destroy", 5);
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

        //Destroy(gameObject);
        gameObject.SetActive(false);
    }

    protected void Destroy()
    {
        gameObject.SetActive(false);
    }
}
