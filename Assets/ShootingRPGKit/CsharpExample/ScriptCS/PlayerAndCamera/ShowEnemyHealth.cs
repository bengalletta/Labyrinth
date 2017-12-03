using UnityEngine;
using System.Collections;

public class ShowEnemyHealth : MonoBehaviour {

	public Texture2D border;
	public Texture2D hpBar;
	public Texture2D shieldBar;
	private string enemyName = "";
	public float duration = 7.0f;
	private bool  show = false;
	
	public int borderWidth = 200;
	public int borderHeigh = 26;
	public int hpBarHeight = 20;
	public float hpBarY = 28.0f;
	public float barMultiply = 1.8f;
	private float hpBarWidth;
	
	public GUIStyle textStyle;
	
	private int maxHp;
	private int hp;
	private int maxShield;
	private int shield;
	private float wait;
	private GameObject target;
	
	void  Start (){
		hpBarWidth = 100 * barMultiply;
	}
	
	void Update(){
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
			hp = target.GetComponent<Status>().health;
			shield = target.GetComponent<Status>().shield;
		}
	}
	
	void OnGUI(){
		if(show){
			float hpPercent = hp * 100 / maxHp *barMultiply;
			GUI.DrawTexture(new Rect(Screen.width /2 - borderWidth /2 , 25 , borderWidth, borderHeigh), border);
	    	GUI.DrawTexture(new Rect(Screen.width /2 - hpBarWidth /2 , hpBarY , hpPercent, hpBarHeight), hpBar);

			if(maxShield > 0){
				float shieldPercent = shield * 100 / maxShield * barMultiply;
				GUI.DrawTexture(new Rect(Screen.width /2 - hpBarWidth /2 , hpBarY , shieldPercent, hpBarHeight), shieldBar);
			}
	    	GUI.Label(new Rect(Screen.width /2 - hpBarWidth /2 , hpBarY, hpBarWidth, hpBarHeight), enemyName , textStyle);
		}
	
	}
	
	public void GetHP(int mhealth , GameObject mon , string monName){
		maxHp = mhealth;
		maxShield = mon.GetComponent<Status>().maxShield;
		target = mon;
		enemyName = monName;
		wait = 0;
		show = true;
	}
}
