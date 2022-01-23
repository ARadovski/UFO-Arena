using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHealthBar : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
// Fix error? NullReferenceException: Object reference not set to an instance of an object
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        if (mainCamera != null){
            transform.forward = mainCamera.transform.forward;
        }
        else {
            Debug.Log("Main camera is null!");
        }
            
    }
}
