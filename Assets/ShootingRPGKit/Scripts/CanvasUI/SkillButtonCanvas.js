#pragma strict
import UnityEngine.UI;

var buttonId : int = 0;
var iconImageObj : Image;
var icon : Sprite;
var iconLocked : Sprite;

var skillName : String = "";
var description : String = "";
var description2 : String = "";
var description3 : String = "";

var skillTree : SkillTreeCanvas;

var tooltip : GameObject;
var tooltipIcon : Image;
var tooltipName : Text;
var tooltipText1 : Text;
var tooltipText2 : Text;
var tooltipText3 : Text;

function Start () {
	if(!skillTree){
		skillTree = transform.root.GetComponent(SkillTreeCanvas);
	}
}

function Update () {
	if(tooltip && tooltip.activeSelf == true){
		var tooltipPos : Vector2 = Input.mousePosition;
		tooltipPos.x += 7;
		tooltip.transform.position = tooltipPos;
	}
}

function UpdateIcon(){
	iconImageObj.color = Color.white;
	if(skillTree.skillSlots[buttonId].locked){
		iconImageObj.sprite = iconLocked;
		return;
	}else{
		iconImageObj.sprite = icon;
	}
	
	if(!skillTree.skillSlots[buttonId].learned){
		iconImageObj.color = Color.gray;
	}
}

function ShowSkillTooltip(){
	if(!tooltip){
		return;
	}
	tooltipIcon.sprite = icon;
	tooltipName.text = skillName;
	
	tooltipText1.text = description;
	tooltipText2.text = description2;
	tooltipText3.text = description3;
	
	tooltip.SetActive(true);
}

function HideTooltip(){
	if(!tooltip){
		return;
	}
	tooltip.SetActive(false);
}
