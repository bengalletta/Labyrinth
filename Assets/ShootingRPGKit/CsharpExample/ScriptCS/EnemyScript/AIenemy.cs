using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Status))]
[RequireComponent (typeof(CharacterMotorC))]
[AddComponentMenu("Shooting-RPG Kit(CS)/Create Player(Legacy)")]
public class AIenemy : MonoBehaviour {
	
	private GameObject mainModel;
	public bool useMecanim = false;
	public Animator animator; //For Mecanim
	public Transform followTarget;
	public float approachDistance = 2.0f;
	public float detectRange = 15.0f;
	//float detectRadius = 20.0ff;
	public float lostSight = 100.0f;
	public float speed = 4.0f;
	public bool canHearGunSound = true;
	public float moveToPointDuration = 5.0f; //Use when enemy hearing gun sound. It will done it's move when it cannot reach that point in time.
	public AnimationClip movingAnimation;
	public AnimationClip idleAnimation;
	public AnimationClip attackAnimation;
	public AnimationClip hurtAnimation;
	
	[HideInInspector]
	public bool flinch = false;
	
	public bool freeze = false;
	
	public GameObject bulletPrefab;
	public Transform attackPoint;
	public GameObject attackEffect;
	
	public float attackCast = 0.3f;
	public float attackDelay = 0.5f;
	
	public AIState followState = AIState.Idle;
	private float distance = 0.0f;
	private int atk = 0;
	private int matk = 0;
	[HideInInspector]
	public bool cancelAttack = false;
	private bool  attacking = false;
	private bool  castSkill = false;
	private GameObject[] gos; 
	
	public AudioClip attackVoice;
	public AudioClip hurtVoice;
	public float attackSoundRadius = 0; // Can attract the enemy to gun fire position.
	private Vector3 moveToPoint;
	private float moveEnough = 0.0f;
	public bool usePathfinding = false; //Require Nav Mesh Agent
	private UnityEngine.AI.NavMeshAgent agent;
	
	void Start (){
		gameObject.tag = "Enemy"; 
		if(!attackPoint){
			//attackPoint = this.transform;
			attackPoint = new GameObject().transform;
			attackPoint.position = transform.position;
			attackPoint.parent = this.transform;
		}
		mainModel = GetComponent<Status>().mainModel;
		if(!mainModel){
			mainModel = this.gameObject;
		}
		GetComponent<Status>().useMecanim = useMecanim;
		//Assign MainModel in Status Script
		GetComponent<Status>().mainModel = mainModel;
		//Set ATK = Monster's Status
		atk = GetComponent<Status>().atk;
		matk = GetComponent<Status>().matk;
		
		followState = AIState.Idle;
		
		if(!useMecanim){
			//If using Legacy Animation
			mainModel.GetComponent<Animation>().Play(idleAnimation.name);
			GetComponent<Status>().hurt = hurtAnimation;
		}else{
			//If using Mecanim Animation
			if(!animator){
				animator = mainModel.GetComponent<Animator>();
			}
		}
		
		if(hurtVoice){
			GetComponent<Status>().hurtVoice = hurtVoice;
		}
	}
	
	Vector3 GetDestination(){
		Vector3 destination = followTarget.position;
		destination.y = transform.position.y;
		return destination;
	}
	
	void Update(){
		Status stat = GetComponent<Status>();
		CharacterController controller = GetComponent<CharacterController>();

		FindClosestEnemy();
		/*gos = GameObject.FindGameObjectsWithTag("Player"); 
		if(gos.Length > 0) {
			followTarget = FindClosest().transform;
		}*/
		if(useMecanim){
			animator.SetBool("hurt" , stat.flinch);
		}
		
		if(stat.flinch){
			cancelAttack = true;
			Vector3 knock = transform.TransformDirection(Vector3.back);
			controller.Move(knock * 5* Time.deltaTime);
			followState = AIState.Moving;
			return;
		}
		
		if(freeze || stat.freeze){
			return;
		}

		if(followState != AIState.MoveToPoint && !followTarget){
			return;
		}
		
		if(usePathfinding){
			agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		}
		//-----------------------------------
		if(followState == AIState.MoveToPoint){
			//---------Move To Point---------------
			//moveToPoint
			if(!useMecanim){
				//If using Legacy Animation
				mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
			}else{
				animator.SetBool("run" , true);
			}
			if(usePathfinding){
				PathFinding(moveToPoint);
			}else{
				Vector3 forward = transform.TransformDirection(Vector3.forward);
				controller.Move(forward * speed * Time.deltaTime);
			}
			
			Vector3 destination = moveToPoint;
			destination.y = transform.position.y;
			transform.LookAt(destination);

			if(followTarget){
				distance = (transform.position - GetDestination()).magnitude;
			}

			float dist2 = (transform.position - destination).magnitude;
			
			int getHealth = GetComponent<Status>().maxHealth - GetComponent<Status>().health;
			if(distance < detectRange || getHealth > 0){
				followState = AIState.Moving;
			}else if(dist2 <= 1.5f || Time.time > moveEnough){
				followState = AIState.Idle;
				if(!useMecanim){
					//If using Legacy Animation
					mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f); 
				}else{
					animator.SetBool("run" , false);
				}
			}
		}else if(followState == AIState.Moving){
			Vector3 lookTo = followTarget.position;
			lookTo.y = transform.position.y;
			transform.LookAt(lookTo);
			if(!useMecanim){
				//If using Legacy Animation
				mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
			}else{
				animator.SetBool("run" , true);
			}
			if(!usePathfinding && (followTarget.position - transform.position).magnitude <= approachDistance || usePathfinding && agent.hasPath && agent.remainingDistance <= agent.stoppingDistance + 0.1f && (followTarget.position - transform.position).magnitude <= approachDistance){
				followState = AIState.Pausing;
				if(!useMecanim){
					//If using Legacy Animation
					mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f); 
				}else{
					animator.SetBool("run" , false);
				}
				//----Attack----
				//Attack();
			}else if((followTarget.position - transform.position).magnitude >= lostSight){
				//Lost Sight
				GetComponent<Status>().health = GetComponent<Status>().maxHealth;
				if(!useMecanim){
					//If using Legacy Animation
					mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f); 
				}else{
					animator.SetBool("run" , false);
				}
				followState = AIState.Idle;
			}else{
				if(usePathfinding){
					PathFinding(followTarget.position);
				}else{
					Vector3 forward = transform.TransformDirection(Vector3.forward);
					controller.Move(forward * speed * Time.deltaTime);
				}
			}
		}else if(followState == AIState.Pausing){
			StartCoroutine(Attack());
			/*if(!useMecanim){
                //If using Legacy Animation
                	mainModel.animation.CrossFade(idleAnimation.name, 0.2ff); 
                }else{
					animator.SetBool("run" , false);
				}*/
			Vector3 destination = followTarget.position;
			destination.y = transform.position.y;
			transform.LookAt(destination);
			
			distance = (transform.position - GetDestination()).magnitude;
			if (distance > approachDistance) {
				followState = AIState.Moving;
			}
		}else if(followState == AIState.Idle){
			//----------------Idle Mode--------------
			//mainModel.animation.CrossFade(idleAnimation.name, 0.2ff);
			Vector3 destinationheight = followTarget.position;
			destinationheight.y = transform.position.y - destinationheight.y;
			int getHealth = GetComponent<Status>().maxHealth - GetComponent<Status>().health;
			
			distance = (transform.position - GetDestination()).magnitude;
			if(distance < detectRange && Mathf.Abs(destinationheight.y) <= 4 || getHealth > 0){
				followState = AIState.Moving;
			}
		}
		//-----------------------------------
	}
	
	void SetDestination(Vector3 des){
		if(!canHearGunSound || followState == AIState.Moving || followState == AIState.Pausing){
			return;
		}
		moveToPoint = des;
		moveEnough = Time.time + moveToPointDuration;
		followState = AIState.MoveToPoint;
	}
	
	public IEnumerator Attack(){
		RaycastHit hit;
		if (Physics.Linecast (transform.position, followTarget.position , out hit)) {
			if(hit.transform.tag == "Wall"){
				if(usePathfinding){
					PathFinding(followTarget.position);
				}
				followState = AIState.Pausing;
				yield break;
			}
		}
		cancelAttack = false;
		if(GetComponent<Status>().flinch || GetComponent<Status>().freeze || freeze || attacking){
			yield break;
		}
		//Set ATK = Monster's Status
		atk = GetComponent<Status>().atk;
		matk = GetComponent<Status>().matk;
		
		freeze = true;
		attacking = true;
		if(attackAnimation){
			if(!useMecanim){
				//If using Legacy Animation
				//mainModel.animation[attackAnimation.name].layer = 5;
				mainModel.GetComponent<Animation>().PlayQueued(attackAnimation.name, QueueMode.PlayNow);
			}else{
				animator.Play(attackAnimation.name);
			}
		}
		yield return new WaitForSeconds(attackCast);
		if(GetComponent<Status>().flinch){
			freeze = false;
			attacking = false;
			yield break;
		}
		attackPoint.transform.LookAt(followTarget);
		if(!cancelAttack){
			if(attackVoice){
				GetComponent<AudioSource>().clip = attackVoice;
				GetComponent<AudioSource>().Play();
			}
			if(attackSoundRadius > 0){
				GunSoundRadius(attackSoundRadius);
			}
			if(attackEffect){
				GameObject eff = Instantiate(attackEffect, attackPoint.transform.position , attackPoint.transform.rotation) as GameObject;
				eff.transform.parent = attackPoint.transform;
			}
			GameObject bulletShootout = Instantiate(bulletPrefab, attackPoint.transform.position , attackPoint.transform.rotation) as GameObject;
			bulletShootout.GetComponent<BulletStatus>().Setting(atk , matk , "Enemy" , this.gameObject);
		}
		
		yield return new WaitForSeconds(attackDelay);
		freeze = false;
		attacking = false;
		//CheckDistance();
		followState = AIState.Moving;
		
	}
	
	void GunSoundRadius(float radius){
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
		
		for (int i= 0; i < hitColliders.Length; i++) {
			if(hitColliders[i].tag == "Enemy"){	  
				hitColliders[i].SendMessage("SetDestination" , transform.position);
			}
		}
	}
	
	void CheckDistance(){
		if(!followTarget){
			if(!useMecanim){
				//If using Legacy Animation
				mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f);  
			}else{
				animator.SetBool("run" , false);
			}
			followState = AIState.Idle;
			return;
		}
		float distancea = (followTarget.position - transform.position).magnitude;
		if (distancea <= approachDistance){
			Vector3 destinya = followTarget.position;
			destinya.y = transform.position.y;
			transform.LookAt(destinya);
			StartCoroutine(Attack());
		}else{
			followState = AIState.Moving;
			if(!useMecanim){
				//If using Legacy Animation
				mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
			}else{
				animator.SetBool("run" , true);
			}
		}
	}
	
	GameObject FindClosest(){ 
		// Find all game objects with tag Enemy
		float distance = Mathf.Infinity;
		float findingradius = detectRange;
		GameObject closest = null; 
		
		Collider[] objectsAroundMe = Physics.OverlapSphere(transform.position , findingradius);
		foreach(Collider obj in objectsAroundMe){
			if(obj.CompareTag("Player") || obj.CompareTag("Ally")){
				Vector3 diff = (obj.transform.position - transform.position); 
				float curDistance = diff.sqrMagnitude; 
				if (curDistance < distance) { 
					//------------
					closest = obj.gameObject;
					distance = curDistance;
				} 
			}
		}
		return closest; 
	}

	void FindClosestEnemy(){ 
		// Find all game objects with tag Enemy
		float distance = Mathf.Infinity;
		float findingradius = detectRange;
		
		if(GetComponent<Status>().health < GetComponent<Status>().maxHealth){
			findingradius += lostSight + 3.0f;
		}
		
		Collider[] objectsAroundMe = Physics.OverlapSphere(transform.position , findingradius);
		foreach(Collider obj in objectsAroundMe){
			if(obj.CompareTag("Player") || obj.CompareTag("Ally")){
				Vector3 diff = (obj.transform.position - transform.position); 
				float curDistance = diff.sqrMagnitude; 
				if (curDistance < distance) { 
					//------------
					followTarget = obj.transform;
					distance = curDistance;
				} 
			}
		}
		
	}
	
	
	public IEnumerator UseSkill(Transform skill , float castTime , float delay , string anim , float dist){
		cancelAttack = false;
		if(flinch || !followTarget || (followTarget.position - transform.position).magnitude >= dist || GetComponent<Status>().silence || GetComponent<Status>().freeze  || castSkill){
			yield break;
		}
		
		freeze = true;
		castSkill = true;
		if(!useMecanim){
			//If using Legacy Animation
			mainModel.GetComponent<Animation>()[anim].layer = 10;
			mainModel.GetComponent<Animation>().Play(anim);
		}else{
			animator.Play(anim);
		}
		yield return new WaitForSeconds(castTime);
		if(flinch){
			freeze = false;
			castSkill = false;
			yield break;
		}
		attackPoint.transform.LookAt(followTarget);
		if(!cancelAttack){
			Transform bulletShootout = Instantiate(skill, attackPoint.transform.position , attackPoint.transform.rotation) as Transform;
			bulletShootout.GetComponent<BulletStatus>().Setting(atk , matk , "Enemy" , this.gameObject);
		}
		
		yield return new WaitForSeconds(delay);
		freeze = false;
		castSkill = false;
		if(!useMecanim){
			//If using Legacy Animation
			mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
		}else{
			animator.SetBool("run" , true);
		}
		
	}
	
	void SetLevel(int lv){
		Status stat = GetComponent<Status>();
		stat.level = lv;
		
		AutoCalculateStatus autoLv = GetComponent<AutoCalculateStatus>();
		if(autoLv){
			autoLv.CalculateStatLv();
		}
		detectRange = 500;
	}
	
	void PathFinding(Vector3 target){
		//Require Nav Mesh Agent.
		UnityEngine.AI.NavMeshAgent agent;
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		agent.SetDestination (target);
	}
}

public enum AIState{
	Moving = 0,
	Pausing = 1,
	Idle = 2,
	MoveToPoint = 3
}
