using UnityEngine;
using System.Collections;

public class BulletSetParent : MonoBehaviour {
	public float duration = 1.0f;
	public string shooterTag = "Player";
	public bool penetrate = false;
	public GameObject hitEffect;
	// Use this for initialization
	void Start (){
		hitEffect = GetComponent<BulletStatus>().hitEffect;
		//Set this object parent of the Shooter GameObject from BulletStatus
		this.transform.parent = GetComponent<BulletStatus>().shooter.transform;
		this.transform.position = new Vector3(transform.position.x , transform.position.y ,  GetComponent<BulletStatus>().shooter.transform.position.z);
		Destroy (gameObject, duration);
	}
	
	void OnTriggerEnter(Collider other){  
		if (other.gameObject.tag == "Wall") {
			if(hitEffect && !penetrate){
				Instantiate(hitEffect, transform.position , transform.rotation);
			}
			if(!penetrate){
				//Destroy this object if it not Penetrate
				Destroy (gameObject);
			}
		}
	}
	
}
