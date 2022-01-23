using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsateLight : MonoBehaviour
{
    Light lightToPulse;
    float intensity;
    [SerializeField] float inverseOfRange;
    [SerializeField] float pulseRate = 1;
    private void Awake()
    {
        lightToPulse = GetComponent<Light>();
    }
    private void OnEnable()
    {
        StartCoroutine(Pulsate());
    }

    IEnumerator Pulsate()
    {
        while(true)
        {
            intensity = 1 + Mathf.Sin(Time.time * pulseRate)/inverseOfRange;
            lightToPulse.intensity = intensity;
            Debug.Log("intensity: " + intensity);

            yield return null;
        } 
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
