using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour {
	public GameObject mainModel;
	public string characterName = "";
	public int playerId = 0;
	[Range (1, 100)]
		public int level = 1;
	public int atk = 0;
	public int def = 0;
	public int matk = 0;
	public int mdef = 0;
	public int melee = 0;
	public int exp = 0;
	public int maxExp = 100;
	public int maxHealth = 100;
	public int health = 100;
	public int maxMana = 100;
	public int mana = 100;
	public int maxShield = 100;
	public int shield = 100;
	public bool guard = false;
	
	public int shieldRecovery = 2;
	public float shieldRecoveryDelay = 5.0f;
	
	[HideInInspector]
		public int maxShieldPlus = 100;
	private float recoverShield = 0.0f;
	private bool onHit = false;
	
	public int statusPoint = 0;
	public int skillPoint = 0;
	private bool dead = false;
	public bool stability = false;	// Character will not flinch if it set to true.
	public bool immortal = false; // Character will take no damage if it set to true.
	
	[HideInInspector]
		public int addAtk = 0;
	[HideInInspector]
		public int addDef = 0;
	[HideInInspector]
		public int addMatk = 0;
	[HideInInspector]
		public int addMdef = 0;
	[HideInInspector]
		public int addMelee = 0;
	[HideInInspector]
		public int addHPpercent = 0;
	[HideInInspector]
		public int addMPpercent = 0;
	
	public Transform deathPrefab;

	[HideInInspector]
		public string spawnPointName = "PlayerSpawnPoint"; //Store the name for Spawn Point When Change Scene
	
	//---------States----------
	[HideInInspector]
		public int buffAtk = 0;
	[HideInInspector]
		public int buffDef = 0;
	[HideInInspector]
		public int buffMatk = 0;
	[HideInInspector]
		public int buffMdef = 0;
	[HideInInspector]
		public int buffMelee = 0;
	
	[HideInInspector]
		public int currentWeaponAtk = 0;
	[HideInInspector]
		public int weaponAtk = 0;
	[HideInInspector]
		public int weaponAtk2 = 0;
	[HideInInspector]
		public int weaponMatk = 0;
	[HideInInspector]
		public int weaponMelee = 0;
	[HideInInspector]
		public int armorDef = 0;
	[HideInInspector]
		public int armorMdef = 0;
	//@HideInInspector
	public int armorShield = 0;

	[HideInInspector]
		public bool flinch = false;
	[HideInInspector]
		public bool dodge = false;
	[HideInInspector]
		public AnimationClip hurt;
	
	//Negative Buffs
	[HideInInspector]
		public bool poison = false;
	[HideInInspector]
		public bool silence = false;
	[HideInInspector]
		public bool web = false;
	[HideInInspector]
		public bool stun = false;
	
	[HideInInspector]
		public bool freeze = false; // Use for Freeze Character
	
	//Positive Buffs
	[HideInInspector]
		public bool brave = false;
	[HideInInspector]
		public bool barrier = false;
	[HideInInspector]
		public bool mbarrier = false;
	[HideInInspector]
		public bool faith = false;
	[HideInInspector]
		public bool sharp = false;
	
	//Effect
	public GameObject poisonEffect;
	public GameObject silenceEffect;
	public GameObject stunEffect;
	public GameObject webbedUpEffect;
	
	public AnimationClip stunAnimation;
	public AnimationClip webbedUpAnimation;
	
	[HideInInspector]
		public AudioClip hurtVoice;
	[HideInInspector]
		public bool useMecanim = false;

	public elem[] elementEffective = new elem[5];
	// 0 = Normal , 1 = Fire , 2 = Ice , 3 = Earth , 4 = Wind
	public resist statusResist;
	
	void Awake(){
		if(!mainModel){
			mainModel = this.gameObject;
		}
		CalculateStatus();
	}
	
	void Update(){
		//Shield Recovery
		if(onHit && maxShieldPlus > 0){
			if(recoverShield >= shieldRecoveryDelay){
				ShieldRecover();
			}else{
				recoverShield += Time.deltaTime;
			}
		}
	}
	
	public string OnDamage(int amount , int element , bool ignoreGuard){	
		if(dead){
			return "";
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
			GetComponent<AudioSource>().clip = hurtVoice;
			GetComponent<AudioSource>().Play();
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
	
	public string OnMagicDamage(int amount , int element , bool ignoreGuard){
		if(dead){
			return "";
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
			GetComponent<AudioSource>().clip = hurtVoice;
			GetComponent<AudioSource>().Play();
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
		
		if(health <= 0){
			health = 0;
			enabled = false;
			dead = true;
			Death();
		}
		return amount.ToString();
	}
	
	void ShieldRecover(){
		int amount = maxShieldPlus * shieldRecovery / 100;
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

	public void Heal(int hp , int mp){
		health += hp;
		if(health >= maxHealth){
			health = maxHealth;
		}
		
		mana += mp;
		if (mana >= maxMana){
			mana = maxMana;
		}
	}

	public void ShieldRecovery(int amount){
		shield += amount;
		if (shield >= maxShieldPlus){
			shield = maxShieldPlus;
		}
	}

	public void Death(){
		if(gameObject.tag == "Player"){
			SaveData();
			if(GetComponent<UiMasterC>()){
				GetComponent<UiMasterC>().DestroyAllUi();
			}
		}
		Destroy(gameObject);
		if(deathPrefab){
			Instantiate(deathPrefab, mainModel.transform.position , mainModel.transform.rotation);
		}else{
			print("This Object didn't assign the Death Body");
		}
	}
	
	public void gainEXP ( int gain  ){
		exp += gain;
		if(exp >= maxExp){
			int remain = exp - maxExp;
			LevelUp(remain);
		}
	}
	
	public void LevelUp ( int remainingEXP  ){
		exp = 0;
		exp += remainingEXP;
		level++;
		skillPoint++;
		
		//If your character have AutoCalculate Status by Level
		if(GetComponent<AutoCalculateStatus>()){
			GetComponent<AutoCalculateStatus>().CalculateStatLv();
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
		maxExp = 125 / 100 * maxExp;
		
		gainEXP(0);
		if(GetComponent<SkillWindow>()){
			GetComponent<SkillWindow>().LearnSkillByLevel(level);
		}
	}
	
	void SaveData (){
		//Save Temp Data
		int saveSlot = -1; //-1 is a Temp ID
		GameObject player = this.gameObject;
		PlayerPrefs.SetInt("PreviousSave" +saveSlot.ToString(), 10);
		PlayerPrefs.SetString("Name" +saveSlot.ToString(), player.GetComponent<Status>().characterName);
		PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
		PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
		PlayerPrefs.SetFloat("PlayerZ", player.transform.position.z);
		PlayerPrefs.SetInt("PlayerLevel" +saveSlot.ToString(), player.GetComponent<Status>().level);
		PlayerPrefs.SetInt("PlayerID" +saveSlot.ToString(), player.GetComponent<Status>().playerId);
		
		PlayerPrefs.SetInt("PlayerATK" +saveSlot.ToString(), player.GetComponent<Status>().atk);
		PlayerPrefs.SetInt("PlayerDEF" +saveSlot.ToString(), player.GetComponent<Status>().def);
		PlayerPrefs.SetInt("PlayerMATK" +saveSlot.ToString(), player.GetComponent<Status>().matk);
		PlayerPrefs.SetInt("PlayerMDEF" +saveSlot.ToString(), player.GetComponent<Status>().mdef);
		PlayerPrefs.SetInt("PlayerMelee" +saveSlot.ToString(), player.GetComponent<Status>().melee);
		PlayerPrefs.SetInt("PlayerEXP" +saveSlot.ToString(), player.GetComponent<Status>().exp);
		PlayerPrefs.SetInt("PlayerMaxEXP" +saveSlot.ToString(), player.GetComponent<Status>().maxExp);
		PlayerPrefs.SetInt("PlayerMaxHP" +saveSlot.ToString(), player.GetComponent<Status>().maxHealth);
		PlayerPrefs.SetInt("PlayerHP" +saveSlot.ToString(), player.GetComponent<Status>().maxHealth);
		PlayerPrefs.SetInt("PlayerMaxMP" +saveSlot.ToString(), player.GetComponent<Status>().maxMana);
		//	PlayerPrefs.SetInt("PlayerMP", player.GetComponent<Status>().mana);
		PlayerPrefs.SetInt("PlayerMaxShield" +saveSlot.ToString(), player.GetComponent<Status>().maxShield);
		PlayerPrefs.SetInt("PlayerSTP" +saveSlot.ToString(), player.GetComponent<Status>().statusPoint);
		PlayerPrefs.SetInt("PlayerSTK" +saveSlot.ToString(), player.GetComponent<Status>().skillPoint);

		PlayerPrefs.SetInt("Cash" +saveSlot.ToString(), player.GetComponent<Inventory>().cash);
		int itemSize = player.GetComponent<Inventory>().itemSlot.Length;
		int a = 0;
		if(itemSize > 0){
			while(a < itemSize){
				PlayerPrefs.SetInt("Item" + a.ToString() +saveSlot.ToString(), player.GetComponent<Inventory>().itemSlot[a]);
				PlayerPrefs.SetInt("ItemQty" + a.ToString() +saveSlot.ToString(), player.GetComponent<Inventory>().itemQuantity[a]);
				a++;
			}
		}
		
		int equipSize = player.GetComponent<Inventory>().equipment.Length;
		a = 0;
		if(equipSize > 0){
			while(a < equipSize){
				PlayerPrefs.SetInt("Equipm" + a.ToString() +saveSlot.ToString(), player.GetComponent<Inventory>().equipment[a]);
				PlayerPrefs.SetInt("EquipAmmo" + a.ToString() +saveSlot.ToString(), player.GetComponent<Inventory>().equipAmmo[a]);
				a++;
			}
		}
		PlayerPrefs.SetInt("PrimaryEquip" +saveSlot.ToString(), player.GetComponent<Inventory>().primaryEquip);
		PlayerPrefs.SetInt("SecondaryEquip" +saveSlot.ToString(), player.GetComponent<Inventory>().secondaryEquip);
		PlayerPrefs.SetInt("MeleeEquip" +saveSlot.ToString(), player.GetComponent<Inventory>().meleeEquip);
		PlayerPrefs.SetInt("ArmoEquip" +saveSlot.ToString(), player.GetComponent<Inventory>().armorEquip);
		
		PlayerPrefs.SetInt("PrimaryAmmo" +saveSlot.ToString(), player.GetComponent<GunTrigger>().primaryWeapon.ammo);
		PlayerPrefs.SetInt("SecondaryAmmo" +saveSlot.ToString(), player.GetComponent<GunTrigger>().secondaryWeapon.ammo);
		//Save Skill Slot
		a = 0;
		while(a < player.GetComponent<SkillWindow>().skill.Length){
			PlayerPrefs.SetInt("Skill" + a.ToString() +saveSlot.ToString(), player.GetComponent<SkillWindow>().skill[a]);
			a++;
		}
		//Skill List Slot
		a = 0;
		while(a < player.GetComponent<SkillWindow>().skillListSlot.Length){
			PlayerPrefs.SetInt("SkillList" + a.ToString() +saveSlot.ToString(), player.GetComponent<SkillWindow>().skillListSlot[a]);
			a++;
		}
		
		//Save Ammo
		PlayerPrefs.SetInt("HandgunAmmo" +saveSlot.ToString(), player.GetComponent<GunTrigger>().allAmmo.handgunAmmo);
		PlayerPrefs.SetInt("MachinegunAmmo" +saveSlot.ToString(), player.GetComponent<GunTrigger>().allAmmo.machinegunAmmo);
		PlayerPrefs.SetInt("ShotgunAmmo" +saveSlot.ToString(), player.GetComponent<GunTrigger>().allAmmo.shotgunAmmo);
		PlayerPrefs.SetInt("MagnumAmmo" +saveSlot.ToString(), player.GetComponent<GunTrigger>().allAmmo.magnumAmmo);
		PlayerPrefs.SetInt("SmgAmmo" +saveSlot.ToString(), player.GetComponent<GunTrigger>().allAmmo.smgAmmo);
		PlayerPrefs.SetInt("SniperRifleAmmo" +saveSlot.ToString(), player.GetComponent<GunTrigger>().allAmmo.sniperRifleAmmo);
		PlayerPrefs.SetInt("GrenadeRounds" +saveSlot.ToString(), player.GetComponent<GunTrigger>().allAmmo.grenadeRounds);
	}
	
	public void CalculateStatus(){
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
			//recoverShield = 0.0ff;
		}
		
		int hpPer = maxHealth * addHPpercent / 100;
		int mpPer = maxMana * addMPpercent / 100;
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
	IEnumerator OnPoison (int hurtTime){
		GameObject eff = null;
		int amount = 0;
		if(poison){
			yield break;
		}
		int chance= 100;
		chance -= statusResist.poisonResist;
		if(chance > 0){
			int per = Random.Range(0, 100);
			if(per <= chance){
				poison = true;
				amount = maxHealth * 5 / 100; // Hurt 5% of Max HP
			}else{
				yield break;
			}
			
		}else{
			yield break;
		}
		//--------------------
		while(poison && hurtTime > 0){
			if(poisonEffect){ //Show Poison Effect
				eff = Instantiate(poisonEffect, transform.position, poisonEffect.transform.rotation) as GameObject;
				eff.transform.parent = transform;
			}
			yield return new WaitForSeconds(0.7f); // Reduce HP  Every 0.7f Seconds
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
	
	
	IEnumerator OnSilence(float dur){
		GameObject eff = null;
		if(silence){
			yield break;;
		}
		int chance = 100;
		chance -= statusResist.silenceResist;
		if(chance > 0){
			int per = Random.Range(0, 100);
			if(per <= chance){
				silence = true;
				if(silenceEffect){ //Show Poison Effect
					eff = Instantiate(silenceEffect, transform.position, transform.rotation) as GameObject;
					eff.transform.parent = transform;
				}
			}else{
				yield break;
			}
			
		}else{
			yield break;
		}
		yield return new WaitForSeconds(dur);
		if(eff){ //Destroy Effect if it still on a map
			Destroy(eff.gameObject);
		}
		silence = false;
		
	}
	
	IEnumerator OnWebbedUp(float dur){
		GameObject eff = null;
		if(web){
			yield break;
		}
		int chance = 100;
		chance -= statusResist.webResist;
		if(chance > 0){
			int per = Random.Range(0, 100);
			if(per <= chance){
				web = true;
				freeze = true; // Freeze Character On (Character cannot do anything)
				if(webbedUpEffect){ //Show Poison Effect
					eff = Instantiate(webbedUpEffect, transform.position, transform.rotation) as GameObject;
					eff.transform.parent = transform;
				}
				if(webbedUpAnimation){// If you Assign the Animation then play it
					if(useMecanim){
						GetComponent<PlayerMecanimAnimation>().PlayAnim(webbedUpAnimation.name);
					}else{
						mainModel.GetComponent<Animation>()[webbedUpAnimation.name].layer = 25;
						mainModel.GetComponent<Animation>().Play(webbedUpAnimation.name);
					}
				}
			}else{
				yield break;
			}
			
		}else{
			yield break;
		}
		yield return new WaitForSeconds(dur);
		if(eff){ //Destroy Effect if it still on a map
			Destroy(eff.gameObject);
		}
		if(webbedUpAnimation && !useMecanim){// If you Assign the Animation then stop playing
			mainModel.GetComponent<Animation>().Stop(webbedUpAnimation.name);
		}
		freeze = false; // Freeze Character Off
		web = false;
		
	}
	
	IEnumerator OnStun(float dur){
		GameObject eff = null;
		if(stun){
			yield break;
		}
		int chance = 100;
		chance -= statusResist.stunResist;
		if(chance > 0){
			int per = Random.Range(0, 100);
			if(per <= chance){
				stun = true;
				freeze = true; // Freeze Character On (Character cannot do anything)
				if(stunEffect){ //Show Stun Effect
					eff = Instantiate(stunEffect, transform.position, stunEffect.transform.rotation) as GameObject;
					eff.transform.parent = transform;
				}
				if(stunAnimation){// If you Assign the Animation then play it
					if(useMecanim){
						GetComponent<PlayerMecanimAnimation>().PlayAnim(stunAnimation.name);
					}else{
						mainModel.GetComponent<Animation>()[stunAnimation.name].layer = 25;
						mainModel.GetComponent<Animation>().Play(stunAnimation.name);
					}
				}
			}else{
				yield break;
			}
			
		}else{
			yield break;
		}
		yield return new WaitForSeconds(dur);
		if(eff){ //Destroy Effect if it still on a map
			Destroy(eff.gameObject);
		}
		if(stunAnimation && !useMecanim){// If you Assign the Animation then stop playing
			mainModel.GetComponent<Animation>().Stop(stunAnimation.name);
		}
		freeze = false; // Freeze Character Off
		stun = false;
		
	}
	
	public void ApplyAbnormalStat ( int statId  ,   float dur  ){
		if(statId == 0){
			//OnPoison(Mathf.FloorToInt(dur));
			StartCoroutine(OnPoison(Mathf.FloorToInt(dur)));
		}
		if(statId == 1){
			//OnSilence(dur);
			StartCoroutine(OnSilence(dur));
		}
		if(statId == 2){
			//OnStun(dur);
			StartCoroutine(OnStun(dur));
		}
		if(statId == 3){
			//OnWebbedUp(dur);
			StartCoroutine(OnWebbedUp(dur));
		}
		
		
	}
	
	
	IEnumerator OnBarrier (int amount , float dur){
		//Increase Defense
		if(barrier){
			yield break;;
		}
		barrier = true;
		buffDef = 0;
		buffDef += amount;
		CalculateStatus();
		yield return new WaitForSeconds(dur);
		buffDef = 0;
		barrier = false;
		CalculateStatus();
	}
	
	IEnumerator OnMagicBarrier(int amount , float dur){
		//Increase Magic Defense
		if(mbarrier){
			yield break;
		}
		mbarrier = true;
		buffMdef = 0;
		buffMdef += amount;
		CalculateStatus();
		yield return new WaitForSeconds(dur);
		buffMdef = 0;
		mbarrier = false;
		CalculateStatus();
		
	}
	
	IEnumerator OnBrave (int amount , float dur){
		//Increase Attack
		if(brave){
			yield break;
		}
		brave = true;
		buffMelee = 0;
		buffMelee += amount;
		CalculateStatus();
		yield return new WaitForSeconds(dur);
		buffMelee = 0;
		brave = false;
		CalculateStatus();
		
	}
	
	IEnumerator OnSharp (int amount , float dur){
		//Increase Attack
		if(sharp){
			yield break;
		}
		sharp = true;
		buffAtk = 0;
		buffAtk += amount;
		CalculateStatus();
		yield return new WaitForSeconds(dur);
		buffAtk = 0;
		sharp = false;
		CalculateStatus();
		
	}
	
	IEnumerator OnFaith (int amount , float dur){
		//Increase Magic Attack
		if(faith){
			yield break;
		}
		faith = true;
		buffMatk = 0;
		buffMatk += amount;
		CalculateStatus();
		yield return new WaitForSeconds(dur);
		buffMatk = 0;
		faith = false;
		CalculateStatus();
		
	}
	
	public void ApplyBuff (int statId , float dur, int amount){
		if(statId == 1){
			//Increase Defense
			//OnBarrier(amount , dur);
			StartCoroutine(OnBarrier(amount , dur));
		}
		if(statId == 2){
			//Increase Magic Defense
			//OnMagicBarrier(amount , dur);
			StartCoroutine(OnMagicBarrier(amount , dur));
		}
		if(statId == 3){
			//Increase Melee Attack
			//OnBrave(amount , dur);
			StartCoroutine(OnBrave(amount , dur));
		}
		if(statId == 4){
			//Increase Magic Attack
			//OnFaith(amount , dur);
			StartCoroutine(OnFaith(amount , dur));
		}
		if(statId == 5){
			//Increase Gun Attack
			//OnSharp(amount , dur);
			StartCoroutine(OnSharp(amount , dur));
		}
		
	}
	
	public void Flinch(){
		if(stability){
			return;
		}
		//KnockBack();
		StartCoroutine(KnockBack());
		if(hurt && !useMecanim && mainModel){
			//For Legacy Animation
			mainModel.GetComponent<Animation>()[hurt.name].layer = 4;
			mainModel.GetComponent<Animation>().PlayQueued(hurt.name, QueueMode.PlayNow);
		}
	}
	
	public IEnumerator KnockBack (){
		flinch = true;
		yield return new WaitForSeconds(0.2f);
		flinch = false;
	}
}

[System.Serializable]
public class elem{
	public string elementName = "";
	public int effective = 100;
}
[System.Serializable]
public class resist{
	public int poisonResist = 0;
	public int silenceResist = 0;
	public int webResist = 0;
	public int stunResist = 0;
}
