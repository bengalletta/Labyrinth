using UnityEngine;
using System.Collections;

public class AddAmmo : MonoBehaviour {
	public AllAmmo addAmmo;
	public float duration = 30.0f;
	private Transform master;
	
	void Start (){
		master = transform.root;
		if(duration > 0){
			Destroy (master.gameObject, duration);
		}
	}
	
	void OnTriggerEnter(Collider other){
		//Pick up Item
		if (other.gameObject.tag == "Player") {
			AddAmmoToPlayer(other.gameObject);
		}
	}
	
	void OnCollisionEnter(Collision other){
		//Pick up Item
		if (other.gameObject.tag == "Player") {
			AddAmmoToPlayer(other.gameObject);
		}
	}
	
	void AddAmmoToPlayer(GameObject other){
		other.GetComponent<GunTrigger>().allAmmo.handgunAmmo += addAmmo.handgunAmmo;
		other.GetComponent<GunTrigger>().allAmmo.machinegunAmmo += addAmmo.machinegunAmmo;
		other.GetComponent<GunTrigger>().allAmmo.shotgunAmmo += addAmmo.shotgunAmmo;
		other.GetComponent<GunTrigger>().allAmmo.magnumAmmo += addAmmo.magnumAmmo;
		other.GetComponent<GunTrigger>().allAmmo.smgAmmo += addAmmo.smgAmmo;
		other.GetComponent<GunTrigger>().allAmmo.sniperRifleAmmo += addAmmo.sniperRifleAmmo;
		other.GetComponent<GunTrigger>().allAmmo.grenadeRounds += addAmmo.grenadeRounds;
		
		master = transform.root;
		Destroy(master.gameObject);
	}
	
	
	
}