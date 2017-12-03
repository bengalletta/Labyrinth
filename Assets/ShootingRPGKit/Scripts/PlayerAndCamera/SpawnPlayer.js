#pragma strict
var playerPrefab : GameObject[] = new GameObject[4];
var spawnId : int = 0;
private var mainCam : Transform;
var autoLoadTempData : boolean = false; //Auto load character data from Temporary Save Data.

var characterDatabase : GameObject;
private var charId : int = 0;
private var player : CharacterData;

function Awake () {
	if(characterDatabase)
		player = characterDatabase.GetComponent(CharacterData);

		//Check for Current Player in the scene
		var currentPlayer : GameObject = GameObject.FindWithTag("Player");
		if(currentPlayer){
			// If there are the player in the scene already. Check for the Spawn Point Name
			// If it match then Move Player to the SpawnpointPosition
			var spawnPointName : String = currentPlayer.GetComponent(Status).spawnPointName;
			var spawnPoint : GameObject = GameObject.Find(spawnPointName);
			if(spawnPoint){
				currentPlayer.transform.position = spawnPoint.transform.position;
				currentPlayer.transform.eulerAngles = spawnPoint.transform.eulerAngles;
			}else{
				currentPlayer.transform.position = transform.position;
				currentPlayer.transform.eulerAngles = transform.eulerAngles;
			}
			var oldCam : GameObject = currentPlayer.GetComponent(GunTrigger).mainCam;
			if(!oldCam){
				return;
			}
			var cam : GameObject[] = GameObject.FindGameObjectsWithTag("MainCamera"); 
    		for (var cam2 : GameObject in cam) { 
  			 	if(cam2 != oldCam){
  			 		Destroy(cam2.gameObject);
  			 	}
  			 }
  			 
  			 if(currentPlayer.GetComponent(SpawnPartner)){
				currentPlayer.GetComponent(SpawnPartner).MoveToMaster();
			}
			// If there are the player in the scene already. We will not spawn the new player.
			return;
		}
		//Spawn Player
		if(autoLoadTempData && characterDatabase){
			LoadTempData();
		}else{
			var spawnPlayer : GameObject = Instantiate(playerPrefab[spawnId], transform.position , transform.rotation);
		}
}

function LoadTempData(){
	var saveSlot : int = -1; //-1 is a Temp ID
	//var saveSlot : int = PlayerPrefs.GetInt("SaveSlot");
	charId = PlayerPrefs.GetInt("PlayerID" +saveSlot.ToString());
	//var player : CharacterData = characterDatabase.GetComponent(CharacterData);
	
		var respawn : GameObject = Instantiate(player.player[charId].playerPrefab, transform.position , transform.rotation);
		//yield WaitForSeconds(0.2);
		respawn.GetComponent(Status).characterName = PlayerPrefs.GetString("Name" +saveSlot.ToString());
		respawn.GetComponent(Status).level = PlayerPrefs.GetInt("PlayerLevel" +saveSlot.ToString());
		respawn.GetComponent(Status).playerId = PlayerPrefs.GetInt("PlayerID" +saveSlot.ToString());
		respawn.GetComponent(Status).atk = PlayerPrefs.GetInt("PlayerATK" +saveSlot.ToString());
		respawn.GetComponent(Status).def = PlayerPrefs.GetInt("PlayerDEF" +saveSlot.ToString());
		respawn.GetComponent(Status).matk = PlayerPrefs.GetInt("PlayerMATK" +saveSlot.ToString());
		respawn.GetComponent(Status).mdef = PlayerPrefs.GetInt("PlayerMDEF" +saveSlot.ToString());
		respawn.GetComponent(Status).mdef = PlayerPrefs.GetInt("PlayerMDEF" +saveSlot.ToString());
		respawn.GetComponent(Status).exp = PlayerPrefs.GetInt("PlayerEXP" +saveSlot.ToString());
		respawn.GetComponent(Status).maxExp = PlayerPrefs.GetInt("PlayerMaxEXP" +saveSlot.ToString());
		respawn.GetComponent(Status).maxHealth = PlayerPrefs.GetInt("PlayerMaxHP" +saveSlot.ToString());
		respawn.GetComponent(Status).health = PlayerPrefs.GetInt("PlayerHP" +saveSlot.ToString());
		//respawn.GetComponent(Status).health = PlayerPrefs.GetInt("PlayerMaxHP");
		respawn.GetComponent(Status).maxMana = PlayerPrefs.GetInt("PlayerMaxMP" +saveSlot.ToString());
		respawn.GetComponent(Status).mana = PlayerPrefs.GetInt("PlayerMaxMP" +saveSlot.ToString());
		respawn.GetComponent(Status).statusPoint = PlayerPrefs.GetInt("PlayerSTP" +saveSlot.ToString());
		respawn.GetComponent(Status).maxShield = PlayerPrefs.GetInt("PlayerMaxShield" +saveSlot.ToString());
		respawn.GetComponent(Status).melee = PlayerPrefs.GetInt("PlayerMelee" +saveSlot.ToString());
		
		//-------------------------------
		respawn.GetComponent(Inventory).cash = PlayerPrefs.GetInt("Cash" +saveSlot.ToString());
		var itemSize : int = respawn.GetComponent(Inventory).itemSlot.length;
			var a : int = 0;
			if(itemSize > 0){
				while(a < itemSize){
					respawn.GetComponent(Inventory).itemSlot[a] = PlayerPrefs.GetInt("Item" + a.ToString() +saveSlot.ToString());
					respawn.GetComponent(Inventory).itemQuantity[a] = PlayerPrefs.GetInt("ItemQty" + a.ToString() +saveSlot.ToString());
					//-------
					a++;
				}
			}
			
			var equipSize : int = respawn.GetComponent(Inventory).equipment.length;
			a = 0;
			if(equipSize > 0){
				while(a < equipSize){
					respawn.GetComponent(Inventory).equipment[a] = PlayerPrefs.GetInt("Equipm" + a.ToString() +saveSlot.ToString());
					respawn.GetComponent(Inventory).equipAmmo[a] = PlayerPrefs.GetInt("EquipAmmo" + a.ToString() +saveSlot.ToString());
					a++;
				}
			}
			respawn.GetComponent(Inventory).primaryEquip = 0;
			//respawn.GetComponent(Inventory).primaryEquip = PlayerPrefs.GetInt("PrimaryEquip" +saveSlot.ToString());
			respawn.GetComponent(Inventory).secondaryEquip = 0;
			respawn.GetComponent(Inventory).meleeEquip = 0;
			respawn.GetComponent(Inventory).armorEquip = PlayerPrefs.GetInt("ArmoEquip" +saveSlot.ToString());
		
		respawn.GetComponent(Inventory).cancelAssign = true;
		if(PlayerPrefs.GetInt("PrimaryEquip" +saveSlot.ToString()) > 0){
			respawn.GetComponent(Inventory).EquipItem(PlayerPrefs.GetInt("PrimaryEquip" +saveSlot.ToString()) , respawn.GetComponent(Inventory).equipment.Length + 5);
			respawn.GetComponent(GunTrigger).primaryWeapon.ammo = PlayerPrefs.GetInt("PrimaryAmmo" +saveSlot.ToString());
			
		}
		if(PlayerPrefs.GetInt("SecondaryEquip" +saveSlot.ToString()) > 0){
			respawn.GetComponent(Inventory).EquipItem(PlayerPrefs.GetInt("SecondaryEquip" +saveSlot.ToString()) , respawn.GetComponent(Inventory).equipment.Length + 5);
			respawn.GetComponent(GunTrigger).secondaryWeapon.ammo = PlayerPrefs.GetInt("SecondaryAmmo" +saveSlot.ToString());
		}
		if(PlayerPrefs.GetInt("MeleeEquip" +saveSlot.ToString()) > 0){
			respawn.GetComponent(Inventory).EquipItem(PlayerPrefs.GetInt("MeleeEquip" +saveSlot.ToString()) , respawn.GetComponent(Inventory).equipment.Length + 5);
		}
			respawn.GetComponent(Inventory).RemoveWeaponMesh();
			//----------------------------------
		//Screen.lockCursor = true;
		
		 var mon : GameObject[]; 
  		 mon = GameObject.FindGameObjectsWithTag("Enemy"); 
  			 for (var mo : GameObject in mon) { 
  			 	if(mo){
  			 		mo.GetComponent(AIenemy).followTarget = respawn.transform;
  			 	}
  			 }
			
			//Load Skill Slot
			a = 0;
			while(a < respawn.GetComponent(SkillWindow).skill.length){
				respawn.GetComponent(SkillWindow).skill[a] = PlayerPrefs.GetInt("Skill" + a.ToString() +saveSlot.ToString());
				a++;
			}
			//Skill List Slot
			a = 0;
			while(a < respawn.GetComponent(SkillWindow).skillListSlot.length){
				respawn.GetComponent(SkillWindow).skillListSlot[a] = PlayerPrefs.GetInt("SkillList" + a.ToString() +saveSlot.ToString());
				a++;
			}
			respawn.GetComponent(SkillWindow).AssignAllSkill();
		//---------------Set Target to Minimap--------------
//  		var minimap : GameObject = GameObject.FindWithTag("Minimap");
//  		if(minimap){
//  			var mapcam : GameObject = minimap.GetComponent(MinimapOnOff).minimapCam;
//  			mapcam.GetComponent(MinimapCamera).target = respawn.transform;
//  		}
  		//Load Ammo
		respawn.GetComponent(GunTrigger).allAmmo.handgunAmmo = PlayerPrefs.GetInt("HandgunAmmo" +saveSlot.ToString());
		respawn.GetComponent(GunTrigger).allAmmo.machinegunAmmo = PlayerPrefs.GetInt("MachinegunAmmo" +saveSlot.ToString());
		respawn.GetComponent(GunTrigger).allAmmo.shotgunAmmo = PlayerPrefs.GetInt("ShotgunAmmo" +saveSlot.ToString());
		respawn.GetComponent(GunTrigger).allAmmo.magnumAmmo = PlayerPrefs.GetInt("MagnumAmmo" +saveSlot.ToString());
		respawn.GetComponent(GunTrigger).allAmmo.smgAmmo = PlayerPrefs.GetInt("SmgAmmo" +saveSlot.ToString());
		respawn.GetComponent(GunTrigger).allAmmo.sniperRifleAmmo = PlayerPrefs.GetInt("SniperRifleAmmo" +saveSlot.ToString());
		respawn.GetComponent(GunTrigger).allAmmo.grenadeRounds = PlayerPrefs.GetInt("GrenadeRounds" +saveSlot.ToString());
}
