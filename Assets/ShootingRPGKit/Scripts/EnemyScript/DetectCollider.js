#pragma strict
var master : Transform;
private var ai : AIenemy;

function Start () {
	if(!master){
		master = transform.root;
	}
	ai = master.GetComponent(AIenemy);
	gameObject.layer = 2;
	GetComponent(Rigidbody).isKinematic = true;
	GetComponent.<Collider>().isTrigger = true;
}

function OnTriggerEnter (other : Collider) {
	if(ai.followState == AIState.Moving || ai.followState == AIState.Pausing){
		return;
	}
  	if (other.gameObject.tag == "Player" || other.gameObject.tag == "Ally") {
		ai.followState = AIState.Moving;
	}
}

@script RequireComponent(Rigidbody)