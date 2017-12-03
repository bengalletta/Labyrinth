#pragma strict
var monsterPrefab : GameObject[] = new GameObject[3]; //Store All prefab of Monster.
var spawnpointTag : String = "SpawnPoint"; //Spawn Point Tag where we will spawn monster.
var randomPoint : float = 10.0; //For slightly Random position from respawn point.
var setTagTo : String = "Enemy"; //Set tag of object we spawn.

class WaveBasic{
	var waveName : String = "";
	var spawnStep : int[] = new int[30]; //Spawn Monster from ID.
	var maxMonster : int = 5;
	var spawnDelay : float = 2.0;
	var sendMessage : String = ""; //Send Message to Object after spawn.
	var sendValue : int = 0;
}

var waveManage : WaveBasic[] = new WaveBasic[5];

var wave : int = 0; //Current Wave
var begin : boolean = false;
private var maxStep : int;
private var step : int;
private var wait : float = 0;
private var freeze : boolean = false;
private var showGui : boolean = false;
private var showText : String = "";
var completeSendMessage : String = "MissionClear";

var ignoreObject : GameObject[]; //Store the GameObject that Monster will ignore it's collision

var uiSkin : GUISkin;

function Start () {
	maxStep = waveManage[wave].spawnStep.Length;
	yield WaitForSeconds(10);
	BeginWave();
}

function BeginWave(){
	var w : int = wave + 1;
	showText = "Wave " + w.ToString();
	showGui = true;
	yield WaitForSeconds(4.5);
	begin = true;
	showGui = false;
}

function OnGUI(){
	if(showGui){ //Draw Current Wave GUI
		GUI.skin = uiSkin;
		GUI.Box (Rect (Screen.width /2 -120 ,Screen.height /2 -40 ,240,80), showText);
	}
}

function Update () {
	if(!begin || freeze){
		return;
	}
	
	if(step >= maxStep){ //Check After we spawn all of objects in current wave.
		var h : GameObject[] = GameObject.FindGameObjectsWithTag(setTagTo);
		if(h.Length <= 0){
			WaveClear();
		}
		return;
	}
	
	if(wait >= waveManage[wave].spawnDelay){
		SpawnMon();
		wait = 0;
	}else{
		wait += Time.deltaTime;
	}
}

function SpawnMon(){
	var h : GameObject[] = GameObject.FindGameObjectsWithTag(setTagTo);
	if(h.Length > waveManage[wave].maxMonster){ // Not spawn if current monsters more than Max Monster
		return;
	}
	
	var spawnpoints : GameObject[] = GameObject.FindGameObjectsWithTag (spawnpointTag);
	 if(spawnpoints.Length > 0){
		var spawnpoint : Transform = spawnpoints[Random.Range(0, spawnpoints.length)].transform;
		 
		var ranPos : Vector3 = spawnpoint.position; //Slightly Random position from respawn point.
		ranPos.x += Random.Range(0.0,randomPoint);
		ranPos.z += Random.Range(0.0,randomPoint);
		
		//Spawn Prefab
		var en : GameObject = Instantiate(monsterPrefab[waveManage[wave].spawnStep[step]], ranPos , spawnpoint.rotation);
		//Set Tag and Name
		en.name = monsterPrefab[waveManage[wave].spawnStep[step]].name;
		en.tag = setTagTo;
		//Set Ignore Collision
		if(ignoreObject.Length > 0){
			IgnoreCol(en);
		}
		//Send Message to the Object we spawned
		if(waveManage[wave].sendMessage != ""){
			en.SendMessage(waveManage[wave].sendMessage , waveManage[wave].sendValue);
		}
	 }
	step++;
}

function WaveClear(){
	freeze = true;
	showText = "Wave Clear";
	showGui = true;
	yield WaitForSeconds(4.5);
	showGui = false;
	yield WaitForSeconds(2.5);
	//Begin New Wave
	if(wave < waveManage.Length -1){
		wave++;
		step = 0;
		maxStep = waveManage[wave].spawnStep.Length;
		BeginWave();
		freeze = false;
	}else{
		ClearMission();
	}
}

function ClearMission(){
	showText = "Mission Clear";
	showGui = true;
	yield WaitForSeconds(4.5);
	showGui = false;
	//Clear
	if(completeSendMessage != ""){
		SendMessage(completeSendMessage);
	}
}

function MissionFail(){
	freeze = true;
	showText = "Mission Fail";
	showGui = true;
	yield WaitForSeconds(4.5);
	showGui = false;
	yield WaitForSeconds(3);
}

function IgnoreCol(mon : GameObject){
	//Physics.IgnoreCollision(collider, master.collider);
    for (var pl : GameObject in ignoreObject) {
    	if(pl != this.gameObject){
    		Physics.IgnoreCollision(mon.GetComponent.<Collider>(), pl.GetComponent.<Collider>());
    	}
    }

}
