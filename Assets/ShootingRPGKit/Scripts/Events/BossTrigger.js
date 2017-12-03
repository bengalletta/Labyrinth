#pragma strict
var boss : GameObject;
var destroyObj : GameObject[];

function OnTriggerEnter (other : Collider) {
	if (other.gameObject.tag == "Player") {
		boss.GetComponent(Status).guard = false;
		boss.GetComponent(AIenemy).followState = 0;
		if(destroyObj.Length > 0){
			for (var ob : GameObject in destroyObj) {
		    	Destroy(ob.gameObject);
		    }
		}
     }
}