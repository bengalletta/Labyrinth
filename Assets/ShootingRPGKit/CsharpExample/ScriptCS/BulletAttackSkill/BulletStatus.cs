using UnityEngine;
using System.Collections;

public class BulletStatus : MonoBehaviour {
	public int damage = 10;
	public int damageMax = 20;
	
	private int playerAttack = 5;
	public int totalDamage = 0;
	public int variance = 15;
	public string shooterTag = "Player";
	[HideInInspector]
		public GameObject shooter;
	
	public Transform Popup;
	
	public GameObject hitEffect;
	public bool flinch = false;
	public bool penetrate = false;
	public bool ignoreGuard = false;
	private string popDamage = "";
	
	public AtkType AttackType = AtkType.Physic;
	
	public Elementala element = Elementala.Normal;

	public BombHit bombHitSetting;
	
	void Start (){
		gameObject.layer = 2;
		if(variance >= 100){
			variance = 100;
		}
		if(variance <= 1){
			variance = 1;
		}
		
	}
	
	public void Setting (int str , int mag , string tag , GameObject owner){
		if(AttackType == AtkType.Physic){
			playerAttack = str;
		}else{
			playerAttack = mag;
		}
		shooterTag = tag;
		shooter = owner;
		int varMin = 100 - variance;
		int varMax = 100 + variance;
		int randomDmg = Random.Range(damage, damageMax);
		totalDamage = (randomDmg + playerAttack) * Random.Range(varMin ,varMax) / 100;
	}
	
	void OnTriggerEnter ( Collider other  ){  	
		//When Player Shoot at Enemy		   
		if(shooterTag == "Player" && other.tag == "Enemy"){
			DamageToEnemy(other.transform);
			if(bombHitSetting.enable){
				ExplosionDamage();
			}
			//When Enemy Shoot at Player
		}else if(shooterTag == "Enemy" && other.tag == "Player" || shooterTag == "Enemy" && other.tag == "Ally"){  	
			DamageToPlayer(other.transform);
			if(bombHitSetting.enable){
				ExplosionDamage();
			}
		}else if(shooterTag == "Player" && other.tag == "WeakPoint"){ 
			DamageWeakPoint(other.transform);
			if(bombHitSetting.enable){
				ExplosionDamage();
			}
		}else if(other.tag == "Breakable"){ 
			DamageToEnemy(other.transform);
			if(bombHitSetting.enable){
				ExplosionDamage();
			}
		}
	}
	
	public void DamageToEnemy (Transform other){
		Transform dmgPop = Instantiate(Popup, other.transform.position , transform.rotation) as Transform;
		
		if(AttackType == AtkType.Physic){
			popDamage = other.GetComponent<Status>().OnDamage(totalDamage , (int)element , ignoreGuard);
		}else{
			popDamage = other.GetComponent<Status>().OnMagicDamage(totalDamage , (int)element , ignoreGuard);
		}
		
		if(shooter && shooter.GetComponent<ShowEnemyHealth>()){
			shooter.GetComponent<ShowEnemyHealth>().GetHP(other.GetComponent<Status>().maxHealth , other.gameObject , other.name);
		}
		dmgPop.GetComponent<DamagePopup>().damage = popDamage;	
		
		if(hitEffect){
			Instantiate(hitEffect, transform.position , transform.rotation);
		}
		if(flinch){
			other.GetComponent<Status>().Flinch();
		}
		if(!penetrate){
			Destroy (gameObject);
		}
	}
	
	public void DamageToPlayer (Transform other){
		if(AttackType == AtkType.Physic){
			popDamage = other.GetComponent<Status>().OnDamage(totalDamage , (int)element , ignoreGuard);
		}else{
			popDamage = other.GetComponent<Status>().OnMagicDamage(totalDamage , (int)element , ignoreGuard);
		}
		Transform dmgPop = Instantiate(Popup, transform.position , transform.rotation) as Transform;	
		
		dmgPop.GetComponent<DamagePopup>().damage = popDamage;
		
		if(hitEffect){
			Instantiate(hitEffect, transform.position , transform.rotation);
		}
		if(flinch){
			other.GetComponent<Status>().Flinch();
		}
		if(!penetrate){
			Destroy (gameObject);
		}
	}
	
	public void ExplosionDamage (){
		Collider[] hitColliders= Physics.OverlapSphere(transform.position, bombHitSetting.bombRadius);
		if(bombHitSetting.bombEffect){
			Instantiate(bombHitSetting.bombEffect , transform.position , transform.rotation);
		}
		
		for (int i= 0; i < hitColliders.Length; i++) {
			if(shooterTag == "Player" && hitColliders[i].tag == "Enemy"){	  
				DamageToEnemy(hitColliders[i].transform);
			}else if(shooterTag == "Enemy" && hitColliders[i].tag == "Player" || shooterTag == "Enemy" && hitColliders[i].tag == "Ally"){  	
				DamageToPlayer(hitColliders[i].transform);
			}
		}
	}
	
	public void DamageWeakPoint ( Transform mon  ){
		if(!mon.GetComponent<WeakPoint>()){
			return;
		}
		Transform other = mon.GetComponent<WeakPoint>().master;
		int realDamage = totalDamage * mon.GetComponent<WeakPoint>().damagePercent / 100;
		bool  igg = mon.GetComponent<WeakPoint>().ignoreGuard;
		
		Transform dmgPop = Instantiate(Popup, other.transform.position , transform.rotation) as Transform;
		
		if(AttackType == AtkType.Physic){
			popDamage = other.GetComponent<Status>().OnDamage(realDamage , (int)element , igg);
		}else{
			popDamage = other.GetComponent<Status>().OnMagicDamage(realDamage , (int)element , igg);
		}
		
		if(shooter && shooter.GetComponent<ShowEnemyHealth>()){
			shooter.GetComponent<ShowEnemyHealth>().GetHP(other.GetComponent<Status>().maxHealth , other.gameObject , other.name);
		}
		dmgPop.GetComponent<DamagePopup>().damage = popDamage;
		dmgPop.GetComponent<DamagePopup>().critical = mon.GetComponent<WeakPoint>().isCritical;
		
		if(hitEffect){
			Instantiate(hitEffect, transform.position , transform.rotation);
		}
		if(flinch){
			other.GetComponent<Status>().Flinch();
		}
		if(!penetrate){
			Destroy (gameObject);
		}
		
	}
	
}

public enum AtkType {
	Physic = 0,
	Magic = 1,
}
public enum Elementala{
	Normal = 0,
	Fire = 1,
	Ice = 2,
	Earth = 3,
	Lightning = 4,
}
[System.Serializable]
public class BombHit{
	public bool enable = false;
	public GameObject bombEffect;
	public float bombRadius = 20;
}