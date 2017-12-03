#pragma strict
var equipmentId : int = 1;
var ammo : int = 30;
private var master : Transform;
private var database : GameObject;
private var dataItem : ItemData;
var forceSwap : boolean = true; 

function Start () {
	master = transform.root;
}

function OnTriggerEnter (other : Collider) {
	//Pick up Item
	if (other.gameObject.tag == "Player") {
		Equip(other.gameObject);
    }
    
}

function Equip(other : GameObject){
	database = other.GetComponent(Inventory).database;
	dataItem = database.GetComponent(ItemData);
	var gun : GunTrigger = other.GetComponent(GunTrigger);
	var inven : Inventory = other.GetComponent(Inventory);
	var tempEquipment : int = 0;
	var tempAmmo : int = 0;
		
	if(dataItem.equipment[equipmentId].equipmentType == EqType.PrimaryWeapon){
		tempEquipment = inven.primaryEquip;
		tempAmmo = gun.primaryWeapon.ammo;
		
		inven.EquipItem(equipmentId , inven.equipment.Length +5);
		gun.primaryWeapon.ammo = ammo;
		if(forceSwap && gun.weaponEquip != 0){
			gun.ForceSwap();
		}
		
	}else if(dataItem.equipment[equipmentId].equipmentType == EqType.SecondaryWeapon){
		tempEquipment = inven.secondaryEquip;
		tempAmmo = gun.secondaryWeapon.ammo;
		
		inven.EquipItem(equipmentId , inven.equipment.Length +5);
		gun.secondaryWeapon.ammo = ammo;
		if(forceSwap && gun.weaponEquip != 1){
			gun.ForceSwap();
		}
		
	}else if(dataItem.equipment[equipmentId].equipmentType == EqType.MeleeWeapon){
		tempEquipment = inven.meleeEquip;
		inven.EquipItem(equipmentId , inven.equipment.Length +5);
		
	}else{
		tempEquipment = inven.armorEquip;
		inven.EquipItem(equipmentId , inven.equipment.Length +5);
	}
	inven.AddEquipment(tempEquipment , tempAmmo);
	
    master = transform.root;
    Destroy(master.gameObject);
}
