using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Inherit from EnemySript?
public class BossScript : MonoBehaviour
{
    public int power;
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] int minionPoolQuantity;

    private void Awake()
    {
        PoolManager.instance.CreateNewPool(minionPrefab, minionPoolQuantity);
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
            SpawnManager.activeEnemyCount += 1;
        }
    }
}
