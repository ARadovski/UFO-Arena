using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
// Rework into separate script without inheritance?
public class UfoLarge : EnemyScript
{
    [SerializeField] float jumpForce;

// POLYMORPHISM
    protected override void OnEnable()
    {
        base.OnEnable();
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

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
