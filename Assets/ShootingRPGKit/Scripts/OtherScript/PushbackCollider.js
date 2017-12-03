#pragma strict
var backSpeed : float = 4.0;
var pushTag : String = "Player";

function OnTriggerStay (other : Collider) {
	if(other.gameObject.tag == pushTag){
		other.GetComponent(CharacterController).Move(transform.rotation * Vector3.back * backSpeed *Time.deltaTime);
	}

}