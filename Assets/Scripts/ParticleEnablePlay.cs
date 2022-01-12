using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEnablePlay : MonoBehaviour
{
    ParticleSystem particles;
    private void Awake()
    {
        particles = GetComponentInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        particles.Play();
    }
}
