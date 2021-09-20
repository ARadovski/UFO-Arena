using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private Rigidbody enemyRb;
    private GameObject player;

    public float forwardSpeed = 1;
    public float lateralSpeed;
    public float forceMultiplier = 2;
    
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        // Destroy if falling below arena plane
        if (transform.position.y < -1)
        {
            Destroy(gameObject);
        }

        // Looking at the player
        transform.LookAt(player.transform.position);

        // Moving toward the player
        //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, forwardSpeed * Time.deltaTime);
        enemyRb.AddForce((player.transform.position - transform.position).normalized * forwardSpeed * enemyRb.mass * forceMultiplier);


        // Moving laterally in relation to the player
        enemyRb.AddForce(transform.right * lateralSpeed * enemyRb.mass * forceMultiplier);
    }

    // Self-destruct on triggering invisible boundary
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "InvisibleBoundary")
        {
            Destroy(gameObject);
        }
    }
}
