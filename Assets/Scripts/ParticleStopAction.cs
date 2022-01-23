using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleStopAction : MonoBehaviour
{
    GameObject parentObject;
    [SerializeField] bool trail;
    GameObject poolManager;
    private void Awake()
    {
        poolManager = GameObject.FindObjectOfType<GameManager>().gameObject;

        parentObject = transform.parent.gameObject;
        var main = GetComponent<ParticleSystem>().main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    void OnParticleSystemStopped()
    {
        parentObject.SetActive(false);
        if (trail)
        {
            transform.parent.SetParent(poolManager.transform);
        }
    }
}
