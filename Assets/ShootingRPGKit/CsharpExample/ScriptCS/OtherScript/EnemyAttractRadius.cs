using UnityEngine;
using System.Collections;

public class EnemyAttractRadius : MonoBehaviour {
	
	public float radius = 30.0f;
	
	void  Start (){
		Collider[] hitColliders= Physics.OverlapSphere(transform.position, radius);
		
		for (int i= 0; i < hitColliders.Length; i++) {
			if(hitColliders[i].tag == "Enemy"){	  
				hitColliders[i].SendMessage("SetDestination" , transform.position);
			}
		}
	}
	
	
}
