#pragma strict
var target : GameObject;
private var done : boolean = false;
var baseScene : String = "Base";
var rewardCash : int = 5500;
var missionUnlock : int = 4;
private var menu : boolean = false;

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
//	var currentUnlock : int = PlayerPrefs.GetInt("MissionUnlock" +saveSlot.ToString());
//	if(currentUnlock <= missionUnlock){
//		PlayerPrefs.SetInt("MissionUnlock" +saveSlot.ToString(), missionUnlock);
//	}

	Destroy(player);
	OnOffMenu();
	Application.LoadLevel(baseScene);

}


function OnOffMenu(){
	//Freeze Time Scale to 0 if Window is Showing
	if(!menu && Time.timeScale != 0.0){
			menu = true;
			Time.timeScale = 0.0;
			Screen.lockCursor = false;
	}else if(menu){
			menu = false;
			Time.timeScale = 1.0;
			Screen.lockCursor = true;
	}
}

@script RequireComponent(Operator)
