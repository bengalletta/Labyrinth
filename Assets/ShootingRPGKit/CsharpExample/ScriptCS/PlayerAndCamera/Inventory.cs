using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour {
	private bool menu = false;
	private bool itemMenu = true;
	private bool equipMenu = false;
	
	public bool autoAssignWeapon = false; // Use for Auto Assign Weapon Equip to Gun Trigger.
	public int[] itemSlot = new int[18];
	public int[] itemQuantity = new int[18];
	public int[] equipment = new int[15];
	public int[] equipAmmo = new int[15];
	
	public int primaryEquip = 0;
	public int secondaryEquip = 0;
	public int meleeEquip = 0;
	public int armorEquip = 0;
	
	public GameObject database;

	public int cash = 500;
	
	public GUISkin skin;
	public Rect windowRect = new Rect (360 ,140 ,480 ,550);
	private Rect originalRect;

	public int itemPageMultiply = 6;
	public int equipmentPageMultiply = 5;
	private int page = 0;
	
	public GUIStyle itemNameText;
	public GUIStyle itemDescriptionText;
	public GUIStyle itemQuantityText;
	private GunTrigger gun;
	private ItemData dataItem;
	[HideInInspector]
	public bool cancelAssign = false;
	public bool useLegacyUi = false;
	
	void Start(){
		dataItem = database.GetComponent<ItemData>();
		gun = GetComponent<GunTrigger>();
		originalRect = windowRect;
		SetEquipmentStatus();
		if(autoAssignWeapon){
			//yield return new WaitForSeconds(0.2f);
			if(cancelAssign){
				return;
			}
			int e = primaryEquip;
			if(e > 0){
				EquipItem(e , equipment.Length +5);
				gun.primaryWeapon.ammo = dataItem.equipment[e].maxAmmo;
			}
			
			e = secondaryEquip;
			if(e > 0){
				EquipItem(e , equipment.Length +5);
				gun.secondaryWeapon.ammo = dataItem.equipment[e].maxAmmo;
			}
			
			e = meleeEquip;
			if(e > 0){
				EquipItem(e , equipment.Length +5);
			}
		}
	}
	
	public void SetEquipmentStatus (){
		//Reset Power of Current Weapon & Armor
		GetComponent<Status>().weaponAtk = 0;
		GetComponent<Status>().weaponMatk = 0;
		GetComponent<Status>().weaponAtk2 = 0;
		GetComponent<Status>().armorDef = 0;
		GetComponent<Status>().armorMdef = 0;
		GetComponent<Status>().weaponMelee = 0;
		GetComponent<Status>().addHPpercent = 0;
		GetComponent<Status>().addMPpercent = 0;
		GetComponent<Status>().armorShield = 0;
		//Set New Variable of Primary Weapon
		GetComponent<Status>().weaponAtk += dataItem.equipment[primaryEquip].attack;
		GetComponent<Status>().armorDef += dataItem.equipment[primaryEquip].defense;
		GetComponent<Status>().weaponMatk += dataItem.equipment[primaryEquip].magicAttack;
		GetComponent<Status>().armorMdef += dataItem.equipment[primaryEquip].magicDefense;
		GetComponent<Status>().weaponMelee += dataItem.equipment[primaryEquip].meleeDamage;
		GetComponent<Status>().armorShield += dataItem.equipment[primaryEquip].shieldPlus;
		//Set New Variable of Secondary Weapon
		GetComponent<Status>().weaponAtk2 += dataItem.equipment[secondaryEquip].attack;
		GetComponent<Status>().armorDef += dataItem.equipment[secondaryEquip].defense;
		GetComponent<Status>().weaponMatk += dataItem.equipment[secondaryEquip].magicAttack;
		GetComponent<Status>().armorMdef += dataItem.equipment[secondaryEquip].magicDefense;
		GetComponent<Status>().weaponMelee += dataItem.equipment[secondaryEquip].meleeDamage;
		GetComponent<Status>().armorShield += dataItem.equipment[secondaryEquip].shieldPlus;
		//Set New Variable of Melee Weapon
		GetComponent<Status>().armorDef += dataItem.equipment[meleeEquip].defense;
		GetComponent<Status>().weaponMatk += dataItem.equipment[meleeEquip].magicAttack;
		GetComponent<Status>().armorMdef += dataItem.equipment[meleeEquip].magicDefense;
		GetComponent<Status>().weaponMelee += dataItem.equipment[meleeEquip].meleeDamage;
		GetComponent<Status>().armorShield += dataItem.equipment[meleeEquip].shieldPlus;
		//Set New Variable of Armor
		GetComponent<Status>().weaponAtk += dataItem.equipment[armorEquip].attack;
		GetComponent<Status>().armorDef += dataItem.equipment[armorEquip].defense;
		GetComponent<Status>().weaponMatk += dataItem.equipment[armorEquip].magicAttack;
		GetComponent<Status>().armorMdef += dataItem.equipment[armorEquip].magicDefense;
		GetComponent<Status>().weaponMelee += dataItem.equipment[armorEquip].meleeDamage;
		GetComponent<Status>().armorShield += dataItem.equipment[armorEquip].shieldPlus;
		
		GetComponent<Status>().CalculateStatus();
	}
	
	void Update (){
		if(useLegacyUi){
			if(Input.GetKeyDown("i") || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
				OnOffMenu();
			}
		}
	}
	
	public void UseItem(int slot){
		int id = itemSlot[slot];
		GetComponent<Status>().Heal(dataItem.usableItem[id].hpRecover , dataItem.usableItem[id].mpRecover);
		GetComponent<Status>().ShieldRecovery(dataItem.usableItem[id].shieldRecover);
		itemQuantity[slot]--;
		if(itemQuantity[slot] <= 0){
			itemSlot[slot] = 0;
			itemQuantity[slot] = 0;
		}
		AutoSortItem();
		
	}

	public void EquipItem(int id , int slot){
		if(id == 0){
			return;
		}
		dataItem = database.GetComponent<ItemData>();
		gun = GetComponent<GunTrigger>();
		//Backup Your Current Equipment before Unequip
		int tempEquipment = 0;
		int tempAmmo = 0;
		
		if(dataItem.equipment[id].equipmentType == EqType.PrimaryWeapon){
			//Primary Weapon
			tempEquipment = primaryEquip;
			tempAmmo = gun.primaryWeapon.ammo;
			primaryEquip = id;
			if(slot <= equipment.Length){
				gun.primaryWeapon.ammo = equipAmmo[slot];
			}
			if(dataItem.equipment[id].attackPrefab){
				gun.primaryWeapon.bulletPrefab = dataItem.equipment[id].attackPrefab;
			}
			//Change Weapon Mesh
			if(dataItem.equipment[id].model && gun.weaponPosition){
				//gun.primaryWeaponModel.SetActive(true);
				GameObject wea = Instantiate(dataItem.equipment[id].model,gun.weaponPosition.position,gun.weaponPosition.rotation) as GameObject;
				wea.transform.parent = gun.weaponPosition;
				
				//Set new Attack Point.
				if(wea.GetComponent<WeaponAttackPoint>()){
					//Get Attack Point from WeaponAttackPoint of new weapon's model
					gun.primaryWeapon.weaponAtkPoint = wea.GetComponent<WeaponAttackPoint>().attackPoint;
				}else{
					GameObject n = new GameObject();
					n.transform.parent = wea.transform;
					gun.primaryWeapon.weaponAtkPoint = n.transform;
				}
				if(gun.primaryWeaponModel)
					Destroy(gun.primaryWeaponModel.gameObject);
				gun.primaryWeaponModel = wea;
			}
		}else if(dataItem.equipment[id].equipmentType == EqType.SecondaryWeapon){
			//Secondary Weapon
			if(dataItem.equipment[id].attackPrefab){
				gun.secondaryWeapon.bulletPrefab = dataItem.equipment[id].attackPrefab;
			}
			tempEquipment = secondaryEquip;
			tempAmmo = gun.secondaryWeapon.ammo;
			secondaryEquip = id;
			if(slot <= equipment.Length){
				gun.secondaryWeapon.ammo = equipAmmo[slot];
			}
			//Change Weapon Mesh
			if(dataItem.equipment[id].model && gun.weaponPosition){
				//gun.secondaryWeaponModel.SetActive(true);
				GameObject wea = Instantiate(dataItem.equipment[id].model,gun.weaponPosition.position,gun.weaponPosition.rotation) as GameObject;
				wea.transform.parent = gun.weaponPosition;
				
				//Set new Attack Point.
				if(wea.GetComponent<WeaponAttackPoint>()){
					//Get Attack Point from WeaponAttackPoint of new weapon's model
					gun.secondaryWeapon.weaponAtkPoint = wea.GetComponent<WeaponAttackPoint>().attackPoint;
				}else{
					GameObject n = new GameObject();
					n.transform.parent = wea.transform;
					gun.secondaryWeapon.weaponAtkPoint = n.transform;
				}
				
				if(gun.secondaryWeaponModel)
					Destroy(gun.secondaryWeaponModel.gameObject);
				gun.secondaryWeaponModel = wea;
			}
		}else if(dataItem.equipment[id].equipmentType == EqType.MeleeWeapon){
			//Melee Weapon
			if(dataItem.equipment[id].attackPrefab){
				gun.meleeAttack.meleePrefab = dataItem.equipment[id].attackPrefab;
			}
			tempEquipment = meleeEquip;
			meleeEquip = id;
			//Change Weapon Mesh
			if(dataItem.equipment[id].model && gun.weaponPosition){
				GameObject wea = Instantiate(dataItem.equipment[id].model,gun.weaponPosition.transform.position,gun.weaponPosition.transform.rotation) as GameObject;
				wea.transform.parent = gun.weaponPosition;
				if(gun.meleeWeaponModel)
					Destroy(gun.meleeWeaponModel.gameObject);
				gun.meleeWeaponModel = wea;
				gun.meleeWeaponModel.SetActive(false);
			}
		}else{
			//Armor
			tempEquipment = armorEquip;
			armorEquip = id;
		}
		
		if(slot <= equipment.Length){
			equipment[slot] = 0;
			equipAmmo[slot] = 0;
			AddEquipment(tempEquipment , tempAmmo);
		}
		//Assign Weapon Value from Database to GunTrigger
		SetWeaponValue(id);
		//Assign Weapon Animation to PlayerLegacyAnimation Script
		AssignWeaponAnimation(id);
		//Reset Power of Current Weapon & Armor
		SetEquipmentStatus();
		GetComponent<GunTrigger>().SettingWeapon();
		
		AutoSortEquipment();
		//AddEquipment(tempEquipment , tempAmmo);
		
	}
	
	public void UnEquip (int id){
		bool full = false;
		if(dataItem.equipment[id].equipmentType == EqType.PrimaryWeapon){
			full = AddEquipment(primaryEquip , gun.primaryWeapon.ammo);
			//-----------------------------------
		}else if(dataItem.equipment[id].equipmentType == EqType.SecondaryWeapon){
			full = AddEquipment(secondaryEquip , gun.secondaryWeapon.ammo);
			//-----------------------------------
		}else if(dataItem.equipment[id].equipmentType == EqType.MeleeWeapon){
			full = AddEquipment(meleeEquip , 0);
			//-----------------------------------
		}else{
			full = AddEquipment(armorEquip , 0);
		}
		if(!full){
			if(dataItem.equipment[id].equipmentType == EqType.PrimaryWeapon){
				primaryEquip = 0;
				gun.primaryWeapon.bulletPrefab = null;
				if(gun.primaryWeaponModel){
					gun.primaryWeaponModel.SetActive(false);
				}
				//------------------------
			}else if(dataItem.equipment[id].equipmentType == EqType.SecondaryWeapon){
				secondaryEquip = 0;
				gun.secondaryWeapon.bulletPrefab = null;
				if(gun.secondaryWeaponModel){
					gun.secondaryWeaponModel.SetActive(false);
				}
				//--------------------
			}else if(dataItem.equipment[id].equipmentType == EqType.MeleeWeapon){
				meleeEquip = 0;
				gun.meleeAttack.meleePrefab = null;
				if(gun.meleeWeaponModel){
					gun.meleeWeaponModel.SetActive(false);
				}
				//--------------------
			}else{
				armorEquip = 0;
			}
			SetEquipmentStatus();
		}
	}
	
	void OnGUI(){
		GUI.skin = skin;
		if(menu && itemMenu){
			windowRect = GUI.Window (1, windowRect, ItemWindow, "Items");
		}
		if(menu && equipMenu){
			windowRect = GUI.Window (1, windowRect, ItemWindow, "Equipment");
		}
		
		if(menu){
			if (GUI.Button ( new Rect(windowRect.x -50, windowRect.y +105,50,100), "Item")) {
				//Switch to Item Tab
				page = 0;
				itemMenu = true;
				equipMenu = false;
			}
			if (GUI.Button ( new Rect(windowRect.x -50, windowRect.y +225,50,100), "Equip")) {
				//Switch to Equipment Tab
				page = 0;
				equipMenu = true;
				itemMenu = false;	
			}
		}
	}
	
	//-----------Item Window-------------
	void ItemWindow (int windowID){
		if(menu && itemMenu){
			//GUI.Box ( new Rect(260,140,280,385), "Items");
			//Close Window Button
			if (GUI.Button ( new Rect(windowRect.width -50 ,8,40,40), "X")) {
				OnOffMenu();
			}
			//Items Slot
			if (GUI.Button ( new Rect(30,30,60,60),dataItem.usableItem[itemSlot[0 + page]].icon)){
				UseItem(0 + page);
			}
			GUI.Label ( new Rect(125, 40, 320, 75), dataItem.usableItem[itemSlot[0 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 65, 320, 75), dataItem.usableItem[itemSlot[0 + page]].description.ToString() , itemDescriptionText); //Item Description
			if(itemQuantity[0 + page] > 0){
				GUI.Label ( new Rect(73, 73, 40, 30), itemQuantity[0 + page].ToString() , itemQuantityText); //Quantity
			}
			//------------------------------------------------------
			if (GUI.Button ( new Rect(30,105,60,60),dataItem.usableItem[itemSlot[1 + page]].icon)){
				UseItem(1 + page);
			}
			GUI.Label ( new Rect(125, 115, 320, 75), dataItem.usableItem[itemSlot[1 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 140, 320, 75), dataItem.usableItem[itemSlot[1 + page]].description.ToString() , itemDescriptionText); //Item Description
			if(itemQuantity[1 + page] > 0){
				GUI.Label ( new Rect(73, 148, 40, 30), itemQuantity[1 + page].ToString() , itemQuantityText); //Quantity
			}
			//------------------------------------------------------
			if (GUI.Button ( new Rect(30,180,60,60),dataItem.usableItem[itemSlot[2 + page]].icon)){
				UseItem(2 + page);
			}
			GUI.Label ( new Rect(125, 190, 320, 75), dataItem.usableItem[itemSlot[2 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 215, 320, 75), dataItem.usableItem[itemSlot[2 + page]].description.ToString() , itemDescriptionText); //Item Description
			if(itemQuantity[2 + page] > 0){
				GUI.Label ( new Rect(73, 223, 40, 30), itemQuantity[2 + page].ToString() , itemQuantityText); //Quantity
			}
			//------------------------------------------------------
			if (GUI.Button ( new Rect(30,255,60,60),dataItem.usableItem[itemSlot[3 + page]].icon)){
				UseItem(3 + page);
			}
			GUI.Label ( new Rect(125, 265, 320, 75), dataItem.usableItem[itemSlot[3 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 290, 320, 75), dataItem.usableItem[itemSlot[3 + page]].description.ToString() , itemDescriptionText); //Item Description
			if(itemQuantity[3 + page] > 0){
				GUI.Label ( new Rect(73, 298, 40, 30), itemQuantity[3 + page].ToString() , itemQuantityText); //Quantity
			}
			//------------------------------------------------------
			if (GUI.Button ( new Rect(30,330,60,60),dataItem.usableItem[itemSlot[4 + page]].icon)){
				UseItem(4 + page);
			}
			GUI.Label ( new Rect(125, 340, 320, 75), dataItem.usableItem[itemSlot[4 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 365, 320, 75), dataItem.usableItem[itemSlot[4 + page]].description.ToString() , itemDescriptionText); //Item Description
			if(itemQuantity[4 + page] > 0){
				GUI.Label ( new Rect(73, 373, 40, 30), itemQuantity[4 + page].ToString() , itemQuantityText); //Quantity
			}
			//------------------------------------------------------
			if (GUI.Button ( new Rect(30,405,60,60),dataItem.usableItem[itemSlot[5 + page]].icon)){
				UseItem(5 + page);
			}
			GUI.Label ( new Rect(125, 415, 320, 75), dataItem.usableItem[itemSlot[5 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 440, 320, 75), dataItem.usableItem[itemSlot[5 + page]].description.ToString() , itemDescriptionText); //Item Description
			if(itemQuantity[5 + page] > 0){
				GUI.Label ( new Rect(73, 448, 40, 30), itemQuantity[5 + page].ToString() , itemQuantityText); //Quantity
			}
			//------------------------------------------------------
			
			if (GUI.Button ( new Rect(220,485,50,52), "1")) {
				page = 0;
			}
			if (GUI.Button ( new Rect(290,485,50,52), "2")) {
				page = itemPageMultiply;
			}
			if (GUI.Button ( new Rect(360,485,50,52), "3")) {
				page = itemPageMultiply *2;
			}
			
			GUI.Label ( new Rect(20, 505, 150, 50), "$ " + cash.ToString() , itemDescriptionText);
			//---------------------------
		}
		
		//---------------Equipment Tab----------------------------
		if(menu && equipMenu){
			//Close Window Button
			if (GUI.Button ( new Rect(windowRect.width -50 ,8,40,40), "X")) {
				OnOffMenu();
			}
			//Primary Weapon
			GUI.Label ( new Rect(60, 95, 150, 50), "Primary");			
			if (GUI.Button ( new Rect(50,20,70,70), dataItem.equipment[primaryEquip].icon)){
				if(primaryEquip == 0){
					return;
				}
				UnEquip(primaryEquip);
			}
			//Secondary Weapon
			GUI.Label ( new Rect(145, 95, 150, 50), "Secondary");			
			if (GUI.Button ( new Rect(140,20,70,70), dataItem.equipment[secondaryEquip].icon)){
				if(secondaryEquip == 0){
					return;
				}
				UnEquip(secondaryEquip);
			}
			//Melee Weapon
			GUI.Label ( new Rect(245, 95, 150, 50), "Melee");			
			if (GUI.Button ( new Rect(230,20,70,70), dataItem.equipment[meleeEquip].icon)){
				if(meleeEquip == 0){
					return;
				}
				UnEquip(meleeEquip);
			}
			//Armor
			GUI.Label ( new Rect(330, 95, 150, 50), "Armor");
			if (GUI.Button ( new Rect(320,20,70,70), dataItem.equipment[armorEquip].icon)){
				if(armorEquip == 0){
					return;
				}
				UnEquip(armorEquip);
				
			}
			//--------Equipment Slot---------
			if (GUI.Button ( new Rect(30,130,60,60),dataItem.equipment[equipment[0 + page]].icon)){
				EquipItem(equipment[0 + page] , 0 + page);
			}
			GUI.Label ( new Rect(125, 140, 320, 75), dataItem.equipment[equipment[0 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 165, 320, 75), dataItem.equipment[equipment[0 + page]].description.ToString() , itemDescriptionText); //Item Description
			
			if(equipment[0 + page] > 0)
				GUI.Label ( new Rect(33, 180, 60, 30),"Ammo: " + equipAmmo[0 + page].ToString() , itemQuantityText); //Ammo
			//------------------------------------------------------
			if (GUI.Button ( new Rect(30,205,60,60),dataItem.equipment[equipment[1 + page]].icon)){
				EquipItem(equipment[1 + page] , 1 + page);
			}
			GUI.Label ( new Rect(125, 215, 320, 75), dataItem.equipment[equipment[1 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 240, 320, 75), dataItem.equipment[equipment[1 + page]].description.ToString() , itemDescriptionText); //Item Description
			
			if(equipment[1 + page] > 0)
				GUI.Label ( new Rect(33, 255, 60, 30),"Ammo: " + equipAmmo[1 + page].ToString() , itemQuantityText); //Ammo
			//------------------------------------------------------
			if (GUI.Button ( new Rect(30,280,60,60),dataItem.equipment[equipment[2 + page]].icon)){
				EquipItem(equipment[2 + page] , 2 + page);
			}
			GUI.Label ( new Rect(125, 290, 320, 75), dataItem.equipment[equipment[2 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 315, 320, 75), dataItem.equipment[equipment[2 + page]].description.ToString() , itemDescriptionText); //Item Description
			
			if(equipment[2 + page] > 0)
				GUI.Label ( new Rect(33, 330, 60, 30),"Ammo: " + equipAmmo[2 + page].ToString() , itemQuantityText); //Ammo
			//------------------------------------------------------
			if (GUI.Button ( new Rect(30,355,60,60),dataItem.equipment[equipment[3 + page]].icon)){
				EquipItem(equipment[3 + page] , 3 + page);
			}
			GUI.Label ( new Rect(125, 365, 320, 75), dataItem.equipment[equipment[3 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 390, 320, 75), dataItem.equipment[equipment[3 + page]].description.ToString() , itemDescriptionText); //Item Description
			
			if(equipment[3 + page] > 0)
				GUI.Label ( new Rect(33, 405, 60, 30),"Ammo: " + equipAmmo[3 + page].ToString() , itemQuantityText); //Ammo
			//------------------------------------------------------
			if (GUI.Button ( new Rect(30,430,60,60),dataItem.equipment[equipment[4 + page]].icon)){
				EquipItem(equipment[4 + page] , 4 + page);
			}
			GUI.Label ( new Rect(125, 440, 320, 75), dataItem.equipment[equipment[4 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 465, 320, 75), dataItem.equipment[equipment[4 + page]].description.ToString() , itemDescriptionText); //Item Description
			
			if(equipment[4 + page] > 0)
				GUI.Label ( new Rect(33, 480, 60, 30),"Ammo: " + equipAmmo[4 + page].ToString() , itemQuantityText); //Ammo
			//------------------------------------------------------
			
			if (GUI.Button ( new Rect(220,485,50,52), "1")) {
				page = 0;
			}
			if (GUI.Button ( new Rect(290,485,50,52), "2")) {
				page = equipmentPageMultiply;
			}
			if (GUI.Button ( new Rect(360,485,50,52), "3")) {
				page = equipmentPageMultiply *2;
			}
			
			GUI.Label ( new Rect(20, 505, 150, 50), "$ " + cash.ToString() , itemDescriptionText);
			
		}
		GUI.DragWindow (new Rect (0,0,10000,10000)); 
	}
	
	public bool AddItem(int id , int quan){
		bool  full = false;
		bool  geta = false;
		
		int pt = 0;
		while(pt < itemSlot.Length && !geta){
			if(itemSlot[pt] == id){
				itemQuantity[pt] += quan;
				geta = true;
			}else if(itemSlot[pt] == 0){
				itemSlot[pt] = id;
				itemQuantity[pt] = quan;
				geta = true;
			}else{
				pt++;
				if(pt >= itemSlot.Length){
					full = true;
					print("Full");
				}
			}
			
		}
		
		return full;
		
	}
	
	public bool AddEquipment(int id , int ammo1){
		bool  full = false;
		bool  geta = false;
		
		
		int pt = 0;
		while(pt < equipment.Length && !geta){
			if(equipment[pt] == 0){
				equipment[pt] = id;
				equipAmmo[pt] = ammo1;
				geta = true;
			}else{
				pt++;
				if(pt >= equipment.Length){
					full = true;
					print("Full");
				}
			}
			
		}
		
		return full;
		
	}
	//------------AutoSort----------
	public void AutoSortItem(){
		int pt = 0;
		int nextp = 0;
		bool  clearr = false;
		while(pt < itemSlot.Length){
			if(itemSlot[pt] == 0){
				nextp = pt + 1;
				while(nextp < itemSlot.Length && !clearr){
					if(itemSlot[nextp] > 0){
						//Fine Next Item and Set
						itemSlot[pt] = itemSlot[nextp];
						itemQuantity[pt] = itemQuantity[nextp];
						itemSlot[nextp] = 0;
						itemQuantity[nextp] = 0;
						clearr = true;
					}else{
						nextp++;
					}
					
				}
				//Continue New Loop
				clearr = false;
				pt++;
			}else{
				pt++;
			}
			
		}
		
	}
	
	public void AutoSortEquipment(){
		int pt = 0;
		int nextp = 0;
		bool  clearr = false;
		while(pt < equipment.Length){
			if(equipment[pt] == 0){
				nextp = pt + 1;
				while(nextp < equipment.Length && !clearr){
					if(equipment[nextp] > 0){
						//Fine Next Item and Set
						equipment[pt] = equipment[nextp];
						equipAmmo[pt] = equipAmmo[nextp];
						equipment[nextp] = 0;
						equipAmmo[nextp] = 0;
						clearr = true;
					}else{
						nextp++;
					}
					
				}
				//Continue New Loop
				clearr = false;
				pt++;
			}else{
				pt++;
			}
			
		}
		
	}
	
	
	void OnOffMenu(){
		//Freeze Time Scale to 0 if Window is Showing
		if(!menu && Time.timeScale != 0.0f){
			menu = true;
			Time.timeScale = 0.0f;
			ResetPosition();
			Screen.lockCursor = false;
		}else if(menu){
			menu = false;
			Time.timeScale = 1.0f;
			Screen.lockCursor = true;
		}
		
	}
	
	public void SetWeaponValue(int id){
		int equipType = (int)dataItem.equipment[id].equipmentType;
		StopCoroutine("Reload");
		if(equipType == 0){ // Primary Weapon
			GetComponent<GunTrigger>().primaryWeapon.attackDelay = dataItem.equipment[id].attackDelay;
			GetComponent<GunTrigger>().primaryWeapon.maxAmmo = dataItem.equipment[id].maxAmmo;
			GetComponent<GunTrigger>().primaryWeapon.useAmmo = dataItem.equipment[id].useAmmo;
			GetComponent<GunTrigger>().primaryWeapon.gunFireEffect = dataItem.equipment[id].gunFireEffect;
			GetComponent<GunTrigger>().primaryWeapon.cameraShakeValue = dataItem.equipment[id].cameraShakeValue;
			GetComponent<GunTrigger>().primaryWeapon.shootAnimationSpeed = dataItem.equipment[id].attackAnimationSpeed;
			GetComponent<GunTrigger>().primaryWeapon.gunSound = dataItem.equipment[id].gunSound;
			GetComponent<GunTrigger>().primaryWeapon.reloadSound = dataItem.equipment[id].reloadSound;
			GetComponent<GunTrigger>().primaryWeapon.soundRadius = dataItem.equipment[id].soundRadius;
			
			GetComponent<GunTrigger>().primaryWeapon.automatic = dataItem.equipment[id].automatic;
			GetComponent<GunTrigger>().primaryWeapon.animateWhileMoving = dataItem.equipment[id].animateWhileMoving;
			if(dataItem.equipment[id].aimIcon){
				GetComponent<GunTrigger>().primaryWeapon.aimIcon = dataItem.equipment[id].aimIcon;
			}
			GetComponent<GunTrigger>().primaryWeapon.zoomIcon = dataItem.equipment[id].zoomIcon;
			GetComponent<GunTrigger>().primaryWeapon.zoomLevel = dataItem.equipment[id].zoomLevel;
		}
		if(equipType == 1){ // Secondary Weapon
			GetComponent<GunTrigger>().secondaryWeapon.attackDelay = dataItem.equipment[id].attackDelay;
			GetComponent<GunTrigger>().secondaryWeapon.maxAmmo = dataItem.equipment[id].maxAmmo;
			GetComponent<GunTrigger>().secondaryWeapon.useAmmo = dataItem.equipment[id].useAmmo;
			GetComponent<GunTrigger>().secondaryWeapon.gunFireEffect = dataItem.equipment[id].gunFireEffect;
			GetComponent<GunTrigger>().secondaryWeapon.cameraShakeValue = dataItem.equipment[id].cameraShakeValue;
			GetComponent<GunTrigger>().secondaryWeapon.shootAnimationSpeed = dataItem.equipment[id].attackAnimationSpeed;
			GetComponent<GunTrigger>().secondaryWeapon.gunSound = dataItem.equipment[id].gunSound;
			GetComponent<GunTrigger>().secondaryWeapon.reloadSound = dataItem.equipment[id].reloadSound;
			GetComponent<GunTrigger>().secondaryWeapon.soundRadius = dataItem.equipment[id].soundRadius;
			
			GetComponent<GunTrigger>().secondaryWeapon.automatic = dataItem.equipment[id].automatic;
			GetComponent<GunTrigger>().secondaryWeapon.animateWhileMoving = dataItem.equipment[id].animateWhileMoving;
			if(dataItem.equipment[id].aimIcon){
				GetComponent<GunTrigger>().secondaryWeapon.aimIcon = dataItem.equipment[id].aimIcon;
			}
			GetComponent<GunTrigger>().secondaryWeapon.zoomIcon = dataItem.equipment[id].zoomIcon;
			GetComponent<GunTrigger>().secondaryWeapon.zoomLevel = dataItem.equipment[id].zoomLevel;
		}
		if(equipType == 2){ // Melee Weapon
			GetComponent<GunTrigger>().meleeAttack.meleeAnimationSpeed = dataItem.equipment[id].attackAnimationSpeed;
			GetComponent<GunTrigger>().meleeAttack.meleeCast = dataItem.equipment[id].attackCast;
			GetComponent<GunTrigger>().meleeAttack.meleeDelay = dataItem.equipment[id].attackDelay;
			GetComponent<GunTrigger>().meleeAttack.meleeSound = dataItem.equipment[id].gunSound;
		}
		GetComponent<GunTrigger>().SettingWeapon();
	}
	
	public void AssignWeaponAnimation(int id){
		int equipType = (int)dataItem.equipment[id].equipmentType;
		
		//Assign Attack Animation for Primary Weapon
		if(equipType == 0){
			GetComponent<GunTrigger>().primaryWeapon.shootAnimation = dataItem.equipment[id].attackAnimation[0];
			
			if(dataItem.equipment[id].reloadAnimation){
				GetComponent<GunTrigger>().primaryWeapon.reloadAnimation = dataItem.equipment[id].reloadAnimation;
			}
			if(dataItem.equipment[id].equipAnimation){
				GetComponent<GunTrigger>().primaryWeapon.equipAnimation = dataItem.equipment[id].equipAnimation;
			}
		}
		
		//Assign Attack Animation for Secondary Weapon
		if(equipType == 1){
			GetComponent<GunTrigger>().secondaryWeapon.shootAnimation = dataItem.equipment[id].attackAnimation[0];
			
			if(dataItem.equipment[id].reloadAnimation){
				GetComponent<GunTrigger>().secondaryWeapon.reloadAnimation = dataItem.equipment[id].reloadAnimation;
			}
			if(dataItem.equipment[id].equipAnimation){
				GetComponent<GunTrigger>().secondaryWeapon.equipAnimation = dataItem.equipment[id].equipAnimation;
			}
		}
		
		//--------------------------------------------------------
		
		//Assign All Attack Combo Animation for Melee
		if(equipType == 2 && dataItem.equipment[id].attackAnimation.Length > 0){
			int allPrefab = dataItem.equipment[id].attackAnimation.Length;
			GetComponent<GunTrigger>().meleeAttack.meleeAnimation = new AnimationClip[allPrefab];
			GetComponent<GunTrigger>().c = 0;
			
			int a = 0;
			if(allPrefab > 0){
				while(a < allPrefab){
					GetComponent<GunTrigger>().meleeAttack.meleeAnimation[a] = dataItem.equipment[id].attackAnimation[a];
					a++;
				}
			}
			
		}
		
		PlayerLegacyAnimation playerAnim = GetComponent<PlayerLegacyAnimation>();
		if(!playerAnim){
			//If use Mecanim
			AssignMecanimAnimation(id , equipType);
			return;
		}
		//--------------------------------------------------------
		//equipType , 0 = Primary , 1 = Secondary
		if(equipType <= 1){//Assign Idle and Move Animation if it's a Primary or Secondary Weapon
			if(dataItem.equipment[id].legacyAnimationSet.idle){
				playerAnim.weaponAnimSet[equipType].idle = dataItem.equipment[id].legacyAnimationSet.idle;
			}
			if(dataItem.equipment[id].legacyAnimationSet.run){
				playerAnim.weaponAnimSet[equipType].run = dataItem.equipment[id].legacyAnimationSet.run;
			}
			if(dataItem.equipment[id].legacyAnimationSet.right){
				playerAnim.weaponAnimSet[equipType].right = dataItem.equipment[id].legacyAnimationSet.right;
			}
			if(dataItem.equipment[id].legacyAnimationSet.left){
				playerAnim.weaponAnimSet[equipType].left = dataItem.equipment[id].legacyAnimationSet.left;
			}
			if(dataItem.equipment[id].legacyAnimationSet.back){
				playerAnim.weaponAnimSet[equipType].back = dataItem.equipment[id].legacyAnimationSet.back;
			}
			if(dataItem.equipment[id].legacyAnimationSet.jump){
				playerAnim.weaponAnimSet[equipType].jump = dataItem.equipment[id].legacyAnimationSet.jump;
			}
			if(dataItem.equipment[id].legacyAnimationSet.hurt){
				playerAnim.weaponAnimSet[equipType].hurt = dataItem.equipment[id].legacyAnimationSet.hurt;
			}
			if(dataItem.equipment[id].legacyAnimationSet.crouchIdle){
				playerAnim.weaponAnimSet[equipType].crouchIdle = dataItem.equipment[id].legacyAnimationSet.crouchIdle;
			}
			if(dataItem.equipment[id].legacyAnimationSet.crouchForward){
				playerAnim.weaponAnimSet[equipType].crouchForward = dataItem.equipment[id].legacyAnimationSet.crouchForward;
			}
			if(dataItem.equipment[id].legacyAnimationSet.crouchRight){
				playerAnim.weaponAnimSet[equipType].crouchRight = dataItem.equipment[id].legacyAnimationSet.crouchRight;
			}
			if(dataItem.equipment[id].legacyAnimationSet.crouchLeft){
				playerAnim.weaponAnimSet[equipType].crouchLeft = dataItem.equipment[id].legacyAnimationSet.crouchLeft;
			}
			if(dataItem.equipment[id].legacyAnimationSet.crouchBack){
				playerAnim.weaponAnimSet[equipType].crouchBack = dataItem.equipment[id].legacyAnimationSet.crouchBack;
			}
		}
		playerAnim.SetAnimation();
		
	}
	
	public void AssignMecanimAnimation (int id  , int type){
		//Set Weapon Type ID to Mecanim Animator and Set New Idle
		if(type == 0){
			GetComponent<PlayerMecanimAnimation>().primaryWeaponType = dataItem.equipment[id].weaponAnimationType;
		}else if(type == 1){
			GetComponent<PlayerMecanimAnimation>().secondaryWeaponType = dataItem.equipment[id].weaponAnimationType;
		}
		GetComponent<PlayerMecanimAnimation>().SetAnimation();
		GetComponent<PlayerMecanimAnimation>().SetIdle();
	}
	
	void ResetPosition(){
		//Reset GUI Position when it out of Screen.
		if(windowRect.x >= Screen.width -30 || windowRect.y >= Screen.height -30 || windowRect.x <= -70 || windowRect.y <= -70 ){
			windowRect = originalRect;
		}
	}
	
	public void RemoveWeaponMesh(){
		if(primaryEquip == 0){
			gun.primaryWeapon.bulletPrefab = null;
			if(gun.primaryWeaponModel){
				gun.primaryWeaponModel.SetActive(false);
			}
			//------------------------
		}
		if(secondaryEquip == 0){
			gun.secondaryWeapon.bulletPrefab = null;
			if(gun.secondaryWeaponModel){
				gun.secondaryWeaponModel.SetActive(false);
			}
			//--------------------
		}
		if(meleeEquip == 0){
			gun.meleeAttack.meleePrefab = null;
			if(gun.meleeWeaponModel){
				gun.meleeWeaponModel.SetActive(false);
			}
			//--------------------
		}
	}
}