#pragma strict
var operatorPicture : Texture2D[] = new Texture2D[3];
private var showPicture : Texture2D;
var showText : String;
var skin : GUISkin;

var startMessage : OperatorSet[] = new OperatorSet[1];
var clearMissionMessage : OperatorSet[] = new OperatorSet[1];
var otherMessage : OperatorSet[] = new OperatorSet[1];

var textDuration : float = 4.5;

private var show : boolean = false;

class OperatorSet{
	var message : String;
	var pictureId : int = 0;
}

function Start () {
	yield WaitForSeconds(1.2);
	show = false;
	show = true;
	var i: int = 0;
	while(i < startMessage.Length && show){
		AnimateText(startMessage[i].message);
		showPicture = operatorPicture[startMessage[i].pictureId];
		yield WaitForSeconds(textDuration);
		i++;
	}
	show = false;
}

function Update () {

}

function OnGUI(){
	if(show){
		if(showPicture)
			GUI.DrawTexture(Rect(Screen.width - 310, Screen.height /2 - 150, 250, 250), showPicture);
		
		GUI.skin = skin;
		GUI.Box(new Rect(Screen.width - 330 , Screen.height /2 + 110 ,300,40), showText);
	}
}

function MissionClear(){
	show = false;
	show = true;
	var i: int = 0;
	while(i < clearMissionMessage.Length && show){
		AnimateText(clearMissionMessage[i].message);
		showPicture = operatorPicture[clearMissionMessage[i].pictureId];
		yield WaitForSeconds(textDuration);
		i++;
	}
	show = false;
}

function ShowOtherMessage(){
	//Call other function to prevent yield problem when calling from other object.
	OtherMessage();
}

function OtherMessage(){
	show = false;
	show = true;
	var i: int = 0;
	while(i < otherMessage.Length && show){
		AnimateText(otherMessage[i].message);
		showPicture = operatorPicture[otherMessage[i].pictureId];
		yield WaitForSeconds(textDuration);
		i++;
	}
	show = false;
}


function AnimateText(strComplete : String){
	var i: int = 0;
	showText = "";
	while(i < strComplete.Length){
		showText += strComplete[i++];
		yield WaitForSeconds(0.05);
	}

}

