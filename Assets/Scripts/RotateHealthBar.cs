using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHealthBar : MonoBehaviour
{
    private Camera mainCamera;

    void Awake()
    {
// Fix error? NullReferenceException: Object reference not set to an instance of an object
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {
        transform.forward = mainCamera.transform.forward;
    }
}
