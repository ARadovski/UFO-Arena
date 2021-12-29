using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject boss;
    public GameObject[] powerupPrefabs;
    private GameObject[] enemies;

    [SerializeField] private GameManager gameManager;

    public float spawnBoundary = 12;
    public int bossFrequency = 5;
    private int waveNumber;
    private int randomIndex;
    private Vector3 randomLocation;
    private bool bossRound;

    void Update()
    {

// Find a better way to do this
// Abstract away from Update()
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (gameManager.gameIsActive && enemies.Length == 0)
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
            Instantiate(type[randomIndex], randomLocation, Quaternion.identity);
        }     
    }

    void SpawnBoss()
    {
        GameObject newBoss = Instantiate(boss, new Vector3(0, 10, 0), boss.transform.rotation);
        newBoss.GetComponent<BossScript>().power = waveNumber;
        newBoss.GetComponent<EnemyScript>().health *= waveNumber / 2;
    }
}
