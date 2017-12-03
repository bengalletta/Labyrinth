using UnityEngine;
using System.Collections;

public class SpawnPartner : MonoBehaviour {
	public GameObject[] mercenariesPrefab = new GameObject[2];
	public int spawnId = 0;
	[HideInInspector]
	public GameObject currentPartner;
	
	void Start(){
		Vector3 pos = transform.position;
		pos += Vector3.back * 3;
		if(mercenariesPrefab[spawnId]){
			GameObject m = Instantiate(mercenariesPrefab[spawnId] , pos , transform.rotation) as GameObject;
			m.GetComponent<AIfriend>().master = this.transform;
			currentPartner = m;
		}
	}
	
	public void MoveToMaster(){
		if(currentPartner){
			Physics.IgnoreCollision(GetComponent<Collider>(), currentPartner.GetComponent<Collider>());
			currentPartner.transform.position = transform.position;
		}
	}
	
}