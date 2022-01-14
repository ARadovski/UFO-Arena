using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Inherit from EnemySript?
public class Boss : MonoBehaviour
{
    public int power;
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] int minionPoolQuantity;

    SpawnManager spawnManager;

    private void Awake()
    {
        PoolManager.instance.CreateNewPool(minionPrefab, minionPoolQuantity);
        spawnManager = FindObjectOfType<SpawnManager>();
    }

    private void OnEnable()
    {
        StartCoroutine(LaunchMinions());
    }

    private IEnumerator LaunchMinions()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);

            PoolManager.instance.ReusePooledObject(minionPrefab, transform.position, transform.rotation);
            spawnManager.CountEnemies();
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
