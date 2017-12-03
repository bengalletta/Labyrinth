#pragma strict
var target : GameObject;
private var done : boolean = false;
var baseScene : String = "Base";
var rewardCash : int = 5500;
var missionUnlock : int = 4;

function Start () {
	gameObject.name = "Mission";
}

function Update () {
	if(done){
		return;
	}
	if(!target){
		SendMessage("MissionClear");
	}
}

function MissionClear(){
	done = true;
	print("Clear");
	yield WaitForSeconds(10);
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
