using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {
	
	public string teleportToMap = "Level1";
	public string spawnPointName = "PlayerSpawn1"; //Use for Move Player to the SpawnPoint Position
	//Vector3 spawnPosition;
	
	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			other.GetComponent<Status>().spawnPointName = spawnPointName;
			ChangeMap();
		}
		
	}
	
	void  ChangeMap (){
		Application.LoadLevel (teleportToMap);
	}
}