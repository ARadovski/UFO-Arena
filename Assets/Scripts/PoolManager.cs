using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	public Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();

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

	public void CreateNewPool(GameObject prefab, int poolCapacity)
	{
		int poolKey = prefab.GetInstanceID();
		if (!poolDictionary.ContainsKey(poolKey))
		{
			poolDictionary.Add(poolKey, new Queue<GameObject>());
			for (int i = 0; i < poolCapacity; i++)
			{
				GameObject newPooledObject = Instantiate(prefab) as GameObject;
				newPooledObject.SetActive(false);

				poolDictionary[poolKey].Enqueue(newPooledObject);	
// parenting to a holder GameObject
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
// Investigate why EnemyScript gets disabled on smallEnemy, fix and eliminate this conditional:
		if (pooledObject.GetComponent<EnemyScript>() != null){
			pooledObject.GetComponent<EnemyScript>().enabled = true;
		}

		poolDictionary[poolKey].Enqueue(pooledObject);

		return pooledObject;
	}

}
