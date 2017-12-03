using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class DetectCollider : MonoBehaviour {
	public Transform master;
	private AIenemy ai;
	
	void Start (){
		if(!master){
			master = transform.root;
		}
		ai = master.GetComponent<AIenemy>();
		gameObject.layer = 2;
		GetComponent<Rigidbody>().isKinematic = true;
		GetComponent<Collider>().isTrigger = true;
	}
	
	void OnTriggerEnter (Collider other){
		if(ai.followState == AIState.Moving || ai.followState == AIState.Pausing){
			return;
		}
		if(other.gameObject.tag == "Player" || other.gameObject.tag == "Ally"){
			ai.followTarget = other.transform;
			ai.followState = AIState.Moving;
		}
	}
}
