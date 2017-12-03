using UnityEngine;
using System.Collections;

public class GameOverC : MonoBehaviour {
	
	public string returnToScene = "Base";
	private bool show  = false;
	public GameObject characterDatabase;
	private int charId = 0;
	private CharacterData player;
	private string goScene = "Base";
	
	void Start(){
		Screen.lockCursor = false;
		if(characterDatabase){
			player = characterDatabase.GetComponent<CharacterData>();
		}
		StartCoroutine(Delay());
	}

	IEnumerator Delay(){
		yield return new WaitForSeconds(3.5f);
		show = true;
	}
	
	void OnGUI(){
		if(show){
			if (GUI.Button ( new Rect(Screen.width /2 -110 , Screen.height /2 - 40,220,60), "Retry")) {
				if(player){
					goScene = Application.loadedLevelName;
					LoadTempData();
				}else{
					Application.LoadLevel(Application.loadedLevel);
				}
			}
			if (GUI.Button ( new Rect(Screen.width /2 -110 , Screen.height /2 + 30,220,60), "Abort Mission")) {
				if(player){
					goScene = returnToScene;
					LoadTempData();
				}else{
					Application.LoadLevel(returnToScene);
				}
			}
		}
	}
	
	void LoadTempData(){
		DontDestroyOnLoad(transform.gameObject);
		int saveSlot = -1; //-1 is a Temp ID
		charId = PlayerPrefs.GetInt("PlayerID" +saveSlot.ToString());
		//CharacterData player = characterDatabase.GetComponent<CharacterData>();
		
		GameObject respawn = Instantiate(player.player[charId].playerPrefab, transform.position , transform.rotation) as GameObject;
		//yield return new WaitForSeconds(0.2f);
		respawn.GetComponent<Status>().immortal = true; // Make Character Immortal until load all data.
		respawn.GetComponent<Status>().characterName = PlayerPrefs.GetString("Name" +saveSlot.ToString());
		respawn.GetComponent<Status>().level = PlayerPrefs.GetInt("PlayerLevel" +saveSlot.ToString());
		respawn.GetComponent<Status>().playerId = PlayerPrefs.GetInt("PlayerID" +saveSlot.ToString());
		respawn.GetComponent<Status>().atk = PlayerPrefs.GetInt("PlayerATK" +saveSlot.ToString());
		respawn.GetComponent<Status>().def = PlayerPrefs.GetInt("PlayerDEF" +saveSlot.ToString());
		respawn.GetComponent<Status>().matk = PlayerPrefs.GetInt("PlayerMATK" +saveSlot.ToString());
		respawn.GetComponent<Status>().mdef = PlayerPrefs.GetInt("PlayerMDEF" +saveSlot.ToString());
		respawn.GetComponent<Status>().mdef = PlayerPrefs.GetInt("PlayerMDEF" +saveSlot.ToString());
		respawn.GetComponent<Status>().exp = PlayerPrefs.GetInt("PlayerEXP" +saveSlot.ToString());
		respawn.GetComponent<Status>().maxExp = PlayerPrefs.GetInt("PlayerMaxEXP" +saveSlot.ToString());
		respawn.GetComponent<Status>().maxHealth = PlayerPrefs.GetInt("PlayerMaxHP" +saveSlot.ToString());
		respawn.GetComponent<Status>().health = PlayerPrefs.GetInt("PlayerHP" +saveSlot.ToString());
		//respawn.GetComponent<Status>().health = PlayerPrefs.GetInt("PlayerMaxHP");
		respawn.GetComponent<Status>().maxMana = PlayerPrefs.GetInt("PlayerMaxMP" +saveSlot.ToString());
		respawn.GetComponent<Status>().mana = PlayerPrefs.GetInt("PlayerMaxMP" +saveSlot.ToString());
		respawn.GetComponent<Status>().statusPoint = PlayerPrefs.GetInt("PlayerSTP" +saveSlot.ToString());
		respawn.GetComponent<Status>().skillPoint = PlayerPrefs.GetInt("PlayerSTP" +saveSlot.ToString());
		
		respawn.GetComponent<Status>().maxShield = PlayerPrefs.GetInt("PlayerMaxShield" +saveSlot.ToString());
		respawn.GetComponent<Status>().melee = PlayerPrefs.GetInt("PlayerMelee" +saveSlot.ToString());
		
		//-------------------------------
		respawn.GetComponent<Inventory>().cash = PlayerPrefs.GetInt("Cash" +saveSlot.ToString());
		int itemSize = respawn.GetComponent<Inventory>().itemSlot.Length;
		int a = 0;
		if(itemSize > 0){
			while(a < itemSize){
				respawn.GetComponent<Inventory>().itemSlot[a] = PlayerPrefs.GetInt("Item" + a.ToString() +saveSlot.ToString());
				respawn.GetComponent<Inventory>().itemQuantity[a] = PlayerPrefs.GetInt("ItemQty" + a.ToString() +saveSlot.ToString());
				//-------
				a++;
			}
		}
		
		int equipSize = respawn.GetComponent<Inventory>().equipment.Length;
		a = 0;
		if(equipSize > 0){
			while(a < equipSize){
				respawn.GetComponent<Inventory>().equipment[a] = PlayerPrefs.GetInt("Equipm" + a.ToString() +saveSlot.ToString());
				respawn.GetComponent<Inventory>().equipAmmo[a] = PlayerPrefs.GetInt("EquipAmmo" + a.ToString() +saveSlot.ToString());
				a++;
			}
		}
		respawn.GetComponent<Inventory>().primaryEquip = 0;
		//respawn.GetComponent<Inventory>().primaryEquip = PlayerPrefs.GetInt("PrimaryEquip" +saveSlot.ToString());
		respawn.GetComponent<Inventory>().secondaryEquip = 0;
		respawn.GetComponent<Inventory>().meleeEquip = 0;
		respawn.GetComponent<Inventory>().armorEquip = PlayerPrefs.GetInt("ArmoEquip" +saveSlot.ToString());
		
		respawn.GetComponent<Inventory>().cancelAssign = true;
		if(PlayerPrefs.GetInt("PrimaryEquip" +saveSlot.ToString()) > 0){
			respawn.GetComponent<Inventory>().EquipItem(PlayerPrefs.GetInt("PrimaryEquip" +saveSlot.ToString()) , respawn.GetComponent<Inventory>().equipment.Length + 5);
			respawn.GetComponent<GunTrigger>().primaryWeapon.ammo = PlayerPrefs.GetInt("PrimaryAmmo" +saveSlot.ToString());
			
		}
		if(PlayerPrefs.GetInt("SecondaryEquip" +saveSlot.ToString()) > 0){
			respawn.GetComponent<Inventory>().EquipItem(PlayerPrefs.GetInt("SecondaryEquip" +saveSlot.ToString()) , respawn.GetComponent<Inventory>().equipment.Length + 5);
			respawn.GetComponent<GunTrigger>().secondaryWeapon.ammo = PlayerPrefs.GetInt("SecondaryAmmo" +saveSlot.ToString());
		}
		if(PlayerPrefs.GetInt("MeleeEquip" +saveSlot.ToString()) > 0){
			respawn.GetComponent<Inventory>().EquipItem(PlayerPrefs.GetInt("MeleeEquip" +saveSlot.ToString()) , respawn.GetComponent<Inventory>().equipment.Length + 5);
		}
		respawn.GetComponent<Inventory>().RemoveWeaponMesh();
		//----------------------------------
		//Screen.lockCursor = true;
		
		GameObject[] mon; 
		mon = GameObject.FindGameObjectsWithTag("Enemy"); 
		foreach(GameObject mo in mon) { 
			if(mo){
				mo.GetComponent<AIenemy>().followTarget = respawn.transform;
			}
		}
		
		//Load Skill Slot
		a = 0;
		while(a < respawn.GetComponent<SkillWindow>().skill.Length){
			respawn.GetComponent<SkillWindow>().skill[a] = PlayerPrefs.GetInt("Skill" + a.ToString() +saveSlot.ToString());
			a++;
		}
		//Skill List Slot
		a = 0;
		while(a < respawn.GetComponent<SkillWindow>().skillListSlot.Length){
			respawn.GetComponent<SkillWindow>().skillListSlot[a] = PlayerPrefs.GetInt("SkillList" + a.ToString() +saveSlot.ToString());
			a++;
		}
		respawn.GetComponent<SkillWindow>().AssignAllSkill();
		//---------------Set Target to Minimap--------------
		GameObject minimap = GameObject.FindWithTag("Minimap");
		if(minimap){
			GameObject mapcam = minimap.GetComponent<MinimapOnOff>().minimapCam;
			mapcam.GetComponent<MinimapCamera>().target = respawn.transform;
		}
		//Load Ammo
		respawn.GetComponent<GunTrigger>().allAmmo.handgunAmmo = PlayerPrefs.GetInt("HandgunAmmo" +saveSlot.ToString());
		respawn.GetComponent<GunTrigger>().allAmmo.machinegunAmmo = PlayerPrefs.GetInt("MachinegunAmmo" +saveSlot.ToString());
		respawn.GetComponent<GunTrigger>().allAmmo.shotgunAmmo = PlayerPrefs.GetInt("ShotgunAmmo" +saveSlot.ToString());
		respawn.GetComponent<GunTrigger>().allAmmo.magnumAmmo = PlayerPrefs.GetInt("MagnumAmmo" +saveSlot.ToString());
		respawn.GetComponent<GunTrigger>().allAmmo.smgAmmo = PlayerPrefs.GetInt("SmgAmmo" +saveSlot.ToString());
		respawn.GetComponent<GunTrigger>().allAmmo.sniperRifleAmmo = PlayerPrefs.GetInt("SniperRifleAmmo" +saveSlot.ToString());
		respawn.GetComponent<GunTrigger>().allAmmo.grenadeRounds = PlayerPrefs.GetInt("GrenadeRounds" +saveSlot.ToString());
		
		Application.LoadLevel(goScene);
		StartCoroutine(DelayUnImmortal(respawn));
	}

	IEnumerator DelayUnImmortal(GameObject respawn){
		yield return new WaitForSeconds(0.1f);
		respawn.GetComponent<Status>().immortal = false;
		Destroy(gameObject);
	}
}