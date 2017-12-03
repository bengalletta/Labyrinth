using UnityEngine;
using System.Collections;

public class StealthKillArea : MonoBehaviour {
	private GameObject master;
	public Texture2D button;
	public GameObject hitEffect;
	public Transform popup;
	public bool showMeleeWeapon = true;
	private bool enter = false;
	private bool attacking = false;
	private GameObject player;
	
	public int damagePercent = 1500;
	public AnimationClip playerAnimation;
	public AnimationClip enemyAnimation;
	public float enemyAnimationDelay = 0.5f;
	
	private bool useMecanim = true;
	private bool useMecanimMon = true;
	private AIenemy ai;
	
	void Start (){
		if(!master){
			master = transform.root.gameObject;
		}
		ai = master.GetComponent<AIenemy>();
		useMecanimMon = ai.useMecanim;
	}
	
	void Update (){
		if(Input.GetKeyDown("e") && enter && !attacking && ai.followState == AIState.Idle){
			//Attacking();
			StartCoroutine(Attacking());
		}
	}
	
	void OnGUI (){
		if(!player){
			return;
		}
		
		if(enter){
			GUI.DrawTexture( new Rect(Screen.width / 2 - 145, Screen.height - 180, 290, 80), button);
		}
	}
	
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player" && ai.followState == AIState.Idle) {
			player = other.gameObject;
			enter = true;
		}
		
	}
	
	void OnTriggerExit(Collider other){
		if (other.gameObject == player){
			enter = false;
		}
	}
	
	IEnumerator Attacking(){
		if(attacking){
			yield break;
		}
		attacking = true;
		useMecanim = player.GetComponent<GunTrigger>().useMecanim;
		player.GetComponent<Status>().freeze = true;
		master.GetComponent<Status>().freeze = true;
		if(showMeleeWeapon){
			player.GetComponent<GunTrigger>().ShowMeleeWeapon(true);
		}
		
		if(!useMecanim && playerAnimation){
			//For Legacy Animation
			player.GetComponent<Status>().mainModel.GetComponent<Animation>()[playerAnimation.name].layer = 18;
			player.GetComponent<Status>().mainModel.GetComponent<Animation>().PlayQueued(playerAnimation.name, QueueMode.PlayNow);
		}else if(playerAnimation){
			//For Mecanim Animation
			player.GetComponent<PlayerMecanimAnimation>().AttackAnimation(playerAnimation.name);
		}
		yield return new WaitForSeconds(enemyAnimationDelay);
		
		if(!useMecanimMon && enemyAnimation){
			//For Legacy Animation
			master.GetComponent<Status>().mainModel.GetComponent<Animation>()[enemyAnimation.name].layer = 18;
			master.GetComponent<Status>().mainModel.GetComponent<Animation>().PlayQueued(enemyAnimation.name, QueueMode.PlayNow);
		}else if(enemyAnimation){
			//For Mecanim Animation
			master.GetComponent<AIenemy>().animator.Play(enemyAnimation.name);
		}
		
		if(showMeleeWeapon){
			player.GetComponent<GunTrigger>().ShowMeleeWeapon(false);
		}
		
		player.GetComponent<Status>().freeze = false;
		master.GetComponent<Status>().freeze = false;
		attacking = false;
		CalculateDamage();
		
	}
	
	void  CalculateDamage (){
		Transform dmgPop = Instantiate(popup, master.transform.position , transform.rotation) as Transform;
		int totalDamage = player.GetComponent<Status>().melee * damagePercent / 100;
		
		string popDamage = master.GetComponent<Status>().OnDamage(totalDamage , 0 , true);
		
		if(player && player.GetComponent<ShowEnemyHealth>()){
			player.GetComponent<ShowEnemyHealth>().GetHP(master.GetComponent<Status>().maxHealth , master.gameObject , master.name);
		}
		dmgPop.GetComponent<DamagePopup>().damage = popDamage;	
		
		if(hitEffect){
			Instantiate(hitEffect, transform.position , transform.rotation);
		}
	}
	
	
}