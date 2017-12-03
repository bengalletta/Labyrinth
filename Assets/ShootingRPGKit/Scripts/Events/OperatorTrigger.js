#pragma strict
var operatorObj : GameObject;
var message : OperatorSet[] = new OperatorSet[1];

function OnTriggerEnter (other : Collider) {
	if (other.gameObject.tag == "Player") {
		operatorObj.GetComponent(Operator).otherMessage = message;
		operatorObj.GetComponent(Operator).ShowOtherMessage();
		Destroy(gameObject);
     }
}
 