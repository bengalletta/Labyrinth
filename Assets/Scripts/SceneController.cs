using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour {


	[SerializeField] private GameObject enemyPrefab;
	private GameObject _enemy;
	public int enemyCount;
	public int maxEnemyCount = 15;
	public float timeBetweenSpawns = 2.0f;
	private Vector3 spawnPosition1;
	private Vector3 spawnPosition2;
	public GameObject[] enemies;

	void Start(){
		InvokeRepeating("Spawn", 0, timeBetweenSpawns);
	}



	void Spawn(){

		spawnPosition1 = new Vector3 (267, 2, 181);
		spawnPosition2 = new Vector3 (234, 2, 161);
		float number = Random.Range(1.0f, 4.0f);

			_enemy = Instantiate (enemyPrefab) as GameObject;
		_enemy.transform.position = (spawnPosition1);
			float angle = Random.Range (0, 360);
			_enemy.transform.Rotate (0, angle, 0);


		enemyCount++;
		if(enemyCount >= maxEnemyCount ){
			CancelInvoke("Spawn");
		}
	}


	
//	void Update() {
//
//
//		if (_enemy == null || enemyCount < 15) {
//			_enemy = Instantiate(enemyPrefab) as GameObject;
//			_enemy.transform.position = new Vector3(267, 2, 181);
//			float angle = Random.Range(0, 360);
//			_enemy.transform.Rotate(0, angle, 0);
//		}
//
////		if (enemyCount < 15) {
////			_enemy = Instantiate(enemyPrefab) as GameObject;
////			_enemy.transform.position = new Vector3(267, 2, 181);
////			float angle = Random.Range(0, 360);
////			_enemy.transform.Rotate(0, angle, 0);
////		}
//	}
}
