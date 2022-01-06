using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
// turn prefabs + quantity into a List for convenience and to have different pool quantities
    public int enemyPoolQuantity;
    public GameObject bossPrefab;
    public GameObject[] powerupPrefabs;
    [SerializeField] int powerupsPerRound = 1;
    public int powerupPoolQuantity;

    public static int activeEnemyNumber;

    [SerializeField] private GameManager gameManager;

    public float spawnBoundary = 12;
    public int bossFrequency = 5;
    private int waveNumber;
    private int randomIndex;
    private Vector3 randomLocation;
    private bool bossRound;

    private void Awake()
    {
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            PoolManager.instance.CreateNewPool(enemyPrefabs[i], enemyPoolQuantity);
            Debug.Log("Pool created for enemy prefab #" + enemyPrefabs[i].GetInstanceID());
        }

        for (int i = 0; i < powerupPrefabs.Length; i++)
        {
            PoolManager.instance.CreateNewPool(powerupPrefabs[i], powerupPoolQuantity);
            Debug.Log("Pool created for powerup prefab #" + powerupPrefabs[i].GetInstanceID());
        }

        PoolManager.instance.CreateNewPool(bossPrefab, 1);
        Debug.Log("Pool created for boss prefab #" + bossPrefab.GetInstanceID());
    }
    void Update()
    {
        if (gameManager.gameIsActive)
        {
            if (activeEnemyNumber <= 0)
            {
                SpawnWave();
            }

            if (!bossRound && waveNumber % bossFrequency == 0)
            {
                SpawnBoss();
            }
        } 
    }

    void SpawnWave()
    {
        Debug.Log("Spawning enemies");
        bossRound = false;

        waveNumber++;
        Debug.Log("Wave number: " + waveNumber);

        SpawnEnemies();
        SpawnPowerups();
    }

    void SpawnBoss()
    {
        Debug.Log("Spawning a Boss");
        bossRound = true;
        
        GameObject newBoss = PoolManager.instance.ReusePooledObject(bossPrefab, new Vector3(0, 10, 0), bossPrefab.transform.rotation);
    // BossSript.power is currently unused
        newBoss.GetComponent<BossScript>().power = waveNumber;
        newBoss.GetComponent<EnemyScript>().health *= waveNumber / 2;

        activeEnemyNumber += 1;
        Debug.Log("activeEnemyNumber: " + activeEnemyNumber);
    }

    void SpawnEnemies()
    {
        SpawnObjects(enemyPrefabs, waveNumber);
        activeEnemyNumber = waveNumber;
        Debug.Log("activeEnemyNumber: " + activeEnemyNumber);
    }

    void SpawnPowerups()
    {
        SpawnObjects(powerupPrefabs, powerupsPerRound);
    }

    void SpawnObjects(GameObject[] type, int numberOfEnemies)
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            randomIndex = Random.Range(0, type.Length);
            randomLocation = new Vector3(Random.Range(-spawnBoundary, spawnBoundary), 1, Random.Range(-spawnBoundary, spawnBoundary));
            
            PoolManager.instance.ReusePooledObject(type[randomIndex], randomLocation, Quaternion.identity);
        }     
    }
}
