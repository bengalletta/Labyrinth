using UnityEngine;
using System.Collections;

public class PushbackCollider : MonoBehaviour {
	public float backSpeed = 4.0f;
	public string pushTag = "Player";

	void OnTriggerStay(Collider other){
		if(other.gameObject.tag == pushTag){
			other.GetComponent<CharacterController>().Move(transform.rotation * Vector3.back * backSpeed *Time.deltaTime);
		}
	}

}
