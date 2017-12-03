#pragma strict
var door : Transform;
var doorMoveDirection : Vector3 = Vector3.down;
var moveSpeed : float = 4.0;
var moveDuration : float = 3.0;
private var opened : boolean = false;
private var done : boolean = false;
var missionObj : GameObject;

var message : TextDialogue[] = new TextDialogue[1];

function Start () {
	if(!missionObj){
		missionObj = GameObject.Find("Mission");
	}
	if(!door){
		door = transform.root;
	}
}

function Update () {
	if(opened && !done){
		door.Translate(doorMoveDirection * moveSpeed * Time.deltaTime);
	}
}

function Open(){
	if(done || opened){
		return;
	}
	if(missionObj.GetComponent(RescueMission).prisonUnlock){
		opened = true;
		yield WaitForSeconds(moveDuration);
		done = true;
		opened = false;
		Destroy(gameObject);
	}else{
		GetComponent(Dialogue).message = message;
		//GetComponent(Dialogue).NextPage();
	}

}

@script RequireComponent(Dialogue)
