#pragma strict
private var show : boolean = false;
var textStyle : GUIStyle;
var textStyle2 : GUIStyle;

var skin : GUISkin;
var windowRect : Rect = new Rect (180, 170, 300, 400);
private var originalRect : Rect;

function Start(){
	originalRect = windowRect;
}

function Update () {
	if(Input.GetKeyDown("c")){
		OnOffMenu();
	}
}

function OnGUI(){
	GUI.skin = skin;
	var stat : Status = GetComponent(Status);
	if(show){
		windowRect = GUI.Window (0, windowRect, StatWindow, "Status");	
	}
}
	
function StatWindow(windowID : int){
			var stat : Status = GetComponent(Status);
			//Status
			var lv : int = stat.level;
			var atk : int = stat.atk;
			var def : int = stat.def;
			var matk : int = stat.matk;
			var mdef : int = stat.mdef;
			var melee : int = stat.melee;
			var exp : int = stat.exp;
			var next : int = stat.maxExp - exp;
			var stPoint : int = stat.statusPoint;
			//GUI.Box ( new Rect(180,170,240,380), "Status");
			GUI.Label ( new Rect(20, 40, 100, 50), "Level" , textStyle);
			GUI.Label ( new Rect(100, 40, 100, 50), lv.ToString() , textStyle2);
			
			GUI.Label ( new Rect(20, 70, 100, 50), "Damage" , textStyle);
			GUI.Label ( new Rect(180, 70, 100, 50), atk.ToString() + "  (" + stat.addAtk.ToString() + ")" , textStyle2);
			
			GUI.Label ( new Rect(20, 100, 100, 50), "Melee Damage" , textStyle);
			GUI.Label ( new Rect(180, 100, 100, 50), melee.ToString() + "  (" + stat.addMelee.ToString() + ")"  , textStyle2);
			
			GUI.Label ( new Rect(20, 130, 100, 50), "Defense" , textStyle);
			GUI.Label ( new Rect(180, 130, 100, 50), def.ToString() + "  (" + stat.addDef.ToString() + ")"  , textStyle2);
			
			GUI.Label ( new Rect(20, 160, 100, 50), "Magic Damage" , textStyle);
			GUI.Label ( new Rect(180, 160, 100, 50), matk.ToString() + "  (" + stat.addMatk.ToString() + ")"  , textStyle2);
			
			GUI.Label ( new Rect(20, 190, 100, 50), "Magic Defense" , textStyle);
			GUI.Label ( new Rect(180, 190, 100, 50), mdef.ToString() + "  (" + stat.addMdef.ToString() + ")"  , textStyle2);
			
			GUI.Label ( new Rect(20, 220, 100, 50), "HP" , textStyle);
			GUI.Label ( new Rect(155, 220, 100, 50), stat.health.ToString() + " / " + stat.maxHealth.ToString() , textStyle2);
			
			GUI.Label ( new Rect(20, 250, 100, 50), "MP" , textStyle);
			GUI.Label ( new Rect(155, 250, 100, 50), stat.mana.ToString() + " / " + stat.maxMana.ToString() , textStyle2);
			
			GUI.Label ( new Rect(20, 280, 100, 50), "Shield" , textStyle);
			GUI.Label ( new Rect(155, 280, 100, 50), stat.shield.ToString() + " / " + stat.maxShield.ToString() , textStyle2);
			
			GUI.Label ( new Rect(20, 320, 100, 50), "EXP" , textStyle);
			GUI.Label ( new Rect(155, 320, 100, 50), exp.ToString() , textStyle2);
			
			GUI.Label ( new Rect(20, 350, 100, 50), "Next LV" , textStyle);
			GUI.Label ( new Rect(155, 350, 100, 50), next.ToString() , textStyle2);
			
			//Close Window Button
			if (GUI.Button ( new Rect(windowRect.width - 40 , 5 ,30,30), "X")) {
				OnOffMenu();
			}
			
			GUI.DragWindow (new Rect (0,0,10000,10000)); 
	}

function OnOffMenu(){
	//Freeze Time Scale to 0 if Status Window is Showing
	if(!show && Time.timeScale != 0.0){
			show = true;
			Time.timeScale = 0.0;
			ResetPosition();
			Screen.lockCursor = false;
	}else if(show){
			show = false;
			Time.timeScale = 1.0;
			Screen.lockCursor = true;
	}
}

function ResetPosition(){
		//Reset GUI Position when it out of Screen.
		if(windowRect.x >= Screen.width -30 || windowRect.y >= Screen.height -30 || windowRect.x <= -70 || windowRect.y <= -70 ){
			windowRect = originalRect;
		}
		
}

@script RequireComponent (Status)