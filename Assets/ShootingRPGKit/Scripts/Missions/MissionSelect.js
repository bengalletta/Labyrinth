#pragma strict
class MissionDetails{
	var missionName : String = "";
	var unlockTier : int = 0;
	var missionScene : String = "";
}
var mission : MissionDetails[] = new MissionDetails[4];
private var showGui : boolean = false;
private var saveSlot : int = 0;
private var currentUnlock : int = 0;

function Start () {
	saveSlot = PlayerPrefs.GetInt("SaveSlot");
	currentUnlock = PlayerPrefs.GetInt("MissionUnlock" +saveSlot.ToString());
}

function Update () {

}

function OnGUI(){
	if(showGui){
		GUI.Box (Rect (Screen.width / 2 - 240,150,480,400), "Mission");
		if (GUI.Button (Rect (Screen.width / 2 + 190,155,30,30), "X")) {
			OnOffMenu();
		}
		
		if(currentUnlock >= mission[0].unlockTier){
			if (GUI.Button (Rect (Screen.width / 2 - 150,170,300,80), mission[0].missionName)) {
				StartMission(0);
			}
		}
		if(currentUnlock >= mission[1].unlockTier){
			if (GUI.Button (Rect (Screen.width / 2 - 150,260,300,80), mission[1].missionName)) {
				StartMission(1);
			}
		}
		if(currentUnlock >= mission[2].unlockTier){
			if (GUI.Button (Rect (Screen.width / 2 - 150,350,300,80), mission[2].missionName)) {
				StartMission(2);
			}
		}
		if(currentUnlock >= mission[3].unlockTier){
			if (GUI.Button (Rect (Screen.width / 2 - 150,440,300,80), mission[3].missionName)) {
				StartMission(3);
			}
		}
	}

}

function StartMission(id : int){
	showGui = false;
	Time.timeScale = 1.0;
	Screen.lockCursor = true;
	Application.LoadLevel(mission[id].missionScene);
}

function OnOffMenu(){
	//Freeze Time Scale to 0 if Window is Showing
	if(!showGui && Time.timeScale != 0.0){
			showGui = true;
			Time.timeScale = 0.0;
			Screen.lockCursor = false;
	}else if(showGui){
			showGui = false;
			Time.timeScale = 1.0;
			Screen.lockCursor = true;
	}
}
