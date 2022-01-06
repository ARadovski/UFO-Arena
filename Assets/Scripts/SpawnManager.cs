using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
// turn prefbs/ quantity into a List for convenience and to have different pool quantities
    public int enemyPoolQuantity;
    public GameObject boss;
    public GameObject[] powerupPrefabs;
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
    }
    void Update()
    {
// Find a better way to do this - add up numeberOfEnemies, subtract from it on enemy death
// Abstract away from Update()
        if (gameManager.gameIsActive)
        {
            CheckIfSpawn();
        } 
    }

    void CheckIfSpawn()
    {
        if (activeEnemyNumber <= 0)
        {
            Debug.Log("Spawning enemies");
            bossRound = false;
            waveNumber++;
            Debug.Log("Wave number: " + waveNumber);
            SpawnObjects(enemyPrefabs, waveNumber);
            SpawnObjects(powerupPrefabs, 1);
            activeEnemyNumber = waveNumber;
            Debug.Log("activeEnemyNumber: " + activeEnemyNumber);
        }

        if (!bossRound && waveNumber % bossFrequency == 0)
        {
            Debug.Log("Spawning a Boss");
            bossRound = true;
            SpawnBoss();
            activeEnemyNumber += 1;
            Debug.Log("activeEnemyNumber: " + activeEnemyNumber);
        }
    }

    void SpawnObjects(GameObject[] type, int numberOfEnemies)
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            randomIndex = Random.Range(0, type.Length);
            randomLocation = new Vector3(Random.Range(-spawnBoundary, spawnBoundary), 1, Random.Range(-spawnBoundary, spawnBoundary));
            
            PoolManager.instance.ReusePooledObject(type[randomIndex], randomLocation, Quaternion.identity);
            //Instantiate(type[randomIndex], randomLocation, Quaternion.identity);
        }     
    }

    void SpawnBoss()
    {
        GameObject newBoss = Instantiate(boss, new Vector3(0, 10, 0), boss.transform.rotation);
        newBoss.GetComponent<BossScript>().power = waveNumber;
        newBoss.GetComponent<EnemyScript>().health *= waveNumber / 2;
    }
}
