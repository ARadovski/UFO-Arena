using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed;
    private Rigidbody playerRb;

    private bool hasPowerup;

    [SerializeField] private LayerMask layerMask;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerRb.centerOfMass = new Vector3(0, 0, 0);
    }

    void Update()
    {
        MovePlayer();
        LookAtMouse();
    }

    // Control player movement with forces to rigidbody
    void MovePlayer()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        playerRb.AddForce(Vector3.forward * verticalInput * playerSpeed);
        playerRb.AddForce(Vector3.right * horizontalInput * playerSpeed);
    }

    // Turn to look at mouse pointer raycast
    void LookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
        {
            transform.LookAt(new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z));
            /*transform.forward = raycastHit.point;*/
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Powerup")
        {
            Destroy(other.gameObject);
            hasPowerup = true;
        }
    }
}
