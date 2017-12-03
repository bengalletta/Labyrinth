using UnityEngine;
using System.Collections;

public class DamageReceived : MonoBehaviour {

	public float Health = 1.0f;
	public Transform deathSpawn = null;

	// Use this for initialization
	private void Damage (float damage) {
		Health -= damage;
		if(Health<=0){
			DestroyMe();
		}
	}
	private void DestroyMe(){
		if (deathSpawn == null){
			Destroy (this.gameObject);
		}else {
			Instantiate(deathSpawn, this.transform.position, this.transform.rotation);
			Destroy(this.gameObject);
		}

	}


}
