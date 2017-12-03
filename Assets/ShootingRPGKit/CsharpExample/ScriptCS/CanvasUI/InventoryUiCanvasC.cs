using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryUiCanvasC : MonoBehaviour {
	public GameObject player;
	
	public Text moneyText;
	
	public Image[] itemIcons = new Image[16];
	public Image[] equipmentIcons = new Image[16];
	
	public Image weaponIcons;
	public Image weapon2Icons;
	public Image armorIcons;
	public Image meleeIcons;
	
	public GameObject tooltip;
	public Image tooltipIcon;
	public Text tooltipName;
	public Text tooltipText1;

	public GameObject usableTab;
	public GameObject equipmentTab;
	
	public GameObject database;
	private ItemData db; 
	
	void Start(){
		db = database.GetComponent<ItemData>();
	}
	
	void Update(){
		if(tooltip && tooltip.activeSelf == true){
			Vector2 tooltipPos = Input.mousePosition;
			tooltipPos.x += 7;
			tooltip.transform.position = tooltipPos;
		}
		if(!player){
			return;
		}
		//itemIcons[0].GetComponent<Image>().sprite = db.usableItem[player.GetComponent<Inventory>().itemSlot[0]].iconSprite;
		
		for(int a = 0; a < itemIcons.Length; a++){
			itemIcons[a].GetComponent<Image>().sprite = db.usableItem[player.GetComponent<Inventory>().itemSlot[a]].iconSprite;
		}

		for(int b = 0; b < equipmentIcons.Length; b++){
			equipmentIcons[b].GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().equipment[b]].iconSprite;
		}
		
		if(weaponIcons){
			weaponIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().primaryEquip].iconSprite;
		}
		if(weapon2Icons){
			weapon2Icons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().secondaryEquip].iconSprite;
		}
		if(armorIcons){
			armorIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().armorEquip].iconSprite;
		}
		if(meleeIcons){
			meleeIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<Inventory>().meleeEquip].iconSprite;
		}
		if(moneyText){
			moneyText.GetComponent<Text>().text = player.GetComponent<Inventory>().cash.ToString();
		}
	}
	
	public void ShowItemTooltip(int slot){
		if(!tooltip || !player){
			return;
		}
		if(player.GetComponent<Inventory>().itemSlot[slot] <= 0){
			HideTooltip();
			return;
		}
		
		tooltipIcon.GetComponent<Image>().sprite = db.usableItem[player.GetComponent<Inventory>().itemSlot[slot]].iconSprite;
		tooltipName.GetComponent<Text>().text = db.usableItem[player.GetComponent<Inventory>().itemSlot[slot]].itemName + "  x" + player.GetComponent<Inventory>().itemQuantity[slot].ToString();
		
		tooltipText1.GetComponent<Text>().text = db.usableItem[player.GetComponent<Inventory>().itemSlot[slot]].description;
		tooltip.SetActive(true);
	}
	
	public void ShowEquipmentTooltip(int slot){
		if(!tooltip || !player){
			return;
		}
		if(player.GetComponent<Inventory>().equipment[slot] <= 0){
			HideTooltip();
			return;
		}
		Inventory inv = player.GetComponent<Inventory>();
		tooltipIcon.GetComponent<Image>().sprite = db.equipment[inv.equipment[slot]].iconSprite;

		if(db.equipment[inv.equipment[slot]].equipmentType == EqType.PrimaryWeapon || db.equipment[inv.equipment[slot]].equipmentType == EqType.SecondaryWeapon){
			tooltipName.GetComponent<Text>().text = db.equipment[inv.equipment[slot]].itemName + " (" + inv.equipAmmo[slot].ToString() + "/" + db.equipment[inv.equipment[slot]].maxAmmo.ToString() + ")";
		}else{
			tooltipName.GetComponent<Text>().text = db.equipment[inv.equipment[slot]].itemName;
		}
		
		tooltipText1.GetComponent<Text>().text = db.equipment[inv.equipment[slot]].description;

		tooltip.SetActive(true);
	}
	
	public void ShowOnEquipTooltip(int type){
		if(!tooltip || !player){
			return;
		}
		//0 = Weapon, 1 = Weapon2, 2 = Armor, 3 = Melee
		int id = 0;
		if(type == 0){
			id = player.GetComponent<Inventory>().primaryEquip;
		}
		if(type == 1){
			id = player.GetComponent<Inventory>().secondaryEquip;
		}
		if(type == 2){
			id = player.GetComponent<Inventory>().armorEquip;
		}
		if(type == 3){
			id = player.GetComponent<Inventory>().meleeEquip;
		}
		
		if(id <= 0){
			HideTooltip();
			return;
		}
		
		tooltipIcon.GetComponent<Image>().sprite = db.equipment[id].iconSprite;

		if(type == 0){
			tooltipName.GetComponent<Text>().text = db.equipment[id].itemName + " (" + player.GetComponent<GunTrigger>().primaryWeapon.ammo.ToString() + "/" + db.equipment[id].maxAmmo.ToString() + ")";
		}else if(type == 1){
			tooltipName.GetComponent<Text>().text = db.equipment[id].itemName + " (" + player.GetComponent<GunTrigger>().secondaryWeapon.ammo.ToString() + "/" + db.equipment[id].maxAmmo.ToString() + ")";
		}else{
			tooltipName.GetComponent<Text>().text = db.equipment[id].itemName;
		}
		
		tooltipText1.GetComponent<Text>().text = db.equipment[id].description;

		tooltip.SetActive(true);
	}
	
	public void HideTooltip(){
		if(!tooltip){
			return;
		}
		tooltip.SetActive(false);
	}
	
	public void UseItem(int itemSlot){
		if(!player){
			return;
		}
		player.GetComponent<Inventory>().UseItem(itemSlot);
		ShowItemTooltip(itemSlot);
		
	}
	
	public void EquipItem(int itemSlot){
		if(!player){
			return;
		}
		player.GetComponent<Inventory>().EquipItem(player.GetComponent<Inventory>().equipment[itemSlot] , itemSlot);
		ShowEquipmentTooltip(itemSlot);
	}
	
	public void UnEquip(int type){
		//0 = Weapon, 1 = Weapon2, 2 = Armor, 3 = Melee
		if(!player){
			return;
		}
		int id = 0;
		if(type == 0){
			id = player.GetComponent<Inventory>().primaryEquip;
		}
		if(type == 1){
			id = player.GetComponent<Inventory>().secondaryEquip;
		}
		if(type == 2){
			id = player.GetComponent<Inventory>().armorEquip;
		}
		if(type == 3){
			id = player.GetComponent<Inventory>().meleeEquip;
		}
		player.GetComponent<Inventory>().UnEquip(id);
		ShowOnEquipTooltip(type);
	}
	
	public void CloseMenu(){
		Time.timeScale = 1.0f;
		//Screen.lockCursor = true;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		gameObject.SetActive(false);
	}
	
	public void OpenUsableTab(){
		usableTab.SetActive(true);
		equipmentTab.SetActive(false);
	}
	
	public void OpenEquipmentTab(){
		usableTab.SetActive(false);
		equipmentTab.SetActive(true);
	}
	
	
}