#pragma strict
var eventSystemPrefab : GameObject;
var healthBarPrefab : GameObject;
var statusWindowPrefab : GameObject;
var skillWindowPrefab : GameObject;
var inventoryPrefab : GameObject;
private var st : GameObject;
private var sk : GameObject;
private var inv : GameObject;
private var ev : GameObject;
private var hp : GameObject;

function Start() {
	if(eventSystemPrefab){
		ev = Instantiate(eventSystemPrefab , eventSystemPrefab.transform.position , eventSystemPrefab.transform.rotation);
		DontDestroyOnLoad(ev.gameObject);
	}
	if(healthBarPrefab){
		hp = Instantiate(healthBarPrefab , healthBarPrefab.transform.position , healthBarPrefab.transform.rotation);
		hp.GetComponent(HealthBarCanvas).player = this.gameObject;
	}
	if(statusWindowPrefab){
		st = Instantiate(statusWindowPrefab , statusWindowPrefab.transform.position , statusWindowPrefab.transform.rotation);
		st.GetComponent(StatusWindowCanvas).player = this.gameObject;
		DontDestroyOnLoad(st.gameObject);
		st.SetActive(false);
	}
	if(inventoryPrefab){
		inv = Instantiate(inventoryPrefab , inventoryPrefab.transform.position , inventoryPrefab.transform.rotation);
		inv.GetComponent(InventoryUiCanvas).player = this.gameObject;
		DontDestroyOnLoad(inv.gameObject);
		inv.SetActive(false);
	}
	if(skillWindowPrefab){
		sk = Instantiate(skillWindowPrefab , skillWindowPrefab.transform.position , skillWindowPrefab.transform.rotation);
		sk.GetComponent(SkillTreeCanvas).player = this.gameObject;
		DontDestroyOnLoad(sk.gameObject);
		sk.SetActive(false);
	}
}

function Update () {
	if(st && Input.GetKeyDown("c")){
		OnOffStatusMenu();
	}
	if(inv && Input.GetKeyDown("i")){
		OnOffInventoryMenu();
	}
	if(sk && Input.GetKeyDown("k")){
		OnOffSkillMenu();
	}
}

function CloseAllMenu(){
	if(st)
		st.SetActive(false);
	if(inv)
		inv.SetActive(false);
	if(sk)
		sk.SetActive(false);
}

function OnOffStatusMenu(){
	//Freeze Time Scale to 0 if Status Window is Showing
	if(st.activeSelf == false){
		Time.timeScale = 0.0;
		//Screen.lockCursor = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		CloseAllMenu();
		st.SetActive(true);
	}else{
		Time.timeScale = 1.0;
		//Screen.lockCursor = true;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		CloseAllMenu();
	}
}

function OnOffInventoryMenu(){
	//Freeze Time Scale to 0 if Status Window is Showing
	if(inv.activeSelf == false){
		Time.timeScale = 0.0;
		//Screen.lockCursor = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		CloseAllMenu();
		inv.SetActive(true);
	}else{
		Time.timeScale = 1.0;
		//Screen.lockCursor = true;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		CloseAllMenu();
	}
}

function OnOffSkillMenu(){
	//Freeze Time Scale to 0 if Status Window is Showing
	if(sk.activeSelf == false){
		Time.timeScale = 0.0;
		//Screen.lockCursor = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		CloseAllMenu();
		sk.SetActive(true);
		sk.GetComponent(SkillTreeCanvas).Start();
	}else{
		Time.timeScale = 1.0;
		//Screen.lockCursor = true;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		CloseAllMenu();
	}
}

function DestroyAllUi(){
	if(st)
		Destroy(st);
	if(inv)
		Destroy(inv);
	if(sk)
		Destroy(sk);
	if(ev)
		Destroy(ev);
	if(hp)
		Destroy(hp);
	if(ev)
		Destroy(ev);
}
