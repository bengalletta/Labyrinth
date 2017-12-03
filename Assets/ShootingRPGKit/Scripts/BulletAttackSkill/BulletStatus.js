#pragma strict
var damage : int = 10;
var damageMax : int = 20;

private var playerAttack : int = 5;
var totalDamage : int = 0;
var variance : int = 15;
var shooterTag : String = "Player";
@HideInInspector
var shooter : GameObject;

var Popup : Transform;

var hitEffect : GameObject;
var flinch : boolean = false;
var penetrate : boolean = false;
var ignoreGuard : boolean = false;
private var popDamage : String = "";

enum AtkType {
	Physic = 0,
	Magic = 1,
}

var AttackType : AtkType = AtkType.Physic;

enum Elementala{
	Normal = 0,
	Fire = 1,
	Ice = 2,
	Earth = 3,
	Lightning = 4,
}
var element : Elementala = Elementala.Normal;

class BombHit{
	var enable : boolean = false;
	var bombEffect : GameObject;
	var bombRadius : float = 20;
}
var bombHitSetting : BombHit;

function Start(){
	gameObject.layer = 2;
	if(variance >= 100){
		variance = 100;
	}
	if(variance <= 1){
		variance = 1;
	}

}

function Setting(str : int , mag : int , tag : String , owner : GameObject){
	if(AttackType == AtkType.Physic){
		playerAttack = str;
	}else{
		playerAttack = mag;
	}
	shooterTag = tag;
	shooter = owner;
	var varMin : int = 100 - variance;
	var varMax : int = 100 + variance;
	var randomDmg : int = Random.Range(damage, damageMax);
	totalDamage = (randomDmg + playerAttack) * Random.Range(varMin ,varMax) / 100;
}

function OnTriggerEnter (other : Collider) {  	
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

function DamageToEnemy(other : Transform){
		var dmgPop : Transform = Instantiate(Popup, other.transform.position , transform.rotation);
    	
		if(AttackType == AtkType.Physic){
			popDamage = other.GetComponent(Status).OnDamage(totalDamage , parseInt(element) , ignoreGuard);
		}else{
			popDamage = other.GetComponent(Status).OnMagicDamage(totalDamage , parseInt(element) , ignoreGuard);
		}

		if(shooter && shooter.GetComponent(ShowEnemyHealth)){
    		shooter.GetComponent(ShowEnemyHealth).GetHP(other.GetComponent(Status).maxHealth , other.gameObject , other.name);
    	}
		dmgPop.GetComponent(DamagePopup).damage = popDamage;	
		
		if(hitEffect){
    		Instantiate(hitEffect, transform.position , transform.rotation);
 		}
 		if(flinch){
 		 	other.GetComponent(Status).Flinch();
 		}
		if(!penetrate){
 		 	 Destroy (gameObject);
 		}
}

function DamageToPlayer(other : Transform){
		if(AttackType == AtkType.Physic){
			popDamage = other.GetComponent(Status).OnDamage(totalDamage , parseInt(element) , ignoreGuard);
		}else{
			popDamage = other.GetComponent(Status).OnMagicDamage(totalDamage , parseInt(element) , ignoreGuard);
		}
		var dmgPop : Transform = Instantiate(Popup, transform.position , transform.rotation);	

		dmgPop.GetComponent(DamagePopup).damage = popDamage;
  		  
  		 if(hitEffect){
    		Instantiate(hitEffect, transform.position , transform.rotation);
 		 }
 		 if(flinch){
 		 	other.GetComponent(Status).Flinch();
 		 }
 		 if(!penetrate){
 		 	 Destroy (gameObject);
 		 }
}

function ExplosionDamage(){
		var hitColliders = Physics.OverlapSphere(transform.position, bombHitSetting.bombRadius);
		if(bombHitSetting.bombEffect){
    		Instantiate(bombHitSetting.bombEffect , transform.position , transform.rotation);
 		}
 		 
		for (var i = 0; i < hitColliders.Length; i++) {
			if(shooterTag == "Player" && hitColliders[i].tag == "Enemy"){	  
		    	DamageToEnemy(hitColliders[i].transform);
		    }else if(shooterTag == "Enemy" && hitColliders[i].tag == "Player" || shooterTag == "Enemy" && hitColliders[i].tag == "Ally"){  	
				DamageToPlayer(hitColliders[i].transform);
		    }
		}
}

function DamageWeakPoint(mon : Transform){
	if(!mon.GetComponent(WeakPoint)){
		return;
	}
	var other : Transform = mon.GetComponent(WeakPoint).master;
	var realDamage : int = totalDamage * mon.GetComponent(WeakPoint).damageMultiply;
	var igg : boolean = mon.GetComponent(WeakPoint).ignoreGuard;
	
	var dmgPop : Transform = Instantiate(Popup, other.transform.position , transform.rotation);
    	
		if(AttackType == AtkType.Physic){
			popDamage = other.GetComponent(Status).OnDamage(realDamage , parseInt(element) , igg);
		}else{
			popDamage = other.GetComponent(Status).OnMagicDamage(realDamage , parseInt(element) , igg);
		}

		if(shooter && shooter.GetComponent(ShowEnemyHealth)){
    		shooter.GetComponent(ShowEnemyHealth).GetHP(other.GetComponent(Status).maxHealth , other.gameObject , other.name);
    	}
		dmgPop.GetComponent(DamagePopup).damage = popDamage;
		dmgPop.GetComponent(DamagePopup).critical = mon.GetComponent(WeakPoint).isCritical;
		
		if(hitEffect){
    		Instantiate(hitEffect, transform.position , transform.rotation);
 		}
 		if(flinch){
 		 	other.GetComponent(Status).Flinch();
 		}
		if(!penetrate){
 		 	 Destroy (gameObject);
 		}

}
	