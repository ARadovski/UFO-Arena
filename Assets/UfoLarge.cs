using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class UfoLarge : EnemyScript
{
    [SerializeField] float jumpForce;

// POLYMORPHISM
    protected override void Start()
    {
        base.Start();
        StartCoroutine(Hop());
    }

    private IEnumerator Hop()
    {  
        while(true)
        {
            yield return new WaitForSeconds(3);     
            enemyRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        
    }
}
