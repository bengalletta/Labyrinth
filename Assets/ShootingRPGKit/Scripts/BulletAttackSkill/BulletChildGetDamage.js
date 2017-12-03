#pragma strict
var master : GameObject;
var damagePercent : int = 100;

function Start () {
	if(!master){
		master = transform.root.gameObject;
	}
	var dmg : int = master.GetComponent(BulletStatus).totalDamage;
	
	if(damagePercent != 100){
		dmg = dmg * damagePercent / 100;
	}
	GetComponent(BulletStatus).totalDamage = dmg;
	GetComponent(BulletStatus).shooterTag = master.GetComponent(BulletStatus).shooterTag;
	GetComponent(BulletStatus).shooter = master.GetComponent(BulletStatus).shooter;
	
}
