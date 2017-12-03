using UnityEngine;
using System.Collections;

public class AllyHealthBar : MonoBehaviour {
	public string showName = "";
	public Texture2D hpBar;
	public Texture2D shieldBar;
	private Status stat;
	public GUIStyle nameFont;
	public GUIStyle statusFont;

	void Start(){
		stat = GetComponent<Status>();
	}
	
	void OnGUI (){
		int hp = stat.health * 100 / stat.maxHealth;
		GUI.Label ( new Rect(50, 180, 200, 40), "HP : " + stat.health.ToString() , statusFont);
		GUI.DrawTexture( new Rect(50 ,160 ,hp,10), hpBar);
		
		if(stat.maxShieldPlus > 0){
			int sh = stat.shield * 100 / stat.maxShieldPlus;
			GUI.Label ( new Rect(150, 180, 200, 40), "Shield : " + stat.shield.ToString() , statusFont);
			GUI.DrawTexture( new Rect(150 , 160 ,sh,10), shieldBar );
		}
		
		GUI.Label ( new Rect(50, 130, 250, 40), showName , nameFont);
	}
}
