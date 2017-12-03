#pragma strict
var door : Transform;
var doorMoveDirection : Vector3 = Vector3.down;
var moveSpeed : float = 4.0;
var moveDuration : float = 3.0;
private var opened : boolean = false;
private var done : boolean = false;

function Start () {
	if(!door){
		door = transform.root;
	}
}

function Update () {
	if(opened && !done){
		door.Translate(doorMoveDirection * moveSpeed * Time.deltaTime);
	}
}

function OnTriggerEnter (other : Collider) {
	if(other.tag == "Player"){
		Open();
	}

}

function Open(){
	if(done || opened){
		return;
	}
	opened = true;
	yield WaitForSeconds(moveDuration);
	done = true;
	opened = false;
	Destroy(gameObject);

}
