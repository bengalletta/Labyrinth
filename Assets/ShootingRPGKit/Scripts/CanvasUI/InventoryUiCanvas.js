#pragma strict
import UnityEngine.UI;

var player : GameObject;

var moneyText : Text;

var itemIcons : Image[] = new Image[16];
var equipmentIcons : Image[] = new Image[16];

var weaponIcons : Image;
var weapon2Icons : Image;
var armorIcons : Image;
var meleeIcons : Image;

var tooltip : GameObject;
var tooltipIcon : Image;
var tooltipName : Text;
var tooltipText1 : Text;

var usableTab : GameObject;
var equipmentTab : GameObject;

var database : GameObject;
private var db : ItemData; 

function Start(){
	db = database.GetComponent(ItemData);
}

function Update(){
	if(tooltip && tooltip.activeSelf == true){
		var tooltipPos : Vector2 = Input.mousePosition;
		tooltipPos.x += 7;
		tooltip.transform.position = tooltipPos;
	}
	if(!player){
		return;
	}
	//itemIcons[0].GetComponent(Image).sprite = db.usableItem[player.GetComponent(Inventory).itemSlot[0]].iconSprite;
	
	for(var a : int = 0; a < itemIcons.Length; a++){
		itemIcons[a].GetComponent(Image).sprite = db.usableItem[player.GetComponent(Inventory).itemSlot[a]].iconSprite;
	}
	
	for(var b : int = 0; b < equipmentIcons.Length; b++){
		equipmentIcons[b].GetComponent(Image).sprite = db.equipment[player.GetComponent(Inventory).equipment[b]].iconSprite;
	}
	
	if(weaponIcons){
		weaponIcons.GetComponent(Image).sprite = db.equipment[player.GetComponent(Inventory).primaryEquip].iconSprite;
	}
	if(weapon2Icons){
		weapon2Icons.GetComponent(Image).sprite = db.equipment[player.GetComponent(Inventory).secondaryEquip].iconSprite;
	}
	if(armorIcons){
		armorIcons.GetComponent(Image).sprite = db.equipment[player.GetComponent(Inventory).armorEquip].iconSprite;
	}
	if(meleeIcons){
		meleeIcons.GetComponent(Image).sprite = db.equipment[player.GetComponent(Inventory).meleeEquip].iconSprite;
	}
	if(moneyText){
		moneyText.GetComponent(Text).text = player.GetComponent(Inventory).cash.ToString();
	}
}

function ShowItemTooltip(slot : int){
	if(!tooltip || !player){
		return;
	}
	if(player.GetComponent.<Inventory>().itemSlot[slot] <= 0){
		HideTooltip();
		return;
	}
		
	tooltipIcon.GetComponent.<Image>().sprite = db.usableItem[player.GetComponent.<Inventory>().itemSlot[slot]].iconSprite;
	tooltipName.GetComponent.<Text>().text = db.usableItem[player.GetComponent.<Inventory>().itemSlot[slot]].itemName + "  x" + player.GetComponent.<Inventory>().itemQuantity[slot].ToString();
		
	tooltipText1.GetComponent.<Text>().text = db.usableItem[player.GetComponent.<Inventory>().itemSlot[slot]].description;
	tooltip.SetActive(true);
}

function ShowEquipmentTooltip(slot : int){
	if(!tooltip || !player){
		return;
	}
	if(player.GetComponent.<Inventory>().equipment[slot] <= 0){
		HideTooltip();
		return;
	}
	var inv : Inventory = player.GetComponent.<Inventory>();
	tooltipIcon.GetComponent.<Image>().sprite = db.equipment[inv.equipment[slot]].iconSprite;
	if(db.equipment[inv.equipment[slot]].equipmentType == EqType.PrimaryWeapon || db.equipment[inv.equipment[slot]].equipmentType == EqType.SecondaryWeapon){
		tooltipName.GetComponent.<Text>().text = db.equipment[inv.equipment[slot]].itemName + " (" + inv.equipAmmo[slot].ToString() + "/" + db.equipment[inv.equipment[slot]].maxAmmo.ToString() + ")";
	}else{
		tooltipName.GetComponent.<Text>().text = db.equipment[inv.equipment[slot]].itemName;
	}
	
	tooltipText1.GetComponent.<Text>().text = db.equipment[inv.equipment[slot]].description;
	
	tooltip.SetActive(true);
}

function ShowOnEquipTooltip(type : int){
	if(!tooltip || !player){
		return;
	}
	//0 = Weapon, 1 = Weapon2, 2 = Armor, 3 = Melee
	var id : int = 0;
	if(type == 0){
		id = player.GetComponent.<Inventory>().primaryEquip;
	}
	if(type == 1){
		id = player.GetComponent.<Inventory>().secondaryEquip;
	}
	if(type == 2){
		id = player.GetComponent.<Inventory>().armorEquip;
	}
	if(type == 3){
		id = player.GetComponent.<Inventory>().meleeEquip;
	}
		
	if(id <= 0){
		HideTooltip();
		return;
	}
		
	tooltipIcon.GetComponent.<Image>().sprite = db.equipment[id].iconSprite;

	if(type == 0){
		tooltipName.GetComponent.<Text>().text = db.equipment[id].itemName + " (" + player.GetComponent.<GunTrigger>().primaryWeapon.ammo.ToString() + "/" + db.equipment[id].maxAmmo.ToString() + ")";
	}else if(type == 1){
		tooltipName.GetComponent.<Text>().text = db.equipment[id].itemName + " (" + player.GetComponent.<GunTrigger>().secondaryWeapon.ammo.ToString() + "/" + db.equipment[id].maxAmmo.ToString() + ")";
	}else{
		tooltipName.GetComponent.<Text>().text = db.equipment[id].itemName;
	}
		
	tooltipText1.GetComponent.<Text>().text = db.equipment[id].description;

	tooltip.SetActive(true);
}

function HideTooltip(){
	if(!tooltip){
		return;
	}
	tooltip.SetActive(false);
}

function UseItem(itemSlot : int){
	if(!player){
		return;
	}
	player.GetComponent(Inventory).UseItem(itemSlot);
	ShowItemTooltip(itemSlot);
}

function EquipItem(itemSlot : int){
	if(!player){
		return;
	}
	player.GetComponent(Inventory).EquipItem(player.GetComponent(Inventory).equipment[itemSlot] , itemSlot);
	ShowEquipmentTooltip(itemSlot);
}

function UnEquip(type : int){
	//0 = Weapon, 1 = Weapon2, 2 = Armor, 3 = Melee
	if(!player){
		return;
	}
	var id : int = 0;
	if(type == 0){
		id = player.GetComponent.<Inventory>().primaryEquip;
	}
	if(type == 1){
		id = player.GetComponent.<Inventory>().secondaryEquip;
	}
	if(type == 2){
		id = player.GetComponent.<Inventory>().armorEquip;
	}
	if(type == 3){
		id = player.GetComponent.<Inventory>().meleeEquip;
	}
	player.GetComponent.<Inventory>().UnEquip(id);
	ShowOnEquipTooltip(type);
}

function CloseMenu(){
	Time.timeScale = 1.0;
	//Screen.lockCursor = true;
	Cursor.lockState = CursorLockMode.Locked;
	Cursor.visible = false;
	gameObject.SetActive(false);
}

function OpenUsableTab(){
	usableTab.SetActive(true);
	equipmentTab.SetActive(false);
}

function OpenEquipmentTab(){
	usableTab.SetActive(false);
	equipmentTab.SetActive(true);
}

