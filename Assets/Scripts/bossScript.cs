using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Inherit from EnemysSript?
public class BossScript : MonoBehaviour
{
    public int power;
    private float health;
    [SerializeField] private GameObject minionPrefab;

    void Start()
    {
        health = GetComponent<EnemyScript>().health;
        StartCoroutine(LaunchMinions());
    }

    private IEnumerator LaunchMinions()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
// Implement object pooling?
            Instantiate(minionPrefab, transform.position, transform.rotation);
        }
    }
}
