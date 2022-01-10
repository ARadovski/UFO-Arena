using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	public Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();

// Is there a better way to organize references to pooled objects etc.?
	public Dictionary<string, GameObject> particlePool = new Dictionary<string, GameObject>();
	public GameObject[] particlePrefabs;
	public int particlePoolCapacity;

	static PoolManager _instance;
	public static PoolManager instance 
	{
		get {
			if (_instance == null)
			{
				_instance = FindObjectOfType<PoolManager>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
// Populate library with items from the particlePrefab list, replace parented particles in Rocket prefab with ReusePooledObject at explosion time
		foreach (KeyValuePair<string, GameObject> particle in particlePool)
		{
			CreateNewPool(particle.Value, particlePoolCapacity);
		}
	}
	public void CreateNewPool(GameObject prefab, int poolCapacity)
	{
		int poolKey = prefab.GetInstanceID();
		if (!poolDictionary.ContainsKey(poolKey))
		{
			Debug.Log("Creating pool for prefab #" + prefab.GetInstanceID() + " " + prefab);
			poolDictionary.Add(poolKey, new Queue<GameObject>());

			// Creating new pool holder object and parenting it to PoolManager
			GameObject newPool = new GameObject(prefab + " pool");
			newPool.transform.SetParent(transform);

			for (int i = 0; i < poolCapacity; i++)
			{
				GameObject newPooledObject = Instantiate(prefab) as GameObject;
				newPooledObject.SetActive(false);
				// parenting to a holder GameObject
				newPooledObject.transform.SetParent(newPool.transform);

				poolDictionary[poolKey].Enqueue(newPooledObject);	

			}
		}	
	}

	public GameObject ReusePooledObject(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		int poolKey = prefab.GetInstanceID();
		GameObject pooledObject = poolDictionary[poolKey].Dequeue();
		pooledObject.transform.position = position;
		pooledObject.transform.rotation = rotation;
		pooledObject.SetActive(true);

		if(prefab.gameObject.name != "Bullet"){
			Debug.Log("Enabled 1 " + prefab);
		}	

// Investigate why EnemyScript gets disabled on smallEnemy, fix and eliminate this conditional:
		if (pooledObject.GetComponent<EnemyScript>() != null){
			pooledObject.GetComponent<EnemyScript>().enabled = true;
		}

		poolDictionary[poolKey].Enqueue(pooledObject);
		return pooledObject;
	}

}
