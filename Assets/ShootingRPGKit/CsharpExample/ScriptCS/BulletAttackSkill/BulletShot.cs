using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BulletStatus))]
[RequireComponent (typeof(Rigidbody))]
[AddComponentMenu("Shooting-RPG Kit(CS)/Create Bullet")]
public class BulletShot : MonoBehaviour {
	public float speed = 20.0f;
	public Vector3 relativeDirection= Vector3.forward;
	public float duration = 1.0f;
	private GameObject hitEffect;
	private bool bomb;
	
	void Start (){
		GetComponent<Rigidbody>().isKinematic = true;
		hitEffect = GetComponent<BulletStatus>().hitEffect;
		bomb = GetComponent<BulletStatus>().bombHitSetting.enable;
		GetComponent<Collider>().isTrigger = true;
		Destroy();
	}
	
	
	void  Update (){
		transform.Translate(relativeDirection * speed * Time.deltaTime);
	}
	
	void  Destroy (){
		Destroy (gameObject, duration);
		
	}

	void  OnTriggerEnter ( Collider other  ){
		
		if (other.gameObject.tag == "Wall") {
			if(hitEffect){
				Instantiate(hitEffect, transform.position , transform.rotation);
			}
			if(bomb){
				GetComponent<BulletStatus>().ExplosionDamage();
			}
			Destroy (gameObject);
			
		}
	}
		
}
