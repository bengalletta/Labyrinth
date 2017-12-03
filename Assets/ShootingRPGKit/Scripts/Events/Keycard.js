#pragma strict
//Use with Rescue Mission.
private var mission : GameObject;
private var master : Transform;

function OnTriggerEnter (other : Collider) {
		//Pick up Item
	if (other.gameObject.tag == "Player") {
		mission = GameObject.Find("Mission");
		if(mission){
			mission.GetComponent(RescueMission).prisonUnlock = true;
		}
		master = transform.root;
    	Destroy(master.gameObject);
     }
 }