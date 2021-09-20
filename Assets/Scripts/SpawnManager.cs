using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject boss;
    public GameObject[] powerupPrefabs;
    private GameObject[] enemies;

    public float spawnBoundary = 15;
    public int bossFrequency = 5;
    private int waveNumber;
    private int randomIndex;
    private Vector3 randomLocation;

    private bool bossRound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            bossRound = false;
            waveNumber++;
            SpawnObjects(enemyPrefabs, waveNumber);
            SpawnObjects(powerupPrefabs, 1);
        }

        if (!bossRound && waveNumber % bossFrequency == 0)
        {
            bossRound = true;
            SpawnBoss(waveNumber);
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

    void SpawnBoss(int strength)
    {
        GameObject newBoss = Instantiate(boss, Vector3.zero, boss.transform.rotation);
        newBoss.GetComponent<bossScript>().power = strength;
    }
}
