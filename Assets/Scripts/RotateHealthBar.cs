using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHealthBar : MonoBehaviour
{
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        Debug.Log(mainCamera.gameObject.name);
    }

    void Update()
    {
// Fix error?
        transform.forward = mainCamera.transform.forward;
    }
}
