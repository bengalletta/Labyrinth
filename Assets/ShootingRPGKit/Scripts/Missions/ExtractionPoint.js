#pragma strict
var waitAlly : boolean = false;
private var playerEnter : boolean = false;
private var allyEnter : boolean = false;
var missionObj : GameObject;
var completeSendMessage : String = "MissionClear";
private var done : boolean = false;

function Start () {
	if(!missionObj){
		missionObj = GameObject.Find("Mission");
	}
}

function Update () {
	if(done){
		return;
	}
	
	if(waitAlly && playerEnter && allyEnter){
		ClearMission();
	}else if(!waitAlly && playerEnter){
		ClearMission();
	}

}

function ClearMission(){
	done = true;
	if(completeSendMessage != ""){
		missionObj.SendMessage(completeSendMessage);
	}
}

function OnTriggerEnter (other : Collider) {
	if(other.tag == "Player"){
		playerEnter = true;
	}
	if(other.tag == "Ally"){
		allyEnter = true;
	}

}

function OnTriggerExit (other : Collider) {
	if(other.tag == "Player"){
		playerEnter = false;
	}
	if(other.tag == "Ally"){
		allyEnter = false;
	}

}
