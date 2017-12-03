#pragma strict
var border : Texture2D;
var hpBar : Texture2D;
var shieldBar : Texture2D;
private var enemyName : String = "";
var duration : float = 7.0;
private var show : boolean = false;

var borderWidth : int = 200;
var borderHeigh : int = 26;
var hpBarHeight : int = 20;
var hpBarY : float = 28.0;
var barMultiply : float = 1.8;
private var hpBarWidth : float;

var textStyle : GUIStyle;

private var maxHp : int;
private var hp : int;
private var maxShield : int;
private var shield : int;
private var wait : float;
private var target : GameObject;
private var lv : String = "";

function Start () {
	hpBarWidth = 100 * barMultiply;
}

function Update () {
	 if(show){
	  	if(wait >= duration){
	       show = false;
	     }else{
	      	wait += Time.deltaTime;
	     }
	 
	 }
	 if(show && !target){
	 	hp = 0;
	 	shield = 0;
	 }else if(show && target){
	 	hp = target.GetComponent(Status).health;
	 	shield = target.GetComponent(Status).shield;
	 }

}

function OnGUI () {
	if(show){
		var hpPercent : int = hp * 100 / maxHp *barMultiply;
		
		GUI.DrawTexture(Rect(Screen.width /2 - borderWidth /2 , 25 , borderWidth, borderHeigh), border);
    	GUI.DrawTexture(Rect(Screen.width /2 - hpBarWidth /2 , hpBarY , hpPercent, hpBarHeight), hpBar);
    	
    	if(maxShield > 0){
			var shieldPercent : int = shield * 100 / maxShield *barMultiply;
			GUI.DrawTexture(Rect(Screen.width /2 - hpBarWidth /2 , hpBarY , shieldPercent, hpBarHeight), shieldBar);
		}
    	
    	GUI.Label (Rect (Screen.width /2 - hpBarWidth /2 , hpBarY, hpBarWidth, hpBarHeight), enemyName + " (" + lv + ")" , textStyle); //Draw Enemy's name and level.
	
	}

}

function GetHP(mhealth : int , mon : GameObject , monName : String){
	maxHp = mhealth;
	maxShield = mon.GetComponent(Status).maxShield;
	target = mon;
	enemyName = monName;
	wait = 0;
	lv = mon.GetComponent(Status).level.ToString();
	show = true;

}