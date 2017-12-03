using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Status))]
public class StatusWindow : MonoBehaviour {
	private bool  show = false;
	public GUIStyle textStyle;
	public GUIStyle textStyle2;

	public GUISkin skin;
	public Rect windowRect = new Rect (180, 170, 300, 400);
	private Rect originalRect;
	
	void Start (){
		originalRect = windowRect;
	}
	
	void Update (){
		if(Input.GetKeyDown("c")){
			OnOffMenu();
		}
	}
	
	void OnGUI(){
		GUI.skin = skin;
		Status stat = GetComponent<Status>();
		if(show){
			windowRect = GUI.Window (0, windowRect, StatWindow, "Status");
		}
	}
	
	void StatWindow(int windowID){
		Status stat = GetComponent<Status>();
		//Status
		int lv = stat.level;
		int atk = stat.atk;
		int def = stat.def;
		int matk = stat.matk;
		int mdef = stat.mdef;
		int melee = stat.melee;
		int exp = stat.exp;
		int next = stat.maxExp - exp;
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
	
	void OnOffMenu (){
		//Freeze Time Scale to 0 if Status Window is Showing
		if(!show && Time.timeScale != 0.0f){
			show = true;
			Time.timeScale = 0.0f;
			ResetPosition();
			Screen.lockCursor = false;
		}else if(show){
			show = false;
			Time.timeScale = 1.0f;
			Screen.lockCursor = true;
		}
	}
	
	void ResetPosition (){
		//Reset GUI Position when it out of Screen.
		if(windowRect.x >= Screen.width -30 || windowRect.y >= Screen.height -30 || windowRect.x <= -70 || windowRect.y <= -70 ){
			windowRect = originalRect;
		}
		
	}

}