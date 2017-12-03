using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBarCanvasC : MonoBehaviour {
	public Image hpBar;
	public Image mpBar;
	public Image shieldBar;
	public Image expBar;
	public Text hpText;
	public Text mpText;
	public Text shieldText;
	public Text lvText;
	public GameObject player;

	//public Sprite hp2;
	
	void Start(){
		DontDestroyOnLoad(transform.gameObject);
		if(!player){
			player = GameObject.FindWithTag("Player");
		}
	}
	
	void Update(){
		if(!player){
			Destroy(gameObject);
			return;
		}
		Status stat = player.GetComponent<Status>();
		
		int maxHp = stat.maxHealth;
		float hp = stat.health;
		int maxMp = stat.maxMana;
		float mp = stat.mana;
		int exp = stat.exp;
		float maxExp = stat.maxExp;
		int shield = stat.shield;
		float maxShield = stat.maxShieldPlus;
		//float target = (float)cur_hp / (float)cur_mhp;
		float curHp = hp/maxHp;
		float curMp = mp/maxMp;
		float curExp = exp/maxExp;
		float curShield = shield/maxShield;

		/*if(curHp >= 0.75){
			hpBar.color = Color.green;
			hpBar.sprite = hp2;
		}else{
			hpBar.color = Color.red;
		}*/
		
		//HP Gauge
		if(curHp > hpBar.fillAmount){
			hpBar.fillAmount += 1 / 1 * Time.unscaledDeltaTime;
			if(hpBar.fillAmount > curHp){
				hpBar.fillAmount = curHp;
			}
		}	
		if(curHp < hpBar.fillAmount){
			hpBar.fillAmount -= 1 / 1 * Time.unscaledDeltaTime;
			if(hpBar.fillAmount < curHp){
				hpBar.fillAmount = curHp;
			}
		}
		
		//MP Gauge
		if(curMp > mpBar.fillAmount){
			mpBar.fillAmount += 1 / 1 * Time.unscaledDeltaTime;
			if(mpBar.fillAmount > curMp){
				mpBar.fillAmount = curMp;
			}
		}	
		if(curMp < mpBar.fillAmount){
			mpBar.fillAmount -= 1 / 1 * Time.unscaledDeltaTime;
			if(mpBar.fillAmount < curMp){
				mpBar.fillAmount = curMp;
			}
		}

		//Shield Gauge
		if(stat.maxShieldPlus > 0){
			if(curShield > shieldBar.fillAmount){
				shieldBar.fillAmount += 1 / 1 * Time.unscaledDeltaTime;
				if(shieldBar.fillAmount > curShield){
					shieldBar.fillAmount = curShield;
				}
			}	
			if(curShield < shieldBar.fillAmount){
				shieldBar.fillAmount -= 1 / 1 * Time.unscaledDeltaTime;
				if(shieldBar.fillAmount < curShield){
					shieldBar.fillAmount = curShield;
				}
			}
		}
		
		//EXP Gauge
		if(expBar){
			expBar.fillAmount = curExp;
		}
		if(lvText){
			lvText.text = stat.level.ToString();
		}
		if(hpText){
			hpText.text = hp.ToString() + "/" + maxHp.ToString();
		}
		if(mpText){
			mpText.text = mp.ToString() + "/" + maxMp.ToString();
		}
		if(shieldText){
			shieldText.text = shield.ToString() + "/" + maxShield.ToString();
		}
	}
}