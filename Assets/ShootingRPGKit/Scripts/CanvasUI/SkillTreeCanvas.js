#pragma strict
import UnityEngine.UI;

class SkillSlot{
	var skillName : String = "";
	var skillId : int = 0;
	var unlockConditionId : int[];
	var unlockLevel : int = 1;
	var skPointUse : int = 1;
	
	//@HideInInspector
	var learned : boolean = false;
	var locked : boolean = false;
}

var skillSlots : SkillSlot[] = new SkillSlot[5];
var skillPointText : Text;
var shortcutPanel : GameObject;
var player : GameObject;
private var buttonSelect : int = 0;

function Start () {
	CheckUnlockSkill();
	UpdateSkillButton();
}

function Update () {
	if(!player){
		return;
	}
	if(skillPointText){
		skillPointText.text = player.GetComponent(Status).skillPoint.ToString();
	}
}

function CheckLearnedSkill(){
	if(!player){
		return;
	}
	var sk : SkillWindow = player.GetComponent(SkillWindow);
	for(var a : int = 0; a < skillSlots.Length; a++){
		if(skillSlots[a].skillId > 0){
			skillSlots[a].learned = sk.HaveSkill(skillSlots[a].skillId);
		}
	}
}

function CheckUnlockSkill(){
	if(!player){
		return;
	}
	CheckLearnedSkill();
	var sk : SkillWindow = player.GetComponent(SkillWindow);
	var lv : int = player.GetComponent(Status).level;
	
	for(var a : int = 0; a < skillSlots.Length; a++){
		//Check Player Level
		var lvPass : boolean = false;
		var allUnlock : int = 0;
		if(lv >= skillSlots[a].unlockLevel){
			lvPass = true;
		}
		
		//Check unlockConditionId
		if(skillSlots[a].unlockConditionId.Length > 0){
			allUnlock = 0;
			for(var b : int = 0; b < skillSlots[a].unlockConditionId.Length; b++){
				if(sk.HaveSkill(skillSlots[a].unlockConditionId[b])){
					allUnlock++;
				}
			}
		
		}
		//If Overall Pass
		if(lvPass && allUnlock >= skillSlots[a].unlockConditionId.Length){
			skillSlots[a].locked = false;
		}
		
	}
}

function ButtonSkillClick(buttonId : int){
	if(!player || skillSlots[buttonId].locked){
		return;
	}
	if(!skillSlots[buttonId].learned){
		LearnSkill(buttonId);
	}else{
		buttonSelect = buttonId;
		ShowShortcutSetting();
	}
}

function ShowShortcutSetting(){
	shortcutPanel.SetActive(true);
	shortcutPanel.transform.position = Input.mousePosition;
}

function SetSlot(slot : int){
	shortcutPanel.SetActive(false);
	var sk : SkillWindow = player.GetComponent(SkillWindow);
	sk.AssignSkillByID(slot , skillSlots[buttonSelect].skillId);
}

function LearnSkill(buttonId : int){
	if(player.GetComponent(Status).skillPoint < skillSlots[buttonId].skPointUse){
		print("Not enough Skill Point");
		return;
	}
	player.GetComponent(Status).skillPoint -= skillSlots[buttonId].skPointUse;
	var sk : SkillWindow = player.GetComponent(SkillWindow);
	
	sk.AddSkill(skillSlots[buttonId].skillId);
	CheckUnlockSkill();
	UpdateSkillButton();
}

function UpdateSkillButton(){
	var skillButton: SkillButtonCanvas[];
	skillButton = GetComponentsInChildren.<SkillButtonCanvas>();
	if(skillButton != null) {
		for(var button : SkillButtonCanvas in skillButton){
			button.UpdateIcon();
		}
	}
}

function CloseMenu(){
	Time.timeScale = 1.0;
	//Screen.lockCursor = true;
	Cursor.lockState = CursorLockMode.Locked;
	Cursor.visible = false;
	gameObject.SetActive(false);
}

