#pragma strict
var currentLv : int = 1;
var maxLevel : int = 100;

class StatusParam{
	var atk : int = 0;
	var def : int = 0;
	var matk : int = 0;
	var mdef : int = 0;
	var melee : int = 0;
	var maxHealth : int = 100;
	var maxMana : int = 100;
	var maxShield : int = 100;
}
var minStatus : StatusParam;
var maxStatus : StatusParam;

private var min : int = 0;
private var max : int = 0;

function Start () {
	CalculateStatLv();
}

function CalculateStatLv(){
	var stat : Status = GetComponent(Status);
	currentLv = stat.level;
	//[min_stat*(max_lv-lv)/(max_lv- 1)] + [max_stat*(lv- 1)/(max_lv- 1)]

	//Atk
	min = minStatus.atk * (maxLevel - currentLv)/(maxLevel - 1);
	max = maxStatus.atk * (currentLv - 1)/(maxLevel - 1);
	stat.atk = min + max;
	//Def
	min = minStatus.def * (maxLevel - currentLv)/(maxLevel - 1);
	max = maxStatus.def * (currentLv - 1)/(maxLevel - 1);
	stat.def = min + max;
	//Matk
	min = minStatus.matk * (maxLevel - currentLv)/(maxLevel - 1);
	max = maxStatus.matk * (currentLv - 1)/(maxLevel - 1);
	stat.matk = min + max;
	//Mdef
	min = minStatus.mdef * (maxLevel - currentLv)/(maxLevel - 1);
	max = maxStatus.mdef * (currentLv - 1)/(maxLevel - 1);
	stat.mdef = min + max;
	//Melee
	min = minStatus.melee * (maxLevel - currentLv)/(maxLevel - 1);
	max = maxStatus.melee * (currentLv - 1)/(maxLevel - 1);
	stat.melee = min + max;
	//Shield
	min = minStatus.maxShield * (maxLevel - currentLv)/(maxLevel - 1);
	max = maxStatus.maxShield * (currentLv - 1)/(maxLevel - 1);
	stat.maxShield = min + max;
	stat.shield = stat.maxShield;

	//HP
	min = minStatus.maxHealth * (maxLevel - currentLv)/(maxLevel - 1);
	max = maxStatus.maxHealth * (currentLv - 1)/(maxLevel - 1);
	stat.maxHealth = min + max;
	stat.health = stat.maxHealth;
	//MP
	min = minStatus.maxMana * (maxLevel - currentLv)/(maxLevel - 1);
	max = maxStatus.maxMana * (currentLv - 1)/(maxLevel - 1);
	stat.maxMana = min + max;
	stat.mana = stat.maxMana;
	
	stat.CalculateStatus();
}

@script RequireComponent (Status)
@script AddComponentMenu ("Shooting RPG Kit/Auto Calculate Status by Level")
