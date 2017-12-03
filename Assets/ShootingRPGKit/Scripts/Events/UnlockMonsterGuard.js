#pragma strict
//Use with Trigger to unlock the guard of monster.
var target : GameObject;

function OnTriggerEnter (other : Collider) {
	if (other.gameObject.tag == "Player") {
		target.GetComponent(Status).guard = false;
     }
}