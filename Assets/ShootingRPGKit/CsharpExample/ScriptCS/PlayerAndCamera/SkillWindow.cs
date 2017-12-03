using UnityEngine;
using System.Collections;

public class SkillWindow : MonoBehaviour {
	public GameObject player;
	public GameObject database;
	
	public int[] skill = new int[4];
	public int[] skillListSlot = new int[15];

	public LearnSkillLV[] learnSkill = new LearnSkillLV[2];
	
	private bool menu = false;
	private bool shortcutPage = true;
	private bool skillListPage = false;
	private int skillSelect = 0;
	
	public GUISkin skin1;
	public Rect windowRect = new Rect (360 ,80 ,360 ,185);
	private Rect originalRect;
	
	public GUIStyle skillNameText;
	public GUIStyle skillDescriptionText;
	public GUIStyle showLearnSkillText;
	
	private bool showSkillLearned = false;
	private string showSkillName = "";
	
	public int pageMultiply = 5;
	private int page = 0;
	public bool autoAssignSkill = false;
	public bool useLegacyUi = false;
	
	void Start(){
		if(!player){
			player = this.gameObject;
		}
		originalRect = windowRect;
		if(autoAssignSkill){
			AssignAllSkill();
		}
	}
	
	void Update(){
		if(Input.GetKeyDown("k") && useLegacyUi) {
			OnOffMenu();
		}
	}
	
	void OnOffMenu(){
		//Freeze Time Scale to 0 if Window is Showing
		if(!menu && Time.timeScale != 0.0f){
			menu = true;
			skillListPage = false;
			shortcutPage = true;
			Time.timeScale = 0.0f;
			ResetPosition();
			Screen.lockCursor = false;
		}else if(menu){
			menu = false;
			Time.timeScale = 1.0f;
			Screen.lockCursor = true;
		}
	}
	
	void OnGUI(){
		if(showSkillLearned){
			GUI.Label ( new Rect(Screen.width /2 -50, 85, 400, 50), "You Learned  " + showSkillName , showLearnSkillText);
		}
		if(menu && shortcutPage){
			windowRect = GUI.Window (3, windowRect, SkillShortcut, "Skill");
		}
		//---------------Skill List----------------------------
		if(menu && skillListPage){
			windowRect = GUI.Window (3, windowRect, AllSkill, "Skill");
		}
	}
	
	public void AssignSkill (int id , int sk){
		SkillData dataSkill = database.GetComponent<SkillData>();
		player.GetComponent<GunTrigger>().skill[id].manaCost = dataSkill.skill[skillListSlot[sk]].manaCost;
		player.GetComponent<GunTrigger>().skill[id].skillPrefab = dataSkill.skill[skillListSlot[sk]].skillPrefab;
		player.GetComponent<GunTrigger>().skill[id].skillAnimation = dataSkill.skill[skillListSlot[sk]].skillAnimation;
		player.GetComponent<GunTrigger>().skill[id].icon = dataSkill.skill[skillListSlot[sk]].icon;
		player.GetComponent<GunTrigger>().skill[id].castTime = dataSkill.skill[skillListSlot[sk]].castTime;
		player.GetComponent<GunTrigger>().skill[id].skillDelay = dataSkill.skill[skillListSlot[sk]].skillDelay;
		skill[id] = skillListSlot[sk];
		print(sk);
	}
	
	public void AssignAllSkill(){
		if(!player){
			player = this.gameObject;
		}
		int n = 0;
		SkillData dataSkill = database.GetComponent<SkillData>();
		while(n < skill.Length){
			player.GetComponent<GunTrigger>().skill[n].manaCost = dataSkill.skill[skill[n]].manaCost;
			player.GetComponent<GunTrigger>().skill[n].skillPrefab = dataSkill.skill[skill[n]].skillPrefab;
			player.GetComponent<GunTrigger>().skill[n].skillAnimation = dataSkill.skill[skill[n]].skillAnimation;
			player.GetComponent<GunTrigger>().skill[n].icon = dataSkill.skill[skill[n]].icon;
			player.GetComponent<GunTrigger>().skill[n].castTime = dataSkill.skill[skill[n]].castTime;
			player.GetComponent<GunTrigger>().skill[n].skillDelay = dataSkill.skill[skill[n]].skillDelay;
			
			n++;
		}
	}

	public void AssignSkillByID(int slot , int skillId){
		//Use With Canvas UI
		SkillData dataSkill = database.GetComponent<SkillData>();
		GetComponent<GunTrigger>().skill[slot].manaCost = dataSkill.skill[skillId].manaCost;
		GetComponent<GunTrigger>().skill[slot].skillPrefab = dataSkill.skill[skillId].skillPrefab;
		GetComponent<GunTrigger>().skill[slot].skillAnimation = dataSkill.skill[skillId].skillAnimation;
		
		GetComponent<GunTrigger>().skill[slot].icon = dataSkill.skill[skillId].icon;
		GetComponent<GunTrigger>().skill[slot].castTime = dataSkill.skill[skillId].castTime;
		GetComponent<GunTrigger>().skill[slot].skillDelay = dataSkill.skill[skillId].skillDelay;

		skill[slot] = skillId;
	}
	
	void SkillShortcut(int windowID){
		SkillData dataSkill = database.GetComponent<SkillData>();
		windowRect.width = 470;
		windowRect.height = 200;
		//Close Window Button
		if(GUI.Button(new Rect(windowRect.width - 40,5,30,30), "X")){
			OnOffMenu();
		}
		
		//Skill Shortcut
		if(GUI.Button ( new Rect(40,45,80,80), dataSkill.skill[skill[0]].icon)){
			skillSelect = 0;
			skillListPage = true;
			shortcutPage = false;
		}
		GUI.Label(new Rect(70, 140, 20, 20), "1");
		
		if(GUI.Button(new Rect(140,45,80,80), dataSkill.skill[skill[1]].icon)){
			skillSelect = 1;
			skillListPage = true;
			shortcutPage = false;
		}
		GUI.Label(new Rect(170, 140, 20, 20), "2");
		
		if(GUI.Button(new Rect(240,45,80,80), dataSkill.skill[skill[2]].icon)){
			skillSelect = 2;
			skillListPage = true;
			shortcutPage = false;
		}
		GUI.Label(new Rect(270, 140, 20, 20), "3");
		
		if(GUI.Button ( new Rect(340,45,80,80), dataSkill.skill[skill[3]].icon)) {
			skillSelect = 3;
			skillListPage = true;
			shortcutPage = false;
		}
		GUI.Label(new Rect(370, 140, 20, 20), "4");
		
		GUI.DragWindow(new Rect (0,0,10000,10000));
		
	}
	
	void AllSkill(int windowID){
		SkillData dataSkill = database.GetComponent<SkillData>();
		windowRect.width = 400;
		windowRect.height = 575;
		//Close Window Button
		if(GUI.Button(new Rect(windowRect.width - 40,5,30,30), "X")){
			OnOffMenu();
		}
		if(GUI.Button(new Rect(20,60,75,75), dataSkill.skill[skillListSlot[0 + page]].icon)){
			AssignSkill(skillSelect , 0 + page);
			shortcutPage = true;
			skillListPage = false;
			
		}
		GUI.Label(new Rect(110, 70, 140, 40), dataSkill.skill[skillListSlot[0 + page]].skillName , skillNameText); //Show Skill's Name
		GUI.Label(new Rect(110, 95, 140, 40), dataSkill.skill[skillListSlot[0 + page]].description , skillDescriptionText); //Show Skill's Description
		GUI.Label(new Rect(310, 70, 140, 40), "MP : " + dataSkill.skill[skillListSlot[0 + page]].manaCost , skillDescriptionText); //Show Skill's MP Cost
		//-----------------------------
		if(GUI.Button ( new Rect(20,150,75,75), dataSkill.skill[skillListSlot[1 + page]].icon)) {
			AssignSkill(skillSelect , 1 + page);
			shortcutPage = true;
			skillListPage = false;
			
		}
		GUI.Label(new Rect(110, 160, 140, 40), dataSkill.skill[skillListSlot[1 + page]].skillName , skillNameText); //Show Skill's Name
		GUI.Label(new Rect(110, 185, 140, 40), dataSkill.skill[skillListSlot[1 + page]].description , skillDescriptionText); //Show Skill's Description
		GUI.Label(new Rect(310, 160, 140, 40), "MP : " + dataSkill.skill[skillListSlot[1 + page]].manaCost , skillDescriptionText); //Show Skill's MP Cost
		//-----------------------------
		if(GUI.Button ( new Rect(20,240,75,75), dataSkill.skill[skillListSlot[2 + page]].icon)) {
			AssignSkill(skillSelect , 2 + page);
			shortcutPage = true;
			skillListPage = false;
			
		}
		GUI.Label(new Rect(110, 250, 140, 40), dataSkill.skill[skillListSlot[2 + page]].skillName , skillNameText); //Show Skill's Name
		GUI.Label(new Rect(110, 275, 140, 40), dataSkill.skill[skillListSlot[2 + page]].description , skillDescriptionText); //Show Skill's Description
		GUI.Label(new Rect(310, 250, 140, 40), "MP : " + dataSkill.skill[skillListSlot[2 + page]].manaCost , skillDescriptionText); //Show Skill's MP Cost
		//-----------------------------
		if(GUI.Button ( new Rect(20,330,75,75), dataSkill.skill[skillListSlot[3 + page]].icon)) {
			AssignSkill(skillSelect , 3 + page);
			shortcutPage = true;
			skillListPage = false;
			
		}
		GUI.Label(new Rect(110, 340, 140, 40), dataSkill.skill[skillListSlot[3 + page]].skillName , skillNameText); //Show Skill's Name
		GUI.Label(new Rect(110, 365, 140, 40), dataSkill.skill[skillListSlot[3 + page]].description , skillDescriptionText); //Show Skill's Description
		GUI.Label(new Rect(310, 340, 140, 40), "MP : " + dataSkill.skill[skillListSlot[3 + page]].manaCost , skillDescriptionText); //Show Skill's MP Cost
		//-----------------------------
		if(GUI.Button ( new Rect(20,420,75,75), dataSkill.skill[skillListSlot[4 + page]].icon)) {
			AssignSkill(skillSelect , 4 + page);
			shortcutPage = true;
			skillListPage = false;
			
		}
		GUI.Label(new Rect(110, 430, 140, 40), dataSkill.skill[skillListSlot[4 + page]].skillName , skillNameText); //Show Skill's Name
		GUI.Label(new Rect(110, 455, 140, 40), dataSkill.skill[skillListSlot[4 + page]].description , skillDescriptionText); //Show Skill's Description
		GUI.Label(new Rect(310, 430, 140, 40), "MP : " + dataSkill.skill[skillListSlot[4 + page]].manaCost , skillDescriptionText); //Show Skill's MP Cost
		//-----------------------------
		
		
		if(GUI.Button ( new Rect(150,515,50,52), "1")){
			page = 0;
		}
		if(GUI.Button ( new Rect(220,515,50,52), "2")){
			page = pageMultiply;
		}
		if(GUI.Button ( new Rect(290,515,50,52), "3")){
			page = pageMultiply *2;
		}
		
		GUI.DragWindow (new Rect (0,0,10000,10000));
	}
	
	public void AddSkill(int id){
		bool  geta = false;
		int pt = 0;
		while(pt < skillListSlot.Length && !geta){
			if(skillListSlot[pt] == id){
				// Check if you already have this skill.
				geta = true;
			}else if(skillListSlot[pt] == 0){
				// Add Skill to empty slot.
				skillListSlot[pt] = id;
				//ShowLearnedSkill(id);
				StartCoroutine(ShowLearnedSkill(id));
				geta = true;
			}else{
				pt++;
			}
			
		}
		
	}
	
	public IEnumerator ShowLearnedSkill(int id){
		SkillData dataSkill = database.GetComponent<SkillData>();
		showSkillLearned = true;
		showSkillName = dataSkill.skill[id].skillName;
		yield return new WaitForSeconds(10.5f);
		showSkillLearned = false;
		
	}
	
	void ResetPosition (){
		//Reset GUI Position when it out of Screen.
		if(windowRect.x >= Screen.width -30 || windowRect.y >= Screen.height -30 || windowRect.x <= -70 || windowRect.y <= -70 ){
			windowRect = originalRect;
		}
	}
	
	public void LearnSkillByLevel (int lv){
		int c = 0;
		while(c < learnSkill.Length){
			if(lv >= learnSkill[c].level){
				AddSkill(learnSkill[c].skillId);
			}
			c++;
		}
		
	}

	public bool HaveSkill(int id){
		bool have = false;
		for(int a = 0; a < skillListSlot.Length; a++){
			if(skillListSlot[a] == id){
				have = true;
			}
		}
		return have;
	}
}

[System.Serializable]
public class LearnSkillLV{
	public int level = 1;
	public int skillId = 1;
}