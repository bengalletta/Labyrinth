#pragma strict
private var currentEnemies : int;
private var enemy : GameObject[];
var enemyTag : String = "Enemy";
var fontStyle : GUIStyle;
private var finish : boolean = false;
var completeSendMessage : String = "MissionClear";
var baseScene : String = "Base";
var rewardCash : int = 1000;
var missionUnlock : int = 1;

function Start(){
	gameObject.name = "Mission";
	enemy = GameObject.FindGameObjectsWithTag(enemyTag);
}

function Update () {
	if(finish){
		return;
	}
	enemy = GameObject.FindGameObjectsWithTag(enemyTag);
	currentEnemies = enemy.Length;
	if(enemy.Length <= 0){
		//Clear
		if(completeSendMessage != ""){
			SendMessage(completeSendMessage);
		}
		finish = true;
	}

}

function MissionClear(){
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

function OnGUI(){
	GUI.Label (Rect (Screen.width - 160, Screen.height /2 - 175, 150, 50), "Enemies", fontStyle);
	GUI.Label (Rect (Screen.width - 130, Screen.height /2 - 150 , 150, 50), currentEnemies.ToString(), fontStyle);
}