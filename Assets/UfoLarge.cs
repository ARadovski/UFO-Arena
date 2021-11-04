using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class UfoLarge : EnemyScript
{
    [SerializeField] float jumpForce = 100;

    private bool isGrounded = true;

    protected override void Move()
    {
        base.Move();
        Hop();
    }


    private void Hop()
    {
        if (isGrounded)
        {
            enemyRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }
}
