using UnityEngine;
using System.Collections;

public class AddCash : MonoBehaviour {
	public int cashMin = 10;
	public int cashMax = 50;
	public float duration = 30.0f;
	private Transform master;

	void  Start (){
		master = transform.root;
		if(duration > 0){
			Destroy(master.gameObject, duration);
		}
	}
	
	void OnTriggerEnter(Collider other){
		//Pick up Item
		if (other.gameObject.tag == "Player") {
			AddCashToPlayer(other.gameObject);
		}
	}
	
	void AddCashToPlayer (GameObject other){
		int gotCash= Random.Range(cashMin , cashMax);
		other.GetComponent<Inventory>().cash += gotCash;
		master = transform.root;
		Destroy(master.gameObject);
	}
	
	
}