using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Status))]
public class AIfriend : MonoBehaviour {
	private enum AIStatef { Moving = 0, Pausing = 1 , Escape = 2 , Idle = 3, FollowMaster = 4}
	public Transform master;
	
	private GameObject mainModel;
	public bool  useMecanim = false;
	public Animator animator; //For Mecanim
	public Transform followTarget;
	public float approachDistance = 3.0f;
	public float detectRange = 15.0f;
	public float lostSight = 100.0f;
	public float speed = 4.0f;
	public AnimationClip movingAnimation;
	public AnimationClip idleAnimation;
	public AnimationClip[] attackAnimation = new AnimationClip[1];
	public AnimationClip hurtAnimation;

	private bool  flinch = false;
	public bool stability = false;

	public bool freeze = false;
	
	public Transform attackPrefab;
	public Transform attackPoint;
	public GameObject attackEffect;
	
	public float attackCast = 0.5f;
	public float attackDelay = 1.0f;
	private int continueAttack = 1;
	public float continueAttackDelay = 0.8f;
	
	private AIStatef followState;
	private float distance = 30.0f;
	private float masterDistance = 30.0f;
	private int atk = 0;
	private int mag = 0;
	
	private bool cancelAttack = false;
	private bool meleefwd = false;
	
	public enum AIatkType {
		Immobile = 0,
		MeleeDash = 1,
	}
	
	public AIatkType attackType = AIatkType.Immobile;
	
	public AudioClip[] attackVoice = new AudioClip[3];
	public AudioClip hurtVoice;
	public float attackSoundRadius = 0; // Can attract the enemy to gun fire position.
	
	public bool  usePathfinding = false; //Require Nav Mesh Agent
	/*float rayLength = 1.0f; // Use with Path Finding
float rotateSpeed = 4.0f; // Use with Path Finding*/
	
	void  Start (){
		gameObject.tag = "Ally"; 
		mainModel = GetComponent<Status>().mainModel;
		if(!mainModel){
			mainModel = this.gameObject;
		}
		if(!master){
			print("Please Assign It's Master first");
		}
		
		if(!attackPoint){
			attackPoint = this.transform;
		}
		GetComponent<Status>().useMecanim = useMecanim;
		
		continueAttack = attackAnimation.Length;
		atk = GetComponent<Status>().atk;
		mag = GetComponent<Status>().matk;
		
		followState = AIStatef.FollowMaster;
		
		if(!useMecanim){
			//If using Legacy Animation
			mainModel.GetComponent<Animation>().Play(movingAnimation.name);
			if(hurtAnimation){
				mainModel.GetComponent<Animation>()[hurtAnimation.name].layer = 10;
				GetComponent<Status>().hurt = hurtAnimation;
			}
		}else{
			//If using Mecanim Animation
			if(!animator){
				animator = mainModel.GetComponent<Animator>();
			}
			animator.SetBool("run" , true);
		}
		
		Physics.IgnoreCollision(GetComponent<Collider>(), master.GetComponent<Collider>());
		if(hurtVoice){
			GetComponent<Status>().hurtVoice = hurtVoice;
		}
	}
	
	Vector3 GetDestination (){
		Vector3 destination = followTarget.position;
		destination.y = transform.position.y;
		return destination;
	}
	
	Vector3 GetMasterPosition (){
		if(!master){
			return Vector3.zero;
		}
		Vector3 destination = master.position;
		destination.y = transform.position.y;
		return destination;
	}
	
	void FixedUpdate(){
		CharacterController controller = GetComponent<CharacterController>();
		Status stat = GetComponent<Status>();
		if(!master){
			stat.Death();
			return;
		}
		if (meleefwd && !stat.freeze){
			Vector3 lui = transform.TransformDirection(Vector3.forward);
			controller.Move(lui * 5 * Time.deltaTime);
			return;
		}
		if(freeze || stat.freeze){
			return;
		}
		if(useMecanim){
			animator.SetBool("hurt" , stat.flinch);
		}
		if (stat.flinch){
			cancelAttack = true;
			Vector3 lui = transform.TransformDirection(Vector3.back);
			controller.SimpleMove(lui * 5);
			return;
		}
		
		FindClosest();
		
		if (followState == AIStatef.FollowMaster) {
			//---------------------------------
			if ((master.position - transform.position).magnitude <= 2.0f) {
				followState = AIStatef.Idle;
				//mainModel.animation.CrossFade(idleAnimation.name, 0.2ff);
				if(!useMecanim){
					//If using Legacy Animation
					mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f); 
				}else{
					animator.SetBool("run" , false);
				}
			}else {
				if(usePathfinding){
					PathFinding(master);
				}else{
					Vector3 forward = transform.TransformDirection(Vector3.forward);
					Vector3 mas = master.position;
					mas.y = transform.position.y;
					transform.LookAt(mas);
					controller.Move(forward * speed * Time.deltaTime);
				}
				
			}
			
			//---------------------------------
		}else if (followState == AIStatef.Moving) {
			masterDistance = (transform.position - GetMasterPosition()).magnitude;
			if (masterDistance > 7.0f){//////////////////GetMasterPosition
				followState = AIStatef.FollowMaster;
				//mainModel.animation.CrossFade(movingAnimation.name, 0.2ff);
				if(!useMecanim){
					//If using Legacy Animation
					mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
				}else{
					animator.SetBool("run" , true);
				}
			}else if ((followTarget.position - transform.position).magnitude <= approachDistance) {
				followState = AIStatef.Pausing;
				//mainModel.animation.CrossFade(idleAnimation.name, 0.2ff);
				if(!useMecanim){
					//If using Legacy Animation
					mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f); 
				}else{
					animator.SetBool("run" , false);
				}
				//----Attack----
				if(attackPrefab){
					//Attack();
					StartCoroutine(Attack());
				}
			}else if ((followTarget.position - transform.position).magnitude >= lostSight)
			{//Lost Sight
				GetComponent<Status>().health = GetComponent<Status>().maxHealth;
				followState = AIStatef.Idle;
				//mainModel.animation.CrossFade(idleAnimation.name, 0.2ff);
				if(!useMecanim){
					//If using Legacy Animation
					mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f); 
				}else{
					animator.SetBool("run" , false);
				}
			}else {
				Vector3 forward = transform.TransformDirection(Vector3.forward);
				//controller.Move(forward * speed * Time.deltaTime);
				if(usePathfinding && (followTarget.position - transform.position).magnitude >= 5){
					PathFinding(followTarget);
				}else{
					Vector3 destiny = followTarget.position;
					destiny.y = transform.position.y;
					transform.LookAt(destiny);
					controller.Move(forward * speed * Time.deltaTime);
				}
				
			}
		}
		else if (followState == AIStatef.Pausing){
			Vector3 destinya = followTarget.position;
			destinya.y = transform.position.y;
			transform.LookAt(destinya);
			
			distance = (transform.position - GetDestination()).magnitude;
			masterDistance = (transform.position - GetMasterPosition()).magnitude;
			if (masterDistance > 12.0f){//////////////////GetMasterPosition
				followState = AIStatef.FollowMaster;
				//mainModel.animation.CrossFade(movingAnimation.name, 0.2ff);
				if(!useMecanim){
					//If using Legacy Animation
					mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
				}else{
					animator.SetBool("run" , true);
				}
			}else if (distance > approachDistance) {
				followState = AIStatef.Moving;
				//mainModel.animation.CrossFade(movingAnimation.name, 0.2ff);
				if(!useMecanim){
					//If using Legacy Animation
					mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
				}else{
					animator.SetBool("run" , true);
				}
			}
		}
		//----------------Idle Mode--------------
		else if (followState == AIStatef.Idle){
			Vector3 destinyheight = followTarget.position;
			destinyheight.y = transform.position.y - destinyheight.y;

			distance = (transform.position - GetDestination()).magnitude;
			masterDistance = (transform.position - GetMasterPosition()).magnitude;
			if (distance < detectRange && Mathf.Abs(destinyheight.y) <= 4 && followTarget){
				followState = AIStatef.Moving;
				//mainModel.animation.CrossFade(movingAnimation.name, 0.2ff);
				if(!useMecanim){
					//If using Legacy Animation
					mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
				}else{
					animator.SetBool("run" , true);
				}
			}else if (masterDistance > 3.0f){//////////////////GetMasterPosition
				followState = AIStatef.FollowMaster;
				//mainModel.animation.CrossFade(movingAnimation.name, 0.2ff);
				if(!useMecanim){
					//If using Legacy Animation
					mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
				}else{
					animator.SetBool("run" , true);
				}
			}
		}
		//-----------------------------------
	}
	
	IEnumerator Attack(){
		Status stat = GetComponent<Status>();
		atk = GetComponent<Status>().atk;
		mag = GetComponent<Status>().matk;
		Transform bulletShootout;
		cancelAttack = false;
		int c = 0;
		if(flinch){
			yield break;
		}
		while (c < continueAttack && followTarget){
			freeze = true;
			if(attackType == AIatkType.MeleeDash){
				StartCoroutine(MeleeDash());
			}
			if(followTarget){
				Vector3 destiny = followTarget.position;
				destiny.y = transform.position.y;
				transform.LookAt(destiny);
			}
			
			if(!useMecanim){
				//If using Legacy Animation
				mainModel.GetComponent<Animation>().PlayQueued(attackAnimation[c].name, QueueMode.PlayNow);
			}else{
				animator.Play(attackAnimation[c].name);
			}
			
			yield return new WaitForSeconds(attackCast);
			if(flinch || stat.freeze){
				freeze = false;
				c = continueAttack;
				yield break;
			}
			//attackPoint.transform.LookAt(followTarget);
			if(!cancelAttack || GetComponent<Status>().freeze){
				if(attackVoice.Length > c && attackVoice[c]){
					GetComponent<AudioSource>().PlayOneShot(attackVoice[c]);
				}
				if(attackSoundRadius > 0){
					GunSoundRadius(attackSoundRadius);
				}
				if(attackEffect){
					GameObject eff = Instantiate(attackEffect, attackPoint.transform.position , attackPoint.transform.rotation) as GameObject;
					eff.transform.parent = attackPoint.transform;
				}
				bulletShootout = Instantiate(attackPrefab, attackPoint.transform.position , attackPoint.transform.rotation) as Transform;
				bulletShootout.GetComponent<BulletStatus>().Setting(atk , mag , "Player" , this.gameObject);
				c++;
				yield return new WaitForSeconds(continueAttackDelay);
				//print(c);
				//yield return new WaitForSeconds(attackDelay);
			}else{
				freeze = false;
				c = continueAttack;
			}
		}
		yield return new WaitForSeconds(attackDelay);
		//yield return new WaitForSeconds(attackDelay);
		c = 0;
		freeze = false;
		//mainModel.animation.CrossFade(movingAnimation.name, 0.2ff);
		CheckDistance();
		
	}
	
	void CheckDistance(){
		masterDistance = (transform.position - GetMasterPosition()).magnitude;
		if (masterDistance > 7.0f){//////////////////GetMasterPosition
			followState = AIStatef.FollowMaster;
			if(!useMecanim){
				//If using Legacy Animation
				mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
			}else{
				animator.SetBool("run" , true);
			}
			return;
		}
		if(!followTarget){
			if(!useMecanim){
				//If using Legacy Animation
				mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f); 
			}else{
				animator.SetBool("run" , false);
			}
			followState = AIStatef.Idle;
			return;
		}
		float distancea = (followTarget.position - transform.position).magnitude;
		if (distancea <= approachDistance){
			Vector3 destinya = followTarget.position;
			destinya.y = transform.position.y;
			transform.LookAt(destinya);
			Attack();
		}else{
			followState = AIStatef.Moving;
			if(!useMecanim){
				//If using Legacy Animation
				mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
			}else{
				animator.SetBool("run" , true);
			}
		}
	}
	
	
	//GameObject FindClosest (){ 
	void FindClosest(){ 
		// Find Closest Player   
		GameObject[] gos; 
		gos = GameObject.FindGameObjectsWithTag("Enemy"); 
		if(gos.Length <= 0){
			return;
		}
		GameObject closest = null; 
		
		float distance = Mathf.Infinity; 
		Vector3 position = transform.position; 
		
		foreach(GameObject go in gos) { 
			Vector3 diff = (go.transform.position - position); 
			float curDistance = diff.sqrMagnitude; 
			if (curDistance < distance) { 
				//------------
				closest = go; 
				distance = curDistance; 
			} 
		} 
		// target = closest;
		if(!closest){
			followTarget = null;
			followState = AIStatef.FollowMaster;
			//mainModel.animation.CrossFade(movingAnimation.name, 0.2ff);
			if(!useMecanim){
				//If using Legacy Animation
				mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
			}else{
				animator.SetBool("run" , true);
			}
			return;
		}
		followTarget = closest.transform;
		//return closest; 
	}
	
	IEnumerator MeleeDash(){
		meleefwd = true;
		yield return new WaitForSeconds(0.2f);
		meleefwd = false;
	}
	
	void  GunSoundRadius ( float radius  ){
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
		
		for (int i= 0; i < hitColliders.Length; i++) {
			if(hitColliders[i].tag == "Enemy"){	  
				hitColliders[i].SendMessage("SetDestination" , transform.position);
			}
		}
	}

	void PathFinding ( Transform target  ){
		//Require Nav Mesh Agent.
		UnityEngine.AI.NavMeshAgent agent;
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		//agent.destination = target.position; 
		agent.SetDestination (target.position);
	}
}