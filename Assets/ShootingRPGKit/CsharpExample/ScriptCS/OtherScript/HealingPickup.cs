using UnityEngine;
using System.Collections;

public class HealingPickup : MonoBehaviour {
	public int hpRecover = 20;
	public int mpRecover = 0;
	private Transform master;
	
	void  OnTriggerEnter (Collider col){
		if(col.tag == "Player"){
			col.GetComponent<Status>().Heal(hpRecover , mpRecover);
			master = transform.root;
			Destroy(master.gameObject);
		}
	}
}