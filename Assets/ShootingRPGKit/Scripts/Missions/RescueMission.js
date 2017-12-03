#pragma strict
var hostage : GameObject;
var prisonUnlock : boolean = false;

var failMessage : OperatorSet[] = new OperatorSet[1];
private var done : boolean = false;
var baseScene : String = "Base";
var rewardCash : int = 2500;
var missionUnlock : int = 2;

function Start () {
	gameObject.name = "Mission";
}

function Update () {
	if(done){
		return;
	}
	if(!hostage){
		MissionFail();
	}
}

function MissionFail(){
	done = true;
	GetComponent(Operator).otherMessage = failMessage;
	GetComponent(Operator).ShowOtherMessage();
	yield WaitForSeconds(10);
	var player : GameObject = GameObject.FindWithTag("Player");
	if(player){
		Application.LoadLevel(baseScene);
	}
}

function MissionClear(){
	done = true;
	print("Clear");
	yield WaitForSeconds(5);
	var player : GameObject = GameObject.FindWithTag("Player");
	player.GetComponent(Inventory).cash += rewardCash;
	
	var saveSlot : int = PlayerPrefs.GetInt("SaveSlot");
	var currentUnlock : int = PlayerPrefs.GetInt("MissionUnlock" +saveSlot.ToString());
	if(currentUnlock <= missionUnlock){
		PlayerPrefs.SetInt("MissionUnlock" +saveSlot.ToString(), missionUnlock);
	}
	Application.LoadLevel(baseScene);

}

@script RequireComponent(Operator)
