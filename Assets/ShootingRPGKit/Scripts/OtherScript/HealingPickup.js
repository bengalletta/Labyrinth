#pragma strict
var hpRecover : int = 20;
var mpRecover : int = 0;
private var master : Transform;

function OnTriggerEnter (col : Collider) {

	if(col.tag == "Player"){
		col.GetComponent(Status).Heal(hpRecover , mpRecover);
		master = transform.root;
    	Destroy(master.gameObject);
	}
}