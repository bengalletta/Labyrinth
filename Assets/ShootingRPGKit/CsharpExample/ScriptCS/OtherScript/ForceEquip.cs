using UnityEngine;
using System.Collections;

public class ForceEquip : MonoBehaviour {
	public int equipmentId = 1;
	public int ammo = 30;
	private Transform master;
	private GameObject database;
	private ItemData dataItem;
	public bool  forceSwap = true; 
	
	void Start(){
		master = transform.root;
	}
	
	void OnTriggerEnter(Collider other){
		//Pick up Item
		if (other.gameObject.tag == "Player") {
			Equip(other.gameObject);
		}
		
	}
	
	void  Equip ( GameObject other  ){
		database = other.GetComponent<Inventory>().database;
		dataItem = database.GetComponent<ItemData>();
		GunTrigger gun = other.GetComponent<GunTrigger>();
		Inventory inven = other.GetComponent<Inventory>();
		int tempEquipment = 0;
		int tempAmmo = 0;
		
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
	
}