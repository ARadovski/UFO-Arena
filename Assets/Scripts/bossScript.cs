using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossScript : MonoBehaviour
{
    public int power;
    private float health;

    [SerializeField] private GameObject minionPrefab;
    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<EnemyScript>().health;
        StartCoroutine(LaunchMinions());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LaunchMinions()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            Instantiate(minionPrefab, transform.position, transform.rotation);
        }
    }
}
