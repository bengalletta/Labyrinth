#pragma strict
var damage : int = 50;
var ignoreGuard : boolean = false;

function OnTriggerEnter (other : Collider) {
		if (other.gameObject.tag == "Player") {
			other.GetComponent(Status).OnDamage(damage , 0 , ignoreGuard);
		}
 }