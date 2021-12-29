using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset;

    void Update()
    {
// Implement smoothing?
        transform.position = player.transform.position + offset;
    }
}
