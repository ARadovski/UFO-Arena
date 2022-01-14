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
    
    public static int activeEnemyCount;

    [SerializeField] private GameManager gameManager;

    public float spawnBoundary = 12;
    public int bossFrequency = 5;
    private int waveNumber;
    private int randomIndex;
    private Vector3 randomLocation;
    private bool bossRound;

    private void Awake()
    {
        GameManager.OnStartGame += ResetWaves;
        GameManager.OnStartGame += SpawnWave;
        GameManager.OnGameOver += ResetWaves;

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            PoolManager.instance.CreateNewPool(enemyPrefabs[i], enemyPoolQuantity);
        }

        for (int i = 0; i < powerupPrefabs.Length; i++)
        {
            PoolManager.instance.CreateNewPool(powerupPrefabs[i], powerupPoolQuantity);
        }

        PoolManager.instance.CreateNewPool(bossPrefab, 1);
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
        newBoss.GetComponent<Boss>().power = waveNumber;
        newBoss.GetComponent<EnemyScript>().health *= waveNumber / 2;

        CountEnemies();
    }

    void SpawnEnemies()
    {
        SpawnObjects(enemyPrefabs, waveNumber);
// Temp workaround to counter bug that throws off activeEnemyCount in some (unexplained) cases 
        CountEnemies();
// Could this work with a singleton?:
        //activeEnemyCount += waveNumber;
        
    }

// Workaround instead of unreliable manual count?
    public void CountEnemies()
    {
        activeEnemyCount = FindObjectsOfType<EnemyScript>().Length;
        Debug.Log("activeEnemyNumber: " + activeEnemyCount);

        CheckIfSpawn();
// Could this work with a singleton?
        //activeEnemyCount += changeAmount;
    }

    void CheckIfSpawn()
    {
        if (gameManager.gameIsActive)
        {
            if (activeEnemyCount <= 0)
            {
                SpawnWave();
            }

            if (!bossRound && waveNumber % bossFrequency == 0)
            {
                SpawnBoss();
            }
        } 
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

    void ResetWaves()
    {
        waveNumber = 0;
        activeEnemyCount = 0;
    }

    private void OnDestroy()
    {
        GameManager.OnStartGame -= ResetWaves;
        GameManager.OnStartGame -= SpawnWave;
        GameManager.OnGameOver -= ResetWaves;
    }
}
