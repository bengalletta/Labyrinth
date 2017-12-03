#pragma strict
var showName : String = "";
var hpBar : Texture2D;
var shieldBar : Texture2D;
private var stat : Status;
var nameFont : GUIStyle;
var statusFont : GUIStyle;

function Start () {
	stat = GetComponent(Status);
}

function OnGUI(){
	var hp : int = stat.health * 100 / stat.maxHealth;
	GUI.Label (Rect (50, 180, 200, 40), "HP : " + stat.health.ToString() , statusFont);
	GUI.DrawTexture(Rect(50 ,160 ,hp,10), hpBar);
	
	if(stat.maxShieldPlus > 0){
		var sh : int = stat.shield * 100 / stat.maxShieldPlus;
		GUI.Label (Rect (150, 180, 200, 40), "Shield : " + stat.shield.ToString() , statusFont);
		GUI.DrawTexture(Rect(150 , 160 ,sh,10), shieldBar );
	}
	
	GUI.Label (Rect (50, 130, 250, 40), showName , nameFont);
}