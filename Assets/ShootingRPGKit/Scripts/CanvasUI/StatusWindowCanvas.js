#pragma strict
import UnityEngine.UI;

var player : GameObject;
var charName : Text;
var lv : Text;
var atk : Text;
var def : Text;
var matk : Text;
var mdef : Text;
var melee : Text;
var exp : Text;
var nextLv : Text;
var stPoint : Text;

var totalAtk : Text;
var totalDef : Text;
var totalMatk : Text;
var totalMdef : Text;
var totalMelee : Text;

var atkUpButton : Button;
var defUpButton : Button;
var matkUpButton : Button;
var mdefUpButton : Button;

function Start () {
	DontDestroyOnLoad(transform.gameObject);
	if(!player){
		player = GameObject.FindWithTag("Player");
	}
}

function Update () {
	if(!player){
		Destroy(gameObject);
		return;
	}
	var stat : Status = player.GetComponent(Status);
	if(charName){
		charName.text = stat.characterName.ToString();
	}
	if(lv){
		lv.text = stat.level.ToString();
	}
	if(atk){
		atk.text = stat.atk.ToString();
	}
	if(def){
		def.text = stat.def.ToString();
	}
	if(matk){
		matk.text = stat.matk.ToString();
	}
	if(mdef){
		mdef.text = stat.mdef.ToString();
	}
	if(melee){
		melee.text = stat.melee.ToString();
	}
	
	if(exp){
		exp.text = stat.exp.ToString();
	}
	if(nextLv){
		nextLv.text = (stat.maxExp - stat.exp).ToString();
	}
	if(stPoint){
		stPoint.text = stat.statusPoint.ToString();
	}
	
	if(totalAtk){
		totalAtk.text = "(" + stat.addAtk.ToString() + ")";
	}
	if(totalDef){
		totalDef.text = "(" + (stat.def + stat.addDef + stat.buffDef).ToString() + ")";
	}
	if(totalMatk){
		totalMatk.text = "(" + stat.addMatk.ToString() + ")";
	}
	if(totalMdef){
		totalMdef.text = "(" + (stat.mdef + stat.addMdef + stat.buffMdef).ToString() + ")";
	}
	if(totalMelee){
		totalMelee.text = "(" + stat.addMelee.ToString()+ ")";
	}
	
	if(stat.statusPoint > 0){
		if(atkUpButton)
			atkUpButton.gameObject.SetActive(true);
		if(defUpButton)
			defUpButton.gameObject.SetActive(true);
		if(matkUpButton)
			matkUpButton.gameObject.SetActive(true);
		if(mdefUpButton)
			mdefUpButton.gameObject.SetActive(true);
	}else{
		if(atkUpButton)
			atkUpButton.gameObject.SetActive(false);
		if(defUpButton)
			defUpButton.gameObject.SetActive(false);
		if(matkUpButton)
			matkUpButton.gameObject.SetActive(false);
		if(mdefUpButton)
			mdefUpButton.gameObject.SetActive(false);
	}
	
}

function UpgradeStatus(statusId : int){
	//0 = Atk , 1 = Def , 2 = Matk , 3 = Mdef
	if(!player){
		return;
	}
	var stat : Status = player.GetComponent(Status);
	if(statusId == 0 && stat.statusPoint > 0){
		stat.atk += 1;
		stat.statusPoint -= 1;
		stat.CalculateStatus();
	}
	if(statusId == 1 && stat.statusPoint > 0){
		stat.def += 1;
		stat.maxHealth += 5;
		stat.statusPoint -= 1;
		stat.CalculateStatus();
	}
	if(statusId == 2 && stat.statusPoint > 0){
		stat.matk += 1;
		stat.maxMana += 3;
		stat.statusPoint -= 1;
		stat.CalculateStatus();
	}
	if(statusId == 3 && stat.statusPoint > 0){
		stat.mdef += 1;
		stat.statusPoint -= 1;
		stat.CalculateStatus();
	}
}

function CloseMenu(){
	Time.timeScale = 1.0;
	//Screen.lockCursor = true;
	Cursor.lockState = CursorLockMode.Locked;
	Cursor.visible = false;
	gameObject.SetActive(false);
}
