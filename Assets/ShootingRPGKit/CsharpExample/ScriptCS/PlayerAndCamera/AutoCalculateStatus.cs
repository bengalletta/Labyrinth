using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Status))]
[AddComponentMenu("Shooting-RPG Kit(CS)/Auto Calculate Status by Level")]
public class AutoCalculateStatus : MonoBehaviour {
	public int currentLv = 1;
	public int maxLevel = 100;

	public StatusParam minStatus;
	public StatusParam maxStatus;
	
	private int min = 0;
	private int max = 0;

	void Start (){
		CalculateStatLv();
	}
	
	public void CalculateStatLv (){
		Status stat = GetComponent<Status>();
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
			
}

[System.Serializable]
public class StatusParam{
	public int atk = 0;
	public int def = 0;
	public int matk = 0;
	public int mdef = 0;
	public int melee = 0;
	public int maxHealth = 100;
	public int maxMana = 100;
	public int maxShield = 100;
}