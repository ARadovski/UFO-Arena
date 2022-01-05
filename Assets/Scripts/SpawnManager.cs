using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
// turn prefbs/ quantity into a List for convenience and to have different pool quantities
    public int prefabPoolQuantity;
    public GameObject boss;
    public GameObject[] powerupPrefabs;
    private GameObject[] activeEnemies;

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
            PoolManager.instance.CreateNewPool(enemyPrefabs[i], prefabPoolQuantity);
            Debug.Log(enemyPrefabs[i].GetInstanceID());
        }
    }
    void Update()
    {
// Find a better way to do this - add up numeberOfEnemies, subtract from it on enemy death
// Abstract away from Update()
        activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (gameManager.gameIsActive && activeEnemies.Length == 0)
        {
            bossRound = false;
            waveNumber++;
            SpawnObjects(enemyPrefabs, waveNumber);
            SpawnObjects(powerupPrefabs, 1);
        }

        if (!bossRound && waveNumber % bossFrequency == 0)
        {
            bossRound = true;
            SpawnBoss();
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
