using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbiter : MonoBehaviour
{
    [SerializeField] float closeupRotateSpeed = .1f;
    void Update()
    {
        transform.Rotate(Vector3.up * closeupRotateSpeed);
    }
}
