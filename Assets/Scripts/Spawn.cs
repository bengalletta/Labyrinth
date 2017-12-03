using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {

	[SerializeField] private GameObject enemyPrefab;
	private GameObject myEnemy;


	public List<GameObject> spawnList;

	// Use this for initialization
	void Start () 
	{
		spawnList = new List<GameObject>();
		InvokeRepeating("CheckAlive", 1, 1);
		//instantiate random number, which will be used for populate 
		int random = Random.Range(0,16);

		//Populate for the first time
		for (int i = 0; i < random; i++)
		{
			myEnemy = Instantiate(enemyPrefab) as GameObject;
			spawnList.Add(myEnemy);
		}
	}

	void CheckAlive()
	{
		for (int i = 0; i < spawnList.Count; i++)
		{
			if(!spawnList[i])
			{
				myEnemy = Instantiate(enemyPrefab) as GameObject;
				spawnList[i] = myEnemy;
			}
		}
	}
}