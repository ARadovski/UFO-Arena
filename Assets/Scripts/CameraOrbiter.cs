using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbiter : MonoBehaviour
{
    [SerializeField] float closeupRotateSpeed = 10f;
    void Update()
    {
// Why does this not work when multiplied by Time.deltaTime or in FixedUpdate?
        transform.Rotate(Vector3.up * closeupRotateSpeed);
    }
}
