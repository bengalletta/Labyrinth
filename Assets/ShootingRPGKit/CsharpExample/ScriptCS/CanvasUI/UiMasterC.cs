using UnityEngine;
using System.Collections;

public class UiMasterC : MonoBehaviour {
	public GameObject eventSystemPrefab;
	public GameObject healthBarPrefab;
	public GameObject statusWindowPrefab;
	public GameObject skillWindowPrefab;
	public GameObject inventoryPrefab;
	private GameObject st;
	private GameObject sk;
	private GameObject inv;
	private GameObject ev;
	private GameObject hp;

	void Start(){
		if(eventSystemPrefab){
			ev = Instantiate(eventSystemPrefab , eventSystemPrefab.transform.position , eventSystemPrefab.transform.rotation) as GameObject;
			DontDestroyOnLoad(ev.gameObject);
		}
		if(healthBarPrefab){
			hp = Instantiate(healthBarPrefab , healthBarPrefab.transform.position , healthBarPrefab.transform.rotation) as GameObject;
			hp.GetComponent<HealthBarCanvasC>().player = this.gameObject;
		}
		if(statusWindowPrefab){
			st = Instantiate(statusWindowPrefab , statusWindowPrefab.transform.position , statusWindowPrefab.transform.rotation) as GameObject;
			st.GetComponent<StatusWindowCanvasC>().player = this.gameObject;
			DontDestroyOnLoad(st.gameObject);
			st.SetActive(false);
		}
		if(inventoryPrefab){
			inv = Instantiate(inventoryPrefab , inventoryPrefab.transform.position , inventoryPrefab.transform.rotation) as GameObject;
			inv.GetComponent<InventoryUiCanvasC>().player = this.gameObject;
			DontDestroyOnLoad(inv.gameObject);
			inv.SetActive(false);
		}
		if(skillWindowPrefab){
			sk = Instantiate(skillWindowPrefab , skillWindowPrefab.transform.position , skillWindowPrefab.transform.rotation) as GameObject;
			sk.GetComponent<SkillTreeCanvasC>().player = this.gameObject;
			DontDestroyOnLoad(sk.gameObject);
			sk.SetActive(false);
		}
	}
	
	void Update(){
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
	
	public void CloseAllMenu(){
		if(st)
			st.SetActive(false);
		if(inv)
			inv.SetActive(false);
		if(sk)
			sk.SetActive(false);
	}
	
	public void OnOffStatusMenu(){
		//Freeze Time Scale to 0 if Status Window is Showing
		if(st.activeSelf == false){
			Time.timeScale = 0.0f;
			//Screen.lockCursor = false;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			CloseAllMenu();
			st.SetActive(true);
		}else{
			Time.timeScale = 1.0f;
			//Screen.lockCursor = true;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			CloseAllMenu();
		}
	}
	
	public void OnOffInventoryMenu(){
		//Freeze Time Scale to 0 if Status Window is Showing
		if(inv.activeSelf == false){
			Time.timeScale = 0.0f;
			//Screen.lockCursor = false;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			CloseAllMenu();
			inv.SetActive(true);
		}else{
			Time.timeScale = 1.0f;
			//Screen.lockCursor = true;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			CloseAllMenu();
		}
	}

	public void OnOffSkillMenu(){
		//Freeze Time Scale to 0 if Status Window is Showing
		if(sk.activeSelf == false){
			Time.timeScale = 0.0f;
			//Screen.lockCursor = false;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			CloseAllMenu();
			sk.SetActive(true);
			sk.GetComponent<SkillTreeCanvasC>().Start();
		}else{
			Time.timeScale = 1.0f;
			//Screen.lockCursor = true;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			CloseAllMenu();
		}
	}

	public void DestroyAllUi(){
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
}