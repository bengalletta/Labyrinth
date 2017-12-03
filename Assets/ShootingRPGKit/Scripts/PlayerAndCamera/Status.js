#pragma strict
var mainModel : GameObject;
var characterName : String = "";
var playerId : int = 0;
@Range (1, 100)
var level : int = 1;
var atk : int = 0;
var def : int = 0;
var matk : int = 0;
var mdef : int = 0;
var melee : int = 0;
var exp : int = 0;
var maxExp : int = 100;
var maxHealth : int = 100;
var health : int = 100;
var maxMana : int = 100;
var mana : int = 100;
var maxShield : int = 100;
var shield : int = 100;
var guard : boolean = false;

var shieldRecovery : int = 2;
var shieldRecoveryDelay : float = 5.0f;

@HideInInspector
var maxShieldPlus : int = 100;
private var recoverShield : float = 0.0f;
private var onHit : boolean = false;

var statusPoint : int = 0;
var skillPoint : int = 0;
private var dead : boolean = false;
var stability : boolean = false;	// Character will not flinch if it set to true.
var immortal : boolean = false; // Character will take no damage if it set to true.

@HideInInspector
var addAtk : int = 0;
@HideInInspector
var addDef : int = 0;
@HideInInspector
var addMatk : int = 0;
@HideInInspector
var addMdef : int = 0;
@HideInInspector
var addMelee : int = 0;
@HideInInspector
var addHPpercent : int = 0;
@HideInInspector
var addMPpercent : int = 0;

var deathPrefab : Transform;

@HideInInspector
var spawnPointName : String = "PlayerSpawnPoint"; //Store the name for Spawn Point When Change Scene

//---------States----------
@HideInInspector
var buffAtk : int = 0;
@HideInInspector
var buffDef : int = 0;
@HideInInspector
var buffMatk : int = 0;
@HideInInspector
var buffMdef : int = 0;
@HideInInspector
var buffMelee : int = 0;

@HideInInspector
var currentWeaponAtk : int = 0;
@HideInInspector
var weaponAtk : int = 0;
@HideInInspector
var weaponAtk2 : int = 0;
@HideInInspector
var weaponMatk : int = 0;
@HideInInspector
var weaponMelee : int = 0;
@HideInInspector
var armorDef : int = 0;
@HideInInspector
var armorMdef : int = 0;
//@HideInInspector
var armorShield : int = 0;

@HideInInspector
var flinch : boolean = false;
@HideInInspector
var dodge : boolean = false;
@HideInInspector
var hurt : AnimationClip;

//Negative Buffs
@HideInInspector
var poison : boolean = false;
@HideInInspector
var silence : boolean = false;
@HideInInspector
var web : boolean = false;
@HideInInspector
var stun : boolean = false;

@HideInInspector
var freeze : boolean = false; // Use for Freeze Character

//Positive Buffs
@HideInInspector
var brave : boolean = false;
@HideInInspector
var barrier : boolean = false;
@HideInInspector
var mbarrier : boolean = false;
@HideInInspector
var faith : boolean = false;
@HideInInspector
var sharp : boolean = false;

//Effect
var poisonEffect : GameObject;
var silenceEffect : GameObject;
var stunEffect : GameObject;
var webbedUpEffect : GameObject;

var stunAnimation : AnimationClip;
var webbedUpAnimation : AnimationClip;

@HideInInspector
var hurtVoice : AudioClip;
@HideInInspector
var useMecanim : boolean = false;

class elem{
	var elementName : String = "";
	var effective : int = 100;
}
var elementEffective : elem[] = new elem[5];
// 0 = Normal , 1 = Fire , 2 = Ice , 3 = Earth , 4 = Wind

class resist{
	var poisonResist : int = 0;
	var silenceResist : int = 0;
	var webResist : int = 0;
	var stunResist : int = 0;
}
var statusResist : resist;

function Awake () {
	if(!mainModel){
		mainModel = this.gameObject;
	}
	CalculateStatus();
}

function Update(){
	//Shield Recovery
	if(onHit && maxShieldPlus > 0){
		if(recoverShield >= shieldRecoveryDelay){
			ShieldRecover();
		}else{
			recoverShield += Time.deltaTime;
		}
	}
}

function OnDamage (amount : int , element : int , ignoreGuard : boolean) : String  {	
	if(dead){
		return;
	}
	if(immortal){
		return "Invulnerable";
	}
	if(guard && !ignoreGuard){
		return "Guard";
	}
	if(dodge){
		return "Evaded";
	}
	if(hurtVoice){
		GetComponent.<AudioSource>().clip = hurtVoice;
		GetComponent.<AudioSource>().Play();
	}

	amount -= addDef;
	
	//Calculate Element Effective
	amount *= elementEffective[element].effective;
	amount /= 100;
	
	if(amount < 1){
		amount = 1;
	}
	
	//Get Damage to Shield First
	onHit = true;
	recoverShield = 0.0f;
	if(shield > 0){
		shield -= amount;
		if(shield <= 0){
			health += shield; //Reduce HP while shield is - value.
			shield = 0;
		}
	}else{
		health -= amount; //Damage to Health when you out of shield.
	}

	if (health <= 0){
		health = 0;
		enabled = false;
		dead = true;
		Death();
	}
	return amount.ToString();
}

function OnMagicDamage (amount : int  , element : int , ignoreGuard : boolean) : String {
	if(dead){
		return;
	}
	if(immortal){
		return "Invulnerable";
	}
	if(guard && !ignoreGuard){
		return "Guard";
	}
	if(dodge){
		return "Evaded";
	}
	if(hurtVoice){
		GetComponent.<AudioSource>().clip = hurtVoice;
		GetComponent.<AudioSource>().Play();
	}

	amount -= addMdef;
	
	//Calculate Element Effective
	amount *= elementEffective[element].effective;
	amount /= 100;
	
	if(amount < 1){
		amount = 1;
	}
	
	//Get Damage to Shield First
	onHit = true;
	recoverShield = 0.0f;
	if(shield > 0){
		shield -= amount;
		if(shield <= 0){
			health += shield; //Reduce HP while shield is - value.
			shield = 0;
		}
	}else{
		health -= amount; //Damage to Health when you out of shield.
	}

	if (health <= 0){
		health = 0;
		enabled = false;
		dead = true;
		Death();
	}
	return amount.ToString();
}

function ShieldRecover(){
	var amount : int = maxShieldPlus * shieldRecovery / 100;
		if(amount <= 1){
			amount = 1;
		}
	shield += amount;
	if(shield >= maxShieldPlus){
		shield = maxShieldPlus;
		recoverShield = 0.0f;
		onHit = false;
	}else{
		recoverShield = shieldRecoveryDelay - 0.1f;
	}
}

function Heal(hp : int , mp : int){
	health += hp;
	if (health >= maxHealth){
		health = maxHealth;
	}
	
	mana += mp;
	if (mana >= maxMana){
		mana = maxMana;
	}
}

function ShieldRecovery(amount : int){
	shield += amount;
	if (shield >= maxShieldPlus){
		shield = maxShieldPlus;
	}

}


function Death(){
	if(gameObject.tag == "Player"){
		SaveData();
		if(GetComponent(UiMaster)){
			GetComponent(UiMaster).DestroyAllUi();
		}
	}
	Destroy(gameObject);
	if(deathPrefab){
		Instantiate(deathPrefab, mainModel.transform.position , mainModel.transform.rotation);
	}else{
		print("This Object didn't assign the Death Body");
	}
}

function gainEXP(gain : int){
	exp += gain;
	if(exp >= maxExp){
		var remain : int = exp - maxExp;
		LevelUp(remain);
	}
}

function LevelUp(remainingEXP : int){
	exp = 0;
	exp += remainingEXP;
	level++;
	skillPoint++;
	//If your character have AutoCalculate Status by Level
	if(GetComponent(AutoCalculateStatus)){
		GetComponent(AutoCalculateStatus).CalculateStatLv();
	}else{
		//If your character not have AutoCalculate.
		statusPoint += 5;
		//Extend Max Health and Max Mana
		maxHealth += 20;
		maxMana += 10;
		//Recover Health and Mana
		health = maxHealth;
		mana = maxMana;
	}
	//Extend Max EXP
	maxExp = 1.4 * maxExp;
	
	gainEXP(0);
	if(GetComponent(SkillWindow)){
		GetComponent(SkillWindow).LearnSkillByLevel(level);
	}
}

function SaveData(){
	//Save Temp Data
	var saveSlot : int = -1; //-1 is a Temp ID
	var player : GameObject = this.gameObject;
			PlayerPrefs.SetInt("PreviousSave" +saveSlot.ToString(), 10);
			PlayerPrefs.SetString("Name" +saveSlot.ToString(), player.GetComponent(Status).characterName);
			PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
			PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
			PlayerPrefs.SetFloat("PlayerZ", player.transform.position.z);
			PlayerPrefs.SetInt("PlayerLevel" +saveSlot.ToString(), player.GetComponent(Status).level);
			PlayerPrefs.SetInt("PlayerID" +saveSlot.ToString(), player.GetComponent(Status).playerId);
			
			PlayerPrefs.SetInt("PlayerATK" +saveSlot.ToString(), player.GetComponent(Status).atk);
			PlayerPrefs.SetInt("PlayerDEF" +saveSlot.ToString(), player.GetComponent(Status).def);
			PlayerPrefs.SetInt("PlayerMATK" +saveSlot.ToString(), player.GetComponent(Status).matk);
			PlayerPrefs.SetInt("PlayerMDEF" +saveSlot.ToString(), player.GetComponent(Status).mdef);
			PlayerPrefs.SetInt("PlayerMelee" +saveSlot.ToString(), player.GetComponent(Status).melee);
			PlayerPrefs.SetInt("PlayerEXP" +saveSlot.ToString(), player.GetComponent(Status).exp);
			PlayerPrefs.SetInt("PlayerMaxEXP" +saveSlot.ToString(), player.GetComponent(Status).maxExp);
			PlayerPrefs.SetInt("PlayerMaxHP" +saveSlot.ToString(), player.GetComponent(Status).maxHealth);
			PlayerPrefs.SetInt("PlayerHP" +saveSlot.ToString(), player.GetComponent(Status).maxHealth);
			PlayerPrefs.SetInt("PlayerMaxMP" +saveSlot.ToString(), player.GetComponent(Status).maxMana);
		//	PlayerPrefs.SetInt("PlayerMP", player.GetComponent(Status).mana);
			PlayerPrefs.SetInt("PlayerMaxShield" +saveSlot.ToString(), player.GetComponent(Status).maxShield);
			PlayerPrefs.SetInt("PlayerSTP" +saveSlot.ToString(), player.GetComponent(Status).statusPoint);
			PlayerPrefs.SetInt("PlayerSTK" +saveSlot.ToString(), player.GetComponent(Status).skillPoint);
			
			PlayerPrefs.SetInt("Cash" +saveSlot.ToString(), player.GetComponent(Inventory).cash);
			var itemSize : int = player.GetComponent(Inventory).itemSlot.length;
			var a : int = 0;
			if(itemSize > 0){
				while(a < itemSize){
					PlayerPrefs.SetInt("Item" + a.ToString() +saveSlot.ToString(), player.GetComponent(Inventory).itemSlot[a]);
					PlayerPrefs.SetInt("ItemQty" + a.ToString() +saveSlot.ToString(), player.GetComponent(Inventory).itemQuantity[a]);
					a++;
				}
			}
			
			var equipSize : int = player.GetComponent(Inventory).equipment.length;
			a = 0;
			if(equipSize > 0){
				while(a < equipSize){
					PlayerPrefs.SetInt("Equipm" + a.ToString() +saveSlot.ToString(), player.GetComponent(Inventory).equipment[a]);
					PlayerPrefs.SetInt("EquipAmmo" + a.ToString() +saveSlot.ToString(), player.GetComponent(Inventory).equipAmmo[a]);
					a++;
				}
			}
			PlayerPrefs.SetInt("PrimaryEquip" +saveSlot.ToString(), player.GetComponent(Inventory).primaryEquip);
			PlayerPrefs.SetInt("SecondaryEquip" +saveSlot.ToString(), player.GetComponent(Inventory).secondaryEquip);
			PlayerPrefs.SetInt("MeleeEquip" +saveSlot.ToString(), player.GetComponent(Inventory).meleeEquip);
			PlayerPrefs.SetInt("ArmoEquip" +saveSlot.ToString(), player.GetComponent(Inventory).armorEquip);
			
			PlayerPrefs.SetInt("PrimaryAmmo" +saveSlot.ToString(), player.GetComponent(GunTrigger).primaryWeapon.ammo);
			PlayerPrefs.SetInt("SecondaryAmmo" +saveSlot.ToString(), player.GetComponent(GunTrigger).secondaryWeapon.ammo);
			//Save Skill Slot
			a = 0;
				while(a < player.GetComponent(SkillWindow).skill.length){
					PlayerPrefs.SetInt("Skill" + a.ToString() +saveSlot.ToString(), player.GetComponent(SkillWindow).skill[a]);
					a++;
			}
			//Skill List Slot
			a = 0;
			while(a < player.GetComponent(SkillWindow).skillListSlot.length){
				PlayerPrefs.SetInt("SkillList" + a.ToString() +saveSlot.ToString(), player.GetComponent(SkillWindow).skillListSlot[a]);
				a++;
			}
			
			//Save Ammo
			PlayerPrefs.SetInt("HandgunAmmo" +saveSlot.ToString(), player.GetComponent(GunTrigger).allAmmo.handgunAmmo);
			PlayerPrefs.SetInt("MachinegunAmmo" +saveSlot.ToString(), player.GetComponent(GunTrigger).allAmmo.machinegunAmmo);
			PlayerPrefs.SetInt("ShotgunAmmo" +saveSlot.ToString(), player.GetComponent(GunTrigger).allAmmo.shotgunAmmo);
			PlayerPrefs.SetInt("MagnumAmmo" +saveSlot.ToString(), player.GetComponent(GunTrigger).allAmmo.magnumAmmo);
			PlayerPrefs.SetInt("SmgAmmo" +saveSlot.ToString(), player.GetComponent(GunTrigger).allAmmo.smgAmmo);
			PlayerPrefs.SetInt("SniperRifleAmmo" +saveSlot.ToString(), player.GetComponent(GunTrigger).allAmmo.sniperRifleAmmo);
			PlayerPrefs.SetInt("GrenadeRounds" +saveSlot.ToString(), player.GetComponent(GunTrigger).allAmmo.grenadeRounds);
}

function CalculateStatus(){
		addAtk = atk + buffAtk + currentWeaponAtk;
		addDef = def + buffDef + armorDef;
		addMatk = matk + buffMatk + weaponMatk;
		addMdef = mdef + buffMdef + armorMdef;
		addMelee = melee + buffMelee + weaponMelee;
		maxShieldPlus = maxShield + armorShield;
		
		if(shield >= maxShieldPlus){
			shield = maxShieldPlus;
		}
		if(shield <= maxShieldPlus){
			onHit = true;
			//recoverShield = 0.0f;
		}
		
		var hpPer : int = maxHealth * addHPpercent / 100;
		var mpPer : int = maxMana * addMPpercent / 100;
		maxHealth += hpPer;
		maxMana += mpPer;
		if (health >= maxHealth){
			health = maxHealth;
		}
		if (mana >= maxMana){
			mana = maxMana;
		}
}

//----------States--------
function OnPoison(hurtTime : int){
	if(poison){
		return;
	}
	var chance = 100;
	chance -= statusResist.poisonResist;
	if(chance > 0){
		var per = Random.Range(0, 100);
		if(per <= chance){
			poison = true;
			var amount : int = maxHealth * 5 / 100; // Hurt 5% of Max HP
		}else{
			return;
		}
		
	}else{
		return;
	}
	//--------------------
	while(poison && hurtTime > 0){
		if(poisonEffect){ //Show Poison Effect
				var eff : GameObject = Instantiate(poisonEffect, transform.position, poisonEffect.transform.rotation);
				eff.transform.parent = transform;
			}
		yield WaitForSeconds(0.7); // Reduce HP  Every 0.7 Seconds
		health -= amount;
	
		if (health <= 1){
			health = 1;
		}
		if(eff){ //Destroy Effect if it still on a map
			Destroy(eff.gameObject);
		}
		hurtTime--;
		if(hurtTime <= 0){
			poison = false;
		}
	}
}


function OnSilence(dur : float){
	if(silence){
		return;
	}
	var chance = 100;
	chance -= statusResist.silenceResist;
	if(chance > 0){
		var per = Random.Range(0, 100);
		if(per <= chance){
			silence = true;
			if(silenceEffect){ //Show Poison Effect
				var eff : GameObject = Instantiate(silenceEffect, transform.position, transform.rotation);
				eff.transform.parent = transform;
			}
		}else{
			return;
		}
		
	}else{
		return;
	}
	yield WaitForSeconds(dur);
	if(eff){ //Destroy Effect if it still on a map
			Destroy(eff.gameObject);
		}
	silence = false;

}

function OnWebbedUp(dur : float){
	if(web){
		return;
	}
	var chance = 100;
	chance -= statusResist.webResist;
	if(chance > 0){
		var per = Random.Range(0, 100);
		if(per <= chance){
			web = true;
			freeze = true; // Freeze Character On (Character cannot do anything)
			if(webbedUpEffect){ //Show Poison Effect
				var eff : GameObject = Instantiate(webbedUpEffect, transform.position, transform.rotation);
				eff.transform.parent = transform;
			}
			if(webbedUpAnimation){// If you Assign the Animation then play it
				if(useMecanim){
					GetComponent(PlayerMecanimAnimation).PlayAnim(webbedUpAnimation.name);
				}else{
					mainModel.GetComponent.<Animation>()[webbedUpAnimation.name].layer = 25;
					mainModel.GetComponent.<Animation>().Play(webbedUpAnimation.name);
				}
			}
		}else{
			return;
		}
		
	}else{
		return;
	}
	yield WaitForSeconds(dur);
	if(eff){ //Destroy Effect if it still on a map
			Destroy(eff.gameObject);
		}
	if(webbedUpAnimation && !useMecanim){// If you Assign the Animation then stop playing
		mainModel.GetComponent.<Animation>().Stop(webbedUpAnimation.name);
	}
	freeze = false; // Freeze Character Off
	web = false;

}

function OnStun(dur : float){
	if(stun){
		return;
	}
	var chance = 100;
	chance -= statusResist.stunResist;
	if(chance > 0){
		var per = Random.Range(0, 100);
		if(per <= chance){
			stun = true;
			freeze = true; // Freeze Character On (Character cannot do anything)
			if(stunEffect){ //Show Stun Effect
				var eff : GameObject = Instantiate(stunEffect, transform.position, stunEffect.transform.rotation);
				eff.transform.parent = transform;
			}
			if(stunAnimation){// If you Assign the Animation then play it
				if(useMecanim){
					GetComponent(PlayerMecanimAnimation).PlayAnim(stunAnimation.name);
				}else{
					mainModel.GetComponent.<Animation>()[stunAnimation.name].layer = 25;
					mainModel.GetComponent.<Animation>().Play(stunAnimation.name);
				}
			}
		}else{
			return;
		}
		
	}else{
		return;
	}
	yield WaitForSeconds(dur);
	if(eff){ //Destroy Effect if it still on a map
			Destroy(eff.gameObject);
		}
	if(stunAnimation && !useMecanim){// If you Assign the Animation then stop playing
		mainModel.GetComponent.<Animation>().Stop(stunAnimation.name);
	}
	freeze = false; // Freeze Character Off
	stun = false;

}

function ApplyAbnormalStat(statId : int , dur : float){
	if(statId == 0){
		OnPoison(Mathf.FloorToInt(dur));
	}
	if(statId == 1){
		OnSilence(dur);
	}
	if(statId == 2){
		OnStun(dur);
	}
	if(statId == 3){
		OnWebbedUp(dur);
	}
	

}


function OnBarrier(amount : float , dur : float){
	//Increase Defense
	if(barrier){
		return;
	}
	barrier = true;
	buffDef = 0;
	buffDef += amount;
	CalculateStatus();
	yield WaitForSeconds(dur);
	buffDef = 0;
	barrier = false;
	CalculateStatus();

}

function OnMagicBarrier(amount : float , dur : float){
	//Increase Magic Defense
	if(mbarrier){
		return;
	}
	mbarrier = true;
	buffMdef = 0;
	buffMdef += amount;
	CalculateStatus();
	yield WaitForSeconds(dur);
	buffMdef = 0;
	mbarrier = false;
	CalculateStatus();

}

function OnBrave(amount : float , dur : float){
	//Increase Attack
	if(brave){
		return;
	}
	brave = true;
	buffMelee = 0;
	buffMelee += amount;
	CalculateStatus();
	yield WaitForSeconds(dur);
	buffMelee = 0;
	brave = false;
	CalculateStatus();

}

function OnSharp(amount : float , dur : float){
	//Increase Attack
	if(sharp){
		return;
	}
	sharp = true;
	buffAtk = 0;
	buffAtk += amount;
	CalculateStatus();
	yield WaitForSeconds(dur);
	buffAtk = 0;
	sharp = false;
	CalculateStatus();

}

function OnFaith(amount : float , dur : float){
	//Increase Magic Attack
	if(faith){
		return;
	}
	faith = true;
	buffMatk = 0;
	buffMatk += amount;
	CalculateStatus();
	yield WaitForSeconds(dur);
	buffMatk = 0;
	faith = false;
	CalculateStatus();

}

function ApplyBuff(statId : int , dur : float , amount : int){
	if(statId == 1){
		//Increase Defense
		OnBarrier(amount , dur);
	}
	if(statId == 2){
		//Increase Magic Defense
		OnMagicBarrier(amount , dur);
	}
	if(statId == 3){
		//Increase Melee Attack
		OnBrave(amount , dur);
	}
	if(statId == 4){
		//Increase Magic Attack
		OnFaith(amount , dur);
	}
	if(statId == 5){
		//Increase Gun Attack
		OnSharp(amount , dur);
	}

}

function Flinch(){
	if(stability){
		return;
	}
	KnockBack();
	if(hurt && !useMecanim && mainModel){
		//For Legacy Animation
		mainModel.GetComponent.<Animation>()[hurt.name].layer = 4;
		mainModel.GetComponent.<Animation>().PlayQueued(hurt.name, QueueMode.PlayNow);
	}
}
	
function KnockBack(){
	flinch = true;
	yield WaitForSeconds(0.2);
	flinch = false;
}