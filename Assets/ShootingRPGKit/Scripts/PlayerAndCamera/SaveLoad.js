#pragma strict
var autoLoad : boolean = false;
private var player : GameObject;
private var menu : boolean = false;
private var lastPosition : Vector3;
private var mainCam : Transform;

private var saveSlot : int = 0;

function Start () {
	if(!player){
    	player = GameObject.FindWithTag ("Player");
    }
    saveSlot = PlayerPrefs.GetInt("SaveSlot");
    GetComponent(Status).characterName = PlayerPrefs.GetString("Name" +saveSlot.ToString());
    //If PlayerPrefs Loadgame = 10 That mean You Start with Load Game Menu.
	//If You Set Autoload = true It will LoadGame when you start.
     if(PlayerPrefs.GetInt("Loadgame") == 10 || autoLoad){
   		 LoadGame();
   		 if(!autoLoad){
   			 //If You didn't Set autoLoad then reset PlayerPrefs Loadgame to 0 after LoadGame.
   		 	PlayerPrefs.SetInt("Loadgame", 0);
   		 }
    }

}

function Update () {
	if (Input.GetKeyDown(KeyCode.Escape)) {
		//Open Save Load Menu
		OnOffMenu();
	}

}

function OnOffMenu(){
	//Freeze Time Scale to 0 if Window is Showing
	if(!menu && Time.timeScale != 0.0){
			menu = true;
			Time.timeScale = 0.0;
			Screen.lockCursor = false;
	}else if(menu){
			menu = false;
			Time.timeScale = 1.0;
			Screen.lockCursor = true;
	}
}

function OnGUI(){
	if(menu){
		GUI.Box (Rect (Screen.width / 2 - 180,190,360,340), "Menu");
		if (GUI.Button (Rect (Screen.width / 2 - 110,265,220,100), "Save Game")) {
			SaveData();
			OnOffMenu();
		}
		
		/*if (GUI.Button (Rect (Screen.width / 2 - 110,355,220,80), "Load Game")) {
			LoadData();
			OnOffMenu();
		}*/
		
		if (GUI.Button (Rect (Screen.width / 2 - 110,405,220,100), "Quit Game")) {
			//var cam : GameObject = GameObject.FindWithTag ("MainCamera");
			//Destroy(cam);
			Destroy(player);
			//Time.timeScale = 1.0;
			Application.LoadLevel ("MainMenu");
			OnOffMenu();
			//Application.Quit();
		}
		
		if (GUI.Button (Rect (Screen.width / 2 + 110,195,60,60), "X")) {
			OnOffMenu();
		}
	}

}


function SaveData(){
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
			PlayerPrefs.SetInt("PlayerHP" +saveSlot.ToString(), player.GetComponent(Status).health);
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
			
			print("Saved");
}


function LoadData(){
		//oldPlayer = GameObject.FindWithTag ("Player");
		var respawn : GameObject = GameObject.FindWithTag ("Player");
		
		respawn.GetComponent(Status).characterName = PlayerPrefs.GetString("Name" +saveSlot.ToString());
		lastPosition.x = PlayerPrefs.GetFloat("PlayerX");
		lastPosition.y = PlayerPrefs.GetFloat("PlayerY");
		lastPosition.z = PlayerPrefs.GetFloat("PlayerZ");
		respawn.transform.position = lastPosition;
		//var respawn : GameObject = Instantiate(player, lastPosition , transform.rotation);
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
		respawn.GetComponent(Status).skillPoint = PlayerPrefs.GetInt("PlayerSTK" +saveSlot.ToString());
		
		respawn.GetComponent(Status).maxShield = PlayerPrefs.GetInt("PlayerMaxShield" +saveSlot.ToString());
		respawn.GetComponent(Status).melee = PlayerPrefs.GetInt("PlayerMelee" +saveSlot.ToString());
		
		mainCam = GameObject.FindWithTag ("MainCamera").transform;
		//mainCam.GetComponent(ARPGcamera).target = respawn.transform;
		//-------------------------------
		respawn.GetComponent(Inventory).cash = PlayerPrefs.GetInt("Cash" +saveSlot.ToString());
		var itemSize : int = player.GetComponent(Inventory).itemSlot.length;
			var a : int = 0;
			if(itemSize > 0){
				while(a < itemSize){
					respawn.GetComponent(Inventory).itemSlot[a] = PlayerPrefs.GetInt("Item" + a.ToString() +saveSlot.ToString());
					respawn.GetComponent(Inventory).itemQuantity[a] = PlayerPrefs.GetInt("ItemQty" + a.ToString() +saveSlot.ToString());
					//-------
					a++;
				}
			}
			
			var equipSize : int = player.GetComponent(Inventory).equipment.length;
			a = 0;
			if(equipSize > 0){
				while(a < equipSize){
					respawn.GetComponent(Inventory).equipment[a] = PlayerPrefs.GetInt("Equipm" + a.ToString() +saveSlot.ToString());
					respawn.GetComponent(Inventory).equipAmmo[a] = PlayerPrefs.GetInt("EquipAmmo" + a.ToString() +saveSlot.ToString());
					a++;
				}
			}
			respawn.GetComponent(Inventory).primaryEquip = 0;
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
			while(a < player.GetComponent(SkillWindow).skill.length){
				respawn.GetComponent(SkillWindow).skill[a] = PlayerPrefs.GetInt("Skill" + a.ToString() +saveSlot.ToString());
				a++;
			}
			//Skill List Slot
			a = 0;
			while(a < player.GetComponent(SkillWindow).skillListSlot.length){
				player.GetComponent(SkillWindow).skillListSlot[a] = PlayerPrefs.GetInt("SkillList" + a.ToString() +saveSlot.ToString());
				a++;
			}
			respawn.GetComponent(SkillWindow).AssignAllSkill();
		//---------------Set Target to Minimap--------------
  		var minimap : GameObject = GameObject.FindWithTag("Minimap");
  		if(minimap){
  			var mapcam : GameObject = minimap.GetComponent(MinimapOnOff).minimapCam;
  			mapcam.GetComponent(MinimapCamera).target = respawn.transform;
  		}
  		//Load Ammo
		respawn.GetComponent(GunTrigger).allAmmo.handgunAmmo = PlayerPrefs.GetInt("HandgunAmmo" +saveSlot.ToString());
		respawn.GetComponent(GunTrigger).allAmmo.machinegunAmmo = PlayerPrefs.GetInt("MachinegunAmmo" +saveSlot.ToString());
		respawn.GetComponent(GunTrigger).allAmmo.shotgunAmmo = PlayerPrefs.GetInt("ShotgunAmmo" +saveSlot.ToString());
		respawn.GetComponent(GunTrigger).allAmmo.magnumAmmo = PlayerPrefs.GetInt("MagnumAmmo" +saveSlot.ToString());
		respawn.GetComponent(GunTrigger).allAmmo.smgAmmo = PlayerPrefs.GetInt("SmgAmmo" +saveSlot.ToString());
		respawn.GetComponent(GunTrigger).allAmmo.sniperRifleAmmo = PlayerPrefs.GetInt("SniperRifleAmmo" +saveSlot.ToString());
		respawn.GetComponent(GunTrigger).allAmmo.grenadeRounds = PlayerPrefs.GetInt("GrenadeRounds" +saveSlot.ToString());
		
		player = GameObject.FindWithTag ("Player");
}

//Function LoadGame is unlike the Function LoadData.
//This Function will not spawn new Player.
function LoadGame(){
		if(!player){
	    	player = GameObject.FindWithTag ("Player");
	    }
		player.GetComponent(Status).characterName = PlayerPrefs.GetString("Name" +saveSlot.ToString());
		player.GetComponent(Status).level = PlayerPrefs.GetInt("PlayerLevel" +saveSlot.ToString());
		player.GetComponent(Status).playerId = PlayerPrefs.GetInt("PlayerID" +saveSlot.ToString());
		player.GetComponent(Status).atk = PlayerPrefs.GetInt("PlayerATK" +saveSlot.ToString());
		player.GetComponent(Status).def = PlayerPrefs.GetInt("PlayerDEF" +saveSlot.ToString());
		player.GetComponent(Status).matk = PlayerPrefs.GetInt("PlayerMATK" +saveSlot.ToString());
		player.GetComponent(Status).mdef = PlayerPrefs.GetInt("PlayerMDEF" +saveSlot.ToString());
		player.GetComponent(Status).mdef = PlayerPrefs.GetInt("PlayerMDEF" +saveSlot.ToString());
		player.GetComponent(Status).exp = PlayerPrefs.GetInt("PlayerEXP" +saveSlot.ToString());
		player.GetComponent(Status).maxExp = PlayerPrefs.GetInt("PlayerMaxEXP" +saveSlot.ToString());
		player.GetComponent(Status).maxHealth = PlayerPrefs.GetInt("PlayerMaxHP" +saveSlot.ToString());
		player.GetComponent(Status).health = PlayerPrefs.GetInt("PlayerMaxHP" +saveSlot.ToString());
		player.GetComponent(Status).maxMana = PlayerPrefs.GetInt("PlayerMaxMP" +saveSlot.ToString());
		player.GetComponent(Status).mana = PlayerPrefs.GetInt("PlayerMaxMP" +saveSlot.ToString());	
		player.GetComponent(Status).statusPoint = PlayerPrefs.GetInt("PlayerSTP" +saveSlot.ToString());
		player.GetComponent(Status).skillPoint = PlayerPrefs.GetInt("PlayerSTK" +saveSlot.ToString());
		
		player.GetComponent(Status).maxShield = PlayerPrefs.GetInt("PlayerMaxShield" +saveSlot.ToString());
		player.GetComponent(Status).melee = PlayerPrefs.GetInt("PlayerMelee" +saveSlot.ToString());
		//-------------------------------
		player.GetComponent(Inventory).cash = PlayerPrefs.GetInt("Cash" +saveSlot.ToString());
		var itemSize : int = player.GetComponent(Inventory).itemSlot.length;
			var a : int = 0;
			if(itemSize > 0){
				while(a < itemSize){
					player.GetComponent(Inventory).itemSlot[a] = PlayerPrefs.GetInt("Item" + a.ToString() +saveSlot.ToString());
					player.GetComponent(Inventory).itemQuantity[a] = PlayerPrefs.GetInt("ItemQty" + a.ToString() +saveSlot.ToString());
					//-------
					a++;
				}
			}
			
			var equipSize : int = player.GetComponent(Inventory).equipment.length;
			a = 0;
			if(equipSize > 0){
				while(a < equipSize){
					player.GetComponent(Inventory).equipment[a] = PlayerPrefs.GetInt("Equipm" + a.ToString() +saveSlot.ToString());
					player.GetComponent(Inventory).equipAmmo[a] = PlayerPrefs.GetInt("EquipAmmo" + a.ToString() +saveSlot.ToString());
					a++;
				}
			}
			player.GetComponent(Inventory).primaryEquip = 0;
			player.GetComponent(Inventory).secondaryEquip = 0;
			player.GetComponent(Inventory).meleeEquip = 0;
			player.GetComponent(Inventory).armorEquip = PlayerPrefs.GetInt("ArmoEquip" +saveSlot.ToString());
		
		player.GetComponent(Inventory).cancelAssign = true;
		if(PlayerPrefs.GetInt("PrimaryEquip" +saveSlot.ToString()) > 0){
			player.GetComponent(Inventory).EquipItem(PlayerPrefs.GetInt("PrimaryEquip" +saveSlot.ToString()) , player.GetComponent(Inventory).equipment.Length + 5);
			player.GetComponent(GunTrigger).primaryWeapon.ammo = PlayerPrefs.GetInt("PrimaryAmmo" +saveSlot.ToString());
		}
		if(PlayerPrefs.GetInt("SecondaryEquip" +saveSlot.ToString()) > 0){
			player.GetComponent(Inventory).EquipItem(PlayerPrefs.GetInt("SecondaryEquip" +saveSlot.ToString()) , player.GetComponent(Inventory).equipment.Length + 5);
			player.GetComponent(GunTrigger).secondaryWeapon.ammo = PlayerPrefs.GetInt("SecondaryAmmo" +saveSlot.ToString());
		}
		if(PlayerPrefs.GetInt("MeleeEquip" +saveSlot.ToString()) > 0){
			player.GetComponent(Inventory).EquipItem(PlayerPrefs.GetInt("MeleeEquip" +saveSlot.ToString()) , player.GetComponent(Inventory).equipment.Length + 5);
		}
			//----------------------------------
		//Screen.lockCursor = true;
		
		 var mon : GameObject[]; 
  		 mon = GameObject.FindGameObjectsWithTag("Enemy"); 
  			 for (var mo : GameObject in mon) { 
  			 	if(mo){
  			 		mo.GetComponent(AIenemy).followTarget = player.transform;
  			 	}
  			 }
  		
			//Load Skill Slot
			a = 0;
			while(a < player.GetComponent(SkillWindow).skill.length){
				player.GetComponent(SkillWindow).skill[a] = PlayerPrefs.GetInt("Skill" + a.ToString() +saveSlot.ToString());
				a++;
			}
			//Skill List Slot
			a = 0;
			while(a < player.GetComponent(SkillWindow).skillListSlot.length){
				player.GetComponent(SkillWindow).skillListSlot[a] = PlayerPrefs.GetInt("SkillList" + a.ToString() +saveSlot.ToString());
				a++;
			}
			player.GetComponent(SkillWindow).AssignAllSkill();
			
		//Load Ammo
		player.GetComponent(GunTrigger).allAmmo.handgunAmmo = PlayerPrefs.GetInt("HandgunAmmo" +saveSlot.ToString());
		player.GetComponent(GunTrigger).allAmmo.machinegunAmmo = PlayerPrefs.GetInt("MachinegunAmmo" +saveSlot.ToString());
		player.GetComponent(GunTrigger).allAmmo.shotgunAmmo = PlayerPrefs.GetInt("ShotgunAmmo" +saveSlot.ToString());
		player.GetComponent(GunTrigger).allAmmo.magnumAmmo = PlayerPrefs.GetInt("MagnumAmmo" +saveSlot.ToString());
		player.GetComponent(GunTrigger).allAmmo.smgAmmo = PlayerPrefs.GetInt("SmgAmmo" +saveSlot.ToString());
		player.GetComponent(GunTrigger).allAmmo.sniperRifleAmmo = PlayerPrefs.GetInt("SniperRifleAmmo" +saveSlot.ToString());
		player.GetComponent(GunTrigger).allAmmo.grenadeRounds = PlayerPrefs.GetInt("GrenadeRounds" +saveSlot.ToString());

		print("Loaded");
	
}