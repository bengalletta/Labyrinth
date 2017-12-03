#pragma strict
private var master : GameObject;
var button : Texture2D;
var hitEffect : GameObject;
var popup : Transform;
var showMeleeWeapon : boolean = true;
private var enter : boolean = false;
private var attacking : boolean = false;
private var player : GameObject;

var damageMultiply : float = 10.0;
var playerAnimation : AnimationClip;
var enemyAnimation : AnimationClip;
var enemyAnimationDelay : float = 0.5;

private var useMecanim : boolean = true;
private var useMecanimMon : boolean = true;
private var ai : AIenemy;

function Start () {
	if(!master){
		master = transform.root.gameObject;
	}
	ai = master.GetComponent(AIenemy);
	useMecanimMon = ai.useMecanim;
}

function Update () {
	if(Input.GetKeyDown("e") && enter && !attacking && ai.followState == AIState.Idle){
		Attacking();
	}
}

function OnGUI(){
	if(!player){
		return;
	}
	
	if(enter){
		GUI.DrawTexture(Rect(Screen.width / 2 - 145, Screen.height - 180, 290, 80), button);
	}
}

function OnTriggerEnter (other : Collider) {
	if (other.gameObject.tag == "Player" && ai.followState == AIState.Idle) {
		player = other.gameObject;
		enter = true;
	}
	
}

function OnTriggerExit  (other : Collider) {
	if (other.gameObject == player) {
		enter = false;
	}
}

function Attacking(){
	if(attacking){
		return;
	}
	attacking = true;
	useMecanim = player.GetComponent(GunTrigger).useMecanim;
	player.GetComponent(Status).freeze = true;
	master.GetComponent(Status).freeze = true;
	if(showMeleeWeapon){
		player.GetComponent(GunTrigger).ShowMeleeWeapon(true);
	}
	
	if(!useMecanim && playerAnimation){
		//For Legacy Animation
		player.GetComponent(Status).mainModel.GetComponent.<Animation>()[playerAnimation.name].layer = 18;
		player.GetComponent(Status).mainModel.GetComponent.<Animation>().PlayQueued(playerAnimation.name, QueueMode.PlayNow);
	}else if(playerAnimation){
		//For Mecanim Animation
		player.GetComponent(PlayerMecanimAnimation).AttackAnimation(playerAnimation.name);
	}
	yield WaitForSeconds(enemyAnimationDelay);
	
	if(!useMecanimMon && enemyAnimation){
		//For Legacy Animation
		master.GetComponent(Status).mainModel.GetComponent.<Animation>()[enemyAnimation.name].layer = 18;
		master.GetComponent(Status).mainModel.GetComponent.<Animation>().PlayQueued(enemyAnimation.name, QueueMode.PlayNow);
	}else if(enemyAnimation){
		//For Mecanim Animation
		master.GetComponent(AIenemy).animator.Play(enemyAnimation.name);
	}
	
	if(showMeleeWeapon){
		player.GetComponent(GunTrigger).ShowMeleeWeapon(false);
	}
	
	player.GetComponent(Status).freeze = false;
	master.GetComponent(Status).freeze = false;
	attacking = false;
	CalculateDamage();
	
}

function CalculateDamage(){
		var dmgPop : Transform = Instantiate(popup, master.transform.position , transform.rotation);
    	var totalDamage : int = player.GetComponent(Status).melee * damageMultiply;

		var popDamage : String = master.GetComponent(Status).OnDamage(totalDamage , 0 , true);

		if(player && player.GetComponent(ShowEnemyHealth)){
    		player.GetComponent(ShowEnemyHealth).GetHP(master.GetComponent(Status).maxHealth , master.gameObject , master.name);
    	}
		dmgPop.GetComponent(DamagePopup).damage = popDamage;	
		
		if(hitEffect){
    		Instantiate(hitEffect, transform.position , transform.rotation);
 		}
}

